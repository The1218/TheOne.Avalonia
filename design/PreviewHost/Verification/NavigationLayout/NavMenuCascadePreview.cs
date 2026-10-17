using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavMenuCascadePreview
{
    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            Verify(theme, size, check);
    }

    private static void Verify(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var demo = new NavMenuDemoView();
        demo.Model.IsCollapsed = true;
        var window = new Window
        {
            Width = 1400, Height = 780, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = demo }
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        var menu = demo.Menu;
        menu.HoverOpenDelay = TimeSpan.FromMilliseconds(40);
        menu.HoverCloseDelay = TimeSpan.FromMilliseconds(120);
        var label = $"NavMenu/cascade/{theme.Key}/{size}";
        var root = (NavMenuItem)menu.ContainerFromIndex(0)!;
        var trigger = demo.FindControl<Button>("OpenSubmenu")!;
        trigger.Focus();
        Move(Header(root));
        Wait(80);
        check(menu.IsSubmenuOpen && trigger.IsFocused && menu.SelectedItem is null,
            $"{label}: hover opens without stealing focus or selecting a page");
        if (menu.Submenu is not { } child)
        {
            window.Close();
            return;
        }

        Bounded(menu, check, label + "/root");
        Bounded(child, check, label + "/second");
        var branch = (NavMenuItem)child.ContainerFromIndex(0)!;
        var title = Part<ContentPresenter>(branch, "PART_HeaderPresenter");
        var arrow = Part<ToggleButton>(branch, "PART_ExpandCollapseChevron");
        var titleRight = title.TranslatePoint(new Point(title.Bounds.Width, 0), Header(branch))!.Value.X;
        var arrowLeft = arrow.TranslatePoint(new Point(0, 0), Header(branch))!.Value.X;
        check(Grid.GetColumn(title) == 0 && titleRight <= arrowLeft + 1,
            $"{label}: title and cascade arrow occupy separate columns");
        Move(Header(branch));
        Wait(80);
        check(child.IsSubmenuOpen && child.Submenu is { ItemCount: 1000 },
            $"{label}: hovering branch opens a separate third-level popup");
        if (child.Submenu is not { } grandchild)
        {
            window.Close();
            return;
        }

        Bounded(grandchild, check, label + "/third");
        var outerPresenter = child.GetVisualAncestors().OfType<FlyoutPresenter>().First();
        var innerPresenter = grandchild.GetVisualAncestors().OfType<FlyoutPresenter>().First();
        var outerRight = outerPresenter.TranslatePoint(new Point(outerPresenter.Bounds.Width, 0), window)!.Value.X;
        var innerLeft = innerPresenter.TranslatePoint(new Point(0, 0), window)!.Value.X;
        check(innerLeft >= outerRight,
            $"{label}: cascade is adjacent without covering parent rows (outer-right={outerRight}, inner-left={innerLeft}, anchor-width={Header(branch).Bounds.Width}, menu-width={child.Bounds.Width})");
        check(!Part<RevealHost>(branch, "TreeContent").IsVisible && !demo.Model.Roots[0].Entries[0].IsExpanded,
            $"{label}: cascade preserves model expansion and never lays out inline descendants");
        check(
            grandchild.ActualThemeVariant == theme && grandchild.ItemHeight == menu.ItemHeight &&
            grandchild.IconSize == menu.IconSize && grandchild.Bounds.Height <= menu.SubmenuMaxHeight,
            $"{label}: all popup levels inherit theme, density and finite viewport");
        var leaf = (NavMenuItem)grandchild.ContainerFromIndex(0)!;
        Move(Header(leaf));
        Wait(180);
        check(menu.IsSubmenuOpen && child.IsSubmenuOpen,
            $"{label}: pointer inside deepest popup keeps its ancestors open");
        Move(Header(branch));
        Wait(60);
        var edge = Header(branch)
            .TranslatePoint(new Point(Header(branch).Bounds.Width + 3, Header(branch).Bounds.Height / 2), window)!
            .Value;
        window.MouseMove(edge);
        Wait(45);
        check(menu.IsSubmenuOpen && child.IsSubmenuOpen,
            $"{label}: crossing the anchor-popup gap does not close immediately");
        Move(Header(leaf));
        Wait(160);
        check(menu.IsSubmenuOpen && child.IsSubmenuOpen, $"{label}: entering popup cancels pending close");
        var sibling = (NavMenuItem)child.ContainerFromIndex(2)!;
        Move(Header(sibling));
        Wait(180);
        check(menu.IsSubmenuOpen && !child.IsSubmenuOpen,
            $"{label}: hovering a sibling leaf closes only the obsolete child popup");
        var siblingBranch = (NavMenuItem)child.ContainerFromIndex(1)!;
        Move(Header(siblingBranch));
        Wait(80);
        check(
            child.Submenu is { ItemCount: 1000 } &&
            ReferenceEquals(child.Submenu.ItemsSource, demo.Model.Roots[0].Entries[1].Entries),
            $"{label}: sibling branch opens its own thousand-item source");
        Bounded(child.Submenu!, check, label + "/sibling-branch");
        Move(Header(branch));
        Wait(80);
        grandchild = child.Submenu!;
        branch.Focus();
        Press(window, Key.Right, PhysicalKey.ArrowRight);
        check(grandchild.GetRealizedContainers().Any(row => row.IsFocused),
            $"{label}: Right enters an already hovered cascade");
        Press(window, Key.End, PhysicalKey.End);
        check(
            ReferenceEquals(menu.SelectedItem, demo.Model.Roots[0].Entries[0].Entries[999]) &&
            grandchild.ContainerFromIndex(999) is { IsEffectivelyVisible: true },
            $"{label}: End realizes the thousandth leaf and synchronizes root selection");
        Bounded(grandchild, check, label + "/distant");
        Press(window, Key.Left, PhysicalKey.ArrowLeft);
        check(menu.IsSubmenuOpen && !child.IsSubmenuOpen && branch.IsFocused,
            $"{label}: Left closes one level and returns focus to its anchor");
        Press(window, Key.Right, PhysicalKey.ArrowRight);
        check(child.IsSubmenuOpen, $"{label}: Right reopens the next level");
        Press(window, Key.Escape, PhysicalKey.Escape);
        check(menu.IsSubmenuOpen && !child.IsSubmenuOpen && branch.IsFocused,
            $"{label}: Escape returns exactly one level");
        Press(window, Key.Right, PhysicalKey.ArrowRight);
        grandchild = child.Submenu!;
        grandchild.SelectPath(999);
        Flush();
        ((NavMenuItem)grandchild.ContainerFromIndex(999)!).Focus();
        Press(window, Key.Enter, PhysicalKey.Enter);
        check(!menu.IsSubmenuOpen && demo.Model.Invocations == 1 && root.IsActiveAncestor,
            $"{label}: deepest invocation fires once, closes the entire chain and retains active root");
        window.MouseMove(new Point(1390, 770));
        Flush();
        Move(Header(root));
        Wait(80);
        var oldChild = menu.Submenu!;
        Move(Header((NavMenuItem)menu.ContainerFromIndex(1)!));
        Wait(80);
        check(
            menu.Submenu is { ItemCount: 1000 } &&
            ReferenceEquals(menu.Submenu.ItemsSource, demo.Model.Roots[1].Entries) &&
            !oldChild.IsAttachedToVisualTree() && root.IsActiveAncestor,
            $"{label}: moving to sibling icon replaces popup and preserves active page ancestor");
        Bounded(menu.Submenu!, check, label + "/sibling-icon");
        Move(Header(root));
        Wait(80);
        child = menu.Submenu!;
        branch = (NavMenuItem)child.ContainerFromIndex(0)!;
        Move(Header(branch));
        Wait(80);
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        ControlSizing.SetSize(window, size == ControlSize.Large ? ControlSize.Small : ControlSize.Large);
        Motion.SetEnabled(window, true);
        Flush();
        check(
            child.Submenu!.ActualThemeVariant == window.ActualThemeVariant &&
            child.Submenu.ItemHeight == menu.ItemHeight && Motion.GetEnabled(child.Submenu),
            $"{label}: live theme, density and motion propagate through open cascade");
        menu.HoverCloseDelay = TimeSpan.FromMilliseconds(300);
        Move(Header(child.Submenu!.GetRealizedContainers().First()));
        window.MouseMove(new Point(1390, 770));
        Wait(40);
        check(menu.IsSubmenuOpen, $"{label}: leaving the full chain retains it during configured grace period");
        Wait(350);
        check(!menu.IsSubmenuOpen && menu.Submenu is null,
            $"{label}: leaving the full chain releases all levels after delay");
        // Route both pointer events before rendering: a slow frame must not turn this
        // cancellation test into an already-open popup's separate close-delay test.
        Move(Header(root), false);
        Move(Header((NavMenuItem)menu.ContainerFromIndex(2)!), false);
        Wait(100);
        check(!menu.IsSubmenuOpen, $"{label}: fast pointer switching cancels stale pending opening");
        demo.Model.OpenOnHover = false;
        Flush();
        Move(Header(root));
        Wait(80);
        check(!menu.IsSubmenuOpen, $"{label}: compiled preview switch disables hover opening");
        menu.OpenSubmenu(0);
        Flush();
        check(menu.IsSubmenuOpen, $"{label}: disabling hover preserves explicit opening API");
        var explicitChild = menu.Submenu!;
        var explicitBranch = (NavMenuItem)explicitChild.ContainerFromIndex(0)!;
        var arrowButton = Part<ToggleButton>(explicitBranch, "PART_ExpandCollapseChevron");
        var arrowPoint =
            arrowButton.TranslatePoint(new Point(arrowButton.Bounds.Width / 2, arrowButton.Bounds.Height / 2), window)!
                .Value;
        window.MouseMove(arrowPoint);
        window.MouseDown(arrowPoint, MouseButton.Left);
        window.MouseUp(arrowPoint, MouseButton.Left);
        Flush();
        check(explicitChild.IsSubmenuOpen && !explicitBranch.IsExpanded,
            $"{label}: native chevron click opens cascade without changing model expansion");
        ((NavMenuItem)explicitChild.ContainerFromIndex(0)!).IsEnabled = false;
        Flush();
        check(menu.IsSubmenuOpen && !explicitChild.IsSubmenuOpen,
            $"{label}: disabling branch anchor immediately releases only its descendants");
        menu.CloseSubmenu();
        window.MouseMove(new Point(1390, 770));
        Flush();
        check(menu.SelectPath(0, 0, 999), $"{label}: synchronous API creates only the selected cascade path");
        Wait(40);
        var apiMenu = menu.Submenu ?? throw new InvalidOperationException("API path did not open its parent menu.");
        var apiParent = apiMenu.GetVisualAncestors().OfType<FlyoutPresenter>().First();
        var apiChild = (apiMenu.Submenu ?? throw new InvalidOperationException("API path did not open its child menu."))
            .GetVisualAncestors().OfType<FlyoutPresenter>().First();
        check(
            apiChild.TranslatePoint(default, window)!.Value.X >=
            apiParent.TranslatePoint(new Point(apiParent.Bounds.Width, 0), window)!.Value.X,
            $"{label}: API cascade follows ancestor overlay placement after layout");
        menu.CloseSubmenu();
        demo.Model.OpenOnHover = true;
        Flush();
        window.MouseMove(new Point(1390, 770));
        Move(Header(root), false);
        menu.ScrollIntoView(999);
        Flush();
        Wait(100);
        check(!menu.IsSubmenuOpen, $"{label}: recycled pending anchor cannot open a stale model");
        menu.ScrollIntoView(0);
        Flush();
        Move(Header((NavMenuItem)menu.ContainerFromIndex(0)!), false);
        window.Close();
        Wait(100);
        check(!menu.IsSubmenuOpen && menu.Submenu is null, $"{label}: detach cancels opening and closing timers");
    }

    private static Border Header(Control row)
    {
        return Part<Border>(row, "PART_LayoutRoot");
    }

    private static T Part<T>(Control row, string name) where T : Control
    {
        return row.GetVisualDescendants().OfType<T>().First(part => part.Name == name && part.TemplatedParent == row);
    }

    private static void Bounded(NavMenu menu, Action<bool, string> check, string label)
    {
        check(menu.ItemsPanelRoot is VirtualizingStackPanel && menu.GetRealizedContainers().Count() is > 0 and < 80,
            $"{label}: {menu.GetRealizedContainers().Count()}/1000 actual recycled containers");
    }

    private static void Move(Control control, bool render = true)
    {
        var host = TopLevel.GetTopLevel(control)!;
        host.MouseMove(control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!
            .Value);
        if (render) Flush();
    }

    private static void Press(TopLevel host, Key key, PhysicalKey physical)
    {
        host.KeyPress(key, RawInputModifiers.None, physical, null);
        host.KeyRelease(key, RawInputModifiers.None, physical, null);
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
    }
}