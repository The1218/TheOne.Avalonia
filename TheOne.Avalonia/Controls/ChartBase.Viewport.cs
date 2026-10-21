using Avalonia;
using Avalonia.Data;
using Avalonia.Input;

namespace TheOne.Avalonia.Controls;

[Flags]
public enum ChartNavigationAxes
{
    None = 0,
    Horizontal = 1,
    Vertical = 2,
    Both = Horizontal | Vertical
}

public sealed class ChartViewportChangedEventArgs(Rect oldViewport, Rect newViewport) : EventArgs
{
    public Rect OldViewport { get; } = oldViewport;
    public Rect NewViewport { get; } = newViewport;
}

public partial class ChartBase
{
    private const double MinimumViewportSpan = 0.001;
    private static readonly Rect FullViewport = new(0, 0, 1, 1);

    public static readonly StyledProperty<Rect> ViewportProperty = AvaloniaProperty.Register<ChartBase, Rect>(
        nameof(Viewport),
        new Rect(0, 0, 1, 1), defaultBindingMode: BindingMode.TwoWay,
        validate: value => double.IsFinite(value.X) && double.IsFinite(value.Y) && double.IsFinite(value.Width) &&
                           double.IsFinite(value.Height) && value.Width > 0 && value.Height > 0,
        coerce: (_, value) => NormalizeViewport(value));

    public static readonly StyledProperty<bool> IsZoomEnabledProperty =
        AvaloniaProperty.Register<ChartBase, bool>(nameof(IsZoomEnabled));

    public static readonly StyledProperty<bool> IsPanEnabledProperty =
        AvaloniaProperty.Register<ChartBase, bool>(nameof(IsPanEnabled));

    public static readonly StyledProperty<ChartNavigationAxes> NavigationAxesProperty =
        AvaloniaProperty.Register<ChartBase, ChartNavigationAxes>(nameof(NavigationAxes), ChartNavigationAxes.Both,
            validate: value => (value & ~ChartNavigationAxes.Both) == 0);

    private bool _applyingViewport;
    private Rect _panPlot;

    private IPointer? _panPointer;
    private Point _panStart;
    private Rect _panStartViewport;
    private bool _panStarted;

    /// <summary>A normalized window into full data extents, bounded to [0,1] and at most 1,000× zoom.</summary>
    public Rect Viewport
    {
        get => GetValue(ViewportProperty);
        set => SetValue(ViewportProperty, value);
    }

    /// <summary>Opt in to wheel and keyboard zoom. Programmatic Zoom does not require this flag.</summary>
    public bool IsZoomEnabled
    {
        get => GetValue(IsZoomEnabledProperty);
        set => SetValue(IsZoomEnabledProperty, value);
    }

    /// <summary>Opt in to captured pointer dragging and keyboard pan. Programmatic Pan does not require this flag.</summary>
    public bool IsPanEnabled
    {
        get => GetValue(IsPanEnabledProperty);
        set => SetValue(IsPanEnabledProperty, value);
    }

    public ChartNavigationAxes NavigationAxes
    {
        get => GetValue(NavigationAxesProperty);
        set => SetValue(NavigationAxesProperty, value);
    }

    public event EventHandler<ChartViewportChangedEventArgs>? ViewportChanged;

    private Rect GetCartesianViewport()
    {
        return Viewport;
    }

    private static Rect NormalizeViewport(Rect value)
    {
        var width = Math.Clamp(value.Width, MinimumViewportSpan, 1);
        var height = Math.Clamp(value.Height, MinimumViewportSpan, 1);
        return new Rect(Math.Clamp(value.X, 0, 1 - width), Math.Clamp(value.Y, 0, 1 - height), width, height);
    }

    private void ApplyViewport(Rect viewport)
    {
        _applyingViewport = true;
        try
        {
            SetCurrentValue(ViewportProperty, NormalizeViewport(viewport));
        }
        finally
        {
            _applyingViewport = false;
        }
    }

