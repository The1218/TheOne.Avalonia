using Avalonia;
using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class ShowcasePage : UserControl
{
    private readonly Dictionary<int, UserControl> _views = [];
    private readonly Dictionary<int, Border> _sections = [];
    public event EventHandler<int>? ControlRequested;

    public ShowcasePage() => InitializeComponent();

    public ShowcasePage(PreviewPageDescriptor page) : this()
    {
        DataContext = page;
        // The catalogue has a fixed, small number of sections. Each growing sample owns a bounded viewport.
        foreach (var id in page.ControlIds!)
        {
            var descriptor = PreviewPageCatalog.Controls.First(control => control.Id == id);
            var view = PreviewPageCatalog.CreateControl(id);
            var section = new Border { Classes = { "section" }, Child = new StackPanel { Spacing = 20, Children = { view, new ControlDetailsView(descriptor.Title, view) } } };
            _views.Add(id, view);
            _sections.Add(id, section);
            Sections.Children.Add(section);
            var link = new Button { Content = descriptor.Title, Margin = new Thickness(0, 8, 8, 0) };
            link.Click += (_, _) => ControlRequested?.Invoke(this, id);
            SectionLinks.Children.Add(link);
        }
    }
    public UserControl GetControlView(int id) => _views[id];
    public Border GetSection(int id) => _sections[id];
    internal IEnumerable<Control> SourceViews => _views.Values;
}
