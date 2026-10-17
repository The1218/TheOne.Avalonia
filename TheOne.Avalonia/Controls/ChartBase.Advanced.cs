using Avalonia;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    public static readonly StyledProperty<double> SecondaryYMinimumProperty =
        AvaloniaProperty.Register<ChartBase, double>(nameof(SecondaryYMinimum), double.NaN,
            validate: value => !double.IsInfinity(value));

    public static readonly StyledProperty<double> SecondaryYMaximumProperty =
        AvaloniaProperty.Register<ChartBase, double>(nameof(SecondaryYMaximum), double.NaN,
            validate: value => !double.IsInfinity(value));

    public static readonly StyledProperty<string?> YAxisTitleProperty =
        AvaloniaProperty.Register<ChartBase, string?>(nameof(YAxisTitle));

    public static readonly StyledProperty<string?> SecondaryYAxisTitleProperty =
        AvaloniaProperty.Register<ChartBase, string?>(nameof(SecondaryYAxisTitle));

    public static readonly StyledProperty<string> SecondaryValueFormatProperty =
        AvaloniaProperty.Register<ChartBase, string>(nameof(SecondaryValueFormat), "0.##");

    public static readonly StyledProperty<string?> SecondaryUnitProperty =
        AvaloniaProperty.Register<ChartBase, string?>(nameof(SecondaryUnit));

    public static readonly StyledProperty<bool> ShowSecondaryAxisProperty =
        AvaloniaProperty.Register<ChartBase, bool>(nameof(ShowSecondaryAxis), true);

    public double SecondaryYMinimum
    {
        get => GetValue(SecondaryYMinimumProperty);
        set => SetValue(SecondaryYMinimumProperty, value);
    }

    public double SecondaryYMaximum
    {
        get => GetValue(SecondaryYMaximumProperty);
        set => SetValue(SecondaryYMaximumProperty, value);
    }

    public string? YAxisTitle
    {
        get => GetValue(YAxisTitleProperty);
        set => SetValue(YAxisTitleProperty, value);
    }

    public string? SecondaryYAxisTitle
    {
        get => GetValue(SecondaryYAxisTitleProperty);
        set => SetValue(SecondaryYAxisTitleProperty, value);
    }

    public string SecondaryValueFormat
    {
        get => GetValue(SecondaryValueFormatProperty);
        set => SetValue(SecondaryValueFormatProperty, value);
    }

    public string? SecondaryUnit
    {
        get => GetValue(SecondaryUnitProperty);
        set => SetValue(SecondaryUnitProperty, value);
    }

    public bool ShowSecondaryAxis
    {
        get => GetValue(ShowSecondaryAxisProperty);
        set => SetValue(ShowSecondaryAxisProperty, value);
    }

    /// <summary>A signed share of the matching axis/category/sign total in a percentage stack, or null otherwise.</summary>
    public double? GetStackPercentage(int seriesIndex, int pointIndex)
    {
        if (!IsPercentCartesian || GetSelection(seriesIndex, pointIndex) is not { } selected ||
            SeriesSource is not { } source) return null;
        if (selected.Point.Y == 0) return 0;
        var negative = selected.Point.Y < 0;
        var maximum = 0d;
        foreach (var series in source)
            if (MatchingContribution(series, pointIndex, selected.Series.YAxis, negative, out var point))
                maximum = Math.Max(maximum, Math.Abs(point!.Y));
        if (maximum == 0) return 0;
        var total = 0d;
        foreach (var series in source)
            if (MatchingContribution(series, pointIndex, selected.Series.YAxis, negative, out var point))
                total += Math.Abs(point!.Y) / maximum;
        return (negative ? -1 : 1) * (Math.Abs(selected.Point.Y) / maximum / total);
    }

    private bool MatchingContribution(ChartSeries series, int index, ChartYAxis axis, bool negative,
        out ChartPoint? point)
    {
        point = null;
        if (!series.IsVisible || series.YAxis != axis || series.ItemsSource is not { } points ||
            index >= points.Count) return false;
        point = points[index];
        if (!IsValid(point) || point.Y < 0 != negative) return false;
        if (IsStackedAreaCartesian && SeriesSource is { } source)
            foreach (var candidate in source)
                if (candidate.IsVisible && candidate.YAxis == axis && candidate.ItemsSource is { } canonical &&
                    index < canonical.Count && double.IsFinite(canonical[index].X))
                    return point.X == canonical[index].X;
        return true;
    }
}