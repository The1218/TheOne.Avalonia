using System.Collections.Specialized;
using global::Avalonia;
using global::Avalonia.Collections;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>Gauge layouts. Other visual and data differences are configured by properties.</summary>
public enum GaugeStyle { Semicircle, Basic, Speed, Progress, StageSpeed, Grade, Temperature, Barometer }
public enum GaugeNeedleShape { Auto, Triangle, Line, Marker }
public enum GaugeTickPlacement { Auto, Inside, Outside }

/// <summary>An ordered band ending at Fraction of the gauge range. Labels are used by Grade.</summary>
public sealed class GaugeSegment : AvaloniaObject
{
    public static readonly StyledProperty<double> FractionProperty = AvaloniaProperty.Register<GaugeSegment, double>(nameof(Fraction), 1, validate: value => double.IsFinite(value) && value > 0 && value <= 1);
    public static readonly StyledProperty<IBrush?> BrushProperty = AvaloniaProperty.Register<GaugeSegment, IBrush?>(nameof(Brush));
    public static readonly StyledProperty<string?> LabelProperty = AvaloniaProperty.Register<GaugeSegment, string?>(nameof(Label));
    public double Fraction { get => GetValue(FractionProperty); set => SetValue(FractionProperty, value); }
    public IBrush? Brush { get => GetValue(BrushProperty); set => SetValue(BrushProperty, value); }
    public string? Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
}

public sealed partial class GaugeChart
{
    public GaugeChart() => SetCurrentValue(GaugeSegmentsProperty, new AvaloniaList<GaugeSegment>());

