namespace TheOne.Avalonia.Controls;

/// <summary>A fixed StackedBar chart control with its own theme and visual state.</summary>
public sealed class StackedBarChart : CartesianChartBase
{
    public override ChartKind Kind => ChartKind.StackedBar;
    private protected override bool IsCategoricalCartesian => true;
    private protected override bool IsStackedCartesian => true;

    private protected override ChartBase CreateExportSnapshot()
    {
        return new StackedBarChart();
    }
}