using global::Avalonia;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>Shared axes, data preparation and interaction for fixed Cartesian chart controls.</summary>
public abstract class CartesianChartBase : ChartBase
{
    private protected override void PrepareChartGeometry(Rect plot) => BuildCartesianFrame(plot);
    private protected override void DrawChartPlot(DrawingContext context, Rect plot) => DrawCartesian(context, plot);
}
