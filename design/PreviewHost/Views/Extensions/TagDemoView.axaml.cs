using Avalonia.Controls;
namespace TheOne.DesignPreview;
public partial class TagDemoView : UserControl
{
    public TagDemoViewModel Model { get; } = new();
    public TagDemoView() { DataContext = Model; global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this); }
}
