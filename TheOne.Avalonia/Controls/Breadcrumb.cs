using System.Collections.Specialized;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>A caller-owned navigation path. The final item is the noninteractive current page.</summary>
public partial class Breadcrumb : ItemsControl
{
    public static readonly StyledProperty<object?> SeparatorProperty = AvaloniaProperty.Register<Breadcrumb, object?>(nameof(Separator), "/");
    public static readonly StyledProperty<IDataTemplate?> SeparatorTemplateProperty = AvaloniaProperty.Register<Breadcrumb, IDataTemplate?>(nameof(SeparatorTemplate));
    public static readonly StyledProperty<ICommand?> ItemInvokedCommandProperty = AvaloniaProperty.Register<Breadcrumb, ICommand?>(nameof(ItemInvokedCommand));
    public static readonly DirectProperty<Breadcrumb, object?> CurrentItemProperty = AvaloniaProperty.RegisterDirect<Breadcrumb, object?>(nameof(CurrentItem), control => control.CurrentItem);
    public static readonly RoutedEvent<BreadcrumbItemInvokedEventArgs> ItemInvokedEvent = RoutedEvent.Register<Breadcrumb, BreadcrumbItemInvokedEventArgs>(nameof(ItemInvoked), RoutingStrategies.Bubble);

    private readonly Dictionary<Control, List<IDisposable>> _bindings = [];
    private object? _currentItem;
    private IInputElement? _focusToRecover;
    private int _focusIndex;
    private bool _focusRecoveryQueued;
    private long _focusRecoveryVersion;
    private DispatcherOperation? _focusRecoveryOperation;
    private TopLevel? _focusWatchRoot;
    private bool _isAttached;
    private bool _isInvoking;
    private long _lifetimeVersion;
    private long _pathVersion;

    public Breadcrumb() => ItemsView.CollectionChanged += OnPathCollectionChanged;

