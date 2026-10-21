using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal sealed class SpecializedChartShowcaseView : UserControl
{
    public SpecializedChartShowcaseView(ChartKind kind)
    {
        var chart = CreateSample(kind);
        Description CreateProperties()
        {
        var properties = new Description();
        if (kind == ChartKind.Heatmap)
        {
            properties.Items.Add(Row("HeatmapPoint", "X、Y 是网格坐标，Value 独立控制颜色；支持负值和稀疏网格，非有限 Value 留空。",
                "new HeatmapPoint(2, 3, -25, \"星期三 · 14时\")"));
            properties.Items.Add(Row("CellWidth / CellHeight / CellGap", "前两项使用数据坐标单位，CellGap 使用 DIP。数据、色阶与命中按同一目标帧准备。",
                "<one:HeatmapChart CellWidth=\"1\" CellHeight=\"1\" CellGap=\"2\" SeriesSource=\"{CompiledBinding HeatmapSeries}\" />"));
            properties.Items.Add(Row("ValueMinimum / ValueMaximum / LowColor / MidColor / HighColor", "默认范围从有效 Value 计算；自定义上下限可固定跨图的颜色含义，未指定颜色时跟随主题。",
                "<one:HeatmapChart ValueMinimum=\"-100\" ValueMaximum=\"100\" LowColor=\"#4085D3\" MidColor=\"#F0F4F4\" HighColor=\"#D96C55\" />"));
        }
        else if (kind == ChartKind.Radar)
        {
            properties.Items.Add(Row("Axes / RadarAxis", "每维有独立 Name、Minimum、Maximum；点按集合顺序对应维度，Y 为该维原始值，X 需为有限数。",
                "radar.Axes = new ObservableCollection<RadarAxis> {\n    new() { Name = \"质量\", Maximum = 100 },\n    new() { Name = \"速度\", Maximum = 200 },\n    new() { Name = \"覆盖\", Maximum = 100 }\n};"));
            properties.Items.Add(Row("GridLevels / FillOpacity", "网格层数与多边形填充透明度独立配置。缺少维度时不将剩余点连成误导的闭合面积。",
                "<one:RadarChart GridLevels=\"5\" FillOpacity=\"0.16\" Axes=\"{CompiledBinding Dimensions}\" SeriesSource=\"{CompiledBinding RadarSeries}\" />"));
        }
        else if (kind == ChartKind.Funnel)
        {
            properties.Items.Add(Row("SortMode", "默认 None 保留业务阶段顺序；只有显式选择 Ascending 或 Descending 才重排，选中索引仍对应原始数据。",
                "<one:FunnelChart SortMode=\"None\" SeriesSource=\"{CompiledBinding FunnelSeries}\" ShowDataLabels=\"True\" />"));
            properties.Items.Add(Row("Gap / MinimumWidthRatio", "阶段间距使用 DIP。最窄比例用于控制小值的可见宽度；数值和提示仍保留原始值。",
                "<one:FunnelChart Gap=\"4\" MinimumWidthRatio=\"0\" />"));
        }
        else if (kind == ChartKind.BoxPlot)
        {
            properties.Items.Add(Row("BoxPlotPoint / Outliers", "使用最小值、下四分位、中位数、上四分位和最大值描述分布，离群值单独显示；无效数值顺序留空。",
                "new BoxPlotPoint(0, 12, 18, 23, 29, 38, \"周一\") { Outliers = new ObservableCollection<double> { 6, 45 } }"));
            properties.Items.Add(Row("PointWidthRatio / ShowOutliers / OutlierRadius", "箱体宽度按类别槽位比例调整；可独立显示离群值并配置半径，保留真实数值和原始索引。",
                "<one:BoxPlotChart PointWidthRatio=\"0.6\" ShowOutliers=\"True\" OutlierRadius=\"3\" SeriesSource=\"{CompiledBinding Distribution}\" />"));
        }
        else if (kind == ChartKind.Candlestick)
        {
            properties.Items.Add(Row("CandlestickPoint", "Open、Close、Low、High 分别为开、收、最低及最高值，X 对应时间或类别；保留十字星与上涨／下跌区别。",
                "new CandlestickPoint(0, 102, 108, 99, 111, \"10-12\")"));
            properties.Items.Add(Row("PointWidthRatio / UpBrush / DownBrush / IsHollowUp", "配置烛体宽度、涨跌色及上涨空心外观；未覆盖画刷时使用主题资源。",
                "<one:CandlestickChart IsHollowUp=\"True\" PointWidthRatio=\"0.6\" SeriesSource=\"{CompiledBinding Prices}\" />"));
        }
        else
        {
            properties.Items.Add(Row("ItemsSource / ChartHierarchyNode", "Label 为显示文字，Key 为业务标识，Y 为叶子权重；存在 Children 时父级权重从子节点聚合。",
                "new ChartHierarchyNode(\"研发\", 0, \"engineering\") { Children = new ObservableCollection<ChartHierarchyNode> {\n    new(\"客户端\", 45, \"client\"), new(\"服务端\", 55, \"service\")\n} }"));
            properties.Items.Add(Row("Root", "单击选择、双击分支下钻。Root 支持双向绑定。",
                $"<one:{kind}Chart ItemsSource=\"{{CompiledBinding Departments}}\" Root=\"{{CompiledBinding CurrentBranch, Mode=TwoWay}}\" />"));
            properties.Items.Add(Row("DrillDown / DrillUp / ResetRoot", "从按钮、菜单或面包屑导航到分支、返回上级或恢复完整层级。",
                "chart.DrillDown(branch);\nchart.DrillUp();\nchart.ResetRoot();"));
            properties.Items.Add(kind == ChartKind.Treemap
                ? Row("Gap / HeaderHeight", "分支间距和分组标题高度使用 DIP，图块按有效叶子权重分配面积。",
                    "<one:TreemapChart Gap=\"3\" HeaderHeight=\"20\" ItemsSource=\"{CompiledBinding Departments}\" />")
                : Row("InnerRadius / RingGap / StartAngle", "配置内孔比例、层级环间距和起始角度，扇区角度仍由原始权重决定。",
                    "<one:SunburstChart InnerRadius=\"0.2\" RingGap=\"2\" StartAngle=\"-90\" ItemsSource=\"{CompiledBinding Departments}\" />"));
        }
        return properties;
        }
        var variants = new WrapPanel();
        for (var i = 0; i < 2; i++)
        {
            var variantIndex = i;
            Control CreateVariant()
            {
            var variant = CreateSample(kind);
            variant.IsAnimationEnabled = false;
            variant.Title = variantIndex == 0 ? "标签与选择" : "配置变体";
            variant.ShowDataLabels = variantIndex == 0;
            if (variantIndex == 0) variant.SelectedIndex = 1;
            if (variant is HeatmapChart heatmap && variantIndex == 1) heatmap.CellGap = 0;
            if (variant is RadarChart radar && variantIndex == 1) { radar.GridLevels = 3; radar.FillOpacity = 0; }
            if (variant is FunnelChart funnel && variantIndex == 1) { funnel.Gap = 8; funnel.SortMode = FunnelSortMode.Descending; }
            if (variant is BoxPlotChart box && variantIndex == 1) { box.ShowOutliers = false; box.PointWidthRatio = .4; }
            if (variant is CandlestickChart candle && variantIndex == 1) candle.IsHollowUp = true;
            if (variant is TreemapChart treemap && variantIndex == 1) { treemap.Gap = 6; treemap.HeaderHeight = 28; }
            if (variant is SunburstChart sunburst && variantIndex == 1) { sunburst.InnerRadius = .4; sunburst.RingGap = 5; }
            return variant;
            }
            variants.Children.Add(new PreviewDeferredContent(CreateVariant, keyboardEntry: true)
                { MaxWidth = 330, Margin = new Thickness(0, 0, 16, 16) });
        }
        Content = new StackPanel
        {
            Spacing = 18, Children =
            {
                new TextBlock { Text = $"{kind}Chart", FontSize = 22, FontWeight = FontWeight.SemiBold },
                new TextBlock { Text = "原生直接绘制 · 可观察数据源 · 点击或键盘选择 · 主题与三档尺寸", TextWrapping = TextWrapping.Wrap },
                ChartAnimationShowcase.Create(chart),
                PreviewSection.Create("图形配置与选中示例", variants),
                PreviewSection.Create("额外属性、模型与业务操作", new PreviewDeferredContent(CreateProperties, 360, keyboardEntry: true))
            }
        };
    }

    internal static ChartBase CreateSample(ChartKind kind)
    {
        var chart = ChartControlFactory.CreateControl(kind);
        chart.MinHeight = 280;
        chart.ShowLegend = kind == ChartKind.Radar;
        chart.ShowDataLabels = true;
        chart.TooltipMode = ChartTooltipMode.Single;
        if (chart is HeatmapChart heatmap)
        {
            var cells = new ObservableCollection<ChartPoint>();
            for (var row = 0; row < 6; row++)
            for (var col = 0; col < 8; col++)
                if ((row + col) % 9 != 0)
                    cells.Add(new HeatmapPoint(col, row, Math.Sin(col * .7 + row) * 100, $"{col + 9}:00 · 组 {row + 1}"));
            heatmap.SeriesSource = new ObservableCollection<ChartSeries> { new() { Name = "负载偏差", ItemsSource = cells } };
            heatmap.ValueMinimum = -100;
            heatmap.ValueMaximum = 100;
        }
        else if (chart is RadarChart radar)
        {
            radar.Axes = new ObservableCollection<RadarAxis>
            {
                new() { Name = "质量", Maximum = 100 }, new() { Name = "吞吐", Maximum = 200 },
                new() { Name = "覆盖", Maximum = 100 }, new() { Name = "稳定", Maximum = 100 },
                new() { Name = "成本", Maximum = 100 }
            };
            radar.SeriesSource = new ObservableCollection<ChartSeries>
            {
                new() { Name = "当前", ItemsSource = new ObservableCollection<ChartPoint> { new(0, 85), new(1, 140), new(2, 72), new(3, 92), new(4, 66) } },
                new() { Name = "目标", ItemsSource = new ObservableCollection<ChartPoint> { new(0, 95), new(1, 180), new(2, 90), new(3, 98), new(4, 82) } }
            };
        }
        else if (chart is FunnelChart funnel)
            funnel.SeriesSource = new ObservableCollection<ChartSeries>
            {
                new() { Name = "转化", ItemsSource = new ObservableCollection<ChartPoint>
                { new(0, 1200, "访问"), new(1, 800, "注册"), new(2, 420, "试用"), new(3, 180, "成交") } }
            };
        else if (chart is BoxPlotChart box)
            box.SeriesSource = new[]
            {
                new ChartSeries { Name = "处理耗时", ItemsSource = new ChartPoint[]
                {
                    new BoxPlotPoint(0, 12, 18, 23, 29, 38, "周一") { Outliers = new ObservableCollection<double> { 6, 45 } },
                    new BoxPlotPoint(1, 15, 21, 26, 33, 41, "周二"),
                    new BoxPlotPoint(2, 10, 17, 22, 31, 39, "周三") { Outliers = new ObservableCollection<double> { 48 } },
                    new BoxPlotPoint(3, 16, 22, 28, 35, 44, "周四"), new BoxPlotPoint(4, 13, 20, 24, 32, 40, "周五")
                } }
            };
        else if (chart is CandlestickChart candle)
            candle.SeriesSource = new[]
            {
                new ChartSeries { Name = "价格", ItemsSource = new ChartPoint[]
                {
                    new CandlestickPoint(0, 102, 108, 99, 111, "10-12"), new CandlestickPoint(1, 108, 104, 101, 110, "10-13"),
                    new CandlestickPoint(2, 104, 112, 103, 115, "10-14"), new CandlestickPoint(3, 112, 112, 109, 116, "10-15"),
                    new CandlestickPoint(4, 112, 107, 105, 114, "10-16"), new CandlestickPoint(5, 107, 116, 106, 119, "10-19")
                } }
            };
        else if (chart is HierarchyChartBase hierarchy)
        {
            hierarchy.ItemsSource = new ObservableCollection<ChartHierarchyNode>
            {
                new("研发", 0, "engineering") { Children = new ObservableCollection<ChartHierarchyNode>
                {
                    new("客户端", 0, "client") { Children = new ObservableCollection<ChartHierarchyNode>
                        { new("桌面端", 32, "desktop"), new("移动端", 21, "mobile") } },
                    new("服务端", 47, "service")
                } },
                new("运营", 0, "operations") { Children = new ObservableCollection<ChartHierarchyNode>
                    { new("内容", 24, "content"), new("客户", 36, "customer") } },
                new("基础设施", 0, "infrastructure") { Children = new ObservableCollection<ChartHierarchyNode>
                    { new("计算", 28, "compute"), new("存储", 18, "storage") } }
            };
            if (hierarchy is SunburstChart sunburst) sunburst.InnerRadius = .2;
        }
        return chart;
    }

    private static DescriptionItem Row(string name, string description, string example) =>
        ChartAnimationShowcase.Row(name, description, example);
}
