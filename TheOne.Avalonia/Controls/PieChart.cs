namespace TheOne.Avalonia.Controls;

/// <summary>A fixed Pie chart control with its own theme and visual state.</summary>
public sealed class PieChart : RadialChartBase
{
    public override ChartKind Kind => ChartKind.Pie;
    private protected override ChartBase CreateExportSnapshot() => new PieChart();
}
