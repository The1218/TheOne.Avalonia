using Avalonia;
using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class ShowcasePage : UserControl
{
    private readonly Dictionary<int, Border> _sections = [];
    private readonly Dictionary<int, Lazy<UserControl>> _views = [];

    public ShowcasePage()
    {
        InitializeComponent();
    }

    public ShowcasePage(PreviewPageDescriptor page) : this()
    {
        DataContext = page;
        // Explicit view lookup remains available; content policy decides what is actually presented.
        foreach (var id in page.ControlIds!)
        {
            var descriptor = PreviewPageCatalog.Controls.First(control => control.Id == id);
            var view = new Lazy<UserControl>(() => PreviewPageCatalog.CreateControl(id));
            var content = PreviewPageContentPolicy.CreateContent(descriptor.Title, () => view.Value);
            if (ControlDetailsView.CreateDeferred(descriptor.Title) is { } details) content.Children.Add(details);
            var section = new Border { Classes = { "section" }, Child = content };
            _views.Add(id, view);
            _sections.Add(id, section);
            Sections.Children.Add(section);
            var link = new Button { Content = descriptor.Title, Margin = new Thickness(0, 8, 8, 0) };
            link.Click += (_, _) => ControlRequested?.Invoke(this, id);
            SectionLinks.Children.Add(link);
        }
    }

    public event EventHandler<int>? ControlRequested;

    public UserControl GetControlView(int id)
    {
        return _views[id].Value;
    }

    public Border GetSection(int id)
    {
        return _sections[id];
    }
}
