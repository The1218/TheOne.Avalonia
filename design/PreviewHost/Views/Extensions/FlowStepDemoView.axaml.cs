using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class FlowStepDemoView : UserControl
{
    public FlowStepDemoViewModel Model { get; } = new();
    public FlowStepDemoView()
    {
        DataContext = Model;
        global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
        SizeChanged += (_, _) => UpdateColumns();
        UpdateColumns();
    }
    private void UpdateColumns()
    {
        var grid = this.FindControl<Grid>("LowerExamples")!;
        var custom = this.FindControl<StackPanel>("CustomExamples")!;
        var compact = Bounds.Width < 780;
        grid.ColumnDefinitions = ColumnDefinitions.Parse(compact ? "*" : "*,*");
        grid.RowDefinitions = RowDefinitions.Parse(compact ? "Auto,Auto" : "Auto");
        Grid.SetColumn(custom, compact ? 0 : 1); Grid.SetRow(custom, compact ? 1 : 0);
    }
}
