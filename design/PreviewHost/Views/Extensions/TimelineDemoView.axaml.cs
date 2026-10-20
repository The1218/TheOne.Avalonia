using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class TimelineDemoView : UserControl
{
    public TimelineDemoView() { DataContext = Model; AvaloniaXamlLoader.Load(this); }
    public TimelineDemoViewModel Model { get; } = new();
    private void AddEvent(object? sender, RoutedEventArgs e) => Model.Add();
    private void RemoveEvent(object? sender, RoutedEventArgs e) { if (Model.Events.Count > 0) Model.Events.RemoveAt(Model.Events.Count - 1); }
    private void ResetEvents(object? sender, RoutedEventArgs e) => Model.Reset();
    private void CycleStatus(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: TimelineItemModel model })
            model.Status = (TimelineStatus)(((int)model.Status + 1) % Enum.GetValues<TimelineStatus>().Length);
    }
}
