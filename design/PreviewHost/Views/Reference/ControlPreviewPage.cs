using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace TheOne.DesignPreview;

internal sealed class ControlPreviewPage : UserControl
{

    internal ControlPreviewPage(PreviewPageDescriptor descriptor, UserControl demo)
    {
        Demo = demo;
        var plan = PreviewPageContentPolicy.For(descriptor.Title);
        UsesViewport = plan.UsesViewport;
        var supplements = new StackPanel { Spacing = 24 };
        if (UsesViewport && ControlStyleShowcase.Create(descriptor.Title) is { } styles)
            supplements.Children.Add(styles);
        // These pages author property/API/model Descriptions beside their live examples.
        if (descriptor.Title is not ("NavPage" or "TreeSelect" or "OneIcon" or "Breadcrumb" or "DateTimePicker" or "Timeline") &&
            ControlDetailsView.CreateDeferred(descriptor.Title) is { } details)
            supplements.Children.Add(details);
        foreach (var name in descriptor.Title switch
                 {
                     "Form" => new[] { "FormItem" }, "FlowStep" => new[] { "FlowNode" },
                     "LoadingContainer" => new[] { "LoadingIndicator" },
                     "ColorPicker" => new[] { "ColorView" }, "RefreshContainer" => new[] { "RefreshVisualizer" },
                     "Description" => new[] { "DescriptionItem" }, _ => Array.Empty<string>()
                 })
            if (ControlDetailsView.CreateDeferred(name) is { } extra) supplements.Children.Add(extra);
        // Canvas editors own the remaining viewport; documentation has a separate
        // scroll surface and cannot steal height from or overlap the live editor.
        Content = UsesViewport
            ? new TabControl
            {
                Items =
                {
                    new TabItem { Header = "工作区", Content = demo },
                    new TabItem
                    {
                        Header = "样式、属性与 API",
                        Content = new ScrollViewer
                            { Content = supplements, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled }
                    }
                }
            }
            : Presentation(descriptor.Title, demo, supplements);
    }

    internal UserControl Demo { get; }
    internal bool UsesViewport { get; }

    private Control Presentation(string name, UserControl demo, Control supplements)
    {
        var page = PreviewPageContentPolicy.CreateContent(name, demo);
        if (supplements is Panel { Children.Count: > 0 }) page.Children.Add(supplements);
        return page;
    }

    internal void SetViewportHeight(double height)
    {
        if (Demo is GanttDemoView gantt) gantt.SetViewportHeight(height);
    }
}
