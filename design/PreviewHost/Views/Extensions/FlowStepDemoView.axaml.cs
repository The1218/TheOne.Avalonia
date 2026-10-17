using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class FlowStepDemoView : UserControl
{
    public FlowStepDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
        SizeChanged += (_, _) => UpdateColumns();
        UpdateColumns();
    }

    public FlowStepDemoViewModel Model { get; } = new();

    private void UpdateColumns()
    {
        var grid = this.FindControl<Grid>("LowerExamples")!;
        var custom = this.FindControl<StackPanel>("CustomExamples")!;
        var compact = Bounds.Width < 780;
        grid.ColumnDefinitions = ColumnDefinitions.Parse(compact ? "*" : "*,*");
        grid.RowDefinitions = RowDefinitions.Parse(compact ? "Auto,Auto" : "Auto");
        Grid.SetColumn(custom, compact ? 0 : 1);
        Grid.SetRow(custom, compact ? 1 : 0);
    }
}