    public static readonly StyledProperty<GaugeStyle> GaugeStyleProperty = AvaloniaProperty.Register<GaugeChart, GaugeStyle>(nameof(GaugeStyle), GaugeStyle.Semicircle, validate: Enum.IsDefined);
    public static readonly StyledProperty<double> GaugeStartAngleProperty = AvaloniaProperty.Register<GaugeChart, double>(nameof(GaugeStartAngle), double.NaN, validate: value => double.IsNaN(value) || (double.IsFinite(value) && value >= -360 && value <= 360));
    public static readonly StyledProperty<double> GaugeSweepAngleProperty = AvaloniaProperty.Register<GaugeChart, double>(nameof(GaugeSweepAngle), double.NaN, validate: value => double.IsNaN(value) || (double.IsFinite(value) && value > 0 && value <= 360));
    public static readonly StyledProperty<bool?> ShowGaugeProgressProperty = AvaloniaProperty.Register<GaugeChart, bool?>(nameof(ShowGaugeProgress));
    public static readonly StyledProperty<bool?> ShowGaugeAnchorProperty = AvaloniaProperty.Register<GaugeChart, bool?>(nameof(ShowGaugeAnchor));
    public static readonly StyledProperty<bool> ShowGaugeValueProperty = AvaloniaProperty.Register<GaugeChart, bool>(nameof(ShowGaugeValue), true);
    public static readonly StyledProperty<bool> ShowGaugeTickLabelsProperty = AvaloniaProperty.Register<GaugeChart, bool>(nameof(ShowGaugeTickLabels), true);
    public static readonly StyledProperty<GaugeNeedleShape> GaugeNeedleShapeProperty = AvaloniaProperty.Register<GaugeChart, GaugeNeedleShape>(nameof(GaugeNeedleShape), GaugeNeedleShape.Auto, validate: Enum.IsDefined);
    public static readonly StyledProperty<GaugeTickPlacement> GaugeTickPlacementProperty = AvaloniaProperty.Register<GaugeChart, GaugeTickPlacement>(nameof(GaugeTickPlacement), GaugeTickPlacement.Auto, validate: Enum.IsDefined);
    public static readonly StyledProperty<double> GaugeNeedleLengthProperty = AvaloniaProperty.Register<GaugeChart, double>(nameof(GaugeNeedleLength), 0.72, validate: value => double.IsFinite(value) && value > 0 && value <= 1);
    public static readonly StyledProperty<double> GaugeNeedleWidthProperty = AvaloniaProperty.Register<GaugeChart, double>(nameof(GaugeNeedleWidth), double.NaN, validate: value => double.IsNaN(value) || (double.IsFinite(value) && value > 0 && value <= 50));
    public static readonly StyledProperty<double> GaugeValueFontSizeProperty = AvaloniaProperty.Register<GaugeChart, double>(nameof(GaugeValueFontSize), double.NaN, validate: value => double.IsNaN(value) || (double.IsFinite(value) && value > 0 && value <= 200));
    public static readonly StyledProperty<double> GaugeValueOffsetProperty = AvaloniaProperty.Register<GaugeChart, double>(nameof(GaugeValueOffset), double.NaN, validate: value => double.IsNaN(value) || (double.IsFinite(value) && value >= -1 && value <= 1));
    public static readonly StyledProperty<double> GaugeGradeLabelRotationProperty = AvaloniaProperty.Register<GaugeChart, double>(nameof(GaugeGradeLabelRotation), double.NaN, validate: value => double.IsNaN(value) || (double.IsFinite(value) && value >= -90 && value <= 90));
    public static readonly StyledProperty<double> GaugeGradeLabelGapProperty = AvaloniaProperty.Register<GaugeChart, double>(nameof(GaugeGradeLabelGap), double.NaN, validate: value => double.IsNaN(value) || (double.IsFinite(value) && value >= 0 && value <= 100));
    public static readonly StyledProperty<double> GaugeCaptionOffsetProperty = AvaloniaProperty.Register<GaugeChart, double>(nameof(GaugeCaptionOffset), double.NaN, validate: value => double.IsNaN(value) || (double.IsFinite(value) && value >= -1 && value <= 1));
    public static readonly StyledProperty<string?> GaugeCaptionProperty = AvaloniaProperty.Register<GaugeChart, string?>(nameof(GaugeCaption));
    public static readonly StyledProperty<IBrush?> GaugeTrackBrushProperty = AvaloniaProperty.Register<GaugeChart, IBrush?>(nameof(GaugeTrackBrush));
    public static readonly StyledProperty<IBrush?> GaugeNeedleBrushProperty = AvaloniaProperty.Register<GaugeChart, IBrush?>(nameof(GaugeNeedleBrush));
    public static readonly StyledProperty<IBrush?> GaugeTickBrushProperty = AvaloniaProperty.Register<GaugeChart, IBrush?>(nameof(GaugeTickBrush));
    public static readonly StyledProperty<IBrush?> GaugeValueBrushProperty = AvaloniaProperty.Register<GaugeChart, IBrush?>(nameof(GaugeValueBrush));
    public static readonly StyledProperty<IList<GaugeSegment>?> GaugeSegmentsProperty = AvaloniaProperty.Register<GaugeChart, IList<GaugeSegment>?>(nameof(GaugeSegments));
    public static readonly StyledProperty<double> GaugeSecondaryMinimumProperty = AvaloniaProperty.Register<GaugeChart, double>(nameof(GaugeSecondaryMinimum), 0, validate: double.IsFinite);
    public static readonly StyledProperty<double> GaugeSecondaryMaximumProperty = AvaloniaProperty.Register<GaugeChart, double>(nameof(GaugeSecondaryMaximum), 60, validate: double.IsFinite);
    public static readonly StyledProperty<int> GaugeSecondaryTickCountProperty = AvaloniaProperty.Register<GaugeChart, int>(nameof(GaugeSecondaryTickCount), 7, validate: value => value >= 2 && value <= 12);
    public static readonly StyledProperty<IBrush?> GaugeSecondaryBrushProperty = AvaloniaProperty.Register<GaugeChart, IBrush?>(nameof(GaugeSecondaryBrush));

