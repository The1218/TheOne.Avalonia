using Avalonia.Controls;
namespace TheOne.DesignPreview;

public partial class RelationGraphDemoView : UserControl
{
    public RelationGraphDemoViewModel Model { get; } = new();
    public RelationGraphDemoView() { DataContext = Model; global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this); }
}
