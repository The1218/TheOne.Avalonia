using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System.Runtime.InteropServices;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class SplitButtonPreview
{
    public static void Verify()
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) VerifyDensity(theme, size);
        VerifyInteraction(); VerifyVirtualMenu(); VerifyConsumer();
    }
    private static (Window Window, SplitButtonDemoView Demo) Open(ThemeVariant theme, ControlSize size, int width = 1060)
    {
        var demo = new SplitButtonDemoView();
        var window = new Window { Width = width, Height = 880, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = demo } };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        return (window, demo);
    }
    private static Button Primary(SplitButton button) => Part<Button>(button, "PART_PrimaryButton");
    private static Button Secondary(SplitButton button) => Part<Button>(button, "PART_SecondaryButton");
    private static ContentPresenter Chrome(Button button) => Part<ContentPresenter>(button, "PART_ContentPresenter");
    private static MenuFlyoutPresenter Menu(SplitButton button) => (MenuFlyoutPresenter)((PopupFlyoutBase)button.Flyout!).Popup.Child!;
    private static void VerifyDensity(ThemeVariant theme, ControlSize size)
    {
        var (window, demo) = Open(theme, size);
        var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
        var font = size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
        var width = size switch { ControlSize.Small => 28, ControlSize.Large => 40, _ => 32 };
        foreach (var name in new[] { "ExportSplit", "PublishSplit", "AutoSplit", "PinnedSplit" })
        {
            var split = demo.FindControl<SplitButton>(name)!; var first = Primary(split); var second = Secondary(split);
            var accent = split.Classes.Contains("primary"); var isChecked = split is ToggleSplitButton { IsChecked: true };
            var label = $"{name}/{theme.Key}/{size}";
            Check(split.Bounds.Height == height && second.Bounds.Width == width && split.FontSize == font && first.FontSize == font, $"{label}: native split parts have the expected density and font");
            Check(first.MinHeight == 0 && second.MinHeight == 0 && first.Padding == split.Padding && second.Padding == new Thickness(0), $"{label}: global button sizing cannot offset the primary content or inflate the arrow region");
            Check(split.Focusable && !first.Focusable && !second.Focusable && split.FocusAdorner is not null, $"{label}: the split button remains one keyboard focus stop with the shared focus adorner");
            Check(Part<Border>(split, "SplitButtonSurface").ClipToBounds && Part<Border>(split, "SplitButtonOutline").CornerRadius.TopLeft == 8 && !Part<Border>(split, "SplitButtonOutline").IsHitTestVisible, $"{label}: a clipped surface and final rounded stroke protect both edge buttons");
            var background = accent ? isChecked ? "One.AccentPressed" : "One.Accent" : isChecked ? "One.AccentSoft" : "One.Surface";
            Check(Colour(Part<Border>(split, "SplitButtonSurface").Background) == Resource(split, background) && Colour(Chrome(first).Background) == Resource(split, background), $"{label}: default and checked surfaces use Still semantic colors");
            Hover(window, second);
            Check(Colour(Chrome(second).Background) == Resource(split, accent ? "One.AccentHover" : isChecked ? "One.AccentSoft" : "One.Subtle") && Colour(Chrome(first).Background) == Resource(split, background), $"{label}: actual arrow hover changes only its own region");
            using (var bitmap = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(split.Bounds.Width), height), new Vector(96, 96)))
            {
                bitmap.Render(split);
                Check(Pixel(bitmap, bitmap.PixelSize.Width - 1, height / 2) == Colour(split.BorderBrush) && Pixel(bitmap, bitmap.PixelSize.Width - 1, 0).A == 0,
                    $"{label}: actual Skia pixels retain the edge stroke and transparent rounded corner during arrow hover");
            }
            window.MouseMove(new Point(1, 1)); Hover(window, first);
            Check(Colour(Chrome(first).Background) == Resource(split, accent ? "One.AccentHover" : isChecked ? "One.AccentSoft" : "One.Subtle") && Colour(Chrome(second).Background) == Resource(split, background), $"{label}: actual primary hover changes only its own region");
            var point = Centre(second, window); window.MouseMove(point); window.MouseDown(point, MouseButton.Left); ControlPreview.Flush();
            Check(Colour(Chrome(second).Background) == Resource(split, accent ? "One.AccentPressed" : isChecked ? "One.AccentSoft" : "One.Line"), $"{label}: pointer press retains the correct primary or checked palette");
            window.MouseUp(point, MouseButton.Left); ControlPreview.Flush();
            Check(split.Flyout!.IsOpen && demo.Model.MainCount == 0 && (split is not ToggleSplitButton toggle || toggle.IsChecked == isChecked), $"{label}: arrow click opens the native flyout without executing or toggling the main action");
            Check(Colour(Chrome(second).Background) == Resource(split, accent ? "One.AccentPressed" : "One.AccentSoft"), $"{label}: open flyout has a persistent arrow-region highlight");
            var menu = Menu(split); var item = menu.GetRealizedContainers().OfType<MenuItem>().First();
            Check(ControlSizing.GetSize(item) == size && item.FontSize == font, $"{label}: the native menu inherits the triggering button's density");
            split.Flyout.Hide(); ControlPreview.Flush();
        }
        foreach (var disabled in demo.GetVisualDescendants().OfType<SplitButton>().Where(button => !button.IsEffectivelyEnabled))
            Check(Colour(Chrome(Primary(disabled)).Background) == Resource(disabled, "One.DisabledSurface") && Colour(Chrome(Primary(disabled)).Foreground) == Resource(disabled, "One.DisabledText"), $"{theme.Key}/{size}: disabled checked and unchecked parts share the disabled palette");
        window.Close();
    }
    private static void VerifyInteraction()
    {
        var (window, demo) = Open(ThemeVariant.Light, ControlSize.Normal);
        var split = demo.FindControl<SplitButton>("ExportSplit")!; var toggle = demo.FindControl<ToggleSplitButton>("AutoSplit")!;
        var clicks = 0; split.Click += (_, _) => clicks++;
        Click(window, Primary(split));
        Check(demo.Model.MainCount == 1 && clicks == 1 && demo.Model.Status == "已导出 PDF 报表", "Native primary pointer click raises one public click and executes its command exactly once");
        Click(window, Secondary(split)); var menu = Menu(split); var menuHost = TopLevel.GetTopLevel(menu)!;
        Click(menuHost, menu.GetRealizedContainers().OfType<MenuItem>().Single(item => Equals(item.Header, "导出为 CSV")));
        Check(!split.Flyout!.IsOpen && demo.Model.ChoiceCount == 1 && demo.Model.MainCount == 1 && split.Content?.ToString() == "导出 · CSV", "Real native menu selection changes the primary option and closes the flyout");
        split.Focus(NavigationMethod.Tab); Press(window, Key.Space, PhysicalKey.Space);
        Check(demo.Model.MainCount == 2 && demo.Model.Status == "已导出 CSV 报表", "Space invokes the main action with its current bound option");
        Press(window, Key.Enter, PhysicalKey.Enter); Check(demo.Model.MainCount == 3 && clicks == 3, $"Enter preserves one command execution and public click per invocation (count={demo.Model.MainCount}, clicks={clicks}, enabled={split.IsEffectivelyEnabled}, focus={window.FocusManager?.GetFocusedElement()?.GetType().Name})");
        Click(window, Primary(toggle)); Check(toggle.IsChecked && demo.Model.AutoPublish && demo.Model.MainCount == 4, "ToggleSplitButton primary click updates IsChecked, its two-way model and one command");
        Click(window, Secondary(toggle)); menu = Menu(toggle); menuHost = TopLevel.GetTopLevel(menu)!;
        Click(menuHost, menu.GetRealizedContainers().OfType<MenuItem>().Single(item => Equals(item.Header, "每周")));
        Check(toggle.IsChecked && demo.Model.AutoPublish && demo.Model.FrequencyLabel == "发布频率：每周" && demo.Model.MainCount == 4, "Toggle menu changes settings while preserving the checked state and main-action count");
        toggle.Focus(NavigationMethod.Tab); Press(window, Key.Space, PhysicalKey.Space);
        Check(!toggle.IsChecked && !demo.Model.AutoPublish && demo.Model.MainCount == 5, "Space toggles the native checked state through the two-way binding");
        split.Focus(); Press(window, Key.F4, PhysicalKey.F4); Check(split.Flyout!.IsOpen, "F4 opens the native split-button flyout");
        Press(TopLevel.GetTopLevel(Menu(split))!, Key.Escape, PhysicalKey.Escape); Check(!split.Flyout.IsOpen, "Escape closes a focused native menu flyout");
        split.Focus(); Press(window, Key.Down, PhysicalKey.ArrowDown, RawInputModifiers.Alt); Check(split.Flyout.IsOpen, "Alt + Down opens the native flyout"); split.Flyout.Hide(); ControlPreview.Flush();
        Click(window, demo.FindControl<CheckBox>("SplitCommandEnabled")!); var count = demo.Model.MainCount;
        Check(!split.IsEffectivelyEnabled && !toggle.IsEffectivelyEnabled && !Secondary(split).IsEffectivelyEnabled, "CanExecuteChanged preserves native whole-control disabling, including the arrow");
        Click(window, Primary(split)); Click(window, Secondary(split)); split.Focus(); Press(window, Key.Enter, PhysicalKey.Enter);
        Check(demo.Model.MainCount == count && !split.Flyout.IsOpen, "A non-executable command rejects both pointer and keyboard activation");
        Click(window, demo.FindControl<Button>("ResetSplitDemo")!);
        Check(split.IsEffectivelyEnabled && demo.Model.MainCount == 0 && demo.Model.ChoiceCount == 0 && !toggle.IsChecked, "Real reset button restores command availability and bound toggle state");
        Click(window, demo.FindControl<CheckBox>("SplitDemoEnabled")!); Click(window, Primary(toggle));
        Check(!toggle.IsEffectivelyEnabled && !toggle.IsChecked && demo.Model.MainCount == 0, "Explicitly disabled ToggleSplitButton cannot change the bound checked state");
        demo.Model.IsEnabled = true; ControlPreview.Flush();
        window.RequestedThemeVariant = ThemeVariant.Dark; ControlSizing.SetSize(window, ControlSize.Large); ControlPreview.Flush();
        Check(split.Bounds.Height == 44 && Secondary(split).Bounds.Width == 40 && Colour(Part<Border>(split, "SplitButtonSurface").Background) == Color.Parse("#19211E"), "Live size and theme updates reach both native parts");
        Motion.SetEnabled(window, true); Hover(window, Secondary(split)); EnhancementsPreview.Advance(150);
        Check(Chrome(Secondary(split)).Transitions?.Count > 0, "Split-button part hover uses the shared brush transitions");
        Motion.SetEnabled(window, false); ControlPreview.Flush(); Check(Chrome(Secondary(split)).Transitions is null, "Reduced-motion inheritance disables split-button part transitions");
        window.Close();
    }
    private static void VerifyVirtualMenu()
    {
        var (window, demo) = Open(ThemeVariant.Dark, ControlSize.Small);
        var split = demo.FindControl<SplitButton>("TemplateSplit")!; Click(window, Secondary(split)); var menu = Menu(split);
        Check(menu.ItemCount == 10000 && menu.GetRealizedContainers().Count() is > 0 and < 80 && menu.ItemsPanel?.Build() is VirtualizingStackPanel, "Ten-thousand-item native split flyout has a bounded recycling menu viewport");
        var initial = menu.GetRealizedContainers().ToHashSet(ReferenceEqualityComparer.Instance);
        var host = TopLevel.GetTopLevel(menu)!; Wheel(host, menu, -6);
        Check(menu.GetRealizedContainers().Count() < 80 && menu.GetRealizedContainers().All(item => !Equals((item.DataContext as SplitTemplateOption)?.Name, "模板 00001")), "Actual wheel input scrolls a large split flyout without materializing all menu items");
        menu.ScrollIntoView(9999); ControlPreview.Flush(); demo.UpdateMenuMetrics();
        var last = (MenuItem)menu.ContainerFromIndex(9999)!;
        Check(last.IsEffectivelyVisible && menu.GetRealizedContainers().Count() < 80 && demo.Model.MenuMetrics.Contains("10,000"), "Large split flyout realizes the final template and displays actual container statistics");
        Click(host, last);
        Check(!split.Flyout!.IsOpen && demo.Model.Status == "已选择：模板 10000" && demo.Model.ChoiceCount == 1 && demo.Model.MainCount == 0, "Real last-template click executes its bound command without firing the primary action");
        Click(window, Primary(split));
        Check(demo.Model.MainCount == 1 && demo.Model.Status == "已使用：模板 10000", "Primary template action uses the previously selected distant option");
        Click(window, Secondary(split)); menu = Menu(split); menu.ScrollIntoView(0); ControlPreview.Flush();
        Check(menu.GetRealizedContainers().Any(initial.Contains) && menu.GetRealizedContainers().Count() < 80, "Native split flyout recycles menu containers after distant round-trip scrolling");
        split.Flyout.Hide(); window.Close();
    }
    private static void VerifyConsumer()
    {
        var split = new ToggleSplitButton { Content = "Consumer", IsChecked = true, Classes = { "small" }, CornerRadius = new CornerRadius(14), BorderThickness = new Thickness(2), Padding = new Thickness(18, 2) };
        var window = new Window { Width = 400, Height = 180, Content = split }; window.Show(); ControlPreview.Flush();
        Check(split.Bounds.Height == 30 && Part<Border>(split, "SplitButtonOutline").CornerRadius.TopLeft == 14 && Part<Border>(split, "SplitButtonOutline").BorderThickness == new Thickness(2) && Primary(split).Padding == new Thickness(18, 2), "Plain native ToggleSplitButton inherits OneTheme while local border, radius and padding override density defaults");
        split.Theme = (ControlTheme)split.FindResource(typeof(ToggleSplitButton))!; ControlPreview.Flush();
        Check(Primary(split).Content?.ToString() == "Consumer" && split.IsChecked, "The explicit typed ToggleSplitButton theme shares the native split template and checked state");
        window.Resources["One.SplitButton.Small.SecondaryWidth"] = 34d; ControlPreview.Flush();
        Check(Secondary(split).Bounds.Width == 34, "Consumer density-resource overrides update the existing native arrow region");
        window.Close();
    }
    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(12); ControlPreview.Flush();
        var card = gallery.FindControl<Border>("SplitButtonsCard")!;
        Check(card.IsVisible && card.Child is SplitButtonDemoView, "Split-button catalogue loads its interactive examples on demand");
        var demo = (SplitButtonDemoView)card.Child!; Click(window, Primary(demo.FindControl<SplitButton>("ExportSplit")!));
        Check(demo.Model.MainCount == 1, "Catalogue split-button primary action works through real pointer input");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 0; ControlPreview.Flush();
        Check(demo.FindControl<ToggleSplitButton>("AutoSplit")!.Bounds.Height == 30, "Global density selector sizes the native ToggleSplitButton preview");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 1; window.NavigateToCategory(0); gallery.Search("ToggleSplitButton"); ControlPreview.Flush();
        Check(card.IsVisible && !gallery.FindControl<Border>("DataGridCard")!.IsVisible, "Global ToggleSplitButton search finds its interactive preview");
        gallery.Search(""); ControlPreview.Flush();
    }
    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, demo) = Open(theme, size); var name = theme.Key!.ToString()!.ToLowerInvariant();
            EnhancementsPreview.Save(window, Path.Combine(output, $"splitbuttons-{name}-{size.ToString().ToLowerInvariant()}.png"));
            if (size == ControlSize.Normal)
            {
                var split = demo.FindControl<SplitButton>("ExportSplit")!; Click(window, Secondary(split));
                EnhancementsPreview.Save(window, Path.Combine(output, $"splitbuttons-{name}-flyout-open.png"));
                EnhancementsPreview.Save(Menu(split), Path.Combine(output, $"splitbuttons-{name}-menu.png"));
                split.Flyout!.Hide();
                var templates = demo.FindControl<SplitButton>("TemplateSplit")!; Click(window, Secondary(templates)); Menu(templates).ScrollIntoView(9999); ControlPreview.Flush(); demo.UpdateMenuMetrics();
                EnhancementsPreview.Save(Menu(templates), Path.Combine(output, $"splitbuttons-{name}-virtual-menu.png")); templates.Flyout!.Hide();
            }
            window.Close();
        }
    }
    private static Point Centre(Control control, TopLevel host) => control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
    private static void Press(TopLevel host, Key key, PhysicalKey physical, RawInputModifiers modifiers = RawInputModifiers.None) { host.KeyPress(key, modifiers, physical, null); host.KeyRelease(key, modifiers, physical, null); ControlPreview.Flush(); }
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
    private static Color Pixel(RenderTargetBitmap bitmap, int x, int y)
    {
        var bytes = new byte[4]; var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect(x, y, 1, 1), pin.AddrOfPinnedObject(), 4, 4); }
        finally { pin.Free(); }
        return bitmap.Format == PixelFormat.Rgba8888 ? Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2]) : Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }
}