    public GaugeStyle GaugeStyle { get => GetValue(GaugeStyleProperty); set => SetValue(GaugeStyleProperty, value); }
    public double GaugeStartAngle { get => GetValue(GaugeStartAngleProperty); set => SetValue(GaugeStartAngleProperty, value); }
    public double GaugeSweepAngle { get => GetValue(GaugeSweepAngleProperty); set => SetValue(GaugeSweepAngleProperty, value); }
    public bool? ShowGaugeProgress { get => GetValue(ShowGaugeProgressProperty); set => SetValue(ShowGaugeProgressProperty, value); }
    public bool? ShowGaugeAnchor { get => GetValue(ShowGaugeAnchorProperty); set => SetValue(ShowGaugeAnchorProperty, value); }
    public bool ShowGaugeValue { get => GetValue(ShowGaugeValueProperty); set => SetValue(ShowGaugeValueProperty, value); }
    public bool ShowGaugeTickLabels { get => GetValue(ShowGaugeTickLabelsProperty); set => SetValue(ShowGaugeTickLabelsProperty, value); }
    public GaugeNeedleShape GaugeNeedleShape { get => GetValue(GaugeNeedleShapeProperty); set => SetValue(GaugeNeedleShapeProperty, value); }
    public GaugeTickPlacement GaugeTickPlacement { get => GetValue(GaugeTickPlacementProperty); set => SetValue(GaugeTickPlacementProperty, value); }
    public double GaugeNeedleLength { get => GetValue(GaugeNeedleLengthProperty); set => SetValue(GaugeNeedleLengthProperty, value); }
    public double GaugeNeedleWidth { get => GetValue(GaugeNeedleWidthProperty); set => SetValue(GaugeNeedleWidthProperty, value); }
    public double GaugeValueFontSize { get => GetValue(GaugeValueFontSizeProperty); set => SetValue(GaugeValueFontSizeProperty, value); }
    public double GaugeValueOffset { get => GetValue(GaugeValueOffsetProperty); set => SetValue(GaugeValueOffsetProperty, value); }
    /// <summary>Caption top relative to the pivot in radius units; NaN uses the layout default.</summary>
    public double GaugeCaptionOffset { get => GetValue(GaugeCaptionOffsetProperty); set => SetValue(GaugeCaptionOffsetProperty, value); }
    /// <summary>NaN follows the readable arc tangent; zero keeps grade labels horizontal.</summary>
    public double GaugeGradeLabelRotation { get => GetValue(GaugeGradeLabelRotationProperty); set => SetValue(GaugeGradeLabelRotationProperty, value); }
    /// <summary>Minimum gap in pixels between grade text and the band; NaN follows typography.</summary>
    public double GaugeGradeLabelGap { get => GetValue(GaugeGradeLabelGapProperty); set => SetValue(GaugeGradeLabelGapProperty, value); }
    public string? GaugeCaption { get => GetValue(GaugeCaptionProperty); set => SetValue(GaugeCaptionProperty, value); }
    public IBrush? GaugeTrackBrush { get => GetValue(GaugeTrackBrushProperty); set => SetValue(GaugeTrackBrushProperty, value); }
    public IBrush? GaugeNeedleBrush { get => GetValue(GaugeNeedleBrushProperty); set => SetValue(GaugeNeedleBrushProperty, value); }
    public IBrush? GaugeTickBrush { get => GetValue(GaugeTickBrushProperty); set => SetValue(GaugeTickBrushProperty, value); }
    public IBrush? GaugeValueBrush { get => GetValue(GaugeValueBrushProperty); set => SetValue(GaugeValueBrushProperty, value); }
    public IList<GaugeSegment>? GaugeSegments { get => GetValue(GaugeSegmentsProperty); set => SetValue(GaugeSegmentsProperty, value); }
    public double GaugeSecondaryMinimum { get => GetValue(GaugeSecondaryMinimumProperty); set => SetValue(GaugeSecondaryMinimumProperty, value); }
    public double GaugeSecondaryMaximum { get => GetValue(GaugeSecondaryMaximumProperty); set => SetValue(GaugeSecondaryMaximumProperty, value); }
    public int GaugeSecondaryTickCount { get => GetValue(GaugeSecondaryTickCountProperty); set => SetValue(GaugeSecondaryTickCountProperty, value); }
    public IBrush? GaugeSecondaryBrush { get => GetValue(GaugeSecondaryBrushProperty); set => SetValue(GaugeSecondaryBrushProperty, value); }

