using System.Collections.Specialized;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>Header scrolling and adjacent-tab separators for the card theme.</summary>
public sealed class CardTabStrip : DockPanel
{
    public static readonly DirectProperty<CardTabStrip, bool> HasOverflowProperty = AvaloniaProperty.RegisterDirect<CardTabStrip, bool>(nameof(HasOverflow), control => control.HasOverflow);
    public static readonly DirectProperty<CardTabStrip, bool> CanScrollBackwardProperty = AvaloniaProperty.RegisterDirect<CardTabStrip, bool>(nameof(CanScrollBackward), control => control.CanScrollBackward);
    public static readonly DirectProperty<CardTabStrip, bool> CanScrollForwardProperty = AvaloniaProperty.RegisterDirect<CardTabStrip, bool>(nameof(CanScrollForward), control => control.CanScrollForward);
    private bool _hasOverflow, _canScrollBackward, _canScrollForward;
    private bool? _verticalLayout;
    private bool _refreshQueued;
    private bool _scrollSelected;
    private TabItem[] _styledItems = [];
    private TabControl? _owner;
    private ScrollViewer? _scroll;
    private Button? _back, _forward;
    public bool HasOverflow => _hasOverflow;
    public bool CanScrollBackward => _canScrollBackward;
    public bool CanScrollForward => _canScrollForward;
    private bool IsVertical => _owner?.TabStripPlacement is Dock.Left or Dock.Right;

    public CardTabStrip()
    {
        AttachedToVisualTree += (_, _) => Attach();
        DetachedFromVisualTree += (_, _) => Detach();
    }

    private void Attach()
    {
        Detach();
        _owner = TemplatedParent as TabControl;
        _scroll = this.GetVisualDescendants().OfType<ScrollViewer>().SingleOrDefault(control => control.Name == "PART_HeaderScroll");
        _back = Children.OfType<Button>().SingleOrDefault(control => control.Name == "PART_HeaderBack");
        _forward = Children.OfType<Button>().SingleOrDefault(control => control.Name == "PART_HeaderForward");
        if (_owner is null || _scroll is null || _back is null || _forward is null) return;
        _owner.PropertyChanged += OwnerChanged;
        _owner.Items.CollectionChanged += ItemsChanged;
        _owner.ContainerPrepared += ContainerPrepared;
        _scroll.ScrollChanged += ScrollChanged;
        _scroll.AddHandler(PointerWheelChangedEvent, WheelChanged, RoutingStrategies.Tunnel);
        _back.Click += BackClicked; _forward.Click += ForwardClicked;
        _scrollSelected = true; QueueRefresh();
    }

    private void Detach()
    {
        if (_owner is not null)
        {
            _owner.PropertyChanged -= OwnerChanged; _owner.Items.CollectionChanged -= ItemsChanged;
            _owner.ContainerPrepared -= ContainerPrepared;
        }
        if (_scroll is not null) { _scroll.ScrollChanged -= ScrollChanged; _scroll.RemoveHandler(PointerWheelChangedEvent, WheelChanged); }
        if (_back is not null) _back.Click -= BackClicked;
        if (_forward is not null) _forward.Click -= ForwardClicked;
        foreach (var item in _styledItems) ClearClasses(item);
        _styledItems = [];
        _owner = null; _scroll = null; _back = _forward = null; _verticalLayout = null;
    }

