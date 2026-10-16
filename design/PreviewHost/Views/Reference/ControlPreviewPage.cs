using Avalonia.Controls;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal sealed class ControlPreviewPage : UserControl
{
    internal UserControl Demo { get; }
    internal ControlDetailsView Details { get; }
    internal bool UsesViewport { get; }
    internal ControlPreviewPage(PreviewPageDescriptor descriptor, UserControl demo)
    {
        Demo = demo; UsesViewport = demo is WorkflowEditorDemoView or ReportDesign or ReportPreview;
        Details = new ControlDetailsView(descriptor.Title, demo, UsesViewport);
        var supplements = new StackPanel { Spacing = 16, Children = { Details } };
        foreach (var name in descriptor.Title switch
        {
            "Form" => new[] { "FormItem" }, "FlowStep" => new[] { "FlowNode" }, "LoadingContainer" => new[] { "LoadingIndicator" },
            "ColorPicker" => new[] { "ColorView" }, "RefreshContainer" => new[] { "RefreshVisualizer" }, _ => Array.Empty<string>()
        }) supplements.Children.Add(new ControlDetailsView(name, demo));
        if (UsesViewport)
        {
            AttachedToVisualTree += (_, _) => Details.Observe();
            DetachedFromVisualTree += (_, _) => Details.StopObserving();
        }
        // Canvas editors own the remaining viewport; documentation has a separate
        // scroll surface and cannot steal height from or overlap the live editor.
        Content = UsesViewport ? new TabControl { Items =
        {
            new TabItem { Header = "完整示例", Content = demo },
            new TabItem { Header = "样式、属性与 API", Content = new ScrollViewer { Content = supplements, HorizontalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled } }
        } } : new StackPanel { Spacing = 24, Children = { demo, supplements } };
    }
    internal void SetViewportHeight(double height)
    {
        if (Demo is GanttDemoView gantt) gantt.SetViewportHeight(height);
    }
}
