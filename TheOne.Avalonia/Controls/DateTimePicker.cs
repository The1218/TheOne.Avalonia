using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

/// <summary>A native calendar input with an integrated 24-hour time editor.</summary>
/// <remarks>SelectedDate contains both the date and time. Range limits apply to calendar dates.</remarks>
public sealed class DateTimePicker : CalendarDatePicker
{
    private Calendar? _calendar;
    private DispatcherOperation? _calendarSync;

    /// <summary>The display format; null uses yyyy-MM-dd HH:mm.</summary>
    public static readonly StyledProperty<string?> FormatProperty =
        CalendarOptions.FormatProperty.AddOwner<DateTimePicker>();

    /// <summary>The time of day, synchronized with SelectedDate without replacing bindings.</summary>
    public static readonly StyledProperty<TimeSpan?> TimeProperty =
        CalendarOptions.TimeProperty.AddOwner<DateTimePicker>(
            new StyledPropertyMetadata<TimeSpan?>(defaultBindingMode: BindingMode.TwoWay));

    public DateTimePicker()
    {
        CalendarOptions.SetDisplayStyle(this, DateInputDisplayStyle.DateTime);
        CalendarOpened += (_, _) => SynchronizeCalendar();
    }

    protected override Type StyleKeyOverride => typeof(CalendarDatePicker);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _calendarSync?.Abort();
        _calendarSync = null;
        _calendar = null;
        base.OnApplyTemplate(e);
        _calendar = e.NameScope.Find<Calendar>("PART_Calendar");
        SynchronizeCalendar();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedDateProperty && _calendar is not null && _calendarSync is null)
        {
            // Native calendar selection writes a local value, replacing the template binding.
            // Wait until its SelectedDates mutation has finished, then use the latest owner value.
            _calendarSync = Dispatcher.UIThread.InvokeAsync(() =>
            {
                _calendarSync = null;
                SynchronizeCalendar();
            }, DispatcherPriority.Loaded);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _calendarSync?.Abort();
        _calendarSync = null;
        base.OnDetachedFromVisualTree(e);
    }

    private void SynchronizeCalendar()
    {
        if (_calendar is not null && _calendar.SelectedDate != SelectedDate)
            _calendar.SetCurrentValue(Calendar.SelectedDateProperty, SelectedDate);
    }

    public string? Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    public TimeSpan? Time
    {
        get => GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }
}
