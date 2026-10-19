using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

public enum ChartTooltipMode
{
    Single,
    Shared
}

public partial class ChartBase
{
    public static readonly StyledProperty<bool> ShowCrosshairProperty =
        AvaloniaProperty.Register<ChartBase, bool>(nameof(ShowCrosshair));

    public static readonly StyledProperty<ChartTooltipMode> TooltipModeProperty =
        AvaloniaProperty.Register<ChartBase, ChartTooltipMode>(nameof(TooltipMode),
            validate: value => value is ChartTooltipMode.Single or ChartTooltipMode.Shared);

    public static readonly DirectProperty<ChartBase, Point?> CrosshairPositionProperty =
        AvaloniaProperty.RegisterDirect<ChartBase, Point?>(nameof(CrosshairPosition), chart => chart.CrosshairPosition);

    public static readonly DirectProperty<ChartBase, IReadOnlyList<ChartSelection>> HoveredPointsProperty =
        AvaloniaProperty.RegisterDirect<ChartBase, IReadOnlyList<ChartSelection>>(nameof(HoveredPoints),
            chart => chart.HoveredPoints);

    private readonly Dictionary<int, List<(int Series, int Index)>> _sharedCategorical = [];
    private readonly Dictionary<double, List<(int Series, int Index)>> _sharedNumeric = [];
    private readonly Dictionary<double, double> _sharedScreens = [];

    private Point? _crosshairPosition;
    private long _hoverClearGeneration;
    private bool _hoverClearPosted;
    private long _hoverGeometryVersion = -1;
    private IReadOnlyList<ChartSelection> _hoveredPoints = Array.Empty<ChartSelection>();
    private long _interactionGeneration;
    private double[] _sharedXs = [];

    public bool ShowCrosshair
    {
        get => GetValue(ShowCrosshairProperty);
        set => SetValue(ShowCrosshairProperty, value);
    }

    public ChartTooltipMode TooltipMode
    {
        get => GetValue(TooltipModeProperty);
        set => SetValue(TooltipModeProperty, value);
    }

    public Point? CrosshairPosition => _crosshairPosition;
    public IReadOnlyList<ChartSelection> HoveredPoints => _hoveredPoints;

    private void ResetSharedIndex()
    {
        _sharedNumeric.Clear();
        _sharedCategorical.Clear();
        _sharedScreens.Clear();
        _sharedXs = [];
    }

    private void AddSharedIndex(int series, int index, double x, double screenX, bool categorical)
    {
        List<(int Series, int Index)> entries;
        if (categorical)
        {
            if (!_sharedCategorical.TryGetValue(index, out entries!)) _sharedCategorical[index] = entries = [];
        }
        else
        {
            if (screenX < _cartesianPlot.Left || screenX > _cartesianPlot.Right) return;
            if (!_sharedNumeric.TryGetValue(x, out entries!)) _sharedNumeric[x] = entries = [];
            _sharedScreens[x] = screenX;
            if (entries.Count > 0 && entries[^1].Series == series) return;
        }

        entries.Add((series, index));
    }

    private void FinishSharedIndex()
    {
        _sharedXs = _sharedNumeric.Keys.Order().ToArray();
    }

    /// <summary>Nearest visible numeric X (exact matches per series), or the pointed categorical index. No interpolation.</summary>
    public IReadOnlyList<ChartSelection> GetSharedDataPoints(Point point)
    {
        using var stable = new StableDataFrame(this);
        BuildGeometry(null);
        if (_geometryDirty || IsRadial || !double.IsFinite(point.X) || !double.IsFinite(point.Y) ||
            !_cartesianPlot.Contains(point) || _cartesianFrame is not { } frame ||
            SeriesSource is not { } source) return Array.Empty<ChartSelection>();
        var version = _geometryVersion;
        List<(int Series, int Index)>? entries = null;
        if (frame.Categorical)
        {
            var category = frame.Horizontal
                ? frame.Viewport.Top + (point.Y - _cartesianPlot.Top) / _cartesianPlot.Height * frame.Viewport.Height
                : frame.Viewport.Left + (point.X - _cartesianPlot.Left) / _cartesianPlot.Width * frame.Viewport.Width;
            var index = Math.Clamp((int)Math.Floor(category * frame.Count), 0, frame.Count - 1);
            _sharedCategorical.TryGetValue(index, out entries);
        }
        else if (_sharedXs.Length > 0)
        {
            var low = 0;
            var high = _sharedXs.Length;
            while (low < high)
            {
                var middle = low + (high - low) / 2;
                if (_sharedScreens[_sharedXs[middle]] < point.X) low = middle + 1;
                else high = middle;
            }

            var nearest = Math.Min(low, _sharedXs.Length - 1);
            if (nearest > 0 && Math.Abs(_sharedScreens[_sharedXs[nearest - 1]] - point.X) <=
                Math.Abs(_sharedScreens[_sharedXs[nearest]] - point.X)) nearest--;
            entries = _sharedNumeric[_sharedXs[nearest]];
        }

        if (entries is null) return Array.Empty<ChartSelection>();
        var result = new ChartSelection[entries.Count];
        for (var i = 0; i < entries.Count; i++)
        {
            var key = entries[i];
            var series = source[key.Series];
            result[i] = new ChartSelection(key.Series, key.Index, series, series.ItemsSource![key.Index]);
        }

        return version == _geometryVersion && !_geometryDirty && ReferenceEquals(source, SeriesSource)
            ? Array.AsReadOnly(result)
            : Array.Empty<ChartSelection>();
    }

