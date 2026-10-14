namespace TheOne.Avalonia.Controls;

/// <summary>A fixed Doughnut chart control with its own theme and visual state.</summary>
public sealed class DoughnutChart : RadialChartBase
{
    public override ChartKind Kind => ChartKind.Doughnut;
    private protected override bool HasRadialHole => true;
    private protected override ChartBase CreateExportSnapshot() => new DoughnutChart();
}
