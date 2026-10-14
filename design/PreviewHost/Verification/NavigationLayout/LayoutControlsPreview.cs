using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class LayoutControlsPreview
{
    public static void Verify()
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) VerifyDensity(theme, size);
        VerifyPaneModes(); VerifyResizing(); VerifyConsumer();
    }
    private static (Window Window, LayoutDemoView Demo) Open(ThemeVariant theme, ControlSize size, int width = 1120)
    {
        var demo = new LayoutDemoView();
        var window = new Window { Width = width, Height = 900, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = demo } };
        Motion.SetEnabled(window, false); ControlSizing.SetSize(window, size); window.Show(); ControlPreview.Flush();
        return (window, demo);
    }
    private static void VerifyDensity(ThemeVariant theme, ControlSize size)
    {
        var (window, demo) = Open(theme, size);
        var split = demo.FindControl<SplitView>("PaneSample")!;
        var column = demo.FindControl<GridSplitter>("ColumnSplitter")!;
        var row = demo.FindControl<GridSplitter>("RowSplitter")!;
        var paneList = demo.FindControl<ListBox>("PaneList")!;
        var (opened, compact, thickness) = size switch { ControlSize.Small => (220, 48, 8), ControlSize.Large => (280, 64, 12), _ => (248, 56, 10) };
        var label = $"Layout/{theme.Key}/{size}";
        Check(split.OpenPaneLength == opened && split.CompactPaneLength == compact && Near(split.Bounds.Height, 320) && split.Padding == new Thickness(size switch { ControlSize.Small => 12, ControlSize.Large => 20, _ => 16 }) && split.FontSize == (size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 }), $"{label}: pane density preserves the consumer's container height");
        Check(Near(column.Bounds.Width, thickness) && Near(row.Bounds.Height, thickness) && column.Focusable && row.Focusable, $"{label}: both splitter directions have the expected keyboard-focusable hit targets");
        Check(Colour(Part<Border>(split, "SplitViewSurface").Background) == Resource(split, "One.Surface") && Colour(Part<Panel>(split, "PART_PaneRoot").Background) == Resource(split, "One.Subtle"), $"{label}: content and pane use live semantic surfaces");
        Check(Part<Border>(split, "SplitViewSurface").ClipToBounds && !Part<Border>(split, "SplitViewOutline").IsHitTestVisible && Part<Border>(split, "SplitViewOutline").CornerRadius.TopLeft == 8, $"{label}: rounded outside frame is painted over clipped pane and content");
        Hover(window, column);
        Check(Colour(Part<Border>(column, "SplitterSurface").Background) == Resource(column, "One.AccentSoft") && Colour(column.Foreground) == Resource(column, "One.Accent"), $"{label}: actual splitter hover uses the shared accent feedback");
        var point = Centre(column, window); window.MouseDown(point, MouseButton.Left); ControlPreview.Flush();
        Check(Colour(column.Background) == Resource(column, "One.Accent") && Colour(column.Foreground) == Resource(column, "One.OnAccent"), $"{label}: actual pointer press highlights the grip");
        window.MouseUp(point, MouseButton.Left); row.Focus(); column.Focus(NavigationMethod.Tab); window.MouseMove(new Point(1, 1)); ControlPreview.Flush();
        Check(Colour(column.BorderBrush) == Resource(column, "One.Focus") && column.BorderThickness == new Thickness(1), $"{label}: keyboard focus has a visible semantic outline");
        Click(window, demo.FindControl<Button>("PaneLast")!);
        Check(paneList.SelectedIndex == 9999 && paneList.GetRealizedContainers().Any(item => Equals(item.DataContext, VirtualizationPreview.Rows[^1])), $"{label}: native sidebar selection realizes the ten-thousandth item");
        Check(paneList.GetRealizedContainers().Count() is > 0 and < 80, $"{label}: sidebar has a bounded virtual viewport rather than ten thousand controls");
        Click(window, demo.FindControl<Button>("TogglePane")!);
        var icon = paneList.GetVisualDescendants().OfType<PathIcon>().First();
        var iconOrigin = icon.TranslatePoint(default, Part<Panel>(split, "PART_PaneRoot"))!.Value;
        Check(iconOrigin.X >= 0 && iconOrigin.X + icon.Bounds.Width <= compact && !demo.FindControl<TextBlock>("PaneTitle")!.IsVisible && paneList.GetVisualDescendants().OfType<TextBlock>().Where(text => text.Name == "WorkspaceLabel").All(text => !text.IsVisible), $"{label}: compact rails retain the leading navigation icons without centering expanded content outside the clip");
        Click(window, demo.FindControl<Button>("TogglePane")!);
        paneList.SelectedIndex = 0; paneList.ScrollIntoView(0); ControlPreview.Flush();
        Wheel(window, paneList, -4); demo.UpdateMetrics();
        Check(paneList.GetRealizedContainers().All(item => !Equals(item.DataContext, VirtualizationPreview.Rows[0])) && demo.FindControl<TextBlock>("PaneMetrics")!.Text!.Contains("10,000"), $"{label}: real sidebar wheel scrolling updates its virtual viewport and preview metrics");
        window.Close();
    }
    private static void VerifyPaneModes()
    {
        var content = new Button { Content = "Content", HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Stretch, VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Stretch };
        var pane = new Button { Content = "Pane", HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Stretch, VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Stretch };
        var split = new SplitView { Content = content, Pane = pane, Padding = new Thickness(0), OpenPaneLength = 180, CompactPaneLength = 44, UseLightDismissOverlayMode = true };
        var window = new Window { Width = 720, Height = 400, WindowDecorations = WindowDecorations.None, Content = split };
        Motion.SetEnabled(window, true); window.Show(); ControlPreview.Flush();
        foreach (var placement in Enum.GetValues<SplitViewPanePlacement>())
        foreach (var mode in Enum.GetValues<SplitViewDisplayMode>())
        {
            split.PanePlacement = placement; split.DisplayMode = mode; ControlPreview.Flush();
            foreach (var open in new[] { false, true, false, true })
            {
                split.IsPaneOpen = open; ControlPreview.Flush();
                var root = Part<Panel>(split, "PART_PaneRoot"); var main = Part<Panel>(split, "ContentRoot"); var container = Part<Grid>(split, "Container");
                var horizontal = placement is SplitViewPanePlacement.Left or SplitViewPanePlacement.Right;
                var paneLength = open ? 180 : mode is SplitViewDisplayMode.CompactInline or SplitViewDisplayMode.CompactOverlay ? 44 : 0;
                var occupied = mode switch { SplitViewDisplayMode.Overlay => 0, SplitViewDisplayMode.CompactOverlay => 44, _ => paneLength };
                var total = horizontal ? container.Bounds.Width : container.Bounds.Height;
                var actualPane = horizontal ? root.Bounds.Width : root.Bounds.Height;
                var actualMain = horizontal ? main.Bounds.Width : main.Bounds.Height;
                Check(Near(actualPane, paneLength) && Near(actualMain, total - occupied), $"SplitView/{placement}/{mode}/{open}: rapid state changes keep native reserved space and pane size synchronized");
                var paneOrigin = root.TranslatePoint(default, container)!.Value;
                var expectedOrigin = placement is SplitViewPanePlacement.Right or SplitViewPanePlacement.Bottom ? total - paneLength : 0;
                Check(Near(horizontal ? paneOrigin.X : paneOrigin.Y, expectedOrigin), $"SplitView/{placement}/{mode}/{open}: pane remains at the correct edge, including overlay spans");
                Check(Part<Border>(split, "LightDismissLayer").IsVisible == (open && mode is SplitViewDisplayMode.Overlay or SplitViewDisplayMode.CompactOverlay), $"SplitView/{placement}/{mode}/{open}: native light-dismiss input layer follows the display mode");
            }
        }
        split.PanePlacement = SplitViewPanePlacement.Left; split.DisplayMode = SplitViewDisplayMode.Overlay; split.IsPaneOpen = true; ControlPreview.Flush();
        Check(Resource(split, "One.SplitView.Overlay") is { } overlay && Colour(Part<Border>(split, "LightDismissLayer").Background) == overlay, "SplitView optional overlay uses the library scrim resource");
        Click(window, content); Check(!split.IsPaneOpen, "Real outside-pane pointer click closes the native overlay");
        var prevent = true; split.PaneClosing += (_, args) => args.Cancel = prevent;
        split.IsPaneOpen = true; ControlPreview.Flush(); Click(window, content);
        Check(split.IsPaneOpen, "Native PaneClosing cancellation preserves an open overlay after an outside click");
        prevent = false; pane.Focus(); Press(window, Key.Escape, PhysicalKey.Escape);
        Check(!split.IsPaneOpen, "Escape from inside the pane closes the native overlay");
        split.DisplayMode = SplitViewDisplayMode.Inline; split.IsPaneOpen = true; ControlPreview.Flush(); pane.Focus(); Press(window, Key.Escape, PhysicalKey.Escape);
        Check(split.IsPaneOpen, "Escape preserves an inline pane according to native behavior");
        window.Close();
    }
    private static void VerifyResizing()
    {
        var (window, demo) = Open(ThemeVariant.Light, ControlSize.Normal);
        var grid = demo.FindControl<Grid>("ResizableGrid")!; var rows = demo.FindControl<Grid>("RowGrid")!;
        var column = demo.FindControl<GridSplitter>("ColumnSplitter")!; var row = demo.FindControl<GridSplitter>("RowSplitter")!;
        var original = grid.ColumnDefinitions[0].ActualWidth;
        Drag(window, column, new Vector(42, 0));
        Check(Near(grid.ColumnDefinitions[0].ActualWidth, original + 42), "Native pointer dragging resizes columns continuously");
        column.Focus(NavigationMethod.Tab); Press(window, Key.Right, PhysicalKey.ArrowRight);
        Check(Near(grid.ColumnDefinitions[0].ActualWidth, original + 42 + column.KeyboardIncrement), "Right arrow resizes a keyboard-focused column splitter");
        var height = rows.RowDefinitions[0].ActualHeight; Drag(window, row, new Vector(0, 16));
        Check(Near(rows.RowDefinitions[0].ActualHeight, height + 16), "Native pointer dragging resizes rows continuously");
        row.Focus(NavigationMethod.Tab); Press(window, Key.Up, PhysicalKey.ArrowUp);
        Check(Near(rows.RowDefinitions[0].ActualHeight, height + 16 - row.KeyboardIncrement), "Up arrow resizes a keyboard-focused row splitter");
        Check(demo.FindControl<TextBlock>("LayoutStatus")!.Text!.Contains($"行高 {rows.RowDefinitions[0].ActualHeight:0} / {rows.RowDefinitions[2].ActualHeight:0}"), "Preview row-size metrics update after native keyboard resizing");
        Drag(window, column, new Vector(-1500, 0));
        Check(Near(grid.ColumnDefinitions[0].ActualWidth, 120), "Column drag respects the consumer's minimum width");
        Drag(window, row, new Vector(0, 1500));
        Check(Near(rows.RowDefinitions[2].ActualHeight, 52), "Row drag respects the consumer's minimum height");
        Click(window, demo.FindControl<Button>("ResetLayout")!);
        Check(Near(grid.ColumnDefinitions[0].ActualWidth, original), "Real reset button restores the original star proportions");
        Click(window, demo.FindControl<CheckBox>("ResizePreview")!);
        var point = Centre(column, window); window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseMove(point + new Vector(45, 0)); ControlPreview.Flush();
        Check(Near(grid.ColumnDefinitions[0].ActualWidth, original) && window.GetVisualDescendants().OfType<Border>().Any(border => border.Name == "SplitterPreview"), "ShowsPreview displays the themed native adorner without changing layout while dragging");
        window.MouseUp(point + new Vector(45, 0), MouseButton.Left); ControlPreview.Flush();
        Check(Near(grid.ColumnDefinitions[0].ActualWidth, original + 45), "Preview resizing commits the native width only on pointer release");
        point = Centre(column, window); window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseMove(point + new Vector(25, 0)); ControlPreview.Flush();
        Press(window, Key.Escape, PhysicalKey.Escape); window.MouseUp(point + new Vector(25, 0), MouseButton.Left); ControlPreview.Flush();
        Check(Near(grid.ColumnDefinitions[0].ActualWidth, original + 45) && !window.GetVisualDescendants().OfType<Border>().Any(border => border.Name == "SplitterPreview"), "Escape cancels preview resizing and removes the native adorner");
        Click(window, demo.FindControl<CheckBox>("ResizeEnabled")!); var before = grid.ColumnDefinitions[0].ActualWidth; Drag(window, column, new Vector(20, 0));
        Check(!column.IsEffectivelyEnabled && !row.IsEffectivelyEnabled && Near(grid.ColumnDefinitions[0].ActualWidth, before) && Colour(column.Background) == Resource(column, "One.DisabledSurface"), "Disabled preview prevents resizing and uses the shared disabled surface");
        column.ResizeDirection = GridResizeDirection.Auto; row.ResizeDirection = GridResizeDirection.Auto;
        Click(window, demo.FindControl<CheckBox>("ResizeEnabled")!); Click(window, demo.FindControl<CheckBox>("ResizePreview")!);
        before = grid.ColumnDefinitions[0].ActualWidth; Drag(window, column, new Vector(15, 0));
        height = rows.RowDefinitions[0].ActualHeight; Drag(window, row, new Vector(0, -12));
        Check(Near(grid.ColumnDefinitions[0].ActualWidth, before + 15) && Near(rows.RowDefinitions[0].ActualHeight, height - 12), "Both splitter axes retain native Auto direction inferred from their bounds");
        Motion.SetEnabled(window, true); Hover(window, column); EnhancementsPreview.Advance(150);
        Check(Part<Border>(column, "SplitterSurface").Transitions?.Count > 0, "Splitter hover animates only brushes without animating drag geometry");
        Motion.SetEnabled(window, false); ControlPreview.Flush();
        Check(Part<Border>(column, "SplitterSurface").Transitions is null, "Inherited reduced-motion setting removes splitter brush transitions");
        window.RequestedThemeVariant = ThemeVariant.Dark; ControlSizing.SetSize(window, ControlSize.Large); ControlPreview.Flush();
        Check(column.Bounds.Width == 12 && row.Bounds.Height == 12 && Colour(Part<Border>(demo.FindControl<SplitView>("PaneSample")!, "SplitViewSurface").Background) == Color.Parse("#19211E"), "Live density and theme changes update existing layout controls");
        window.Close();
    }
    private static void VerifyConsumer()
    {
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto,*") };
        var splitter = new GridSplitter { Classes = { "small" }, VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Stretch, HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Stretch };
        Grid.SetColumn(splitter, 1); grid.Children.Add(splitter);
        var split = new SplitView { Classes = { "large" }, Content = grid, Pane = new TextBlock { Text = "Consumer pane" }, IsPaneOpen = true, DisplayMode = SplitViewDisplayMode.CompactInline, OpenPaneLength = 190, CompactPaneLength = 40, BorderThickness = new Thickness(2), CornerRadius = new CornerRadius(14) };
        var window = new Window { Width = 600, Height = 300, Content = split }; window.Show(); ControlPreview.Flush();
        Check(split.OpenPaneLength == 190 && split.CompactPaneLength == 40 && splitter.Bounds.Width == 8 && Part<Border>(split, "SplitViewOutline").CornerRadius.TopLeft == 14, "Plain consumers inherit OneTheme while local pane lengths, radius and per-control size overrides win");
        window.Resources["One.GridSplitter.Small.Thickness"] = 9d; ControlPreview.Flush();
        Check(Near(splitter.Bounds.Width, 9), "Consumer semantic density resources update existing splitter geometry");
        window.Close();
    }
    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(11); ControlPreview.Flush();
        var card = gallery.FindControl<Border>("LayoutControlsCard")!;
        Check(card.IsVisible && card.Child is LayoutDemoView, "Layout catalogue creates its interactive preview on demand");
        var demo = (LayoutDemoView)card.Child!; var split = demo.FindControl<SplitView>("PaneSample")!;
        Click(window, demo.FindControl<Button>("TogglePane")!); Check(!split.IsPaneOpen, "Catalogue sidebar toggle works through actual pointer input");
        demo.FindControl<ComboBox>("PaneMode")!.SelectedIndex = 2; demo.FindControl<ComboBox>("PaneDirection")!.SelectedIndex = 1; ControlPreview.Flush();
        Check(split.DisplayMode == SplitViewDisplayMode.CompactOverlay && split.PanePlacement == SplitViewPanePlacement.Right, "Preview selectors update native display mode and placement");
        Check(demo.FindControl<ListBox>("PaneList")!.SelectedIndex == 0, "Sidebar selection survives pane placement template replacement");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 2; ControlPreview.Flush();
        Check(split.CompactPaneLength == 64 && demo.FindControl<GridSplitter>("ColumnSplitter")!.MinWidth == 12, "Global size selector updates pane lengths and splitter hit areas");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 1; window.NavigateToCategory(0); gallery.Search("GridSplitter"); ControlPreview.Flush();
        Check(card.IsVisible && !gallery.FindControl<Border>("DataGridCard")!.IsVisible, "Global GridSplitter search reveals the layout preview");
        gallery.Search(""); ControlPreview.Flush();
    }
    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, demo) = Open(theme, size); demo.UpdateMetrics();
            var name = theme.Key!.ToString()!.ToLowerInvariant();
            EnhancementsPreview.Save(window, Path.Combine(output, $"layout-{name}-{size.ToString().ToLowerInvariant()}.png"));
            if (size == ControlSize.Normal)
            {
                var split = demo.FindControl<SplitView>("PaneSample")!;
                split.IsPaneOpen = false; ControlPreview.Flush(); demo.UpdateMetrics();
                EnhancementsPreview.Save(window, Path.Combine(output, $"layout-{name}-compact-closed.png"));
                demo.FindControl<ComboBox>("PaneDirection")!.SelectedIndex = 1;
                demo.FindControl<ComboBox>("PaneMode")!.SelectedIndex = 2;
                split.IsPaneOpen = true; ControlPreview.Flush(); demo.UpdateMetrics();
                EnhancementsPreview.Save(window, Path.Combine(output, $"layout-{name}-right-overlay.png"));
                demo.FindControl<ComboBox>("PaneDirection")!.SelectedIndex = 2;
                demo.FindControl<ComboBox>("PaneMode")!.SelectedIndex = 1;
                ControlPreview.Flush(); demo.UpdateMetrics();
                EnhancementsPreview.Save(window, Path.Combine(output, $"layout-{name}-top-inline.png"));
                demo.FindControl<ComboBox>("PaneDirection")!.SelectedIndex = 0;
                demo.FindControl<ComboBox>("PaneMode")!.SelectedIndex = 3;
                var column = demo.FindControl<GridSplitter>("ColumnSplitter")!;
                column.ShowsPreview = true; ControlPreview.Flush();
                var point = Centre(column, window); window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseMove(point + new Vector(45, 0)); ControlPreview.Flush();
                EnhancementsPreview.Save(window, Path.Combine(output, $"layout-{name}-resize-preview.png"));
                Press(window, Key.Escape, PhysicalKey.Escape); window.MouseUp(point + new Vector(45, 0), MouseButton.Left);
            }
            window.Close();
        }
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        {
            var (window, demo) = Open(theme, ControlSize.Normal, 680); demo.UpdateMetrics();
            EnhancementsPreview.Save(window, Path.Combine(output, $"layout-{theme.Key!.ToString()!.ToLowerInvariant()}-compact-window.png"));
            window.Close();
        }
    }
    private static bool Near(double a, double b) => Math.Abs(a - b) < 1.1;
    private static Point Centre(Control control, TopLevel host) => control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
    private static void Drag(TopLevel window, Control control, Vector delta)
    {
        var point = Centre(control, window); window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseMove(point + delta); window.MouseUp(point + delta, MouseButton.Left); ControlPreview.Flush();
    }
    private static void Press(TopLevel window, Key key, PhysicalKey physical) { window.KeyPress(key, RawInputModifiers.None, physical, null); window.KeyRelease(key, RawInputModifiers.None, physical, null); ControlPreview.Flush(); }
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
}
