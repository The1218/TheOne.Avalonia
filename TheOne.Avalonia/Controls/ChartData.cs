using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public enum ChartKind
{
    Line,
    Area,
    Bar,
    HorizontalBar,
    Scatter,
    Pie,
    Doughnut,
    Gauge,
    StackedBar,
    StackedHorizontalBar,
    StackedArea,
    PercentStackedBar,
    PercentStackedHorizontalBar,
    PercentStackedArea,
    Heatmap,
    Radar,
    Funnel,
    BoxPlot,
    Candlestick,
    Treemap,
    Sunburst
}

public enum ChartYAxis
{
    Primary,
    Secondary
}

internal readonly record struct ChartRenderPoint(int Index, Point Position, Point Baseline);

/// <summary>A mutable, observable data point. Non-finite coordinates are treated as gaps.</summary>
public class ChartPoint : AvaloniaObject
{
    public static readonly StyledProperty<double> XProperty = AvaloniaProperty.Register<ChartPoint, double>(nameof(X));
    public static readonly StyledProperty<double> YProperty = AvaloniaProperty.Register<ChartPoint, double>(nameof(Y));

    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<ChartPoint, string?>(nameof(Label));

    public ChartPoint()
    {
    }

    public ChartPoint(double x, double y, string? label = null)
    {
        X = x;
        Y = y;
        Label = label;
    }

    public double X
    {
        get => GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    public double Y
    {
        get => GetValue(YProperty);
        set => SetValue(YProperty, value);
    }

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
}

/// <summary>One series. Observable collections and point properties update attached charts automatically.</summary>
public sealed class ChartSeries : AvaloniaObject
{
    public static readonly StyledProperty<string> NameProperty =
        AvaloniaProperty.Register<ChartSeries, string>(nameof(Name), "Series");

    public static readonly StyledProperty<IList<ChartPoint>?> ItemsSourceProperty =
        AvaloniaProperty.Register<ChartSeries, IList<ChartPoint>?>(nameof(ItemsSource));

    public static readonly StyledProperty<IBrush?> BrushProperty =
        AvaloniaProperty.Register<ChartSeries, IBrush?>(nameof(Brush));

    public static readonly StyledProperty<bool> IsVisibleProperty =
        AvaloniaProperty.Register<ChartSeries, bool>(nameof(IsVisible), true);

    public static readonly StyledProperty<ChartYAxis> YAxisProperty =
        AvaloniaProperty.Register<ChartSeries, ChartYAxis>(nameof(YAxis),
            validate: value => value is ChartYAxis.Primary or ChartYAxis.Secondary);

    public static readonly StyledProperty<string?> UnitProperty =
        AvaloniaProperty.Register<ChartSeries, string?>(nameof(Unit));

    public static readonly StyledProperty<string?> ValueFormatProperty =
        AvaloniaProperty.Register<ChartSeries, string?>(nameof(ValueFormat));

    public ChartSeries()
    {
        ItemsSource = new ObservableCollection<ChartPoint>();
    }

    public string Name
    {
        get => GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public IList<ChartPoint>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IBrush? Brush
    {
        get => GetValue(BrushProperty);
        set => SetValue(BrushProperty, value);
    }

    public bool IsVisible
    {
        get => GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    /// <summary>The independent value scale used by this series in Cartesian charts.</summary>
    public ChartYAxis YAxis
    {
        get => GetValue(YAxisProperty);
        set => SetValue(YAxisProperty, value);
    }

    /// <summary>Optional tooltip/data-label unit; null inherits the assigned chart axis unit.</summary>
    public string? Unit
    {
        get => GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    /// <summary>Optional raw-value numeric format; null inherits the assigned chart axis format.</summary>
    public string? ValueFormat
    {
        get => GetValue(ValueFormatProperty);
        set => SetValue(ValueFormatProperty, value);
    }
}

public sealed record ChartSelection(int SeriesIndex, int PointIndex, ChartSeries Series, ChartPoint Point);

public sealed class ChartSelectionChangedEventArgs(ChartSelection? selection) : EventArgs
{
    public ChartSelection? Selection { get; } = selection;
}
