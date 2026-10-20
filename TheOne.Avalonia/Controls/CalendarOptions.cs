using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public enum DateInputDisplayStyle
{
    Date,
    DateTime,
    Month,
    Year
}

/// <summary>Date formatting, optional time editing and wheel navigation for native calendars.</summary>
public sealed class CalendarOptions : AvaloniaObject
{
    public static readonly AttachedProperty<string?> FormatProperty =
        AvaloniaProperty.RegisterAttached<CalendarOptions, CalendarDatePicker, string?>("Format");

    public static readonly AttachedProperty<DateInputDisplayStyle> DisplayStyleProperty =
        AvaloniaProperty.RegisterAttached<CalendarOptions, Control, DateInputDisplayStyle>("DisplayStyle");

    public static readonly AttachedProperty<CalendarMode> InitialViewProperty =
        AvaloniaProperty.RegisterAttached<CalendarOptions, CalendarDatePicker, CalendarMode>("InitialView");

    public static readonly AttachedProperty<bool> WheelNavigationProperty =
        AvaloniaProperty.RegisterAttached<CalendarOptions, Calendar, bool>("WheelNavigation");

    public static readonly AttachedProperty<TimeSpan?> TimeProperty =
        AvaloniaProperty.RegisterAttached<CalendarOptions, Control, TimeSpan?>("Time",
            validate: value => value is null || (value >= TimeSpan.Zero && value < TimeSpan.FromDays(1)));

    public static readonly AttachedProperty<bool> ShowTimeProperty =
        AvaloniaProperty.RegisterAttached<CalendarOptions, Control, bool>("ShowTime");

    private static readonly AttachedProperty<bool> UpdatingProperty =
        AvaloniaProperty.RegisterAttached<CalendarOptions, CalendarDatePicker, bool>("Updating");

    private static readonly AttachedProperty<bool> ChoosingDateProperty =
        AvaloniaProperty.RegisterAttached<CalendarOptions, CalendarDatePicker, bool>("ChoosingDate");

    static CalendarOptions()
    {
        FormatProperty.Changed.AddClassHandler<CalendarDatePicker>((picker, _) => ApplyFormat(picker));
        DisplayStyleProperty.Changed.AddClassHandler<Control>((control, _) =>
        {
            control.SetValue(ShowTimeProperty, GetDisplayStyle(control) == DateInputDisplayStyle.DateTime);
            if (control is CalendarDatePicker picker)
            {
                Attach(picker);
                ApplyFormat(picker);
                if (GetDisplayStyle(picker) == DateInputDisplayStyle.DateTime)
                    picker.SetCurrentValue(TimeProperty, picker.SelectedDate?.TimeOfDay ?? GetTime(picker));
            }
        });
        CalendarDatePicker.SelectedDateProperty.Changed.AddClassHandler<CalendarDatePicker>((picker, change) =>
        {
            if (picker.GetValue(UpdatingProperty) || GetDisplayStyle(picker) != DateInputDisplayStyle.DateTime) return;
            picker.SetValue(UpdatingProperty, true);
            try
            {
                if (picker.SelectedDate is not { } date)
                    picker.SetCurrentValue(TimeProperty, null);
                else if (picker.GetValue(ChoosingDateProperty))
                    picker.SetCurrentValue(CalendarDatePicker.SelectedDateProperty,
                        date.Date + (GetTime(picker) ?? TimeSpan.Zero));
                else picker.SetCurrentValue(TimeProperty, date.TimeOfDay);
            }
            finally
            {
                picker.SetValue(UpdatingProperty, false);
            }
        });
        TimeProperty.Changed.AddClassHandler<CalendarDatePicker>((picker, _) =>
        {
            if (picker.GetValue(UpdatingProperty) || GetDisplayStyle(picker) != DateInputDisplayStyle.DateTime ||
                picker.SelectedDate is not { } date) return;
            picker.SetValue(UpdatingProperty, true);
            try
            {
                picker.SetCurrentValue(CalendarDatePicker.SelectedDateProperty,
                    date.Date + (GetTime(picker) ?? TimeSpan.Zero));
            }
            finally
            {
                picker.SetValue(UpdatingProperty, false);
            }
        });
        InitialViewProperty.Changed.AddClassHandler<CalendarDatePicker>((picker, _) => Attach(picker));
        WheelNavigationProperty.Changed.AddClassHandler<Calendar>((calendar, change) =>
        {
            calendar.RemoveHandler(InputElement.PointerWheelChangedEvent, OnWheel);
            if (change.GetNewValue<bool>())
                calendar.AddHandler(InputElement.PointerWheelChangedEvent, OnWheel, RoutingStrategies.Tunnel);
        });
    }

    public static string? GetFormat(CalendarDatePicker picker)
    {
        return picker.GetValue(FormatProperty);
    }

    public static void SetFormat(CalendarDatePicker picker, string? value)
    {
        picker.SetValue(FormatProperty, value);
    }

    public static DateInputDisplayStyle GetDisplayStyle(Control control)
    {
        return control.GetValue(DisplayStyleProperty);
    }

    public static void SetDisplayStyle(Control control, DateInputDisplayStyle value)
    {
        control.SetValue(DisplayStyleProperty, value);
    }

