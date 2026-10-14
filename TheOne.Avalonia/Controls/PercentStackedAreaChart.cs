namespace TheOne.Avalonia.Controls;

/// <summary>A fixed PercentStackedArea chart control with its own theme and visual state.</summary>
public sealed class PercentStackedAreaChart : CartesianChartBase
{
    public override ChartKind Kind => ChartKind.PercentStackedArea;
    private protected override bool IsAreaCartesian => true;
    private protected override bool IsStackedCartesian => true;
    private protected override bool IsPercentCartesian => true;
    private protected override ChartBase CreateExportSnapshot() => new PercentStackedAreaChart();
}
