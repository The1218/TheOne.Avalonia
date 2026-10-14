using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TheOne.Avalonia.Controls;
using Avalonia;

namespace TheOne.NativeAotSmoke;

public abstract class Observable : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); return true;
    }
}
public sealed class AuditRow(string caption) : Observable
{
    private string _caption = caption; private bool _expanded;
    public string Caption { get => _caption; set => Set(ref _caption, value); }
    public bool Expanded { get => _expanded; set => Set(ref _expanded, value); }
    public ObservableCollection<AuditRow> Children { get; } = [];
}
public sealed class AuditModel : Observable
{
    private string _draft = "Compiled consumer"; private bool _selected; private bool _expanded;
    private decimal? _number = 12; private double _range = 35; private DateTime? _date = new(2026, 10, 13);
    private DateTimeOffset? _offsetDate = new DateTimeOffset(2026, 10, 13, 0, 0, 0, TimeSpan.Zero); private TimeSpan? _time = new(9, 30, 0);
    private AuditRow? _choice; private AuditRow _page = new("First page");
    private GaugeStyle _gaugeStyle;
    public GaugeStyle GaugeStyle { get => _gaugeStyle; set => Set(ref _gaugeStyle, value); }
    private ChartKind _chartKind = ChartKind.Line; private int _chartSeriesIndex, _chartPointIndex = -1;
    private Rect _chartViewport = new(0, 0, 1, 1); private ChartNavigationAxes _chartAxes = ChartNavigationAxes.Both;
    private ChartSamplingMode _chartSamplingMode = ChartSamplingMode.MinMax; private int _chartSamplingBudget = 512;
    private string _chartSecondaryTitle = "Secondary consumer scale";
    private bool _chartCrosshair = true, _chartSharedTooltip = true, _chartLegendInteractive = true;
    private ChartTooltipMode _chartTooltipMode = ChartTooltipMode.Shared;
    private bool _chartSmooth, _chartAnimateUpdates = true, _chartGaugeNeedle = true, _chartGaugeTicks = true;
    private TimeSpan _chartAnimationDuration = TimeSpan.FromMilliseconds(500);
    public string Draft { get => _draft; set => Set(ref _draft, value); }
    public bool Selected { get => _selected; set => Set(ref _selected, value); }
    public bool Expanded { get => _expanded; set => Set(ref _expanded, value); }
    public decimal? Number { get => _number; set => Set(ref _number, value); }
    public double Range { get => _range; set => Set(ref _range, value); }
    public DateTime? Date { get => _date; set => Set(ref _date, value); }
    public DateTimeOffset? OffsetDate { get => _offsetDate; set => Set(ref _offsetDate, value); }
    public TimeSpan? Time { get => _time; set => Set(ref _time, value); }
    public AuditRow? Choice { get => _choice; set => Set(ref _choice, value); }
    public AuditRow Page { get => _page; set => Set(ref _page, value); }
    public ChartKind ChartKind { get => _chartKind; set => Set(ref _chartKind, value); }
    public int ChartSeriesIndex { get => _chartSeriesIndex; set => Set(ref _chartSeriesIndex, value); }
    public int ChartPointIndex { get => _chartPointIndex; set => Set(ref _chartPointIndex, value); }
    public Rect ChartViewport { get => _chartViewport; set => Set(ref _chartViewport, value); }
    public ChartNavigationAxes ChartAxes { get => _chartAxes; set => Set(ref _chartAxes, value); }
    public ChartSamplingMode ChartSamplingMode { get => _chartSamplingMode; set => Set(ref _chartSamplingMode, value); }
    public int ChartSamplingBudget { get => _chartSamplingBudget; set => Set(ref _chartSamplingBudget, value); }
    public string ChartSecondaryTitle { get => _chartSecondaryTitle; set => Set(ref _chartSecondaryTitle, value); }
    public bool ChartCrosshair { get => _chartCrosshair; set => Set(ref _chartCrosshair, value); }
    public bool ChartSharedTooltip { get => _chartSharedTooltip; set { Set(ref _chartSharedTooltip, value); Set(ref _chartTooltipMode, value ? ChartTooltipMode.Shared : ChartTooltipMode.Single, nameof(ChartTooltipMode)); } }
    public ChartTooltipMode ChartTooltipMode => _chartTooltipMode;
    public bool ChartLegendInteractive { get => _chartLegendInteractive; set => Set(ref _chartLegendInteractive, value); }
    public bool ChartSmooth { get => _chartSmooth; set => Set(ref _chartSmooth, value); }
    public bool ChartAnimateUpdates { get => _chartAnimateUpdates; set => Set(ref _chartAnimateUpdates, value); }
    public TimeSpan ChartAnimationDuration { get => _chartAnimationDuration; set => Set(ref _chartAnimationDuration, value); }
    public bool ChartGaugeNeedle { get => _chartGaugeNeedle; set => Set(ref _chartGaugeNeedle, value); }
    public bool ChartGaugeTicks { get => _chartGaugeTicks; set => Set(ref _chartGaugeTicks, value); }
    public ChartYAxis[] ChartAxisOptions { get; } = Enum.GetValues<ChartYAxis>();
    public ObservableCollection<ChartSeries> ChartSeries { get; } =
    [
        new ChartSeries { Name = "Consumer series", ItemsSource = new ObservableCollection<ChartPoint>(Enumerable.Range(0, 1000).Select(index => new ChartPoint(index, 20 + 10 * Math.Sin(index / 20d), $"Consumer point {index}"))) },
        new ChartSeries { Name = "Native stack", ItemsSource = new ObservableCollection<ChartPoint>(Enumerable.Range(0, 1000).Select(index => new ChartPoint(index, 5))) }
    ];
    public ObservableCollection<AuditRow> Rows { get; } = new(Enumerable.Range(0, 1000).Select(i => new AuditRow($"Record {i}")));
    public ObservableCollection<AuditRow> Roots { get; } = [];
    public string[] Suggestions { get; } = ["Compiled", "Consumer", "Control"];
    public int Applied { get; private set; }
    public ICommand ApplyCommand { get; }
    public AuditModel() { Roots.Add(new AuditRow("Root") { Children = { new AuditRow("Child") } }); ApplyCommand = new Command(() => Applied++); }
    private sealed class Command(Action apply) : ICommand
    {
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => apply();
    }
}
