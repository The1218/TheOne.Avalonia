using System.Globalization;
using global::Avalonia;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    public static readonly StyledProperty<bool> ShowGaugeNeedleProperty = AvaloniaProperty.Register<ChartBase, bool>(nameof(ShowGaugeNeedle));
    public static readonly StyledProperty<bool> ShowGaugeTicksProperty = AvaloniaProperty.Register<ChartBase, bool>(nameof(ShowGaugeTicks));
    public static readonly StyledProperty<int> GaugeMajorTickCountProperty = AvaloniaProperty.Register<ChartBase, int>(nameof(GaugeMajorTickCount), 5, validate: value => value >= 2 && value <= 25);
    public static readonly StyledProperty<int> GaugeMinorTickCountProperty = AvaloniaProperty.Register<ChartBase, int>(nameof(GaugeMinorTickCount), 2, validate: value => value >= 0 && value <= 10);
    public bool ShowGaugeNeedle { get => GetValue(ShowGaugeNeedleProperty); set => SetValue(ShowGaugeNeedleProperty, value); }
    public bool ShowGaugeTicks { get => GetValue(ShowGaugeTicksProperty); set => SetValue(ShowGaugeTicksProperty, value); }
    /// <summary>Number of major ticks, including the minimum and maximum.</summary>
    public int GaugeMajorTickCount { get => GetValue(GaugeMajorTickCountProperty); set => SetValue(GaugeMajorTickCountProperty, value); }
    /// <summary>Maximum minor marks between consecutive major ticks. Compact gauges reduce them to keep readable spacing.</summary>
    public int GaugeMinorTickCount { get => GetValue(GaugeMinorTickCountProperty); set => SetValue(GaugeMinorTickCountProperty, value); }
    private static bool IsChartGaugeProperty(AvaloniaProperty property) => property == ShowGaugeNeedleProperty || property == ShowGaugeTicksProperty || property == GaugeMajorTickCountProperty || property == GaugeMinorTickCountProperty;

}
