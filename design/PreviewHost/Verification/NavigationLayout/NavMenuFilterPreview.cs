using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavMenuFilterPreview
{
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private static void Wait(int milliseconds = 50)
    {
        var frame = new DispatcherFrame(); var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; }; timer.Start();
        try { Dispatcher.UIThread.PushFrame(frame); } finally { timer.Stop(); } Flush();
    }
    private static T Part<T>(Control parent, string name) where T : Control => parent.GetVisualDescendants().OfType<T>().First(control => control.Name == name);
    private static ListBox Suggestions(NavMenu menu) => Part<Popup>(menu.FilterBox!, "PART_Popup").Child!.GetVisualDescendants().OfType<ListBox>().Single();
    private static void Click(Control control)
    {
        var host = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
        host.MouseMove(point); host.MouseDown(point, MouseButton.Left); host.MouseUp(point, MouseButton.Left); Wait();
    }
    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var demo = new NavMenuDemoView(); var menu = demo.Menu; var model = demo.Model;
            var window = new Window { Width = 1120, Height = 1060, Content = demo, RequestedThemeVariant = theme };
            ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush(); Wait();
            var box = menu.FilterBox!; var input = Part<TextBox>(box, "PART_TextBox");
            var scroll = Part<ScrollViewer>(menu, "PART_MenuScrollViewer");
            var label = $"NavMenu/filter/{theme.Key}/{size}";
            void Key(Key key) { window.KeyPress(key, RawInputModifiers.None, PhysicalKey.None, null); window.KeyRelease(key, RawInputModifiers.None, PhysicalKey.None, null); Wait(); }
            check(menu.ShowFilter && box.IsVisible && scroll.Bounds.Y >= box.Bounds.Bottom && input.ActualThemeVariant == theme && ControlSizing.GetSize(input) == size, label + ": native autocomplete sits above the bounded menu with inherited theme and density");
            check(menu.FilterResults.Count == 4999 && menu.FilterResults.Any(result => ReferenceEquals(result.Item, model.Roots[0].Entries[0].Entries[999]) && result.Path.Contains("产品分类")), label + ": lightweight suggestions include original models at every level and exclude disabled nodes");
            menu.SelectPath(999); Flush();
            model.Roots[0].IsExpanded = false; model.Roots[0].Entries[0].IsExpanded = false;
            input.Focus(); window.KeyTextInput("三级页面"); Wait();
            box.IsDropDownOpen = true; Wait();
            var suggestions = Suggestions(menu);
            check(suggestions.ItemCount == 1000 && suggestions.ItemsPanel?.Build() is VirtualizingStackPanel && suggestions.GetRealizedContainers().Count() < 80, label + ": a thousand deep results use real recycled suggestion containers");
            Key(global::Avalonia.Input.Key.Down);
            check(input.IsFocused && box.Text == "三级页面" && box.SelectedItem is NavMenuFilterResult && !ReferenceEquals(menu.SelectedItem, ((NavMenuFilterResult)box.SelectedItem).Item), label + ": arrow navigation previews a suggestion without overwriting the query or committing navigation");
            suggestions.SelectedIndex = 999; suggestions.ScrollIntoView(999); Flush();
            check(suggestions.ContainerFromIndex(999) is not null && suggestions.ContainerFromIndex(0) is null && suggestions.GetRealizedContainers().Count() < 80, label + ": scrolling distant suggestions recycles the first container");
            Key(global::Avalonia.Input.Key.Enter);
            var target = model.Roots[0].Entries[0].Entries[999];
            check(ReferenceEquals(model.Selected, target) && model.Search == "" && !menu.IsSearching && ReferenceEquals(menu.ItemsSource, model.Roots) && !box.IsDropDownOpen, label + ": Enter exits projection and locates the original thousandth descendant");
            var root = (NavMenuItem)menu.ContainerFromIndex(0)!; var child = (NavMenuItem)root.ContainerFromIndex(0)!;
            check(root.IsExpanded && child.IsExpanded && child.ContainerFromIndex(999) is not null && child.GetRealizedContainers().Count() < 80 && input.IsFocused, label + ": navigation opens the ancestor path, retains input focus and creates a bounded number of tree containers");
            model.Search = "不存在"; Wait(); Key(global::Avalonia.Input.Key.Escape);
            check(model.Search == "" && ReferenceEquals(model.Selected, target), label + ": Esc restores the pre-query state without a suggestion commit");
            model.IsCollapsed = true; Wait();
            var button = Part<Button>(menu, "PART_FilterButton"); var popup = Part<Popup>(menu, "PART_FilterPopup");
            check(button.IsEffectivelyVisible && !box.IsEffectivelyVisible && !menu.IsFilterOpen && button.Bounds.Width == menu.ItemHeight && button.Bounds.Height == menu.ItemHeight,
                label + $": compact header shows only the correctly sized search icon (button={button.IsEffectivelyVisible}, input={box.IsEffectivelyVisible}, open={menu.IsFilterOpen}, bounds={button.Bounds}, row={menu.ItemHeight})");
            model.Search = "三级页面"; Wait();
            check(!menu.IsFilterOpen && !popup.IsOpen && !box.IsDropDownOpen, label + ": bound queries cannot open suggestions behind the closed search icon");
            model.Search = ""; Wait();
            Click(button);
            check(menu.IsFilterOpen && box.IsEffectivelyVisible && input.IsFocused && input.ActualThemeVariant == theme && ControlSizing.GetSize(input) == size && popup.Width <= window.ClientSize.Width - 16,
                label + ": real icon click opens and focuses the same compiled input with bounded size and inherited theme");
            model.Search = "标签页面 1000"; Wait(); box.IsDropDownOpen = true; Wait();
            suggestions = Suggestions(menu); suggestions.SelectedIndex = 0; suggestions.ScrollIntoView(0); Wait();
            var header = (Control)suggestions.ContainerFromIndex(0)!;
            var host = TopLevel.GetTopLevel(suggestions)!; var point = header.TranslatePoint(new Point(30, header.Bounds.Height / 2), host)!.Value;
            host.MouseMove(point); host.MouseDown(point, MouseButton.Left); host.MouseUp(point, MouseButton.Left); Wait();
            check(ReferenceEquals(model.Selected, model.Roots[0].Entries[1].Entries[999]) && menu.Submenu?.Submenu?.ContainerFromIndex(999) is not null && button.IsFocused && !menu.IsFilterOpen && !popup.IsOpen && !box.IsDropDownOpen,
                label + $": clicking a compact-mode result closes search, returns icon focus and locates the original deep cascade (selected={model.Selected?.Title}, query={model.Search}, search={menu.IsFilterOpen}, chain={menu.Submenu is not null}/{menu.Submenu?.Submenu is not null}, focus={button.IsFocused})");
            check(Part<Popup>(box, "PART_Popup").MinWidth >= 280 && menu.Bounds.Width == menu.CollapsedWidth, label + ": compact input retains a readable bounded suggestion popup");
            Click(button); window.KeyTextInput("三级页面"); Wait(); box.IsDropDownOpen = true; Wait(); suggestions = Suggestions(menu);
            check(menu.IsFilterOpen && suggestions.ItemCount == 1000 && suggestions.GetRealizedContainers().Count() < 80 && input.IsFocused,
                label + ": reopened compact search retains native virtualization for a thousand suggestions");
            Key(global::Avalonia.Input.Key.Down); suggestions.SelectedIndex = 999; suggestions.ScrollIntoView(999); Wait();
            check(suggestions.ContainerFromIndex(999) is not null && suggestions.ContainerFromIndex(0) is null && suggestions.GetRealizedContainers().Count() < 80,
                label + ": compact suggestion navigation recycles distant containers");
            Key(global::Avalonia.Input.Key.Enter);
            check(ReferenceEquals(model.Selected, target) && menu.Submenu?.Submenu?.ContainerFromIndex(999) is not null && !menu.IsFilterOpen && button.IsFocused && model.Search == "",
                label + ": compact Enter commits the thousandth result and closes search without dismissing its target cascade");
            Click(button); window.KeyTextInput("不存在"); Wait(); Key(global::Avalonia.Input.Key.Escape);
            check(!menu.IsFilterOpen && !popup.IsOpen && !box.IsDropDownOpen && model.Search == "" && ReferenceEquals(model.Selected, target) && button.IsFocused,
                label + ": compact Escape closes both popups, restores selection and returns icon focus");
            Key(global::Avalonia.Input.Key.Enter);
            check(menu.IsFilterOpen && input.IsFocused, label + ": Enter on the focused search icon opens and focuses compact search");
            Key(global::Avalonia.Input.Key.Escape);
            Click(button); window.KeyTextInput("标签页面"); Wait();
            window.MouseDown(new Point(window.ClientSize.Width - 20, window.ClientSize.Height - 20), MouseButton.Left);
            window.MouseUp(new Point(window.ClientSize.Width - 20, window.ClientSize.Height - 20), MouseButton.Left); Wait();
            check(!menu.IsFilterOpen && model.Search == "" && ReferenceEquals(model.Selected, target), label + ": outside click cancels compact search and restores the pre-query state");
            Click(button); var opposite = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
            window.RequestedThemeVariant = opposite; ControlSizing.SetSize(window, size == ControlSize.Large ? ControlSize.Small : ControlSize.Large); Wait();
            check(input.ActualThemeVariant == opposite && ControlSizing.GetSize(input) == ControlSizing.GetSize(window), label + ": open compact popup follows live theme and density changes");
            window.RequestedThemeVariant = theme; ControlSizing.SetSize(window, size); Wait();
            window.Width = 260; Wait();
            check(popup.Width <= window.ClientSize.Width - 16 && popup.Child!.Bounds.Width <= window.ClientSize.Width - 16,
                label + ": resizing the window constrains the open compact input");
            window.Width = 1120; Wait();
            model.ShowFilter = false; Wait();
            check(!menu.IsFilterOpen && !popup.IsOpen && !button.IsEffectivelyVisible && menu.FilterResults.Count == 0,
                label + ": hiding ShowFilter dismisses compact search and releases its index");
            model.ShowFilter = true; Wait(); Click(button); model.IsCollapsed = false; Wait();
            check(!menu.IsFilterOpen && ReferenceEquals(box, menu.FilterBox) && box.IsEffectivelyVisible && !button.IsEffectivelyVisible,
                label + ": expanding the sidebar dismisses compact search and restores the same inline input");
            menu.CloseSubmenu(); model.IsCollapsed = false;
            model.Roots[0].Entries[0].Entries[0].Title = "动态建议"; Wait();
            check(menu.FilterResults.Any(result => result.Label == "动态建议"), label + ": node text notifications refresh the deep suggestion index");
            model.Roots[0].Entries[0].Entries.Add(new NavEntry("新增建议")); Wait();
            check(menu.FilterResults.Any(result => result.Label == "新增建议"), label + ": collection notifications refresh an unexpanded descendant source");
            model.ShowFilter = false; Flush();
            check(!box.IsEffectivelyVisible && menu.FilterResults.Count == 0 && scroll.Bounds.Y == 0 && !box.IsDropDownOpen, label + ": ShowFilter reclaims header space and releases suggestion subscriptions");
            model.Roots[0].Entries[0].Entries[0].Title = "隐藏后更新"; model.ShowFilter = true; Wait();
            check(box.IsVisible && menu.FilterResults.Any(result => result.Label == "隐藏后更新"), label + ": re-enabling ShowFilter rebuilds from current source models");
            var replacement = new System.Collections.ObjectModel.ObservableCollection<NavEntry> { new("替换根菜单") }; menu.ItemsSource = replacement; Wait();
            check(menu.FilterResults.Count == 1 && ReferenceEquals(menu.FilterResults[0].Item, replacement[0]), label + ": replacing the root source removes stale suggestions");
            model.IsCollapsed = true; Wait(); menu.OpenFilter(); Wait(); window.Content = null; Wait();
            check(!menu.IsFilterOpen && !popup.IsOpen && !box.IsDropDownOpen, label + ": detaching the menu dismisses both search popups");
            window.Close(); Flush();
        }
    }
    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var demo = new NavMenuDemoView(); var window = new Window { Width = 1120, Height = 1060, Content = demo, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None };
            ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush();
            demo.Model.Search = "三级页面"; Wait(); demo.Menu.FilterBox!.Focus(); demo.Menu.FilterBox.IsDropDownOpen = true; Wait();
            using var bitmap = window.CaptureRenderedFrame()!; bitmap.Save(Path.Combine(output, $"navmenu-filter-{theme.Key}-{size}.png"), PngBitmapEncoderOptions.Default);
            demo.Model.Search = ""; demo.Model.IsCollapsed = true; Wait();
            using (var compact = window.CaptureRenderedFrame()!) compact.Save(Path.Combine(output, $"navmenu-filter-icon-{theme.Key}-{size}.png"), PngBitmapEncoderOptions.Default);
            Click(Part<Button>(demo.Menu, "PART_FilterButton")); demo.Model.Search = "三级页面"; Wait(); demo.Menu.FilterBox.IsDropDownOpen = true; Wait();
            using (var popup = window.CaptureRenderedFrame()!) popup.Save(Path.Combine(output, $"navmenu-filter-popup-{theme.Key}-{size}.png"), PngBitmapEncoderOptions.Default);
            window.Close(); Flush();
        }
    }
}
