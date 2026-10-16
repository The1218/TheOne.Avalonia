using System.ComponentModel;
using System.Windows.Input;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Presenters;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Interactivity;
using global::Avalonia.Layout;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>Validates caller-owned, compiled-bound fields using the same validators as VerificationStep.</summary>
public class Form : ContentControl
{
    public static readonly StyledProperty<IStepValidator?> ValidatorProperty = AvaloniaProperty.Register<Form, IStepValidator?>(nameof(Validator));
    public static readonly StyledProperty<object?> ValidationModelProperty = AvaloniaProperty.Register<Form, object?>(nameof(ValidationModel));
    public static readonly StyledProperty<ICommand?> SubmitCommandProperty = AvaloniaProperty.Register<Form, ICommand?>(nameof(SubmitCommand));
    public static readonly StyledProperty<object?> SubmitCommandParameterProperty = AvaloniaProperty.Register<Form, object?>(nameof(SubmitCommandParameter));
    public static readonly StyledProperty<bool> ShowErrorSummaryProperty = AvaloniaProperty.Register<Form, bool>(nameof(ShowErrorSummary), true);
    public static readonly StyledProperty<Orientation> LabelOrientationProperty = FormItem.LabelOrientationProperty.AddOwner<Form>();
    public static readonly StyledProperty<double> LabelWidthProperty = FormItem.LabelWidthProperty.AddOwner<Form>();
    public static readonly DirectProperty<Form, IReadOnlyList<StepValidationError>> ValidationErrorsProperty = AvaloniaProperty.RegisterDirect<Form, IReadOnlyList<StepValidationError>>(nameof(ValidationErrors), form => form.ValidationErrors);
    public static readonly DirectProperty<Form, bool> HasErrorsProperty = AvaloniaProperty.RegisterDirect<Form, bool>(nameof(HasErrors), form => form.HasErrors);
    public static readonly DirectProperty<Form, IReadOnlyList<StepValidationError>> UnmappedErrorsProperty = AvaloniaProperty.RegisterDirect<Form, IReadOnlyList<StepValidationError>>(nameof(UnmappedErrors), form => form.UnmappedErrors);
    public static readonly DirectProperty<Form, bool> HasUnmappedErrorsProperty = AvaloniaProperty.RegisterDirect<Form, bool>(nameof(HasUnmappedErrors), form => form.HasUnmappedErrors);
    public static readonly DirectProperty<Form, bool> IsBusyProperty = AvaloniaProperty.RegisterDirect<Form, bool>(nameof(IsBusy), form => form.IsBusy);
    public static readonly DirectProperty<Form, bool> IsValidatedProperty = AvaloniaProperty.RegisterDirect<Form, bool>(nameof(IsValidated), form => form.IsValidated);
    public static readonly DirectProperty<Form, bool> CanSubmitProperty = AvaloniaProperty.RegisterDirect<Form, bool>(nameof(CanSubmit), form => form.CanSubmit);
    public static readonly RoutedEvent<RoutedEventArgs> ValidationFailedEvent = RoutedEvent.Register<Form, RoutedEventArgs>(nameof(ValidationFailed), RoutingStrategies.Bubble);
    private IReadOnlyList<StepValidationError> _errors = [];
    private IReadOnlyList<StepValidationError> _unmappedErrors = [];
    private bool _hasUnmappedErrors;
    private bool _hasErrors, _busy, _validated, _canSubmit, _submitting, _attached;
    private int _generation;
    private CancellationTokenSource? _validation;
    private INotifyPropertyChanged? _model;
    private ICommand? _command;
    private ContentPresenter? _presenter;

