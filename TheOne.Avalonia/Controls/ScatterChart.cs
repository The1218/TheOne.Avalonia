namespace TheOne.Avalonia.Controls;

/// <summary>A fixed Scatter chart control with its own theme and visual state.</summary>
public sealed class ScatterChart : CartesianChartBase
{
    public override ChartKind Kind => ChartKind.Scatter;
    private protected override bool IsScatterCartesian => true;
    private protected override ChartBase CreateExportSnapshot() => new ScatterChart();
}
