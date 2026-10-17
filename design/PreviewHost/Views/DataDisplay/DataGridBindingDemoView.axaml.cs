using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class DataGridBindingDemoView : UserControl
{
    public DataGridBindingDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public DataGridBindingDemoViewModel Model { get; } = new();
    public DataGrid Grid => this.FindControl<DataGrid>("CollectionGrid")!;
}