using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Presenters;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Templates;
using global::Avalonia.Interactivity;
using global::Avalonia.LogicalTree;
using global::Avalonia.Layout;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>A sequential form with virtualized headers and validation before every forward transition.</summary>
public class VerificationStep : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<StepItem>?> ItemsSourceProperty = AvaloniaProperty.Register<VerificationStep, IEnumerable<StepItem>?>(nameof(ItemsSource));
    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty = AvaloniaProperty.Register<VerificationStep, IDataTemplate?>(nameof(ContentTemplate));
    public static readonly StyledProperty<double> HeaderMaxWidthProperty = AvaloniaProperty.Register<VerificationStep, double>(nameof(HeaderMaxWidth), 960, validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<HorizontalAlignment> HeaderHorizontalAlignmentProperty = AvaloniaProperty.Register<VerificationStep, HorizontalAlignment>(nameof(HeaderHorizontalAlignment), HorizontalAlignment.Center, validate: Enum.IsDefined);
    public static readonly StyledProperty<string> NextTextProperty = AvaloniaProperty.Register<VerificationStep, string>(nameof(NextText), "下一步");
    public static readonly StyledProperty<string> CompleteTextProperty = AvaloniaProperty.Register<VerificationStep, string>(nameof(CompleteText), "完成");
    public static readonly StyledProperty<string> PreviousTextProperty = AvaloniaProperty.Register<VerificationStep, string>(nameof(PreviousText), "上一步");
    public static readonly DirectProperty<VerificationStep, int> SelectedIndexProperty = AvaloniaProperty.RegisterDirect<VerificationStep, int>(nameof(SelectedIndex), control => control.SelectedIndex);
    public static readonly DirectProperty<VerificationStep, IDataTemplate?> CurrentContentTemplateProperty = AvaloniaProperty.RegisterDirect<VerificationStep, IDataTemplate?>(nameof(CurrentContentTemplate), control => control.CurrentContentTemplate);
    public static readonly DirectProperty<VerificationStep, object?> CurrentContentProperty = AvaloniaProperty.RegisterDirect<VerificationStep, object?>(nameof(CurrentContent), control => control.CurrentContent);
    public static readonly DirectProperty<VerificationStep, bool> IsBusyProperty = AvaloniaProperty.RegisterDirect<VerificationStep, bool>(nameof(IsBusy), control => control.IsBusy);
    public static readonly DirectProperty<VerificationStep, bool> IsCompletedProperty = AvaloniaProperty.RegisterDirect<VerificationStep, bool>(nameof(IsCompleted), control => control.IsCompleted);
    public static readonly DirectProperty<VerificationStep, bool> CanGoBackProperty = AvaloniaProperty.RegisterDirect<VerificationStep, bool>(nameof(CanGoBack), control => control.CanGoBack);
    public static readonly DirectProperty<VerificationStep, bool> CanGoNextProperty = AvaloniaProperty.RegisterDirect<VerificationStep, bool>(nameof(CanGoNext), control => control.CanGoNext);
    public static readonly DirectProperty<VerificationStep, string> ForwardTextProperty = AvaloniaProperty.RegisterDirect<VerificationStep, string>(nameof(ForwardText), control => control.ForwardText);
    public static readonly DirectProperty<VerificationStep, IReadOnlyList<StepValidationError>> ValidationErrorsProperty = AvaloniaProperty.RegisterDirect<VerificationStep, IReadOnlyList<StepValidationError>>(nameof(ValidationErrors), control => control.ValidationErrors);
    public static readonly DirectProperty<VerificationStep, bool> HasErrorsProperty = AvaloniaProperty.RegisterDirect<VerificationStep, bool>(nameof(HasErrors), control => control.HasErrors);
    public static readonly StyledProperty<ICommand?> CompletedCommandProperty = AvaloniaProperty.Register<VerificationStep, ICommand?>(nameof(CompletedCommand));
    public static readonly StyledProperty<object?> CompletedCommandParameterProperty = AvaloniaProperty.Register<VerificationStep, object?>(nameof(CompletedCommandParameter));
    public static readonly DirectProperty<VerificationStep, IReadOnlyList<StepValidationError>> UnmappedErrorsProperty = AvaloniaProperty.RegisterDirect<VerificationStep, IReadOnlyList<StepValidationError>>(nameof(UnmappedErrors), control => control.UnmappedErrors);
    public static readonly DirectProperty<VerificationStep, bool> HasUnmappedErrorsProperty = AvaloniaProperty.RegisterDirect<VerificationStep, bool>(nameof(HasUnmappedErrors), control => control.HasUnmappedErrors);
    public static readonly RoutedEvent<RoutedEventArgs> ValidationFailedEvent = RoutedEvent.Register<VerificationStep, RoutedEventArgs>(nameof(ValidationFailed), RoutingStrategies.Bubble);
    private readonly List<StepItem> _items = [];
    private readonly HashSet<StepItem> _validated = [];
    private readonly List<(INotifyPropertyChanged Source, PropertyChangedEventHandler Handler)> _subscriptions = [];
    private INotifyCollectionChanged? _collection;
    private ListBox? _headers;
    private Button? _previous, _next;
    private ContentPresenter? _contentPresenter;
    private int _index = -1, _generation;
    private bool _busy, _completed, _canBack, _canNext, _hasErrors, _syncing;
    private object? _content;
    private IDataTemplate? _currentContentTemplate;
    private IReadOnlyList<StepFormProperty>? _formProperties;
    private string _forwardText = "下一步";
    private IReadOnlyList<StepValidationError> _errors = [];
    private CancellationTokenSource? _validation;
    private IReadOnlyList<StepValidationError> _unmappedErrors = [];
    private bool _hasUnmappedErrors;
    private ICommand? _observedCommand;

    public IEnumerable<StepItem>? ItemsSource { get => GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public IDataTemplate? ContentTemplate { get => GetValue(ContentTemplateProperty); set => SetValue(ContentTemplateProperty, value); }
    public double HeaderMaxWidth { get => GetValue(HeaderMaxWidthProperty); set => SetValue(HeaderMaxWidthProperty, value); }
    public HorizontalAlignment HeaderHorizontalAlignment { get => GetValue(HeaderHorizontalAlignmentProperty); set => SetValue(HeaderHorizontalAlignmentProperty, value); }
    public string NextText { get => GetValue(NextTextProperty); set => SetValue(NextTextProperty, value); }
    public string CompleteText { get => GetValue(CompleteTextProperty); set => SetValue(CompleteTextProperty, value); }
    public string PreviousText { get => GetValue(PreviousTextProperty); set => SetValue(PreviousTextProperty, value); }
    public int SelectedIndex => _index;
    public object? CurrentContent => _content;
    public IDataTemplate? CurrentContentTemplate => _currentContentTemplate;
    public bool IsBusy => _busy;
    public bool IsCompleted => _completed;
    public bool CanGoBack => _canBack;
    public bool CanGoNext => _canNext;
    public string ForwardText => _forwardText;
    public IReadOnlyList<StepValidationError> ValidationErrors => _errors;
    public bool HasErrors => _hasErrors;
    public ICommand? CompletedCommand { get => GetValue(CompletedCommandProperty); set => SetValue(CompletedCommandProperty, value); }
    public object? CompletedCommandParameter { get => GetValue(CompletedCommandParameterProperty); set => SetValue(CompletedCommandParameterProperty, value); }
    public IReadOnlyList<StepValidationError> UnmappedErrors => _unmappedErrors;
    public bool HasUnmappedErrors => _hasUnmappedErrors;
    public event EventHandler<RoutedEventArgs>? ValidationFailed { add => AddHandler(ValidationFailedEvent, value); remove => RemoveHandler(ValidationFailedEvent, value); }
    protected override Type StyleKeyOverride => typeof(VerificationStep);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        if (_headers is not null)
        {
            _headers.SelectionChanged -= HeaderSelected;
            _headers.ContainerPrepared -= HeaderContainerPrepared;
            _headers.SizeChanged -= HeaderSizeChanged;
        }
        if (_previous is not null) _previous.Click -= PreviousClicked;
        if (_next is not null) _next.Click -= NextClicked;
        base.OnApplyTemplate(args);
        _headers = args.NameScope.Find<ListBox>("PART_Steps");
        _previous = args.NameScope.Find<Button>("PART_PreviousButton");
        _next = args.NameScope.Find<Button>("PART_NextButton");
        _contentPresenter = args.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
        if (_headers is not null)
        {
            _headers.SelectionChanged += HeaderSelected;
            _headers.ContainerPrepared += HeaderContainerPrepared;
            _headers.SizeChanged += HeaderSizeChanged;
        }
        if (_previous is not null) _previous.Click += PreviousClicked;
        if (_next is not null) _next.Click += NextClicked;
        Select(_index);
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ItemsSourceProperty) Rebuild();
        else if (change.Property == HeaderHorizontalAlignmentProperty) UpdateHeaderWidths();
        else if (change.Property == ContentTemplateProperty) { SetErrors([]); Select(_index); }
        else if (change.Property == CompletedCommandProperty)
        {
            ObserveCommand(this.IsAttachedToVisualTree() ? CompletedCommand : null);
            Synchronize();
        }
        else if (change.Property == NextTextProperty || change.Property == CompleteTextProperty || change.Property == CompletedCommandParameterProperty) Synchronize();
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args); Subscribe();
        ObserveCommand(CompletedCommand);
        Select(_index);
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        ObserveCommand(null);
        CancelValidation(); SetErrors([]); Unsubscribe(); base.OnDetachedFromVisualTree(args);
    }
    private void ObserveCommand(ICommand? command)
    {
        if (ReferenceEquals(_observedCommand, command)) return;
        if (_observedCommand is { } previous) previous.CanExecuteChanged -= CommandCanExecuteChanged;
        _observedCommand = command;
        if (command is not null) command.CanExecuteChanged += CommandCanExecuteChanged;
    }
    private void CommandCanExecuteChanged(object? sender, EventArgs args)
    {
        if (Dispatcher.UIThread.CheckAccess()) Synchronize();
        else Dispatcher.UIThread.Post(Synchronize);
    }
    private bool CanComplete() => _index != _items.Count - 1 || CompletedCommand?.CanExecute(CompletedCommandParameter) != false;
    private void Subscribe()
    {
        Unsubscribe();
        _collection = ItemsSource as INotifyCollectionChanged;
        if (_collection is not null) _collection.CollectionChanged += CollectionChanged;
        foreach (var item in _items)
        {
            Watch(item, (_, args) =>
            {
                if (args.PropertyName is null or "" or nameof(StepItem.Content) or nameof(StepItem.Validator)) Rebuild();
            });
            var validationModel = item.EffectiveValidator is IStepValidationScope registered ? registered.Model : item.Content is Control contentControl ? contentControl.DataContext : item.Content;
            if (validationModel is INotifyPropertyChanged model) Watch(model, (_, args) =>
            {
                if (item.EffectiveValidator is not IStepValidationScope scope || scope.IsAffectedBy(args.PropertyName))
                { item.ModelChanged(args.PropertyName); Invalidate(item); }
            });
        }
        void Watch(INotifyPropertyChanged source, PropertyChangedEventHandler handler)
        { source.PropertyChanged += handler; _subscriptions.Add((source, handler)); }
    }
    private void Unsubscribe()
    {
        if (_collection is not null) _collection.CollectionChanged -= CollectionChanged;
        _collection = null;
        foreach (var (source, handler) in _subscriptions) source.PropertyChanged -= handler;
        _subscriptions.Clear();
    }
    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs args) => Rebuild();
    private void Rebuild()
    {
        Dispatcher.UIThread.VerifyAccess(); CancelValidation(); Unsubscribe();
        _items.Clear();
        if (ItemsSource is { } source) _items.AddRange(source);
        if (_items.Any(item => item is null) || _items.Distinct().Count() != _items.Count)
            throw new ArgumentException("VerificationStep items must be non-null and have distinct identities.", nameof(ItemsSource));
        _validated.Clear(); SetAndRaise(IsCompletedProperty, ref _completed, false);
        SetErrors([]); Select(_items.Count == 0 ? -1 : 0);
        if (this.IsAttachedToVisualTree()) Subscribe();
    }
    private void Invalidate(StepItem item)
    {
        Dispatcher.UIThread.VerifyAccess();
        var from = _items.IndexOf(item);
        if (from < 0) return;
        // A changed earlier step must be revisited before any dependent step can finish.
        CancelValidation();
        for (var index = from; index < _items.Count; index++) _validated.Remove(_items[index]);
        SetAndRaise(IsCompletedProperty, ref _completed, false); SetErrors([]);
        Select(_index > from ? from : _index);
    }
    public void Reset()
    {
        Dispatcher.UIThread.VerifyAccess(); Rebuild();
    }
    public bool GoBack()
    {
        Dispatcher.UIThread.VerifyAccess();
        if (_index <= 0) return false;
        CancelValidation(); SetAndRaise(IsCompletedProperty, ref _completed, false); SetErrors([]); Select(_index - 1); return true;
    }
    /// <summary>Backward navigation is free; forward navigation is limited to the immediate next step and validates the current one.</summary>
    public Task<bool> GoToAsync(int index, CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (index < 0 || index >= _items.Count) return Task.FromResult(false);
        if (index == _index) return Task.FromResult(true);
        if (index == _index + 1) return GoNextAsync(cancellationToken);
        if (index > _index) return Task.FromResult(false);
        CancelValidation(); SetAndRaise(IsCompletedProperty, ref _completed, false); SetErrors([]); Select(index); return Task.FromResult(true);
    }
    /// <summary>Validates current bound model attributes and native input errors; returns false on invalid, busy or cancelled transitions.</summary>
    public async Task<bool> GoNextAsync(CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (!this.IsAttachedToVisualTree() || !IsEffectivelyEnabled || _index < 0 || _busy) return false;
        if (_completed) return true;
        if (cancellationToken.IsCancellationRequested || !CanComplete()) return false;
        var item = _items[_index]; var generation = _generation;
        using var lifetime = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _validation = lifetime; SetBusy(true); SetErrors([]);
        try
        {
            IReadOnlyList<StepValidationError> errors;
            try { errors = item.EffectiveValidator is { } validator ? await validator.ValidateAsync(lifetime.Token).WaitAsync(lifetime.Token) : []; }
            catch (OperationCanceledException) when (lifetime.IsCancellationRequested) { return false; }
            catch (Exception error) { errors = [new("", error.GetBaseException().Message)]; }
            if (lifetime.IsCancellationRequested || generation != _generation || !this.IsAttachedToVisualTree() || !IsEffectivelyEnabled) return false;
            var allErrors = errors.Concat(_formProperties?.SelectMany(field => field.DraftErrors) ?? []).Distinct().ToList();
            foreach (var error in StepValidationVisuals.NativeErrors(Inputs()))
                if (!allErrors.Contains(error)) allErrors.Add(error);
            SetErrors(allErrors.ToArray());
            if (allErrors.Count > 0)
            {
                Synchronize();
                RaiseEvent(new RoutedEventArgs(ValidationFailedEvent)); return false;
            }
            // Command availability may have changed during asynchronous validation.
            if (!CanComplete()) return false;
            _validated.Add(item);
            if (_index + 1 < _items.Count) Select(_index + 1);
            else
            {
                SetAndRaise(IsCompletedProperty, ref _completed, true); Synchronize();
                CompletedCommand?.Execute(CompletedCommandParameter);
            }
            return true;
        }
        finally
        {
            if (ReferenceEquals(_validation, lifetime))
            {
                _validation = null; SetBusy(false);
                if (_hasErrors) FocusError();
            }
        }
    }
    private void FocusError()
    {
        var inputs = Inputs().Where(control => control.Focusable && control.IsEffectivelyEnabled && control.IsEffectivelyVisible).ToArray();
        var invalid = inputs.FirstOrDefault(DataValidationErrors.GetHasErrors)
            ?? inputs.FirstOrDefault(control => _errors.Any(error => error.MemberName.Length > 0 && error.MemberName == MemberOf(control)));
        if (invalid is not null) { invalid.Focus(); return; }
        if (_formProperties is null) return;
        var field = _formProperties.FirstOrDefault(property => property.Errors.Count > 0);
        var form = Inputs().OfType<ListBox>().FirstOrDefault(control => control.Name == "PART_FormFields");
        if (field is null || form is null) return;
        form.ScrollIntoView(field);
        var generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (generation != _generation || !_hasErrors || !this.IsAttachedToVisualTree()) return;
            Inputs().FirstOrDefault(control => control.Focusable && control.IsEffectivelyEnabled && MemberOf(control) == field.Property.Name)?.Focus();
        }, DispatcherPriority.Loaded);
    }
    private static string MemberOf(Control control) => StepValidationVisuals.MemberOf(control);
    internal void RefreshFieldErrors() => SetErrors(_errors);
    private IEnumerable<Control> Inputs() => _contentPresenter?.GetVisualDescendants().OfType<Control>()
        .Where(control => ReferenceEquals(StepValidationVisuals.Owner(control), this)) ?? [];
    private void CancelValidation()
    {
        _generation++; var validation = _validation; _validation = null;
        validation?.Cancel(); SetBusy(false);
    }
    private void SetBusy(bool value)
    { SetAndRaise(IsBusyProperty, ref _busy, value); PseudoClasses.Set(":busy", value); Synchronize(); }
    private void SetErrors(IReadOnlyList<StepValidationError> errors)
    {
        var fields = Inputs().OfType<StepField>().ToArray();
        if (_formProperties is { } properties)
            foreach (var property in properties) property.Errors = errors.Where(error => error.MemberName == property.Property.Name).ToArray();
        foreach (var field in fields) field.SetErrors(errors.Where(error => error.MemberName.Length > 0 && error.MemberName == field.MemberName).ToArray());
        var unmapped = errors.Where(error => error.MemberName.Length == 0 || !fields.Any(field => field.MemberName == error.MemberName) && !(_formProperties?.Any(field => field.Property.Name == error.MemberName) ?? false)).ToArray();
        SetAndRaise(UnmappedErrorsProperty, ref _unmappedErrors, unmapped);
        SetAndRaise(HasUnmappedErrorsProperty, ref _hasUnmappedErrors, unmapped.Length > 0);
        SetAndRaise(ValidationErrorsProperty, ref _errors, errors);
        SetAndRaise(HasErrorsProperty, ref _hasErrors, errors.Count > 0); PseudoClasses.Set(":error", _hasErrors);
    }
    private void Select(int index)
    {
        SetAndRaise(SelectedIndexProperty, ref _index, index);
        var content = index < 0 ? null : _items[index].Content;
        var template = ContentTemplate ?? (content is not null ? this.FindDataTemplate(content) : null);
        var form = template is null && content is not Control && index >= 0 ? _items[index].FormProperties : null;
        if (!ReferenceEquals(_formProperties, form))
        {
            _formProperties = form;
            if (form is not null) template = new FuncDataTemplate<object>((_, _) => StepForm.Build(this, form));
        }
        else if (form is not null) template = _currentContentTemplate;
        SetAndRaise(CurrentContentTemplateProperty, ref _currentContentTemplate, template);
        if (!ReferenceEquals(_content, content))
        {
            if (_content is ILogical oldChild) LogicalChildren.Remove(oldChild);
            if (content is ILogical newChild && !LogicalChildren.Contains(newChild)) LogicalChildren.Add(newChild);
            SetAndRaise(CurrentContentProperty, ref _content, content);
        }
        Synchronize();
    }
    private void Synchronize()
    {
        if (_syncing) return;
        _syncing = true;
        try
        {
            SetAndRaise(CanGoBackProperty, ref _canBack, _index > 0);
            SetAndRaise(CanGoNextProperty, ref _canNext, _index >= 0 && !_busy && !_completed && CanComplete());
            SetAndRaise(ForwardTextProperty, ref _forwardText, _index == _items.Count - 1 && _index >= 0 ? CompleteText : NextText);
            PseudoClasses.Set(":completed", _completed);
            for (var index = 0; index < _items.Count; index++)
            {
                var item = _items[index]; item.Number = index + 1; item.HasPrevious = index > 0; item.HasNext = index + 1 < _items.Count;
                item.State = index == _index && !_completed ? _hasErrors ? StepState.Error : StepState.Current : _validated.Contains(item) ? StepState.Success : StepState.Waiting;
            }
            if (_headers is not null)
            {
                if (!ReferenceEquals(_headers.ItemsSource, ItemsSource)) _headers.ItemsSource = ItemsSource;
                UpdateHeaderWidths();
                _headers.SelectedIndex = _index;
                if (_index >= 0) _headers.ScrollIntoView(_index);
            }
        }
        finally { _syncing = false; }
    }
    private async void HeaderSelected(object? sender, SelectionChangedEventArgs args)
    {
        if (_syncing || _headers is null) return;
        var target = _headers.SelectedIndex; Synchronize(); await GoToAsync(target);
    }
    private double HeaderItemWidth(int index)
    {
        if (HeaderHorizontalAlignment != HorizontalAlignment.Stretch || _items.Count == 0) return 200;
        var available = Math.Min(_headers?.Bounds.Width ?? HeaderMaxWidth, HeaderMaxWidth);
        if (available / _items.Count <= 120) return 120;
        // Share rounded pixel boundaries so item rounding cannot introduce a spurious scrollbar.
        var scale = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1;
        var pixels = Math.Floor(available * scale + .001);
        return (Math.Round((index + 1d) * pixels / _items.Count) - Math.Round(index * pixels / _items.Count)) / scale;
    }
    private void HeaderContainerPrepared(object? sender, ContainerPreparedEventArgs args) => args.Container.SetCurrentValue(WidthProperty, HeaderItemWidth(args.Index));
    private void HeaderSizeChanged(object? sender, SizeChangedEventArgs args) => UpdateHeaderWidths();
    private void UpdateHeaderWidths()
    {
        if (_headers is null) return;
        foreach (var container in _headers.GetRealizedContainers())
        {
            var width = HeaderItemWidth(_headers.IndexFromContainer(container));
            if (Math.Abs(container.Width - width) > .01 || double.IsNaN(container.Width)) container.SetCurrentValue(WidthProperty, width);
        }
    }
    private void PreviousClicked(object? sender, RoutedEventArgs args) => GoBack();
    private async void NextClicked(object? sender, RoutedEventArgs args) => await GoNextAsync();
}
