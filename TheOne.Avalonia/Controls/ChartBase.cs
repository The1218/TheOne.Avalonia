using System.Globalization;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>A native, AOT-safe chart. Data is drawn directly without creating item controls.</summary>
public abstract partial class ChartBase : Control
{
    private static readonly IBrush[] FallbackSeriesBrushes =
    [
        Brush.Parse("#187576"), Brush.Parse("#295694"), Brush.Parse("#836B1A"),
        Brush.Parse("#74387F"), Brush.Parse("#7E3D46"), Brush.Parse("#187648")
    ];

    public static readonly StyledProperty<IList<ChartSeries>?> SeriesSourceProperty =
        AvaloniaProperty.Register<ChartBase, IList<ChartSeries>?>(nameof(SeriesSource));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ChartBase, string?>(nameof(Title));

    public static readonly StyledProperty<bool> ShowLegendProperty =
        AvaloniaProperty.Register<ChartBase, bool>(nameof(ShowLegend), true);

    public static readonly StyledProperty<bool> ShowGridProperty =
        AvaloniaProperty.Register<ChartBase, bool>(nameof(ShowGrid), true);

    public static readonly StyledProperty<bool> ShowMarkersProperty =
        AvaloniaProperty.Register<ChartBase, bool>(nameof(ShowMarkers), true);

    public static readonly StyledProperty<bool> ShowDataLabelsProperty =
        AvaloniaProperty.Register<ChartBase, bool>(nameof(ShowDataLabels));

    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<ChartBase, double>(nameof(StrokeThickness), 2.5,
            validate: value => double.IsFinite(value) && value > 0);

    public static readonly StyledProperty<double> InnerRadiusProperty =
        AvaloniaProperty.Register<ChartBase, double>(nameof(InnerRadius), 0.62,
            validate: value => double.IsFinite(value) && value >= 0 && value < 1);

    public static readonly StyledProperty<double> YMinimumProperty =
        AvaloniaProperty.Register<ChartBase, double>(nameof(YMinimum), double.NaN,
            validate: value => !double.IsInfinity(value));

    public static readonly StyledProperty<double> YMaximumProperty =
        AvaloniaProperty.Register<ChartBase, double>(nameof(YMaximum), double.NaN,
            validate: value => !double.IsInfinity(value));

    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<ChartBase, double>(nameof(Minimum), validate: double.IsFinite);

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<ChartBase, double>(nameof(Maximum), 100, validate: double.IsFinite);

    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<ChartBase, double>(nameof(Value), validate: double.IsFinite);

    public static readonly StyledProperty<string> ValueFormatProperty =
        AvaloniaProperty.Register<ChartBase, string>(nameof(ValueFormat), "0.##");

    public static readonly StyledProperty<string?> UnitProperty =
        AvaloniaProperty.Register<ChartBase, string?>(nameof(Unit));

    public static readonly StyledProperty<string> EmptyTextProperty =
        AvaloniaProperty.Register<ChartBase, string>(nameof(EmptyText), "暂无数据");

