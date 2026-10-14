using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class GanttPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var density in Enum.GetValues<ControlSize>()) VerifyRendering(theme, density, check, output);
        VerifyHierarchy(check); VerifyChanges(check); VerifyLargeData(check, output);
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private static Window Open(Control content, ThemeVariant? theme = null, ControlSize density = ControlSize.Normal, double width = 1360, double height = 920)
    {
        var window = new Window { Width = width, Height = height, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme ?? ThemeVariant.Light,
            Content = new Border { Padding = new Thickness(24), Child = content } };
        ControlSizing.SetSize(window, density); Motion.SetEnabled(window, false); window.Show(); Flush(); return window;
    }
    private static T Part<T>(Control control, string name) where T : Control => control.GetVisualDescendants().OfType<T>().Single(c => c.Name == name && c.TemplatedParent == control);
    private static GanttChart Sample(IEnumerable<GanttTask>? tasks = null) => new() { ItemsSource = tasks ?? GanttDemoViewModel.CreateTasks(),
        ViewStart = new(2026,10,5), ViewEnd = new(2026,10,26), Today = new(2026,10,14), TaskHeader = "任务名称", OwnerHeader = "负责人", ProgressHeader = "进度" };
    private static void Save(Window window, string? output, string name)
    {
        if (output is null) return;
        using var frame = window.CaptureRenderedFrame()!; frame.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default);
    }
    private static ListBoxItem Row(ListBox list, string title) => list.GetVisualDescendants().OfType<ListBoxItem>().Single(item => AutomationProperties.GetName(item)?.StartsWith(title + ",", StringComparison.Ordinal) == true);
    private static void Click(Window window, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void VerifyRendering(ThemeVariant theme, ControlSize density, Action<bool, string> check, string? output)
    {
        var view = new GanttDemoView(); var window = Open(view, theme, density);
        var chart = view.FindControl<GanttChart>("Schedule")!; var list = Part<ListBox>(chart, "PART_Rows"); var horizontal = Part<ScrollBar>(chart, "PART_HorizontalScrollBar");
        var header = Part<GanttHeaderPresenter>(chart, "PART_Header");
        var label = $"Gantt/{theme.Key}/{density}"; var height = density == ControlSize.Small ? 36 : density == ControlSize.Large ? 52 : 44;
        check(chart.VisibleTaskCount == 10 && chart.ValidationError is null && chart.SelectedTask?.Id == "timeline", label + ": compiled source and selection bindings initialize");
        check(chart.TryFindResource("One.Surface", theme, out var surface) && surface is ISolidColorBrush expectedSurface && chart.Background is ISolidColorBrush actualSurface && actualSurface.Color == expectedSurface.Color &&
            chart.TryFindResource("One.Accent", theme, out var accent) && accent is ISolidColorBrush expectedAccent && chart.ProgressBrush is ISolidColorBrush actualAccent && actualAccent.Color == expectedAccent.Color,
            label + ": Gantt brushes resolve the window theme variant");
        var rows = list.GetVisualDescendants().OfType<ListBoxItem>().ToArray();
        check(chart.RowHeight == height && rows.All(r => Math.Abs(r.Bounds.Height - height) < 1), label + $": native rows use the requested density (chart={chart.RowHeight}, rows={string.Join(',', rows.Select(r => r.Bounds.Height))})");
        Save(window, output, $"gantt-{theme.Key}-{density}-wide");
        using (var frame = window.CaptureRenderedFrame()!)
        {
            var row = Row(list, "需求梳理"); var pane = Math.Min(chart.TaskListWidth, row.Bounds.Width * .48);
            var point = row.TranslatePoint(new Point(pane + 25, row.Bounds.Height / 2 + Math.Min(26, row.Bounds.Height * .56) / 2 - 3), window)!.Value;
            var color = ((ISolidColorBrush)chart.ProgressBrush!).Color;
            check(HasInk(frame, point, color), label + ": completed task is visible in actual pixels");
            var todayX = pane + 9.5 * chart.DayWidth - horizontal.Value;
            var bodyPoint = row.TranslatePoint(new Point(todayX, 8), window)!.Value;
            var headerPoint = header.TranslatePoint(new Point(todayX, 8), window)!.Value;
            check(HasInk(frame, bodyPoint, color, 170) && HasInk(frame, headerPoint, color, 170), label + ": today line aligns across frozen header and body");
        }
        Save(window, output, $"gantt-{theme.Key}-{density}-wide");
        window.Width = 700; Flush();
        check(horizontal.IsVisible && horizontal.Maximum > 0 && Row(list, "需求梳理").Bounds.Width < 700, label + ": narrow viewport keeps fixed rows and a native timeline scrollbar");
        horizontal.Value = horizontal.Maximum; Flush();
        check(horizontal.Value > 0 && header.Bounds.Y == 0, label + ": horizontal navigation retains the frozen header");
        Save(window, output, $"gantt-{theme.Key}-{density}-narrow");
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light; Flush();
        check(chart.TryFindResource("One.Surface", window.ActualThemeVariant, out var switchedSurface) && switchedSurface is ISolidColorBrush expectedSwitched && chart.Background is ISolidColorBrush actualSwitched && actualSwitched.Color == expectedSwitched.Color,
            label + ": live theme switching updates the existing chart");
        var expansionButtons = list.GetVisualDescendants().OfType<Button>().Where(button => AutomationProperties.GetName(button) is { } name && (name.StartsWith("Expand ", StringComparison.Ordinal) || name.StartsWith("Collapse ", StringComparison.Ordinal))).ToArray();
        check(expansionButtons.Length > 0 && expansionButtons.All(button => button.Content is PathIcon icon && Equals(icon.Foreground, chart.Foreground)), label + ": live theme switching also updates native expansion icons");
        window.Close();
    }
    private static void VerifyHierarchy(Action<bool, string> check)
    {
        var tasks = GanttDemoViewModel.CreateTasks(); var chart = Sample(tasks); var window = Open(chart); var list = Part<ListBox>(chart, "PART_Rows");
        chart.SelectedTask = tasks.First(t => t.Id == "timeline"); Flush();
        Click(window, Row(list, "控件开发").GetVisualDescendants().OfType<Button>().Single());
        check(chart.VisibleTaskCount == 7 && chart.SelectedTask?.Id == "development", "Gantt/hierarchy: native collapse hides descendants and moves hidden selection to the parent");
        Click(window, Row(list, "控件开发").GetVisualDescendants().OfType<Button>().Single());
        check(chart.VisibleTaskCount == 10, "Gantt/hierarchy: native expand restores both task and timeline rows");
        Row(list, "控件开发").Focus();
        window.KeyPress(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null); window.KeyRelease(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null); Flush();
        check(chart.VisibleTaskCount == 7, "Gantt/keyboard: left collapses the selected summary");
        window.KeyPress(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, null); window.KeyRelease(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, null); Flush();
        check(chart.VisibleTaskCount == 10, "Gantt/keyboard: right expands the selected summary");
        check(chart.Selection is { IsSummary: true } selection && selection.Start == new DateOnly(2026,10,12) && selection.EndExclusive == new DateOnly(2026,10,22) && Math.Abs(selection.Progress - .3625) < .0001,
            "Gantt/summary: selection details use descendant dates and duration-weighted progress");
        Click(window, Row(list, "需求梳理"));
        check(chart.SelectedTask?.Id == "requirements", "Gantt/selection: native row clicks select the corresponding task");
        var before = tasks.Select(t => (t.Start, t.EndExclusive, t.Progress)).ToArray();
        var selected = Row(list, "需求梳理"); var p = selected.TranslatePoint(new Point(chart.TaskListWidth + 30, selected.Bounds.Height / 2), window)!.Value;
        window.MouseDown(p, MouseButton.Left); window.MouseMove(p + new Vector(70, 0), RawInputModifiers.LeftMouseButton); window.MouseUp(p + new Vector(70, 0), MouseButton.Left); Flush();
        check(before.SequenceEqual(tasks.Select(t => (t.Start, t.EndExclusive, t.Progress))), "Gantt/read-only: pointer dragging never changes dates or progress");
        var dayWidth = chart.DayWidth;
        window.MouseWheel(p, new Vector(0, 1), RawInputModifiers.Control); Flush();
        check(chart.DayWidth > dayWidth && before.SequenceEqual(tasks.Select(t => (t.Start, t.EndExclusive, t.Progress))), "Gantt/zoom: modified wheel zooms the timeline without editing tasks");
        var horizontal = Part<ScrollBar>(chart, "PART_HorizontalScrollBar"); var offset = horizontal.Value;
        window.MouseWheel(p, new Vector(0, -1), RawInputModifiers.Shift); Flush();
        check(horizontal.Value > offset, "Gantt/scrolling: Shift wheel reaches the timeline scrollbar before native vertical scrolling");
        tasks.First(t => t.Id == "design").IsExpanded = false; Flush();
        check(chart.BringTaskIntoView("interaction"), "Gantt/reveal: task IDs can reveal hidden descendants"); Flush();
        check(chart.VisibleTaskCount == 10 && chart.SelectedTask?.Id == "interaction", "Gantt/reveal: ancestor expansion and native selection stay synchronized");
        window.Close();
    }
    private static void VerifyChanges(Action<bool, string> check)
    {
        var tasks = GanttDemoViewModel.CreateTasks(); var chart = Sample(tasks); var window = Open(chart);
        var list = Part<ListBox>(chart, "PART_Rows");
        tasks[1].Title = "需求已更新"; Flush(); check(Row(list, "需求已更新") is not null, "Gantt/model: property notifications update realized rows");
        var child = new GanttTask { Id = "new", ParentId = "design", Title = "新增任务", Start = new(2026,10,6), EndExclusive = new(2026,10,9) };
        tasks.Add(child); Flush(); check(chart.VisibleTaskCount == 11, "Gantt/model: collection additions update both panels");
        child.ParentId = "absent"; Flush(); check(chart.VisibleTaskCount == 0 && chart.ValidationError is not null, "Gantt/validation: missing parents are rejected");
        child.ParentId = "design"; Flush(); check(chart.VisibleTaskCount == 11 && chart.ValidationError is null, "Gantt/validation: correcting the source restores the chart");
        tasks[0].ParentId = "requirements"; Flush(); check(chart.ValidationError is not null, "Gantt/validation: parent cycles are rejected");
        tasks[0].ParentId = null; child.Id = "requirements"; Flush(); check(chart.ValidationError is not null, "Gantt/validation: duplicate IDs are rejected");
        child.Id = "new"; child.EndExclusive = child.Start; Flush(); check(chart.ValidationError is not null, "Gantt/validation: empty task intervals are rejected");
        child.EndExclusive = child.Start.AddDays(1); child.Progress = double.NaN; Flush(); check(chart.ValidationError is not null, "Gantt/validation: non-finite progress is rejected");
        child.Progress = .5; Flush(); check(chart.ValidationError is null, "Gantt/validation: valid data recovers after invalid progress");
        tasks.Remove(child); Flush(); check(chart.VisibleTaskCount == 10, "Gantt/model: removed tasks release their rows");
        var host = (Border)window.Content!; host.Child = null; Flush(); tasks.RemoveAt(9); host.Child = chart; Flush();
        check(chart.VisibleTaskCount == 9, "Gantt/lifetime: reattachment observes changes made while detached");
        chart.ItemsSource = new List<GanttTask> { new() { Id = "plain", Title = "Plain" } }; Flush();
        ((List<GanttTask>)chart.ItemsSource).Add(new() { Id = "plain2", Title = "Plain 2" }); chart.RefreshData(); Flush();
        check(chart.VisibleTaskCount == 2, "Gantt/model: RefreshData supports non-notifying collections");
        chart.ViewEnd = chart.ViewStart; Flush(); check(!list.IsVisible, "Gantt/viewport: invalid ranges do not draw reversed date geometry");
        chart.ItemsSource = null; chart.ViewEnd = chart.ViewStart.AddDays(1); Flush(); check(chart.VisibleTaskCount == 0 && chart.ValidationError is null, "Gantt/empty: null source is an empty chart");
        var summary = new GanttTask { Id = "milestone-summary", Title = "Milestone summary" };
        chart.ItemsSource = new[] { summary, new GanttTask { Id = "only-milestone", ParentId = summary.Id, Title = "Milestone", Start = new(2026,10,23), IsMilestone = true } };
        chart.SelectedTask = summary; Flush();
        check(chart.Selection is { Start: var start, EndExclusive: var end, Progress: 0 } && start == new DateOnly(2026,10,23) && end == new DateOnly(2026,10,24),
            "Gantt/summary: a milestone-only group covers the milestone's full displayed date");
        chart.FitToProject(); Flush(); check(chart.ViewStart == new DateOnly(2026,10,23) && chart.ViewEnd == new DateOnly(2026,10,24), "Gantt/viewport: milestone-only projects have a non-empty fitted range");
        window.Close();
    }
    private static void VerifyLargeData(Action<bool, string> check, string? output)
    {
        var view = new GanttDemoView(); var window = Open(view); var chart = view.FindControl<GanttChart>("Schedule")!;
        view.Model.LargeData = true; Flush(); var list = Part<ListBox>(chart, "PART_Rows");
        check(chart.VisibleTaskCount == 1511 && list.GetVisualDescendants().OfType<ListBoxItem>().Count() < 40, "Gantt/virtualization: large data realizes only viewport rows");
        check(chart.BringTaskIntoView("extra-1499"), "Gantt/virtualization: the last task can be reached"); Flush();
        var last = Row(list, "迭代任务 1,500");
        var scroll = list.GetVisualDescendants().OfType<ScrollViewer>().Single();
        check(scroll.Offset.Y > 0 && last.Bounds.Height == chart.RowHeight && list.GetVisualDescendants().OfType<ListBoxItem>().Count() < 40,
            "Gantt/scrolling: native vertical scrolling keeps distant task and timeline on one row");
        var header = Part<GanttHeaderPresenter>(chart, "PART_Header");
        check(header.TranslatePoint(default, chart)!.Value.Y < 2, "Gantt/scrolling: vertical scrolling leaves the calendar header fixed");
        Save(window, output, "gantt-large-data");
        view.Model.SearchText = "控件开发"; Flush(); check(chart.VisibleTaskCount == 4, "Gantt/demo: a matching summary retains descendants and calculated dates");
        view.Model.SearchText = "回归"; Flush(); check(chart.VisibleTaskCount == 2 && chart.ValidationError is null, "Gantt/demo: search retains matching tasks and their ancestors");
        view.Model.SearchText = "missing"; Flush(); check(chart.VisibleTaskCount == 0, "Gantt/demo: no matches shows the empty state");
        view.Model.SearchText = ""; view.Model.SelectedOwner = "许安"; Flush(); check(chart.VisibleTaskCount == 2, "Gantt/demo: owner filtering preserves hierarchy");
        chart.FitToProject(); Flush(); check(chart.DayWidth >= 4 && chart.ViewEnd > chart.ViewStart, "Gantt/viewport: project fit creates a valid date range");
        window.Close();
    }
    private static bool HasInk(Bitmap bitmap, Point point, Color expected, int tolerance = 45)
    {
        var stride = bitmap.PixelSize.Width * 4; var bytes = new byte[stride * bitmap.PixelSize.Height]; var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), pin.AddrOfPinnedObject(), bytes.Length, stride); } finally { pin.Free(); }
        var scale = bitmap.Dpi.X / 96;
        for (var y = Math.Max(0, (int)(point.Y * scale) - 2); y <= Math.Min(bitmap.PixelSize.Height - 1, (int)(point.Y * scale) + 2); y++)
        for (var x = Math.Max(0, (int)(point.X * scale) - 2); x <= Math.Min(bitmap.PixelSize.Width - 1, (int)(point.X * scale) + 2); x++)
        {
            var i = y * stride + x * 4; var rgba = bitmap.Format == PixelFormat.Rgba8888;
            // Thin strokes have fractional pixel coverage; solid fills use the stricter default.
            if (Math.Abs(bytes[i + (rgba ? 0 : 2)] - expected.R) + Math.Abs(bytes[i + 1] - expected.G) + Math.Abs(bytes[i + (rgba ? 2 : 0)] - expected.B) < tolerance) return true;
        }
        return false;
    }
}
