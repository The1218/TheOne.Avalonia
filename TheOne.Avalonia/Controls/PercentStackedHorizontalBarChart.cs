namespace TheOne.Avalonia.Controls;

/// <summary>A fixed PercentStackedHorizontalBar chart control with its own theme and visual state.</summary>
public sealed class PercentStackedHorizontalBarChart : CartesianChartBase
{
    public override ChartKind Kind => ChartKind.PercentStackedHorizontalBar;
    private protected override bool IsCategoricalCartesian => true;
    private protected override bool IsStackedCartesian => true;
    private protected override bool IsPercentCartesian => true;
    private protected override bool IsHorizontalCartesian => true;

    private protected override ChartBase CreateExportSnapshot()
    {
        return new PercentStackedHorizontalBarChart();
    }
}