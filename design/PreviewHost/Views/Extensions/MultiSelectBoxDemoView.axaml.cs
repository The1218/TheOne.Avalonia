using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class MultiSelectBoxDemoView : UserControl
{
    public MultiSelectBoxDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public MultiSelectBoxDemoViewModel Model { get; } = new();
    public MultiSelectBox Sample => this.FindControl<MultiSelectBox>("MultiSelectSample")!;
}