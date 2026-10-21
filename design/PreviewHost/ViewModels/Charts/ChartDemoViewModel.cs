using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class ChartDemoViewModel : ReactiveObject
{
    private ChartSeries? _comparisonSeries;
    private bool _observingSeries;
    private string _primaryUnit = "", _dataTitle = "月度销售 · 本期与上期";
    private int _revision;

    public ChartDemoViewModel()
    {
        StartObservingSeries();
        Reset();
    }

    public ObservableCollection<ChartSeries> Series { get; } = [];

    public ChartKind Kind
    {
        get;
        set
        {
            if (IsFixedKind && value != field)
                return;
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(KindIndex));
            this.RaisePropertyChanged(nameof(IsGauge));
            this.RaisePropertyChanged(nameof(IsCartesian));
            this.RaisePropertyChanged(nameof(ChartTitle));
            this.RaisePropertyChanged(nameof(Unit));
        }
    }

    public bool IsFixedKind
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int KindIndex
    {
        get => (int)Kind;
        set
        {
            if (!IsFixedKind && value is >= 0 and <= 13)
                Kind = (ChartKind)value;
        }
    }

    public bool Enabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool ShowComparison
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            if (Series.Count > 1)
                Series[1].IsVisible = value;
        }
    } = true;

    public bool HasComparison => Series.Count > 1;

    public bool ShowLegend
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool ShowGrid
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool ShowDataLabels
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool IsGauge => Kind == ChartKind.Gauge;
    public bool IsCartesian => Kind is not (ChartKind.Pie or ChartKind.Doughnut or ChartKind.Gauge);

    public bool ZoomEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool PanEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool SamplingEnabled
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(SamplingMode));
        }
    } = true;

    public ChartSamplingMode SamplingMode => SamplingEnabled ? ChartSamplingMode.MinMax : ChartSamplingMode.None;

    public bool UseSecondaryAxis
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            if (Series.Count > 1)
                Series[1].YAxis = value ? ChartYAxis.Secondary : ChartYAxis.Primary;
        }
    }

    public bool ShowSecondaryAxis
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool ShowCrosshair
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool SharedTooltip
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(TooltipMode));
        }
    } = true;

    public ChartTooltipMode TooltipMode => SharedTooltip ? ChartTooltipMode.Shared : ChartTooltipMode.Single;

    public bool LegendInteractive
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool Smooth
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool AnimateUpdates
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public ChartAnimationStyle[] AnimationStyles { get; } = Enum.GetValues<ChartAnimationStyle>();

    public ChartAnimationStyle AnimationStyle
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public GaugeStyle[] GaugeStyles { get; } = Enum.GetValues<GaugeStyle>();

    public GaugeStyle GaugeStyle
    {
        get;
        set
        {
            if (field == value) return;
            this.RaiseAndSetIfChanged(ref field, value);
            GaugeNeedle = value is not (GaugeStyle.Semicircle or GaugeStyle.Temperature);
            GaugeTicks = value != GaugeStyle.Semicircle;
        }
    }

    public bool GaugeNeedle
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool GaugeTicks
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public int AnimationDurationIndex
    {
        get;
        set
        {
            if (value is < 0 or > 2)
                return;
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(UpdateAnimationDuration));
        }
    } = 1;

    public TimeSpan UpdateAnimationDuration => TimeSpan.FromMilliseconds(AnimationDurationIndex switch
    {
        0 => 200,
        2 => 1000,
        _ => 500
    });

    public string ExportStatus
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "PNG 包含最终数据与图表，不包含光标提示。";

    public Rect Viewport
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(NavigationStatus));
        }
    } = new(0, 0, 1, 1);

    public ChartNavigationAxes NavigationAxes
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(NavigationAxesIndex));
        }
    } = ChartNavigationAxes.Both;

    public int NavigationAxesIndex
    {
        get => (int)NavigationAxes;
        set
        {
            if (value is >= 0 and <= 3)
                NavigationAxes = (ChartNavigationAxes)value;
        }
    }

    public string NavigationStatus =>
        $"横向视野 {Viewport.X:P0}–{Viewport.Right:P0} · 纵向视野 {Viewport.Y:P0}–{Viewport.Bottom:P0} · + / − 缩放，Ctrl + 方向键平移，Ctrl + 0 复位。";

    public string ChartTitle => IsGauge ? "年度目标完成度" : _dataTitle;
    public string Unit => IsGauge ? "%" : _primaryUnit;

    public string SecondaryUnit
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string PrimaryAxisTitle
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string SecondaryAxisTitle
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public int SelectedSeries
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int SelectedPoint
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = -1;

    public double GaugeValue
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 72;

    public string SelectedDetail
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "点击或使用方向键选择数据点。";

    public string Status
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public void Reset()
    {
        _revision = 0;
        Kind = ChartKind.Line;
        SelectedPoint = -1;
        SelectedSeries = 0;
        Enabled = true;
        GaugeValue = 72;
        ShowComparison = true;
        Viewport = new Rect(0, 0, 1, 1);
        NavigationAxes = ChartNavigationAxes.Both;
        ZoomEnabled = true;
        PanEnabled = true;
        SamplingEnabled = true;
        ShowSecondaryAxis = true;
        UseSecondaryAxis = false;
        SetDataAxes("月度销售 · 本期与上期");
        ShowCrosshair = true;
        SharedTooltip = true;
        LegendInteractive = true;
        Smooth = false;
        AnimateUpdates = true;
        GaugeStyle = GaugeStyle.Semicircle;
        GaugeNeedle = true;
        GaugeTicks = true;
        AnimationDurationIndex = 1;
        Series.Clear();
        Series.Add(new ChartSeries
        {
            Name = "本期",
            ItemsSource = new ObservableCollection<ChartPoint>(
                new[] { 42d, 58, 51, 76, 68, 92 }.Select((value, index) =>
                    new ChartPoint(index, value, $"{index + 1}月")))
        });
        Series.Add(new ChartSeries
        {
            Name = "上期",
            ItemsSource = new ObservableCollection<ChartPoint>(
                new[] { 34d, 43, 49, 56, 61, 70 }.Select((value, index) =>
                    new ChartPoint(index, value, $"{index + 1}月")))
        });
        SelectedDetail = "点击或使用方向键选择数据点。";
        UpdateStatus();
    }

    public void UpdateData()
    {
        _revision++;
        foreach (var series in Series)
            if (series.ItemsSource is { } points)
                for (var index = 0; index < points.Count; index++)
                    if (double.IsFinite(points[index].Y))
                        points[index].Y += Math.Sin(index + _revision) * 6;
        GaugeValue = (GaugeValue + 13) % 101;
        if (SelectedSeries >= 0 && SelectedSeries < Series.Count &&
            Series[SelectedSeries].ItemsSource is { } selectedPoints && SelectedPoint >= 0 &&
            SelectedPoint < selectedPoints.Count)
            UpdateSelectionDetail(new ChartSelection(SelectedSeries, SelectedPoint, Series[SelectedSeries],
                selectedPoints[SelectedPoint]));
        UpdateStatus();
    }

    public void LoadThousandPoints()
    {
        UseSecondaryAxis = false;
        SetDataAxes("实时趋势 · 1,000 个数据点");
        Kind = ChartKind.Line;
        Viewport = new Rect(0, 0, 1, 1);
        SelectedPoint = -1;
        SelectedSeries = 0;
        Series.Clear();
        Series.Add(new ChartSeries
        {
            Name = "实时趋势",
            ItemsSource = new ObservableCollection<ChartPoint>(Enumerable.Range(0, 1000).Select(index =>
                new ChartPoint(index, 50 + 22 * Math.Sin(index / 24d) + 6 * Math.Cos(index / 7d), $"记录 {index + 1}")))
        });
        UpdateStatus();
    }

    public void LoadGapData()
    {
        UseSecondaryAxis = false;
        SetDataAxes("净变化 · 负数与缺失值");
        Kind = ChartKind.Line;
        Viewport = new Rect(0, 0, 1, 1);
        SelectedPoint = -1;
        SelectedSeries = 0;
        Series.Clear();
        Series.Add(new ChartSeries
        {
            Name = "净变化",
            ItemsSource = new ObservableCollection<ChartPoint>(
                new[] { 20d, -12, -25, double.NaN, 18, 32, double.PositiveInfinity, -8 }.Select((value, index) =>
                    new ChartPoint(index, value, $"时段 {index + 1}")))
        });
        UpdateStatus();
    }

    public void LoadStackData()
    {
        UseSecondaryAxis = false;
        SetDataAxes("业务贡献 · 正负堆叠");
        Kind = ChartKind.StackedBar;
        Viewport = new Rect(0, 0, 1, 1);
        SelectedPoint = -1;
        SelectedSeries = 0;
        ShowComparison = true;
        Series.Clear();
        Series.Add(new ChartSeries
        {
            Name = "主营",
            ItemsSource = new ObservableCollection<ChartPoint>(
                new[] { 28d, 36, -18, 42, -24, 38 }.Select((value, index) =>
                    new ChartPoint(index, value, $"{index + 1}月")))
        });
        Series.Add(new ChartSeries
        {
            Name = "其他",
            ItemsSource = new ObservableCollection<ChartPoint>(
                new[] { 14d, 21, -12, 18, -16, 24 }.Select((value, index) =>
                    new ChartPoint(index, value, $"{index + 1}月")))
        });
        UpdateStatus();
    }

    public void LoadHundredThousandPoints()
    {
        Kind = ChartKind.Line;
        Viewport = new Rect(0, 0, 1, 1);
        SelectedPoint = -1;
        SelectedSeries = 0;
        SamplingEnabled = true;
        UseSecondaryAxis = false;
        SetDataAxes("传感器趋势 · 100,000 条原始记录");
        Series.Clear();
        Series.Add(new ChartSeries
        {
            Name = "传感器",
            ItemsSource = new ObservableCollection<ChartPoint>(Enumerable.Range(0, 100000).Select(index =>
                new ChartPoint(index,
                    index is >= 44000 and < 46000 ? double.NaN :
                    index == 99777 || index % 17003 == 0 ? 95 :
                    index == 99888 || index % 23003 == 0 ? 5 :
                    50 + 18 * Math.Sin(index / 180d) + 4 * Math.Cos(index / 13d), $"记录 {index + 1}")))
        });
        UpdateStatus();
    }

    public void LoadSecondaryData()
    {
        Kind = ChartKind.Line;
        Viewport = new Rect(0, 0, 1, 1);
        SelectedPoint = -1;
        SelectedSeries = 0;
        ShowComparison = true;
        Series.Clear();
        SetDataAxes("销售与访问 · 独立双轴", "万元", "人", "销售额（万元）", "访问人数（人）");
        Series.Add(new ChartSeries
        {
            Name = "销售额",
            ItemsSource = new ObservableCollection<ChartPoint>(
                new[] { 28d, 36, 31, 42, 39, 48 }.Select((value, index) =>
                    new ChartPoint(index, value, $"{index + 1}月")))
        });
        Series.Add(new ChartSeries
        {
            Name = "访问人数", YAxis = ChartYAxis.Secondary,
            ItemsSource = new ObservableCollection<ChartPoint>(
                new[] { 2400d, 3200, 2700, 3800, 3400, 4600 }.Select((value, index) =>
                    new ChartPoint(index, value, $"{index + 1}月")))
        });
        UseSecondaryAxis = true;
        UpdateStatus();
    }

    public void LoadThousandSeries()
    {
        Kind = ChartKind.Line;
        Viewport = new Rect(0, 0, 1, 1);
        SelectedPoint = -1;
        SelectedSeries = 0;
        UseSecondaryAxis = false;
        ShowLegend = true;
        LegendInteractive = true;
        SetDataAxes("系列图例 · 有界分页");
        Series.Clear();
        foreach (var index in Enumerable.Range(0, 1000))
            Series.Add(new ChartSeries
            {
                Name = $"系列 {index + 1}",
                ItemsSource = new[]
                {
                    new ChartPoint(0, 30 + index % 20), new ChartPoint(1, 45 + index % 20),
                    new ChartPoint(2, 35 + index % 20)
                }
            });
        UpdateStatus();
    }

    internal void StartObservingSeries()
    {
        if (_observingSeries)
            return;
        _observingSeries = true;
        Series.CollectionChanged += SeriesChanged;
        ObserveComparisonSeries();
        this.RaisePropertyChanged(nameof(HasComparison));
    }

    internal void StopObservingSeries()
    {
        _observingSeries = false;
        Series.CollectionChanged -= SeriesChanged;
        if (_comparisonSeries is not null)
            _comparisonSeries.PropertyChanged -= ComparisonSeriesChanged;
        _comparisonSeries = null;
    }

    private void SeriesChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        ObserveComparisonSeries();
        this.RaisePropertyChanged(nameof(HasComparison));
    }

    private void ObserveComparisonSeries()
    {
        var next = Series.Count > 1 ? Series[1] : null;
        if (ReferenceEquals(next, _comparisonSeries))
            return;
        if (_comparisonSeries is not null)
            _comparisonSeries.PropertyChanged -= ComparisonSeriesChanged;
        _comparisonSeries = next;
        if (next is null)
            return;
        next.PropertyChanged += ComparisonSeriesChanged;
        ShowComparison = next.IsVisible;
        UseSecondaryAxis = next.YAxis == ChartYAxis.Secondary;
    }

    private void ComparisonSeriesChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (_comparisonSeries is not { } series)
            return;
        if (args.Property == ChartSeries.IsVisibleProperty)
            ShowComparison = series.IsVisible;
        if (args.Property == ChartSeries.YAxisProperty)
            UseSecondaryAxis = series.YAxis == ChartYAxis.Secondary;
    }

    private void SetDataAxes(string title, string primaryUnit = "", string secondaryUnit = "", string primaryTitle = "",
        string secondaryTitle = "")
    {
        _dataTitle = title;
        _primaryUnit = primaryUnit;
        this.RaisePropertyChanged(nameof(ChartTitle));
        this.RaisePropertyChanged(nameof(Unit));
        SecondaryUnit = secondaryUnit;
        PrimaryAxisTitle = primaryTitle;
        SecondaryAxisTitle = secondaryTitle;
    }

    private void UpdateStatus()
    {
        Status =
            $"{Series.Sum(series => series.ItemsSource?.Count ?? 0):N0} 个数据点 · 更新 {_revision} 次 · 数据直接绘制，未为每个数据点创建控件。";
    }

    public void UpdateSelectionDetail(ChartSelection? selection)
    {
        SelectedDetail = selection is { } selected
            ? $"{selected.Series.Name} · {selected.Point.Label ?? selected.Point.X.ToString("0.##")}：{selected.Point.Y:0.##}（第 {selected.PointIndex + 1} 项）"
            : "点击或使用方向键选择数据点。";
    }
}
