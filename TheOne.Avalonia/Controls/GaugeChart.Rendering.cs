using System.Globalization;
using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public sealed partial class GaugeChart
{
    private static readonly FontFeatureCollection GaugeNumberFeatures = FontFeatureCollection.Parse("tnum");
    private int _gaugeAnimatedPrecision;

    private void DrawLegacyGauge(DrawingContext? context, Rect plot)
    {
        if (context is null) return;
        using var clip = context.PushClip(plot);
        if (Maximum <= Minimum)
        {
            DrawText(context, "最大值必须大于最小值", new Point(plot.Center.X, plot.Center.Y - FontSize / 2), MutedBrush,
                centered: true);
            return;
        }

        var displayedValue = PresentGaugeValue(Value);
        var valueSize = Math.Max(FontSize, Math.Min(30, FontSize * 2));
        var footer = ShowGaugeNeedle ? valueSize + ChartPadding + 10 : FontSize + ChartPadding + 6;
        var topSpace = ShowGaugeTicks ? FontSize * 1.75 + ChartPadding : ChartPadding / 2;
        var sideSpace = ShowGaugeTicks
            ? Math.Max(topSpace,
                Math.Max(GaugeTickText(Minimum).Width, GaugeTickText(Maximum).Width) + FontSize * 0.65 + 6)
            : topSpace;
        var radius = Math.Min(Math.Max(0, (plot.Width - sideSpace * 2) / 2),
            Math.Max(0, plot.Height - footer - topSpace));
        if (radius < FontSize * (ShowGaugeNeedle ? 2 : 4))
        {
            var top = plot.Center.Y - FontSize;
            DrawGaugeCaption(context, plot, plot.Center.X, top, FontSize, displayedValue,
                Math.Max(0, plot.Width - ChartPadding));
            return;
        }

        var center = new Point(plot.Center.X,
            plot.Top + topSpace + (plot.Height - radius - footer - topSpace) / 2 + radius);
        var inner = radius * (1 - GaugeBandRatio);
        var clamped = Math.Clamp(displayedValue, Minimum, Maximum);
        var span = Maximum - Minimum;
        var scale = Math.Max(Math.Abs(Minimum), Math.Abs(Maximum));
        var ratio = double.IsFinite(span)
            ? (clamped - Minimum) / span
            : (clamped / scale - Minimum / scale) / (Maximum / scale - Minimum / scale);
        ratio = Math.Clamp(ratio, 0, 1);
        context.DrawGeometry(GridBrush ?? MutedBrush ?? Brushes.LightGray, null,
            AnnularSector(center, radius, inner, Math.PI, Math.PI));
        if (ratio > 0)
            context.DrawGeometry(AccentBrush ?? DefaultAccentBrush, null,
                AnnularSector(center, radius, inner, Math.PI, Math.PI * ratio));
        if (ShowGaugeTicks) DrawGaugeTickMarks(context, center, radius);
        if (ShowGaugeNeedle)
        {
            var angle = Math.PI + ratio * Math.PI;
            var tip = RadialPoint(center, Math.Max(0, inner - ChartPadding / 2), angle);
            var side = new Vector(-Math.Sin(angle), Math.Cos(angle)) * Math.Max(1, StrokeThickness);
            var needle = new StreamGeometry();
            using (var path = needle.Open())
            {
                path.BeginFigure(center + side);
                path.LineTo(tip);
                path.LineTo(center - side);
                path.EndFigure(true);
            }

            context.DrawGeometry(Foreground, null, needle);
            context.DrawEllipse(Background, new Pen(Foreground, SelectionThickness), center, MarkerRadius * 1.2,
                MarkerRadius * 1.2);
        }

        if (ShowGaugeTicks) DrawGaugeTickLabels(context, plot, center, radius);
        valueSize = Math.Max(FontSize, Math.Min(valueSize, radius * 0.4));
        var valueY = ShowGaugeNeedle ? center.Y + ChartPadding / 2 + MarkerRadius : center.Y - valueSize - ChartPadding;
        var captionWidth = Math.Max(0, plot.Width - ChartPadding * 2);
        var showEndpoints = !ShowGaugeTicks;
        if (ShowGaugeNeedle && !ShowGaugeTicks)
        {
            var minimum = Text(FormatValue(Minimum), MutedBrush);
            var maximum = Text(FormatValue(Maximum), MutedBrush);
            if (minimum.Width + maximum.Width + ChartPadding * 2 < plot.Width)
            {
                var reserved = Math.Max(0,
                    2 * Math.Min(radius - minimum.Width - ChartPadding, radius - maximum.Width - ChartPadding));
                if (reserved >= FontSize * 4) captionWidth = Math.Min(captionWidth, reserved);
                else showEndpoints = false;
            }
        }

        DrawGaugeCaption(context, plot, center.X, valueY, valueSize, displayedValue, captionWidth);
        if (showEndpoints)
        {
            var minimum = Text(FormatValue(Minimum), MutedBrush);
            var maximum = Text(FormatValue(Maximum), MutedBrush);
            if (minimum.Width + maximum.Width + ChartPadding * 2 < plot.Width)
            {
                context.DrawText(minimum, new Point(Math.Max(plot.Left, center.X - radius), center.Y + 8));
                context.DrawText(maximum,
                    new Point(Math.Min(plot.Right - maximum.Width, center.X + radius - maximum.Width), center.Y + 8));
            }
        }
    }

    private FormattedText GaugeNumberText(string name, IBrush? brush, double size)
    {
        var text = Text(name, brush, size);
        text.SetFontFeatures(GaugeNumberFeatures);
        return text;
    }

    private string FormatGaugeNumber(double value)
    {
        // Interpolation must not invent fractional digits for an integer slider target.
        // Optional decimals use a constant precision throughout an animated update.
        if (IsGaugeValueAnimating && ValueFormat == "0.##")
        {
            if (Value != Math.Truncate(Value)) _gaugeAnimatedPrecision = 2;
            return value.ToString(_gaugeAnimatedPrecision == 0 ? "F0" : "F2", CultureInfo.CurrentCulture);
        }

        return FormatValue(value);
    }

    private void DrawGaugeCaption(DrawingContext context, Rect plot, double centerX, double top, double size,
        double value, double width, IBrush? brush = null)
    {
        if (width <= 0) return;
        var minimumSize = Math.Min(FontSize, size);

        string FitName(double number)
        {
            var name = ValueFormat == "0.##"
                ? number.ToString("0.00", CultureInfo.CurrentCulture)
                : FormatValue(number);
            return name.Length > 18 ? number.ToString("0.###E+0", CultureInfo.CurrentCulture) : name;
        }

        var candidates = new[] { FitName(Minimum), FitName(Maximum), FitName(Value) };
        var gap = string.IsNullOrEmpty(Unit) ? 0 : Math.Max(3, FontSize * 0.25);

        double RequiredWidth(double fontSize)
        {
            var numberWidth = candidates.Max(name => GaugeNumberText(name, brush, fontSize).Width);
            var unitWidth = Text(GaugeEllipsis(Unit ?? "", width * 0.4, fontSize), brush ?? MutedBrush, fontSize).Width;
            return numberWidth + (unitWidth > 0 ? gap : 0) + unitWidth;
        }

        // Fit against the range and target, never the changing interpolated frame.
        if (RequiredWidth(size) > width)
        {
            var low = minimumSize;
            var high = size;
            for (var iteration = 0; iteration < 10; iteration++)
            {
                var middle = (low + high) / 2;
                if (RequiredWidth(middle) <= width) low = middle;
                else high = middle;
            }

            size = low;
        }

        var unit = Text(GaugeEllipsis(Unit ?? "", width * 0.4, size), brush ?? MutedBrush, size);
        gap = unit.Width > 0 ? gap : 0;
        var numberSpace = Math.Max(0, width - unit.Width - gap);
        var name = FormatGaugeNumber(value);
        var targetName = FormatGaugeNumber(Value);
        if (GaugeNumberText(targetName, brush, size).Width > numberSpace)
        {
            name = value.ToString("0.###E+0", CultureInfo.CurrentCulture);
            targetName = Value.ToString("0.###E+0", CultureInfo.CurrentCulture);
        }

        var text = GaugeNumberText(name, brush, size);
        var reference = GaugeNumberText(targetName, brush, size);
        var baseline = Math.Max(reference.Baseline, unit.Baseline);
        var left = centerX - (reference.Width + gap + unit.Width) / 2;
        var numberRight = left + reference.Width;
        using var clip = context.PushClip(new Rect(centerX - width / 2, plot.Top, width, plot.Height));
        context.DrawText(text,
            new Point(Math.Round(numberRight - text.Width), Math.Round(top + baseline - text.Baseline)));
        if (unit.Width > 0)
            context.DrawText(unit,
                new Point(Math.Round(numberRight + gap), Math.Round(top + baseline - unit.Baseline)));
    }

    private string GaugeEllipsis(string value, double width, double? size = null)
    {
        if (string.IsNullOrEmpty(value) || width <= 0) return "";
        var cap = Math.Min(128, value.Length);
        if (cap > 0 && char.IsHighSurrogate(value[cap - 1])) cap--;
        var name = value[..cap].Replace('\r', ' ').Replace('\n', ' ');
        if (cap == value.Length && Text(name, MutedBrush, size).Width <= width) return name;
        var low = 0;
        var high = name.Length;
        while (low < high)
        {
            var middle = low + (high - low + 1) / 2;
            if (Text(name[..middle] + "…", MutedBrush, size).Width <= width) low = middle;
            else high = middle - 1;
        }

        if (low > 0 && char.IsHighSurrogate(name[low - 1])) low--;
        return low == 0 ? "" : name[..low] + "…";
    }

    private void DrawGaugeTickMarks(DrawingContext context, Point center, double outer)
    {
        var intervals = GaugeMajorTickCount - 1;
        var spacing = Math.Max(5, FontSize * 0.55);
        var subdivisions = Math.Min(GaugeMinorTickCount + 1,
            Math.Max(1, (int)Math.Floor(Math.PI * outer / (intervals * spacing))));
        var steps = (GaugeMajorTickCount - 1) * subdivisions;
        var majorPen = new Pen(MutedBrush ?? Foreground, Math.Max(1, SelectionThickness * 0.7))
            { LineCap = PenLineCap.Round };
        var minorPen = new Pen(MutedBrush ?? Foreground, Math.Max(0.75, SelectionThickness * 0.45))
            { LineCap = PenLineCap.Round };
        var gap = Math.Max(2, FontSize * 0.2);
        var majorLength = Math.Max(4, FontSize * 0.45);
        for (var tick = 0; tick <= steps; tick++)
        {
            var angle = Math.PI + tick / (double)steps * Math.PI;
            var major = tick % subdivisions == 0;
            var start = RadialPoint(center, outer + gap, angle);
            var end = RadialPoint(center, outer + gap + majorLength * (major ? 1 : 0.55), angle);
            context.DrawLine(major ? majorPen : minorPen, start, end);
        }
    }

    private void DrawGaugeTickLabels(DrawingContext context, Rect plot, Point center, double radius)
    {
        var placed = new List<Rect>();

        void Label(int tick)
        {
            var fraction = tick / (double)(GaugeMajorTickCount - 1);
            var value = fraction == 0 ? Minimum :
                fraction == 1 ? Maximum : (1 - fraction) * Minimum + fraction * Maximum;
            var text = GaugeTickText(value);
            if (text.Width + 4 > plot.Width || text.Height + 4 > plot.Height) return;
            var angle = Math.PI + fraction * Math.PI;
            var extent = Math.Abs(Math.Cos(angle)) * text.Width / 2 + Math.Abs(Math.Sin(angle)) * text.Height / 2;
            var offset = Math.Max(2, FontSize * 0.2) + Math.Max(4, FontSize * 0.45) + 4 + extent;
            var position = RadialPoint(center, radius + offset, angle);
            var bounds = new Rect(Math.Clamp(position.X - text.Width / 2, plot.Left + 2, plot.Right - text.Width - 2),
                Math.Clamp(position.Y - text.Height / 2, plot.Top + 2, plot.Bottom - text.Height - 2), text.Width,
                text.Height);
            if (placed.Any(previous => previous.Inflate(3).Intersects(bounds))) return;
            placed.Add(bounds);
            context.DrawText(text, bounds.TopLeft);
        }

        // Reserve endpoint labels first; compact gauges omit intermediate labels before losing the range.
        Label(0);
        Label(GaugeMajorTickCount - 1);
        for (var tick = 1; tick < GaugeMajorTickCount - 1; tick++) Label(tick);
    }

    private FormattedText GaugeTickText(double value)
    {
        var name = TickText(value, (Maximum - Minimum) / (GaugeMajorTickCount - 1), ValueFormat);
        // The adaptive format already abbreviates large ranges. Keep its precision for
        // narrow ranges at a high magnitude; layout can omit labels that do not fit.
        if (name.Length > 10 && ValueFormat is not (null or "0.##"))
            name = value.ToString("0.##E+0", CultureInfo.CurrentCulture);
        return Text(name, MutedBrush, Math.Max(8, FontSize - 1));
    }
}