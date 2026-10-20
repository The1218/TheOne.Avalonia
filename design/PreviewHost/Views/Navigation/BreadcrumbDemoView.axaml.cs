using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class BreadcrumbDemoView : UserControl
{
    public BreadcrumbDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public BreadcrumbDemoViewModel Model { get; } = new();
    private void ResetPath(object? sender, RoutedEventArgs e) => Model.Reset();
}