    private void QueueRefresh()
    {
        if (_refreshQueued) return;
        _refreshQueued = true;
        Dispatcher.UIThread.Post(() => { _refreshQueued = false; RefreshTabs(); }, DispatcherPriority.Loaded);
    }
    private static void ClearClasses(TabItem item)
    {
        item.Classes.Remove("card-tab"); item.Classes.Remove("before-selected"); item.Classes.Remove("last-tab");
    }
    private void OwnerChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == TabControl.SelectedIndexProperty || args.Property == TabControl.TabStripPlacementProperty)
        { _scrollSelected = true; QueueRefresh(); }
    }
    private void ItemsChanged(object? sender, NotifyCollectionChangedEventArgs args) => EnsureSelection();
    private void EnsureSelection() { _scrollSelected = true; QueueRefresh(); }
    private void ContainerPrepared(object? sender, ContainerPreparedEventArgs args) => QueueRefresh();
    private void RefreshTabs()
    {
        if (_owner is null || _scroll is null) return;
        if (_verticalLayout != IsVertical)
        {
            _verticalLayout = IsVertical;
            if (_back is not null) SetDock(_back, IsVertical ? Dock.Bottom : Dock.Right);
            if (_forward is not null) SetDock(_forward, IsVertical ? Dock.Bottom : Dock.Right);
        }
        var items = _owner.GetRealizedContainers().OfType<TabItem>().ToArray();
        foreach (var old in _styledItems.Except(items)) ClearClasses(old);
        _styledItems = items;
        foreach (var item in items)
        {
            var index = _owner.IndexFromContainer(item);
            item.Classes.Set("card-tab", true);
            item.Classes.Set("before-selected", index + 1 == _owner.SelectedIndex);
            item.Classes.Set("last-tab", index == _owner.Items.Count - 1);
        }
        if (_scrollSelected)
        {
            _scrollSelected = false;
            _owner.ScrollIntoView(_owner.SelectedIndex);
            // Selection exposes the close button and changes a virtual header's width.
            // Repeat after the resulting layout, using the current measured extent.
            var owner = _owner; var selected = owner.SelectedIndex;
            Dispatcher.UIThread.Post(() =>
            {
                if (ReferenceEquals(_owner, owner) && owner.SelectedIndex == selected)
                    owner.ScrollIntoView(selected);
            }, DispatcherPriority.Background);
        }
        RefreshScroll();
    }
    private void ScrollChanged(object? sender, ScrollChangedEventArgs args) => RefreshScroll();
    private void RefreshScroll()
    {
        if (_scroll is null) return;
        var extent = IsVertical ? _scroll.Extent.Height : _scroll.Extent.Width;
        var viewport = IsVertical ? _scroll.Viewport.Height : _scroll.Viewport.Width;
        var offset = IsVertical ? _scroll.Offset.Y : _scroll.Offset.X;
        SetAndRaise(HasOverflowProperty, ref _hasOverflow, extent > viewport + 0.5);
        SetAndRaise(CanScrollBackwardProperty, ref _canScrollBackward, offset > 0.5);
        SetAndRaise(CanScrollForwardProperty, ref _canScrollForward, offset + viewport < extent - 0.5);
    }
    private void BackClicked(object? sender, RoutedEventArgs args) => ScrollBy(-1);
    private void ForwardClicked(object? sender, RoutedEventArgs args) => ScrollBy(1);
    private void ScrollBy(double direction)
    {
        if (_scroll is null) return;
        var viewport = IsVertical ? _scroll.Viewport.Height : _scroll.Viewport.Width;
        SetOffset((IsVertical ? _scroll.Offset.Y : _scroll.Offset.X) + direction * Math.Max(48, viewport * 0.7));
    }
    private void SetOffset(double offset)
    {
        if (_scroll is null) return;
        var maximum = IsVertical ? _scroll.Extent.Height - _scroll.Viewport.Height : _scroll.Extent.Width - _scroll.Viewport.Width;
        offset = Math.Clamp(offset, 0, Math.Max(0, maximum));
        _scroll.Offset = IsVertical ? new Vector(0, offset) : new Vector(offset, 0);
    }
    private void WheelChanged(object? sender, PointerWheelEventArgs args)
    {
        if (_scroll is null || !HasOverflow) return;
        var delta = IsVertical || args.Delta.X == 0 ? args.Delta.Y : args.Delta.X;
        SetOffset((IsVertical ? _scroll.Offset.Y : _scroll.Offset.X) - delta * 48);
        args.Handled = true;
    }
}
