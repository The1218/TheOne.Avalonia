using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class GanttDemoView : UserControl
{
    public GanttDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
        SetViewportHeight(720);
    }

    public GanttDemoViewModel Model { get; } = new();
    private GanttChart Chart => this.FindControl<GanttChart>("Schedule")!;

    internal void SetViewportHeight(double height)
    {
        if (!double.IsFinite(height) || height <= 0) return;
        // Bound only the virtual task viewport. The full demo and its toolbar,
        // status, details and documentation keep their natural height.
        Chart.Height = Math.Max(Chart.HeaderHeight + Chart.RowHeight * 3, height * .65);
    }

    private void TodayClick(object? sender, RoutedEventArgs e)
    {
        Chart.ScrollToToday();
    }

    private void FitClick(object? sender, RoutedEventArgs e)
    {
        Chart.FitToProject();
    }

    private void DayClick(object? sender, RoutedEventArgs e)
    {
        Model.DayWidth = 48;
    }

    private void WeekClick(object? sender, RoutedEventArgs e)
    {
        Model.DayWidth = 16;
    }

    private void MonthClick(object? sender, RoutedEventArgs e)
    {
        Model.DayWidth = 5;
    }

    private void ZoomOutClick(object? sender, RoutedEventArgs e)
    {
        Model.DayWidth = Math.Max(4, Model.DayWidth / 1.2);
    }

    private void ZoomInClick(object? sender, RoutedEventArgs e)
    {
        Model.DayWidth = Math.Min(240, Model.DayWidth * 1.2);
    }

    private void ExpandClick(object? sender, RoutedEventArgs e)
    {
        foreach (var task in Model.Tasks) task.IsExpanded = true;
    }

    private void CollapseClick(object? sender, RoutedEventArgs e)
    {
        foreach (var task in Model.Tasks) task.IsExpanded = false;
    }

    private void LastClick(object? sender, RoutedEventArgs e)
    {
        Chart.BringTaskIntoView(Model.LargeData ? "extra-1499" : "acceptance");
    }
}