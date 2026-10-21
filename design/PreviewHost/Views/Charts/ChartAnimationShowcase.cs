using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal static class ChartAnimationShowcase
{
    public static Control Create(ChartBase chart)
    {
        chart.IsAnimationEnabled = true;
        chart.AnimationDuration = TimeSpan.FromMilliseconds(500);
        chart.MinHeight = 260;
        chart.HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Stretch;
        var styles = new ComboBox
        {
            ItemsSource = Enum.GetValues<ChartAnimationStyle>(),
            SelectedItem = ChartAnimationStyle.Auto, MinWidth = 110, Margin = new Thickness(0, 0, 12, 8)
        };
        styles.SelectionChanged += (_, _) =>
        {
            if (styles.SelectedItem is ChartAnimationStyle style) chart.AnimationStyle = style;
        };
        var update = new Button { Content = "更新数据，查看动效", Margin = new Thickness(0, 0, 12, 8) };
        var revision = 0;
        update.Click += (_, _) =>
        {
            revision++;
            if (chart is GaugeChart) chart.Value = revision % 2 == 0 ? 28 : 76;
            else if (chart is HierarchyChartBase hierarchy)
            {
                var pending = new Stack<ChartHierarchyNode>(hierarchy.ItemsSource ?? []);
                var index = 0;
                while (pending.TryPop(out var node))
                    if (node.Children is { Count: > 0 } children)
                        foreach (var child in children) pending.Push(child);
                    else node.Y = 15 + (index++ * 17 + revision * 11) % 60;
            }
            else if (chart.SeriesSource is { } series)
                foreach (var entry in series)
                    if (entry.ItemsSource is { } points)
                        for (var i = 0; i < points.Count; i++)
                            if (points[i] is HeatmapPoint cell) cell.Value = Math.Sin(i * .8 + revision) * 100;
                            else if (points[i] is BoxPlotPoint box)
                            {
                                var shift = revision % 2 == 0 ? -3d : 3d;
                                box.Minimum += shift; box.LowerQuartile += shift; box.Median += shift;
                                box.UpperQuartile += shift; box.Maximum += shift;
                                if (box.Outliers is { } outliers)
                                    for (var n = 0; n < outliers.Count; n++) outliers[n] += shift;
                            }
                            else if (points[i] is CandlestickPoint candle)
                            {
                                var shift = revision % 2 == 0 ? -2d : 2d;
                                candle.Open += shift; candle.Close += shift; candle.Low += shift; candle.High += shift;
                            }
                            else points[i].Y = 20 + (i * 17 + revision * 29) % 75;
        };
        var enabled = new CheckBox { Content = "启用动画", IsChecked = true, Margin = new Thickness(0, 0, 12, 8) };
        enabled.IsCheckedChanged += (_, _) => chart.IsAnimationEnabled = enabled.IsChecked == true;
        var actions = new WrapPanel { Children = { styles, update, enabled } };
        if (chart is HierarchyChartBase tree)
        {
            var drill = new Button { Content = "下钻选中分支", Margin = new Thickness(0, 0, 12, 8) };
            var up = new Button { Content = "返回上级", Margin = new Thickness(0, 0, 12, 8) };
            var reset = new Button { Content = "完整层级", Margin = new Thickness(0, 0, 12, 8) };
            drill.Click += (_, _) => { if (tree.SelectedNode is { } selected) tree.DrillDown(selected); };
            up.Click += (_, _) => tree.DrillUp();
            reset.Click += (_, _) => tree.ResetRoot();
            void SyncNavigation()
            {
                drill.IsEnabled = tree.SelectedNode?.Children is { Count: > 0 };
                up.IsEnabled = tree.CanDrillUp;
            }
            tree.PropertyChanged += (_, args) =>
            {
                if (args.Property == HierarchyChartBase.SelectedNodeProperty || args.Property == HierarchyChartBase.CanDrillUpProperty)
                    SyncNavigation();
            };
            SyncNavigation();
            actions.Children.Add(drill); actions.Children.Add(up); actions.Children.Add(reset);
        }
        return PreviewSection.Create("动画样式 · Auto / Fade / Reveal / None", new StackPanel
        {
            Spacing = 12, Children =
            {
                actions, chart,
                new Description { Items =
                {
                    Row("AnimationStyle / Classes", "Auto 保留数据插值；Fade 渐入，Reveal 从左向右展开，None 立即呈现。关闭全局 Motion 同样立即呈现。",
                        "<one:LineChart AnimationStyle=\"Reveal\" AnimationDuration=\"0:0:0.5\" />\n<!-- 或 Classes=\"animation-fade\"；animation-none > animation-reveal > animation-fade -->"),
                    Row("性能与导出", "布局与目标几何按数据版本缓存，动画复用插值存储。超过 6000 个绘制标记自动停用动画；PNG 导出始终为完整目标状态。",
                        "chart.IsAnimationEnabled = true;\nchart.ExportPng(stream);")
                } }
            }
        });
    }

    internal static DescriptionItem Row(string header, string description, string example) => new()
    {
        Header = header, Content = new StackPanel
        {
            Spacing = 6, Children =
            {
                new TextBlock { Text = description, TextWrapping = TextWrapping.Wrap },
                new PreviewDeferredContent(() => new PreviewCodeBlock
                {
                    Text = example,
                    CodeType = example.TrimStart().StartsWith('<') ? CodeType.Axaml : CodeType.CSharp
                }, Math.Clamp((example.Count(character => character == '\n') + 1) * 24 + 62, 108, 350), keyboardEntry: true)
            }
        }
    };
}
