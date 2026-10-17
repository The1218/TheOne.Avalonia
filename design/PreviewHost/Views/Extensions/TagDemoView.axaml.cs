using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class TagDemoView : UserControl
{
    public TagDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public TagDemoViewModel Model { get; } = new();
}