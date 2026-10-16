using Avalonia.Controls;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class MultiSelectBoxDemoView : UserControl
{
    public MultiSelectBoxDemoViewModel Model { get; } = new();
    public MultiSelectBox Sample => this.FindControl<MultiSelectBox>("MultiSelectSample")!;
    public MultiSelectBoxDemoView()
    {
        DataContext = Model;
        global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
}
