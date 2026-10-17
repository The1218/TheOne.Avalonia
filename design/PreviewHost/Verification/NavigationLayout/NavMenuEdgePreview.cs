using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavMenuEdgePreview
{
    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 900, 420, 260 })
        foreach (var right in new[] { false, true })
            Verify(theme, size, width, right, check);
    }

    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 900, 300 })
        {
            var demo = new NavMenuDemoView();
            var menu = demo.Menu;
            menu.DataContext = demo.Model;
            ((Border)menu.Parent!).Child = null;
            demo.Model.IsCollapsed = true;
            menu.Height = double.NaN;
            menu.MaxHeight = 300;
            menu.HorizontalAlignment = HorizontalAlignment.Right;
            var window = new Window
            {
                Width = width, Height = 460, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
                Content = new Border { Padding = new Thickness(16), Child = menu }
            };
            ControlSizing.SetSize(window, size);
            Motion.SetEnabled(window, false);
            window.Show();
            Flush();
            menu.SelectPath(0, 0, 6);
            Wait(40);
            using var image = window.CaptureRenderedFrame()!;
            image.Save(Path.Combine(output, $"navmenu-{(width == 900 ? "flipped" : "compact")}-{theme.Key}-{size}.png"),
                PngBitmapEncoderOptions.Default);
            window.Close();
            Flush();
        }
    }

    private static void Verify(ThemeVariant theme, ControlSize size, int width, bool right, Action<bool, string> check)
    {
        var demo = new NavMenuDemoView();
        var menu = demo.Menu;
        menu.DataContext = demo.Model;
        ((Border)menu.Parent!).Child = null;
        demo.Model.IsCollapsed = true;
        menu.Height = double.NaN;
        menu.MaxHeight = 300;
        menu.HorizontalAlignment = right ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        var window = new Window
        {
            Width = width, Height = 460, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(16), Child = menu }
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        menu.HoverOpenDelay = TimeSpan.FromMilliseconds(30);
        menu.HoverCloseDelay = TimeSpan.FromMilliseconds(240);
        var label = $"NavMenu/edge/{theme.Key}/{size}/{width}/{(right ? "right" : "left")}";
        check(menu.OpenSubmenu(0), $"{label}: root opens at window edge");
        Wait(40);
        var child = menu.Submenu!;
        Bounds(child, window, check, label + "/first");
        check(menu.SubmenuOpensLeft == right, $"{label}: root flips toward available space");
        var branch = (NavMenuItem)child.ContainerFromIndex(0)!;
        Move(Header(branch));
        Wait(70);
        check(child.Submenu is { ItemCount: 1000 }, $"{label}: hover reaches thousand-item next level");
        if (child.Submenu is not { } leafMenu)
        {
            window.Close();
            return;
        }

        Bounds(leafMenu, window, check, label + "/second");
        var parentPanel = Panel(child);
        var leafPanel = Panel(leafMenu);
        check(
            child.IsCompactSubmenu
                ? parentPanel.Opacity == 0 && !parentPanel.IsHitTestVisible
                : !Rect(parentPanel, window).Intersects(Rect(leafPanel, window)),
            $"{label}: narrow level replaces parent, or cascade avoids covering it");
        Move(Header((NavMenuItem)leafMenu.ContainerFromIndex(0)!));
        Wait(290);
        check(menu.IsSubmenuOpen && child.IsSubmenuOpen,
            $"{label}: entering flipped or compact popup keeps its ancestors alive");
        check(leafMenu.SelectPath(999), $"{label}: select uncreated thousandth leaf");
        Flush();
        check(
            ReferenceEquals(menu.SelectedItem, demo.Model.Roots[0].Entries[0].Entries[999]) &&
            leafMenu.GetRealizedContainers().Count() is > 0 and < 80,
            $"{label}: distant selection and actual container recycling remain bounded");
        if (child.IsCompactSubmenu)
        {
            var back = leafMenu.GetVisualAncestors().OfType<FlyoutPresenter>().First().GetVisualDescendants()
                .OfType<Button>().Single(button => button.Name == "PART_SubmenuBack");
            Click(back);
            Wait(30);
            check(
                menu.IsSubmenuOpen && !child.IsSubmenuOpen && parentPanel.Opacity == 1 && parentPanel.IsHitTestVisible,
                $"{label}: back restores parent and its selection/scroll state");
            child.OpenSubmenu(0);
            Wait(40);
        }

        window.Width = 1120;
        Wait(40);
        check(
            !child.IsCompactSubmenu && Panel(child).Opacity == 1 &&
            !Rect(Panel(child), window).Intersects(Rect(Panel(child.Submenu!), window)),
            $"{label}: widening restores adjacent cascade without recreating data");
        window.Width = 260;
        window.Height = 300;
        Wait(40);
        Bounds(menu.Submenu!, window, check, label + "/resize-parent");
        Bounds(child.Submenu!, window, check, label + "/resize-child");
        var last = child.Submenu!;
        last.SelectPath(999);
        Flush();
        var row = (NavMenuItem)last.ContainerFromIndex(999)!;
        row.Focus();
        window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
        window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
        Flush();
        check(!menu.IsSubmenuOpen && demo.Model.Invocations == 1,
            $"{label}: compact deepest leaf invokes once and releases chain");
        window.Close();
        Flush();
    }

    private static void Bounds(NavMenu menu, Window window, Action<bool, string> check, string label)
    {
        var bounds = Rect(Panel(menu), window);
        check(
            bounds.Left >= -1 && bounds.Top >= -1 && bounds.Right <= window.ClientSize.Width + 1 &&
            bounds.Bottom <= window.ClientSize.Height + 1,
            $"{label}: popup stays inside {window.ClientSize} ({bounds})");
        check(menu.ItemsPanelRoot is VirtualizingStackPanel && menu.GetRealizedContainers().Count() is > 0 and < 80,
            $"{label}: thousand-item viewport creates only {menu.GetRealizedContainers().Count()} containers");
    }

    private static FlyoutPresenter Panel(NavMenu menu)
    {
        return menu.GetVisualAncestors().OfType<FlyoutPresenter>().First();
    }

    private static Rect Rect(Control control, Window window)
    {
        return new Rect(control.TranslatePoint(default, window)!.Value, control.Bounds.Size);
    }

    private static Border Header(Control row)
    {
        return row.GetVisualDescendants().OfType<Border>()
            .First(border => border.Name == "PART_LayoutRoot" && border.TemplatedParent == row);
    }

    private static void Move(Control control)
    {
        var host = TopLevel.GetTopLevel(control)!;
        host.MouseMove(control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!
            .Value);
        Flush();
    }

    private static void Click(Control control)
    {
        Move(control);
        var host = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
        host.MouseDown(point, MouseButton.Left);
        host.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void Wait(int milliseconds)
    {
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            frame.Continue = false;
        };
        timer.Start();
        try
        {
            Dispatcher.UIThread.PushFrame(frame);
        }
        finally
        {
            timer.Stop();
        }

        Flush();
        Flush();
    }
}