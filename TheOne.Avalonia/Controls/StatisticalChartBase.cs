using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>Cached numeric axes, viewport navigation and indexed hits for range-valued observations.</summary>
public abstract class StatisticalChartBase : SpecializedChartBase
{
    public static readonly StyledProperty<double> PointWidthRatioProperty = AvaloniaProperty.Register<StatisticalChartBase, double>(
        nameof(PointWidthRatio), .6, validate: value => double.IsFinite(value) && value > 0 && value <= 1);

    private readonly List<AxisLabel> _axisLabels = [];
    private readonly List<HitMark> _hitMarks = [];
    private readonly List<Observation> _observations = [];
    private StreamGeometry? _grid;
    private Pen? _gridPen;
    private double _minimum, _maximum;
    private double _maximumHitHalfWidth;
    private Rect _dataPlot;

    public double PointWidthRatio { get => GetValue(PointWidthRatioProperty); set => SetValue(PointWidthRatioProperty, value); }
    private protected override bool SupportsViewportNavigation => true;
    private protected override bool UseDefaultMarkHitTesting => false;
    // Alias notifications serve bindings; the underlying Y notification already invalidates the chart.
    private protected override bool ShouldProcessChartDataChange(AvaloniaProperty property) =>
        property != BoxPlotPoint.MedianProperty && property != CandlestickPoint.CloseProperty;
    private protected Rect DataPlot => _dataPlot;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == PointWidthRatioProperty) InvalidateChartGeometry();
    }

    private protected abstract bool TryGetObservationRange(ChartPoint point, out double minimum, out double maximum);
    private protected abstract void ClearStatisticalGlyphs();
    private protected abstract void PrepareStatisticalGlyph(int series, int index, ChartPoint point, double x, double width);
    private protected abstract void DrawStatisticalGlyphs(DrawingContext context);
    private protected abstract bool ContainsStatisticalGlyph(int series, int index, Point point);

    private protected sealed override void PrepareSpecializedGeometry(Rect plot)
    {
        ClearStatisticalGlyphs();
        _axisLabels.Clear(); _hitMarks.Clear(); _observations.Clear(); _grid = null; _dataPlot = default;
        _maximumHitHalfWidth = 0;
        if (SeriesSource is not { } source) return;
        var xs = new SortedDictionary<double, string?>();
        var xmin = double.PositiveInfinity; var xmax = double.NegativeInfinity;
        var ymin = double.PositiveInfinity; var ymax = double.NegativeInfinity;
        var visible = new List<int>();
        for (var s = 0; s < source.Count; s++)
        {
            var series = source[s];
            if (!series.IsVisible || series.ItemsSource is not { } points) continue;
            visible.Add(s);
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                if (!TryGetObservationRange(point, out var low, out var high)) continue;
                _observations.Add(new Observation(s, i, point));
                xmin = Math.Min(xmin, point.X); xmax = Math.Max(xmax, point.X);
                ymin = Math.Min(ymin, low); ymax = Math.Max(ymax, high);
                xs.TryAdd(point.X, point.Label);
            }
        }
        if (_observations.Count == 0) return;
        Expand(ref xmin, ref xmax); Expand(ref ymin, ref ymax);
        if (double.IsFinite(YMinimum)) ymin = YMinimum;
        if (double.IsFinite(YMaximum)) ymax = YMaximum;
        if (ymin >= ymax) return;
        _minimum = ymin; _maximum = ymax;
        var viewport = Viewport;
        var longest = 0d;
        for (var tick = 0; tick <= 4; tick++)
        {
            var value = At(ymin, ymax, 1 - viewport.Top - viewport.Height * tick / 4);
            longest = Math.Max(longest, Text(AxisNumber(value, ymin, ymax), MutedBrush).Width);
        }
        var left = Math.Min(plot.Width * .3, longest + 10);
        var bottom = FontSize * 1.6 + 8;
        _dataPlot = new Rect(plot.Left + left, plot.Top + 4, Math.Max(0, plot.Width - left - 6), Math.Max(0, plot.Height - bottom - 4));
        if (_dataPlot.Width <= 0 || _dataPlot.Height <= 0) return;
        SetChartDataPlot(_dataPlot);
        _gridPen = new Pen(GridBrush ?? MutedBrush, 1);
        _grid = new StreamGeometry();
        using (var path = _grid.Open())
        {
            for (var tick = 0; tick <= 4; tick++)
            {
                var y = _dataPlot.Top + tick * _dataPlot.Height / 4;
                var value = At(ymin, ymax, 1 - viewport.Top - viewport.Height * tick / 4);
                var label = Text(AxisNumber(value, ymin, ymax), MutedBrush);
                if (label.Width <= left - 4) _axisLabels.Add(new AxisLabel(label, new Point(_dataPlot.Left - label.Width - 6, y - label.Height / 2)));
                path.BeginFigure(new Point(_dataPlot.Left, y), false); path.LineTo(new Point(_dataPlot.Right, y)); path.EndFigure(false);
            }
        }
        var ordered = xs.Keys.ToArray();
        var spacing = 1d;
        for (var i = 1; i < ordered.Length; i++)
        {
            var delta = Ratio(ordered[i], xmin, xmax) - Ratio(ordered[i - 1], xmin, xmax);
            if (delta > 0) spacing = Math.Min(spacing, delta);
        }
        if (ordered.Length > 1)
        {
            // Reserve half a data slot on each side instead of clipping endpoint bodies.
            var padding = spacing / 2;
            xmin = At(xmin, xmax, -padding); xmax = At(xmin, xmax, 1 + padding / (1 + padding));
            spacing /= 1 + spacing;
        }
        var slot = spacing / viewport.Width * _dataPlot.Width;
        var width = slot * PointWidthRatio / Math.Max(1, visible.Count);
        var ordinals = visible.Select((series, ordinal) => (series, ordinal)).ToDictionary(x => x.series, x => x.ordinal);
        foreach (var observation in _observations)
        {
            var normalX = Ratio(observation.Point.X, xmin, xmax);
            var normalY = 1 - Ratio(observation.Point.Y, ymin, ymax);
            SetChartNormalizedPosition(observation.Series, observation.Index, new Point(normalX, normalY));
            var x = _dataPlot.Left + (normalX - viewport.Left) / viewport.Width * _dataPlot.Width;
            x += (ordinals[observation.Series] - (visible.Count - 1) / 2d) * width;
            if (x + width / 2 < _dataPlot.Left || x - width / 2 > _dataPlot.Right) continue;
            PrepareStatisticalGlyph(observation.Series, observation.Index, observation.Point, x, width);
        }
        _hitMarks.Sort((a, b) => a.X.CompareTo(b.X));
        var candidates = xs.Where(entry => Ratio(entry.Key, xmin, xmax) >= viewport.Left && Ratio(entry.Key, xmin, xmax) <= viewport.Right).ToArray();
        var lastRight = double.NegativeInfinity;
        var labelCount = Math.Min(6, candidates.Length);
        for (var tick = 0; tick < labelCount; tick++)
        {
            var i = labelCount <= 1 ? 0 : tick * (candidates.Length - 1) / (labelCount - 1);
            var entry = candidates[i];
            var x = _dataPlot.Left + (Ratio(entry.Key, xmin, xmax) - viewport.Left) / viewport.Width * _dataPlot.Width;
            var label = Text(entry.Value ?? AxisNumber(entry.Key, xmin, xmax), MutedBrush);
            var leftEdge = Math.Clamp(x - label.Width / 2, _dataPlot.Left, Math.Max(_dataPlot.Left, _dataPlot.Right - label.Width));
            if (leftEdge < lastRight + 6 || label.Width > _dataPlot.Width) continue;
            _axisLabels.Add(new AxisLabel(label, new Point(leftEdge, _dataPlot.Bottom + 6)));
            lastRight = leftEdge + label.Width;
        }
    }

    private protected double MapStatisticalValue(double value)
    {
        var normalized = 1 - Ratio(value, _minimum, _maximum);
        return _dataPlot.Top + Math.Clamp((normalized - Viewport.Top) / Viewport.Height, -1e6, 1e6) * _dataPlot.Height;
    }

    private protected Point? GetStatisticalAnchor(double x, double preferredY, Rect body, double low, double high)
    {
        var visibleBody = body.Inflate(Math.Max(1, StrokeThickness) / 2).Intersect(_dataPlot);
        if (visibleBody.Width > 0 && visibleBody.Height > 0)
            return new Point(Math.Clamp(x, visibleBody.Left, visibleBody.Right), Math.Clamp(preferredY, visibleBody.Top, visibleBody.Bottom));
        var top = Math.Max(_dataPlot.Top, Math.Min(low, high));
        var bottom = Math.Min(_dataPlot.Bottom, Math.Max(low, high));
        if (top <= bottom && x >= _dataPlot.Left && x <= _dataPlot.Right)
            return new Point(x, Math.Clamp(preferredY, top, bottom));
        return null;
    }

    private protected void RegisterStatisticalMark(int series, int index, double x, Rect bounds, Point anchor)
    {
        bounds = bounds.Intersect(_dataPlot);
        if (bounds.Width <= 0 || bounds.Height <= 0) return;
        _maximumHitHalfWidth = Math.Max(_maximumHitHalfWidth, Math.Max(x - bounds.Left, bounds.Right - x));
        _hitMarks.Add(new HitMark(series, index, x, bounds, _hitMarks.Count));
        RegisterMark(series, index, anchor, bounds);
    }

    private protected sealed override void DrawChartPlot(DrawingContext context, Rect plot)
    {
        if (ShowGrid && _grid is not null) context.DrawGeometry(null, _gridPen, _grid);
        foreach (var label in _axisLabels) context.DrawText(label.Text, label.Position);
        using var clip = context.PushClip(_dataPlot);
        DrawStatisticalGlyphs(context);
    }

    private protected sealed override (int Series, int Index)? HitTestMark(Point point)
    {
        if (!_dataPlot.Contains(point)) return null;
        var low = 0; var high = _hitMarks.Count;
        while (low < high)
        {
            var mid = low + (high - low) / 2;
            if (_hitMarks[mid].X < point.X - _maximumHitHalfWidth) low = mid + 1; else high = mid;
        }
        HitMark? best = null;
        for (var i = low; i < _hitMarks.Count && _hitMarks[i].X <= point.X + _maximumHitHalfWidth; i++)
        {
            var mark = _hitMarks[i];
            if (mark.Bounds.Contains(point) && (best is null || mark.Order > best.Value.Order) && ContainsStatisticalGlyph(mark.Series, mark.Index, point)) best = mark;
        }
        return best is { } hit ? (hit.Series, hit.Index) : null;
    }

    private protected override void CopyExportProperties(ChartBase snapshot)
    {
        base.CopyExportProperties(snapshot);
        ((StatisticalChartBase)snapshot).PointWidthRatio = PointWidthRatio;
    }

    private static string AxisNumber(double value, double minimum, double maximum) => TickText(value, (maximum / 4 - minimum / 4), "0.##");
    private static double At(double minimum, double maximum, double fraction)
    {
        var value = minimum * (1 - fraction) + maximum * fraction;
        return double.IsFinite(value) ? value : fraction <= 0 ? minimum : maximum;
    }
    private static double Ratio(double value, double minimum, double maximum)
    {
        var span = maximum - minimum;
        return double.IsFinite(span) ? (value - minimum) / span : (value / 2 - minimum / 2) / (maximum / 2 - minimum / 2);
    }
    private static void Expand(ref double minimum, ref double maximum)
    {
        if (minimum < maximum) return;
        var padding = Math.Max(1, Math.Abs(minimum) * .05);
        var low = minimum - padding; var high = maximum + padding;
        if (double.IsFinite(low)) minimum = low;
        if (double.IsFinite(high)) maximum = high;
    }
    private readonly record struct AxisLabel(FormattedText Text, Point Position);
    private readonly record struct Observation(int Series, int Index, ChartPoint Point);
    private readonly record struct HitMark(int Series, int Index, double X, Rect Bounds, int Order);
}
