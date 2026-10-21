using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>Cached OHLC candles on numeric X coordinates, with theme-aware rise/fall colors.</summary>
public sealed class CandlestickChart : StatisticalChartBase
{
    public static readonly StyledProperty<IBrush?> UpBrushProperty = AvaloniaProperty.Register<CandlestickChart, IBrush?>(nameof(UpBrush));
    public static readonly StyledProperty<IBrush?> DownBrushProperty = AvaloniaProperty.Register<CandlestickChart, IBrush?>(nameof(DownBrush));
    public static readonly StyledProperty<bool> IsHollowUpProperty = AvaloniaProperty.Register<CandlestickChart, bool>(nameof(IsHollowUp));
    private readonly List<CandleGlyph> _glyphs = [];
    private readonly Dictionary<(int, int), CandleGlyph> _lookup = [];
    private Pen? _upPen, _downPen, _selectionPen;

    public override ChartKind Kind => ChartKind.Candlestick;
    public IBrush? UpBrush { get => GetValue(UpBrushProperty); set => SetValue(UpBrushProperty, value); }
    public IBrush? DownBrush { get => GetValue(DownBrushProperty); set => SetValue(DownBrushProperty, value); }
    public bool IsHollowUp { get => GetValue(IsHollowUpProperty); set => SetValue(IsHollowUpProperty, value); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == UpBrushProperty || change.Property == DownBrushProperty || change.Property == IsHollowUpProperty) InvalidateChartGeometry();
    }

    private protected override bool IsValidDataPoint(ChartPoint point) =>
        point is CandlestickPoint candle && double.IsFinite(candle.X) && double.IsFinite(candle.Open) && double.IsFinite(candle.Close) &&
        double.IsFinite(candle.Low) && double.IsFinite(candle.High) && candle.Low <= Math.Min(candle.Open, candle.Close) &&
        candle.High >= Math.Max(candle.Open, candle.Close);
    private protected override bool TryGetObservationRange(ChartPoint point, out double minimum, out double maximum)
    {
        minimum = maximum = 0;
        if (!IsValidDataPoint(point) || point is not CandlestickPoint candle) return false;
        minimum = candle.Low; maximum = candle.High; return true;
    }
    private protected override string? DescribeSpecializedPoint(ChartSelection selection)
    {
        if (selection.Point is not CandlestickPoint candle) return null;
        var series = selection.Series;
        return $"{series.Name} · {candle.Label ?? FormatValue(candle.X)}: O {FormatChartValue(series, candle.Open)} / C {FormatChartValue(series, candle.Close)} / L {FormatChartValue(series, candle.Low)} / H {FormatChartValue(series, candle.High)}{series.Unit ?? Unit}";
    }
    private protected override void ClearStatisticalGlyphs()
    {
        _glyphs.Clear(); _lookup.Clear();
        _upPen = new Pen(UpBrush ?? GetChartBrush(0), StrokeThickness);
        _downPen = new Pen(DownBrush ?? GetChartBrush(1), StrokeThickness);
        _selectionPen = new Pen(Foreground ?? GetChartBrush(0), SelectionThickness);
    }
    private protected override void PrepareStatisticalGlyph(int series, int index, ChartPoint point, double x, double width)
    {
        var candle = (CandlestickPoint)point;
        var low = MapStatisticalValue(candle.Low); var high = MapStatisticalValue(candle.High);
        var open = MapStatisticalValue(candle.Open); var close = MapStatisticalValue(candle.Close);
        var body = new Rect(x - width / 2, Math.Min(open, close), width, Math.Abs(close - open));
        var up = candle.Close >= candle.Open;
        var pen = up ? _upPen! : _downPen!;
        var path = new StreamGeometry();
        using (var drawing = path.Open())
        {
            drawing.BeginFigure(new Point(x, low), false); drawing.LineTo(new Point(x, body.Bottom)); drawing.EndFigure(false);
            drawing.BeginFigure(new Point(x, high), false); drawing.LineTo(new Point(x, body.Top)); drawing.EndFigure(false);
            if (open == close) { drawing.BeginFigure(new Point(x - width / 2, open), false); drawing.LineTo(new Point(x + width / 2, open)); drawing.EndFigure(false); }
        }
        var bounds = new Rect(x - Math.Max(width / 2, 3), Math.Min(high, low) - 3, Math.Max(width, 6), Math.Abs(high - low) + 6);
        FormattedText? label = null; var position = default(Point);
        if (ShowDataLabels && width >= FontSize * 2)
        {
            var candidate = Text(FormatChartValue(SeriesSource![series], candle.Close));
            if (candidate.Width + 4 <= width) { label = candidate; position = new Point(x - candidate.Width / 2, body.Top - candidate.Height - 3); }
        }
        var glyph = new CandleGlyph(series, index, x, body, bounds, low, high, path, pen, up && IsHollowUp, label, position);
        _glyphs.Add(glyph); _lookup[(series, index)] = glyph;
        if (GetStatisticalAnchor(x, close, body, low, high) is { } anchor)
            RegisterStatisticalMark(series, index, x, bounds, anchor);
    }
    private protected override void DrawStatisticalGlyphs(DrawingContext context)
    {
        foreach (var glyph in _glyphs)
        {
            context.DrawGeometry(null, glyph.Pen, glyph.Wick);
            context.DrawRectangle(glyph.Hollow ? null : glyph.Pen.Brush, glyph.Pen, glyph.Body);
            if (glyph.Label is not null) context.DrawText(glyph.Label, glyph.LabelPosition);
        }
        if (Selection is { } selected && _lookup.TryGetValue((selected.SeriesIndex, selected.PointIndex), out var active))
            context.DrawRectangle(null, _selectionPen, active.Bounds);
    }
    private protected override bool ContainsStatisticalGlyph(int series, int index, Point point)
    {
        if (!_lookup.TryGetValue((series, index), out var glyph)) return false;
        return glyph.Body.Inflate(2).Contains(point) || Math.Abs(point.X - glyph.X) <= 4 &&
            point.Y >= Math.Min(glyph.Low, glyph.High) - 3 && point.Y <= Math.Max(glyph.Low, glyph.High) + 3;
    }
    private protected override ChartBase CreateExportSnapshot() => new CandlestickChart();
    private protected override void CopyExportProperties(ChartBase snapshot)
    {
        base.CopyExportProperties(snapshot);
        var target = (CandlestickChart)snapshot; target.UpBrush = UpBrush; target.DownBrush = DownBrush; target.IsHollowUp = IsHollowUp;
    }
    private sealed record CandleGlyph(int Series, int Index, double X, Rect Body, Rect Bounds, double Low, double High,
        StreamGeometry Wick, Pen Pen, bool Hollow, FormattedText? Label, Point LabelPosition);
}
