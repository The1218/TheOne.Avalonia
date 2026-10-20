using System.Windows.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace TheOne.Avalonia.Controls;

/// <summary>A breadcrumb container with a separate, noninteractive separator and native navigation button.</summary>
[PseudoClasses(":current")]
public class BreadcrumbItem : ContentControl
{
    public static readonly StyledProperty<OneIconKind?> IconProperty = AvaloniaProperty.Register<BreadcrumbItem, OneIconKind?>(nameof(Icon),
        validate: value => value is null || Enum.IsDefined(value.Value));
    public static readonly StyledProperty<double> IconSizeProperty = AvaloniaProperty.Register<BreadcrumbItem, double>(nameof(IconSize), 16,
        validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<object?> SeparatorProperty = AvaloniaProperty.Register<BreadcrumbItem, object?>(nameof(Separator), "/");
    public static readonly StyledProperty<IDataTemplate?> SeparatorTemplateProperty = AvaloniaProperty.Register<BreadcrumbItem, IDataTemplate?>(nameof(SeparatorTemplate));
    public static readonly StyledProperty<ICommand?> NavigationCommandProperty = AvaloniaProperty.Register<BreadcrumbItem, ICommand?>(nameof(NavigationCommand));
    public static readonly StyledProperty<object?> NavigationParameterProperty = AvaloniaProperty.Register<BreadcrumbItem, object?>(nameof(NavigationParameter));
    public static readonly DirectProperty<BreadcrumbItem, bool> IsCurrentProperty = AvaloniaProperty.RegisterDirect<BreadcrumbItem, bool>(nameof(IsCurrent), item => item.IsCurrent);
    public static readonly DirectProperty<BreadcrumbItem, bool> ShowSeparatorProperty = AvaloniaProperty.RegisterDirect<BreadcrumbItem, bool>(nameof(ShowSeparator), item => item.ShowSeparator);
    public static readonly DirectProperty<BreadcrumbItem, bool> CanNavigateProperty = AvaloniaProperty.RegisterDirect<BreadcrumbItem, bool>(nameof(CanNavigate), item => item.CanNavigate);

    private bool _isCurrent;
    private bool _showSeparator;
    private bool _canNavigate;
    private Button? _button;
    private Breadcrumb? _owner;
    private object? _item;
    private int _pathIndex;
    private readonly List<IDisposable> _partBindings = [];

    public OneIconKind? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public double IconSize { get => GetValue(IconSizeProperty); set => SetValue(IconSizeProperty, value); }
    public object? Separator { get => GetValue(SeparatorProperty); internal set => SetValue(SeparatorProperty, value); }
    public IDataTemplate? SeparatorTemplate { get => GetValue(SeparatorTemplateProperty); internal set => SetValue(SeparatorTemplateProperty, value); }
    public ICommand? NavigationCommand { get => GetValue(NavigationCommandProperty); internal set => SetValue(NavigationCommandProperty, value); }
    public object? NavigationParameter { get => GetValue(NavigationParameterProperty); internal set => SetValue(NavigationParameterProperty, value); }
    public bool IsCurrent => _isCurrent;
    public bool ShowSeparator => _showSeparator;
    public bool CanNavigate => _canNavigate;

    internal void Configure(Breadcrumb? owner, int index)
    {
        // Collection notifications may precede native presenter removal/index updates.
        // Do not give an old visual the new item at its temporarily stale index.
        if (owner is not null && (index < 0 || index >= owner.ItemsView.Count || !MatchesItem(owner.ItemsView[index]))) owner = null;
        if (owner is not null) _pathIndex = index;
        _owner = owner;
        var current = owner is not null && index >= 0 && index == owner.ItemCount - 1;
        SetAndRaise(IsCurrentProperty, ref _isCurrent, current);
        SetAndRaise(ShowSeparatorProperty, ref _showSeparator, owner is not null && index > 0 && owner.Separator is not null);
        SetAndRaise(CanNavigateProperty, ref _canNavigate, owner is not null && !current);
        PseudoClasses.Set(":current", current);
        Separator = owner?.Separator;
        SeparatorTemplate = owner?.SeparatorTemplate;
        NavigationParameter = owner is not null ? _item : null;
        NavigationCommand = current ? null : owner?.ItemInvokedCommand;
    }

    internal bool FocusNavigation() => CanNavigate && _button is { IsEffectivelyEnabled: true } && _button.Focus(NavigationMethod.Directional);
    internal bool HasNavigationFocus => _button?.IsFocused == true;
    internal bool IsNavigationAvailable => CanNavigate && _button?.IsEffectivelyEnabled == true;
    internal int PathIndex => _pathIndex;
    internal void PrepareItem(object? item) => _item = item;
    internal bool MatchesItem(object? item) => ReferenceEquals(_item, item) || _item is ValueType && item is ValueType && Equals(_item, item);
    internal bool IsNavigationSource(object? source) => ReferenceEquals(source, _button);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_button is not null)
        {
            _button.Click -= OnNavigationClick;
            _button.Click += CancelRetiredNavigationClick;
        }
        foreach (var binding in _partBindings) binding.Dispose();
        _partBindings.Clear();
        base.OnApplyTemplate(e);
        _button = e.NameScope.Find<Button>("PART_Navigate");
        if (_button is not null)
        {
            _button.Click -= CancelRetiredNavigationClick;
            _button.Click += OnNavigationClick;
            // Native button density classes otherwise outrank template bindings to the compact row.
            _partBindings.Add(_button.Bind(MinHeightProperty, CompiledBinding.Create<BreadcrumbItem, double>(item => item.MinHeight, this)));
            _partBindings.Add(_button.Bind(PaddingProperty, CompiledBinding.Create<BreadcrumbItem, Thickness>(item => item.Padding, this)));
            _partBindings.Add(_button.Bind(FontSizeProperty, CompiledBinding.Create<BreadcrumbItem, double>(item => item.FontSize, this)));
        }
        UpdateAccessibleName();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ContentProperty || change.Property == AutomationProperties.NameProperty) UpdateAccessibleName();
    }

    private void UpdateAccessibleName()
    {
        if (_button is null) return;
        var name = AutomationProperties.GetName(this) ?? Content as string ?? (Content as BreadcrumbItemModel)?.Header as string;
        _button.SetCurrentValue(AutomationProperties.NameProperty, name);
    }

    private void OnNavigationClick(object? sender, RoutedEventArgs e)
    {
        if (e.Handled || !ReferenceEquals(e.Source, _button)) return;
        // The routed event may replace/recycle this button. Execute the invocation snapshot once,
        // rather than letting Button read a different Command/CommandParameter afterwards.
        e.Handled = true;
        if (CanNavigate && _owner is { } owner) owner.InvokeItem(this);
    }

    private static void CancelRetiredNavigationClick(object? sender, RoutedEventArgs e) => e.Handled = true;
}
