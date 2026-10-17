namespace TheOne.Avalonia.Controls;

/// <summary>A fixed HorizontalBar chart control with its own theme and visual state.</summary>
public sealed class HorizontalBarChart : CartesianChartBase
{
    public override ChartKind Kind => ChartKind.HorizontalBar;
    private protected override bool IsCategoricalCartesian => true;
    private protected override bool IsHorizontalCartesian => true;

    private protected override ChartBase CreateExportSnapshot()
    {
        return new HorizontalBarChart();
    }
}