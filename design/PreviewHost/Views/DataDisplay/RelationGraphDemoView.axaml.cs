using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class RelationGraphDemoView : UserControl
{
    public RelationGraphDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public RelationGraphDemoViewModel Model { get; } = new();
}