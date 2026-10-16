using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using TheOne.DesignPreview.Reference;

namespace TheOne.DesignPreview;

/// <summary>Independent, labelled examples; the complete business sample keeps its own bindings and parent.</summary>
internal static class ControlStyleShowcase
{
    private static readonly string[] Options = ["设计 Design", "开发 Development", "测试 Quality"];
    internal static bool Supports(string name) => Factory(name) is not null;

    internal static Control? Create(string name)
    {
        var factory = Factory(name);
        if (factory is null) return null;

        var sections = new StackPanel { Name = "StyleShowcase_" + name, Spacing = 16 };
        var styles = PublicStyles(name);
        if (styles.Length > 0)
        {
            var variants = new List<Example> { new("默认", "Classes=\"\"", factory()) };
            foreach (var style in styles)
            {
                var control = factory();
                control.Classes.Add(style.Name);
                if (control is TextBox text && style.Name == "revealPasswordButton") text.PasswordChar = '●';
                variants.Add(new(style.Name, "Classes=\"" + style.Name + "\"", control,
                    style.Name is "clearButton" or "revealPasswordButton" ? "聚焦输入框后显示按钮。" : null));
            }
            sections.Children.Add(Section(name + " · Style 样式", "每个样例直接标注实际 Classes；点击或移入查看原生交互。", variants));
        }

        var states = StateExamples(name, factory);
        if (states.Count > 0)
            sections.Children.Add(Section(name + " · 公共属性与状态", "属性值写在样例上方，各实例互相独立。", states));

        var sizes = new List<Example>();
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var sample = factory();
            ControlSizing.SetSize(sample, size);
            sizes.Add(new(size.ToString(), "ControlSizing.Size=\"" + size + "\"", sample));
        }
        sections.Children.Add(Section(name + " · Size 尺寸", "使用继承式 ControlSizing.Size；下列三个样例显式指定尺寸，便于同时比较。", sizes));
        return sections;
    }

    internal static StyleContract[] PublicStyles(string name)
    {
        var targets = ContractCatalog.StyleTargets(name).ToHashSet(StringComparer.Ordinal);
        // SplitButton has a separate template, and ToggleSplitButton is based on its theme.
        // Button-only selectors cannot style either split control.
        if (name is "SplitButton" or "ToggleSplitButton")
        {
            targets.Remove("Button");
            targets.Add("SplitButton");
        }
        return ContractCatalog.Current.Styles
            .Where(style => !style.Internal && style.Target != "Control" && targets.Contains(style.Target))
            .DistinctBy(style => style.Name)
            .OrderBy(style => style.Name, StringComparer.Ordinal)
            .ToArray();
    }

    private static Func<Control>? Factory(string name) => name switch
    {
        "Button" => () => new Button { Content = "执行操作" },
        "RepeatButton" => () => new RepeatButton { Content = "长按连续操作" },
        "ToggleButton" => () => new ToggleButton { Content = "切换状态", IsChecked = true },
        "IconButton" => () => new IconButton { Content = "添加项目", Icon = Geometry.Parse("M7,0 L9,0 L9,7 L16,7 L16,9 L9,9 L9,16 L7,16 L7,9 L0,9 L0,7 L7,7 Z") },
        "SplitButton" => () => new SplitButton { Content = "保存", Flyout = Actions() },
        "ToggleSplitButton" => () => new ToggleSplitButton { Content = "固定视图", IsChecked = true, Flyout = Actions() },
        "TextBox" => () => new TextBox { Text = "输入示例", PlaceholderText = "请输入内容", Width = 220 },
        "NumericUpDown" => () => new NumericUpDown { Value = 28, Minimum = 0, Maximum = 100, Increment = 1, Width = 220 },
        "AutoCompleteBox" => () => new AutoCompleteBox { ItemsSource = Options, Text = "Design", PlaceholderText = "输入建议关键词", Width = 220 },
        "ComboBox" => () => new ComboBox { ItemsSource = Options, SelectedIndex = 0, Width = 220 },
        "ListBox" => () => new ListBox { ItemsSource = Options, SelectedIndex = 0, Width = 240, Height = 142 },
        "DataGrid" => GridSample,
        "MultiSelectBox" => () => new MultiSelectBox
        {
            ItemsSource = Options,
            SelectedItems = new ObservableCollection<object> { Options[0], Options[1] },
            Width = 280,
            MaxDropDownHeight = 200
        },
        "CheckBox" => () => new CheckBox { Content = "接收通知", IsChecked = true },
        "RadioButton" => () => new RadioButton { Content = "默认选项", GroupName = Guid.NewGuid().ToString(), IsChecked = true },
        "ToggleSwitch" => () => new ToggleSwitch { Content = "自动更新", IsChecked = true },
        "Slider" => () => new Slider { Minimum = 0, Maximum = 100, Value = 60, Width = 220 },
        "ProgressBar" => () => new ProgressBar { Minimum = 0, Maximum = 100, Value = 60, Width = 220 },
        "DatePicker" => () => new DatePicker { SelectedDate = new DateTimeOffset(2026, 10, 16, 0, 0, 0, TimeSpan.Zero), Width = 300 },
        "CalendarDatePicker" => () => new CalendarDatePicker { SelectedDate = new DateTime(2026, 10, 16), Width = 220 },
        "TimePicker" => () => new TimePicker { SelectedTime = new TimeSpan(14, 30, 0), ClockIdentifier = "24HourClock", Width = 260 },
        "Label" => () => new Label { Content = "Label · 文本标签" },
        "SelectableTextBlock" => () => new SelectableTextBlock { Text = "可选择并复制这段文字。", TextWrapping = TextWrapping.Wrap, MaxWidth = 240 },
        "InfoCard" => () => new InfoCard { Header = "项目进度", Content = new TextBlock { Text = "内容根据实际文字自然伸展。", TextWrapping = TextWrapping.Wrap }, Width = 260 },
        "TabControl" => () => Tabs(),
        "Tag" => () => new Tag { Content = "处理中", Status = TagStatus.Processing },
        "Description" => () => new Description { Width = 300, Items = { new DescriptionItem { Header = "属性", Content = "类型、默认值与用途自然换行展示。" } } },
        _ => null
    };

    private static List<Example> StateExamples(string name, Func<Control> factory)
    {
        var result = new List<Example>();
        if (name is "Button" or "RepeatButton" or "IconButton" or "ToggleButton" or "SplitButton" or "ToggleSplitButton"
            or "TextBox" or "NumericUpDown" or "AutoCompleteBox" or "ComboBox" or "ListBox" or "MultiSelectBox"
            or "CheckBox" or "RadioButton" or "ToggleSwitch" or "Slider" or "DatePicker" or "CalendarDatePicker" or "TimePicker")
        {
            result.Add(new("正常", "IsEnabled=\"True\"", factory()));
            var disabled = factory(); disabled.IsEnabled = false;
            result.Add(new("禁用", "IsEnabled=\"False\"", disabled));
        }

        switch (name)
        {
            case "Button":
            case "RepeatButton":
            case "IconButton":
                var loading = factory();
                ButtonLoading.SetIsLoading((Button)loading, true);
                result.Add(new("加载中", "ButtonLoading.IsLoading=\"True\"", loading));
                if (name == "IconButton")
                    foreach (var placement in Enum.GetValues<IconPlacement>())
                    {
                        var icon = (IconButton)factory(); icon.IconPlacement = placement;
                        result.Add(new(placement.ToString(), "IconPlacement=\"" + placement + "\"", icon));
                    }
                break;
            case "ToggleButton":
                result.Add(new("未选中", "IsChecked=\"False\"", new ToggleButton { Content = "切换状态", IsChecked = false }));
                result.Add(new("三态", "IsThreeState=\"True\" · IsChecked=\"{x:Null}\"", new ToggleButton { Content = "未确定", IsThreeState = true, IsChecked = null }));
                break;
            case "ToggleSplitButton":
                result.Add(new("未选中", "IsChecked=\"False\"", new ToggleSplitButton { Content = "固定视图", IsChecked = false, Flyout = Actions() }));
                break;
            case "TextBox":
                result.Add(new("只读", "IsReadOnly=\"True\"", new TextBox { Text = "此内容可选择复制", IsReadOnly = true, Width = 220 }));
                result.Add(new("占位提示", "PlaceholderText=\"请输入内容\"", new TextBox { PlaceholderText = "请输入内容", Width = 220 }));
                result.Add(new("密码", "PasswordChar=\"●\"", new TextBox { Text = "TheOne", PasswordChar = '●', Width = 220 }));
                result.Add(new("多行", "AcceptsReturn=\"True\"", new TextBox { Text = "第一行\n第二行", AcceptsReturn = true, MinHeight = 80, Width = 220 }));
                break;
            case "NumericUpDown":
                result.Add(new("隐藏步进按钮", "ShowButtonSpinner=\"False\"", new NumericUpDown { Value = 28, ShowButtonSpinner = false, Width = 220 }));
                result.Add(new("精度与步长", "Increment=\"0.25\" · FormatString=\"0.00\"", new NumericUpDown { Value = 28.25m, Increment = .25m, FormatString = "0.00", Width = 220 }));
                break;
            case "AutoCompleteBox":
                result.Add(new("建议过滤", "MinimumPrefixLength=\"0\"", new AutoCompleteBox { ItemsSource = Options, MinimumPrefixLength = 0, PlaceholderText = "聚焦后输入以筛选", Width = 220 }));
                break;
            case "ComboBox":
                result.Add(new("未选择", "SelectedIndex=\"-1\" · PlaceholderText=\"请选择\"", new ComboBox { ItemsSource = Options, SelectedIndex = -1, PlaceholderText = "请选择", Width = 220 }));
                break;
            case "ListBox":
                var multiple = (ListBox)factory();
                multiple.SelectionMode = SelectionMode.Multiple | SelectionMode.Toggle;
                multiple.SelectedItems?.Add(Options[1]);
                result.Add(new("多选", "SelectionMode=\"Multiple, Toggle\"", multiple));
                break;
            case "MultiSelectBox":
                var readOnly = (MultiSelectBox)factory(); readOnly.IsReadOnly = true;
                result.Add(new("只读", "IsReadOnly=\"True\"", readOnly));
                var bounded = (MultiSelectBox)factory(); bounded.MaxVisibleTags = 1;
                result.Add(new("限制可见标签", "MaxVisibleTags=\"1\"", bounded));
                result.Add(new("空选择", "PlaceholderText=\"请选择工作阶段\"", new MultiSelectBox { ItemsSource = Options, PlaceholderText = "请选择工作阶段", Width = 280 }));
                break;
            case "CheckBox":
                result.Add(new("未选中", "IsChecked=\"False\"", new CheckBox { Content = "接收通知", IsChecked = false }));
                result.Add(new("三态", "IsThreeState=\"True\" · IsChecked=\"{x:Null}\"", new CheckBox { Content = "部分选中", IsThreeState = true, IsChecked = null }));
                break;
            case "RadioButton":
                result.Add(new("未选中", "IsChecked=\"False\"", new RadioButton { Content = "备选项", GroupName = Guid.NewGuid().ToString(), IsChecked = false }));
                break;
            case "ToggleSwitch":
                result.Add(new("关闭", "IsChecked=\"False\"", new ToggleSwitch { Content = "自动更新", IsChecked = false }));
                break;
            case "Slider":
                result.Add(new("离散刻度", "TickFrequency=\"25\" · IsSnapToTickEnabled=\"True\"", new Slider { Minimum = 0, Maximum = 100, Value = 50, TickFrequency = 25, IsSnapToTickEnabled = true, Width = 220 }));
                break;
            case "ProgressBar":
                result.Add(new("确定进度", "Value=\"60\" · ShowProgressText=\"True\"", new ProgressBar { Value = 60, ShowProgressText = true, Width = 220 }));
                result.Add(new("不确定进度", "IsIndeterminate=\"True\"", new ProgressBar { IsIndeterminate = true, Width = 220 }));
                break;
            case "InfoCard":
                result.Add(new("完整区域", "Header · HeaderActions · Content · Footer", new InfoCard
                {
                    Header = "项目进度", HeaderActions = new Button { Content = "详情" }, Width = 280,
                    Content = new TextBlock { Text = "标题、正文、操作区和页脚独立配置。", TextWrapping = TextWrapping.Wrap },
                    Footer = new TextBlock { Text = "更新于今天", TextWrapping = TextWrapping.Wrap }
                }));
                break;
            case "Tag":
                foreach (var variant in Enum.GetValues<TagVariant>())
                foreach (var status in Enum.GetValues<TagStatus>())
                    result.Add(new(status.ToString(), $"Variant=\"{variant}\" · Status=\"{status}\"", new Tag { Content = status.ToString(), Variant = variant, Status = status }));
                break;
        }
        return result;
    }

    private static TabControl Tabs() => new()
    {
        Width = 260,
        Items =
        {
            new TabItem { Header = "概览", Content = new TextBlock { Text = "当前选项卡的内容自然排版。", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(8) } },
            new TabItem { Header = "记录", Content = new TextBlock { Text = "第二个选项卡", Margin = new Thickness(8) } }
        }
    };

    private static DataGrid GridSample()
    {
        DataGridRowMetadata.Register<GridStyleRow>();
        DataGridRowMetadata.RegisterCollection<GridStyleRow[], GridStyleRow>();
        return new DataGrid
        {
            AutoGenerateColumns = false, IsReadOnly = true, Width = 300, Height = 150,
            ItemsSource = new[] { new GridStyleRow("设计", "已完成"), new GridStyleRow("开发", "进行中") },
            Columns =
            {
                new DataGridTextColumn { Header = "任务", Binding = CompiledBinding.Create<GridStyleRow, string>(row => row.Name) },
                new DataGridTextColumn { Header = "状态", Binding = CompiledBinding.Create<GridStyleRow, string>(row => row.State) }
            }
        };
    }
    private sealed record GridStyleRow(string Name, string State);

    private static Flyout Actions() => new()
    {
        Content = new StackPanel { Spacing = 8, Children = { new Button { Content = "另存为" }, new Button { Content = "导出副本" } } }
    };

    private static Border Section(string title, string explanation, IEnumerable<Example> examples)
    {
        var cells = new WrapPanel { Orientation = Orientation.Horizontal };
        foreach (var example in examples)
        {
            example.Sample.Name = "Variant_" + example.Sample.GetType().Name + "_" + cells.Children.Count;
            var preferredWidth = double.IsFinite(example.Sample.Width) ? example.Sample.Width : 190;
            var fixedWidth = double.IsFinite(example.Sample.Width);
            example.Sample.Width = double.NaN;
            example.Sample.MinWidth = 0;
            example.Sample.MaxWidth = Math.Min(example.Sample.MaxWidth, preferredWidth);
            example.Sample.HorizontalAlignment = fixedWidth ? HorizontalAlignment.Stretch : HorizontalAlignment.Left;
            var cell = new StackPanel { Spacing = 8 };
            cell.Children.Add(new TextBlock { Text = example.Title, FontWeight = FontWeight.Medium });
            var label = new TextBlock { Text = example.Configuration, FontSize = 12, TextWrapping = TextWrapping.Wrap };
            label.Bind(TextBlock.ForegroundProperty, new DynamicResourceExtension("One.Accent"));
            cell.Children.Add(label);
            cell.Children.Add(example.Sample);
            if (example.Note is { } note) cell.Children.Add(Muted(note));
            cells.Children.Add(new SampleCell(preferredWidth) { Child = cell, Margin = new Thickness(0, 0, 20, 16) });
        }
        var panel = new StackPanel { Spacing = 12 };
        panel.Children.Add(new TextBlock { Text = title, FontSize = 17, FontWeight = FontWeight.SemiBold, TextWrapping = TextWrapping.Wrap });
        panel.Children.Add(Muted(explanation));
        panel.Children.Add(cells);
        var border = new Border { BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(10), Padding = new Thickness(20), Child = panel };
        border.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line"));
        border.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Surface"));
        return border;
    }

    private static TextBlock Muted(string text)
    {
        var block = new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap };
        block.Bind(TextBlock.ForegroundProperty, new DynamicResourceExtension("One.Muted"));
        return block;
    }

    private sealed record Example(string Title, string Configuration, Control Sample, string? Note = null);

    // WrapPanel can give a cell less than its preferred width in a narrow page.
    // Measure the text and sample against that width, without fixing the cell's height.
    private sealed class SampleCell(double preferredWidth) : Decorator
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var width = double.IsFinite(availableSize.Width) ? Math.Min(preferredWidth, availableSize.Width) : preferredWidth;
            width = Math.Max(0, width);
            Child?.Measure(new Size(width, availableSize.Height));
            return new Size(width, Child?.DesiredSize.Height ?? 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Child?.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}
