using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.Media.Imaging;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavMenuSearchPreview
{
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }

    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var demo = new NavMenuDemoView();
            var menu = demo.Menu;
            var model = demo.Model;
            var window = new Window { Width = 1120, Height = 1060, Content = demo, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None };
            Motion.SetEnabled(window, false); ControlSizing.SetSize(window, size); window.Show(); Flush();
            var label = $"NavMenu/search/{theme.Key}/{size}";
            model.Roots[0].IsExpanded = false; model.Roots[0].Entries[0].IsExpanded = false;
            menu.SelectPath(999); Flush(); var originalSelection = model.Selected;
            model.Search = "三级页面 1000"; Flush();
            check(menu.IsSearching && menu.SearchMatchCount == 1 && menu.ItemCount == 1, label + ": compiled search finds an unrealized thousandth descendant");
            var parent = (NavMenuItem)menu.ContainerFromIndex(0)!;
            var child = (NavMenuItem)parent.ContainerFromIndex(0)!;
            check(parent.IsExpanded && child.IsExpanded && child.ItemCount == 1 && ReferenceEquals(child.Items[0], model.Roots[0].Entries[0].Entries[999]), label + ": matching descendant shows and expands its original parent path");
            check(!model.Roots[0].IsExpanded && !model.Roots[0].Entries[0].IsExpanded && model.Roots.Count == 1000 && model.Roots[0].Entries.Count == 1000, label + ": projection leaves source collections and model expansion unchanged");
            check(menu.SelectPath(0, 0, 0) && ReferenceEquals(model.Selected, model.Roots[0].Entries[0].Entries[999]), label + ": filtered selection remains an original model with two-way binding");
            model.Search = "三级页面"; Flush();
            check(ReferenceEquals(model.Selected, model.Roots[0].Entries[0].Entries[999]), label + ": a changed query preserves a still-matching selection");
            model.Search = ""; Flush();
            check(!menu.IsSearching && ReferenceEquals(menu.ItemsSource, model.Roots) && ReferenceEquals(model.Selected, originalSelection) && !model.Roots[0].IsExpanded, label + ": clearing restores original source, selection and expansion");
            model.Search = "不存在的页面"; Flush(); check(menu.ItemCount == 0 && menu.SearchMatchCount == 0, label + ": no matches yields an empty virtualized view");
            model.Search = "三级页面"; Flush();
            check(menu.SearchMatchCount == 1000 && menu.SelectPath(0, 0, 999), label + ": broad query keeps a thousand child models and supports distant selection");
            parent = (NavMenuItem)menu.ContainerFromIndex(0)!; child = (NavMenuItem)parent.ContainerFromIndex(0)!;
            check(child.GetRealizedContainers().Count() < 80 && child.ContainerFromIndex(999) is not null && child.ContainerFromIndex(0) is null, label + ": distant operation recycles actual containers in bounded child viewport");
            var host = TopLevel.GetTopLevel(menu)!;
            child.ContainerFromIndex(999)!.Focus(); host.KeyPress(Key.Up, RawInputModifiers.None, PhysicalKey.ArrowUp, null); host.KeyRelease(Key.Up, RawInputModifiers.None, PhysicalKey.ArrowUp, null); Flush();
            check(ReferenceEquals(menu.SelectedItem, model.Roots[0].Entries[0].Entries[998]), label + ": keyboard navigation reaches an uncreated filtered child");
            model.Roots[0].Entries[0].Entries[0].Title = "动态匹配"; model.Search = "动态匹配"; Flush();
            check(menu.SearchMatchCount == 1, label + ": model text updates are searchable");
            model.Roots[0].Entries[0].Entries[0].Title = "已改名"; Flush(); check(menu.SearchMatchCount == 0, label + ": changing model text rebuilds active results");
            model.Roots[0].Entries[0].Entries.Add(new NavEntry("动态匹配新增")); Flush(); check(menu.SearchMatchCount == 1, label + ": changing an unexpanded child collection refreshes results");
            model.Search = " "; Flush(); check(menu.ItemCount == 1000 && ReferenceEquals(model.Selected, originalSelection), label + ": whitespace clears and restores the first search snapshot");
            model.IsCollapsed = true; model.Search = "标签页面 1000"; Flush();
            check(menu.OpenSubmenu(0) && menu.Submenu?.OpenSubmenu(0) == true && menu.Submenu.Submenu?.ItemCount == 1, label + ": compact cascade contains only matching ancestor path");
            check(menu.Submenu!.Submenu!.SelectPath(0) && ReferenceEquals(model.Selected, model.Roots[0].Entries[1].Entries[999]), label + ": compact search selects original leaf");
            model.ClearSearchCommand.Execute(null); Flush();
            check(!menu.IsSubmenuOpen && ReferenceEquals(model.Selected, originalSelection) && menu.ItemCount == 1000, label + ": clearing compact search releases the chain and restores selection");
            model.IsCollapsed = false; menu.ExpandedBinding = null; Flush();
            menu.SelectPath(0, 1, 999); Flush();
            var cachedParent = (NavMenuItem)menu.ContainerFromIndex(0)!;
            check(cachedParent.IsExpanded, label + ": unbound expansion is cached by model identity");
            var cachedSelection = menu.SelectedItem;
            model.Search = "三级页面 1000"; Flush(); model.Search = ""; Flush();
            cachedParent = (NavMenuItem)menu.ContainerFromIndex(0)!;
            check(cachedParent.IsExpanded && cachedParent.ContainerFromIndex(1) is NavMenuItem { IsExpanded: true } && ReferenceEquals(menu.SelectedItem, cachedSelection), label + ": clearing restores unbound expansion and selection across recycled paths");
            // The demo binds a single selected item; exercise the cache's aggregate
            // traversal without echoing that single-item binding into a multi-selection.
            BindingOperations.GetBindingExpressionBase(menu, TreeView.SelectedItemProperty)?.Dispose();
            menu.ClearValue(TreeView.SelectedItemProperty);
            menu.SelectionMode = SelectionMode.Multiple;
            Virtualization.SelectAll(menu);
            check(menu.SelectedItems.Count == 3000 && menu.SelectedItems.Contains(model.Roots[0].Entries[999]) && menu.SelectedItems.Contains(model.Roots[0].Entries[1].Entries[999]), $"{label}: restored expansion cache traverses full uncreated child collections (count={menu.SelectedItems.Count}, bound={BindingOperations.GetBindingExpressionBase(menu, TreeView.SelectedItemProperty) is not null}, mode={menu.SelectionMode}, child={menu.SelectedItems.Contains(model.Roots[0].Entries[999])}, grand={menu.SelectedItems.Contains(model.Roots[0].Entries[1].Entries[999])})");
            menu.SelectionMode = SelectionMode.Single; Flush();
            model.Roots[999].Title = "Still Terminal"; model.Search = "still TERMINAL"; Flush();
            check(menu.SearchMatchCount == 1 && ReferenceEquals(menu.Items[0], model.Roots[999]), label + ": ordinal case-insensitive matching supports distant root data");
            model.Search = "no match"; Flush();
            model.Roots.Add(new NavEntry("no match added")); Flush(); check(menu.ItemCount == 1, label + ": root collection changes refresh an active query");
            window.Close(); Flush();
        }
    }

    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var demo = new NavMenuDemoView(); demo.Model.Search = "三级页面 1000";
            var window = new Window { Width = 1120, Height = 1060, Content = demo, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None };
            Motion.SetEnabled(window, false); ControlSizing.SetSize(window, size); window.Show(); Flush();
            using var bitmap = window.CaptureRenderedFrame()!; bitmap.Save(Path.Combine(output, $"navmenu-search-{theme.Key}-{size}.png"), PngBitmapEncoderOptions.Default); window.Close();
        }
    }
}
