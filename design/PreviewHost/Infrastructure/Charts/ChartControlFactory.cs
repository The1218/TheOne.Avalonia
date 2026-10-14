using TheOne.Avalonia.Controls;
namespace TheOne.DesignPreview;
internal static class ChartControlFactory
{
    public static ChartBase CreateControl(ChartKind kind) => kind switch
    {
        ChartKind.Line => new LineChart(),
        ChartKind.Area => new AreaChart(),
        ChartKind.Bar => new BarChart(),
        ChartKind.HorizontalBar => new HorizontalBarChart(),
        ChartKind.Scatter => new ScatterChart(),
        ChartKind.Pie => new PieChart(),
        ChartKind.Doughnut => new DoughnutChart(),
        ChartKind.Gauge => new GaugeChart(),
        ChartKind.StackedBar => new StackedBarChart(),
        ChartKind.StackedHorizontalBar => new StackedHorizontalBarChart(),
        ChartKind.StackedArea => new StackedAreaChart(),
        ChartKind.PercentStackedBar => new PercentStackedBarChart(),
        ChartKind.PercentStackedHorizontalBar => new PercentStackedHorizontalBarChart(),
        ChartKind.PercentStackedArea => new PercentStackedAreaChart(),
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };
}
