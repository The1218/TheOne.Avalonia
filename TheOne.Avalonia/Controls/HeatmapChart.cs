using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>A native continuous-color heatmap with coordinate-based cells and cached color batches.</summary>
public sealed class HeatmapChart : SpecializedChartBase
{
    public static readonly StyledProperty<double> CellWidthProperty =
        AvaloniaProperty.Register<HeatmapChart, double>(nameof(CellWidth), 1,
            validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<double> CellHeightProperty =
        AvaloniaProperty.Register<HeatmapChart, double>(nameof(CellHeight), 1,
            validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<double> CellGapProperty =
        AvaloniaProperty.Register<HeatmapChart, double>(nameof(CellGap), 2,
            validate: value => double.IsFinite(value) && value >= 0);
    public static readonly StyledProperty<double> ValueMinimumProperty =
        AvaloniaProperty.Register<HeatmapChart, double>(nameof(ValueMinimum), double.NaN,
            validate: value => !double.IsInfinity(value));
    public static readonly StyledProperty<double> ValueMaximumProperty =
        AvaloniaProperty.Register<HeatmapChart, double>(nameof(ValueMaximum), double.NaN,
            validate: value => !double.IsInfinity(value));
    public static readonly StyledProperty<Color?> LowColorProperty =
        AvaloniaProperty.Register<HeatmapChart, Color?>(nameof(LowColor));
    public static readonly StyledProperty<Color?> MidColorProperty =
        AvaloniaProperty.Register<HeatmapChart, Color?>(nameof(MidColor));
    public static readonly StyledProperty<Color?> HighColorProperty =
        AvaloniaProperty.Register<HeatmapChart, Color?>(nameof(HighColor));

    private readonly List<ColorBatch> _batches = [];
    private readonly List<OrderedPaint> _overlapPaint = [];
    private readonly List<CellLabel> _labels = [];
    private readonly Dictionary<(int Series, int Index), Rect> _cells = [];
    private readonly Dictionary<(int X, int Y), List<CellHit>> _hitBuckets = [];
    private Pen? _selectionPen;

    public override ChartKind Kind => ChartKind.Heatmap;

    private protected override bool UseDefaultMarkHitTesting => false;

    /// <summary>The cell width in X coordinate units.</summary>
    public double CellWidth { get => GetValue(CellWidthProperty); set => SetValue(CellWidthProperty, value); }
    /// <summary>The cell height in Y coordinate units.</summary>
    public double CellHeight { get => GetValue(CellHeightProperty); set => SetValue(CellHeightProperty, value); }
    /// <summary>The visual gap between cells, in logical pixels.</summary>
    public double CellGap { get => GetValue(CellGapProperty); set => SetValue(CellGapProperty, value); }
    public double ValueMinimum { get => GetValue(ValueMinimumProperty); set => SetValue(ValueMinimumProperty, value); }
    public double ValueMaximum { get => GetValue(ValueMaximumProperty); set => SetValue(ValueMaximumProperty, value); }
    public Color? LowColor { get => GetValue(LowColorProperty); set => SetValue(LowColorProperty, value); }
    public Color? MidColor { get => GetValue(MidColorProperty); set => SetValue(MidColorProperty, value); }
    public Color? HighColor { get => GetValue(HighColorProperty); set => SetValue(HighColorProperty, value); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CellWidthProperty || change.Property == CellHeightProperty ||
            change.Property == CellGapProperty || change.Property == ValueMinimumProperty ||
            change.Property == ValueMaximumProperty || change.Property == LowColorProperty ||
            change.Property == MidColorProperty || change.Property == HighColorProperty)
            InvalidateChartGeometry();
    }

    private protected override bool IsValidDataPoint(ChartPoint point) =>
        point is HeatmapPoint cell && double.IsFinite(cell.X) && double.IsFinite(cell.Y) && double.IsFinite(cell.Value);

    private protected override string? DescribeSpecializedPoint(ChartSelection selection) =>
        selection.Point is HeatmapPoint point
            ? $"{selection.Series.Name} · {point.Label ?? $"({FormatValue(point.X)}, {FormatValue(point.Y)})"}: {FormatChartValue(selection.Series, point.Value)}{selection.Series.Unit ?? Unit}"
            : null;

    private protected override void PrepareSpecializedGeometry(Rect plot)
    {
        _batches.Clear();
        _overlapPaint.Clear();
        _labels.Clear();
        _cells.Clear();
        _hitBuckets.Clear();
        _selectionPen = new Pen(Foreground ?? GetChartBrush(0), SelectionThickness);
        if (SeriesSource is not { } source || plot.Width <= 0 || plot.Height <= 0) return;
        var xmin = double.PositiveInfinity;
        var xmax = double.NegativeInfinity;
        var ymin = double.PositiveInfinity;
        var ymax = double.NegativeInfinity;
        var vmin = double.PositiveInfinity;
        var vmax = double.NegativeInfinity;
        var count = 0;
        foreach (var series in source)
        {
            if (!series.IsVisible || series.ItemsSource is not { } points) continue;
            foreach (var item in points)
            {
                if (item is not HeatmapPoint cell || !double.IsFinite(cell.X) || !double.IsFinite(cell.Y)) continue;
                // A missing value retains its location and therefore cannot collapse sparse axes.
                xmin = Math.Min(xmin, cell.X);
                xmax = Math.Max(xmax, cell.X);
                ymin = Math.Min(ymin, cell.Y);
                ymax = Math.Max(ymax, cell.Y);
                if (!double.IsFinite(cell.Value)) continue;
                vmin = Math.Min(vmin, cell.Value);
                vmax = Math.Max(vmax, cell.Value);
                count++;
            }
        }
        if (count == 0) return;
        if (double.IsFinite(ValueMinimum)) vmin = ValueMinimum;
        if (double.IsFinite(ValueMaximum)) vmax = ValueMaximum;
        if (vmax < vmin) (vmin, vmax) = (vmax, vmin);
        var xpadding = Math.Min(CellWidth / 2, double.MaxValue - Math.Abs(xmin));
        var ypadding = Math.Min(CellHeight / 2, double.MaxValue - Math.Abs(ymin));
        var xstart = xmin - xpadding;
        var xend = Math.Min(double.MaxValue, xmax + CellWidth / 2);
        var ystart = ymin - ypadding;
        var yend = Math.Min(double.MaxValue, ymax + CellHeight / 2);
        var textHeight = Text("0").Height;
        var leftLabel = Text(FormatValue(ymax), MutedBrush);
        var left = Math.Min(plot.Width * .22, Math.Max(leftLabel.Width, Text(FormatValue(ymin), MutedBrush).Width) + 8);
        var area = new Rect(plot.Left + left, plot.Top + textHeight / 2,
            Math.Max(1, plot.Width - left - 4), Math.Max(1, plot.Height - textHeight * 1.7 - 6));
        var palette = BuildPalette(vmin < 0 && vmax > 0);
        var rectangles = new List<Rect>?[palette.Length];
        var paintOrder = new List<PaintCell>(count);
        var dataLabels = ShowDataLabels && count <= 5000 ? new Dictionary<int, CellLabel>() : null;
        var coordinates = new HashSet<(double X, double Y)>();
        var xIntervals = new Dictionary<double, (double Start, double End)>();
        var yIntervals = new Dictionary<double, (double Start, double End)>();
        var uniqueCoordinates = true;
        for (var seriesIndex = 0; seriesIndex < source.Count; seriesIndex++)
        {
            var series = source[seriesIndex];
            if (!series.IsVisible || series.ItemsSource is not { } points) continue;
            for (var index = 0; index < points.Count; index++)
            {
                if (points[index] is not HeatmapPoint cell || !IsValidDataPoint(cell)) continue;
                var x0 = Scale(cell.X - CellWidth / 2, xstart, xend);
                var x1 = Scale(cell.X + CellWidth / 2, xstart, xend);
                var y0 = Scale(cell.Y - CellHeight / 2, ystart, yend);
                var y1 = Scale(cell.Y + CellHeight / 2, ystart, yend);
                var bounds = new Rect(area.Left + x0 * area.Width, area.Bottom - y1 * area.Height,
                    Math.Max(.01, (x1 - x0) * area.Width), Math.Max(.01, (y1 - y0) * area.Height));
                var gap = Math.Min(CellGap / 2, Math.Min(bounds.Width, bounds.Height) * .45);
                bounds = bounds.Deflate(gap);
                var fraction = vmin < 0 && vmax > 0
                    ? cell.Value < 0 ? .5 * Scale(cell.Value, vmin, 0) : .5 + .5 * Scale(cell.Value, 0, vmax)
                    : Scale(cell.Value, vmin, vmax);
                var color = (int)Math.Round(Math.Clamp(fraction, 0, 1) * (palette.Length - 1));
                (rectangles[color] ??= []).Add(bounds);
                paintOrder.Add(new PaintCell(bounds, color));
                uniqueCoordinates &= coordinates.Add((cell.X, cell.Y));
                AddInterval(xIntervals, cell.X, bounds.Left, bounds.Right);
                AddInterval(yIntervals, cell.Y, bounds.Top, bounds.Bottom);
                _cells[(seriesIndex, index)] = bounds;
                RegisterMark(seriesIndex, index, bounds.Center, bounds);
                AddHit(new CellHit(seriesIndex, index, bounds));
                if (dataLabels is not null)
                {
                    var label = Text(FormatChartValue(series, cell.Value));
                    if (label.Width + 4 <= bounds.Width && label.Height + 2 <= bounds.Height)
                        dataLabels[paintOrder.Count - 1] = new CellLabel(label, bounds.Center - new Vector(label.Width / 2, label.Height / 2));
                }
            }
        }
        // Global color batches are order-independent only after proving that actual screen-space cells
        // are disjoint. This also handles projected subpixel cells and extreme coordinate precision.
        var disjoint = uniqueCoordinates && IntervalsAreDisjoint(xIntervals) && IntervalsAreDisjoint(yIntervals);
        if (disjoint)
        {
            for (var index = 0; index < rectangles.Length; index++)
                if (rectangles[index] is { Count: > 0 } cells) AddBatch(cells, palette[index]);
            if (dataLabels is not null) _labels.AddRange(dataLabels.Values);
        }
        else if (dataLabels is { Count: > 0 } || palette.Any(brush => brush is ISolidColorBrush solid && solid.Color.A < 255))
        {
            // Labels and translucent colors require individual source-order commands. An older label
            // must not remain above a newer cell, and transparent overlaps must composite separately.
            for (var index = 0; index < paintOrder.Count; index++)
            {
                var cell = paintOrder[index];
                _overlapPaint.Add(new OrderedPaint(cell.Bounds, palette[cell.Color],
                    dataLabels is not null && dataLabels.TryGetValue(index, out var label) ? label : null));
            }
        }
        else
        {
            // Preserve original painter order for duplicates and overlaps; only adjacent equal-color
            // cells may share one cached path. Hit testing uses the identical reverse source order.
            var start = 0;
            while (start < paintOrder.Count)
            {
                var color = paintOrder[start].Color;
                var end = start + 1;
                while (end < paintOrder.Count && paintOrder[end].Color == color) end++;
                var geometry = new StreamGeometry();
                using (var path = geometry.Open())
                {
                    path.SetFillRule(FillRule.NonZero);
                    for (var index = start; index < end; index++) AppendCell(path, paintOrder[index].Bounds);
                }
                _batches.Add(new ColorBatch(geometry, palette[color]));
                start = end;
            }
        }
        var tickWidth = Math.Max(Text(FormatValue(xmin), MutedBrush).Width, Text(FormatValue(xmax), MutedBrush).Width);
        var xTicks = xmin == xmax ? 0 : Math.Clamp((int)(area.Width / (tickWidth + 12)), 1, 4);
        for (var tick = 0; tick <= xTicks; tick++)
        {
            var fraction = xTicks == 0 ? 0 : (double)tick / xTicks;
            var value = Interpolate(xmin, xmax, fraction);
            var text = Text(FormatValue(value), MutedBrush);
            _labels.Add(new CellLabel(text, new Point(area.Left + Scale(value, xstart, xend) * area.Width - text.Width / 2, area.Bottom + 4)));
        }
        var yTicks = ymin == ymax ? 0 : Math.Clamp((int)(area.Height / (textHeight + 8)), 1, 4);
        for (var tick = 0; tick <= yTicks; tick++)
        {
            var value = Interpolate(ymin, ymax, yTicks == 0 ? 0 : (double)tick / yTicks);
            var text = Text(FormatValue(value), MutedBrush);
            _labels.Add(new CellLabel(text, new Point(area.Left - text.Width - 6,
                area.Bottom - Scale(value, ystart, yend) * area.Height - text.Height / 2)));
        }
    }

    private void AddBatch(List<Rect> cells, IBrush brush)
    {
        var geometry = new StreamGeometry();
        using (var path = geometry.Open())
        {
            path.SetFillRule(FillRule.NonZero);
            foreach (var bounds in cells) AppendCell(path, bounds);
        }
        _batches.Add(new ColorBatch(geometry, brush));
    }

    private static void AppendCell(StreamGeometryContext path, Rect bounds)
    {
        path.BeginFigure(bounds.TopLeft, true);
        path.LineTo(bounds.TopRight);
        path.LineTo(bounds.BottomRight);
        path.LineTo(bounds.BottomLeft);
        path.EndFigure(true);
    }

    private static void AddInterval(Dictionary<double, (double Start, double End)> intervals,
        double coordinate, double start, double end)
    {
        if (intervals.TryGetValue(coordinate, out var previous))
            intervals[coordinate] = (Math.Min(previous.Start, start), Math.Max(previous.End, end));
        else intervals[coordinate] = (start, end);
    }

    private static bool IntervalsAreDisjoint(Dictionary<double, (double Start, double End)> intervals)
    {
        var ordered = intervals.Values.ToArray();
        Array.Sort(ordered, (first, last) => first.Start.CompareTo(last.Start));
        var end = double.NegativeInfinity;
        foreach (var interval in ordered)
        {
            if (interval.Start < end) return false;
            end = interval.End;
        }
        return true;
    }

    private protected override void DrawChartPlot(DrawingContext context, Rect plot)
    {
        foreach (var batch in _batches) context.DrawGeometry(batch.Brush, null, batch.Geometry);
        foreach (var cell in _overlapPaint)
        {
            context.DrawRectangle(cell.Brush, null, cell.Bounds);
            if (cell.Label is { } label) context.DrawText(label.Text, label.Position);
        }
        foreach (var label in _labels) context.DrawText(label.Text, label.Position);
        if (Selection is { } selected && _cells.TryGetValue((selected.SeriesIndex, selected.PointIndex), out var bounds))
            context.DrawRectangle(null, _selectionPen, bounds);
    }

    private protected override (int Series, int Index)? HitTestMark(Point point)
    {
        if (!_hitBuckets.TryGetValue(((int)Math.Floor(point.X / 32), (int)Math.Floor(point.Y / 32)), out var cells)) return null;
        for (var index = cells.Count - 1; index >= 0; index--)
            if (cells[index].Bounds.Contains(point)) return (cells[index].Series, cells[index].Index);
        return null;
    }

    private void AddHit(CellHit cell)
    {
        var left = (int)Math.Floor(cell.Bounds.Left / 32);
        var right = (int)Math.Floor(cell.Bounds.Right / 32);
        var top = (int)Math.Floor(cell.Bounds.Top / 32);
        var bottom = (int)Math.Floor(cell.Bounds.Bottom / 32);
        for (var y = top; y <= bottom; y++)
        for (var x = left; x <= right; x++)
        {
            if (!_hitBuckets.TryGetValue((x, y), out var cells)) _hitBuckets[(x, y)] = cells = [];
            cells.Add(cell);
        }
    }

    private IBrush[] BuildPalette(bool diverging)
    {
        var neutral = MidColor ?? (Background as ISolidColorBrush)?.Color ?? Colors.White;
        var low = LowColor ?? (diverging ? (GetChartBrush(1) as ISolidColorBrush)?.Color ?? Colors.SteelBlue : neutral);
        var high = HighColor ?? (GetChartBrush(0) as ISolidColorBrush)?.Color ?? Colors.Teal;
        var result = new IBrush[256];
        for (var index = 0; index < result.Length; index++)
        {
            var fraction = index / 255d;
            var first = diverging && fraction >= .5 ? neutral : low;
            var last = diverging && fraction < .5 ? neutral : high;
            var position = diverging ? fraction < .5 ? fraction * 2 : (fraction - .5) * 2 : fraction;
            result[index] = new SolidColorBrush(Color.FromArgb(Lerp(first.A, last.A, position),
                Lerp(first.R, last.R, position), Lerp(first.G, last.G, position), Lerp(first.B, last.B, position)));
        }
        return result;
    }

    private static byte Lerp(byte first, byte last, double fraction) => (byte)Math.Round(first + (last - first) * fraction);
    private static double Interpolate(double first, double last, double fraction) => first * (1 - fraction) + last * fraction;
    private static double Scale(double value, double first, double last)
    {
        if (first == last) return .5;
        var span = last - first;
        return Math.Clamp(double.IsFinite(span) ? (value - first) / span : (value / 2 - first / 2) / (last / 2 - first / 2), 0, 1);
    }

    private protected override ChartBase CreateExportSnapshot() => new HeatmapChart();
    private protected override void CopyExportProperties(ChartBase snapshot)
    {
        base.CopyExportProperties(snapshot);
        var target = (HeatmapChart)snapshot;
        target.CellWidth = CellWidth;
        target.CellHeight = CellHeight;
        target.CellGap = CellGap;
        target.ValueMinimum = ValueMinimum;
        target.ValueMaximum = ValueMaximum;
        target.LowColor = LowColor;
        target.MidColor = MidColor;
        target.HighColor = HighColor;
    }

    private sealed record ColorBatch(StreamGeometry Geometry, IBrush Brush);
    private readonly record struct PaintCell(Rect Bounds, int Color);
    private readonly record struct OrderedPaint(Rect Bounds, IBrush Brush, CellLabel? Label);
    private readonly record struct CellLabel(FormattedText Text, Point Position);
    private readonly record struct CellHit(int Series, int Index, Rect Bounds);
}
