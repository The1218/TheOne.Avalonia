using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class DataGridCollectionsFixture : UserControl
{
    public DataGridBindingDemoViewModel Model { get; } = new();
    public DataGrid List => this.FindControl<DataGrid>("ListGrid")!;
    public DataGrid Typed => this.FindControl<DataGrid>("TypedViewGrid")!;
    public DataGrid Observable => this.FindControl<DataGrid>("ObservableGrid")!;
    public DataGridCollectionsFixture() { DataContext = Model; global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this); }
}
