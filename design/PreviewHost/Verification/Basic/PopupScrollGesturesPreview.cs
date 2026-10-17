using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class PopupScrollPreview
{
    internal static void VerifyGestures(Action<bool, string> check, bool audit, string? filter = null)
    {
        foreach (var sample in Samples())
        {
            if (filter is not null && !sample.Name.StartsWith(filter, StringComparison.Ordinal)) continue;
            if (!audit && filter is null && (sample.Name.StartsWith("Flyout/", StringComparison.Ordinal) ||
                sample.Name.StartsWith("MenuFlyout/", StringComparison.Ordinal) ||
                sample.Name.StartsWith("ContextMenu/", StringComparison.Ordinal) ||
                sample.Name is "ColorPicker" or "NavMenu/submenu" or "DataGrid/filter")) continue;
            var outside = new Border { Width = 180, Height = 40, Background = Brushes.Transparent };
            var canvas = new Canvas { Width = 1800, Height = 1800, Children = { outside, sample.Anchor } };
            Canvas.SetLeft(sample.Anchor, 200); Canvas.SetTop(sample.Anchor, 200);
            sample.Anchor.MaxWidth = 340;
            var outer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto, Content = canvas
            };
            var window = new Window { Width = 800, Height = 600, Content = outer };
            Motion.SetEnabled(window, false);
            window.Show(); Flush();
            outer.Offset = new Vector(100, 100); Flush();
            try
            {
                var root = sample.Open(); Flush();
                var offset = outer.Offset;
                var wheelLeak = false;
                void Wheel(Vector delta)
                {
                    var host = TopLevel.GetTopLevel(root)!;
                    host.MouseWheel(root.TranslatePoint(new Point(root.Bounds.Width / 2, 2), host)!.Value, delta);
                    Flush(); wheelLeak |= outer.Offset != offset;
                }
                foreach (var axis in new[] { new Vector(-1, 0), new Vector(0, -1), new Vector(-1, -.7), new Vector(1, 0), new Vector(0, 1) })
                foreach (var magnitude in new[] { .8, .4, .2, .1, .05, .025, .0125 })
                    Wheel(axis * magnitude);
                outer.Offset = offset; Flush();
                var phaseLeak = 0;
                void Phase(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e) => phaseLeak++;
                outer.AddHandler(InputElement.ScrollGestureInertiaStartingEvent, Phase);
                outer.AddHandler(InputElement.ScrollGestureEndedEvent, Phase);
                var gestureLeak = false;
                var received = 0;
                void Observe(object? sender, ScrollGestureEventArgs e) => received++;
                root.AddHandler(InputElement.ScrollGestureEvent, Observe, handledEventsToo: true);
                void Gesture(InputElement source, Vector direction)
                {
                    var id = ScrollGestureEventArgs.GetNextFreeId();
                    source.RaiseEvent(new ScrollGestureEventArgs(id, direction * .5));
                    Flush(); gestureLeak |= outer.Offset != offset;
                    foreach (var scale in new[] { 4d, 2, 1, .5, .25, .1 })
                    {
                        source.RaiseEvent(new ScrollGestureEventArgs(id, direction * scale));
                        Flush(); gestureLeak |= outer.Offset != offset;
                    }
                    source.RaiseEvent(new ScrollGestureEndedEventArgs(id));
                    Flush(); gestureLeak |= outer.Offset != offset;
                }
                var chrome = root.InputHitTest(new Point(root.Bounds.Width / 2, 2)) as InputElement ?? root;
                foreach (var axis in new[] { new Vector(1, 0), new Vector(0, 1), new Vector(1, .7), new Vector(-1, 0), new Vector(0, -1) })
                    Gesture(chrome, axis);
                var scrollers = root.GetVisualDescendants().Prepend(root).OfType<ScrollViewer>().ToArray();
                foreach (var end in new[] { false, true })
                {
                    foreach (var scroll in scrollers)
                        scroll.Offset = end ? new Vector(Math.Max(0, scroll.Extent.Width - scroll.Viewport.Width), Math.Max(0, scroll.Extent.Height - scroll.Viewport.Height)) : default;
                    Flush();
                    var content = root.InputHitTest(new Point(root.Bounds.Width / 2, root.Bounds.Height / 2)) as InputElement ?? root;
                    Gesture(content, new Vector(end ? 1 : -1, end ? 1 : -1));
                }
                root.RemoveHandler(InputElement.ScrollGestureEvent, Observe);
                if (audit)
                    Console.WriteLine($"GESTURE AUDIT: {sample.Name}: wheelLeak={wheelLeak}; gestureLeak={gestureLeak}; leakedPhases={phaseLeak}; received={received}");
                else
                {
                    check(!wheelLeak, $"Popup gestures/{sample.Name}: fractional horizontal/vertical/diagonal wheel tails stay isolated");
                    check(!gestureLeak && phaseLeak == 0 && received > 0,
                        $"Popup gestures/{sample.Name}: gesture, inertia and end stay isolated at chrome and both boundaries");
                    VerifyNativeInertia(root, outer, check, sample.Name);
                    check(phaseLeak == 0, $"Popup gestures/{sample.Name}: native inertia phases do not reach the outer scroller");
                    if (sample.Name == "ComboBox/200")
                        VerifyHorizontalContent(root, outer, check);
                }
                sample.Close(); Flush();
                if (!audit)
                {
                    offset = outer.Offset; gestureLeak = false;
                    Gesture(root, new Vector(1, 1));
                    check(!gestureLeak && phaseLeak == 0, $"Popup gestures/{sample.Name}: closed cached child contains queued gesture tails");
                    root = sample.Open(); Flush(); offset = outer.Offset; gestureLeak = false;
                    Gesture(root.InputHitTest(new Point(root.Bounds.Width / 2, 2)) as InputElement ?? root, new Vector(1, 1));
                    check(!gestureLeak && phaseLeak == 0, $"Popup gestures/{sample.Name}: reopened popup contains inertial tails");
                    sample.Close(); Flush();
                    outside.BringIntoView(); Flush();
                    var before = outer.Offset;
                    window.MouseWheel(outside.TranslatePoint(new Point(10, 10), window)!.Value, new Vector(-.5, -.5)); Flush();
                    check(outer.Offset.X > before.X && outer.Offset.Y > before.Y, $"Popup gestures/{sample.Name}: closed popup leaves both outer axes available");
                }
                outer.RemoveHandler(InputElement.ScrollGestureInertiaStartingEvent, Phase);
                outer.RemoveHandler(InputElement.ScrollGestureEndedEvent, Phase);
            }
            catch (Exception e) when (audit) { Console.WriteLine($"GESTURE AUDIT ERROR: {sample.Name}: {e.Message}"); }
            finally { window.Close(); Flush(); }
        }
    }

    private static void Advance(int milliseconds)
    {
        var frame = new DispatcherFrame();
        var pulse = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(8) };
        var end = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        pulse.Tick += (_, _) => AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        end.Tick += (_, _) => { end.Stop(); frame.Continue = false; };
        pulse.Start(); end.Start();
        try { Dispatcher.UIThread.PushFrame(frame); }
        finally { pulse.Stop(); end.Stop(); }
        Flush();
    }

    private static void VerifyNativeInertia(Control root, ScrollViewer outer, Action<bool, string> check, string name)
    {
        var scroll = root.GetVisualDescendants().OfType<ScrollViewer>()
            .FirstOrDefault(s => s.Extent.Height > s.Viewport.Height + 100 && s.Viewport.Height > 60);
        if (scroll is null) return;
        var source = scroll.GetVisualDescendants().OfType<ScrollContentPresenter>().First();
        scroll.Offset = scroll.Offset.WithY(Math.Min(40, (scroll.Extent.Height - scroll.Viewport.Height) / 2)); Flush();
        var offset = outer.Offset;
        var before = scroll.Offset;
        var starts = 0; var ends = 0;
        void Starting(object? sender, ScrollGestureInertiaStartingEventArgs e) => starts++;
        void Ended(object? sender, ScrollGestureEndedEventArgs e) => ends++;
        root.AddHandler(InputElement.ScrollGestureInertiaStartingEvent, Starting, handledEventsToo: true);
        root.AddHandler(InputElement.ScrollGestureEndedEvent, Ended, handledEventsToo: true);
        var host = TopLevel.GetTopLevel(source)!;
        var point = source.TranslatePoint(new Point(source.Bounds.Width / 2, source.Bounds.Height / 2), host)!.Value;
        using var pointer = host.TouchBegin(point);
        for (var i = 1; i <= 4; i++)
        {
            Advance(16);
            host.TouchMove(pointer, point + new Vector(0, -8 * i)); Flush();
        }
        host.TouchEnd(pointer, point + new Vector(0, -32)); Advance(80);
        var moved = scroll.Offset != before;
        // A new contact interrupts native inertia, including looping picker columns.
        // Cancel that contact so it cannot become a tap selecting/closing a menu item.
        if (ends == 0)
        {
            using (host.TouchBegin(point)) Flush();
            Advance(40);
        }
        check(ends > 0 && moved && outer.Offset == offset && (starts > 0 || name.StartsWith("MenuItem/", StringComparison.Ordinal)),
            $"Popup gestures/{name}: native touch scrolling/inertia stays internal and interruption cleans up (start={starts},end={ends},moved={moved})");
        root.RemoveHandler(InputElement.ScrollGestureInertiaStartingEvent, Starting);
        root.RemoveHandler(InputElement.ScrollGestureEndedEvent, Ended);
    }

    private static void VerifyHorizontalContent(Control root, ScrollViewer outer, Action<bool, string> check)
    {
        var scroll = root.GetVisualDescendants().OfType<ScrollViewer>().First();
        var items = root.GetVisualDescendants().OfType<ItemsPresenter>().First();
        root.Width = 300;
        scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        items.Width = 1000; Flush();
        var source = scroll.GetVisualDescendants().OfType<ScrollContentPresenter>().First();
        var outerOffset = outer.Offset;
        var before = scroll.Offset;
        var host = TopLevel.GetTopLevel(source)!;
        var at = source.TranslatePoint(new Point(source.Bounds.Width / 2, source.Bounds.Height / 2), host)!.Value;
        host.MouseWheel(at, new Vector(-.25, 0)); Flush();
        check(scroll.Extent.Width > scroll.Viewport.Width && scroll.Offset.X > before.X && outer.Offset == outerOffset,
            "Popup gestures/ComboBox: native horizontal wheel scrolls wide popup content");
        before = scroll.Offset;
        var id = ScrollGestureEventArgs.GetNextFreeId();
        source.RaiseEvent(new ScrollGestureEventArgs(id, new Vector(15.5, 0))); Flush();
        source.RaiseEvent(new ScrollGestureEndedEventArgs(id)); Flush();
        check(scroll.Offset.X > before.X && outer.Offset == outerOffset,
            "Popup gestures/ComboBox: horizontal gesture and end remain native inside wide content");
        scroll.Offset = scroll.Offset.WithX(scroll.Extent.Width - scroll.Viewport.Width); Flush();
        id = ScrollGestureEventArgs.GetNextFreeId();
        source.RaiseEvent(new ScrollGestureEventArgs(id, new Vector(20, 0))); Flush();
        source.RaiseEvent(new ScrollGestureEndedEventArgs(id)); Flush();
        check(outer.Offset == outerOffset, "Popup gestures/ComboBox: horizontal content boundary does not chain out");
    }
}
