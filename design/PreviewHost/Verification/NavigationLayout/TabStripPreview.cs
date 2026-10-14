using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class TabStripPreview
{
    private static (Window Window, TabStripDemoView Demo) Open(ThemeVariant theme, ControlSize size, int width = 1080)
    {
        var demo = new TabStripDemoView();
        var window = new Window { Width = width, Height = 900, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = demo } };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false);
        window.Show(); ControlPreview.Flush(); return (window, demo);
    }
    public static void Verify()
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) VerifyNative(theme, size);
        VerifyOverrides();
    }
    private static void VerifyNative(ThemeVariant theme, ControlSize size)
    {
        var (window, demo) = Open(theme, size); var strip = demo.FindControl<TabStrip>("HugeStrip")!;
        var label = $"TabStrip/{theme.Key}/{size}";
        var first = (TabStripItem)strip.ContainerFromIndex(0)!;
        var font = size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
        var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
        Check(strip.GetType() == typeof(TabStrip) && first.GetType() == typeof(TabStripItem), $"{label}: retains native controls");
        Check(first.FontSize == font && first.MinHeight == height && first.Bounds.Height >= height, $"{label}: inherited three-size typography and hit target");
        Check(strip.ItemsPanelRoot is VirtualizingStackPanel { Orientation: global::Avalonia.Layout.Orientation.Horizontal }, $"{label}: bounded horizontal virtual panel");
        Bounded(strip, label);
        Check(strip.SelectedIndex == 0 && first.IsSelected && Part<Border>(first, "PART_SelectedPipe").IsVisible, $"{label}: native AlwaysSelected and selected indicator");
        Check(Colour(first.Background) == Resource(first, "One.AccentSoft") && Colour(first.Foreground) == Resource(first, "One.Accent"), $"{label}: selected semantic colors");
        var horizontalBar = Part<ScrollBar>(Part<ScrollViewer>(strip, "PART_ScrollViewer"), "PART_HorizontalScrollBar");
        Check(horizontalBar.Bounds.Height <= 16 && horizontalBar.GetVisualDescendants().OfType<RepeatButton>().All(button => button.MinHeight <= 16 && button.MinWidth <= 16), $"{label}: embedded scrollbar buttons retain native track thickness without intercepting header input");
        Hover(window, first);
        Check(Colour(first.Foreground) == Resource(first, "One.AccentHover"), $"{label}: actual selected hover color");
        var point = Centre(first, window); window.MouseDown(point, MouseButton.Left); ControlPreview.Flush();
        Check(Colour(Part<Border>(first, "PART_SelectedPipe").Background) == Resource(first, "One.AccentPressed"), $"{label}: actual pressed indicator");
        window.MouseUp(point, MouseButton.Left); ControlPreview.Flush(); window.MouseMove(new Point(1, 1)); ControlPreview.Flush();
        var second = (TabStripItem)strip.ContainerFromIndex(1)!;
        Hover(window, second); Check(Colour(second.Background) == Resource(second, "One.Subtle"), $"{label}: unselected hover surface");
        Click(window, second); Check(strip.SelectedIndex == 1 && second.IsSelected && !first.IsSelected, $"{label}: real pointer selection");
        var workflow = demo.FindControl<TabStrip>("WorkflowStrip")!;
        Click(window, (TabStripItem)workflow.ContainerFromIndex(1)!);
        Check(workflow.SelectedIndex == 1 && demo.FindControl<TextBlock>("WorkflowStatus")!.Text!.StartsWith("动态"), $"{label}: native selection updates consumer content");
        var disabled = (TabStripItem)workflow.ContainerFromIndex(2)!; Click(window, disabled);
        Check(workflow.SelectedIndex == 1 && Colour(disabled.Foreground) == Resource(disabled, "One.DisabledText"), $"{label}: disabled item preserves selection and readable text");
        var initial = strip.GetRealizedContainers().ToArray();
        strip.SelectedIndex = 8999; ControlPreview.Flush();
        var distant = strip.ContainerFromIndex(8999) as TabStripItem;
        Check(distant is { IsSelected: true } && Equals(distant.Content, demo.Rows[8999]), $"{label}: native programmatic selection realizes an unseen target");
        Check(distant!.TranslatePoint(default, strip) is { X: >= 0 } origin && origin.X + distant!.Bounds.Width <= strip.Bounds.Width + 1, $"{label}: distant selection is inside its viewport");
        Bounded(strip, label + "/distant");
        Check(initial.Intersect(strip.GetRealizedContainers()).Any(), $"{label}: actual containers are reused across distant scrolling");
        strip.ScrollIntoView(9999); ControlPreview.Flush(); var last = (TabStripItem)strip.ContainerFromIndex(9999)!;
        Click(window, last); Check(strip.SelectedIndex == 9999, $"{label}: real last-item click selects data rather than a stale recycled item");
        last.Focus(); Press(window, Key.Left, PhysicalKey.ArrowLeft);
        Check(strip.SelectedIndex == 9998, $"{label}: native arrow navigation selects and realizes an adjacent item");
        Press(window, Key.Home, PhysicalKey.Home); Check(strip.SelectedIndex == 0, $"{label}: Home navigates to the unrealized first item");
        Press(window, Key.End, PhysicalKey.End); Check(strip.SelectedIndex == 9999, $"{label}: End navigates to the unrealized final item");
        Bounded(strip, label + "/keyboard");
        Click(window, demo.FindControl<Button>("ResetTabs")!); Check(strip.SelectedIndex == 0, $"{label}: preview reset returns to first item");
        var scroll = Part<ScrollViewer>(strip, "PART_ScrollViewer");
        window.MouseWheel(Centre(strip, window), new Vector(-4, 0)); ControlPreview.Flush();
        Check(scroll.Offset.X > 0, $"{label}: real horizontal wheel scrolling moves the native viewport");
        Click(window, demo.FindControl<Button>("JumpLast")!); Check(strip.SelectedIndex == 9999, $"{label}: preview last-item jump");
        Click(window, demo.FindControl<CheckBox>("EnableStrip")!); var selected = strip.SelectedIndex;
        Click(window, strip.ContainerFromIndex(9998)!);
        Check(strip.SelectedIndex == selected && !strip.IsEffectivelyEnabled, $"{label}: disabled strip rejects pointer selection");
        Click(window, demo.FindControl<CheckBox>("EnableStrip")!);
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        ControlSizing.SetSize(window, size == ControlSize.Large ? ControlSize.Small : ControlSize.Large); ControlPreview.Flush();
        var updated = (TabStripItem)strip.ContainerFromIndex(strip.SelectedIndex)!;
        Check(updated.ActualThemeVariant == window.ActualThemeVariant && updated.FontSize == (size == ControlSize.Large ? 12 : 16) && updated.IsSelected, $"{label}: runtime theme and density changes preserve native selection");
        Check(Colour(updated.Background) == Resource(updated, "One.AccentSoft"), $"{label}: dynamic resource colors update without rebuilding controls");
        demo.UpdateMetrics(); Check(demo.FindControl<TextBlock>("TabMetrics")!.Text!.Contains("10,000"), $"{label}: interactive preview reports actual realization counts");
        window.Width = 620; ControlPreview.Flush(); Bounded(strip, label + "/compact");
        Check(strip.Bounds.Width <= 572 && Part<Border>(strip, "PART_Outline").Bounds.Size == strip.Bounds.Size, $"{label}: compact layout keeps the full outline within the viewport");
        window.Close();
    }
    private static void VerifyOverrides()
    {
        var strip = new TabStrip { ItemsSource = VirtualizationPreview.Rows, BorderThickness = new Thickness(2), BorderBrush = Brushes.Red, Background = Brushes.Blue,
            CornerRadius = new CornerRadius(6), Padding = new Thickness(7), ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = "Custom " + text }) };
        var window = new Window { Width = 500, Height = 160, Content = strip, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        Check(Part<Border>(strip, "PART_Outline").BorderBrush == Brushes.Red && Part<Border>(strip, "PART_LayoutRoot").Background == Brushes.Blue && Part<Border>(strip, "PART_Outline").CornerRadius.TopLeft == 6, "TabStrip consumer border, surface and radius overrides remain effective");
        var first = (TabStripItem)strip.ContainerFromIndex(0)!;
        Check(first.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == "Custom " + VirtualizationPreview.Rows[0]), "TabStrip retains consumer ItemTemplate");
        first.Background = Brushes.Yellow; first.Foreground = Brushes.Red; first.Padding = new Thickness(3); first.FontSize = 19;
        first.Classes.Add("small"); ControlPreview.Flush(); Hover(window, first);
        Check(first.Background == Brushes.Yellow && first.Foreground == Brushes.Red && first.Padding.Left == 3 && first.FontSize == 19, "TabStripItem local properties override size and selected-hover defaults");
        Check(Part<Border>(first, "PART_LayoutRoot").Transitions is null, "TabStripItem reduced motion removes template brush transitions");
        window.Close();
    }
    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(17); ControlPreview.Flush();
        var card = gallery.FindControl<Border>("TabStripCard")!;
        Check(card.IsVisible && card.Child is TabStripDemoView, "TabStrip category lazily loads its interactive preview");
        var demo = (TabStripDemoView)card.Child!; Click(window, demo.FindControl<Button>("JumpTarget")!);
        Check(demo.FindControl<TabStrip>("HugeStrip")!.SelectedIndex == 9000, "TabStrip catalogue jump uses the editable target index");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 0; window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 2; ControlPreview.Flush();
        var strip = demo.FindControl<TabStrip>("HugeStrip")!;
        Check(((TabStripItem)strip.ContainerFromIndex(9000)!).FontSize == 12 && strip.ActualThemeVariant == ThemeVariant.Dark, "TabStrip catalogue follows global theme and density");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 1; window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 1;
        window.NavigateToCategory(0); gallery.Search("TabStrip"); ControlPreview.Flush();
        Check(card.IsVisible && !gallery.FindControl<Border>("ButtonControlsCard")!.IsVisible, "TabStrip is searchable independently of comparison pages");
        gallery.Search(""); ControlPreview.Flush();
    }
    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, demo) = Open(theme, size); var name = $"tabstrip-{theme.Key!.ToString()!.ToLowerInvariant()}-{size.ToString().ToLowerInvariant()}";
            EnhancementsPreview.Save(window, Path.Combine(output, name + ".png"));
            Click(window, demo.FindControl<Button>("JumpLast")!);
            EnhancementsPreview.Save(window, Path.Combine(output, name + "-last.png")); window.Close();
        }
        var (compact, _) = Open(ThemeVariant.Dark, ControlSize.Normal, 620); EnhancementsPreview.Save(compact, Path.Combine(output, "tabstrip-compact.png")); compact.Close();
    }
    private static void Bounded(TabStrip strip, string label) => Check(strip.GetRealizedContainers().Count() is > 0 and < 80, $"{label}: realizes fewer than 80 of 10,000 data models");
    private static void Press(TopLevel host, Key key, PhysicalKey physical) { host.KeyPress(key, RawInputModifiers.None, physical, null); host.KeyRelease(key, RawInputModifiers.None, physical, null); ControlPreview.Flush(); }
    private static Point Centre(Control control, TopLevel host) => control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
}
