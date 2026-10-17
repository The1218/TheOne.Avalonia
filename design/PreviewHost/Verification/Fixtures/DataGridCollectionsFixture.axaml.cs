using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class DataGridCollectionsFixture : UserControl
{
    public DataGridCollectionsFixture()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public DataGridBindingDemoViewModel Model { get; } = new();
    public DataGrid List => this.FindControl<DataGrid>("ListGrid")!;
    public DataGrid Typed => this.FindControl<DataGrid>("TypedViewGrid")!;
    public DataGrid Observable => this.FindControl<DataGrid>("ObservableGrid")!;
}