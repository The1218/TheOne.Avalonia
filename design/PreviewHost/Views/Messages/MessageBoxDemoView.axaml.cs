using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class MessageBoxDemoView : UserControl
{
    public MessageBoxDemoViewModel Model { get; } = new();
    public MessageBoxDemoView() { DataContext = Model; global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this); }
}
