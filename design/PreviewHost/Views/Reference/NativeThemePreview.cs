using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

/// <summary>One visual reference for native controls without a library-specific comparison matrix.</summary>
internal sealed class NativeThemePreview : UserControl
{
    internal static NativeThemePreview Create(string name)
    {
        Control sample = name switch
        {
            "CheckBox" => new CheckBox { Content = "接收项目通知", IsChecked = true },
            "RadioButton" => new RadioButton { Content = "默认工作区", IsChecked = true, GroupName = "DefaultThemePreview" },
            "ToggleSwitch" => new ToggleSwitch { Content = "自动更新", IsChecked = true },
            "Slider" => new Slider { Value = 60, MinWidth = 180, MaxWidth = 300 },
            "Calendar" => new Calendar { SelectedDate = new DateTime(2026, 10, 16), MinWidth = 0, MaxWidth = 360 },
            "ItemsControl" => new ItemsControl { Items = { "设计工作区", "开发工作区", "测试工作区" } },
            "Menu" => Menu(),
            "ContextMenu" => ContextMenuTarget(),
            "Expander" => new Expander { Header = "项目详情", IsExpanded = true, Content = Text("主题随应用配色和明暗模式更新。") },
            "ScrollViewer" => ScrollSample(),
            "ScrollBar" => new ScrollBar { Orientation = Orientation.Horizontal, Maximum = 100, Value = 35, ViewportSize = 20, MinWidth = 180, MaxWidth = 320 },
            "Separator" => new StackPanel { Spacing = 12, Children = { Text("概览"), new Separator(), Text("项目内容") } },
            "ToolTip" => ToolTipTarget(),
            "SplitView" => new SplitView
            {
                DisplayMode = SplitViewDisplayMode.Inline, IsPaneOpen = true, OpenPaneLength = 120,
                Pane = Surface("导航区域"), Content = new Border { Padding = new Thickness(16), Child = Text("当前工作区") }
            },
            "GridSplitter" => Splitter(),
            "FlyoutPresenter" => new FlyoutPresenter { Content = new StackPanel { Spacing = 8, Children = { Text("项目备注"), Text("内容弹层的默认主题。") } } },
            "ColorPicker" => new ColorPicker { Color = Color.FromRgb(0, 96, 97), HorizontalAlignment = HorizontalAlignment.Left },
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, "No default theme preview is defined.")
        };
        sample.Name = "DefaultTheme_" + name;
        return new NativeThemePreview { Content = PreviewSection.Create(name + " · 默认主题预览", sample) };
    }

    internal static NativeThemePreview CreateProjectFeature(string name)
    {
        if (name != "TabControl") throw new ArgumentOutOfRangeException(nameof(name));
        var tabs = new TabControl { Name = "TabFeaturesSample", Items =
        {
            new TabItem { Header = "需求", Content = Text("整理工作区需求。") },
            new TabItem { Header = "开发", Content = Text("实现当前项目。") },
            new TabItem { Header = "验收", Content = Text("查看交付记录。") }
        } };
        tabs.Classes.Add("card");
        TabFeatures.SetCanClose(tabs, true); TabFeatures.SetCanReorder(tabs, true); Virtualization.SetEnabled(tabs, true);
        return new NativeThemePreview { Content = PreviewSection.Create("TabControl · TabFeatures 关闭与重排", new StackPanel { Spacing = 12, Children =
        {
            Text("Classes=\"card\" · TabFeatures.CanClose=\"True\" · TabFeatures.CanReorder=\"True\"。拖动标题调整顺序，选中后点击关闭按钮。"), tabs
        } }) };
    }

    private static Menu Menu() => new() { Items =
    {
        new MenuItem { Header = "文件", Items = { new MenuItem { Header = "新建项目" }, new MenuItem { Header = "打开工作区" } } },
        new MenuItem { Header = "视图", Items = { new MenuItem { Header = "项目概览" } } }
    } };

    private static Button ContextMenuTarget() => new()
    {
        Content = "右键查看菜单", HorizontalAlignment = HorizontalAlignment.Left,
        ContextMenu = new ContextMenu { Items = { new MenuItem { Header = "复制" }, new MenuItem { Header = "查看详情" } } }
    };

    private static Button ToolTipTarget()
    {
        var target = new Button { Content = "移入查看提示", HorizontalAlignment = HorizontalAlignment.Left };
        ToolTip.SetTip(target, "项目的默认提示样式。"); return target;
    }

    private static ScrollViewer ScrollSample()
    {
        var rows = new StackPanel { Spacing = 12 };
        for (var i = 1; i <= 8; i++) rows.Children.Add(Text("项目记录 " + i));
        return new ScrollViewer { Height = 140, MaxWidth = 320, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, Content = rows };
    }

    private static Grid Splitter()
    {
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,6,*"), MinHeight = 120, MaxWidth = 400 };
        var left = Surface("左侧内容"); var right = Surface("右侧内容");
        var splitter = new GridSplitter { ResizeDirection = GridResizeDirection.Columns };
        Grid.SetColumn(splitter, 1); Grid.SetColumn(right, 2);
        grid.Children.Add(left); grid.Children.Add(splitter); grid.Children.Add(right); return grid;
    }

    private static Border Surface(string text)
    {
        var surface = new Border { Padding = new Thickness(16), Child = Text(text) };
        surface.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Subtle")); return surface;
    }

    private static TextBlock Text(string value) => new() { Text = value, TextWrapping = TextWrapping.Wrap };
}
