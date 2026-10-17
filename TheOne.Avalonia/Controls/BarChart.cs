namespace TheOne.Avalonia.Controls;

/// <summary>A fixed Bar chart control with its own theme and visual state.</summary>
public sealed class BarChart : CartesianChartBase
{
    public override ChartKind Kind => ChartKind.Bar;
    private protected override bool IsCategoricalCartesian => true;

    private protected override ChartBase CreateExportSnapshot()
    {
        return new BarChart();
    }
}