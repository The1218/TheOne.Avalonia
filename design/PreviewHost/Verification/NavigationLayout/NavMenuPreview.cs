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
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using Path = Avalonia.Controls.Shapes.Path;

namespace TheOne.DesignPreview;

internal static class NavMenuPreview
{
    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyConsumer(theme, size, check);
        VerifyGroupsAndTemplates(check);
        NavMenuCollapsedPreview.Verify(check);
        NavMenuCascadePreview.Verify(check);
        NavMenuEdgePreview.Verify(check);
        NavMenuSearchPreview.Verify(check);
        NavMenuSearchInteractionPreview.Verify(check);
        NavMenuMotionPreview.Verify(check);
        NavMenuFilterPreview.Verify(check);
    }

    private static Window Open(Control content, ThemeVariant theme, ControlSize size, double width = 820,
        double height = 1060)
    {
        var window = new Window
        {
            Width = width, Height = height, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = content }
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    private static void VerifyConsumer(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var demo = new NavMenuDemoView();
        demo.Model.ShowFilter = false;
        var window = Open(demo, theme, size);
        var menu = demo.Menu;
        var label = $"NavMenu/{theme.Key}/{size}";
        check(menu.ChildrenBinding is CompiledBinding && menu.ExpandedBinding is CompiledBinding &&
              menu.SearchTextBinding is CompiledBinding && menu.IsGroupBinding is CompiledBinding &&
              menu.IsItemEnabledBinding is CompiledBinding,
            $"{label}: Binding shorthand stores compiled node bindings inferred from ItemsSource");
        var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
        var iconSize = size switch { ControlSize.Small => 16, ControlSize.Large => 20, _ => 18 };
        var root = (NavMenuItem)menu.ContainerFromIndex(0)!;
        check(root.IsExpanded && root.ItemCount == 1000 && root.ItemsSource == demo.Model.Roots[0].Entries,
            $"{label}: typed ChildrenBinding uses Entries, with two-way expanded state");
        Bounded(menu, check, label + "/root");
        Bounded(root, check, label + "/children");
        var header = Part<Border>(root, "PART_LayoutRoot");
        check(
            header.Bounds.Height == height && root.FontSize == menu.FontSize &&
            Part<ToggleButton>(root, "PART_ExpandCollapseChevron").Bounds.Height == height,
            $"{label}: row and chevron follow density ({header.Bounds.Height}/{height})");
        check(
            root.GetVisualDescendants().OfType<ContentPresenter>().Any(p =>
                p.Name == "NavMenuIcon" && p.Bounds.Width == iconSize && p.Bounds.Height == iconSize),
            $"{label}: icon template has current density");
        check(root.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == "产品管理"),
            $"{label}: typed ContentTemplate renders title");
        Click(window, demo.FindControl<Button>("JumpDeep")!);
        check(ReferenceEquals(menu.SelectedItem, demo.Model.Roots[0].Entries[0].Entries[999]),
            $"{label}: jump button selects a not-yet-created third-level page");
        Flush();
        check(ReferenceEquals(demo.Model.Selected, demo.Model.Roots[0].Entries[0].Entries[999]),
            $"{label}: two-way SelectedItem writes the selected model");
        root = (NavMenuItem)menu.ContainerFromIndex(0)!;
        var child = (NavMenuItem)root.ContainerFromIndex(0)!;
        Bounded(menu, check, label + "/deep/root");
        Bounded(root, check, label + "/deep/children");
        Bounded(child, check, label + "/deep/grandchildren");
        var selected = (NavMenuItem)menu.TreeContainerFromItem(menu.SelectedItem!)!;
        check(
            selected.IsSelected && Colour(Part<Border>(selected, "PART_LayoutRoot").Background) ==
            Resource(menu, "One.AccentSoft"), $"{label}: selected page uses semantic selected surface");
        selected.Focus();
        Press(window, Key.Enter, PhysicalKey.Enter);
        check(demo.Model.Invocations == 1, $"{label}: Enter invokes the selected page");
        check(menu.SelectPath(999), $"{label}: navigate to distant root");
        Flush();
        Bounded(menu, check, label + "/distant");
        check(ReferenceEquals(demo.Model.Selected, demo.Model.Roots[999]) && menu.ContainerFromIndex(0) is null,
            $"{label}: distant navigation recycles offscreen ancestors");
        var last = (NavMenuItem)menu.ContainerFromIndex(999)!;
        last.Focus();
        Press(window, Key.Up, PhysicalKey.ArrowUp);
        check(ReferenceEquals(menu.SelectedItem, demo.Model.Roots[998]),
            $"{label}: keyboard realizes adjacent uncreated page");
        Press(window, Key.Home, PhysicalKey.Home);
        Flush();
        check(
            ReferenceEquals(menu.SelectedItem, demo.Model.Roots[0]) &&
            ((NavMenuItem)menu.ContainerFromIndex(0)!).IsExpanded && demo.Model.Roots[0].Entries[0].IsExpanded,
            $"{label}: Home returns to root and preserves recycled expansion");
        Press(window, Key.End, PhysicalKey.End);
        check(ReferenceEquals(menu.SelectedItem, demo.Model.Roots[999]), $"{label}: End reaches the final model");
        check(!menu.SelectPath(0, 8), $"{label}: disabled page cannot be selected through SelectPath");
        root = (NavMenuItem)menu.ContainerFromIndex(0)!;
        var disabled = (NavMenuItem)root.ContainerFromIndex(8)!;
        var old = menu.SelectedItem;
        Click(window, Part<Border>(disabled, "PART_LayoutRoot"));
        check(!disabled.IsEffectivelyEnabled && menu.SelectedItem == old && demo.Model.Invocations == 1,
            $"{label}: disabled pointer interaction does not invoke or navigate");
        demo.Model.Roots[0].Entries.Insert(9, new NavEntry("动态页面"));
        Flush();
        check(menu.SelectPath(0, 9) && demo.Model.Selected?.Title == "动态页面",
            $"{label}: observable children insert without rebuilding controls");
        demo.Model.Selected!.Title = "动态页面 · 已更新";
        Flush();
        check(
            ((NavMenuItem)menu.TreeContainerFromItem(menu.SelectedItem!)!).GetVisualDescendants().OfType<TextBlock>()
            .Any(text => text.Text == "动态页面 · 已更新"), $"{label}: compiled template follows model property updates");
        var target = (NavMenuItem)menu.TreeContainerFromItem(menu.SelectedItem!)!;
        var point = Part<Border>(target, "PART_LayoutRoot").TranslatePoint(new Point(80, height / 2), window)!.Value;
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
        check(demo.Model.Invocations == 2, $"{label}: pointer invokes a page once");
        window.MouseUp(point, MouseButton.Left);
        Flush();
        check(demo.Model.Invocations == 2, $"{label}: release without matching press does not invoke");
        window.MouseMove(new Point(1, 1));
        menu.SelectedItem = null;
        Flush();
        check(Colour(Part<Border>(target, "PART_LayoutRoot").Background) == Resource(menu, "One.Transparent"),
            $"{label}: normal row has transparent surface");
        window.MouseMove(point);
        Flush();
        check(Colour(Part<Border>(target, "PART_LayoutRoot").Background) == Resource(menu, "One.Subtle"),
            $"{label}: hover uses semantic subtle surface");
        menu.IsEnabled = false;
        Flush();
        check(
            Colour(Part<ContentPresenter>(target, "PART_HeaderPresenter").Foreground) ==
            Resource(menu, "One.DisabledText"), $"{label}: disabled label follows semantic text colour");
        menu.IsEnabled = true;
        foreach (var next in new[] { ControlSize.Small, ControlSize.Large, ControlSize.Normal })
        {
            ControlSizing.SetSize(window, next);
            check(menu.SelectPath(0, 9), $"{label}: path selection remains valid while changing density to {next}");
            Flush();
            target = (NavMenuItem)menu.TreeContainerFromItem(menu.SelectedItem!)!;
            var expected = next switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
            check(
                Part<Border>(target, "PART_LayoutRoot").Bounds.Height == expected &&
                Part<ToggleButton>(target, "PART_ExpandCollapseChevron").Height == expected,
                $"{label}: live density updates the visible row to {next} ({Part<Border>(target, "PART_LayoutRoot").Bounds.Height}/{expected})");
        }

        menu.SelectedItem = null;
        point = Part<Border>(target, "PART_LayoutRoot").TranslatePoint(new Point(80, 18), window)!.Value;
        window.MouseMove(point);
        Flush();
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        Flush();
        check(Colour(Part<Border>(target, "PART_LayoutRoot").Background) == Resource(menu, "One.Subtle"),
            $"{label}: live theme switch updates hovered row");
        window.Close();
    }

    private static void VerifyGroupsAndTemplates(Action<bool, string> check)
    {
        var roots = PreviewNavigation.Create();
        var menu = new NavMenu
        {
            ItemsSource = roots,
            ChildrenBinding = CompiledBinding.Create<NavEntry, ObservableCollection<NavEntry>>(entry => entry.Entries),
            ExpandedBinding =
                CompiledBinding.Create<NavEntry, bool>(entry => entry.IsExpanded, mode: BindingMode.TwoWay),
            IsGroupBinding = CompiledBinding.Create<NavEntry, bool>(entry => entry.IsGroup),
            ContentTemplate = new FuncDataTemplate<NavEntry>((entry, _) => new TextBlock { Text = entry!.Title }),
            IconTemplate = new FuncDataTemplate<NavEntry>((entry, _) => new PathIcon { Data = entry!.Icon })
        };
        var window = Open(menu, ThemeVariant.Light, ControlSize.Normal, 340, 680);
        menu.SelectPath(1, 0);
        Flush();
        var previous = menu.SelectedItem;
        var group = (NavMenuItem)menu.ContainerFromIndex(1)!;
        Click(window, Part<Border>(group, "PART_LayoutRoot"));
        check(!group.IsExpanded && !roots[1].IsExpanded && menu.SelectedItem == previous,
            "NavMenu/groups: pointer collapses section without navigating away from page");
        group.Focus();
        Press(window, Key.Enter, PhysicalKey.Enter);
        check(group.IsExpanded && roots[1].IsExpanded && menu.SelectedItem == previous,
            "NavMenu/groups: keyboard expands section without navigating");
        var changes = 0;
        menu.SelectionChanged += (_, _) => changes++;
        menu.SelectedItem = roots[1];
        Flush();
        check(menu.SelectedItem == previous && changes == 0,
            "NavMenu/groups: section selection is rejected without a page-change event");
        var invocations = 0;
        menu.ItemInvokedCommand = ReactiveCommand.Create<object?>(value =>
        {
            if (value == roots[1].Entries[0]) invocations++;
        });
        menu.SelectPath(1, 0);
        Flush();
        var leaf = (NavMenuItem)menu.TreeContainerFromItem(menu.SelectedItem!)!;
        leaf.Focus();
        Press(window, Key.Space, PhysicalKey.Space);
        check(invocations == 1, "NavMenu/command: selected data is passed to ICommand");
        menu.IconTemplate = null;
        Flush();
        check(!leaf.GetVisualDescendants().OfType<ContentPresenter>().First(p => p.Name == "NavMenuIcon").IsVisible,
            "NavMenu/templates: removing IconTemplate removes icon and gap live");
        menu.ContentTemplate =
            new FuncDataTemplate<NavEntry>((entry, _) => new TextBlock { Text = "新模板 / " + entry!.Title });
        Flush();
        check(leaf.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == "新模板 / 按钮与操作"),
            "NavMenu/templates: ContentTemplate replaces realized labels live");
        menu.ContentTemplate = new FuncDataTemplate<NavEntry>((entry, _) => new Button { Content = "内嵌操作" });
        Flush();
        var embedded = leaf.GetVisualDescendants().OfType<Button>().First(button => button is not ToggleButton);
        Click(window, embedded);
        embedded.Focus();
        Press(window, Key.Enter, PhysicalKey.Enter);
        check(invocations == 1,
            "NavMenu/templates: embedded button keeps pointer and keyboard interaction without invoking menu");
        group.Focus();
        Motion.SetEnabled(window, true);
        roots[1].IsExpanded = false;
        Flush();
        var reveal = Part<RevealHost>(group, "TreeContent");
        var chevronPath = Part<ToggleButton>(group, "PART_ExpandCollapseChevron").GetVisualDescendants().OfType<Path>()
            .First();
        check(chevronPath.Transitions is { Count: > 0 }, "NavMenu/motion: enabled chevron retains native transition");
        Motion.SetEnabled(window, false);
        Flush();
        var groupHeader = Part<Border>(group, "PART_LayoutRoot");
        check(
            group.Bounds.Height == groupHeader.Bounds.Height + groupHeader.Margin.Top + groupHeader.Margin.Bottom &&
            !reveal.IsVisible && chevronPath.Transitions is null,
            $"NavMenu/motion: disabling motion leaves only the header and its item spacing, with no chevron animation (row={group.Bounds.Height}, header={groupHeader.Bounds.Height}, margin={groupHeader.Margin}, visible={reveal.IsVisible}, transitions={chevronPath.Transitions?.Count})");
        Motion.SetEnabled(window, true);
        roots[1].IsExpanded = true;
        Advance(280);
        Flush();
        check(reveal.IsVisible && reveal.Bounds.Height > 0 && chevronPath.Transitions is { Count: > 0 },
            "NavMenu/motion: re-enabling restores expansion and chevron transitions");
        Motion.SetEnabled(window, false);
        menu.ChildrenBinding = null;
        Flush();
        check(menu.GetRealizedContainers().OfType<NavMenuItem>().All(row => row.ItemCount == 0),
            "NavMenu/source: changing child binding resets recycled child sources");
        window.Close();
    }

    public static void Render(string output)
    {
        foreach (var collapsed in new[] { false, true })
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var roots = PreviewNavigation.Create();
            foreach (var group in roots.Where(entry => entry.IsGroup)) group.IsExpanded = false;
            var demo = new NavMenuDemoView();
            var menu = new NavMenu
            {
                IsCollapsed = collapsed, ItemsSource = roots,
                ChildrenBinding =
                    CompiledBinding.Create<NavEntry, ObservableCollection<NavEntry>>(node => node.Entries),
                ExpandedBinding =
                    CompiledBinding.Create<NavEntry, bool>(node => node.IsExpanded, mode: BindingMode.TwoWay),
                IsGroupBinding = CompiledBinding.Create<NavEntry, bool>(node => node.IsGroup),
                ContentTemplate = demo.Menu.ContentTemplate, IconTemplate = demo.Menu.IconTemplate
            };
            var grid = new Grid
            {
                ColumnDefinitions = ColumnDefinitions.Parse(collapsed ? "68,24,*" : "260,24,*"),
                Children = { menu, demo }
            };
            Grid.SetColumn(demo, 2);
            var window = Open(grid, theme, size, 1110, 780);
            demo.Model.IsCollapsed = collapsed;
            menu.SelectPath(5, 5);
            menu.CloseSubmenu();
            demo.Menu.SelectPath(0, 0, 6);
            demo.UpdateMetrics();
            Flush();
            Flush();
            Flush();
            using var frame = window.CaptureRenderedFrame()!;
            var path = System.IO.Path.Combine(output,
                $"navmenu-{(collapsed ? "collapsed" : "expanded")}-{theme.Key}-{size}.png");
            frame.Save(path, PngBitmapEncoderOptions.Default);
            Console.WriteLine($"Rendered {path}");
            window.Close();
        }
    }

    private static void Bounded(ItemsControl menu, Action<bool, string> check, string label)
    {
        var count = menu.GetRealizedContainers().Count();
        check(menu.ItemsPanelRoot is VirtualizingStackPanel && count is > 0 and < 100,
            $"{label}: {count}/{menu.ItemCount} actual recycled containers");
    }

    private static T Part<T>(Control row, string name) where T : Control
    {
        return row.GetVisualDescendants().OfType<T>()
            .First(value => value.Name == name && value.TemplatedParent == row);
    }

    private static Color? Colour(IBrush? brush)
    {
        return (brush as ISolidColorBrush)?.Color;
    }

    private static Color? Resource(Control control, string key)
    {
        return control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
    }

    private static void Advance(int milliseconds)
    {
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            frame.Continue = false;
        };
        timer.Start();
        Dispatcher.UIThread.PushFrame(frame);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void Press(TopLevel window, Key key, PhysicalKey physical)
    {
        window.KeyPress(key, RawInputModifiers.None, physical, null);
        window.KeyRelease(key, RawInputModifiers.None, physical, null);
        Flush();
    }

    private static void Click(Window window, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!
            .Value;
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
    }
}