    public static CalendarMode GetInitialView(CalendarDatePicker picker)
    {
        return picker.GetValue(InitialViewProperty);
    }

    public static void SetInitialView(CalendarDatePicker picker, CalendarMode value)
    {
        picker.SetValue(InitialViewProperty, value);
    }

    public static bool GetWheelNavigation(Calendar calendar)
    {
        return calendar.GetValue(WheelNavigationProperty);
    }

    public static void SetWheelNavigation(Calendar calendar, bool value)
    {
        calendar.SetValue(WheelNavigationProperty, value);
    }

    public static TimeSpan? GetTime(Control control)
    {
        return control.GetValue(TimeProperty);
    }

    public static void SetTime(Control control, TimeSpan? value)
    {
        control.SetValue(TimeProperty, value);
    }

    public static bool GetShowTime(Control control)
    {
        return control.GetValue(ShowTimeProperty);
    }

    private static void ApplyFormat(CalendarDatePicker picker)
    {
        var format = GetFormat(picker) ?? GetDisplayStyle(picker) switch
        {
            DateInputDisplayStyle.DateTime => "yyyy-MM-dd HH:mm",
            DateInputDisplayStyle.Month => "yyyy-MM",
            DateInputDisplayStyle.Year => "yyyy",
            _ => "yyyy-MM-dd"
        };
        picker.SetCurrentValue(CalendarDatePicker.SelectedDateFormatProperty, CalendarDatePickerFormat.Custom);
        picker.SetCurrentValue(CalendarDatePicker.CustomDateFormatStringProperty, format);
    }

    private static void Attach(CalendarDatePicker picker)
    {
        picker.CalendarOpened -= OnOpened;
        picker.CalendarOpened += OnOpened;
    }

    private static void OnOpened(object? sender, EventArgs args)
    {
        if (sender is CalendarDatePicker picker)
            Opened(picker,
                picker.GetVisualDescendants().OfType<Popup>().FirstOrDefault(p => p.Name == "PART_Popup")
                    ?.Child as Calendar);
    }

    internal static void Opened(CalendarDatePicker picker, Calendar? calendar)
    {
        if (calendar is null) return;
        calendar.SetCurrentValue(Calendar.DisplayModeProperty, picker.IsSet(InitialViewProperty)
            ? GetInitialView(picker)
            : GetDisplayStyle(picker) switch
            {
                DateInputDisplayStyle.Month => CalendarMode.Year,
                DateInputDisplayStyle.Year => CalendarMode.Decade,
                _ => CalendarMode.Month
            });
        calendar.RemoveHandler(InputElement.PointerPressedEvent, ChoosingDate);
        calendar.RemoveHandler(InputElement.PointerReleasedEvent, ChosenDate);
        calendar.RemoveHandler(InputElement.KeyDownEvent, ChoosingDateKey);
        calendar.AddHandler(InputElement.PointerPressedEvent, ChoosingDate, RoutingStrategies.Tunnel, true);
        calendar.AddHandler(InputElement.PointerReleasedEvent, ChosenDate, RoutingStrategies.Tunnel, true);
        calendar.AddHandler(InputElement.KeyDownEvent, ChoosingDateKey, RoutingStrategies.Tunnel, true);
    }

    private static CalendarDatePicker? Owner(object? sender)
    {
        return (sender as Calendar)?.TemplatedParent as CalendarDatePicker;
    }

    private static void ChoosingDate(object? sender, PointerPressedEventArgs args)
    {
        if (Owner(sender) is { } picker && args.Source is Control source)
            picker.SetValue(ChoosingDateProperty,
                source is CalendarDayButton || source.GetVisualAncestors().OfType<CalendarDayButton>().Any());
    }

    private static void ChosenDate(object? sender, PointerReleasedEventArgs args)
    {
        if (Owner(sender) is { } picker) Dispatcher.UIThread.Post(() => picker.SetValue(ChoosingDateProperty, false));
    }

    private static void ChoosingDateKey(object? sender, KeyEventArgs args)
    {
        if (Owner(sender) is { } picker && args.Key is Key.Left or Key.Right or Key.Up or Key.Down or Key.Home
                or Key.End or Key.Space or Key.Enter)
        {
            picker.SetValue(ChoosingDateProperty, true);
            Dispatcher.UIThread.Post(() => picker.SetValue(ChoosingDateProperty, false));
        }
    }

    private static void OnWheel(object? sender, PointerWheelEventArgs args)
    {
        if (sender is not Calendar { IsEffectivelyEnabled: true } calendar || args.Delta.Y == 0) return;
        // A nested time editor retains its own wheel interaction.
        if (args.Source is Control source &&
            source.GetVisualAncestors().Prepend(source).OfType<TimePicker>().Any()) return;
        var name = args.Delta.Y > 0 ? "PART_PreviousButton" : "PART_NextButton";
        var button = calendar.GetVisualDescendants().OfType<Button>().FirstOrDefault(control => control.Name == name);
        // Use native navigation so SelectedMonth/SelectedYear, ranges and visible cells
        // stay synchronized in Year and Decade views as well as Month view.
        if (button is { IsEffectivelyEnabled: true }) button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        args.Handled = true;
    }
}
