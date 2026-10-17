using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    public static readonly StyledProperty<bool> IsSmoothProperty =
        AvaloniaProperty.Register<ChartBase, bool>(nameof(IsSmooth));

    /// <summary>Use shape-preserving cubic interpolation for Line and ordinary Area. Other kinds retain straight edges.</summary>
    public bool IsSmooth
    {
        get => GetValue(IsSmoothProperty);
        set => SetValue(IsSmoothProperty, value);
    }

    private void AppendCurve(StreamGeometryContext path, IReadOnlyList<ChartRenderPoint> points, bool eligible,
        bool baseline = false, bool reverse = false)
    {
        Point At(int i)
        {
            return baseline ? points[i].Baseline : points[i].Position;
        }

        var count = points.Count;
        if (count < 2) return;
        var smooth = eligible && IsSmooth && SupportsSmooth;
        if (!smooth)
        {
            if (reverse)
                for (var i = count - 2; i >= 0; i--)
                    path.LineTo(At(i));
            else
                for (var i = 1; i < count; i++)
                    path.LineTo(At(i));
            return;
        }

        var slopes = new double[count - 1];
        var widths = new double[count - 1];
        var direction = Math.Sign(At(1).X - At(0).X);
        for (var i = 0; i < count - 1 && smooth; i++)
        {
            var a = At(i);
            var b = At(i + 1);
            var dx = b.X - a.X;
            widths[i] = Math.Abs(dx);
            if (direction == 0 || Math.Sign(dx) != direction)
            {
                smooth = false;
                break;
            }

            slopes[i] = (b.Y - a.Y) / dx;
            if (!double.IsFinite(slopes[i])) smooth = false;
        }

        if (!smooth)
        {
            if (reverse)
                for (var i = count - 2; i >= 0; i--)
                    path.LineTo(At(i));
            else
                for (var i = 1; i < count; i++)
                    path.LineTo(At(i));
            return;
        }

        var tangents = new double[count];
        tangents[0] = slopes[0];
        tangents[^1] = slopes[^1];
        for (var i = 1; i < count - 1; i++)
        {
            var left = slopes[i - 1];
            var right = slopes[i];
            if (left == 0 || right == 0 || Math.Sign(left) != Math.Sign(right)) continue;
            // Weighted harmonic means preserve extrema and avoid spline overshoot on uneven X intervals.
            var scale = Math.Max(widths[i - 1], widths[i]);
            var a = widths[i - 1] / scale;
            var b = widths[i] / scale;
            var w1 = 2 * b + a;
            var w2 = b + 2 * a;
            tangents[i] = (w1 + w2) / (w1 / left + w2 / right);
        }

        void Edge(int i, bool backwards)
        {
            var a = At(i);
            var b = At(i + 1);
            var third = (b.X - a.X) / 3;
            var low = Math.Min(a.Y, b.Y);
            var high = Math.Max(a.Y, b.Y);
            var c1 = new Point(a.X + third, Math.Clamp(a.Y + third * tangents[i], low, high));
            var c2 = new Point(b.X - third, Math.Clamp(b.Y - third * tangents[i + 1], low, high));
            if (backwards) path.CubicBezierTo(c2, c1, a);
            else path.CubicBezierTo(c1, c2, b);
        }

        if (reverse)
            for (var i = count - 2; i >= 0; i--)
                Edge(i, true);
        else
            for (var i = 0; i < count - 1; i++)
                Edge(i, false);
    }
}