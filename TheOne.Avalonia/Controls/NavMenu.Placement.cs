using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Primitives.PopupPositioning;
using global::Avalonia.Input;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public partial class NavMenu
{
    private void PlacementHostSizeChanged(object? sender, SizeChangedEventArgs args) => UpdateSubmenuPlacement(sender, EventArgs.Empty);

    private void UpdateSubmenuPlacement(object? sender, EventArgs args)
    {
        if (_updatingPlacement || _placementAnchor is not { } anchor || _placementTopLevel is not { } host ||
            _submenu is not { } child || _submenuFlyout?.Popup.Child is not FlyoutPresenter presenter ||
            anchor.TranslatePoint(default, host) is not { } origin || host.ClientSize.Width <= 0 || host.ClientSize.Height <= 0) return;
        _updatingPlacement = true;
        try
        {
            const double inset = 8, gap = 8;
            var viewport = new Rect(inset, inset, Math.Max(1, host.ClientSize.Width - inset * 2), Math.Max(1, host.ClientSize.Height - inset * 2));
            var chromeX = presenter.Padding.Left + presenter.Padding.Right + presenter.BorderThickness.Left + presenter.BorderThickness.Right;
            var chromeY = presenter.Padding.Top + presenter.Padding.Bottom + presenter.BorderThickness.Top + presenter.BorderThickness.Bottom;
            var width = Math.Min(SubmenuWidth + chromeX, viewport.Width);
            var source = new Rect(origin, anchor.Bounds.Size);
            if (_popupParent is { } parent) source = parent._submenuBounds;
            var height = Math.Min(viewport.Height, SubmenuMaxHeight + chromeY + FontSize * 1.5 + 8);
            var top = Math.Clamp(origin.Y, viewport.Top, Math.Max(viewport.Top, viewport.Bottom - height));
            var right = new Rect(source.Right + gap, top, width, height);
            var left = new Rect(source.Left - gap - width, top, width, height);
            bool Fits(Rect candidate)
            {
                if (!viewport.Contains(candidate)) return false;
                for (var owner = _popupParent; owner is not null; owner = owner._popupParent)
                    if (!owner._submenuCovered && owner._submenuBounds.Intersects(candidate)) return false;
                var root = this; while (root._popupParent is not null) root = root._popupParent;
                return root.TranslatePoint(default, host) is not { } rootOrigin || !new Rect(rootOrigin, root.Bounds.Size).Intersects(candidate);
            }
            var preferLeft = _popupParent?.SubmenuOpensLeft == true;
            var compact = false;
            Rect target;
            if (preferLeft && Fits(left)) { target = left; SubmenuOpensLeft = true; }
            else if (Fits(right)) { target = right; SubmenuOpensLeft = false; }
            else if (Fits(left)) { target = left; SubmenuOpensLeft = true; }
            else if (_popupParent is not null)
            {
                // A single visible level with a back action is clearer than overlapping ancestors.
                compact = true; SubmenuOpensLeft = false;
                width = Math.Min(source.Width, viewport.Width);
                target = new Rect(Math.Clamp(source.Left, viewport.Left, viewport.Right - width), top, width, height);
            }
            else
            {
                var rightSpace = Math.Max(0, viewport.Right - source.Right - gap);
                var leftSpace = Math.Max(0, source.Left - gap - viewport.Left);
                SubmenuOpensLeft = leftSpace > rightSpace;
                width = Math.Max(1, Math.Min(width, Math.Max(rightSpace, leftSpace)));
                target = new Rect(SubmenuOpensLeft ? source.Left - gap - width : source.Right + gap, top, width, height);
                target = new Rect(Math.Clamp(target.Left, viewport.Left, viewport.Right - width), target.Top, width, height);
            }
            IsCompactSubmenu = compact;
            _popupParent?.SetSubmenuCovered(compact);
            if (_submenuBackButton is { } back) back.IsVisible = compact;
            presenter.MinWidth = 0; presenter.MaxWidth = viewport.Width; presenter.Width = width; presenter.MaxHeight = viewport.Height;
            child.Width = Math.Max(1, width - chromeX);
            child.MaxHeight = Math.Max(1, Math.Min(SubmenuMaxHeight, viewport.Height - chromeY - (compact ? ItemHeight : FontSize * 1.5) - 8));
            if (_submenuBounds != target) _lastPlacementChange = Environment.TickCount64;
            _submenuBounds = target;
            var popup = _submenuFlyout!.Popup;
            popup.Placement = PlacementMode.BottomEdgeAlignedLeft;
            popup.HorizontalOffset = 0; popup.VerticalOffset = 0;
            popup.PlacementConstraintAdjustment = PopupPositionerConstraintAdjustment.SlideY | PopupPositionerConstraintAdjustment.ResizeY;
            popup.PlacementRect = new Rect(target.Position, new Size(1, 1));
        }
        finally { _updatingPlacement = false; }
    }

    private void SetSubmenuCovered(bool covered)
    {
        _submenuCovered = covered;
        if (_submenuFlyout?.Popup.Child is { } presenter)
        { presenter.Opacity = covered ? 0 : 1; presenter.IsHitTestVisible = !covered; }
    }

    private void PopupPointerMoved(object? sender, PointerEventArgs args)
    {
        if (args.Pointer.Type != PointerType.Mouse) return;
        for (NavMenu? owner = this; owner is not null; owner = owner._submenu)
        {
            var overAnchor = owner._popupParent?._submenuCovered != true && owner._submenuAnchor is { } row && new Rect(row.Bounds.Size).Contains(args.GetPosition(row));
            if (overAnchor || owner.ContainsPopupPoint(args)) owner.CancelHoverClose();
            else owner.ScheduleHoverClose();
        }
    }
}
