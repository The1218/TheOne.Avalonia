using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class PaginationDemoView : UserControl
{
    public PaginationDemoViewModel Model { get; } = new();
    public PaginationDemoView()
    {
        DataContext = Model;
        global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
}
