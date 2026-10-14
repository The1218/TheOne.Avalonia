using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class ButtonControlsPreview
{
    private static (Window Window, ButtonControlsDemoView Demo) Open(int width, int height, ThemeVariant theme)
    {
        var demo = new ButtonControlsDemoView();
        var window = new Window { Width = width, Height = height, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new ScrollViewer { HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, Content = new Border { Padding = new Thickness(24), Child = demo } } };
        Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush(); return (window, demo);
    }
    public static void Verify()
    {
        var (window, demo) = Open(1240, 1680, ThemeVariant.Light);
        Check(demo.Samples.Count == 120 && demo.Samples.Count(sample => sample.Selected) == 24 && demo.Samples.Count(sample => sample.Primary) == 24,
            "Button comparison contains three sizes, two themes, all four states, selected toggles and primary buttons");
        foreach (var sample in demo.Samples)
        {
            var control = sample.Control; var content = Part<ContentPresenter>(control, "PART_ContentPresenter");
            var label = $"Buttons/{sample.Theme}/{sample.Size}/{control.StyleKey.Name}/{sample.State}/{sample.Selected}/{sample.Primary}";
            var height = sample.Size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
            var font = sample.Size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
            var padding = sample.Size switch { ControlSize.Small => new Thickness(10, 4), ControlSize.Large => new Thickness(16, 10), _ => new Thickness(12, 7) };
            Check(control.ActualThemeVariant.Key!.ToString() == sample.Theme && control.MinHeight == height && control.Bounds.Height >= height && control.FontSize == font
                && content.Padding == padding && content.CornerRadius == new CornerRadius(8) && content.BorderThickness == new Thickness(1),
                $"{label}: native templates share density, padding, radius and border metrics");
            Check(control.IsEnabled == (sample.State != SelectionVisualState.Disabled) && !control.Focusable && !KeyboardNavigation.GetIsTabStop(control)
                && control.Classes.Contains(":pointerover") == (sample.State is SelectionVisualState.Hover or SelectionVisualState.Pressed)
                && control.Classes.Contains(":pressed") == (sample.State == SelectionVisualState.Pressed)
                && (control is not ToggleButton toggle || toggle.IsChecked == sample.Selected),
                $"{label}: fixed states retain native style keys and do not enter keyboard navigation");
            VerifyPalette(sample, label);
        }
        var frozenClicks = 0; foreach (var sample in demo.Samples) sample.Control.Click += (_, _) => frozenClicks++;
        EnhancementsPreview.Advance(350);
        Check(frozenClicks == 0 && demo.RepeatCount == 0, "Fixed pressed RepeatButton samples do not start native repeat timers or affect live counters");
        Check(Grid.GetColumn(demo.FindControl<ThemeVariantScope>("DarkButtonScope")!) == 1, "Wide button comparison places both theme matrices side by side");
        VerifyInteraction(window, demo);
        window.RequestedThemeVariant = ThemeVariant.Dark; ControlSizing.SetSize(window, ControlSize.Large); ControlPreview.Flush();
        Check(demo.FindControl<Button>("LiveAction")!.FontSize == 16 && Colour(Part<ContentPresenter>(demo.FindControl<Button>("LiveAction")!, "PART_ContentPresenter").Background) == Color.Parse("#61C5A5")
            && demo.Samples.All(sample => sample.Control.ActualThemeVariant.Key!.ToString() == sample.Theme),
            "Live buttons follow host theme and density while fixed matrices retain both palettes and all sizes");
        window.Width = 760; ControlPreview.Flush();
        Check(Grid.GetRow(demo.FindControl<ThemeVariantScope>("DarkButtonScope")!) == 1 && demo.Samples.All(sample => sample.Control.Bounds.Width >= sample.Control.DesiredSize.Width - 0.1),
            "Compact button comparison stacks its themes without clipping button contents");
        window.Close();
    }
    private static void VerifyPalette(ButtonComparisonSample sample, string label)
    {
        var disabled = sample.State == SelectionVisualState.Disabled;
        var content = Part<ContentPresenter>(sample.Control, "PART_ContentPresenter");
        var accent = sample.State switch { SelectionVisualState.Hover => "One.AccentHover", SelectionVisualState.Pressed => "One.AccentPressed", _ => "One.Accent" };
        var background = disabled ? "One.DisabledSurface" : sample.Primary ? accent : sample.Selected ? "One.AccentSoft"
            : sample.State switch { SelectionVisualState.Hover => "One.Subtle", SelectionVisualState.Pressed => "One.Line", _ => "One.Surface" };
        var border = disabled ? "One.Line" : sample.Primary ? "One.Accent" : sample.Selected ? accent : "One.InputLine";
        var text = disabled ? "One.DisabledText" : sample.Primary ? "One.OnAccent" : sample.Selected ? sample.State == SelectionVisualState.Hover ? "One.AccentHover" : "One.Accent" : "One.Text";
        Check(Colour(content.Background) == Resource(sample.Control, background) && Colour(content.BorderBrush) == Resource(sample.Control, border) && Colour(content.Foreground) == Resource(sample.Control, text),
            $"{label}: the native state palette stays consistent across button families");
    }
    private static void VerifyInteraction(Window window, ButtonControlsDemoView demo)
    {
        var action = demo.FindControl<Button>("LiveAction")!; var toggle = demo.FindControl<ToggleButton>("LiveToggle")!;
        var repeat = demo.FindControl<RepeatButton>("LiveRepeat")!; var enabled = demo.FindControl<CheckBox>("ButtonInteractionEnabled")!;
        Click(window, action); Check(demo.ActionCount == 1, "Live Button executes one action for one real click");
        action.Focus(); Press(window, Key.Enter, PhysicalKey.Enter); Press(window, Key.Space, PhysicalKey.Space);
        Check(demo.ActionCount == 3, "Live Button preserves native Enter and Space activation without duplicate actions");
        Click(window, toggle); Check(toggle.IsChecked == true, "Live ToggleButton retains selection after clicking");
        Press(window, Key.Space, PhysicalKey.Space); Check(toggle.IsChecked == false, "Live ToggleButton toggles off with Space");
        var point = Centre(repeat, window); window.MouseMove(point); window.MouseDown(point, MouseButton.Left); ControlPreview.Flush();
        Check(demo.RepeatCount == 1 && repeat.IsPressed, "Live RepeatButton triggers immediately when pressed");
        EnhancementsPreview.AdvanceUntil(() => demo.RepeatCount >= 3, 1200);
        Check(demo.RepeatCount >= 3 && repeat.Delay == 280 && repeat.Interval == 80, "Holding the native RepeatButton repeats after its configured delay and interval");
        window.MouseUp(point, MouseButton.Left); ControlPreview.Flush(); var stopped = demo.RepeatCount; EnhancementsPreview.Advance(180);
        Check(demo.RepeatCount == stopped && !repeat.IsPressed, "Releasing the native repeat button stops future pointer repeats");
        repeat.Focus(); window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null); ControlPreview.Flush();
        EnhancementsPreview.AdvanceUntil(() => demo.RepeatCount >= stopped + 3, 1200);
        Check(demo.RepeatCount >= stopped + 3, "Holding Space repeats through the native RepeatButton keyboard path");
        window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null); ControlPreview.Flush(); stopped = demo.RepeatCount; EnhancementsPreview.Advance(180);
        Check(demo.RepeatCount == stopped, "Releasing Space stops native keyboard repeats");
        window.MouseDown(point, MouseButton.Left); ControlPreview.Flush(); enabled.IsChecked = false; ControlPreview.Flush(); stopped = demo.RepeatCount;
        EnhancementsPreview.Advance(400); window.MouseUp(point, MouseButton.Left); ControlPreview.Flush();
        Check(demo.RepeatCount == stopped && !repeat.IsPressed, "Disabling a held RepeatButton cancels the native repeat timer");
        Click(window, action); Click(window, toggle); Click(window, repeat);
        Check(demo.ActionCount == 3 && toggle.IsChecked == false && demo.RepeatCount == stopped && !action.IsEffectivelyEnabled,
            "Disabled live buttons cannot execute, toggle or repeat through pointer input");
        Click(window, demo.FindControl<Button>("ResetButtonDemo")!);
        Check(action.IsEffectivelyEnabled && demo.ActionCount == 0 && demo.RepeatCount == 0 && toggle.IsChecked == false,
            "Button comparison reset restores enabled state and all counters");
    }
    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(16); ControlPreview.Flush(); var card = gallery.FindControl<Border>("ButtonControlsCard")!;
        Check(card.IsVisible && card.Child is ButtonControlsDemoView, "Button comparison category lazily loads its unified preview");
        var demo = (ButtonControlsDemoView)card.Child!; Click(window, demo.FindControl<Button>("LiveAction")!);
        Check(demo.ActionCount == 1, "Button comparison catalogue accepts real action input");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 0; window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 2; ControlPreview.Flush();
        Check(demo.FindControl<ToggleButton>("LiveToggle")!.FontSize == 12 && demo.Samples.All(sample => sample.Control.ActualThemeVariant.Key!.ToString() == sample.Theme),
            "Global catalogue settings update live buttons while preserving simultaneous theme matrices");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 1; window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 1;
        window.NavigateToCategory(0); gallery.Search("按钮控件对比"); ControlPreview.Flush();
        Check(card.IsVisible && !gallery.FindControl<Border>("SelectionControlsCard")!.IsVisible, "Global search locates the button comparison independently of selection controls"); gallery.Search(""); ControlPreview.Flush();
    }
    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        {
            var (window, _) = Open(1240, 1680, theme); EnhancementsPreview.Save(window, Path.Combine(output, $"button-comparison-{theme.Key!.ToString()!.ToLowerInvariant()}.png")); window.Close();
        }
        var (compact, _) = Open(760, 1060, ThemeVariant.Light); EnhancementsPreview.Save(compact, Path.Combine(output, "button-comparison-compact.png")); compact.Close();
    }
    private static Point Centre(Control control, TopLevel host) => control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
    private static void Press(TopLevel host, Key key, PhysicalKey physical) { host.KeyPress(key, RawInputModifiers.None, physical, null); host.KeyRelease(key, RawInputModifiers.None, physical, null); ControlPreview.Flush(); }
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
}
