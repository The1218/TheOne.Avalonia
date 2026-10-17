using Avalonia.Controls;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed record NavPageRoute(string Name, object? Screen, IReadOnlyList<NavPageRoute> Nodes)
{
    public object? Glyph => null;
}

public sealed class NavPageBindingModel
{
    public string Toolbar => "Compiled toolbar";
    public IReadOnlyList<NavPageRoute> Routes { get; } = [new("自定义导航", "Custom VM", [])];
}

public partial class NavPageBindingFixture : UserControl
{
    public NavPageBindingFixture()
    {
        DataContext = Model;
        global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }

    public NavPageBindingModel Model { get; } = new();
    public NavPage Shell => this.FindControl<NavPage>("NavigationShell")!;
}
