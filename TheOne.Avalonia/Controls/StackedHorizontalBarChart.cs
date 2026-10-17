namespace TheOne.Avalonia.Controls;

/// <summary>A fixed StackedHorizontalBar chart control with its own theme and visual state.</summary>
public sealed class StackedHorizontalBarChart : CartesianChartBase
{
    public override ChartKind Kind => ChartKind.StackedHorizontalBar;
    private protected override bool IsCategoricalCartesian => true;
    private protected override bool IsStackedCartesian => true;
    private protected override bool IsHorizontalCartesian => true;

    private protected override ChartBase CreateExportSnapshot()
    {
        return new StackedHorizontalBarChart();
    }
}