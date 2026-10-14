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

internal static class RadioButtonPreview
{
    private static (Window Window, RadioButton Selected, RadioButton Other, TextBlock Status) Open(ThemeVariant theme, ControlSize size)
    {
        var selected = new RadioButton { Content = "标准模式 / Selected", GroupName = "RadioPreview", IsChecked = true };
        var other = new RadioButton { Content = "紧凑模式 / Unselected", GroupName = "RadioPreview" };
        var status = new TextBlock { Text = "Normal / 默认", Foreground = ResourceBrush(theme, "One.Muted") };
        var panel = new StackPanel { Spacing = 16 };
        panel.Children.Add(new TextBlock { Text = $"RadioButton · {theme.Key} · {size}", FontWeight = FontWeight.SemiBold });
        panel.Children.Add(status); panel.Children.Add(selected); panel.Children.Add(other);
        var window = new Window { Width = 520, Height = 260, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = panel } };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        return (window, selected, other, status);
    }
    public static void Verify()
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, selected, other, _) = Open(theme, size);
            var label = $"RadioButton/{theme.Key}/{size}";
            var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
            var font = size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
            Check(selected.Bounds.Height == height && other.Bounds.Height == height && selected.FontSize == font && other.FontSize == font,
                $"{label}: native row height and typography follow all three sizes");
            Check(Part<Ellipse>(selected, "CheckOuterEllipse").Bounds.Size == new Size(20, 20) && Part<Ellipse>(selected, "CheckGlyph").Bounds.Size == new Size(8, 8),
                $"{label}: the circular selected indicator keeps its outer ring and inner mark");
            foreach (var isChecked in new[] { true, false })
            {
                var radio = isChecked ? selected : other; var sibling = isChecked ? other : selected;
                window.MouseMove(new Point(1, 1)); ControlPreview.Flush();
                VerifyState(radio, "normal", $"{label}/{isChecked}");
                Hover(window, radio); Check(radio.IsPointerOver, $"{label}/{isChecked}: real pointer input activates hover");
                VerifyState(radio, "hover", $"{label}/{isChecked}");
                var point = radio.TranslatePoint(new Point(radio.Bounds.Width / 2, radio.Bounds.Height / 2), window)!.Value;
                window.MouseDown(point, MouseButton.Left); ControlPreview.Flush();
                Check(radio.IsPressed, $"{label}/{isChecked}: real pointer input activates pressed state");
                VerifyState(radio, "pressed", $"{label}/{isChecked}");
                window.MouseUp(point, MouseButton.Left); ControlPreview.Flush();
                Check(radio.IsChecked == true && sibling.IsChecked == false, $"{label}/{isChecked}: clicking preserves native radio group exclusivity");
                selected.IsChecked = true; other.IsChecked = false; radio.IsEnabled = false; ControlPreview.Flush();
                VerifyState(radio, "disabled", $"{label}/{isChecked}");
                Click(window, radio);
                Check(radio.IsChecked == isChecked && !radio.IsPressed && selected.IsChecked == true, $"{label}/{isChecked}: disabled pointer input cannot change the selection");
                radio.IsEnabled = true; window.MouseMove(new Point(1, 1)); ControlPreview.Flush();
            }
            selected.Background = Brushes.Red; Hover(window, selected);
            Check(Part<Border>(selected, "RootBorder").Background == Brushes.Red, $"{label}: an explicit consumer background survives hover");
            window.Close();
        }
    }
    private static void VerifyState(RadioButton radio, string state, string label)
    {
        var disabled = state == "disabled"; var isChecked = radio.IsChecked == true;
        var ring = Part<Ellipse>(radio, isChecked ? "CheckOuterEllipse" : "OuterEllipse");
        var glyph = Part<Ellipse>(radio, "CheckGlyph");
        var fill = disabled ? isChecked ? "One.DisabledSelection" : "One.DisabledSurface" : isChecked ? state switch { "hover" => "One.AccentHover", "pressed" => "One.AccentPressed", _ => "One.Accent" }
            : state switch { "hover" => "One.NeutralHover", "pressed" => "One.NeutralPressed", _ => "One.Subtle" };
        var stroke = isChecked ? "One.Transparent" : "One.OutlineMedium";
        Check(Colour(Part<Border>(radio, "RootBorder").Background)?.A == 0 && ring.Opacity == 1 && glyph.Opacity == (isChecked ? 1 : 0),
            $"{label}/{state}: the row remains transparent while the circular selection mark retains its native visibility");
        Check(Colour(ring.Fill) == Resource(radio, fill) && Colour(ring.Stroke) == Resource(radio, stroke)
              && Colour(glyph.Fill) == Resource(radio, "One.IconOnAccent")
              && Colour(Part<ContentPresenter>(radio, "PART_ContentPresenter").Foreground) == Resource(radio, disabled ? "One.DisabledText" : "One.Text"),
            $"{label}/{state}: circle, mark and label use the correct semantic state palette");
    }
    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, selected, other, status) = Open(theme, size);
            var prefix = $"radio-{theme.Key!.ToString()!.ToLowerInvariant()}-{size.ToString().ToLowerInvariant()}";
            void Save(string state) => EnhancementsPreview.Save(window, Path.Combine(output, $"{prefix}-{state}.png"));
            Save("normal");
            foreach (var radio in new[] { selected, other })
            {
                var suffix = radio == selected ? "checked" : "unchecked";
                status.Text = $"Hover / 悬停 · {suffix}"; Hover(window, radio); Save($"{suffix}-hover");
                var point = radio.TranslatePoint(new Point(radio.Bounds.Width / 2, radio.Bounds.Height / 2), window)!.Value;
                window.MouseDown(point, MouseButton.Left); status.Text = $"Pressed / 按下 · {suffix}"; ControlPreview.Flush(); Save($"{suffix}-pressed");
                window.MouseUp(point, MouseButton.Left); selected.IsChecked = true; other.IsChecked = false; window.MouseMove(new Point(1, 1)); ControlPreview.Flush();
            }
            selected.IsEnabled = other.IsEnabled = false; status.Text = "Disabled / 禁用 · 两种选择状态"; ControlPreview.Flush(); Save("disabled");
            window.Close();
        }
    }
    private static IBrush? ResourceBrush(ThemeVariant theme, string key) => Application.Current!.TryFindResource(key, theme, out var value) ? value as IBrush : null;
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
}
