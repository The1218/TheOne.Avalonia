using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>A native funnel chart. Source order is retained unless sorting is explicitly requested.</summary>
public sealed class FunnelChart : SpecializedChartBase
{
    public static readonly StyledProperty<FunnelSortMode> SortModeProperty =
        AvaloniaProperty.Register<FunnelChart, FunnelSortMode>(nameof(SortMode),
            validate: value => value is FunnelSortMode.None or FunnelSortMode.Ascending or FunnelSortMode.Descending);
    public static readonly StyledProperty<double> GapProperty =
        AvaloniaProperty.Register<FunnelChart, double>(nameof(Gap), 4,
            validate: value => double.IsFinite(value) && value >= 0);
    public static readonly StyledProperty<double> MinimumWidthRatioProperty =
        AvaloniaProperty.Register<FunnelChart, double>(nameof(MinimumWidthRatio),
            validate: value => double.IsFinite(value) && value >= 0 && value <= 1);

    private readonly List<FunnelStage> _stages = [];
    private readonly List<FunnelLabel> _labels = [];
    private readonly Dictionary<(int Series, int Index), FunnelStage> _stageLookup = [];
    private Pen? _selectionPen;
    private Rect _plotBounds;

    public override ChartKind Kind => ChartKind.Funnel;

    private protected override bool UseDefaultMarkHitTesting => false;
    public FunnelSortMode SortMode { get => GetValue(SortModeProperty); set => SetValue(SortModeProperty, value); }
    /// <summary>The vertical gap between stages, in logical pixels.</summary>
    public double Gap { get => GetValue(GapProperty); set => SetValue(GapProperty, value); }
    /// <summary>Optional minimum stage width relative to the widest stage. Zero preserves the value ratio.</summary>
    public double MinimumWidthRatio { get => GetValue(MinimumWidthRatioProperty); set => SetValue(MinimumWidthRatioProperty, value); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SortModeProperty || change.Property == GapProperty || change.Property == MinimumWidthRatioProperty)
            InvalidateChartGeometry();
    }

    private protected override bool IsValidDataPoint(ChartPoint point) =>
        double.IsFinite(point.X) && double.IsFinite(point.Y) && point.Y >= 0;

    private protected override string? DescribeSpecializedPoint(ChartSelection selection) =>
        $"{selection.Series.Name} · {selection.Point.Label ?? FormatValue(selection.Point.X)}: {FormatChartValue(selection.Series, selection.Point.Y)}{selection.Series.Unit ?? Unit}";

    private protected override void PrepareSpecializedGeometry(Rect plot)
    {
        _stages.Clear();
        _stageLookup.Clear();
        _labels.Clear();
        _plotBounds = plot;
        _selectionPen = new Pen(Foreground ?? GetChartBrush(0), SelectionThickness);
        if (SeriesSource is not { } source || plot.Width <= 0 || plot.Height <= 0) return;
        var visible = 0;
        foreach (var series in source)
            if (series.IsVisible && series.ItemsSource is { Count: > 0 }) visible++;
        if (visible == 0) return;
        var columnGap = Math.Min(16, plot.Width / (visible * 4d));
        var width = Math.Max(0, (plot.Width - columnGap * (visible - 1)) / visible);
        var column = 0;
        for (var seriesIndex = 0; seriesIndex < source.Count; seriesIndex++)
        {
            var series = source[seriesIndex];
            if (!series.IsVisible || series.ItemsSource is not { Count: > 0 } points) continue;
            var entries = new List<FunnelEntry>(points.Count);
            var maximum = 0d;
            for (var index = 0; index < points.Count; index++)
                if (IsValidDataPoint(points[index]))
                {
                    entries.Add(new FunnelEntry(index, points[index]));
                    maximum = Math.Max(maximum, points[index].Y);
                }
            var centerX = plot.Left + column * (width + columnGap) + width / 2;
            column++;
            if (entries.Count == 0 || maximum <= 0 || width <= 0) continue;
            if (SortMode != FunnelSortMode.None)
                entries.Sort((first, last) =>
                {
                    var compared = first.Point.Y.CompareTo(last.Point.Y);
                    if (SortMode == FunnelSortMode.Descending) compared = -compared;
                    return compared == 0 ? first.Index.CompareTo(last.Index) : compared;
                });
            var gap = entries.Count <= 1 ? 0 : Math.Min(Gap, plot.Height / entries.Count * .4);
            var height = (plot.Height - gap * (entries.Count - 1)) / entries.Count;
            if (!double.IsFinite(height) || height <= 0) continue;
            for (var position = 0; position < entries.Count; position++)
            {
                var entry = entries[position];
                var topWidth = width * Math.Max(MinimumWidthRatio, entry.Point.Y / maximum);
                var next = position + 1 < entries.Count ? entries[position + 1].Point.Y / maximum : 0;
                // A zero-valued stage must not gain area from the following nonzero stage.
                var bottomWidth = entry.Point.Y == 0 && MinimumWidthRatio == 0
                    ? 0 : width * Math.Max(MinimumWidthRatio, next);
                var top = plot.Top + position * (height + gap);
                var a = new Point(centerX - topWidth / 2, top);
                var b = new Point(centerX + topWidth / 2, top);
                var c = new Point(centerX + bottomWidth / 2, top + height);
                var d = new Point(centerX - bottomWidth / 2, top + height);
                var geometry = new StreamGeometry();
                using (var path = geometry.Open())
                {
                    path.BeginFigure(a, true);
                    path.LineTo(b);
                    path.LineTo(c);
                    path.LineTo(d);
                    path.EndFigure(true);
                }
                var bounds = new Rect(new Point(Math.Min(a.X, d.X), top),
                    new Size(Math.Max(topWidth, bottomWidth), height));
                var stage = new FunnelStage(seriesIndex, entry.Index, geometry, GetChartBrush(seriesIndex, entry.Index),
                    bounds, topWidth, bottomWidth);
                _stages.Add(stage);
                _stageLookup[(seriesIndex, entry.Index)] = stage;
                if (bounds.Width <= 0) continue;
                RegisterMark(seriesIndex, entry.Index, bounds.Center, bounds);
                if (ShowDataLabels)
                {
                    var label = Text($"{entry.Point.Label ?? FormatValue(entry.Point.X)} · {FormatChartValue(series, entry.Point.Y)}");
                    var middleWidth = (topWidth + bottomWidth) / 2;
                    if (label.Width + 8 <= middleWidth && label.Height + 2 <= height)
                        _labels.Add(new FunnelLabel(label, bounds.Center - new Vector(label.Width / 2, label.Height / 2)));
                }
            }
        }
    }

    private protected override void DrawChartPlot(DrawingContext context, Rect plot)
    {
        using var clip = context.PushClip(plot);
        foreach (var stage in _stages) context.DrawGeometry(stage.Brush, null, stage.Geometry);
        foreach (var label in _labels) context.DrawText(label.Text, label.Position);
        if (Selection is { } selected && _stageLookup.TryGetValue((selected.SeriesIndex, selected.PointIndex), out var active))
            context.DrawGeometry(null, _selectionPen, active.Geometry);
    }

    private protected override (int Series, int Index)? HitTestMark(Point point)
    {
        if (!_plotBounds.Contains(point)) return null;
        for (var index = _stages.Count - 1; index >= 0; index--)
        {
            var stage = _stages[index];
            if (!stage.Bounds.Contains(point) || stage.Bounds.Height <= 0) continue;
            var fraction = (point.Y - stage.Bounds.Top) / stage.Bounds.Height;
            var halfWidth = (stage.TopWidth * (1 - fraction) + stage.BottomWidth * fraction) / 2;
            if (Math.Abs(point.X - stage.Bounds.Center.X) <= halfWidth) return (stage.Series, stage.Index);
        }
        return null;
    }

    private protected override ChartBase CreateExportSnapshot() => new FunnelChart();
    private protected override void CopyExportProperties(ChartBase snapshot)
    {
        base.CopyExportProperties(snapshot);
        var target = (FunnelChart)snapshot;
        target.SortMode = SortMode;
        target.Gap = Gap;
        target.MinimumWidthRatio = MinimumWidthRatio;
    }

    private sealed record FunnelEntry(int Index, ChartPoint Point);
    private sealed record FunnelStage(int Series, int Index, StreamGeometry Geometry, IBrush Brush, Rect Bounds,
        double TopWidth, double BottomWidth);
    private readonly record struct FunnelLabel(FormattedText Text, Point Position);
}
