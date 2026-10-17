using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavMenuCollapsedPreview
{
    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            Verify(theme, size, check);
        VerifyGroupsAndDismissal(check);
    }

    private static void Verify(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var demo = new NavMenuDemoView();
        var window = new Window
        {
            Width = 940, Height = 700, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = demo }
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        var menu = demo.Menu;
        var label = $"NavMenu/compact/{theme.Key}/{size}";
        Click(demo.FindControl<ToggleButton>("CompactToggle")!);
        Flush();
        var width = size switch { ControlSize.Small => 44, ControlSize.Large => 60, _ => 52 };
        check(menu.IsCollapsed && demo.Model.IsCollapsed && menu.Bounds.Width == width,
            $"{label}: compiled toggle switches to icon rail ({menu.Bounds.Width}/{width})");
        var root = (NavMenuItem)menu.ContainerFromIndex(0)!;
        check(root.IsExpanded && demo.Model.Roots[0].IsExpanded && !Part<RevealHost>(root, "TreeContent").IsVisible,
            $"{label}: inline children hide without changing model expansion");
        check(menu.GetRealizedContainers().Count() is > 0 and < 80 && menu.GetRealizedTreeContainers().Count() < 100,
            $"{label}: 1,000 compact roots remain bounded");
        var text = root.GetVisualDescendants().OfType<ContentPresenter>()
            .First(presenter => presenter.Name == "NavMenuContent");
        var icon = root.GetVisualDescendants().OfType<ContentPresenter>()
            .First(presenter => presenter.Name == "NavMenuIcon");
        check(!text.IsVisible && icon.IsVisible && !Part<Panel>(root, "PART_ExpandCollapseChevronContainer").IsVisible,
            $"{label}: rail shows icon only, without label or inline chevron");
        var header = Part<Border>(root, "PART_LayoutRoot");
        var iconCenter = icon.TranslatePoint(new Point(icon.Bounds.Width / 2, icon.Bounds.Height / 2), header)!.Value;
        check(Math.Abs(iconCenter.X - header.Bounds.Width / 2) < 1,
            $"{label}: icon remains centered ({iconCenter.X}/{header.Bounds.Width})");
        Click(header);
        Flush();
        check(
            menu.IsSubmenuOpen && menu.Submenu is { ItemCount: 1000 } &&
            ReferenceEquals(menu.Submenu.ItemsSource, demo.Model.Roots[0].Entries),
            $"{label}: icon opens children using typed Entries binding");
        var child = menu.Submenu!;
        var host = TopLevel.GetTopLevel(child)!;
        Bounded(child, check, label + "/popup");
        var presenter = child.GetVisualAncestors().OfType<FlyoutPresenter>().First();
        check(
            child.ActualThemeVariant == theme && child.FontSize == menu.FontSize &&
            child.ItemHeight == menu.ItemHeight && child.IconSize == menu.IconSize &&
            child.Bounds.Height <= menu.SubmenuMaxHeight &&
            Colour(presenter.Background) == Resource(menu, "One.Surface"),
            $"{label}: popup follows current theme, density and finite viewport");
        check(child.SelectPath(0, 999), $"{label}: popup selects an unrealized third-level leaf");
        Flush();
        var selected = demo.Model.Roots[0].Entries[0].Entries[999];
        check(ReferenceEquals(demo.Model.Selected, selected) && ReferenceEquals(menu.SelectedItem, selected),
            $"{label}: popup selection updates owner and compiled ViewModel once");
        Bounded(child, check, label + "/deep");
        var branch = (NavMenuItem)child.ContainerFromIndex(0)!;
        Bounded(child.Submenu!, check, label + "/grandchildren");
        check(!Part<RevealHost>(branch, "TreeContent").IsVisible, $"{label}: cascade does not expand inline children");
        check(root.IsActiveAncestor && Colour(header.Background) == Resource(menu, "One.AccentSoft"),
            $"{label}: selected descendant highlights the rail ancestor");
        var leaf = (NavMenuItem)child.Submenu!.TreeContainerFromItem(child.SelectedItem!)!;
        leaf.Focus();
        Press(host, Key.Enter, PhysicalKey.Enter);
        check(
            !menu.IsSubmenuOpen && menu.Submenu is null && demo.Model.Invocations == 1 &&
            ReferenceEquals(menu.SelectedItem, selected),
            $"{label}: Enter invokes once, closes popup and preserves selection");
        check(root.IsFocused, $"{label}: close restores focus to owning icon");
        check(menu.OpenSubmenu(0), $"{label}: popup can reopen");
        Flush();
        child = menu.Submenu!;
        check(!demo.Model.Roots[0].Entries[0].IsExpanded && ReferenceEquals(menu.SelectedItem, selected),
            $"{label}: reopening preserves model expansion and selected page");
        host = TopLevel.GetTopLevel(child)!;
        ((NavMenuItem)child.ContainerFromIndex(0)!).Focus();
        Press(host, Key.Escape, PhysicalKey.Escape);
        check(!menu.IsSubmenuOpen && demo.Model.Invocations == 1,
            $"{label}: Escape dismisses without another invocation");
        root.Focus();
        Press(window, Key.End, PhysicalKey.End);
        check(
            ReferenceEquals(menu.SelectedItem, demo.Model.Roots[999]) &&
            menu.ContainerFromIndex(999) is { IsEffectivelyVisible: true } && menu.GetRealizedContainers().Count() < 80,
            $"{label}: compact End realizes final root without walking hidden children");
        Press(window, Key.Up, PhysicalKey.ArrowUp);
        check(ReferenceEquals(menu.SelectedItem, demo.Model.Roots[998]),
            $"{label}: compact Up works at recycled boundary");
        Press(window, Key.Home, PhysicalKey.Home);
        root = (NavMenuItem)menu.ContainerFromIndex(0)!;
        root.Focus();
        Press(window, Key.Right, PhysicalKey.ArrowRight);
        check(menu.IsSubmenuOpen,
            $"{label}: Right opens submenu instead of expanding inline children (items={root.ItemCount}, focus={root.IsFocused}, data={((NavEntry?)menu.ItemFromContainer(root))?.Title}, selected={((NavEntry?)menu.SelectedItem)?.Title})");
        menu.CloseSubmenu();
        Flush();
        check(menu.SelectPath(0, 999) && ReferenceEquals(menu.SelectedItem, demo.Model.Roots[0].Entries[999]),
            $"{label}: public SelectPath realizes a distant popup row");
        Flush();
        child = menu.Submenu!;
        Bounded(child, check, label + "/distant");
        var distant = (NavMenuItem)child.ContainerFromIndex(999)!;
        Click(Part<Border>(distant, "PART_LayoutRoot"));
        check(
            demo.Model.Invocations == 2 && !menu.IsSubmenuOpen &&
            ReferenceEquals(demo.Model.Selected, demo.Model.Roots[0].Entries[999]),
            $"{label}: clicking distant leaf invokes owner exactly once");
        menu.OpenSubmenu(0);
        Flush();
        child = menu.Submenu!;
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        ControlSizing.SetSize(window, size == ControlSize.Large ? ControlSize.Small : ControlSize.Large);
        Flush();
        check(
            child.ActualThemeVariant == window.ActualThemeVariant && child.ItemHeight == menu.ItemHeight &&
            child.IconSize == menu.IconSize && child.FontSize == menu.FontSize,
            $"{label}: open popup updates live theme and density");
        Motion.SetEnabled(window, true);
        Flush();
        check(Motion.GetEnabled(child), $"{label}: popup inherits re-enabled motion");
        Motion.SetEnabled(window, false);
        Flush();
        check(!Motion.GetEnabled(child), $"{label}: popup follows motion disabled while open");
        demo.Model.IsCollapsed = false;
        Flush();
        check(!menu.IsSubmenuOpen && menu.Submenu is null && !menu.IsCollapsed && demo.Model.Roots[0].IsExpanded,
            $"{label}: expanding rail closes popup and restores inline model state");
        demo.Model.IsCollapsed = true;
        Flush();
        menu.OpenSubmenu(0);
        Flush();
        menu.IsEnabled = false;
        Flush();
        check(!menu.IsSubmenuOpen && !menu.OpenSubmenu(0), $"{label}: disabling owner closes and rejects popup");
        menu.IsEnabled = true;
        menu.OpenSubmenu(0);
        Flush();
        menu.ScrollIntoView(999);
        Flush();
        check(!menu.IsSubmenuOpen, $"{label}: recycling placement target safely closes popup");
        menu.ScrollIntoView(0);
        Flush();
        menu.OpenSubmenu(0);
        Flush();
        window.Close();
        Flush();
        check(!menu.IsSubmenuOpen && menu.Submenu is null, $"{label}: detaching owner releases popup and its bindings");
    }

    private static void VerifyGroupsAndDismissal(Action<bool, string> check)
    {
        var group = new NavEntry("分组", isGroup: true);
        for (var i = 0; i < 1000; i++) group.Entries.Add(new NavEntry($"页面 {i + 1}"));
        var menu = new NavMenu
        {
            IsCollapsed = true, ItemsSource = new[] { group },
            ChildrenBinding = CompiledBinding.Create<NavEntry, ObservableCollection<NavEntry>>(entry => entry.Entries),
            ExpandedBinding =
                CompiledBinding.Create<NavEntry, bool>(entry => entry.IsExpanded, mode: BindingMode.TwoWay),
            IsGroupBinding = CompiledBinding.Create<NavEntry, bool>(entry => entry.IsGroup),
            ContentTemplate = new FuncDataTemplate<NavEntry>((entry, _) => new TextBlock { Text = entry!.Title })
        };
        var holder = new Border { Child = menu };
        var window = new Window { Width = 600, Height = 480, Content = holder };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        var root = (NavMenuItem)menu.ContainerFromIndex(0)!;
        var icon = root.GetVisualDescendants().OfType<ContentPresenter>()
            .First(presenter => presenter.Name == "NavMenuIcon");
        check(icon.IsVisible && icon.GetVisualDescendants().OfType<PathIcon>().Any(),
            "NavMenu/compact/fallback: missing IconTemplate uses a glyph instead of visible text");
        Click(Part<Border>(root, "PART_LayoutRoot"));
        Flush();
        check(menu.IsSubmenuOpen && menu.SelectedItem is null && group.IsExpanded,
            "NavMenu/compact/group: icon opens group without changing selection or expansion");
        Bounded(menu.Submenu!, check, "NavMenu/compact/group");
        group.Entries.Add(new NavEntry("动态尾项"));
        Flush();
        check(
            menu.Submenu!.ItemCount == 1001 && menu.Submenu.SelectPath(1000) &&
            ((NavEntry?)menu.SelectedItem)?.Title == "动态尾项",
            "NavMenu/compact/source: observable children update live and distant selection synchronizes owner");
        check(
            root.IsActiveAncestor && Colour(Part<ContentPresenter>(root, "PART_HeaderPresenter").Foreground) ==
            Resource(menu, "One.Accent"), "NavMenu/compact/group: active group uses semantic accent icon");
        window.MouseDown(new Point(590, 470), MouseButton.Left);
        window.MouseUp(new Point(590, 470), MouseButton.Left);
        Flush();
        check(!menu.IsSubmenuOpen, "NavMenu/compact/dismiss: pointer outside overlay closes submenu");
        menu.OpenSubmenu(0);
        Flush();
        holder.IsEnabled = false;
        Flush();
        check(!menu.IsSubmenuOpen && !menu.OpenSubmenu(0),
            "NavMenu/compact/ancestor: disabling parent closes popup and rejects reopening");
        holder.IsEnabled = true;
        menu.OpenSubmenu(0);
        Flush();
        menu.ItemsSource = Array.Empty<NavEntry>();
        Flush();
        check(!menu.IsSubmenuOpen && menu.Submenu is null,
            "NavMenu/compact/source: replacing root source releases popup");
        window.Close();
        var branch = new NavEntry("子级");
        for (var i = 0; i < 1000; i++) branch.Entries.Add(new NavEntry($"叶子 {i}"));
        group.Entries.Clear();
        group.Entries.Add(branch);
        menu = new NavMenu
        {
            IsCollapsed = true, ItemsSource = new[] { group },
            ChildrenBinding = CompiledBinding.Create<NavEntry, ObservableCollection<NavEntry>>(entry => entry.Entries)
        };
        window = new Window { Width = 600, Height = 480, Content = menu };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        menu.SelectPath(0, 0, 999);
        Flush();
        menu.CloseSubmenu();
        Flush();
        menu.OpenSubmenu(0);
        Flush();
        check(!((NavMenuItem)menu.Submenu!.ContainerFromIndex(0)!).IsExpanded && menu.Submenu.SelectPath(0, 999),
            "NavMenu/compact/state: without ExpandedBinding, cascade navigation preserves original expansion through popup recycling");
        window.Close();
    }

    private static void Bounded(ItemsControl menu, Action<bool, string> check, string label)
    {
        var count = menu.GetRealizedContainers().Count();
        check(menu.ItemsPanelRoot is VirtualizingStackPanel && count is > 0 and < 80,
            $"{label}: {count}/{menu.ItemCount} actual containers");
    }

    private static T Part<T>(Control control, string name) where T : Control
    {
        return control.GetVisualDescendants().OfType<T>()
            .First(part => part.Name == name && part.TemplatedParent == control);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void Click(Control control)
    {
        var host = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
        host.MouseMove(point);
        host.MouseDown(point, MouseButton.Left);
        host.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static void Press(TopLevel host, Key key, PhysicalKey physical)
    {
        host.KeyPress(key, RawInputModifiers.None, physical, null);
        host.KeyRelease(key, RawInputModifiers.None, physical, null);
        Flush();
    }

    private static Color? Colour(IBrush? brush)
    {
        return (brush as ISolidColorBrush)?.Color;
    }

    private static Color? Resource(Control control, string key)
    {
        return control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
    }
}