using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class ControlVariantsFixture : UserControl
{
    public ControlVariantsFixture()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public ControlVariantsFixtureModel Model { get; } = new();
}