using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class DateTimePickerDemoViewModel : ReactiveObject
{
    public DateTime? Appointment
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new DateTime(2026, 10, 20, 14, 30, 15);

    public DateTime? EmptyDate
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public TimeSpan? Time
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}

public partial class DateTimePickerDemoView : UserControl
{
    public DateTimePickerDemoView()
    {
        DataContext = new DateTimePickerDemoViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void ClearDate(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DateTimePickerDemoViewModel model)
        {
            model.EmptyDate = null;
            model.Time = null;
        }
    }
}