    private static readonly AvaloniaProperty[] GaugeVisualProperties =
    [
        GaugeStyleProperty,
        GaugeStartAngleProperty,
        GaugeSweepAngleProperty,
        ShowGaugeProgressProperty,
        ShowGaugeAnchorProperty,
        ShowGaugeValueProperty,
        ShowGaugeTickLabelsProperty,
        GaugeNeedleShapeProperty,
        GaugeTickPlacementProperty,
        GaugeNeedleLengthProperty,
        GaugeNeedleWidthProperty,
        GaugeValueFontSizeProperty,
        GaugeValueOffsetProperty,
        GaugeCaptionProperty, GaugeCaptionOffsetProperty, GaugeGradeLabelRotationProperty, GaugeGradeLabelGapProperty,
        GaugeTrackBrushProperty,
        GaugeNeedleBrushProperty,
        GaugeTickBrushProperty,
        GaugeValueBrushProperty,
        GaugeSegmentsProperty,
        GaugeSecondaryMinimumProperty,
        GaugeSecondaryMaximumProperty,
        GaugeSecondaryTickCountProperty,
        GaugeSecondaryBrushProperty,
    ];

    static GaugeChart() => AffectsRender<GaugeChart>(GaugeVisualProperties);

    private bool _watchGaugeSegments;
    private readonly List<Action> _segmentSubscriptions = [];
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == GaugeSegmentsProperty) ObserveSegments();
        if (change.Property == IsAnimatingProperty && !IsAnimating) _gaugeAnimatedPrecision = 0;
        if (change.Property == ValueProperty && ValueFormat == "0.##")
        {
            if (!IsGaugeValueAnimating) _gaugeAnimatedPrecision = 0;
            var previous = change.GetOldValue<double>();
            if (previous != Math.Truncate(previous) || Value != Math.Truncate(Value)) _gaugeAnimatedPrecision = 2;
        }
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _watchGaugeSegments = true;
        ObserveSegments();
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _watchGaugeSegments = false;
        ClearSegmentSubscriptions();
        base.OnDetachedFromVisualTree(e);
    }
    private void ClearSegmentSubscriptions()
    {
        foreach (var unsubscribe in _segmentSubscriptions) unsubscribe();
        _segmentSubscriptions.Clear();
    }
    private void ObserveSegments()
    {
        ClearSegmentSubscriptions();
        if (!_watchGaugeSegments) return;
        if (GaugeSegments is INotifyCollectionChanged source)
        {
            NotifyCollectionChangedEventHandler handler = (_, _) => { ObserveSegments(); InvalidateVisual(); };
            source.CollectionChanged += handler;
            _segmentSubscriptions.Add(() => source.CollectionChanged -= handler);
        }
        if (GaugeSegments is not { } segments) return;
        foreach (var segment in segments.Distinct())
        {
            if (segment is null) continue;
            EventHandler<AvaloniaPropertyChangedEventArgs> handler = (_, _) => InvalidateVisual();
            segment.PropertyChanged += handler;
            _segmentSubscriptions.Add(() => segment.PropertyChanged -= handler);
        }
    }
    private protected override void CopyExportProperties(ChartBase snapshot)
    {
        var gauge = (GaugeChart)snapshot;
        foreach (var property in GaugeVisualProperties)
            if (IsSet(property)) gauge.SetValue(property, GetValue(property));
        if (GaugeSegments is { } segments)
            gauge.GaugeSegments = segments.Where(segment => segment is not null).Select(segment => new GaugeSegment
                { Fraction = segment.Fraction, Brush = segment.Brush, Label = segment.Label }).ToArray();
    }
}
