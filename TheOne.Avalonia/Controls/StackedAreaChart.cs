namespace TheOne.Avalonia.Controls;

/// <summary>A fixed StackedArea chart control with its own theme and visual state.</summary>
public sealed class StackedAreaChart : CartesianChartBase
{
    public override ChartKind Kind => ChartKind.StackedArea;
    private protected override bool IsAreaCartesian => true;
    private protected override bool IsStackedCartesian => true;
    private protected override ChartBase CreateExportSnapshot() => new StackedAreaChart();
}
