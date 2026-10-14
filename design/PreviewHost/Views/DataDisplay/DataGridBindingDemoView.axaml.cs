using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class DataGridBindingDemoView : UserControl
{
    public DataGridBindingDemoViewModel Model { get; } = new();
    public DataGrid Grid => this.FindControl<DataGrid>("CollectionGrid")!;
    public DataGridBindingDemoView() { DataContext = Model; global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this); }
}
