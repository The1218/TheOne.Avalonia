using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;

namespace TheOne.DesignPreview;

public partial class HomePage : UserControl
{
    public event EventHandler<int>? ControlRequested;
    public HomePage()
    {
        InitializeComponent();
        Capabilities.ItemsSource = new[] { "表单校验", "搜索多选", "数据表格", "主题与密度" };
        Capabilities.MaxVisibleTags = 1;
        Capabilities.SelectedItems = new ObservableCollection<object> { "表单校验", "主题与密度" };
        foreach (var category in new[] { "Subtle", "Vivid" })
        foreach (var name in new[] { "Brand", "Cool", "Natural", "Warm" })
        {
            var swatch = new Border { Height = 44, CornerRadius = new CornerRadius(6) };
            swatch.Bind(Border.BackgroundProperty, new DynamicResourceExtension($"One.Gradient.{category}.{name}"));
            GradientSamples.Children.Add(new StackPanel
            {
                Width = 140, Spacing = 6, Margin = new Thickness(0, 0, 12, 12),
                Children = { swatch, new TextBlock { Text = $"{category}.{name}", FontSize = 12 } },
            });
        }
        AddFeature(322, "Form / FormItem", "复用 VerificationStep 校验，将标签、帮助和字段错误放在一起。", "<one:Form Validator=\"{Binding Validator}\">\n  <one:FormItem MemberName=\"Email\" Label=\"邮箱\">\n    <TextBox Text=\"{Binding Email}\" />\n  </one:FormItem>\n</one:Form>");
        AddFeature(323, "MultiSelectBox", "搜索、多选标签、禁选项和虚拟化，在同一个输入控件中完成。", "<one:MultiSelectBox\n  ItemsSource=\"{Binding Options}\"\n  SelectedItems=\"{Binding Selected}\" />");
        AddFeature(10, "DataGrid", "保留原生编辑与选择，叠加列筛选、排序和数值统计。", "<!-- Rows 行类型需先注册元数据 -->\n<DataGrid ItemsSource=\"{Binding Rows}\">\n  <DataGrid.Columns>\n    <DataGridTextColumn Header=\"名称\"\n      x:DataType=\"vm:Row\"\n      Binding=\"{Binding Name}\" />\n  </DataGrid.Columns>\n</DataGrid>");
        AddFeature(25, "NavMenu", "树形导航、折叠图标栏与级联子菜单，保持当前页面的选择。", "<one:NavMenu\n  ItemsSource=\"{Binding Menu}\"\n  SelectedItem=\"{Binding CurrentPage}\" />");
        AddFeature(23, "图表与可视化", "折线、柱状、堆叠与仪表直接绘制；复杂关系可使用 RelationGraph。", "<one:LineChart\n  SeriesSource=\"{Binding Series}\" />");
        AddFeature(319, "ReportPreview / 打印", "加载报表 JSON；打印按钮打开 PrintingTools 的设置与预览，再确认提交。", "<one:ReportPreview\n  DocumentJson=\"{Binding ReportJson}\" />\n// PreviewHost: TheOne.PrintingTools.Avalonia");
        SizeChanged += (_, _) => ArrangeSections();
        AttachedToVisualTree += (_, _) => ArrangeSections();
    }
    private void AddFeature(int id, string title, string description, string code)
    {
        var body = new StackPanel { Spacing = 12 };
        body.Children.Add(new TextBlock { Text = title, FontSize = 18, FontWeight = global::Avalonia.Media.FontWeight.SemiBold, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap });
        var detail = new TextBlock { Text = description, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap };
        detail.Bind(TextBlock.ForegroundProperty, new DynamicResourceExtension("One.Muted")); body.Children.Add(detail);
        body.Children.Add(new PreviewCodeBlock { Text = code });
        var button = new Button { Content = "查看完整示例 →", Tag = id, HorizontalAlignment = HorizontalAlignment.Left };
        button.Click += (_, _) => ControlRequested?.Invoke(this, id); body.Children.Add(button);
        var frame = new Border { Child = body, Padding = new Thickness(20), CornerRadius = new CornerRadius(8), BorderThickness = new Thickness(1) };
        frame.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Surface"));
        frame.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line")); Features.Children.Add(frame);
    }
    private void ArrangeSections()
    {
        var columns = Bounds.Width >= 950 ? 3 : Bounds.Width >= 620 ? 2 : 1;
        Features.ColumnDefinitions = new ColumnDefinitions(string.Join(',', Enumerable.Repeat("*", columns)));
        Features.RowDefinitions = new RowDefinitions(string.Join(',', Enumerable.Repeat("Auto", (Features.Children.Count + columns - 1) / columns)));
        for (var i = 0; i < Features.Children.Count; i++) { Grid.SetColumn(Features.Children[i], i % columns); Grid.SetRow(Features.Children[i], i / columns); }
        var split = Bounds.Width >= 760;
        Examples.ColumnDefinitions = new ColumnDefinitions(split ? "*,*" : "*");
        Examples.RowDefinitions = new RowDefinitions(split ? "Auto" : "Auto,Auto");
        Grid.SetColumn(QuickStart, split ? 1 : 0); Grid.SetRow(QuickStart, split ? 0 : 1);
        var densityColumns = Bounds.Width >= 950 ? 3 : Bounds.Width >= 620 ? 2 : 1;
        DensitySamples.ColumnDefinitions = new ColumnDefinitions(string.Join(',', Enumerable.Repeat("*", densityColumns)));
        DensitySamples.RowDefinitions = new RowDefinitions(string.Join(',', Enumerable.Repeat("Auto", (DensitySamples.Children.Count + densityColumns - 1) / densityColumns)));
        for (var i = 0; i < DensitySamples.Children.Count; i++) { Grid.SetColumn(DensitySamples.Children[i], i % densityColumns); Grid.SetRow(DensitySamples.Children[i], i / densityColumns); }
    }
    private void NavigateClicked(object? sender, RoutedEventArgs args)
    { if (sender is Button { Tag: string id } && int.TryParse(id, out var value)) ControlRequested?.Invoke(this, value); }
    private void SaveClicked(object? sender, RoutedEventArgs args) => SampleStatus.Text = $"已保存：{WorkspaceInput.Text} · {Capabilities.SelectedCount} 项能力";
}
