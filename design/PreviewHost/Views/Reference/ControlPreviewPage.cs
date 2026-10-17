using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace TheOne.DesignPreview;

internal sealed class ControlPreviewPage : UserControl
{
    private readonly List<Control> _sourceViews = [];

    internal ControlPreviewPage(PreviewPageDescriptor descriptor, UserControl demo)
    {
        Demo = demo;
        var plan = PreviewPageContentPolicy.For(descriptor.Title);
        if (plan.ShowOriginalDemo) _sourceViews.Add(demo);
        UsesViewport = plan.UsesViewport;
        Details = new ControlDetailsView(descriptor.Title);
        var supplements = new StackPanel { Spacing = 24 };
        if (UsesViewport && ControlStyleShowcase.Create(descriptor.Title) is { } styles)
            supplements.Children.Add(styles);
        // NavPage authors the same property/API/model Description beside its live shell.
        if (descriptor.Title != "NavPage") supplements.Children.Add(Details);
        foreach (var name in descriptor.Title switch
                 {
                     "Form" => new[] { "FormItem" }, "FlowStep" => new[] { "FlowNode" },
                     "LoadingContainer" => new[] { "LoadingIndicator" },
                     "ColorPicker" => new[] { "ColorView" }, "RefreshContainer" => new[] { "RefreshVisualizer" },
                     "Description" => new[] { "DescriptionItem" }, _ => Array.Empty<string>()
                 }) supplements.Children.Add(new ControlDetailsView(name));
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
    internal ControlDetailsView Details { get; }
    internal bool UsesViewport { get; }
    internal IEnumerable<Control> SourceViews => _sourceViews;

    private Control Presentation(string name, UserControl demo, Control supplements)
    {
        var page = PreviewPageContentPolicy.CreateContent(name, demo);
        _sourceViews.AddRange(page.Children.OfType<NativeThemePreview>());
        page.Children.Add(supplements);
        return page;
    }

    internal void SetViewportHeight(double height)
    {
        if (Demo is GanttDemoView gantt) gantt.SetViewportHeight(height);
    }
}