    public IStepValidator? Validator { get => GetValue(ValidatorProperty); set => SetValue(ValidatorProperty, value); }
    /// <summary>Optional notification source for custom validators. Attribute validators supply their registered model automatically.</summary>
    public object? ValidationModel { get => GetValue(ValidationModelProperty); set => SetValue(ValidationModelProperty, value); }
    public ICommand? SubmitCommand { get => GetValue(SubmitCommandProperty); set => SetValue(SubmitCommandProperty, value); }
    public object? SubmitCommandParameter { get => GetValue(SubmitCommandParameterProperty); set => SetValue(SubmitCommandParameterProperty, value); }
    public bool ShowErrorSummary { get => GetValue(ShowErrorSummaryProperty); set => SetValue(ShowErrorSummaryProperty, value); }
    public Orientation LabelOrientation { get => GetValue(LabelOrientationProperty); set => SetValue(LabelOrientationProperty, value); }
    public double LabelWidth { get => GetValue(LabelWidthProperty); set => SetValue(LabelWidthProperty, value); }
    public IReadOnlyList<StepValidationError> ValidationErrors => _errors;
    public bool HasErrors => _hasErrors;
    public IReadOnlyList<StepValidationError> UnmappedErrors => _unmappedErrors;
    public bool HasUnmappedErrors => _hasUnmappedErrors;
    public bool IsBusy => _busy;
    public bool IsValidated => _validated;
    public bool CanSubmit => _canSubmit;
    public event EventHandler<RoutedEventArgs>? ValidationFailed { add => AddHandler(ValidationFailedEvent, value); remove => RemoveHandler(ValidationFailedEvent, value); }
    protected override Type StyleKeyOverride => typeof(Form);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        base.OnApplyTemplate(args);
        _presenter = args.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
        RefreshFieldErrors();
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args);
        _attached = true;
        Observe(); RefreshFieldErrors(); Synchronize();
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        _attached = false; Unobserve(); ClearValidation(); base.OnDetachedFromVisualTree(args); Synchronize();
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValidatorProperty || change.Property == ValidationModelProperty || change.Property == ContentProperty || change.Property == ContentTemplateProperty)
        {
            ClearValidation();
            if (this.IsAttachedToVisualTree()) Observe();
        }
        else if (change.Property == SubmitCommandProperty)
        {
            if (this.IsAttachedToVisualTree()) Observe();
            Synchronize();
        }
        else if (change.Property == IsEffectivelyEnabledProperty && !IsEffectivelyEnabled) CancelValidation();
        else if (change.Property == SubmitCommandParameterProperty || change.Property == IsEnabledProperty || change.Property == IsEffectivelyEnabledProperty) Synchronize();
    }
    private void Observe()
    {
        Unobserve();
        _model = (Validator is IStepValidationScope scope ? scope.Model : ValidationModel) as INotifyPropertyChanged;
        if (_model is not null) _model.PropertyChanged += ModelChanged;
        _command = SubmitCommand;
        if (_command is not null) _command.CanExecuteChanged += CommandChanged;
    }
    private void Unobserve()
    {
        if (_model is not null) _model.PropertyChanged -= ModelChanged;
        if (_command is not null) _command.CanExecuteChanged -= CommandChanged;
        _model = null; _command = null;
    }
    private void ModelChanged(object? sender, PropertyChangedEventArgs args)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (Validator is not IStepValidationScope scope || scope.IsAffectedBy(args.PropertyName)) ClearValidation();
    }
    private void CommandChanged(object? sender, EventArgs args)
    {
        if (Dispatcher.UIThread.CheckAccess()) Synchronize();
        else Dispatcher.UIThread.Post(Synchronize);
    }
    private void Synchronize() => SetAndRaise(CanSubmitProperty, ref _canSubmit,
        _attached && IsEffectivelyEnabled && !_busy && !_submitting && SubmitCommand?.CanExecute(SubmitCommandParameter) != false);
    private void SetBusy(bool value)
    {
        SetAndRaise(IsBusyProperty, ref _busy, value); PseudoClasses.Set(":busy", value); Synchronize();
    }
    private IEnumerable<Control> Inputs() => _presenter?.GetVisualDescendants().OfType<Control>()
        .Where(control => ReferenceEquals(StepValidationVisuals.Owner(control), this)) ?? [];

    /// <summary>Cancels pending validation and clears form messages, retaining model values and native input drafts.</summary>
    public void ClearValidation()
    {
        Dispatcher.UIThread.VerifyAccess();
        CancelValidation(); SetAndRaise(IsValidatedProperty, ref _validated, false); SetErrors([]);
    }
    private void CancelValidation()
    {
        _generation++;
        var previous = _validation; _validation = null; previous?.Cancel();
        SetBusy(false);
    }
    internal void RefreshFieldErrors() => SetErrors(_errors);
    private void SetErrors(IReadOnlyList<StepValidationError> errors)
    {
        var fields = Inputs().OfType<StepField>().ToArray();
        foreach (var field in fields)
            field.SetErrors(errors.Where(error => error.MemberName.Length > 0 && error.MemberName == field.MemberName).ToArray());
        var unmapped = errors.Where(error => error.MemberName.Length == 0 || !fields.Any(field => field.MemberName == error.MemberName)).ToArray();
        SetAndRaise(UnmappedErrorsProperty, ref _unmappedErrors, unmapped);
        SetAndRaise(HasUnmappedErrorsProperty, ref _hasUnmappedErrors, unmapped.Length > 0);
        SetAndRaise(ValidationErrorsProperty, ref _errors, errors);
        SetAndRaise(HasErrorsProperty, ref _hasErrors, errors.Count > 0); PseudoClasses.Set(":error", _hasErrors);
    }
    /// <summary>Runs attribute/custom validation and native conversion validation. Invalid, busy, detached or cancelled calls return false.</summary>
    public async Task<bool> ValidateAsync(CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (!this.IsAttachedToVisualTree() || !IsEffectivelyEnabled || _busy || cancellationToken.IsCancellationRequested) return false;
        var generation = _generation;
        using var lifetime = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _validation = lifetime; SetAndRaise(IsValidatedProperty, ref _validated, false); SetErrors([]); SetBusy(true);
        try
        {
            IReadOnlyList<StepValidationError> errors;
            try { errors = Validator is { } validator ? await validator.ValidateAsync(lifetime.Token).WaitAsync(lifetime.Token) : []; }
            catch (OperationCanceledException) when (lifetime.IsCancellationRequested) { return false; }
            catch (Exception error) { errors = [new("", error.GetBaseException().Message)]; }
            if (lifetime.IsCancellationRequested || generation != _generation || !this.IsAttachedToVisualTree() || !IsEffectivelyEnabled) return false;
            SetErrors(errors.Concat(StepValidationVisuals.NativeErrors(Inputs())).Distinct().ToArray());
            if (_hasErrors) { RaiseEvent(new RoutedEventArgs(ValidationFailedEvent)); return false; }
            SetAndRaise(IsValidatedProperty, ref _validated, true);
            return true;
        }
        finally
        {
            if (ReferenceEquals(_validation, lifetime))
            {
                _validation = null; SetBusy(false);
                if (_hasErrors) FocusFirstError();
            }
        }
    }
    /// <summary>Validates before executing the host command, checking CanExecute again after asynchronous validation.</summary>
    public async Task<bool> SubmitAsync(CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (_submitting || _busy || SubmitCommand?.CanExecute(SubmitCommandParameter) == false) return false;
        _submitting = true; Synchronize();
        try
        {
            if (!await ValidateAsync(cancellationToken) || !_validated || SubmitCommand?.CanExecute(SubmitCommandParameter) == false) return false;
            SubmitCommand?.Execute(SubmitCommandParameter); return true;
        }
        finally { _submitting = false; Synchronize(); }
    }
    public bool FocusFirstError()
    {
        Dispatcher.UIThread.VerifyAccess();
        var inputs = Inputs().Where(control => control.Focusable && control.IsEffectivelyEnabled && control.IsEffectivelyVisible).ToArray();
        var target = inputs.FirstOrDefault(DataValidationErrors.GetHasErrors)
            ?? inputs.FirstOrDefault(control => _errors.Any(error => error.MemberName.Length > 0 && error.MemberName == StepValidationVisuals.MemberOf(control)));
        if (target is null) return false;
        target.BringIntoView(); return target.Focus();
    }
}
