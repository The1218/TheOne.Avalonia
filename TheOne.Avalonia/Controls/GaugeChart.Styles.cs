using global::Avalonia;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public sealed partial class GaugeChart
{
    private double GaugeStart => (double.IsNaN(GaugeStartAngle) ? GaugeStyle switch
        { GaugeStyle.Semicircle or GaugeStyle.Speed or GaugeStyle.Grade => 180, GaugeStyle.Temperature => 160, _ => 135 } : GaugeStartAngle) * Math.PI / 180;
    private double GaugeSweep => (double.IsNaN(GaugeSweepAngle) ? GaugeStyle switch
        { GaugeStyle.Semicircle or GaugeStyle.Speed or GaugeStyle.Grade => 180, GaugeStyle.Temperature => 220, _ => 270 } : GaugeSweepAngle) * Math.PI / 180;
    private bool GaugeTicksOutside => GaugeTickPlacement == GaugeTickPlacement.Outside ||
        (GaugeTickPlacement == GaugeTickPlacement.Auto && GaugeStyle is GaugeStyle.Semicircle or GaugeStyle.Temperature or GaugeStyle.Barometer);
    private IBrush GaugeAccent => AccentBrush ?? DefaultAccentBrush;
    private readonly record struct GaugeBand(double Start, double End, IBrush Brush, string? Label);

    private List<GaugeBand> GaugeBands()
    {
        var bands = new List<GaugeBand>();
        var previous = 0d;
        if (GaugeSegments is { Count: > 0 } segments)
        {
            // Out-of-order ends are skipped; a missing tail uses the track brush.
            foreach (var segment in segments)
            {
                if (segment is null || segment.Fraction <= previous) continue;
                bands.Add(new(previous, segment.Fraction, segment.Brush ?? GaugeAccent, segment.Label));
                previous = segment.Fraction;
            }
        }
        else if (GaugeStyle is GaugeStyle.StageSpeed or GaugeStyle.Grade)
        {
            var brushes = new[] { SecondaryBrush ?? GaugeAccent, TertiaryBrush ?? GaugeAccent, GaugeAccent, QuaternaryBrush ?? GaugeAccent };
            var count = GaugeStyle == GaugeStyle.Grade ? 4 : 3;
            for (var i = 0; i < count; i++)
                bands.Add(new(i / (double)count, (i + 1d) / count, brushes[i], null));
        }
        return bands;
    }
    private static double GaugeFraction(double value, double minimum, double maximum)
    {
        var clamped = Math.Clamp(value, minimum, maximum);
        var span = maximum - minimum;
        var scale = Math.Max(Math.Abs(minimum), Math.Abs(maximum));
        return Math.Clamp(double.IsFinite(span) ? (clamped - minimum) / span :
            (clamped / scale - minimum / scale) / (maximum / scale - minimum / scale), 0, 1);
    }
    private IBrush GaugeBandBrush(List<GaugeBand> bands, double fraction) =>
        bands.FirstOrDefault(band => fraction <= band.End && fraction >= band.Start).Brush ?? GaugeAccent;

    private void DrawGauge(DrawingContext? context, Rect plot)
    {
        // Keep existing semicircle pixel layout for consumers that have not opted into the new API.
        if (GaugeStyle == GaugeStyle.Semicircle && !GaugeVisualProperties.Any(property => property != GaugeStyleProperty &&
            (property != GaugeSegmentsProperty || GaugeSegments is { Count: > 0 }) && IsSet(property)))
        {
            DrawLegacyGauge(context, plot);
            return;
        }
        if (context is null) return;
        using var clip = context.PushClip(plot);
        if (Maximum <= Minimum)
        {
            DrawText(context, "最大值必须大于最小值", plot.Center, MutedBrush, centered: true);
            return;
        }
        var value = PresentGaugeValue(Value);
        var ratio = GaugeFraction(value, Minimum, Maximum);
        var bands = GaugeBands();
        var start = GaugeStart; var sweep = GaugeSweep;
        var tickCount = GaugeMajorTickCount;
        var ticks = ShowGaugeTicks;
        var outside = GaugeTicksOutside;
        var labelWidth = Enumerable.Range(0, tickCount).Max(i => GaugeScaleText((1 - i / (double)(tickCount - 1)) * Minimum + i / (double)(tickCount - 1) * Maximum, Minimum, Maximum, tickCount, MutedBrush).Width);
        if (GaugeStyle == GaugeStyle.Grade && ShowGaugeTickLabels && bands.Count > 0)
            labelWidth = Math.Max(labelWidth, bands.Max(band => Text(GaugeEllipsis(band.Label ?? "", plot.Width / 3)).Width));
        var margin = ticks && (outside || GaugeStyle == GaugeStyle.Grade) ? Math.Max(labelWidth / 2 + FontSize, FontSize * 2.5) : ChartPadding / 2 + 2;
        var horizontalMargin = Math.Min(margin, plot.Width / 3);
        var verticalMargin = Math.Min(GaugeStyle == GaugeStyle.Grade ? FontSize * 2.2 : margin, plot.Height / 3);
        var available = new Rect(plot.Left + horizontalMargin, plot.Top + verticalMargin,
            Math.Max(0, plot.Width - horizontalMargin * 2), Math.Max(0, plot.Height - verticalMargin * 2));
        // The extrema include the pivot as well as every cardinal point inside the arc.
        var points = new List<Point> { new(0, 0), RadialPoint(default, 1, start), RadialPoint(default, 1, start + sweep) };
        for (var i = -4; i <= 8; i++)
        {
            var angle = i * Math.PI / 2;
            if (angle >= start && angle <= start + sweep) points.Add(RadialPoint(default, 1, angle));
        }
        if (GaugeStyle == GaugeStyle.Barometer) points.Add(new Point(0, 1));
        var left = points.Min(p => p.X); var right = points.Max(p => p.X);
        var top = points.Min(p => p.Y); var bottom = points.Max(p => p.Y);
        var footer = sweep <= Math.PI + 0.001 && GaugeStyle != GaugeStyle.Grade ? FontSize * 3.5 : 0;
        var radius = Math.Min(available.Width / (right - left), Math.Max(0, available.Height - footer) / (bottom - top));
        var center = new Point(available.Center.X - (left + right) * radius / 2,
            available.Top + (available.Height - footer - (bottom - top) * radius) / 2 - top * radius);
        var gradeLabels = GaugeStyle == GaugeStyle.Grade && ticks && ShowGaugeTickLabels
            ? PrepareGradeLabels(bands, plot, start, sweep) : [];
        if (GaugeStyle == GaugeStyle.Grade) (center, radius) = FitGradeGeometry(plot, gradeLabels, left, top, right, bottom);
        if (radius < FontSize * 3)
        {
            if (ShowGaugeValue) DrawGaugeCaption(context, plot, plot.Center.X, plot.Center.Y - FontSize, FontSize, value, plot.Width - 4, GaugeValueBrush);
            return;
        }
        var bandScale = GaugeStyle switch { GaugeStyle.Basic => 0.22, GaugeStyle.Progress => 0.5, GaugeStyle.Grade => 0.12, GaugeStyle.Barometer => 0.08, _ => 1d };
        var inner = radius * (1 - GaugeBandRatio * bandScale);
        var track = GaugeTrackBrush ?? GridBrush ?? MutedBrush ?? Brushes.LightGray;
        context.DrawGeometry(track, null, AnnularSector(center, radius, inner, start, sweep));
        foreach (var band in bands)
            context.DrawGeometry(band.Brush, null, AnnularSector(center, radius, inner, start + band.Start * sweep, (band.End - band.Start) * sweep));
        var progress = ShowGaugeProgress ?? GaugeStyle is GaugeStyle.Semicircle or GaugeStyle.Speed or GaugeStyle.Progress or GaugeStyle.Temperature;
        if (progress && ratio > 0)
        {
            context.DrawGeometry(GaugeAccent, null, AnnularSector(center, radius, inner, start, ratio * sweep));
            if (GaugeStyle == GaugeStyle.Temperature)
                context.DrawGeometry(SecondaryBrush ?? GaugeAccent, null, AnnularSector(center, radius, radius - (radius - inner) * 0.3, start, ratio * sweep));
        }
        var occupied = new List<Rect>();
        if (ticks) DrawGaugeScale(context, plot, center, radius, inner, start, sweep, Minimum, Maximum, tickCount, outside,
            GaugeTickBrush ?? MutedBrush ?? Foreground ?? Brushes.Gray, bands, occupied, GaugeStyle != GaugeStyle.Grade);
        if (gradeLabels.Count > 0) DrawGradeLabels(context, plot, center, radius, gradeLabels, occupied);
        if (GaugeStyle == GaugeStyle.Barometer && GaugeSecondaryMaximum > GaugeSecondaryMinimum)
        {
            var secondaryRadius = radius - FontSize * 1.4;
            var secondaryBrush = GaugeSecondaryBrush ?? Foreground ?? GaugeAccent;
            context.DrawGeometry(secondaryBrush, null, AnnularSector(center, secondaryRadius, Math.Max(0, secondaryRadius - Math.Max(1, StrokeThickness * 0.4)), start, sweep));
            if (ticks) DrawGaugeScale(context, plot, center, secondaryRadius - 2, secondaryRadius - 2, start, sweep,
                GaugeSecondaryMinimum, GaugeSecondaryMaximum, GaugeSecondaryTickCount, false, secondaryBrush, [], occupied, true, secondary: true);
        }
        if (ShowGaugeNeedle) DrawStyledGaugeNeedle(context, center, radius, start + ratio * sweep, GaugeNeedleBrush ??
            (GaugeStyle is GaugeStyle.StageSpeed or GaugeStyle.Grade ? GaugeBandBrush(bands, ratio) : GaugeAccent));
        var valueSize = double.IsNaN(GaugeValueFontSize) ? FontSize * (GaugeStyle == GaugeStyle.Temperature ? 3.2 : GaugeStyle == GaugeStyle.Basic ? 1.6 : 2.4) : GaugeValueFontSize;
        valueSize = Math.Min(valueSize, radius * 0.42);
        var valueOffset = double.IsNaN(GaugeValueOffset) ? GaugeStyle switch
            { GaugeStyle.Temperature => -0.2, GaugeStyle.Grade => -0.3, GaugeStyle.StageSpeed => 0.25, GaugeStyle.Barometer => 0.8, GaugeStyle.Speed or GaugeStyle.Semicircle => 0.16, _ => 0.45 } : GaugeValueOffset;
        var valueTop = Math.Clamp(center.Y + valueOffset * radius - valueSize / 2, plot.Top, Math.Max(plot.Top, plot.Bottom - valueSize * 1.3));
        var valueWidth = Math.Max(0, Math.Min(plot.Width - ChartPadding, inner * 1.7));
        if (ShowGaugeValue) DrawGaugeCaption(context, plot, center.X, valueTop, valueSize, value, valueWidth,
            GaugeValueBrush ?? (GaugeStyle is GaugeStyle.Temperature or GaugeStyle.Grade or GaugeStyle.StageSpeed ? GaugeBandBrush(bands, ratio) : Foreground));
        if (!string.IsNullOrWhiteSpace(GaugeCaption))
        {
            var caption = Text(GaugeEllipsis(GaugeCaption, valueWidth), MutedBrush);
            var captionTop = double.IsNaN(GaugeCaptionOffset)
                ? GaugeStyle switch { GaugeStyle.Barometer => center.Y - radius * 0.25, GaugeStyle.Basic => valueTop - FontSize * 1.8, _ => valueTop + valueSize + FontSize * 0.7 }
                : center.Y + GaugeCaptionOffset * radius;
            captionTop = Math.Clamp(captionTop, plot.Top, Math.Max(plot.Top, plot.Bottom - caption.Height));
            if (captionTop + caption.Height <= plot.Bottom)
                context.DrawText(caption, new Point(center.X - caption.Width / 2, captionTop));
        }
    }

    private void DrawStyledGaugeNeedle(DrawingContext context, Point center, double radius, double angle, IBrush brush)
    {
        var shape = GaugeNeedleShape == GaugeNeedleShape.Auto ? GaugeStyle switch
            { GaugeStyle.Grade => GaugeNeedleShape.Marker, GaugeStyle.Progress or GaugeStyle.Barometer => GaugeNeedleShape.Line, _ => GaugeNeedleShape.Triangle } : GaugeNeedleShape;
        var width = double.IsNaN(GaugeNeedleWidth) ? Math.Max(2, StrokeThickness * (shape == GaugeNeedleShape.Line ? 0.55 : shape == GaugeNeedleShape.Marker ? 3 : 1.5)) : GaugeNeedleWidth;
        var direction = new Vector(Math.Cos(angle), Math.Sin(angle));
        var side = new Vector(-Math.Sin(angle), Math.Cos(angle)) * width / 2;
        var tip = center + direction * radius * GaugeNeedleLength;
        var tail = shape == GaugeNeedleShape.Marker ? tip - direction * Math.Max(width * 2, radius * 0.12) :
            center - direction * (GaugeStyle == GaugeStyle.Barometer ? radius * 0.1 : 0);
        if (shape == GaugeNeedleShape.Line) context.DrawLine(new Pen(brush, width) { LineCap = PenLineCap.Round }, tail, tip);
        else
        {
            var geometry = new StreamGeometry();
            using (var path = geometry.Open())
            {
                path.BeginFigure(tail + side, true); path.LineTo(tip); path.LineTo(tail - side); path.EndFigure(true);
            }
            context.DrawGeometry(brush, null, geometry);
        }
        if (ShowGaugeAnchor ?? GaugeStyle is GaugeStyle.Speed or GaugeStyle.Progress or GaugeStyle.Barometer)
            context.DrawEllipse(Background, new Pen(brush, SelectionThickness), center, MarkerRadius, MarkerRadius);
    }

    private FormattedText GaugeScaleText(double value, double minimum, double maximum, int count, IBrush? brush)
    {
        var name = TickText(value, (maximum - minimum) / (count - 1), ValueFormat);
        if (name.Length > 10 && ValueFormat is not (null or "0.##")) name = value.ToString("0.##E+0", System.Globalization.CultureInfo.CurrentCulture);
        return Text(name, brush, Math.Max(8, FontSize - 1));
    }
    private void DrawGaugeScale(DrawingContext context, Rect plot, Point center, double radius, double inner, double start, double sweep,
        double minimum, double maximum, int count, bool outside, IBrush brush, List<GaugeBand> bands, List<Rect> occupied, bool numericLabels, bool secondary = false)
    {
        var intervals = count - 1;
        var subdivisions = Math.Min(GaugeMinorTickCount + 1, Math.Max(1, (int)(sweep * radius / (intervals * Math.Max(5, FontSize * 0.5)))));
        var steps = intervals * subdivisions;
        var length = Math.Max(4, FontSize * 0.6);
        var tickRadius = outside ? radius + 3 : inner - 3;
        for (var i = 0; i <= steps; i++)
        {
            var fraction = i / (double)steps; var angle = start + fraction * sweep;
            var major = i % subdivisions == 0;
            var tickBrush = secondary ? brush : GaugeTickBrush ?? (GaugeStyle is GaugeStyle.StageSpeed or GaugeStyle.Grade && bands.Count > 0 ? GaugeBandBrush(bands, fraction) : brush);
            context.DrawLine(new Pen(tickBrush, Math.Max(0.75, SelectionThickness * (major ? 0.65 : 0.4))),
                RadialPoint(center, tickRadius, angle), RadialPoint(center, tickRadius + (outside ? 1 : -1) * length * (major ? 1 : 0.5), angle));
        }
        if (!numericLabels || !ShowGaugeTickLabels) return;
        void Label(int i)
        {
            var fraction = i / (double)intervals;
            var text = GaugeScaleText((1 - fraction) * minimum + fraction * maximum, minimum, maximum, count, brush);
            var angle = start + fraction * sweep;
            var extent = Math.Abs(Math.Cos(angle)) * text.Width / 2 + Math.Abs(Math.Sin(angle)) * text.Height / 2;
            var distance = tickRadius + (outside ? 1 : -1) * (length + 4 + extent);
            PlaceGaugeLabel(context, plot, center, distance, angle, text, occupied);
        }
        Label(0); Label(intervals);
        for (var i = 1; i < intervals; i++) Label(i);
    }
    private static void PlaceGaugeLabel(DrawingContext context, Rect plot, Point center, double radius, double angle, FormattedText text, List<Rect> occupied)
    {
        if (text.Width == 0 || text.Width + 4 > plot.Width || text.Height + 4 > plot.Height) return;
        var point = RadialPoint(center, radius, angle);
        var bounds = new Rect(Math.Clamp(point.X - text.Width / 2, plot.Left + 2, plot.Right - text.Width - 2),
            Math.Clamp(point.Y - text.Height / 2, plot.Top + 2, plot.Bottom - text.Height - 2), text.Width, text.Height);
        if (occupied.Any(previous => previous.Inflate(3).Intersects(bounds))) return;
        occupied.Add(bounds); context.DrawText(text, bounds.TopLeft);
    }
}