    private void UpdatePointerHover(Point point)
    {
        var previousCrosshair = _crosshairPosition;
        var previousHover = _hover;
        var previousVersion = _hoverGeometryVersion;
        if (!IsEffectivelyEnabled || _legendKeyboard || HitLegendTarget(point) is not null)
        {
            ClearPointerHover();
            return;
        }

        BuildGeometry(null);
        var inPlot = !IsRadial && !_geometryDirty && _cartesianPlot.Width > 0 && _cartesianPlot.Contains(point);
        var hit = TooltipMode == ChartTooltipMode.Shared && !IsRadial ? null : HitTestDataPoint(point);
        var entries = TooltipMode == ChartTooltipMode.Shared && !IsRadial
            ? GetSharedDataPoints(point)
            : hit is null
                ? Array.Empty<ChartSelection>()
                : Array.AsReadOnly(new[] { hit });
        if (!inPlot && TooltipMode == ChartTooltipMode.Shared && !IsRadial && HitTestDataPoint(point) is { } edge &&
            _positions.TryGetValue((edge.SeriesIndex, edge.PointIndex), out var anchor))
            entries = GetSharedDataPoints(anchor);
        var generation = ++_interactionGeneration;
        var version = _geometryVersion;
        SetAndRaise(CrosshairPositionProperty, ref _crosshairPosition, ShowCrosshair && inPlot ? point : null);
        if (generation != _interactionGeneration || version != _geometryVersion || _geometryDirty) return;
        var unchanged = IsScatterCartesian ? SameScatterHover(entries, _hoveredPoints)
            : entries.Count == _hoveredPoints.Count && entries.Select((entry, i) =>
                entry.SeriesIndex == _hoveredPoints[i].SeriesIndex && entry.PointIndex == _hoveredPoints[i].PointIndex &&
                ReferenceEquals(entry.Point, _hoveredPoints[i].Point)).All(value => value);
        if (!unchanged) SetAndRaise(HoveredPointsProperty, ref _hoveredPoints, entries);
        if (generation != _interactionGeneration || version != _geometryVersion || _geometryDirty) return;
        _hoverGeometryVersion = version;
        _hover = entries.Count == 0 ? null : (entries[0].SeriesIndex, entries[0].PointIndex);
        ToolTip.SetTip(this, entries.Count == 0 ? null : SharedTooltip(entries));
        // A native pointer re-evaluation need not submit all Scatter markers again when its overlay is unchanged.
        // The tooltip is still refreshed, including formats/culture or silently edited values.
        if (!IsScatterCartesian || !unchanged || previousCrosshair != _crosshairPosition || previousHover != _hover ||
            previousVersion != version) InvalidateVisual();
    }

    private static bool SameScatterHover(IReadOnlyList<ChartSelection> current, IReadOnlyList<ChartSelection> previous)
    {
        if (current.Count != previous.Count) return false;
        for (var i = 0; i < current.Count; i++)
            if (current[i].SeriesIndex != previous[i].SeriesIndex || current[i].PointIndex != previous[i].PointIndex ||
                !ReferenceEquals(current[i].Point, previous[i].Point)) return false;
        return true;
    }

