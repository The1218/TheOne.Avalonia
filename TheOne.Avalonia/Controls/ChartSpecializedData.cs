using Avalonia;

namespace TheOne.Avalonia.Controls;

/// <summary>A heatmap cell. X and Y locate the cell; Value controls its color.</summary>
public sealed class HeatmapPoint : ChartPoint
{
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<HeatmapPoint, double>(nameof(Value));

    public HeatmapPoint() { }

    public HeatmapPoint(double x, double y, double value, string? label = null) : base(x, y, label)
    {
        Value = value;
    }

    /// <summary>Non-finite values leave a gap without changing the cell coordinates.</summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
}

/// <summary>The name and independent scale of a radar dimension.</summary>
public sealed class RadarAxis : AvaloniaObject
{
    public static readonly StyledProperty<string> NameProperty =
        AvaloniaProperty.Register<RadarAxis, string>(nameof(Name), "Axis");
    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<RadarAxis, double>(nameof(Minimum), validate: double.IsFinite);
    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<RadarAxis, double>(nameof(Maximum), 100, validate: double.IsFinite);

    public string Name
    {
        get => GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }
}

/// <summary>Optional funnel ordering. None retains the business order of the source.</summary>
public enum FunnelSortMode
{
    None,
    Ascending,
    Descending
}