    public static readonly StyledProperty<int> SelectedSeriesIndexProperty =
        AvaloniaProperty.Register<ChartBase, int>(nameof(SelectedSeriesIndex), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<ChartBase, int>(nameof(SelectedIndex), -1, defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IBrush?> BackgroundProperty = Border.BackgroundProperty.AddOwner<ChartBase>();

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        TextBlock.ForegroundProperty.AddOwner<ChartBase>();

    public static readonly StyledProperty<IBrush?> GridBrushProperty =
        AvaloniaProperty.Register<ChartBase, IBrush?>(nameof(GridBrush));

    public static readonly StyledProperty<IBrush?> MutedBrushProperty =
        AvaloniaProperty.Register<ChartBase, IBrush?>(nameof(MutedBrush));

    public static readonly StyledProperty<IBrush?> AccentBrushProperty =
        AvaloniaProperty.Register<ChartBase, IBrush?>(nameof(AccentBrush));

    public static readonly StyledProperty<IBrush?> SecondaryBrushProperty =
        AvaloniaProperty.Register<ChartBase, IBrush?>(nameof(SecondaryBrush));

    public static readonly StyledProperty<IBrush?> TertiaryBrushProperty =
        AvaloniaProperty.Register<ChartBase, IBrush?>(nameof(TertiaryBrush));

    public static readonly StyledProperty<IBrush?> QuaternaryBrushProperty =
        AvaloniaProperty.Register<ChartBase, IBrush?>(nameof(QuaternaryBrush));

    public static readonly StyledProperty<IBrush?> FifthBrushProperty =
        AvaloniaProperty.Register<ChartBase, IBrush?>(nameof(FifthBrush));

    public static readonly StyledProperty<IBrush?> SixthBrushProperty =
        AvaloniaProperty.Register<ChartBase, IBrush?>(nameof(SixthBrush));

    public static readonly StyledProperty<FontFamily> FontFamilyProperty =
        TextBlock.FontFamilyProperty.AddOwner<ChartBase>();

    public static readonly StyledProperty<double> FontSizeProperty = TextBlock.FontSizeProperty.AddOwner<ChartBase>();

    public static readonly DirectProperty<ChartBase, ChartSelection?> SelectionProperty =
        AvaloniaProperty.RegisterDirect<ChartBase, ChartSelection?>(nameof(Selection), chart => chart.Selection);

    private readonly Dictionary<(int Series, int Index), Point> _positions = [];
    private bool _geometryDirty = true;
    private Size _geometrySize;
    private long _geometryVersion;
    private (int Series, int Index)? _hover;
    private bool _observing;
    private bool _pendingSelectionReveal;
    private bool _rendering;

    private ChartSelection? _selection;
    private bool _updatingSelection;

    static ChartBase()
    {
        FocusableProperty.OverrideDefaultValue<ChartBase>(true);
        RegisterChartSizingMeasure();
        AffectsRender<ChartBase>(TitleProperty, ShowLegendProperty, ShowGridProperty, ShowMarkersProperty,
            ShowDataLabelsProperty, StrokeThicknessProperty, InnerRadiusProperty, YMinimumProperty, YMaximumProperty,
            MinimumProperty, MaximumProperty, ValueProperty, ValueFormatProperty, UnitProperty, EmptyTextProperty,
            SelectedSeriesIndexProperty, SelectedIndexProperty, BackgroundProperty, ForegroundProperty,
            GridBrushProperty,
            MutedBrushProperty, AccentBrushProperty, SecondaryBrushProperty, TertiaryBrushProperty,
            QuaternaryBrushProperty, FifthBrushProperty, SixthBrushProperty,
            FontFamilyProperty, FontSizeProperty, ShowCrosshairProperty, TooltipModeProperty,
            IsLegendInteractiveProperty);
    }

    protected ChartBase()
    {
        ActualThemeVariantChanged += (_, _) =>
        {
            MarkGeometryDirty();
            ClearPointerHover();
        };
        SizeChanged += (_, _) =>
        {
            ClearPointerHover();
            CancelLegendPress();
            if (!_pendingSelectionReveal || Bounds.Width < 48 || Bounds.Height < 48) return;
            _pendingSelectionReveal = false;
            if (Selection is { } selection) BringDataPointIntoView(selection.SeriesIndex, selection.PointIndex);
        };
    }

    private protected static IBrush DefaultAccentBrush => FallbackSeriesBrushes[0];

    public abstract ChartKind Kind { get; }

    public IList<ChartSeries>? SeriesSource
    {
        get => GetValue(SeriesSourceProperty);
        set => SetValue(SeriesSourceProperty, value);
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool ShowLegend
    {
        get => GetValue(ShowLegendProperty);
        set => SetValue(ShowLegendProperty, value);
    }

    public bool ShowGrid
    {
        get => GetValue(ShowGridProperty);
        set => SetValue(ShowGridProperty, value);
    }

    public bool ShowMarkers
    {
        get => GetValue(ShowMarkersProperty);
        set => SetValue(ShowMarkersProperty, value);
    }

    public bool ShowDataLabels
    {
        get => GetValue(ShowDataLabelsProperty);
        set => SetValue(ShowDataLabelsProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public double InnerRadius
    {
        get => GetValue(InnerRadiusProperty);
        set => SetValue(InnerRadiusProperty, value);
    }

    public double YMinimum
    {
        get => GetValue(YMinimumProperty);
        set => SetValue(YMinimumProperty, value);
    }

    public double YMaximum
    {
        get => GetValue(YMaximumProperty);
        set => SetValue(YMaximumProperty, value);
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

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string ValueFormat
    {
        get => GetValue(ValueFormatProperty);
        set => SetValue(ValueFormatProperty, value);
    }

    public string? Unit
    {
        get => GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public string EmptyText
    {
        get => GetValue(EmptyTextProperty);
        set => SetValue(EmptyTextProperty, value);
    }

    public int SelectedSeriesIndex
    {
        get => GetValue(SelectedSeriesIndexProperty);
        set => SetValue(SelectedSeriesIndexProperty, value);
    }

    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    public IBrush? GridBrush
    {
        get => GetValue(GridBrushProperty);
        set => SetValue(GridBrushProperty, value);
    }

    public IBrush? MutedBrush
    {
        get => GetValue(MutedBrushProperty);
        set => SetValue(MutedBrushProperty, value);
    }

    public IBrush? AccentBrush
    {
        get => GetValue(AccentBrushProperty);
        set => SetValue(AccentBrushProperty, value);
    }

    public IBrush? SecondaryBrush
    {
        get => GetValue(SecondaryBrushProperty);
        set => SetValue(SecondaryBrushProperty, value);
    }

    public IBrush? TertiaryBrush
    {
        get => GetValue(TertiaryBrushProperty);
        set => SetValue(TertiaryBrushProperty, value);
    }

    public IBrush? QuaternaryBrush
    {
        get => GetValue(QuaternaryBrushProperty);
        set => SetValue(QuaternaryBrushProperty, value);
    }

    public IBrush? FifthBrush
    {
        get => GetValue(FifthBrushProperty);
        set => SetValue(FifthBrushProperty, value);
    }

    public IBrush? SixthBrush
    {
        get => GetValue(SixthBrushProperty);
        set => SetValue(SixthBrushProperty, value);
    }

    public FontFamily FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public ChartSelection? Selection => _selection;
    private protected virtual bool IsRadial => false;
    private protected virtual bool IsGauge => false;
    private protected virtual bool HasRadialHole => false;
    private protected virtual bool IsCategoricalCartesian => false;
    private protected virtual bool IsHorizontalCartesian => false;
    private protected virtual bool IsStackedCartesian => false;
    private protected virtual bool IsPercentCartesian => false;
    private protected virtual bool IsStackedAreaCartesian => IsStackedCartesian && !IsCategoricalCartesian;
    private protected virtual bool IsAreaCartesian => false;
    private protected virtual bool IsScatterCartesian => false;
    private protected virtual bool DrawsLineCartesian => false;
    private protected virtual bool SupportsSmooth => false;
    public event EventHandler<ChartSelectionChangedEventArgs>? SelectionChanged;
    private protected abstract void PrepareChartGeometry(Rect plot);
    private protected abstract void DrawChartPlot(DrawingContext context, Rect plot);
    private protected abstract ChartBase CreateExportSnapshot();

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(DefaultChartWidth, DefaultChartHeight);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _observing = true;
        RefreshData();
        _pendingSelectionReveal = Selection is not null;
        if (_pendingSelectionReveal && Bounds.Width >= 48 && Bounds.Height >= 48 && Selection is { } selection)
        {
            _pendingSelectionReveal = false;
            BringDataPointIntoView(selection.SeriesIndex, selection.PointIndex);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (IsScatterCartesian)
        {
            ClearScatterMarkerBrushes();
            ClearScatterTextFrames();
        }
        _animateDataPending = false;
        FinishDataAnimation();
        CancelPan();
        CancelLegendPress();
        ClearPointerHover();
        _legendKeyboard = false;
        _pendingSelectionReveal = false;
        _observing = false;
        Unsubscribe();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty && IsGauge)
        {
            MarkDataUpdate();
        }
        else if (change.Property == SeriesSourceProperty)
        {
        }
        else if (change.Property == SelectedIndexProperty || change.Property == SelectedSeriesIndexProperty)
        {
            _animateDataPending = false;
            FinishDataAnimation();
        }
        else if (AffectsChartGeometry(change.Property))
        {
            _animateDataPending = false;
            FinishDataAnimation();
            MarkGeometryDirty();
        }

        if (change.Property == IsAnimationEnabledProperty || change.Property == AnimationDurationProperty ||
            change.Property == Motion.EnabledProperty ||
            change.Property == IsEffectivelyEnabledProperty || change.Property == IsVisibleProperty)
            if (!CanAnimateData())
            {
                _animateDataPending = false;
                FinishDataAnimation();
            }

        if (change.Property == SelectedIndexProperty || change.Property == SelectedSeriesIndexProperty)
        {
            _hoverGeometryVersion = _geometryVersion;
        }
        else if (AffectsChartGeometry(change.Property) || change.Property == ShowCrosshairProperty ||
                 change.Property == TooltipModeProperty || change.Property == IsLegendInteractiveProperty ||
                 change.Property == IsEffectivelyEnabledProperty)
        {
            ClearPointerHover();
            CancelLegendPress();
            if (!IsEffectivelyEnabled || IsRadial || !ShowLegend || !IsLegendInteractive)
            {
                _legendKeyboard = false;
                RestoreSelectionAutomation();
            }
        }

        HandleNavigationChange(change);
        if (change.Property == SeriesSourceProperty) RefreshData();
        var formatChanged = change.Property == ValueFormatProperty || change.Property == UnitProperty ||
                            change.Property == SecondaryValueFormatProperty || change.Property == SecondaryUnitProperty;
        if (formatChanged)
        {
            _hover = null;
            ToolTip.SetTip(this, null);
        }

        if (!_updatingSelection && (change.Property == SelectedIndexProperty ||
                                    change.Property == SelectedSeriesIndexProperty || formatChanged))
            UpdateSelection();
    }

    /// <summary>Refresh a plain IList after modifying its contents. Use observable collections for automatic updates.</summary>
    public void RefreshData()
    {
        ReconcileSubscriptions();
        DataChanged();
    }

    private bool IsValid(ChartPoint point)
    {
        return double.IsFinite(point.X) && double.IsFinite(point.Y) && (!IsRadial || point.Y > 0);
    }

    private ChartSelection? GetSelection(int seriesIndex, int pointIndex)
    {
        if (IsGauge || SeriesSource is not { } source || seriesIndex < 0 || seriesIndex >= source.Count) return null;
        var series = source[seriesIndex];
        if (!series.IsVisible || series.ItemsSource is not { } points || pointIndex < 0 || pointIndex >= points.Count ||
            !IsValid(points[pointIndex])) return null;
        if (IsRadial && seriesIndex != FirstVisibleSeries()) return null;
        if (IsStackedAreaCartesian && !HasCanonicalStackX(seriesIndex, pointIndex)) return null;
        return new ChartSelection(seriesIndex, pointIndex, series, points[pointIndex]);
    }

    private bool HasSelectablePoints(int seriesIndex)
    {
        if (SeriesSource is not { } source || seriesIndex < 0 || seriesIndex >= source.Count ||
            source[seriesIndex].ItemsSource is not { } points) return false;
        for (var index = 0; index < points.Count; index++)
            if (GetSelection(seriesIndex, index) is not null)
                return true;
        return false;
    }

    private void UpdateSelection()
    {
        var next = GetSelection(SelectedSeriesIndex, SelectedIndex);
        var changed = SetAndRaise(SelectionProperty, ref _selection, next);
        AutomationProperties.SetItemStatus(this, next is null ? "" : Describe(next));
        if (changed)
        {
            PreserveSelectedSample(next);
            if (next is not null && _observing && !_suppressSelectionReveal)
                BringDataPointIntoView(next.SeriesIndex, next.PointIndex);
            SelectionChanged?.Invoke(this, new ChartSelectionChangedEventArgs(next));
        }
    }

    private void SelectPoint(int series, int index)
    {
        _updatingSelection = true;
        try
        {
            SetCurrentValue(SelectedSeriesIndexProperty, series);
            SetCurrentValue(SelectedIndexProperty, index);
        }
        finally
        {
            _updatingSelection = false;
        }

        UpdateSelection();
    }

    private int FirstVisibleSeries()
    {
        if (SeriesSource is { } source)
            for (var i = 0; i < source.Count; i++)
                if (source[i].IsVisible && source[i].ItemsSource is { Count: > 0 })
                    return i;
        return -1;
    }

    private string Describe(ChartSelection selection, bool bounded = false)
    {
        var secondary = !IsRadial && selection.Series.YAxis == ChartYAxis.Secondary;
        var format = selection.Series.ValueFormat ?? (secondary ? SecondaryValueFormat : ValueFormat);
        var unit = selection.Series.Unit ?? (secondary ? SecondaryUnit : Unit);
        string value;
        try
        {
            value = selection.Point.Y.ToString(format, CultureInfo.CurrentCulture);
        }
        catch (FormatException)
        {
            value = selection.Point.Y.ToString("0.##", CultureInfo.CurrentCulture);
        }

        var name = selection.Series.Name;
        var label = selection.Point.Label ?? FormatValue(selection.Point.X);
        if (bounded)
        {
            name = BoundedHoverName(name);
            label = BoundedHoverName(label);
            if (unit is not null) unit = BoundedHoverName(unit);
            if (value.Length > 64) value = selection.Point.Y.ToString("R", CultureInfo.CurrentCulture);
            value = value.Replace('\r', ' ').Replace('\n', ' ');
        }

        var description = $"{name} · {label}: {value}{unit}";
        if (GetStackPercentage(selection.SeriesIndex, selection.PointIndex) is { } percentage)
            description += $" · {percentage:P1}";
        return description;
    }

    private protected string FormatValue(double value)
    {
        try
        {
            return value.ToString(ValueFormat, CultureInfo.CurrentCulture);
        }
        catch (FormatException)
        {
            return value.ToString("0.##", CultureInfo.CurrentCulture);
        }
    }

    private IBrush SeriesBrush(int series, int point = -1)
    {
        if (SeriesSource is { } source && series >= 0 && series < source.Count &&
            source[series].Brush is { } brush) return brush;
        return (Math.Abs(point >= 0 ? point : series) % 6) switch
        {
            1 => SecondaryBrush ?? FallbackSeriesBrushes[1],
            2 => TertiaryBrush ?? FallbackSeriesBrushes[2],
            3 => QuaternaryBrush ?? FallbackSeriesBrushes[3],
            4 => FifthBrush ?? AccentBrush ?? FallbackSeriesBrushes[4],
            5 => SixthBrush ?? SecondaryBrush ?? FallbackSeriesBrushes[5],
            _ => AccentBrush ?? DefaultAccentBrush
        };
    }

    private protected FormattedText Text(string value, IBrush? brush = null, double? size = null)
    {
        return new FormattedText(value, CultureInfo.CurrentCulture, FlowDirection, new Typeface(FontFamily),
            size ?? FontSize,
            brush ?? Foreground ?? Brushes.Black);
    }

    private protected void DrawText(DrawingContext? context, string value, Point position, IBrush? brush = null,
        double? size = null, bool centered = false)
    {
        if (context is null || string.IsNullOrEmpty(value)) return;
        var text = Text(value, brush, size);
        context.DrawText(text, centered ? position - new Vector(text.Width / 2, 0) : position);
    }

    private Rect PlotBounds()
    {
        var titleHeight = string.IsNullOrEmpty(Title) ? ChartPadding : FontSize + ChartPadding + 16;
        var legendHeight = ShowLegend && !IsGauge ? FontSize + ChartPadding + 10 : ChartPadding;
        return new Rect(ChartPadding, titleHeight, Math.Max(0, Bounds.Width - ChartPadding * 2),
            Math.Max(0, Bounds.Height - titleHeight - legendHeight));
    }

    private void BuildGeometry(DrawingContext? context)
    {
        var plot = PlotBounds();
        if (_geometryDirty || _geometrySize != Bounds.Size)
        {
            if (_geometrySize != Bounds.Size)
            {
                _animateDataPending = false;
                FinishDataAnimation(false);
            }

            var origin = CaptureAnimationOrigin();
            var version = _geometryVersion;
            var size = Bounds.Size;
            var preparedGaugeValue = Value;
            _positions.Clear();
            _bars.Clear();
            _normalizedPositions.Clear();
            _cartesianPlot = default;
            BuildLegendLayout();
            if (plot.Width >= 24 && plot.Height >= 24)
            {
                PrepareChartGeometry(plot);
                if (IsRadial) SetSamplingDiagnostics(0, false);
            }
            else
            {
                SetSamplingDiagnostics(0, false);
            }

            _geometrySize = size;
            _geometryDirty = version != _geometryVersion;
            if (_geometryDirty)
            {
                _positions.Clear();
                _bars.Clear();
                _normalizedPositions.Clear();
                _cartesianPlot = default;
            }

            if (!_geometryDirty)
            {
                _hasPreparedGeometry = true;
                _preparedGaugeValue = preparedGaugeValue;
                StartDataAnimation(origin);
            }
            else
            {
                origin?.Bitmap?.Dispose();
                _hasPreparedGeometry = false;
                _animateDataPending = false;
                FinishDataAnimation();
            }
        }

        // Diagnostics can synchronously replace data. Never paint indices prepared for an older source.
        if (_geometryDirty || context is null || plot.Width < 24 || plot.Height < 24) return;
        DrawChartPlot(context, plot);
    }

    private void MarkGeometryDirty()
    {
        _geometryVersion++;
        _geometryDirty = true;
        InvalidateVisual();
    }

    private static bool AffectsChartGeometry(AvaloniaProperty property)
    {
        return property == SeriesSourceProperty || property == TitleProperty ||
               property == ShowLegendProperty || property == ShowGridProperty || property == ShowMarkersProperty ||
               property == ShowDataLabelsProperty ||
               property == StrokeThicknessProperty || property == InnerRadiusProperty || property == YMinimumProperty ||
               property == YMaximumProperty ||
               property == MinimumProperty || property == MaximumProperty || property == ValueProperty ||
               property == ValueFormatProperty || property == UnitProperty ||
               property == BackgroundProperty || property == ForegroundProperty ||
               property == GridBrushProperty || property == MutedBrushProperty || property == AccentBrushProperty ||
               property == SecondaryBrushProperty ||
               property == TertiaryBrushProperty || property == QuaternaryBrushProperty ||
               property == FifthBrushProperty || property == SixthBrushProperty || property == FontFamilyProperty ||
               property == FontSizeProperty ||
               property == FlowDirectionProperty || property == ViewportProperty ||
               property == SecondaryYMinimumProperty || property == SecondaryYMaximumProperty ||
               property == YAxisTitleProperty || property == SecondaryYAxisTitleProperty ||
               property == SecondaryValueFormatProperty || property == SecondaryUnitProperty ||
               property == ShowSecondaryAxisProperty || property == SamplingModeProperty ||
               property == SamplingThresholdProperty || property == SamplingPointBudgetProperty ||
               property == IsSmoothProperty || IsChartSizingProperty(property) || IsChartGaugeProperty(property);
    }

    /// <summary>The center of a visible mark in local coordinates; null for a missing or clipped point.</summary>
    public Point? GetDataPointPosition(int seriesIndex, int pointIndex)
    {
        using var stable = new StableDataFrame(this);
        BuildGeometry(null);
        return !_geometryDirty && _positions.TryGetValue((seriesIndex, pointIndex), out var point)
            ? IsRadial ? GetRadialPresentedPosition((seriesIndex, pointIndex), point) : point
            : null;
    }

    /// <summary>Find the data mark under a local-coordinate pointer.</summary>
    public ChartSelection? HitTestDataPoint(Point point)
    {
        using var stable = new StableDataFrame(this);
        BuildGeometry(null);
        if (_geometryDirty) return null;
        var hit = IsRadial ? HitTestRadial(point) : HitTestCartesian(point);
        return hit is { } index ? GetSelection(index.Series, index.Index) : null;
    }

    public override void Render(DrawingContext context)
    {
        _rendering = true;
        try
        {
            RenderChart(context);
        }
        finally
        {
            _rendering = false;
        }
    }

    private void RenderChart(DrawingContext context)
    {
        base.Render(context);
        using var clip = context.PushClip(new Rect(Bounds.Size));
        if (!_skipExportBackground) context.DrawRectangle(Background, null, new Rect(Bounds.Size));
        using var opacity = context.PushOpacity(IsEffectivelyEnabled ? 1 : 0.45);
        DrawText(context, Title ?? "", new Point(ChartPadding, Math.Max(2, ChartPadding - 2)), size: FontSize + 2);
        BuildGeometry(context);
        if (!IsGauge && (IsRadial ? _positions.Count == 0 : _normalizedPositions.Count == 0))
            DrawText(context, EmptyText, new Point(Bounds.Width / 2, Bounds.Height / 2), MutedBrush, centered: true);
        if (ShowLegend && !IsGauge) DrawLegend(context);
        if (!IsGaugeValueAnimating && !_isExportSnapshot) DrawPointerOverlay(context);
        if (IsFocused && !_isExportSnapshot)
        {
            var pen = IsScatterCartesian && _cartesianFrame is { } frame
                ? ScatterFramePen(frame, 3, AccentBrush, SelectionThickness) : new Pen(AccentBrush, SelectionThickness);
            context.DrawRectangle(null, pen, new Rect(Bounds.Size).Deflate(1), 6, 6);
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        using var stable = new StableDataFrame(this);
        base.OnPointerMoved(e);
        if (MoveLegendPress(e)) return;
        if (MovePan(e)) return;
        UpdatePointerHover(e.GetPosition(this));
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        CancelLegendPress();
        ClearPointerHover();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        using var stable = new StableDataFrame(this);
        base.OnPointerPressed(e);
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        if (!IsEffectivelyEnabled) return;
        Focus();
        if (BeginLegendPress(e)) return;
        if (_legendKeyboard)
        {
            _legendKeyboard = false;
            RestoreSelectionAutomation();
            InvalidateVisual();
        }

        if (BeginPan(e)) return;
        if (HitTestDataPoint(e.GetPosition(this)) is { } hit)
        {
            SelectPoint(hit.SeriesIndex, hit.PointIndex);
            e.Handled = true;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        using var stable = new StableDataFrame(this);
        base.OnKeyDown(e);
        if (HandleLegendKey(e))
        {
            e.Handled = true;
            return;
        }

        if (HandleNavigationKey(e))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape) CancelPan();
        if (IsGauge || SeriesSource is not { Count: > 0 } source) return;
        if (e.Key == Key.Escape)
        {
            SetCurrentValue(SelectedIndexProperty, -1);
            e.Handled = true;
            return;
        }

        var series = Math.Clamp(SelectedSeriesIndex, 0, source.Count - 1);
        if (!HasSelectablePoints(series))
            series = Enumerable.Range(0, source.Count).FirstOrDefault(HasSelectablePoints, -1);
        if (IsRadial) series = FirstVisibleSeries();
        if (series < 0) return;
        if (e.Key is Key.Up or Key.Down && !IsRadial)
        {
            var direction = e.Key == Key.Up ? -1 : 1;
            for (var s = series + direction; s >= 0 && s < source.Count; s += direction)
                if (HasSelectablePoints(s))
                {
                    series = s;
                    break;
                }
        }
        else if (e.Key is not (Key.Left or Key.Right or Key.Home or Key.End))
        {
            return;
        }

        if (series < 0 || source[series].ItemsSource is not { Count: > 0 } points) return;
        var step = e.Key is Key.Left or Key.End ? -1 : 1;
        var index = e.Key switch
        {
            Key.Home => 0, Key.End => points.Count - 1,
            Key.Left => SelectedIndex < 0 ? points.Count - 1 : SelectedIndex - 1, Key.Right => SelectedIndex + 1,
            _ => Math.Clamp(SelectedIndex, 0, points.Count - 1)
        };
        if (e.Key is Key.Up or Key.Down && GetSelection(series, index) is null)
        {
            var fallback = Enumerable.Range(0, points.Count).Where(i => GetSelection(series, i) is not null)
                .OrderBy(i => Math.Abs(i - index)).FirstOrDefault(-1);
            if (fallback >= 0) index = fallback;
        }

        for (; index >= 0 && index < points.Count; index += step)
            if (GetSelection(series, index) is not null)
            {
                SelectPoint(series, index);
                break;
            }

        e.Handled = true;
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        _legendKeyboard = false;
        CancelLegendPress();
        ClearPointerHover();
        RestoreSelectionAutomation();
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new ChartAutomationPeer(this);
    }

    private sealed class ChartAutomationPeer(ChartBase chart) : ControlAutomationPeer(chart)
    {
        protected override string GetClassNameCore()
        {
            return chart.GetType().Name;
        }

        protected override string? GetNameCore()
        {
            return !string.IsNullOrEmpty(base.GetNameCore())
                ? base.GetNameCore()
                : chart.Title ?? chart.Kind.ToString();
        }

        protected override string? GetHelpTextCore()
        {
            return chart.IsGauge
                ? $"{chart.FormatValue(chart.Value)}{chart.Unit}; {chart.Minimum}–{chart.Maximum}"
                : chart._legendKeyboard
                    ? "图例模式: 方向键 / Home / End 选择系列; Space / Enter 显示或隐藏; F6 / Escape 返回数据导航"
                    : chart.Selection is { } selection
                        ? chart.Describe(selection)
                        : "← / →: 数据点; Home / End: 首尾; ↑ / ↓: 数据系列; Escape: 清除选择; F6: 图例导航";
        }
    }
}
