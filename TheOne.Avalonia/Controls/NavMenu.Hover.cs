using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public partial class NavMenu
{
    public static readonly StyledProperty<bool> OpenOnHoverProperty = AvaloniaProperty.Register<NavMenu, bool>(nameof(OpenOnHover), true);
    public static readonly StyledProperty<TimeSpan> HoverOpenDelayProperty = AvaloniaProperty.Register<NavMenu, TimeSpan>(nameof(HoverOpenDelay), TimeSpan.FromMilliseconds(100), validate: ValidDelay);
    public static readonly StyledProperty<TimeSpan> HoverCloseDelayProperty = AvaloniaProperty.Register<NavMenu, TimeSpan>(nameof(HoverCloseDelay), TimeSpan.FromMilliseconds(300), validate: ValidDelay);
    private readonly DispatcherTimer _hoverOpenTimer = new();
    private readonly DispatcherTimer _hoverCloseTimer = new();
    private NavMenuItem? _pendingHover;
    private object? _pendingHoverItem;
    private long _lastPlacementChange;
    private NavMenuItem? _hoverSuppressedItem;

    public bool OpenOnHover { get => GetValue(OpenOnHoverProperty); set => SetValue(OpenOnHoverProperty, value); }
    public TimeSpan HoverOpenDelay { get => GetValue(HoverOpenDelayProperty); set => SetValue(HoverOpenDelayProperty, value); }
    public TimeSpan HoverCloseDelay { get => GetValue(HoverCloseDelayProperty); set => SetValue(HoverCloseDelayProperty, value); }
    private static bool ValidDelay(TimeSpan value) => value >= TimeSpan.Zero && value.TotalMilliseconds <= int.MaxValue;

    private void InitializePopupInteraction()
    {
        _hoverOpenTimer.Tick += (_, _) =>
        {
            var row = _pendingHover; var item = _pendingHoverItem;
            CancelHoverOpen();
            // A recycled anchor must never open the previous model's submenu.
            if (OpenOnHover && UsesPopupNavigation && IsEffectivelyEnabled && this.IsAttachedToVisualTree() &&
                row is { IsPointerOver: true, IsEffectivelyEnabled: true } && row.Parent == this && ReferenceEquals(ItemFromContainer(row), item))
                ShowSubmenu(row, focus: false);
        };
        _hoverCloseTimer.Tick += (_, _) =>
        {
            CancelHoverClose();
            if (_submenuAnchor?.IsPointerOver != true && !HasPointerInSubmenuChain())
            {
                var remaining = HoverCloseDelay.TotalMilliseconds - (Environment.TickCount64 - _lastPlacementChange);
                if (remaining > 0) { _hoverCloseTimer.Interval = TimeSpan.FromMilliseconds(remaining); _hoverCloseTimer.Start(); }
                else CloseSubmenu();
            }
        };
    }

    private void RowPointerEntered(object? sender, PointerEventArgs args)
    {
        if (args.Pointer.Type != PointerType.Mouse || !UsesPopupNavigation || sender is not NavMenuItem row || row.Parent != this) return;
        CancelHoverCloseChain(); CancelHoverOpen();
        if (_hoverSuppressedItem == row) return;
        if (!OpenOnHover || !row.IsEffectivelyEnabled) { ScheduleHoverClose(); return; }
        if (row.ItemCount == 0) { ScheduleHoverClose(); return; }
        if (row == _submenuAnchor && IsSubmenuOpen) return;
        _pendingHover = row; _pendingHoverItem = ItemFromContainer(row);
        _hoverOpenTimer.Interval = HoverOpenDelay; _hoverOpenTimer.Start();
    }

    private void RowPointerExited(object? sender, PointerEventArgs args)
    {
        if (args.Pointer.Type != PointerType.Mouse || !UsesPopupNavigation || sender is not NavMenuItem row || row.Parent != this) return;
        if (_pendingHover == row) CancelHoverOpen();
        if (_hoverSuppressedItem == row) _hoverSuppressedItem = null;
        ScheduleHoverCloseChain();
    }

    private void PopupAnchorPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs change)
    {
        if (sender is not NavMenuItem row || row.Parent != this ||
            change.Property != IsEffectivelyEnabledProperty && change.Property != ItemsControl.ItemCountProperty) return;
        if (change.Property == ItemsControl.ItemCountProperty) ApplyDensity(row);
        if (row.IsEffectivelyEnabled && row.ItemCount > 0) return;
        if (_pendingHover == row) CancelHoverOpen();
        if (_submenuAnchor == row) CloseSubmenu();
    }

    private bool HasPointerInSubmenuChain() => _submenuFlyout?.Popup.Child?.IsPointerOver == true || _submenu?.HasPointerInSubmenuChain() == true;
    private void CancelHoverCloseChain() { CancelHoverClose(); _popupParent?.CancelHoverCloseChain(); }
    private void ScheduleHoverCloseChain() { ScheduleHoverClose(); _popupParent?.ScheduleHoverCloseChain(); }
    private void ScheduleHoverClose()
    {
        if (!IsSubmenuOpen || _hoverCloseTimer.IsEnabled) return;
        _hoverCloseTimer.Interval = HoverCloseDelay; _hoverCloseTimer.Start();
    }
    private void CancelHoverOpen() { _hoverOpenTimer.Stop(); _pendingHover = null; _pendingHoverItem = null; }
    private void CancelHoverClose() => _hoverCloseTimer.Stop();
    private void CancelHover() { CancelHoverOpen(); CancelHoverClose(); }
}
