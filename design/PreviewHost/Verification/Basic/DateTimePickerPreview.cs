using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class DateTimePickerPreview
{
    internal static T Part<T>(Control control, string name) where T : Control => control.GetVisualDescendants().OfType<T>().Single(part => part.Name == name);
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private static void Click(Control control)
    {
        var top = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), top)!.Value;
        top.MouseMove(point); Flush(); top.MouseDown(point, MouseButton.Left); Flush(); top.MouseUp(point, MouseButton.Left); Flush();
    }
    private static (Window Window, Control Picker) Open(bool date, bool twelve, ThemeVariant theme, ControlSize size)
    {
        Control picker = date
            ? new DatePicker { SelectedDate = new DateTimeOffset(1500, 6, 15, 0, 0, 0, TimeSpan.Zero), MinYear = new DateTimeOffset(1000, 1, 1, 0, 0, 0, TimeSpan.Zero), MaxYear = new DateTimeOffset(1999, 12, 31, 0, 0, 0, TimeSpan.Zero) }
            : new TimePicker { SelectedTime = new TimeSpan(14, 35, 20), UseSeconds = !twelve, ClockIdentifier = twelve ? "12HourClock" : "24HourClock" };
        var window = new Window { Width = 540, Height = 600, RequestedThemeVariant = theme, Content = new StackPanel { Margin = new Thickness(28), Spacing = 24, Children = { new TextBlock { Text = date ? "DatePicker · 日期选择" : $"TimePicker · {(twelve ? "12" : "24")} 小时制", FontSize = 22 }, picker } } };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush();
        return (window, picker);
    }
    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var kind in new[] { "DatePicker", "TimePicker24", "TimePicker24NoSeconds", "TimePicker12" })
        {
            var (window, picker) = Open(kind == "DatePicker", kind == "TimePicker12", theme, size);
            if (kind == "TimePicker24NoSeconds") { ((TimePicker)picker).UseSeconds = false; Flush(); }
            var label = $"{kind}/{theme.Key}/{size}";
            CheckDisplayAlignment(picker, label + "/selected", check);
            picker.IsEnabled = false; Flush(); CheckDisplayAlignment(picker, label + "/disabled", check);
            picker.IsEnabled = true;
            if (picker is DatePicker hostDatePicker)
            {
                var date = hostDatePicker.SelectedDate;
                hostDatePicker.SelectedDate = null; Flush(); CheckDisplayAlignment(picker, label + "/placeholder", check);
                hostDatePicker.SelectedDate = date;
                hostDatePicker.DayVisible = false; hostDatePicker.MonthFormat = "MMMM"; Flush(); CheckDisplayAlignment(picker, label + "/month-year", check);
                hostDatePicker.DayVisible = true; hostDatePicker.ClearValue(DatePicker.MonthFormatProperty);
            }
            else if (picker is TimePicker hostTimePicker)
            {
                var time = hostTimePicker.SelectedTime;
                hostTimePicker.SelectedTime = null; Flush(); CheckDisplayAlignment(picker, label + "/placeholder", check);
                hostTimePicker.SelectedTime = time;
            }
            picker.Height = 68; Flush(); CheckDisplayAlignment(picker, label + "/custom-height", check);
            picker.ClearValue(Control.HeightProperty);
            ControlSizing.SetSize(window, size == ControlSize.Small ? ControlSize.Large : ControlSize.Small); Flush();
            CheckDisplayAlignment(picker, label + "/changed-density", check);
            ControlSizing.SetSize(window, size); Flush();
            Click(Part<Button>(picker, "PART_FlyoutButton")); var popup = Part<Popup>(picker, "PART_Popup");
            var presenter = (PickerPresenterBase)popup.Child!;
            check(popup.IsOpen && presenter.Bounds.Height > 0 && ControlSizing.GetSize(presenter) == size, $"{label}: native popup inherits density");
            var rowHeight = size switch { ControlSize.Small => 32, ControlSize.Large => 48, _ => 40 };
            var highlight = Part<Border>(presenter, "HighlightRect");
            check(highlight.Bounds.Height == rowHeight && ColorOf(highlight.Background) == Resource(presenter, "One.Accent"), $"{label}: selected band uses the semantic accent and matching row height");
            var panels = presenter.GetVisualDescendants().OfType<DateTimePickerPanel>().Where(panel => panel.IsEffectivelyVisible).ToArray();
            foreach (var panel in panels)
            {
                check(panel.Bounds.Height == rowHeight * 5, $"{label}/{panel.PanelType}: resting viewport shows five complete rows without cutting edge text");
                check(panel.ItemHeight == rowHeight && panel.Children.Count > 0 && panel.Children.Count < 20, $"{label}/{panel.PanelType}: native viewport realizes only visible rows");
                CheckSelection(panel);
            }
            foreach (var panel in panels)
            {
                var columnHost = (Control)panel.GetVisualAncestors().First(ancestor => ancestor is Grid { Name: "PART_MonthHost" or "PART_DayHost" or "PART_YearHost" or "PART_HourHost" or "PART_MinuteHost" or "PART_SecondHost" or "PART_PeriodHost" });
                var scroll = panel.GetVisualAncestors().OfType<ScrollViewer>().First();
                var up = columnHost.GetVisualDescendants().OfType<RepeatButton>().Single(button => button.Name!.EndsWith("UpButton"));
                var downButton = columnHost.GetVisualDescendants().OfType<RepeatButton>().Single(button => button.Name!.EndsWith("DownButton"));
                var viewport = scroll.TranslatePoint(new Point(), columnHost)!.Value;
                check(viewport.Y >= up.Bounds.Bottom && viewport.Y + scroll.Bounds.Height <= downButton.Bounds.Top, $"{label}/{panel.PanelType}: arrow rows never overlap or cover the scrolling viewport");
                foreach (var fraction in new[] { 0.3, -0.7 })
                {
                    panel.Offset = panel.Offset.WithY(panel.Offset.Y + panel.ItemHeight * fraction); Flush();
                    var expected = Math.Round(panel.Offset.Y / panel.ItemHeight, MidpointRounding.AwayFromZero) * panel.ItemHeight;
                    if (!panel.ShouldLoop) expected = Math.Clamp(expected, 0, (Math.Ceiling((panel.MaximumValue - panel.MinimumValue + 1d) / panel.Increment) - 1) * panel.ItemHeight);
                    CheckClips(panel); Advance(220);
                    check(Math.Abs(panel.Offset.Y - expected) < 0.01, $"{label}/{panel.PanelType}: fractional offset snaps to nearest row after input stops");
                    CheckSelection(panel); CheckClips(panel);
                }
            }
            if (theme == ThemeVariant.Light && size == ControlSize.Normal && kind is "DatePicker" or "TimePicker12")
            {
                var gesturePanel = kind == "DatePicker" ? panels.First() : panels.Single(panel => panel.PanelType == DateTimePickerPanelType.TimePeriod);
                var scroll = gesturePanel.GetVisualAncestors().OfType<ScrollViewer>().First();
                var source = Part<ScrollContentPresenter>(scroll, "PART_ContentPresenter");
                if (!gesturePanel.ShouldLoop) gesturePanel.SelectedValue = gesturePanel.MinimumValue;
                var id = ScrollGestureEventArgs.GetNextFreeId(); source.RaiseEvent(new ScrollGestureEventArgs(id, Vector.Zero));
                gesturePanel.Offset = gesturePanel.Offset.WithY(gesturePanel.Offset.Y + gesturePanel.ItemHeight * (gesturePanel.ShouldLoop ? 0.65 : -0.65)); Flush();
                var during = gesturePanel.Offset.Y; Advance(220);
                check(Math.Abs(gesturePanel.Offset.Y - during) < 0.01, $"{label}: idle timeout does not interrupt an active touch gesture");
                source.RaiseEvent(new ScrollGestureEndedEventArgs(id)); Flush(); CheckSelection(gesturePanel); CheckClips(gesturePanel);
                check(Math.Abs(gesturePanel.Offset.Y % gesturePanel.ItemHeight) < 0.01 && gesturePanel.Offset.Y >= 0, $"{label}: gesture end aligns immediately and clamps short-list boundaries before native completion");
            }
            var selector = panels.First(); var original = selector.SelectedValue;
            selector.Focus(); var top = TopLevel.GetTopLevel(selector)!;
            top.KeyPress(Key.Down, RawInputModifiers.None, PhysicalKey.ArrowDown, null); top.KeyRelease(Key.Down, RawInputModifiers.None, PhysicalKey.ArrowDown, null); Flush();
            check(selector.SelectedValue != original, $"{label}: keyboard changes native selection"); CheckSelection(selector);
            original = selector.SelectedValue;
            var point = selector.TranslatePoint(new Point(selector.Bounds.Width / 2, selector.Bounds.Height / 2), top)!.Value;
            top.MouseWheel(point, new Vector(0, -1)); Flush(); check(selector.SelectedValue != original, $"{label}: mouse wheel changes native selection"); CheckSelection(selector);
            // Trackpad-style deltas are fractional, unlike the previous full-notch test.
            foreach (var delta in new[] { -0.18, -0.27, -0.31 })
            {
                top.MouseWheel(point, new Vector(0, delta)); Flush(); Advance(25);
            }
            var wheelExpected = Math.Round(selector.Offset.Y / selector.ItemHeight, MidpointRounding.AwayFromZero) * selector.ItemHeight;
            check(Math.Abs(selector.Offset.Y % selector.ItemHeight) > 0.01, $"{label}: real small wheel deltas reproduce a fractional stopped position");
            CheckClips(selector); Advance(220);
            check(Math.Abs(selector.Offset.Y - wheelExpected) < 0.01, $"{label}: continuous small wheel input settles to a whole selected row"); CheckSelection(selector);
            var host = (Control)selector.GetVisualParent()!.GetVisualAncestors().First(ancestor => ancestor is Panel { Name: "PART_MonthHost" or "PART_DayHost" or "PART_YearHost" or "PART_HourHost" or "PART_MinuteHost" or "PART_SecondHost" or "PART_PeriodHost" });
            top.MouseMove(host.TranslatePoint(new Point(20, 15), top)!.Value); Flush();
            var down = host.GetVisualDescendants().OfType<RepeatButton>().Single(button => button.Name!.EndsWith("DownButton"));
            original = selector.SelectedValue; check(down.IsVisible, $"{label}: hover exposes themed native navigation buttons"); Click(down);
            check(selector.SelectedValue != original, $"{label}: navigation button changes selection"); CheckSelection(selector);
            if (kind == "DatePicker" && theme == ThemeVariant.Light && size == ControlSize.Normal)
            {
                down.Delay = 40; down.Interval = 20; var changes = 0; selector.SelectionChanged += (_, _) => changes++;
                var buttonPoint = down.TranslatePoint(new Point(down.Bounds.Width / 2, down.Bounds.Height / 2), top)!.Value;
                top.MouseMove(buttonPoint); Flush(); top.MouseDown(buttonPoint, MouseButton.Left); Advance(120);
                check(changes >= 2 && down.IsVisible, $"{label}: held navigation repeats without losing its pressed hit target");
                top.MouseUp(buttonPoint, MouseButton.Left); Flush(); var stopped = changes; Advance(70);
                check(changes == stopped, $"{label}: releasing navigation stops repetition");
            }
            if (picker is DatePicker datePicker)
            {
                var year = panels.Single(panel => panel.PanelType == DateTimePickerPanelType.Year); year.SelectedValue = 1985; Flush(); CheckSelection(year);
                check(year.Children.Count < 20 && year.Children.OfType<ListBoxItem>().Any(item => Equals(item.Tag, 1985) && item.IsSelected), $"{label}: far selection recycles a thousand-year range");
                year.Offset = year.Offset.WithY(year.Offset.Y + year.ItemHeight * 0.7); Flush();
                Click(Part<Button>(presenter, "PART_AcceptButton")); check(datePicker.SelectedDate?.Year == 1986 && !popup.IsOpen, $"{label}: accept commits the native selected date");
                var selected = datePicker.SelectedDate; Click(Part<Button>(picker, "PART_FlyoutButton")); Part<DateTimePickerPanel>((Control)popup.Child!, "PART_YearSelector").SelectedValue = 1800; Flush(); Click(Part<Button>((Control)popup.Child!, "PART_DismissButton"));
                check(datePicker.SelectedDate == selected && !popup.IsOpen, $"{label}: dismiss preserves the previous date");
            }
            else
            {
                var timePicker = (TimePicker)picker;
                var hour = Part<DateTimePickerPanel>(presenter, "PART_HourSelector"); hour.SelectedValue = kind == "TimePicker12" ? 3 : 16;
                var minute = Part<DateTimePickerPanel>(presenter, "PART_MinuteSelector"); minute.SelectedValue = 42; Flush();
                minute.Offset = minute.Offset.WithY(minute.Offset.Y + minute.ItemHeight * 0.7); Flush();
                Click(Part<Button>(presenter, "PART_AcceptButton")); check(timePicker.SelectedTime?.Minutes == 43 && !popup.IsOpen, $"{label}: accept commits selected time");
                var selected = timePicker.SelectedTime; Click(Part<Button>(picker, "PART_FlyoutButton")); Part<DateTimePickerPanel>((Control)popup.Child!, "PART_MinuteSelector").SelectedValue = 12; Flush(); Click(Part<Button>((Control)popup.Child!, "PART_DismissButton"));
                check(timePicker.SelectedTime == selected && !popup.IsOpen, $"{label}: dismiss preserves previous time");
            }
            Click(Part<Button>(picker, "PART_FlyoutButton"));
            var reopenedPanel = popup.Child!.GetVisualDescendants().OfType<DateTimePickerPanel>().First(panel => panel.IsEffectivelyVisible);
            reopenedPanel.Offset = reopenedPanel.Offset.WithY(reopenedPanel.Offset.Y + reopenedPanel.ItemHeight * 0.35); Flush();
            popup.IsOpen = false; Flush(); var closedOffset = reopenedPanel.Offset; Advance(180);
            check(reopenedPanel.Offset == closedOffset, $"{label}: closing a fractional popup cancels its pending idle callback");
            Click(Part<Button>(picker, "PART_FlyoutButton")); Flush();
            check(Math.Abs(reopenedPanel.Offset.Y % reopenedPanel.ItemHeight) < 0.01, $"{label}: reopening reattaches behavior and restores committed selection alignment");
            popup.IsOpen = false; window.Close(); Flush();
            void CheckClips(DateTimePickerPanel panel)
            {
                var bandTop = (panel.Bounds.Height - panel.ItemHeight) / 2;
                foreach (var item in panel.Children.OfType<ListBoxItem>())
                {
                    var start = Math.Clamp(bandTop - item.Bounds.Y, 0, item.Bounds.Height);
                    var end = Math.Clamp(bandTop + panel.ItemHeight - item.Bounds.Y, 0, item.Bounds.Height);
                    var clip = (RectangleGeometry)Part<ContentPresenter>(item, "PART_ContentPresenter").Clip!;
                    check(clip.Rect == new Rect(0, start, item.Bounds.Width, Math.Max(0, end - start)) &&
                          ColorOf(Part<ContentPresenter>(item, "PART_NormalPresenter").Foreground) == Resource(presenter, "One.Muted"), $"{label}/{panel.PanelType}: moving recycled row remains readable inside and outside accent band");
                }
            }
            void CheckSelection(DateTimePickerPanel panel)
            {
                var item = panel.Children.OfType<ListBoxItem>().Single(child => child.IsSelected);
                var content = Part<ContentPresenter>(item, "PART_ContentPresenter");
                check(ColorOf(content.Foreground) == Resource(presenter, "One.OnAccent") && item.FontWeight == FontWeight.SemiBold, $"{label}/{panel.PanelType}: selected text retains accent contrast without pointer or focus");
                var center = item.TranslatePoint(new Point(0, item.Bounds.Height / 2), presenter)!.Value.Y;
                var bandCenter = highlight.TranslatePoint(new Point(0, highlight.Bounds.Height / 2), presenter)!.Value.Y;
                check(Math.Abs(center - bandCenter) < 1, $"{label}/{panel.PanelType}: native selected item remains centered on the highlight (selected={panel.SelectedValue}, offset={panel.Offset.Y}, item={item.Bounds}, panel={panel.Bounds}, centers={center}/{bandCenter})");
            }
        }
    }
    private static void CheckDisplayAlignment(Control picker, string label, Action<bool, string> check)
    {
        var button = Part<Button>(picker, "PART_FlyoutButton");
        foreach (var text in button.GetVisualDescendants().OfType<TextBlock>().Where(text => text.IsEffectivelyVisible && text.Name is { } name && name.EndsWith("TextBlock")))
        {
            var origin = text.TranslatePoint(default, button)!.Value;
            var centerY = origin.Y + text.Padding.Top + text.TextLayout.Height / 2;
            check(Math.Abs(centerY - button.Bounds.Height / 2) <= 1,
                $"{label}/{text.Name}: rendered text is vertically centered ({centerY:0.##}/{button.Bounds.Height / 2:0.##})");
            var centerX = origin.X + text.Padding.Left + text.TextLayout.WidthIncludingTrailingWhitespace / 2;
            var parent = (Control)text.GetVisualParent()!;
            var parentOrigin = parent.TranslatePoint(default, button)!.Value;
            var expectedX = parentOrigin.X + parent.Bounds.Width / 2;
            if (parent is Grid grid)
            {
                var column = Grid.GetColumn(text);
                expectedX = parentOrigin.X + grid.ColumnDefinitions.Take(column).Sum(definition => definition.ActualWidth) + grid.ColumnDefinitions[column].ActualWidth / 2;
            }
            check(Math.Abs(centerX - expectedX) <= 1,
                $"{label}/{text.Name}: rendered text is horizontally centered in its native segment ({centerX:0.##}/{expectedX:0.##})");
            check(origin.Y + text.Padding.Top >= 0 && origin.Y + text.Padding.Top + text.TextLayout.Height <= button.Bounds.Height + 1,
                $"{label}/{text.Name}: rendered text fits inside the host without clipping");
        }
    }

    private static void Advance(int milliseconds)
    {
        var frame = new DispatcherFrame(); var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; }; timer.Start(); Dispatcher.UIThread.PushFrame(frame); Flush();
    }
    private static Color ColorOf(IBrush? brush) => ((ISolidColorBrush)brush!).Color;
    private static Color Resource(Control control, string key) { control.TryFindResource(key, control.ActualThemeVariant, out var brush); return ColorOf((IBrush)brush!); }
    public static void Render(string output, IReadOnlyCollection<string> controls)
    {
        Directory.CreateDirectory(output);
        foreach (var kind in controls)
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var twelve in kind == "TimePicker" ? new[] { false, true } : new[] { false })
        {
            var (window, picker) = Open(kind == "DatePicker", twelve, theme, size);
            var stem = $"{kind.ToLowerInvariant()}-{theme.Key}-{size}{(kind == "TimePicker" ? twelve ? "-12h" : "-24h" : "")}";
            Save(picker, stem + "-host");
            Click(Part<Button>(picker, "PART_FlyoutButton")); var popup = Part<Popup>(picker, "PART_Popup");
            var presenter = (Control)popup.Child!;
            Save(presenter, stem);
            if (size == ControlSize.Normal)
            {
                var panel = presenter.GetVisualDescendants().OfType<DateTimePickerPanel>().Single(control => control.Name == (kind == "DatePicker" ? "PART_MonthSelector" : "PART_MinuteSelector"));
                panel.Offset = panel.Offset.WithY(panel.Offset.Y + panel.ItemHeight * 0.65); Flush(); Save(presenter, stem + "-fractional");
                Advance(220); Save(presenter, stem + "-snapped");
            }
            void Save(Control target, string name)
            {
                using var bitmap = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(target.Bounds.Width), (int)Math.Ceiling(target.Bounds.Height)), new Vector(96, 96));
                bitmap.Render(target);
                var path = Path.Combine(output, name + ".png"); bitmap.Save(path, PngBitmapEncoderOptions.Default); Console.WriteLine($"Rendered {path}");
            }
            popup.IsOpen = false; window.Close(); Flush();
        }
    }
}