    public object? Separator { get => GetValue(SeparatorProperty); set => SetValue(SeparatorProperty, value); }
    public IDataTemplate? SeparatorTemplate { get => GetValue(SeparatorTemplateProperty); set => SetValue(SeparatorTemplateProperty, value); }
    /// <summary>Receives the original item. Native CanExecute controls each ancestor button.</summary>
    public ICommand? ItemInvokedCommand { get => GetValue(ItemInvokedCommandProperty); set => SetValue(ItemInvokedCommandProperty, value); }
    public object? CurrentItem => _currentItem;
    public event EventHandler<BreadcrumbItemInvokedEventArgs>? ItemInvoked
    {
        add => AddHandler(ItemInvokedEvent, value);
        remove => RemoveHandler(ItemInvokedEvent, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        _lifetimeVersion++;
        AttachNavigationKeys();
        RefreshPath();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        _lifetimeVersion++;
        DetachNavigationKeys();
        CancelFocusRecovery();
        base.OnDetachedFromVisualTree(e);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) => new BreadcrumbItem();
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) => NeedsContainer<BreadcrumbItem>(item, out recycleKey);

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        ClearBindings(container);
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is not BreadcrumbItem row) return;
        row.PrepareItem(item);
        if (!ReferenceEquals(row, item))
        {
            row.SetCurrentValue(BreadcrumbItem.IconProperty, null);
            row.SetCurrentValue(IsEnabledProperty, true);
            if (item is BreadcrumbItemModel model)
            {
                var bindings = _bindings[container] = [];
                // Custom templates receive the original model; the default presentation uses Header.
                if (ItemTemplate is null)
                    bindings.Add(row.Bind(ContentControl.ContentProperty, CompiledBinding.Create<BreadcrumbItemModel, object?>(value => value.Header, model)));
                bindings.Add(row.Bind(BreadcrumbItem.IconProperty, CompiledBinding.Create<BreadcrumbItemModel, OneIconKind?>(value => value.Icon, model)));
                bindings.Add(row.Bind(IsEnabledProperty, CompiledBinding.Create<BreadcrumbItemModel, bool>(value => value.IsEnabled, model)));
            }
        }
        row.Configure(this, index);
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        RefreshPath();
    }

    protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
    {
        base.ContainerIndexChangedOverride(container, oldIndex, newIndex);
        if (container is BreadcrumbItem row) row.Configure(this, newIndex);
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        ClearBindings(container);
        if (container is BreadcrumbItem row)
        {
            RememberFocus(row);
            row.Configure(null, -1);
            row.PrepareItem(null);
        }
        base.ClearContainerForItemOverride(container);
    }

    private void OnPathCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _pathVersion++;
        CancelSpaceGesture();
        foreach (var container in GetRealizedContainers())
            if (container is BreadcrumbItem row) RememberFocus(row);
        RefreshPath();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SeparatorProperty || change.Property == SeparatorTemplateProperty || change.Property == ItemInvokedCommandProperty)
            RefreshPath();
    }

    private void ClearBindings(Control container)
    {
        if (_bindings.Remove(container, out var bindings)) foreach (var binding in bindings) binding.Dispose();
    }

    private void RefreshPath()
    {
        SetAndRaise(CurrentItemProperty, ref _currentItem, ItemCount > 0 ? ItemsView[ItemCount - 1] : null);
        foreach (var container in GetRealizedContainers())
            if (container is BreadcrumbItem row) row.Configure(this, IndexFromContainer(row));
        // Truncating a path from a keyboard command turns its focused button into the current label.
        if (_isAttached && ItemCount > 0 && ContainerFromIndex(ItemCount - 1) is BreadcrumbItem { HasNavigationFocus: true })
        {
            for (var index = ItemCount - 2; index >= 0; index--)
                if (ContainerFromIndex(index) is BreadcrumbItem row && row.FocusNavigation()) return;
            TopLevel.GetTopLevel(this)?.FocusManager?.Focus(null);
        }
    }

    internal void InvokeItem(BreadcrumbItem container)
    {
        if (!_isAttached || _isInvoking || _keyUpBlocked || TopLevel.GetTopLevel(container) != TopLevel.GetTopLevel(this)) return;
        var index = IndexFromContainer(container);
        if (index < 0 || index >= ItemCount - 1 || !container.IsEffectivelyEnabled) return;
        var item = ItemsView[index];
        if (!container.MatchesItem(item)) return;
        var command = ItemInvokedCommand;
        var lifetime = _lifetimeVersion;
        _isInvoking = true;
        try
        {
            if (command is not null && !command.CanExecute(item) || !_isAttached || lifetime != _lifetimeVersion) return;
            var args = new BreadcrumbItemInvokedEventArgs(ItemInvokedEvent, this, item, index);
            RaiseEvent(args);
            if (!args.Handled && _isAttached && lifetime == _lifetimeVersion && command?.CanExecute(item) == true &&
                _isAttached && lifetime == _lifetimeVersion) command.Execute(item);
        }
        finally { _isInvoking = false; }
    }

    private void RememberFocus(BreadcrumbItem row)
    {
        if (!_isAttached || !row.HasNavigationFocus || TopLevel.GetTopLevel(this) is not { } root) return;
        _focusToRecover = root.FocusManager?.GetFocusedElement();
        _focusIndex = row.PathIndex;
        if (_focusToRecover is null || _focusRecoveryQueued) return;
        _focusRecoveryQueued = true;
        _focusWatchRoot = root;
        root.AddHandler(GotFocusEvent, OnRecoveryFocusChanged, RoutingStrategies.Bubble, true);
        if (root is WindowBase window) window.Deactivated += OnRecoveryWindowDeactivated;
        var version = ++_focusRecoveryVersion;
        _focusRecoveryOperation = Dispatcher.UIThread.InvokeAsync(() => RecoverFocus(version), DispatcherPriority.Loaded);
    }

    private void OnRecoveryFocusChanged(object? sender, FocusChangedEventArgs e)
    {
        if (!ReferenceEquals(e.Source, _focusToRecover)) CancelFocusRecovery();
    }

    private void OnRecoveryWindowDeactivated(object? sender, EventArgs e) => CancelFocusRecovery();

    private void CancelFocusRecovery()
    {
        _focusRecoveryVersion++;
        _focusRecoveryOperation?.Abort();
        _focusRecoveryOperation = null;
        _focusToRecover = null;
        _focusRecoveryQueued = false;
        _focusWatchRoot?.RemoveHandler(GotFocusEvent, OnRecoveryFocusChanged);
        if (_focusWatchRoot is WindowBase window) window.Deactivated -= OnRecoveryWindowDeactivated;
        _focusWatchRoot = null;
    }

    private void RecoverFocus(long version)
    {
        if (version != _focusRecoveryVersion) return;
        _focusRecoveryOperation = null;
        var previous = _focusToRecover;
        var index = _focusIndex;
        var topLevel = TopLevel.GetTopLevel(this);
        if (!_isAttached || previous is null || topLevel is null) { CancelFocusRecovery(); return; }
        UpdateLayout();
        if (!_isAttached || version != _focusRecoveryVersion) return;
        var manager = topLevel.FocusManager;
        var focused = manager?.GetFocusedElement();
        CancelFocusRecovery();
        // Never overwrite a user's subsequent focus choice, including a different breadcrumb.
        if (focused is not null && !ReferenceEquals(focused, previous)) return;
        if (focused is Visual visual && TopLevel.GetTopLevel(visual) is not null && !visual.GetVisualAncestors().Contains(this)) return;
        if (GetRealizedContainers().OfType<BreadcrumbItem>().Any(row => row.HasNavigationFocus && row.IsNavigationAvailable)) return;
        var start = Math.Min(index, ItemCount - 2);
        for (var i = start; i >= 0; i--)
            if (ContainerFromIndex(i) is BreadcrumbItem row && row.FocusNavigation()) return;
        for (var i = Math.Max(0, start + 1); i < ItemCount - 1; i++)
            if (ContainerFromIndex(i) is BreadcrumbItem row && row.FocusNavigation()) return;
        manager?.Focus(null);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (!_isAttached || e.Handled || e.KeyModifiers != KeyModifiers.None || e.Source is not Visual source) return;
        var row = source.GetSelfAndVisualAncestors().OfType<BreadcrumbItem>().FirstOrDefault();
        if (row is null || !ReferenceEquals(row.Parent, this)) return;
        var current = IndexFromContainer(row);
        var direction = e.Key switch { Key.Left => -1, Key.Right => 1, Key.Home => 1, Key.End => -1, _ => 0 };
        if (direction == 0) return;
        if (FlowDirection == FlowDirection.RightToLeft && e.Key is Key.Left or Key.Right) direction = -direction;
        var index = e.Key == Key.Home ? 0 : e.Key == Key.End ? ItemCount - 1 : current + direction;
        for (; index >= 0 && index < ItemCount; index += direction)
            if (ContainerFromIndex(index) is BreadcrumbItem target && target.FocusNavigation()) { e.Handled = true; break; }
    }
}

/// <summary>The original navigation item and its current path index; the caller decides how to navigate.</summary>
public sealed class BreadcrumbItemInvokedEventArgs(RoutedEvent routedEvent, object source, object? item, int index) : RoutedEventArgs(routedEvent, source)
{
    public object? Item { get; } = item;
    public int Index { get; } = index;
}
