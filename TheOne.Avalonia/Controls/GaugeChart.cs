using global::Avalonia;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>A fixed Gauge chart control with its own theme and visual state.</summary>
public sealed partial class GaugeChart : ChartBase
{
    public override ChartKind Kind => ChartKind.Gauge;
    private protected override bool IsRadial => true;
    private protected override bool IsGauge => true;
    private protected override void PrepareChartGeometry(Rect plot) => PrepareGaugeGeometry();
    private protected override void DrawChartPlot(DrawingContext context, Rect plot) => DrawGauge(context, plot);
    private protected override ChartBase CreateExportSnapshot() => new GaugeChart();
}
