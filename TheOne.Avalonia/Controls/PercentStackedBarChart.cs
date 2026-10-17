namespace TheOne.Avalonia.Controls;

/// <summary>A fixed PercentStackedBar chart control with its own theme and visual state.</summary>
public sealed class PercentStackedBarChart : CartesianChartBase
{
    public override ChartKind Kind => ChartKind.PercentStackedBar;
    private protected override bool IsCategoricalCartesian => true;
    private protected override bool IsStackedCartesian => true;
    private protected override bool IsPercentCartesian => true;

    private protected override ChartBase CreateExportSnapshot()
    {
        return new PercentStackedBarChart();
    }
}