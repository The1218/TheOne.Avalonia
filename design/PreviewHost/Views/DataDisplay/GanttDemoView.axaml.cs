using Avalonia.Controls;
using Avalonia.Interactivity;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class GanttDemoView : UserControl
{
    public GanttDemoViewModel Model { get; } = new();
    private GanttChart Chart => this.FindControl<GanttChart>("Schedule")!;
    public GanttDemoView() { DataContext = Model; global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this); }
    private void TodayClick(object? sender, RoutedEventArgs e) => Chart.ScrollToToday();
    private void FitClick(object? sender, RoutedEventArgs e) => Chart.FitToProject();
    private void DayClick(object? sender, RoutedEventArgs e) => Model.DayWidth = 48;
    private void WeekClick(object? sender, RoutedEventArgs e) => Model.DayWidth = 16;
    private void MonthClick(object? sender, RoutedEventArgs e) => Model.DayWidth = 5;
    private void ZoomOutClick(object? sender, RoutedEventArgs e) => Model.DayWidth = Math.Max(4, Model.DayWidth / 1.2);
    private void ZoomInClick(object? sender, RoutedEventArgs e) => Model.DayWidth = Math.Min(240, Model.DayWidth * 1.2);
    private void ExpandClick(object? sender, RoutedEventArgs e) { foreach (var task in Model.Tasks) task.IsExpanded = true; }
    private void CollapseClick(object? sender, RoutedEventArgs e) { foreach (var task in Model.Tasks) task.IsExpanded = false; }
    private void LastClick(object? sender, RoutedEventArgs e) => Chart.BringTaskIntoView(Model.LargeData ? "extra-1499" : "acceptance");
}