    private string SharedTooltip(IReadOnlyList<ChartSelection> entries)
    {
        if (TooltipMode == ChartTooltipMode.Single || IsRadial) return Describe(entries[0]);
        var lines = entries.Take(12).Select(entry => Describe(entry, true)).ToList();
        if (entries.Count > 12) lines.Add($"+{entries.Count - 12} 系列");
        return string.Join(Environment.NewLine, lines);
    }

    private static string BoundedHoverName(string? name)
    {
        const int limit = 32;
        if (string.IsNullOrEmpty(name)) return "";
        if (name.Length <= limit) return name.Replace('\r', ' ').Replace('\n', ' ');
        var end = char.IsHighSurrogate(name[limit - 1]) ? limit - 1 : limit;
        return name[..end].Replace('\r', ' ').Replace('\n', ' ') + "…";
    }

    private void ClearPointerHover()
    {
        var redraw = !IsScatterCartesian || _hover is not null || _crosshairPosition is not null || _hoveredPoints.Count != 0;
        var generation = ++_interactionGeneration;
        _hoverGeometryVersion = -1;
        if (!redraw && ToolTip.GetTip(this) is null) return;
        if (_rendering)
        {
            _hoverClearGeneration = generation;
            if (_hoverClearPosted) return;
            _hoverClearPosted = true;
            Dispatcher.UIThread.Post(() =>
            {
                _hoverClearPosted = false;
                if (_interactionGeneration == _hoverClearGeneration) ClearPointerHover();
            });
            return;
        }

        _hover = null;
        ToolTip.SetTip(this, null);
        SetAndRaise(CrosshairPositionProperty, ref _crosshairPosition, null);
        if (generation != _interactionGeneration) return;
        if (_hoveredPoints.Count != 0)
            SetAndRaise(HoveredPointsProperty, ref _hoveredPoints, Array.Empty<ChartSelection>());
        if (generation == _interactionGeneration && redraw) InvalidateVisual();
    }

    private void DrawPointerOverlay(DrawingContext context)
    {
        if (_geometryDirty || !IsEffectivelyEnabled || IsAnimating || IsRadial) return;
        using var clip = context.PushClip(CartesianDataClip);
        var currentHover = _hoverGeometryVersion == _geometryVersion;
        if (currentHover && ShowCrosshair && !IsRadial && CrosshairPosition is { } pointer &&
            _cartesianPlot.Contains(pointer))
        {
            var halo = new Pen(Background, 3);
            var pen = new Pen(MutedBrush ?? Foreground);
            var top = new Point(pointer.X, _cartesianPlot.Top);
            var bottom = new Point(pointer.X, _cartesianPlot.Bottom);
            var left = new Point(_cartesianPlot.Left, pointer.Y);
            var right = new Point(_cartesianPlot.Right, pointer.Y);
            context.DrawLine(halo, top, bottom);
            context.DrawLine(halo, left, right);
            context.DrawLine(pen, top, bottom);
            context.DrawLine(pen, left, right);
        }

        if (currentHover && _hoveredPoints.Count > 0)
            // Bound the visual overlay as well as the tooltip; the query still returns all originals.
            foreach (var entry in _hoveredPoints.Take(12))
                DrawDataHighlight(context, (entry.SeriesIndex, entry.PointIndex));
        else if (Selection is { } selected) DrawDataHighlight(context, (selected.SeriesIndex, selected.PointIndex));
    }

    private void DrawDataHighlight(DrawingContext context, (int Series, int Index) key)
    {
        if (_cartesianFrame?.Categorical == true)
        {
            if (!_bars.TryGetValue(key, out var bar) || bar.Width <= 0 || bar.Height <= 0) return;
            var inset = Math.Min(SelectionThickness / 2, Math.Min(bar.Width, bar.Height) / 4);
            var bounds = bar.Deflate(inset);
            var radius = _cartesianFrame.Stacked ? 0 : Math.Min(3, Math.Min(bounds.Width, bounds.Height) / 4);
            context.DrawRectangle(null, new Pen(Background, SelectionThickness + 2), bounds, radius, radius);
            context.DrawRectangle(null, new Pen(Foreground, SelectionThickness), bounds, radius, radius);
            return;
        }

        if (_positions.TryGetValue(key, out var point))
        {
            var radius = MarkerRadius + SelectionThickness;
            var pen = IsScatterCartesian && _cartesianFrame is { } frame
                ? ScatterFramePen(frame, 2, Foreground, SelectionThickness) : new Pen(Foreground, SelectionThickness);
            context.DrawEllipse(null, pen, point, radius, radius);
        }
    }
}
