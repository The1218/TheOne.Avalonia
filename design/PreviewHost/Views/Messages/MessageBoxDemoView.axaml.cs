using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class MessageBoxDemoView : UserControl
{
    public MessageBoxDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public MessageBoxDemoViewModel Model { get; } = new();
}