using Avalonia;
using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class ShowcasePage : UserControl
{
    private readonly Dictionary<int, Border> _sections = [];
    private readonly List<Control> _sourceViews = [];
    private readonly Dictionary<int, UserControl> _views = [];

    public ShowcasePage()
    {
        InitializeComponent();
    }

    public ShowcasePage(PreviewPageDescriptor page) : this()
    {
        DataContext = page;
        // Original instances remain cached for source lookup; content policy decides what is actually presented.
        foreach (var id in page.ControlIds!)
        {
            var descriptor = PreviewPageCatalog.Controls.First(control => control.Id == id);
            var view = PreviewPageCatalog.CreateControl(id);
            var content = PreviewPageContentPolicy.CreateContent(descriptor.Title, view);
            if (PreviewPageContentPolicy.For(descriptor.Title).ShowOriginalDemo) _sourceViews.Add(view);
            _sourceViews.AddRange(content.Children.OfType<NativeThemePreview>());
            content.Children.Add(new ControlDetailsView(descriptor.Title));
            var section = new Border { Classes = { "section" }, Child = content };
            _views.Add(id, view);
            _sections.Add(id, section);
            Sections.Children.Add(section);
            var link = new Button { Content = descriptor.Title, Margin = new Thickness(0, 8, 8, 0) };
            link.Click += (_, _) => ControlRequested?.Invoke(this, id);
            SectionLinks.Children.Add(link);
        }
    }

    internal IEnumerable<Control> SourceViews => _sourceViews;
    public event EventHandler<int>? ControlRequested;

    public UserControl GetControlView(int id)
    {
        return _views[id];
    }

    public Border GetSection(int id)
    {
        return _sections[id];
    }
}