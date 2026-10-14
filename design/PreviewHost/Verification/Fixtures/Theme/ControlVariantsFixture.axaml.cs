using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class ControlVariantsFixture : UserControl
{
    public ControlVariantsFixtureModel Model { get; } = new();
    public ControlVariantsFixture()
    {
        DataContext = Model;
        global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
}
