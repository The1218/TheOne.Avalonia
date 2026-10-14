using Avalonia;
using Avalonia.Controls;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class ChartShowcaseView : UserControl
{
    private ChartDemoView? _interactiveDemo;
    public ChartKind Kind { get; private set; }
    public IReadOnlyList<ChartBase> Samples { get; private set; } = [];

    public ChartShowcaseView()
    {
        InitializeComponent();
        Editor.PropertyChanged += (_, args) =>
        {
            if(args.Property == Expander.IsExpandedProperty && Editor.IsExpanded) EnsureEditor();
            else if(args.Property == Expander.IsExpandedProperty) _interactiveDemo?.StopRapidUpdate();
        };
    }

    public ChartShowcaseView(ChartKind kind) : this()
    {
        Kind = kind;
        DataContext = PreviewPageCatalog.Controls.First(control => control.Title == $"{kind}Chart");
        var samples = new List<ChartBase>();
        for(var style = 0; style < (kind == ChartKind.Gauge ? Enum.GetValues<GaugeStyle>().Length : 3); style++)
        {
            var chart = CreateSample(kind, style);
            samples.Add(chart);
            Variants.Children.Add(new StackPanel
            {
                Classes = { "chart-variant" }, Margin = new Thickness(0, 0, 16, 16), Spacing = 8,
                Children =
                {
                    chart
                }
            });
        }
        Samples = samples;
    }

    private static ChartBase CreateSample(ChartKind kind, int style)
    {
        var chart = ChartControlFactory.CreateControl(kind);
        var data = new ChartDemoViewModel();
        data.StopObservingSeries();
        chart.SeriesSource = style == 2 && kind is not (ChartKind.Pie or ChartKind.Doughnut or ChartKind.Gauge)
            ? data.Series.Take(1).ToArray() : data.Series;
        chart.Title = StyleTitle(kind, style);
        chart.IsAnimationEnabled = false;
        chart.ShowCrosshair = true;
        chart.TooltipMode = ChartTooltipMode.Shared;
        chart.IsZoomEnabled = true;
        chart.IsPanEnabled = true;
        if(kind == ChartKind.Gauge)
        {
            ChartControlFactory.ConfigureGauge((GaugeChart)chart, (GaugeStyle)style);
        }
        else if(kind is ChartKind.Pie or ChartKind.Doughnut)
        {
            chart.ShowLegend = style == 0;
            chart.ShowDataLabels = style == 1;
            if(kind == ChartKind.Doughnut) chart.InnerRadius = style switch { 1 => 0.35, 2 => 0.78, _ => 0.62 };
            if(style == 2) chart.SelectedIndex = 2;
        }
        else
        {
            chart.ShowDataLabels = style == 1;
            chart.ShowGrid = style == 0;
            chart.ShowLegend = style != 2;
            chart.ShowMarkers = style != 2;
            chart.IsSmooth = style == 1;
            if(kind == ChartKind.Scatter && style == 1) chart.MarkerRadius = 7;
            if(style == 2) chart.StrokeThickness = 4;
        }
        return chart;
    }

    internal static string StyleTitle(ChartKind kind, int style) => kind switch
    {
        ChartKind.Gauge => ChartControlFactory.GaugeStyleTitle((GaugeStyle)style),
        ChartKind.Doughnut => style switch { 0 => "标准环形", 1 => "宽环与占比标签", _ => "细环与选中分区" },
        ChartKind.Pie => style switch { 0 => "图例样式", 1 => "占比标签", _ => "选中扇区" },
        ChartKind.Scatter => style switch { 0 => "标准散点", 1 => "大圆点与数值", _ => "单系列 · 简洁" },
        ChartKind.Line or ChartKind.Area or ChartKind.StackedArea or ChartKind.PercentStackedArea
            => style switch { 0 => "标准趋势", 1 => "平滑曲线与数值", _ => "单系列 · 简洁" },
        _ => style switch { 0 => "标准对比", 1 => "数值标签", _ => "单系列 · 简洁" }
    };

    private ChartDemoView EnsureEditor()
    {
        if(_interactiveDemo is null) Editor.Content = _interactiveDemo = new ChartDemoView { FixedKind = Kind };
        return _interactiveDemo;
    }
    public ChartDemoView OpenEditor()
    {
        var demo = EnsureEditor();
        Editor.IsExpanded = true;
        return demo;
    }
    public void CloseEditor() => Editor.IsExpanded = false;
}
