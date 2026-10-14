using System.Globalization;

namespace TheOne.Avalonia.Controls;

public abstract partial class ChartBase
{
    // Multiples of 1/2/2.5/5 keep ticks readable without changing explicit ranges or data coordinates.
    private static IEnumerable<(double Value, double Step)> AxisTicks(AxisRange range, int divisions)
    {
        var span = range.Maximum - range.Minimum;
        var raw = span / divisions;
        var magnitude = Math.Pow(10, Math.Floor(Math.Log10(raw)));
        var normalized = raw / magnitude;
        var step = (normalized <= 1 ? 1 : normalized <= 2 ? 2 : normalized <= 2.5 ? 2.5 : normalized <= 5 ? 5 : 10) * magnitude;
        if (double.IsFinite(step) && step > 0)
        {
            var first = Math.Ceiling(range.Minimum / step);
            var last = Math.Floor(range.Maximum / step);
            if (double.IsFinite(first) && double.IsFinite(last) && last > first && last - first <= 32)
            {
                for (var index = 0; index <= (int)(last - first); index++)
                {
                    var value = (first + index) * step;
                    if (double.IsFinite(value) && value >= range.Minimum && value <= range.Maximum)
                        yield return (value == 0 ? 0 : value, step);
                }
                yield break;
            }
        }
        // Overflow/subnormal and very narrow high-magnitude ranges retain safe interpolation.
        var previous = double.NaN;
        for (var index = 0; index <= divisions; index++)
        {
            var value = range.At(index / (double)divisions);
            if (value == previous) continue;
            previous = value;
            yield return (value, raw);
        }
    }

    private protected static string TickText(double value, double step, string? format)
    {
        if (format is not (null or "0.##")) return CartesianFormat(value, format);
        if (!double.IsFinite(step) || step <= 0) return value.ToString("G6", CultureInfo.CurrentCulture);
        if (Math.Abs(value) / step > 1e6) return value.ToString("G17", CultureInfo.CurrentCulture);
        if (Math.Abs(value) >= 1e7 || step < 1e-4) return value.ToString("G6", CultureInfo.CurrentCulture);
        var decimals = Math.Clamp((int)Math.Ceiling(-Math.Log10(step)) + 1, 0, 8);
        return value.ToString(decimals == 0 ? "0" : "0." + new string('#', decimals), CultureInfo.CurrentCulture);
    }
}
