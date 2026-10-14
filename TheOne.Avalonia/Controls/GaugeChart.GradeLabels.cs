using global::Avalonia;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public sealed partial class GaugeChart
{
    private readonly record struct GradeLabel(FormattedText Text, double Angle, double Rotation, double RadialExtent);
    private double GradeLabelGap => double.IsNaN(GaugeGradeLabelGap) ? Math.Max(4, FontSize * 0.75) : GaugeGradeLabelGap;

    private List<GradeLabel> PrepareGradeLabels(List<GaugeBand> bands, Rect plot, double start, double sweep)
    {
        var labels = new List<GradeLabel>();
        foreach (var band in bands)
        {
            var text = Text(GaugeEllipsis(band.Label ?? "", plot.Width * 0.3), GaugeTickBrush ?? MutedBrush, FontSize);
            if (text.Width <= 0) continue;
            var angle = start + (band.Start + band.End) / 2 * sweep;
            var rotation = double.IsNaN(GaugeGradeLabelRotation) ? angle + Math.PI / 2 : GaugeGradeLabelRotation * Math.PI / 180;
            // Keep the tangent readable on both sides of the dial.
            if (double.IsNaN(GaugeGradeLabelRotation))
            {
                rotation = (rotation + Math.PI / 2) % Math.PI;
                if (rotation < 0) rotation += Math.PI;
                rotation -= Math.PI / 2;
            }
            var radialExtent = Math.Abs(Math.Cos(angle - rotation)) * text.Width / 2 + Math.Abs(Math.Sin(angle - rotation)) * text.Height / 2;
            labels.Add(new(text, angle, rotation, radialExtent));
        }
        return labels;
    }
    private Point GradeLabelPosition(GradeLabel label, Point center, double radius) =>
        RadialPoint(center, radius + GradeLabelGap + label.RadialExtent, label.Angle);
    private Rect GradeLabelBounds(GradeLabel label, Point center, double radius)
    {
        var position = GradeLabelPosition(label, center, radius);
        var width = Math.Abs(Math.Cos(label.Rotation)) * label.Text.Width + Math.Abs(Math.Sin(label.Rotation)) * label.Text.Height;
        var height = Math.Abs(Math.Sin(label.Rotation)) * label.Text.Width + Math.Abs(Math.Cos(label.Rotation)) * label.Text.Height;
        return new Rect(position.X - width / 2, position.Y - height / 2, width, height);
    }
    private (Point Center, double Radius) FitGradeGeometry(Rect plot, List<GradeLabel> labels, double left, double top, double right, double bottom)
    {
        Rect Extent(double radius)
        {
            var bounds = new Rect(left * radius, top * radius, (right - left) * radius, (bottom - top) * radius);
            foreach (var label in labels) bounds = bounds.Union(GradeLabelBounds(label, default, radius));
            // The default description sits just below the pivot; value text is inside the arc.
            if (!string.IsNullOrWhiteSpace(GaugeCaption))
                bounds = bounds.Union(new Rect(-FontSize, 0, FontSize * 2, FontSize * 1.9));
            return bounds.Inflate(2);
        }
        var low = 0d; var high = Math.Max(plot.Width, plot.Height);
        for (var iteration = 0; iteration < 16; iteration++)
        {
            var middle = (low + high) / 2; var extent = Extent(middle);
            if (extent.Width <= plot.Width && extent.Height <= plot.Height) low = middle; else high = middle;
        }
        var fitted = Extent(low);
        return (plot.Center - new Vector(fitted.Center.X, fitted.Center.Y), low);
    }
    private void DrawGradeLabels(DrawingContext context, Rect plot, Point center, double radius, List<GradeLabel> labels, List<Rect> occupied)
    {
        foreach (var label in labels)
        {
            var bounds = GradeLabelBounds(label, center, radius);
            if (bounds.Left < plot.Left || bounds.Right > plot.Right || bounds.Top < plot.Top || bounds.Bottom > plot.Bottom ||
                occupied.Any(previous => previous.Inflate(3).Intersects(bounds))) continue;
            var position = GradeLabelPosition(label, center, radius);
            using var transform = context.PushTransform(Matrix.CreateRotation(label.Rotation) * Matrix.CreateTranslation(position.X, position.Y));
            context.DrawText(label.Text, new Point(-label.Text.Width / 2, -label.Text.Height / 2));
            occupied.Add(bounds);
        }
    }
}
