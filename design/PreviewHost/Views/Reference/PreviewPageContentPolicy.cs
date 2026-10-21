using Avalonia.Controls;

namespace TheOne.DesignPreview;

internal enum PreviewPageContentMode
{
    ExtensionComparisons,
    DefaultTheme,
    ProjectFeature,
    BusinessExample,
    Workspace
}

internal sealed record PreviewPageContentPlan(
    string Name,
    PreviewPageContentMode Mode,
    bool ShowOriginalDemo,
    bool ShowStyleShowcase,
    bool UsesViewport,
    string Reason);

/// <summary>One content decision per catalogue control; documentation and source ownership are independent.</summary>
internal static class PreviewPageContentPolicy
{
    internal static IReadOnlyList<PreviewPageContentPlan> All { get; } =
        PreviewPageCatalog.Controls.Select(control => Resolve(control.Title)).ToArray();

    internal static PreviewPageContentPlan For(string name)
    {
        return All.Single(plan => plan.Name == name);
    }

    internal static StackPanel CreateContent(string name, UserControl original)
        => CreateContent(name, () => original);

    internal static StackPanel CreateContent(string name, Func<UserControl> original)
    {
        var plan = For(name);
        var content = new StackPanel { Name = "PageContent_" + name, Spacing = 24 };
        if (plan.ShowStyleShowcase && ControlStyleShowcase.Create(name) is { } styles) content.Children.Add(styles);
        if (plan.Mode == PreviewPageContentMode.DefaultTheme) content.Children.Add(NativeThemePreview.Create(name));
        if (plan.Mode == PreviewPageContentMode.ProjectFeature && !plan.ShowOriginalDemo)
            content.Children.Add(NativeThemePreview.CreateProjectFeature(name));
        if (plan.ShowOriginalDemo) content.Children.Add(original());
        return content;
    }

    private static PreviewPageContentPlan Resolve(string name)
    {
        var mode = name switch
        {
            "Button" or "RepeatButton" or "ToggleButton" or "IconButton" or "SplitButton" or "ToggleSplitButton" or
                "TextBox" or "NumericUpDown" or "AutoCompleteBox" or "ComboBox" or "ListBox" or "ProgressBar" or
                "DatePicker" or "CalendarDatePicker" or "TimePicker" or "Label" or "SelectableTextBlock" or
                "Tag" or "InfoCard" or "Description" => PreviewPageContentMode.ExtensionComparisons,
            "CheckBox" or "RadioButton" or "ToggleSwitch" or "Slider" or "Calendar" or "ItemsControl" or
                "Menu" or "ContextMenu" or "Expander" or "ScrollViewer" or "ScrollBar" or "Separator" or "ToolTip" or
                "SplitView" or "GridSplitter" or "FlyoutPresenter"
                or "ColorPicker" => PreviewPageContentMode.DefaultTheme,
            "TabControl" or "TreeView" or "TabStrip" or "DataGrid" or "DataGridTree" or "Carousel" or
                "RefreshContainer" or "TransitioningContentControl" => PreviewPageContentMode.ProjectFeature,
            "WorkflowEditor" or "ReportDesign" or "ReportPreview" => PreviewPageContentMode.Workspace,
            _ => PreviewPageContentMode.BusinessExample
        };
        var original = mode is PreviewPageContentMode.BusinessExample or PreviewPageContentMode.Workspace ||
                       (mode == PreviewPageContentMode.ProjectFeature && name != "TabControl");
        var reason = mode switch
        {
            PreviewPageContentMode.ExtensionComparisons => "公开样式与本库额外属性已在同一矩阵展示，不重复原生完整用法。",
            PreviewPageContentMode.DefaultTheme => "一个默认主题代表样例，不展开原生属性、状态与 API。",
            PreviewPageContentMode.ProjectFeature when name == "TabControl" =>
                "样式矩阵加精简的 TabFeatures 关闭／重排示例，替代重复的完整原生演示。",
            PreviewPageContentMode.ProjectFeature => "保留本项目虚拟化、集合行为或数据扩展的针对场景。",
            PreviewPageContentMode.Workspace => "编辑器保留独立工作区与有限视口，说明放在独立标签页。",
            _ => "保留扩展控件的实际业务场景，直接呈现，不套重复完整用法分区。"
        };
        return new PreviewPageContentPlan(name, mode, original, ControlStyleShowcase.Supports(name),
            mode == PreviewPageContentMode.Workspace, reason);
    }
}
