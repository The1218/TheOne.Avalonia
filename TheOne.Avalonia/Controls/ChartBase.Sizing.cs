using global::Avalonia;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    public static readonly StyledProperty<double> DefaultChartWidthProperty = AvaloniaProperty.Register<ChartBase, double>(nameof(DefaultChartWidth), 400, validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<double> DefaultChartHeightProperty = AvaloniaProperty.Register<ChartBase, double>(nameof(DefaultChartHeight), 280, validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<double> ChartPaddingProperty = AvaloniaProperty.Register<ChartBase, double>(nameof(ChartPadding), 12, validate: value => double.IsFinite(value) && value >= 0);
    public static readonly StyledProperty<double> MarkerRadiusProperty = AvaloniaProperty.Register<ChartBase, double>(nameof(MarkerRadius), 3.5, validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<double> SelectionThicknessProperty = AvaloniaProperty.Register<ChartBase, double>(nameof(SelectionThickness), 2, validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<double> SliceHoverOffsetProperty = AvaloniaProperty.Register<ChartBase, double>(nameof(SliceHoverOffset), 6, validate: value => double.IsFinite(value) && value >= 0);
    public static readonly StyledProperty<double> SliceHoverScaleProperty = AvaloniaProperty.Register<ChartBase, double>(nameof(SliceHoverScale), 1.025, validate: value => double.IsFinite(value) && value >= 1 && value <= 1.2);
    public static readonly StyledProperty<double> GaugeBandRatioProperty = AvaloniaProperty.Register<ChartBase, double>(nameof(GaugeBandRatio), 0.18, validate: value => double.IsFinite(value) && value > 0 && value <= 0.5);

    /// <summary>Desired width when the consumer has not assigned Width.</summary>
    public double DefaultChartWidth { get => GetValue(DefaultChartWidthProperty); set => SetValue(DefaultChartWidthProperty, value); }
    /// <summary>Desired height when the consumer has not assigned Height.</summary>
    public double DefaultChartHeight { get => GetValue(DefaultChartHeightProperty); set => SetValue(DefaultChartHeightProperty, value); }
    public double ChartPadding { get => GetValue(ChartPaddingProperty); set => SetValue(ChartPaddingProperty, value); }
    public double MarkerRadius { get => GetValue(MarkerRadiusProperty); set => SetValue(MarkerRadiusProperty, value); }
    public double SelectionThickness { get => GetValue(SelectionThicknessProperty); set => SetValue(SelectionThicknessProperty, value); }
    public double SliceHoverOffset { get => GetValue(SliceHoverOffsetProperty); set => SetValue(SliceHoverOffsetProperty, value); }
    public double SliceHoverScale { get => GetValue(SliceHoverScaleProperty); set => SetValue(SliceHoverScaleProperty, value); }
    public double GaugeBandRatio { get => GetValue(GaugeBandRatioProperty); set => SetValue(GaugeBandRatioProperty, value); }

    private static void RegisterChartSizingMeasure() => AffectsMeasure<ChartBase>(DefaultChartWidthProperty, DefaultChartHeightProperty);
    private static bool IsChartSizingProperty(AvaloniaProperty property) =>
        property == DefaultChartWidthProperty || property == DefaultChartHeightProperty || property == ChartPaddingProperty ||
        property == MarkerRadiusProperty || property == SelectionThicknessProperty || property == SliceHoverOffsetProperty ||
        property == SliceHoverScaleProperty || property == GaugeBandRatioProperty;
}
