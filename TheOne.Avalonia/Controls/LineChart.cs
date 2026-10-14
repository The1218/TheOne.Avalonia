namespace TheOne.Avalonia.Controls;

/// <summary>A fixed Line chart control with its own theme and visual state.</summary>
public sealed class LineChart : CartesianChartBase
{
    public override ChartKind Kind => ChartKind.Line;
    private protected override bool DrawsLineCartesian => true;
    private protected override bool SupportsSmooth => true;
    private protected override ChartBase CreateExportSnapshot() => new LineChart();
}
