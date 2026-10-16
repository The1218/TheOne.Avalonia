using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class CalendarLayoutPreview
{
    public static void Verify(Action<bool, string> check, string? output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 420 }) Verify(theme, size, width, check, output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) VerifyPopup(theme, size, check);
    }
    private static Calendar Sample(CalendarMode mode = CalendarMode.Month) => new()
    {
        DisplayDate = new DateTime(2026, 10, 13), SelectedDate = new DateTime(2026, 10, 13), DisplayMode = mode,
        HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top
    };
    private static void Verify(ThemeVariant theme, ControlSize size, int width, Action<bool, string> check, string? output)
    {
        var calendar = Sample(); calendar.Width = Math.Min(400, width - 32);
        var window = Open(calendar, theme, size, width); var heights = new List<double>();
        var label = $"Calendar/layout/{theme.Key}/{size}/{width}";
        try
        {
            var item = Part<CalendarItem>(calendar, "PART_CalendarItem"); var header = Part<Button>(item, "PART_HeaderButton");
            var expectedBody = size switch { ControlSize.Small => 258, ControlSize.Large => 342, _ => 294 };
            var cellHeight = size switch { ControlSize.Small => 34, ControlSize.Large => 48, _ => 40 };
            foreach (var mode in new[] { CalendarMode.Month, CalendarMode.Year, CalendarMode.Decade })
            {
                calendar.DisplayMode = mode; Flush(); heights.Add(calendar.Bounds.Height);
                var grid = Part<Grid>(item, mode == CalendarMode.Month ? "PART_MonthView" : "PART_YearView");
                check(grid.Bounds.Height == expectedBody && calendar.Bounds.Width <= width - 32, label + $"/{mode}: body follows density and stays within the viewport ({grid.Bounds.Size}, calendar={calendar.Bounds.Size})");
                if (mode == CalendarMode.Month)
                {
                    var origin = grid.TranslatePoint(default, calendar)!.Value;
                    check(Math.Abs(origin.X - (calendar.Bounds.Width - origin.X - grid.Bounds.Width)) < 1
                        && grid.Children.OfType<CalendarDayButton>().All(button => button.Bounds.Bottom <= grid.Bounds.Height),
                        label + ": the natural date cells stay centered and inside the fixed body");
                }
                if (mode != CalendarMode.Month)
                {
                    var buttons = grid.Children.OfType<CalendarButton>().ToArray();
                    check(buttons.Length == 12 && buttons.All(b => b.Bounds.Height >= cellHeight && b.Bounds.Height <= cellHeight + 2 && b.Bounds.Width < grid.Bounds.Width / 4 - 4),
                        label + $"/{mode}: month/year backgrounds fit their labels instead of filling the grid slots ({string.Join(',', buttons.Select(b => b.Bounds.Size))})");
                    var selected = buttons.First(b => b.Classes.Contains(":selected"));
                    using var bitmap = Render(calendar); var point = selected.TranslatePoint(default, calendar)!.Value;
                    calendar.TryFindResource("One.Accent", theme, out var accent);
                    check(Pixel(bitmap, (int)point.X + 3, (int)(point.Y + selected.Bounds.Height / 2)) == ((ISolidColorBrush)accent!).Color,
                        label + $"/{mode}: selected accent pixels remain inside the compact native button");
                }
                Save(calendar, output, $"calendar-{theme.Key}-{size}-{width}-{mode}");
            }
            check(heights.Distinct().Count() == 1, label + $": switching date/month/year keeps one outer height ({string.Join(',', heights)})");
            calendar.DisplayMode = CalendarMode.Month; Flush(); Click(header);
            check(calendar.DisplayMode == CalendarMode.Year && calendar.Bounds.Height == heights[0], label + ": native header opens month selection without resizing");
            Click(header); check(calendar.DisplayMode == CalendarMode.Decade && calendar.Bounds.Height == heights[0], label + ": native header opens year selection without resizing");
            var yearGrid = Part<Grid>(item, "PART_YearView");
            Click(yearGrid.Children.OfType<CalendarButton>().First(b => b.Classes.Contains(":selected")));
            check(calendar.DisplayMode == CalendarMode.Year && calendar.Bounds.Height == heights[0], label + ": native year selection drills down without resizing");
            Click(yearGrid.Children.OfType<CalendarButton>().First(b => b.Classes.Contains(":selected")));
            check(calendar.DisplayMode == CalendarMode.Month && calendar.Bounds.Height == heights[0], label + ": native month selection drills down without resizing");
            var dayGrid = Part<Grid>(item, "PART_MonthView"); var day = dayGrid.Children.OfType<CalendarDayButton>().First(b => b.Content?.ToString() == "15");
            var selectedDate = new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month, 15);
            Click(day); check(calendar.SelectedDate?.Date == selectedDate.Date, label + ": native day pointer selection remains available");
            var before = calendar.DisplayDate; Click(Part<Button>(item, "PART_NextButton"));
            check(calendar.DisplayDate.Month != before.Month && calendar.Bounds.Height == heights[0], label + ": month paging preserves the frame height");
            dayGrid.Children.OfType<CalendarDayButton>().First(b => b.Content?.ToString() == "15").Focus();
            Press(window, Key.Right);
            check(calendar.Bounds.Height == heights[0], label + ": keyboard navigation retains the fixed body");
            CalendarOptions.SetDisplayStyle(calendar, DateInputDisplayStyle.DateTime); Flush(); var withTime = calendar.Bounds.Height;
            calendar.DisplayMode = CalendarMode.Year; Flush();
            check(calendar.Bounds.Height == withTime && withTime > heights[0], label + ": an optional time editor adds a consistent footer in every mode");
        }
        finally { window.Close(); Flush(); }
        foreach (var mode in new[] { CalendarMode.Year, CalendarMode.Decade })
        {
            var initial = Sample(mode); var initialWindow = Open(initial, theme, size, width);
            try { check(initial.Bounds.Height == heights[0], label + $"/{mode}: opening directly in this mode has the same natural height"); }
            finally { initialWindow.Close(); Flush(); }
        }
    }
    private static void VerifyPopup(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var picker = new CalendarDatePicker { SelectedDate = new DateTime(2026, 10, 13), HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
        var window = Open(picker, theme, size, 760);
        try
        {
            picker.IsDropDownOpen = true; Flush(); var popup = Part<Popup>(picker, "PART_Popup"); var calendar = (Calendar)popup.Child!;
            var original = calendar.Bounds.Height; var item = Part<CalendarItem>(calendar, "PART_CalendarItem");
            Click(Part<Button>(item, "PART_HeaderButton")); Click(Part<Button>(item, "PART_HeaderButton"));
            check(calendar.DisplayMode == CalendarMode.Decade && calendar.Bounds.Height == original, $"CalendarDatePicker/{theme.Key}/{size}: its native popup inherits stable calendar modes");
            Part<Grid>(item, "PART_YearView").Children.OfType<CalendarButton>().First(button => button.IsEffectivelyEnabled).Focus(NavigationMethod.Tab); Flush();
            Press(TopLevel.GetTopLevel(calendar)!, Key.Escape);
            check(!picker.IsDropDownOpen, $"CalendarDatePicker/{theme.Key}/{size}: native Escape still dismisses the popup");
        }
        finally { window.Close(); Flush(); }
    }
    private static Window Open(Control content, ThemeVariant theme, ControlSize size, int width)
    {
        var host = new Border { Padding = new Thickness(16), Child = content };
        host.Bind(Border.BackgroundProperty, new global::Avalonia.Markup.Xaml.MarkupExtensions.DynamicResourceExtension("One.Surface"));
        var window = new Window { Width = width, Height = 650, Content = host, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false); ControlSizing.SetSize(window, size); window.Show(); Flush(); return window;
    }
    private static T Part<T>(TemplatedControl owner, string name) where T : Control => owner.GetVisualDescendants().OfType<T>().First(c => c.Name == name && ReferenceEquals(c.TemplatedParent, owner));
    private static void Click(Control control) { var root = TopLevel.GetTopLevel(control)!; var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), root)!.Value; root.MouseMove(point); root.MouseDown(point, MouseButton.Left); root.MouseUp(point, MouseButton.Left); Flush(); }
    private static void Press(TopLevel root, Key key) { root.KeyPress(key, RawInputModifiers.None, PhysicalKey.None, null); root.KeyRelease(key, RawInputModifiers.None, PhysicalKey.None, null); Flush(); }
    private static RenderTargetBitmap Render(Control control) { var bitmap = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(control.Bounds.Width), (int)Math.Ceiling(control.Bounds.Height)), new Vector(96, 96)); bitmap.Render(control); return bitmap; }
    private static void Save(Control control, string? output, string name) { if (output is null) return; Directory.CreateDirectory(output); using var bitmap = Render(control); bitmap.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default); }
    private static Color Pixel(Bitmap bitmap, int x, int y)
    {
        var pixels = new byte[bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4]; var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), handle.AddrOfPinnedObject(), pixels.Length, bitmap.PixelSize.Width * 4); }
        finally { handle.Free(); }
        var offset = (y * bitmap.PixelSize.Width + x) * 4;
        return bitmap.Format == PixelFormat.Bgra8888 ? Color.FromArgb(pixels[offset + 3], pixels[offset + 2], pixels[offset + 1], pixels[offset]) : Color.FromArgb(pixels[offset + 3], pixels[offset], pixels[offset + 1], pixels[offset + 2]);
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
