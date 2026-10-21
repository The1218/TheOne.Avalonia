using System.Collections.Specialized;
using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>Precomputed five-number summaries with observable outliers, cached geometry and numeric axes.</summary>
public sealed class BoxPlotChart : StatisticalChartBase
{
    public static readonly StyledProperty<bool> ShowOutliersProperty = AvaloniaProperty.Register<BoxPlotChart, bool>(nameof(ShowOutliers), true);
    public static readonly StyledProperty<double> OutlierRadiusProperty = AvaloniaProperty.Register<BoxPlotChart, double>(
        nameof(OutlierRadius), 3, validate: value => double.IsFinite(value) && value > 0 && value <= 32);

    private readonly Dictionary<BoxPlotPoint, IList<double>?> _outlierSources = new(ReferenceEqualityComparer.Instance);
    private readonly Dictionary<INotifyCollectionChanged, int> _outlierCollections = new(ReferenceEqualityComparer.Instance);
    private readonly List<BoxGlyph> _glyphs = [];
    private readonly Dictionary<(int, int), BoxGlyph> _lookup = [];
    private int _animationMarks;
    private readonly Dictionary<int, Pen> _pens = [];
    private Pen? _selectionPen;

    public override ChartKind Kind => ChartKind.BoxPlot;
    private protected override int CustomChartAnimationMarkCount => _animationMarks;
    public bool ShowOutliers { get => GetValue(ShowOutliersProperty); set => SetValue(ShowOutliersProperty, value); }
    public double OutlierRadius { get => GetValue(OutlierRadiusProperty); set => SetValue(OutlierRadiusProperty, value); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ShowOutliersProperty || change.Property == OutlierRadiusProperty) InvalidateChartGeometry();
    }

    private protected override void OnChartDataObjectSubscribed(AvaloniaObject source)
    {
        base.OnChartDataObjectSubscribed(source);
        if (source is not BoxPlotPoint point) return;
        _outlierSources[point] = point.Outliers;
        AddOutliers(point.Outliers);
        point.PropertyChanged += PointChanged;
    }

    private protected override void OnChartDataObjectUnsubscribed(AvaloniaObject source)
    {
        if (source is BoxPlotPoint point && _outlierSources.Remove(point, out var previous))
        {
            point.PropertyChanged -= PointChanged;
            RemoveOutliers(previous);
        }
        base.OnChartDataObjectUnsubscribed(source);
    }

    private void PointChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property != BoxPlotPoint.OutliersProperty || sender is not BoxPlotPoint point || !_outlierSources.TryGetValue(point, out var old)) return;
        AddOutliers(point.Outliers);
        _outlierSources[point] = point.Outliers;
        RemoveOutliers(old);
    }
    private void AddOutliers(IList<double>? source)
    {
        if (source is not INotifyCollectionChanged observable) return;
        if (_outlierCollections.TryGetValue(observable, out var count)) _outlierCollections[observable] = count + 1;
        else { _outlierCollections.Add(observable, 1); observable.CollectionChanged += OutliersChanged; }
    }
    private void RemoveOutliers(IList<double>? source)
    {
        if (source is not INotifyCollectionChanged observable || !_outlierCollections.TryGetValue(observable, out var count)) return;
        if (count > 1) _outlierCollections[observable] = count - 1;
        else { _outlierCollections.Remove(observable); observable.CollectionChanged -= OutliersChanged; }
    }
    private void OutliersChanged(object? sender, NotifyCollectionChangedEventArgs args) => NotifyChartDataChanged();

    private protected override bool IsValidDataPoint(ChartPoint point) =>
        point is BoxPlotPoint box && double.IsFinite(box.X) && double.IsFinite(box.Minimum) &&
        double.IsFinite(box.LowerQuartile) && double.IsFinite(box.Median) && double.IsFinite(box.UpperQuartile) &&
        double.IsFinite(box.Maximum) && box.Minimum <= box.LowerQuartile && box.LowerQuartile <= box.Median &&
        box.Median <= box.UpperQuartile && box.UpperQuartile <= box.Maximum;

    private protected override bool TryGetObservationRange(ChartPoint point, out double minimum, out double maximum)
    {
        minimum = maximum = 0;
        if (!IsValidDataPoint(point) || point is not BoxPlotPoint box) return false;
        minimum = box.Minimum; maximum = box.Maximum;
        if (ShowOutliers && box.Outliers is { } outliers)
            foreach (var value in outliers)
                if (double.IsFinite(value)) { minimum = Math.Min(minimum, value); maximum = Math.Max(maximum, value); }
        return true;
    }

    private protected override string? DescribeSpecializedPoint(ChartSelection selection)
    {
        if (selection.Point is not BoxPlotPoint box) return null;
        var series = selection.Series;
        return $"{series.Name} · {box.Label ?? FormatValue(box.X)}: {FormatChartValue(series, box.Minimum)} / {FormatChartValue(series, box.LowerQuartile)} / {FormatChartValue(series, box.Median)} / {FormatChartValue(series, box.UpperQuartile)} / {FormatChartValue(series, box.Maximum)}{series.Unit ?? Unit}";
    }

    private protected override void ClearStatisticalGlyphs()
    {
        _animationMarks = 0;
        _glyphs.Clear(); _lookup.Clear(); _pens.Clear();
        _selectionPen = new Pen(Foreground ?? GetChartBrush(0), SelectionThickness);
    }

    private protected override void PrepareStatisticalGlyph(int series, int index, ChartPoint point, double x, double width)
    {
        var box = (BoxPlotPoint)point;
        var low = MapStatisticalValue(box.Minimum); var high = MapStatisticalValue(box.Maximum);
        var q1 = MapStatisticalValue(box.LowerQuartile); var q3 = MapStatisticalValue(box.UpperQuartile);
        var median = MapStatisticalValue(box.Median);
        var body = new Rect(x - width / 2, Math.Min(q1, q3), width, Math.Abs(q3 - q1));
        if (!_pens.TryGetValue(series, out var pen)) _pens[series] = pen = new Pen(GetChartBrush(series), StrokeThickness);
        var whiskers = new StreamGeometry();
        using (var path = whiskers.Open())
        {
            Line(path, new Point(x, low), new Point(x, q1));
            Line(path, new Point(x, q3), new Point(x, high));
            Line(path, new Point(x - width / 4, low), new Point(x + width / 4, low));
            Line(path, new Point(x - width / 4, high), new Point(x + width / 4, high));
            Line(path, new Point(x - width / 2, median), new Point(x + width / 2, median));
        }
        var outliers = new List<Point>();
        var bottom = Math.Max(low, high); var top = Math.Min(low, high);
        if (ShowOutliers && box.Outliers is { } values)
            foreach (var value in values)
                if (double.IsFinite(value))
                {
                    var y = MapStatisticalValue(value);
                    top = Math.Min(top, y - OutlierRadius); bottom = Math.Max(bottom, y + OutlierRadius);
                    if (y + OutlierRadius >= DataPlot.Top && y - OutlierRadius <= DataPlot.Bottom) outliers.Add(new Point(x, y));
                }
        var half = Math.Max(width / 2, ShowOutliers ? OutlierRadius : 0);
        var bounds = new Rect(x - half - 3, top - 3, half * 2 + 6, Math.Max(0, bottom - top) + 6);
        FormattedText? label = null;
        var labelPosition = default(Point);
        if (ShowDataLabels && width >= FontSize * 2)
        {
            var candidate = Text(FormatChartValue(SeriesSource![series], box.Median));
            if (candidate.Width + 4 <= width)
            { label = candidate; labelPosition = new Point(x - candidate.Width / 2, body.Top - candidate.Height - 3); }
        }
        var glyph = new BoxGlyph(series, index, x, body, bounds, low, high, median, whiskers, pen, outliers.ToArray(), label, labelPosition);
        _glyphs.Add(glyph); _lookup[(series, index)] = glyph;
        var anchor = GetStatisticalAnchor(x, median, body, low, high);
        if (anchor is null)
            foreach (var outlier in glyph.Outliers)
            {
                var candidate = new Point(Math.Clamp(outlier.X, DataPlot.Left, DataPlot.Right), Math.Clamp(outlier.Y, DataPlot.Top, DataPlot.Bottom));
                if (new Vector(candidate.X - outlier.X, candidate.Y - outlier.Y).Length <= OutlierRadius)
                { anchor = candidate; break; }
            }
        if (anchor is { } position) RegisterStatisticalMark(series, index, x, bounds, position);
        _animationMarks = (int)Math.Min(6001L, (long)_animationMarks + 1 + glyph.Outliers.Length);
    }

    private protected override void DrawStatisticalGlyphs(DrawingContext context)
    {
        foreach (var glyph in _glyphs)
        {
            using (context.PushOpacity(.16)) context.DrawRectangle(glyph.Pen.Brush, null, glyph.Body);
            context.DrawRectangle(null, glyph.Pen, glyph.Body);
            context.DrawGeometry(null, glyph.Pen, glyph.Whiskers);
            foreach (var point in glyph.Outliers) context.DrawEllipse(null, glyph.Pen, point, OutlierRadius, OutlierRadius);
            if (glyph.Label is not null) context.DrawText(glyph.Label, glyph.LabelPosition);
        }
        if (Selection is { } selected && _lookup.TryGetValue((selected.SeriesIndex, selected.PointIndex), out var active))
            context.DrawRectangle(null, _selectionPen, active.Bounds);
    }

    private protected override bool ContainsStatisticalGlyph(int series, int index, Point point)
    {
        if (!_lookup.TryGetValue((series, index), out var glyph)) return false;
        if (glyph.Body.Inflate(2).Contains(point)) return true;
        if (Math.Abs(point.X - glyph.X) <= glyph.Body.Width / 4 + 3 &&
            (Math.Abs(point.Y - glyph.Low) <= 3 || Math.Abs(point.Y - glyph.High) <= 3)) return true;
        if (Math.Abs(point.X - glyph.X) <= 4 && point.Y >= Math.Min(glyph.Low, glyph.High) - 3 && point.Y <= Math.Max(glyph.Low, glyph.High) + 3) return true;
        foreach (var outlier in glyph.Outliers)
            if (new Vector(point.X - outlier.X, point.Y - outlier.Y).Length <= OutlierRadius + 3) return true;
        return false;
    }

    private static void Line(StreamGeometryContext path, Point from, Point to)
    { path.BeginFigure(from, false); path.LineTo(to); path.EndFigure(false); }

    private protected override ChartBase CreateExportSnapshot() => new BoxPlotChart();
    private protected override void CopyExportProperties(ChartBase snapshot)
    {
        base.CopyExportProperties(snapshot);
        var target = (BoxPlotChart)snapshot; target.ShowOutliers = ShowOutliers; target.OutlierRadius = OutlierRadius;
    }
    private sealed record BoxGlyph(int Series, int Index, double X, Rect Body, Rect Bounds, double Low, double High,
        double Median, StreamGeometry Whiskers, Pen Pen, Point[] Outliers, FormattedText? Label, Point LabelPosition);
}
