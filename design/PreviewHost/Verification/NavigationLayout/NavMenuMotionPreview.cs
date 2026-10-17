using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavMenuMotionPreview
{
    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void Advance(int milliseconds)
    {
        var frame = new DispatcherFrame();
        var pulse = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        var deadline = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        pulse.Tick += (_, _) => AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        deadline.Tick += (_, _) =>
        {
            deadline.Stop();
            frame.Continue = false;
        };
        pulse.Start();
        deadline.Start();
        try
        {
            Dispatcher.UIThread.PushFrame(frame);
        }
        finally
        {
            pulse.Stop();
            deadline.Stop();
        }

        Flush();
    }

    private static void Until(Func<bool> condition)
    {
        var watch = Stopwatch.StartNew();
        while (!condition() && watch.ElapsedMilliseconds < 2500) Advance(20);
    }

    private static T Part<T>(Control control, string name) where T : Control
    {
        return control.GetVisualDescendants().OfType<T>().First(item => item.Name == name);
    }

    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var demo = new NavMenuDemoView();
            var menu = demo.Menu;
            var model = demo.Model;
            model.ShowFilter = false;
            var window = new Window { Width = 1120, Height = 1060, Content = demo, RequestedThemeVariant = theme };
            ControlSizing.SetSize(window, size);
            Motion.SetEnabled(window, false);
            window.Show();
            Flush();
            var prefix = $"NavMenu/motion/{theme.Key}/{size}";
            var root = (NavMenuItem)menu.ContainerFromIndex(0)!;
            var branch = Part<RevealHost>(root, "TreeContent");
            check(
                menu.TransitionDuration.TotalMilliseconds == 200 && branch.Duration == menu.TransitionDuration &&
                branch.TransitionExtent == menu.Bounds.Height,
                prefix + ": compiled duration and finite viewport reach the native branch");
            menu.TransitionDuration = TimeSpan.FromMilliseconds(500);
            var fullHeight = branch.Bounds.Height;
            var childHeight = branch.Child!.Bounds.Height;
            check(fullHeight > menu.Bounds.Height * 10 && root.GetRealizedContainers().Count() < 80,
                prefix + ": thousand-item extent remains complete with recycled containers");
            Motion.SetEnabled(window, true);
            model.Roots[0].IsExpanded = false;
            Advance(35);
            check(
                branch.IsVisible && !branch.IsHitTestVisible && branch.Bounds.Height > 0 &&
                branch.Bounds.Height <= menu.Bounds.Height && Math.Abs(branch.Child.Bounds.Height - childHeight) < 1,
                prefix + ": collapse moves a bounded viewport while keeping full child geometry stable");
            var closing = branch.Bounds.Height;
            Advance(40);
            check(branch.Bounds.Height < closing, prefix + ": closing advances through real intermediate geometry");
            model.Roots[0].IsExpanded = true;
            Advance(30);
            check(
                branch.IsVisible && branch.IsHitTestVisible && branch.Bounds.Height > 0 &&
                branch.Bounds.Height <= menu.Bounds.Height,
                prefix + ": reversal resumes the visible pose without a full-height jump");
            Until(() => branch.Bounds.Height > menu.Bounds.Height * 10);
            check(
                branch.IsOpen && branch.Bounds.Height > menu.Bounds.Height * 10 &&
                Math.Abs(branch.Bounds.Height - branch.Child.DesiredSize.Height) < 1,
                prefix +
                $": expansion restores the full native scroll extent ({branch.Bounds.Height}/{branch.Child.DesiredSize.Height}, initial={fullHeight}, open={branch.IsOpen}, attached={branch.IsAttachedToVisualTree()}, same={ReferenceEquals(root, menu.ContainerFromIndex(0))}, expanded={root.IsExpanded})");
            check(
                menu.SelectPath(0, 999) && ReferenceEquals(model.Selected, model.Roots[0].Entries[999]) &&
                root.GetRealizedContainers().Count() < 80 && root.ContainerFromIndex(0) is null,
                prefix + ": distant navigation bypasses motion and recycles actual child containers");
            menu.SelectPath(0);
            Flush();
            model.IsCollapsed = true;
            Advance(40);
            root = (NavMenuItem)menu.ContainerFromIndex(0)!;
            var icon = Part<ContentPresenter>(root, "NavMenuIcon");
            check(icon.RenderTransform is TransformGroup && menu.IsCollapsed,
                prefix + ": sidebar collapse animates icon geometry while compact navigation is immediately available");
            model.IsCollapsed = false;
            Advance(40);
            var label = Part<ContentPresenter>(root, "NavMenuContent");
            check(label.IsVisible && label.Opacity > 0 && label.Opacity < 1,
                prefix + ": expanded labels fade in instead of appearing abruptly");
            model.IsCollapsed = true;
            model.IsCollapsed = false;
            Until(() => icon.RenderTransform is not TransformGroup);
            check(
                icon.RenderTransform is not TransformGroup && label.Opacity == 1 && !menu.IsCollapsed &&
                model.Roots[0].IsExpanded && ReferenceEquals(model.Selected, model.Roots[0]),
                prefix + ": fast reversal restores transforms and preserves model state");
            model.IsCollapsed = true;
            Advance(25);
            Motion.SetEnabled(window, false);
            Flush();
            check(icon.RenderTransform is not TransformGroup && menu.Bounds.Width == menu.CollapsedWidth,
                prefix + ": disabling motion mid-collapse settles to the compact target");
            model.IsCollapsed = false;
            model.Roots[0].IsExpanded = false;
            Flush();
            branch = Part<RevealHost>(root, "TreeContent");
            check(!branch.IsVisible && !menu.IsCollapsed && label.Opacity == 1,
                prefix + ": reduced motion switches both sidebar and branch immediately");
            Motion.SetEnabled(window, true);
            Advance(50);
            check(!branch.IsVisible && icon.RenderTransform is not TransformGroup,
                prefix + ": re-enabling motion does not replay a completed change");
            model.Roots[0].IsExpanded = true;
            Advance(25);
            Motion.SetEnabled(window, false);
            Flush();
            check(branch.IsVisible && branch.IsOpen && branch.Bounds.Height > menu.Bounds.Height,
                prefix + ": disabling branch motion restores complete scroll extent immediately");
            Motion.SetEnabled(window, true);
            model.IsCollapsed = true;
            Advance(25);
            var parent = (Border)menu.Parent!;
            parent.Child = null;
            Flush();
            check(icon.RenderTransform is not TransformGroup && label.Opacity == 1,
                prefix + ": unload cancels pending motion and restores generated visuals");
            parent.Child = menu;
            Flush();
            model.IsCollapsed = false;
            menu.TransitionDuration = TimeSpan.Zero;
            Flush();
            check(icon.RenderTransform is not TransformGroup && label.Opacity == 1,
                prefix + ": zero duration also cancels a pending or active transition");
            window.Close();
            Flush();
        }
    }

    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var demo = new NavMenuDemoView();
            var menu = demo.Menu;
            var window = new Window
            {
                Width = 1120, Height = 1060, Content = demo, RequestedThemeVariant = theme,
                WindowDecorations = WindowDecorations.None
            };
            ControlSizing.SetSize(window, size);
            Motion.SetEnabled(window, false);
            window.Show();
            Flush();
            menu.TransitionDuration = TimeSpan.FromMilliseconds(400);
            Motion.SetEnabled(window, true);

            void Save(string phase)
            {
                using var bitmap = window.CaptureRenderedFrame()!;
                bitmap.Save(Path.Combine(output, $"navmenu-motion-{phase}-{theme.Key}-{size}.png"),
                    PngBitmapEncoderOptions.Default);
            }

            demo.Model.Roots[0].IsExpanded = false;
            Advance(55);
            Save("closing");
            Motion.SetEnabled(window, false);
            Flush();
            Motion.SetEnabled(window, true);
            demo.Model.Roots[0].IsExpanded = true;
            Advance(55);
            Save("opening");
            Motion.SetEnabled(window, false);
            demo.Model.IsCollapsed = true;
            Flush();
            Motion.SetEnabled(window, true);
            demo.Model.IsCollapsed = false;
            Advance(55);
            Save("sidebar");
            window.Close();
            Flush();
        }
    }
}