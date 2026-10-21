using System.Collections.ObjectModel;
using Avalonia;

namespace TheOne.Avalonia.Controls;

/// <summary>Precomputed five-number summary. Median and inherited Y refer to the same observable value.</summary>
public sealed class BoxPlotPoint : ChartPoint
{
    public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<BoxPlotPoint, double>(nameof(Minimum));
    public static readonly StyledProperty<double> LowerQuartileProperty = AvaloniaProperty.Register<BoxPlotPoint, double>(nameof(LowerQuartile));
    public static readonly DirectProperty<BoxPlotPoint, double> MedianProperty = AvaloniaProperty.RegisterDirect<BoxPlotPoint, double>(
        nameof(Median), point => point.Y, (point, value) => point.Y = value);
    public static readonly StyledProperty<double> UpperQuartileProperty = AvaloniaProperty.Register<BoxPlotPoint, double>(nameof(UpperQuartile));
    public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<BoxPlotPoint, double>(nameof(Maximum));
    public static readonly StyledProperty<IList<double>?> OutliersProperty = AvaloniaProperty.Register<BoxPlotPoint, IList<double>?>(nameof(Outliers));

    public BoxPlotPoint() => Outliers = new ObservableCollection<double>();
    public BoxPlotPoint(double x, double minimum, double lowerQuartile, double median, double upperQuartile, double maximum, string? label = null)
        : this()
    {
        X = x; Minimum = minimum; LowerQuartile = lowerQuartile; Median = median;
        UpperQuartile = upperQuartile; Maximum = maximum; Label = label;
    }

    public double Minimum { get => GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double LowerQuartile { get => GetValue(LowerQuartileProperty); set => SetValue(LowerQuartileProperty, value); }
    public double Median { get => Y; set => Y = value; }
    public double UpperQuartile { get => GetValue(UpperQuartileProperty); set => SetValue(UpperQuartileProperty, value); }
    public double Maximum { get => GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public IList<double>? Outliers { get => GetValue(OutliersProperty); set => SetValue(OutliersProperty, value); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == YProperty) RaisePropertyChanged(MedianProperty, change.GetOldValue<double>(), change.GetNewValue<double>());
    }
}

/// <summary>OHLC observation at a numeric X coordinate. Close and inherited Y are the same value.</summary>
public sealed class CandlestickPoint : ChartPoint
{
    public static readonly StyledProperty<double> OpenProperty = AvaloniaProperty.Register<CandlestickPoint, double>(nameof(Open));
    public static readonly DirectProperty<CandlestickPoint, double> CloseProperty = AvaloniaProperty.RegisterDirect<CandlestickPoint, double>(
        nameof(Close), point => point.Y, (point, value) => point.Y = value);
    public static readonly StyledProperty<double> LowProperty = AvaloniaProperty.Register<CandlestickPoint, double>(nameof(Low));
    public static readonly StyledProperty<double> HighProperty = AvaloniaProperty.Register<CandlestickPoint, double>(nameof(High));

    public CandlestickPoint() { }
    public CandlestickPoint(double x, double open, double close, double low, double high, string? label = null)
    {
        X = x; Open = open; Close = close; Low = low; High = high; Label = label;
    }

    public double Open { get => GetValue(OpenProperty); set => SetValue(OpenProperty, value); }
    public double Close { get => Y; set => Y = value; }
    public double Low { get => GetValue(LowProperty); set => SetValue(LowProperty, value); }
    public double High { get => GetValue(HighProperty); set => SetValue(HighProperty, value); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == YProperty) RaisePropertyChanged(CloseProperty, change.GetOldValue<double>(), change.GetNewValue<double>());
    }
}