    /// <summary>Zoom by a positive factor around fractional coordinates in the current view. Factors above one zoom in.</summary>
    public void Zoom(double factor, Point? anchor = null)
    {
        if (!double.IsFinite(factor) || factor <= 0) throw new ArgumentOutOfRangeException(nameof(factor));
        var center = anchor ?? new Point(0.5, 0.5);
        if (!double.IsFinite(center.X) || !double.IsFinite(center.Y))
            throw new ArgumentOutOfRangeException(nameof(anchor));
        if (!SupportsViewportNavigation) return;
        center = new Point(Math.Clamp(center.X, 0, 1), Math.Clamp(center.Y, 0, 1));
        var old = Viewport;
        var width = NavigationAxes.HasFlag(ChartNavigationAxes.Horizontal)
            ? Math.Clamp(old.Width / factor, MinimumViewportSpan, 1)
            : old.Width;
        var height = NavigationAxes.HasFlag(ChartNavigationAxes.Vertical)
            ? Math.Clamp(old.Height / factor, MinimumViewportSpan, 1)
            : old.Height;
        ApplyViewport(new Rect(old.X + (old.Width - width) * center.X, old.Y + (old.Height - height) * center.Y, width,
            height));
    }

    /// <summary>Pan by fractions of the visible spans. Positive deltas move towards larger axis values/category indices.</summary>
    public void Pan(double horizontal, double vertical)
    {
        if (!double.IsFinite(horizontal)) throw new ArgumentOutOfRangeException(nameof(horizontal));
        if (!double.IsFinite(vertical)) throw new ArgumentOutOfRangeException(nameof(vertical));
        if (!SupportsViewportNavigation) return;
        var old = Viewport;
        // Clamp the step before multiplication so even extreme finite API arguments remain safe.
        var x = NavigationAxes.HasFlag(ChartNavigationAxes.Horizontal)
            ? old.X + Math.Clamp(horizontal, -1000, 1000) * old.Width
            : old.X;
        var y = NavigationAxes.HasFlag(ChartNavigationAxes.Vertical)
            ? old.Y + Math.Clamp(vertical, -1000, 1000) * old.Height
            : old.Y;
        ApplyViewport(new Rect(x, y, old.Width, old.Height));
    }

    public void ResetViewport()
    {
        CancelPan();
        ApplyViewport(FullViewport);
    }

    /// <summary>Reveal a valid data mark without changing the current zoom. This explicit operation can move both axes.</summary>
    public bool BringDataPointIntoView(int seriesIndex, int pointIndex)
    {
        if (!SupportsViewportNavigation || GetSelection(seriesIndex, pointIndex) is null) return false;
        BuildGeometry(null);
        if (!_normalizedPositions.TryGetValue((seriesIndex, pointIndex), out var point) || !double.IsFinite(point.X) ||
            !double.IsFinite(point.Y) || point.X < 0 || point.X > 1 || point.Y < 0 || point.Y > 1) return false;
        var old = Viewport;
        var x = point.X < old.Left || point.X > old.Right ? point.X - old.Width / 2 : old.X;
        var y = point.Y < old.Top || point.Y > old.Bottom ? point.Y - old.Height / 2 : old.Y;
        ApplyViewport(new Rect(x, y, old.Width, old.Height));
        BuildGeometry(null);
        return _positions.ContainsKey((seriesIndex, pointIndex));
    }

