using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class PaginationDemoView : UserControl
{
    public PaginationDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public PaginationDemoViewModel Model { get; } = new();
}