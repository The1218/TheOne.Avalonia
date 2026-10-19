using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class TreeSelectDemoView : UserControl
{
    public TreeSelectDemoView() { DataContext = Model; AvaloniaXamlLoader.Load(this); }
    public TreeSelectDemoViewModel Model { get; } = new();
    private void LoadLargeClicked(object? sender, RoutedEventArgs e)
    {
        Model.LoadLargeTree();
        var sample = this.FindControl<TreeSelect>("LargeSample")!;
        sample.SetExpanded("large-root", true);
        sample.OpenDropDown();
    }
}