    private void HandleNavigationChange(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ViewportProperty)
        {
            if (!_applyingViewport) CancelPan();
            ClearPointerHover();
            CancelLegendPress();
            InvalidateVisual();
            ViewportChanged?.Invoke(this,
                new ChartViewportChangedEventArgs(change.GetOldValue<Rect>(), change.GetNewValue<Rect>()));
        }
        else if (change.Property == NavigationAxesProperty || change.Property == SeriesSourceProperty ||
                 (change.Property == IsPanEnabledProperty && !IsPanEnabled) ||
                 (change.Property == IsEffectivelyEnabledProperty && !IsEffectivelyEnabled))
        {
            CancelPan();
        }
    }

    private bool BeginPan(PointerPressedEventArgs e)
    {
        if (!IsPanEnabled || !IsEffectivelyEnabled || !SupportsViewportNavigation || NavigationAxes == ChartNavigationAxes.None)
            return false;
        BuildGeometry(null);
        var pointer = e.GetPosition(this);
        if (_cartesianPlot.Width <= 0 || _cartesianPlot.Height <= 0 || !_cartesianPlot.Contains(pointer)) return false;
        CancelPan();
        _panPointer = e.Pointer;
        _panStart = pointer;
        _panStartViewport = Viewport;
        _panPlot = _cartesianPlot;
        _panStarted = false;
        e.Pointer.Capture(this);
        e.Handled = true;
        return true;
    }

    private bool MovePan(PointerEventArgs e)
    {
        if (_panPointer != e.Pointer) return false;
        var delta = e.GetPosition(this) - _panStart;
        if (!_panStarted && delta.X * delta.X + delta.Y * delta.Y < 16) return true;
        _panStarted = true;
        ClearPointerHover();
        var x = NavigationAxes.HasFlag(ChartNavigationAxes.Horizontal)
            ? _panStartViewport.X - delta.X / _panPlot.Width * _panStartViewport.Width
            : _panStartViewport.X;
        var y = NavigationAxes.HasFlag(ChartNavigationAxes.Vertical)
            ? _panStartViewport.Y +
              (IsHorizontalCartesian ? -1 : 1) * delta.Y / _panPlot.Height * _panStartViewport.Height
            : _panStartViewport.Y;
        ApplyViewport(new Rect(x, y, _panStartViewport.Width, _panStartViewport.Height));
        e.Handled = true;
        return true;
    }

    private void CancelPan()
    {
        var pointer = _panPointer;
        _panPointer = null;
        _panStarted = false;
        if (pointer?.Captured == this) pointer.Capture(null);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (EndLegendPress(e)) return;
        if (_panPointer != e.Pointer || e.InitialPressMouseButton != MouseButton.Left) return;
        var dragged = _panStarted;
        CancelPan();
        if (!dragged && IsEffectivelyEnabled && HitTestDataPoint(e.GetPosition(this)) is { } hit)
            SelectPoint(hit.SeriesIndex, hit.PointIndex);
        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        if (_panPointer == e.Pointer) CancelPan();
        if (_legendPointer == e.Pointer) CancelLegendPress();
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (!IsZoomEnabled || !IsEffectivelyEnabled || !SupportsViewportNavigation || NavigationAxes == ChartNavigationAxes.None ||
            !double.IsFinite(e.Delta.Y) || e.Delta.Y == 0) return;
        BuildGeometry(null);
        var point = e.GetPosition(this);
        if (_cartesianPlot.Width <= 0 || _cartesianPlot.Height <= 0 || !_cartesianPlot.Contains(point)) return;
        var anchor = new Point((point.X - _cartesianPlot.Left) / _cartesianPlot.Width,
            IsHorizontalCartesian
                ? (point.Y - _cartesianPlot.Top) / _cartesianPlot.Height
                : (_cartesianPlot.Bottom - point.Y) / _cartesianPlot.Height);
        var old = Viewport;
        Zoom(Math.Pow(1.2, Math.Clamp(e.Delta.Y, -20, 20)), anchor);
        e.Handled = Viewport != old;
    }

    private bool HandleNavigationKey(KeyEventArgs e)
    {
        if (!IsEffectivelyEnabled || !SupportsViewportNavigation) return false;
        var modifier = (e.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Meta)) != 0;
        if (modifier && e.Key is Key.D0 or Key.NumPad0 && (IsZoomEnabled || IsPanEnabled))
        {
            ResetViewport();
            return true;
        }

        if (IsZoomEnabled && e.Key is Key.Add or Key.OemPlus)
        {
            Zoom(1.2);
            return true;
        }

        if (IsZoomEnabled && e.Key is Key.Subtract or Key.OemMinus)
        {
            Zoom(1 / 1.2);
            return true;
        }

        if (!modifier || !IsPanEnabled) return false;
        switch (e.Key)
        {
            case Key.Left:
                Pan(-0.1, 0);
                return true;
            case Key.Right:
                Pan(0.1, 0);
                return true;
            case Key.Up:
                Pan(0, IsHorizontalCartesian ? -0.1 : 0.1);
                return true;
            case Key.Down:
                Pan(0, IsHorizontalCartesian ? 0.1 : -0.1);
                return true;
            default: return false;
        }
    }
}
