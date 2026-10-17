namespace TheOne.Avalonia.Controls;

/// <summary>A fixed Area chart control with its own theme and visual state.</summary>
public sealed class AreaChart : CartesianChartBase
{
    public override ChartKind Kind => ChartKind.Area;
    private protected override bool IsAreaCartesian => true;
    private protected override bool SupportsSmooth => true;

    private protected override ChartBase CreateExportSnapshot()
    {
        return new AreaChart();
    }
}