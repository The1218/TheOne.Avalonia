using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Ellipse = Avalonia.Controls.Shapes.Ellipse;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class ToggleSwitchPreview
{
    private static (Window Window, ToggleSwitch On, ToggleSwitch Off, TextBlock Status) Open(ThemeVariant theme, ControlSize size)
    {
        ToggleSwitch Create(bool isOn) => new() { OnContent = "通知已开启 / On", OffContent = "通知已关闭 / Off", IsChecked = isOn };
        var on = Create(true); var off = Create(false);
        var status = new TextBlock { Text = "Normal / 默认" };
        var panel = new StackPanel { Spacing = 16 };
        panel.Children.Add(new TextBlock { Text = $"ToggleSwitch · {theme.Key} · {size}", FontWeight = FontWeight.SemiBold });
        panel.Children.Add(status); panel.Children.Add(on); panel.Children.Add(off);
        var window = new Window { Width = 520, Height = 260, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = panel } };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        return (window, on, off, status);
    }
    public static void Verify()
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, on, off, _) = Open(theme, size); var label = $"ToggleSwitch/{theme.Key}/{size}";
            var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
            var font = size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
            Check(on.Bounds.Height == height && off.Bounds.Height == height && on.FontSize == font && off.FontSize == font,
                $"{label}: native row height and typography follow all three sizes");
            Check(Part<Border>(on, "SwitchKnobBounds").Bounds.Size == new Size(40, 20) && Part<Ellipse>(on, "SwitchKnobOn").Bounds.Size == new Size(10, 10),
                $"{label}: track and thumb retain their native geometry");
            foreach (var isOn in new[] { true, false })
            {
                var toggle = isOn ? on : off;
                window.MouseMove(new Point(1, 1)); ControlPreview.Flush(); VerifyState(toggle, "normal", $"{label}/{isOn}");
                Hover(window, toggle); Check(toggle.IsPointerOver, $"{label}/{isOn}: real pointer input activates hover");
                VerifyState(toggle, "hover", $"{label}/{isOn}");
                var point = Centre(toggle, window); window.MouseDown(point, MouseButton.Left); ControlPreview.Flush();
                Check(toggle.IsPressed, $"{label}/{isOn}: real pointer input activates pressed state");
                VerifyState(toggle, "pressed", $"{label}/{isOn}");
                window.MouseUp(point, MouseButton.Left); ControlPreview.Flush();
                Check(toggle.IsChecked == !isOn && Canvas.GetLeft(Part<Panel>(toggle, "PART_MovingKnobs")) == (isOn ? 0 : 20),
                    $"{label}/{isOn}: clicking toggles native state and moves the thumb to the opposite end");
                toggle.Focus(); window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null); window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null); ControlPreview.Flush();
                Check(toggle.IsChecked == isOn, $"{label}/{isOn}: Space preserves native keyboard toggling");
                toggle.IsEnabled = false; ControlPreview.Flush(); VerifyState(toggle, "disabled", $"{label}/{isOn}");
                Click(window, toggle); window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null); window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null); ControlPreview.Flush();
                Check(toggle.IsChecked == isOn && !toggle.IsPressed, $"{label}/{isOn}: disabled mouse and keyboard input cannot change the state");
                toggle.IsEnabled = true; window.MouseMove(new Point(1, 1)); ControlPreview.Flush();
            }
            Drag(window, off, 20); Check(off.IsChecked == true && Canvas.GetLeft(Part<Panel>(off, "PART_MovingKnobs")) == 20, $"{label}: native thumb dragging can turn the switch on");
            Drag(window, off, -20); Check(off.IsChecked == false && Canvas.GetLeft(Part<Panel>(off, "PART_MovingKnobs")) == 0, $"{label}: native thumb dragging can turn the switch off");
            on.Background = Brushes.Red; Hover(window, on);
            Check(Part<Grid>(on, "RootGrid").Background == Brushes.Red && Colour(Part<Grid>(on, "SwitchAreaGrid").Background)?.A == 0,
                $"{label}: explicit consumer background remains visible behind the transparent switch area");
            window.Close();
        }
    }
    private static void VerifyState(ToggleSwitch toggle, string state, string label)
    {
        var disabled = state == "disabled"; var isOn = toggle.IsChecked == true;
        var track = Part<Border>(toggle, isOn ? "SwitchKnobBounds" : "OuterBorder");
        var thumb = Part<Ellipse>(toggle, isOn ? "SwitchKnobOn" : "SwitchKnobOff");
        var fill = disabled ? "One.DisabledSurface" : isOn ? state switch { "hover" => "One.AccentHover", "pressed" => "One.AccentPressed", _ => "One.Accent" }
            : state switch { "hover" => "One.Subtle", "pressed" => "One.Line", _ => "One.Surface" };
        var stroke = disabled ? "One.DisabledText" : isOn ? "One.Accent" : "One.InputLine";
        Check(Colour(Part<Grid>(toggle, "RootGrid").Background)?.A == 0 && Colour(Part<Grid>(toggle, "SwitchAreaGrid").Background)?.A == 0 && track.Opacity == 1 && thumb.Opacity == 1,
            $"{label}/{state}: both row surfaces remain transparent while track and thumb stay visible");
        Check(Colour(track.Background) == Resource(toggle, fill) && Colour(track.BorderBrush) == Resource(toggle, stroke)
              && Colour(thumb.Fill) == Resource(toggle, disabled ? "One.DisabledText" : isOn ? "One.OnAccent" : "One.Muted")
              && Colour(toggle.Foreground) == Resource(toggle, disabled ? "One.DisabledText" : "One.Text"),
            $"{label}/{state}: track, thumb and label use the correct semantic state palette");
        Check(Canvas.GetLeft(Part<Panel>(toggle, "PART_MovingKnobs")) == (isOn ? 20 : 0)
              && Part<ContentPresenter>(toggle, "PART_OnContentPresenter").Opacity == (isOn ? 1 : 0)
              && Part<ContentPresenter>(toggle, "PART_OffContentPresenter").Opacity == (isOn ? 0 : 1)
              && (!disabled || Colour(thumb.Fill) != Colour(track.Background) && track.BorderThickness.Left > 0),
            $"{label}/{state}: native thumb position and displayed label match the state, including a distinct disabled thumb");
    }
    private static Point Centre(ToggleSwitch toggle, TopLevel host) => toggle.TranslatePoint(new Point(toggle.Bounds.Width / 2, toggle.Bounds.Height / 2), host)!.Value;
    private static void Drag(Window window, ToggleSwitch toggle, double delta)
    {
        var moving = Part<Panel>(toggle, "PART_MovingKnobs");
        var from = moving.TranslatePoint(new Point(10, 10), window)!.Value; var to = from + new Vector(delta, 0);
        window.MouseMove(from); window.MouseDown(from, MouseButton.Left); window.MouseMove(to, RawInputModifiers.LeftMouseButton); window.MouseUp(to, MouseButton.Left); ControlPreview.Flush();
    }
    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, on, off, status) = Open(theme, size);
            var prefix = $"toggle-switch-{theme.Key!.ToString()!.ToLowerInvariant()}-{size.ToString().ToLowerInvariant()}";
            void Save(string state) => EnhancementsPreview.Save(window, Path.Combine(output, $"{prefix}-{state}.png"));
            Save("normal");
            foreach (var toggle in new[] { on, off })
            {
                var suffix = toggle == on ? "on" : "off";
                status.Text = $"Hover / 悬停 · {suffix}"; Hover(window, toggle); Save($"{suffix}-hover");
                var point = Centre(toggle, window); window.MouseDown(point, MouseButton.Left); status.Text = $"Pressed / 按下 · {suffix}"; ControlPreview.Flush(); Save($"{suffix}-pressed");
                window.MouseUp(point, MouseButton.Left); on.IsChecked = true; off.IsChecked = false; window.MouseMove(new Point(1, 1)); ControlPreview.Flush();
            }
            on.IsEnabled = off.IsEnabled = false; status.Text = "Disabled / 禁用 · 开启与关闭"; ControlPreview.Flush(); Save("disabled");
            window.Close();
        }
    }
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
}
