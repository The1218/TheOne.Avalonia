using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>Shared sector preparation and interaction for pie and doughnut controls.</summary>
public abstract class RadialChartBase : ChartBase
{
    private protected override bool IsRadial => true;

    private protected override void PrepareChartGeometry(Rect plot)
    {
        DrawRadial(null, plot);
    }

    private protected override void DrawChartPlot(DrawingContext context, Rect plot)
    {
        DrawRadial(context, plot);
    }
}