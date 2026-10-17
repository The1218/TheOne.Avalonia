using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using TheOne.Avalonia.Controls;
using TheOne.DesignPreview.Reference;

namespace TheOne.DesignPreview;

/// <summary>Independent, labelled examples; the complete business sample keeps its own bindings and parent.</summary>
internal static class ControlStyleShowcase
{
    private static readonly string[] Options = ["设计 Design", "开发 Development", "测试 Quality"];

    internal static bool Supports(string name)
    {
        return Factory(name) is not null && (PublicStyles(name).Length > 0 || HasExtraExamples(name));
    }

    internal static Control? Create(string name)
    {
        var factory = Factory(name);
        if (factory is null || !Supports(name)) return null;

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
                variants.Add(new Example(style.Name, "Classes=\"" + style.Name + "\"", control,
                    style.Name is "clearButton" or "revealPasswordButton" ? "聚焦输入框后显示按钮。" : null));
            }

            sections.Children.Add(Section(name + " · Style 样式", "每个样例直接标注实际 Classes；点击或移入查看原生交互。", variants));
        }

        var extras = ExtraExamples(name, factory);
        if (extras.Count > 0)
            sections.Children.Add(Section(name + " · TheOne 额外属性", "仅展示本库扩展的属性；配置值直接标在独立样例上方。", extras));
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

    private static Func<Control>? Factory(string name)
    {
        return name switch
        {
            "Button" => () => new Button { Content = "执行操作" },
            "RepeatButton" => () => new RepeatButton { Content = "长按连续操作" },
            "ToggleButton" => () => new ToggleButton { Content = "切换状态", IsChecked = true },
            "IconButton" => () => new IconButton
            {
                Content = "添加项目",
                Icon = Geometry.Parse("M7,0 L9,0 L9,7 L16,7 L16,9 L9,9 L9,16 L7,16 L7,9 L0,9 L0,7 L7,7 Z")
            },
            "SplitButton" => () => new SplitButton { Content = "保存", Flyout = Actions() },
            "ToggleSplitButton" => () => new ToggleSplitButton
                { Content = "固定视图", IsChecked = true, Flyout = Actions() },
            "TextBox" => () => new TextBox { Text = "输入示例", PlaceholderText = "请输入内容", Width = 220 },
            "NumericUpDown" => () => new NumericUpDown
                { Value = 28, Minimum = 0, Maximum = 100, Increment = 1, Width = 220 },
            "AutoCompleteBox" => () => new AutoCompleteBox
                { ItemsSource = Options, Text = "Design", PlaceholderText = "输入建议关键词", Width = 220 },
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
            "ProgressBar" => () => new ProgressBar { Minimum = 0, Maximum = 100, Value = 60, Width = 220 },
            "DatePicker" => () => new DatePicker
                { SelectedDate = new DateTimeOffset(2026, 10, 16, 0, 0, 0, TimeSpan.Zero), Width = 300 },
            "CalendarDatePicker" => () => new CalendarDatePicker
                { SelectedDate = new DateTime(2026, 10, 16), Width = 220 },
            "TimePicker" => () => new TimePicker
                { SelectedTime = new TimeSpan(14, 30, 0), ClockIdentifier = "24HourClock", Width = 260 },
            "Label" => () => new Label { Content = "Label · 文本标签" },
            "SelectableTextBlock" => () => new SelectableTextBlock
                { Text = "可选择并复制这段文字。", TextWrapping = TextWrapping.Wrap, MaxWidth = 240 },
            "InfoCard" => () => new InfoCard
            {
                Header = "项目进度", Content = new TextBlock { Text = "内容根据实际文字自然伸展。", TextWrapping = TextWrapping.Wrap },
                Width = 260
            },
            "TabControl" => () => Tabs(),
            "Tag" => () => new Tag { Content = "处理中", Status = TagStatus.Processing },
            "Description" => () => new Description
                { Width = 300, Items = { new DescriptionItem { Header = "属性", Content = "类型、默认值与用途自然换行展示。" } } },
            _ => null
        };
    }

    private static bool HasExtraExamples(string name)
    {
        return name is
            "Button" or "RepeatButton" or "ToggleButton" or "IconButton" or "MultiSelectBox" or "InfoCard" or "Tag"
            or "Description";
    }

    private static List<Example> ExtraExamples(string name, Func<Control> factory)
    {
        var result = new List<Example>();
        switch (name)
        {
            case "Button":
            case "RepeatButton":
            case "ToggleButton":
            case "IconButton":
                var loading = factory();
                ButtonLoading.SetIsLoading((Button)loading, true);
                result.Add(new Example("加载中", "ButtonLoading.IsLoading=\"True\"", loading));
                if (name == "IconButton")
                    foreach (var placement in Enum.GetValues<IconPlacement>())
                    {
                        var icon = (IconButton)factory();
                        icon.IconPlacement = placement;
                        result.Add(new Example(placement.ToString(), "IconPlacement=\"" + placement + "\"", icon));
                    }

                break;
            case "MultiSelectBox":
                var bounded = (MultiSelectBox)factory();
                bounded.MaxVisibleTags = 1;
                result.Add(new Example("限制可见标签", "MaxVisibleTags=\"1\"", bounded));
                var countOnly = (MultiSelectBox)factory();
                countOnly.MaxVisibleTags = 0;
                result.Add(new Example("仅显示选中数量", "MaxVisibleTags=\"0\"", countOnly));
                var searched = (MultiSelectBox)factory();
                searched.SearchText = "Development";
                result.Add(new Example("预设搜索条件", "SearchText=\"Development\"", searched, "展开后查看已筛选的选项；已选值保留。"));
                break;
            case "InfoCard":
                result.Add(new Example("操作与页脚区域", "HeaderActions · Footer", new InfoCard
                {
                    Header = "项目进度", HeaderActions = new Button { Content = "详情" }, Width = 280,
                    Content = new TextBlock { Text = "操作区域与页脚通过本库额外属性配置。", TextWrapping = TextWrapping.Wrap },
                    Footer = new TextBlock { Text = "更新于今天", TextWrapping = TextWrapping.Wrap }
                }));
                break;
            case "Tag":
                foreach (var variant in Enum.GetValues<TagVariant>())
                foreach (var status in Enum.GetValues<TagStatus>())
                    result.Add(new Example(status.ToString(), $"Variant=\"{variant}\" · Status=\"{status}\"",
                        new Tag { Content = status.ToString(), Variant = variant, Status = status }));
                break;
            case "Description":
                result.Add(new Example("紧凑标签列", "LabelWidth=\"80\" · StackWidth=\"0\"", new Description
                {
                    Width = 300, LabelWidth = 80, StackWidth = 0,
                    Items = { new DescriptionItem { Header = "标签", Content = "明确设置标签宽度，保持两列展示。" } }
                }));
                result.Add(new Example("上下排列", "StackWidth=\"520\"", new Description
                {
                    Width = 300, StackWidth = 520,
                    Items = { new DescriptionItem { Header = "标签", Content = "可用宽度小于阈值时，说明自然排列在标签下方。" } }
                }));
                break;
        }

        return result;
    }

    private static TabControl Tabs()
    {
        return new TabControl
        {
            Width = 260,
            Items =
            {
                new TabItem
                {
                    Header = "概览",
                    Content = new TextBlock
                        { Text = "当前选项卡的内容自然排版。", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(8) }
                },
                new TabItem { Header = "记录", Content = new TextBlock { Text = "第二个选项卡", Margin = new Thickness(8) } }
            }
        };
    }

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
                new DataGridTextColumn
                    { Header = "任务", Binding = CompiledBinding.Create<GridStyleRow, string>(row => row.Name) },
                new DataGridTextColumn
                    { Header = "状态", Binding = CompiledBinding.Create<GridStyleRow, string>(row => row.State) }
            }
        };
    }

    private static Flyout Actions()
    {
        return new Flyout
        {
            Content = new StackPanel
                { Spacing = 8, Children = { new Button { Content = "另存为" }, new Button { Content = "导出副本" } } }
        };
    }

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
        panel.Children.Add(new TextBlock
            { Text = title, FontSize = 17, FontWeight = FontWeight.SemiBold, TextWrapping = TextWrapping.Wrap });
        panel.Children.Add(Muted(explanation));
        panel.Children.Add(cells);
        var border = new Border
        {
            BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(10), Padding = new Thickness(20),
            Child = panel
        };
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

    private sealed record GridStyleRow(string Name, string State);

    private sealed record Example(string Title, string Configuration, Control Sample, string? Note = null);

    // WrapPanel can give a cell less than its preferred width in a narrow page.
    // Measure the text and sample against that width, without fixing the cell's height.
    private sealed class SampleCell(double preferredWidth) : Decorator
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            var width = double.IsFinite(availableSize.Width)
                ? Math.Min(preferredWidth, availableSize.Width)
                : preferredWidth;
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