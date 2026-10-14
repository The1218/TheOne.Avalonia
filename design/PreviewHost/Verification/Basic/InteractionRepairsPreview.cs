using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System.Runtime.InteropServices;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class InteractionRepairsPreview
{
    public static void Render(LegacyPreviewWindow window, LegacyControlGallery gallery, string output, bool dark)
    {
        var theme = dark ? "dark" : "light";
        window.NavigateToCategory(8); ControlPreview.Flush();
        var picker = gallery.FindControl<CalendarDatePicker>("DateTimeSample")!;
        picker.BringIntoView(); ControlPreview.Flush(); picker.IsDropDownOpen = true; ControlPreview.Flush();
        var calendar = (Calendar)Part<Popup>(picker, "PART_Popup").Child!;
        EnhancementsPreview.Save(calendar, Path.Combine(output, $"calendar-time-{theme}.png"));
        calendar.DisplayMode = CalendarMode.Year; ControlPreview.Flush();
        EnhancementsPreview.Save(calendar, Path.Combine(output, $"calendar-months-{theme}.png"));
        calendar.DisplayMode = CalendarMode.Decade; ControlPreview.Flush();
        EnhancementsPreview.Save(calendar, Path.Combine(output, $"calendar-years-{theme}.png"));
        picker.IsDropDownOpen = false;
        window.NavigateToCategory(9); ControlPreview.Flush();
        EnhancementsPreview.Advance(260);
        EnhancementsPreview.Save(window, Path.Combine(output, $"virtualization-{theme}.png"));
    }
    public static void Verify()
    {
        foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) VerifyEdges(variant, size);
        VerifyCalendar();
        VirtualizationPreview.Verify();
    }
    private static void VerifyEdges(ThemeVariant variant, ControlSize size)
    {
        var number = new NumericUpDown { Value = 3, Width = 260 };
        var text = new TextBox { Text = "Clear me", Classes = { "clearButton" }, Width = 260 };
        var combo = new ComboBox { Width = 260, ItemsSource = new[] { "One", "Two" }, SelectedIndex = 0 };
        var date = new CalendarDatePicker { Width = 260, SelectedDate = new DateTime(2026, 10, 12) };
        var panel = new StackPanel { Spacing = 16, Margin = new Thickness(20) };
        foreach (var control in new Control[] { number, text, combo, date }) panel.Children.Add(control);
        var window = new Window { Width = 320, Height = 300, Content = panel, RequestedThemeVariant = variant, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false); ControlSizing.SetSize(window, size); window.Show(); ControlPreview.Flush();
        foreach (var location in new[] { Location.Left, Location.Right })
        {
            number.ButtonSpinnerLocation = location; ControlPreview.Flush();
            var spinner = Part<ButtonSpinner>(number, "PART_Spinner");
            var button = Part<RepeatButton>(spinner, location == Location.Left ? "PART_IncreaseButton" : "PART_DecreaseButton");
            Hover(window, button);
            VerifyOutline(window, spinner, Part<Border>(spinner, "PART_Border"), $"{variant.Key}/{size}/Numeric/{location}");
        }
        text.Focus(); ControlPreview.Flush();
        Hover(window, text.GetVisualDescendants().OfType<Button>().First(button => button.IsVisible && button.IsEffectivelyVisible));
        VerifyOutline(window, text, Part<Border>(text, "PART_BorderElement"), $"{variant.Key}/{size}/TextBox");
        Hover(window, combo);
        VerifyOutline(window, combo, Part<Border>(combo, "Background"), $"{variant.Key}/{size}/ComboBox");
        Hover(window, date.GetVisualDescendants().OfType<Button>().Single(button => button.Name == "PART_Button"));
        VerifyOutline(window, date, Part<Border>(date, "Background"), $"{variant.Key}/{size}/CalendarDatePicker");
        window.Close();
    }
    private static void VerifyOutline(Window window, Control owner, Border frame, string label)
    {
        // Idle Nova editors have a transparent outline. Exercise a visible stroke as a
        // consumer template would, so this pixel regression still detects edge-button overpainting.
        if (frame.BorderBrush is ISolidColorBrush { Color.A: 0 }) { frame.BorderBrush = Brushes.Red; ControlPreview.Flush(); }
        var outline = owner.GetVisualDescendants().OfType<Border>().Single(control => control.Name == "InputOutline" && control.TemplatedParent == owner);
        Check(!outline.IsHitTestVisible && outline.BorderBrush == frame.BorderBrush && outline.BorderThickness == frame.BorderThickness, $"{label}: outer stroke is redrawn above edge-button backgrounds");
        using var bitmap = new RenderTargetBitmap(new PixelSize(320, 300), new Vector(96, 96)); bitmap.Render(window);
        var point = outline.TranslatePoint(default, window)!.Value;
        var expected = ((ISolidColorBrush)outline.BorderBrush!).Color;
        Check(Pixel(bitmap, (int)point.X, (int)(point.Y + outline.Bounds.Height / 2)) == expected && Pixel(bitmap, (int)(point.X + outline.Bounds.Width - 1), (int)(point.Y + outline.Bounds.Height / 2)) == expected, $"{label}: hovered edge buttons cannot paint over either outer side");
    }
    private static void VerifyCalendar()
    {
        var picker = new CalendarDatePicker { Width = 280, SelectedDate = new DateTime(2026, 10, 12), DisplayDate = new DateTime(2026, 10, 12) };
        var window = new Window { Width = 400, Height = 500, Content = new StackPanel { Margin = new Thickness(20), Children = { picker } }, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        picker.IsDropDownOpen = true; ControlPreview.Flush();
        var calendar = (Calendar)Part<Popup>(picker, "PART_Popup").Child!; var host = TopLevel.GetTopLevel(calendar)!;
        var item = Part<CalendarItem>(calendar, "PART_CalendarItem");
        var previous = Part<Button>(item, "PART_PreviousButton"); var next = Part<Button>(item, "PART_NextButton"); var header = Part<Button>(item, "PART_HeaderButton");
        var oldHeader = header.Content;
        Click(host, next);
        Check(calendar.DisplayDate.Month == 11 && !Equals(oldHeader, header.Content) && picker.IsDropDownOpen, "Real calendar next-button click updates the visible month and keeps the popup open");
        Click(host, previous); Check(calendar.DisplayDate.Month == 10, "Real previous-button click returns to the visible previous month");
        Click(host, header); Check(calendar.DisplayMode == CalendarMode.Year && Part<Grid>(item, "PART_YearView").IsVisible, "Real year/month header click shows the twelve-month grid");
        Click(host, header); Check(calendar.DisplayMode == CalendarMode.Decade, "Second header click opens year selection");
        var year = item.GetVisualDescendants().OfType<CalendarButton>().First(button => button.IsEffectivelyVisible && button.IsEnabled);
        Click(host, year); Check(calendar.DisplayMode == CalendarMode.Year, "Clicking a year returns to month selection");
        var month = item.GetVisualDescendants().OfType<CalendarButton>().First(button => button.IsEffectivelyVisible && button.IsEnabled);
        Click(host, month); Check(calendar.DisplayMode == CalendarMode.Month && Part<Grid>(item, "PART_MonthView").IsVisible, "Clicking a month returns to the date grid");
        calendar.DisplayDate = new DateTime(2026, 10, 12); ControlPreview.Flush();
        Wheel(host, calendar, -1); Check(calendar.DisplayDate.Month == 11 && picker.SelectedDate == new DateTime(2026, 10, 12), "Calendar wheel changes the displayed month without changing the selected date");
        calendar.DisplayMode = CalendarMode.Year; ControlPreview.Flush(); oldHeader = header.Content; Wheel(host, calendar, -1); Check(!Equals(header.Content, oldHeader), "Wheel in month-selection mode changes the displayed year");
        calendar.DisplayMode = CalendarMode.Decade; ControlPreview.Flush(); oldHeader = header.Content; Wheel(host, calendar, -1); Check(!Equals(header.Content, oldHeader), "Wheel in year-selection mode changes the displayed decade");
        CalendarOptions.SetWheelNavigation(calendar, false); var before = calendar.DisplayDate; Wheel(host, calendar, -1); Check(calendar.DisplayDate == before, "Calendar wheel navigation can be disabled explicitly");
        picker.IsDropDownOpen = false;
        CalendarOptions.SetDisplayStyle(picker, DateInputDisplayStyle.DateTime); CalendarOptions.SetFormat(picker, "yyyy/MM/dd HH:mm");
        picker.SelectedDate = new DateTime(2026, 10, 12, 16, 30, 0); picker.DisplayDate = picker.SelectedDate.Value; ControlPreview.Flush();
        Check(Part<TextBox>(picker, "PART_TextBox").Text == "2026/10/12 16:30", "Attached format and date-time display style format the native editor");
        picker.IsDropDownOpen = true; ControlPreview.Flush();
        var time = Part<TimePicker>(calendar, "PART_TimeEditor");
        Check(time.IsVisible && time.SelectedTime == new TimeSpan(16, 30, 0), "Date-time style exposes an embedded native time editor");
        Click(host, Part<Button>(time, "PART_FlyoutButton"));
        var timePopup = Part<Popup>(time, "PART_Popup");
        Check(timePopup.IsOpen && picker.IsDropDownOpen, "Embedded time picker opens its nested popup without dismissing the calendar");
        var timePresenter = (TimePickerPresenter)timePopup.Child!;
        Click(TopLevel.GetTopLevel(timePresenter)!, Part<Button>(timePresenter, "PART_AcceptButton"));
        Check(!timePopup.IsOpen && picker.IsDropDownOpen, "Time popup acceptance returns to the still-open calendar");
        time.SelectedTime = new TimeSpan(18, 45, 0); ControlPreview.Flush();
        Check(picker.SelectedDate?.TimeOfDay == new TimeSpan(18, 45, 0), "Time editor commits hours and minutes to SelectedDate");
        var day = item.GetVisualDescendants().OfType<CalendarDayButton>().First(button => button.IsVisible && button.IsEnabled && button.DataContext is DateTime value && value.Month == 10 && value.Day == 13);
        Click(host, day);
        Check(picker.SelectedDate?.Day == 13 && picker.SelectedDate?.TimeOfDay == new TimeSpan(18, 45, 0) && !picker.IsDropDownOpen, "Picking a calendar day preserves the chosen time and dismisses the popup");
        var editor = Part<TextBox>(picker, "PART_TextBox");
        CalendarInput.SetOpenOnClick(picker, false);
        editor.Focus(); editor.SelectAll(); window.KeyTextInput("2026/10/14 09:25");
        window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null); window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null); ControlPreview.Flush();
        Check(picker.SelectedDate == new DateTime(2026, 10, 14, 9, 25, 0) && CalendarOptions.GetTime(picker) == new TimeSpan(9, 25, 0), "Manual custom-format date-time input synchronizes SelectedDate and the time editor");
        CalendarOptions.SetInitialView(picker, CalendarMode.Decade); picker.IsDropDownOpen = true; ControlPreview.Flush();
        Check(calendar.DisplayMode == CalendarMode.Decade, "Explicit initial calendar view works with automatic editor popup disabled");
        picker.IsDropDownOpen = false; picker.ClearValue(CalendarOptions.InitialViewProperty);
        CalendarOptions.SetDisplayStyle(picker, DateInputDisplayStyle.Month); picker.IsDropDownOpen = true; ControlPreview.Flush();
        Check(calendar.DisplayMode == CalendarMode.Year && !time.IsVisible, "Month display style opens directly in month-selection mode");
        picker.IsDropDownOpen = false; window.Close();
    }
    internal static void Click(TopLevel host, Control control) { var point = Centre(control, host); host.MouseMove(point); host.MouseDown(point, MouseButton.Left); host.MouseUp(point, MouseButton.Left); ControlPreview.Flush(); }
    internal static void Hover(TopLevel host, Control control) { host.MouseMove(Centre(control, host)); ControlPreview.Flush(); }
    internal static void Wheel(TopLevel host, Control control, double delta) { host.MouseWheel(Centre(control, host), new Vector(0, delta)); ControlPreview.Flush(); }
    private static Point Centre(Control control, TopLevel host) => control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
    internal static T Part<T>(Control owner, string name) where T : Control => owner.GetVisualDescendants().OfType<T>().Single(control => control.Name == name && control.TemplatedParent == owner);
    private static Color Pixel(RenderTargetBitmap bitmap, int x, int y)
    {
        var bytes = new byte[4]; var pinned = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect(x, y, 1, 1), pinned.AddrOfPinnedObject(), 4, 4); } finally { pinned.Free(); }
        return bitmap.Format == PixelFormat.Rgba8888 ? Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2]) : Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }
    internal static void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException("Failed: " + message); Console.WriteLine("PASS: " + message); }
}
