using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;
using Ellipse = Avalonia.Controls.Shapes.Ellipse;
using static TheOne.DesignPreview.InteractionRepairsPreview;
using Path = Avalonia.Controls.Shapes.Path;

namespace TheOne.DesignPreview;

internal static class SelectionControlsPreview
{
    private static (Window Window, SelectionControlsDemoView Demo) Open(int width, int height, ThemeVariant theme)
    {
        var demo = new SelectionControlsDemoView();
        var window = new Window
        {
            Width = width, Height = height, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = new Border { Padding = new Thickness(24), Child = demo }
            }
        };
        Motion.SetEnabled(window, false);
        window.Show();
        ControlPreview.Flush();
        return (window, demo);
    }

    public static void Verify()
    {
        var (window, demo) = Open(1240, 1900, ThemeVariant.Light);
        Check(demo.Samples.Count == 150 && demo.Samples.Count(sample => sample.Selected is null) == 6,
            "Selection comparison contains all 144 fixed state cells and six mixed CheckBox samples");
        foreach (var sample in demo.Samples)
        {
            var control = sample.Control;
            var toggle = (ToggleButton)control;
            var label =
                $"Comparison/{sample.Theme}/{sample.Size}/{control.StyleKey.Name}/{sample.State}/{sample.Selected}";
            var height = sample.Size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
            var font = sample.Size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
            Check(
                control.ActualThemeVariant.Key!.ToString() == sample.Theme && toggle.FontSize == font &&
                control.Bounds.Height == height,
                $"{label}: the fixed cell resolves its own theme and density");
            var indicator = control switch
            {
                CheckBox checkboxSample => (Control)Part<Border>(checkboxSample, "NormalRectangle"),
                RadioButton radio => Part<Ellipse>(radio, "CheckOuterEllipse"),
                _ => Part<Border>(control, "SwitchKnobBounds")
            };
            Check(
                Math.Abs(indicator.TranslatePoint(new Point(indicator.Bounds.Width / 2, indicator.Bounds.Height / 2),
                    control)!.Value.Y - control.Bounds.Height / 2) < 0.1,
                $"{label}: markers and tracks share the row's vertical center at every density");
            Check(toggle.IsChecked == sample.Selected && control.IsEnabled ==
                                                      (sample.State != SelectionVisualState.Disabled)
                                                      && control.Classes.Contains(":pointerover") ==
                                                      sample.State is SelectionVisualState.Hover
                                                          or SelectionVisualState.Pressed
                                                      && control.Classes.Contains(":pressed") ==
                                                      (sample.State == SelectionVisualState.Pressed)
                                                      && !control.Focusable &&
                                                      !KeyboardNavigation.GetIsTabStop(control),
                $"{label}: comparison states stay fixed and outside keyboard navigation");
            VerifyPalette(sample, label);
        }

        Check(Grid.GetColumn(demo.FindControl<ThemeVariantScope>("DarkMatrixScope")!) == 1,
            "Wide selection comparison places Light and Dark side by side");
        var check = demo.FindControl<CheckBox>("LiveCheck")!;
        var radioFirst = demo.FindControl<RadioButton>("LiveRadioFirst")!;
        var radioSecond = demo.FindControl<RadioButton>("LiveRadioSecond")!;
        var toggleSwitch = demo.FindControl<ToggleSwitch>("LiveSwitch")!;
        Click(window, check);
        Check(check.IsChecked is null && demo.FindControl<TextBlock>("SelectionStatus")!.Text!.Contains("部分选择"),
            "Comparison live CheckBox can enter its native mixed state and update the status");
        Click(window, check);
        Check(check.IsChecked == false, "Comparison live CheckBox cycles to unchecked");
        Click(window, radioSecond);
        Check(radioSecond.IsChecked == true && radioFirst.IsChecked == false,
            "Comparison live RadioButtons remain mutually exclusive");
        Click(window, toggleSwitch);
        Check(toggleSwitch.IsChecked == false, "Comparison live ToggleSwitch turns off through real pointer input");
        toggleSwitch.Focus();
        window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
        window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
        ControlPreview.Flush();
        Check(toggleSwitch.IsChecked == true, "Comparison live ToggleSwitch preserves native keyboard interaction");
        Click(window, demo.FindControl<CheckBox>("SelectionInteractionEnabled")!);
        Click(window, check);
        Click(window, radioFirst);
        Click(window, toggleSwitch);
        Check(
            !check.IsEffectivelyEnabled && check.IsChecked == false && radioSecond.IsChecked == true &&
            toggleSwitch.IsChecked == true,
            "Disabling the comparison live region prevents all three controls from changing");
        Click(window, demo.FindControl<Button>("ResetSelectionDemo")!);
        Check(
            check.IsEffectivelyEnabled && check.IsChecked == true && radioFirst.IsChecked == true &&
            radioSecond.IsChecked == false && toggleSwitch.IsChecked == true,
            "Comparison reset restores the live selection state");
        window.RequestedThemeVariant = ThemeVariant.Dark;
        ControlSizing.SetSize(window, ControlSize.Large);
        ControlPreview.Flush();
        Check(
            check.FontSize == 16 && Colour(check.Foreground) == Color.Parse("#F2F2F2") && demo.Samples.All(sample =>
                sample.Control.ActualThemeVariant.Key!.ToString() == sample.Theme),
            "Live controls follow the host theme and size while comparison scopes keep both palettes");
        window.Width = 760;
        ControlPreview.Flush();
        Check(
            Grid.GetRow(demo.FindControl<ThemeVariantScope>("DarkMatrixScope")!) == 1 &&
            Grid.GetColumn(demo.FindControl<ThemeVariantScope>("DarkMatrixScope")!) == 0,
            "Compact selection comparison stacks the two theme regions");
        Check(
            demo.Samples.All(sample =>
                sample.Control.Bounds.Width > 0 &&
                sample.Control.GetVisualAncestors().OfType<Grid>().First().Bounds.Width <= demo.Bounds.Width),
            "Compact state cells retain finite widths inside the comparison viewport");
        window.Close();
    }

    private static void VerifyPalette(SelectionComparisonSample sample, string label)
    {
        var control = sample.Control;
        var disabled = sample.State == SelectionVisualState.Disabled;
        var selected = sample.Selected != false;
        IBrush? background;
        IBrush? indicator;
        IBrush? mark;
        IBrush? foreground;
        string fill;
        string glyph;
        if (control is CheckBox check)
        {
            background = Part<Border>(check, "PART_Border").Background;
            indicator = Part<Border>(check, "NormalRectangle").Background;
            mark = Part<Path>(check, "CheckGlyph").Fill;
            foreground = Part<ContentPresenter>(check, "PART_ContentPresenter").Foreground;
            fill = disabled ? selected || control is CheckBox ? "One.DisabledSelection" : "One.DisabledSurface"
                : selected ? sample.State switch
                {
                    SelectionVisualState.Hover => "One.AccentHover",
                    SelectionVisualState.Pressed => "One.AccentPressed", _ => "One.Accent"
                }
                : sample.State switch
                {
                    SelectionVisualState.Hover => "One.NeutralHover",
                    SelectionVisualState.Pressed => "One.NeutralPressed", _ => "One.Subtle"
                };
            glyph = selected ? "One.IconOnAccent" : disabled ? "One.DisabledText" : "One.Text";
        }
        else if (control is RadioButton radio)
        {
            background = Part<Border>(radio, "RootBorder").Background;
            indicator = Part<Ellipse>(radio, selected ? "CheckOuterEllipse" : "OuterEllipse").Fill;
            mark = Part<Ellipse>(radio, "CheckGlyph").Fill;
            foreground = Part<ContentPresenter>(radio, "PART_ContentPresenter").Foreground;
            fill = disabled ? selected || control is CheckBox ? "One.DisabledSelection" : "One.DisabledSurface"
                : selected ? sample.State switch
                {
                    SelectionVisualState.Hover => "One.AccentHover",
                    SelectionVisualState.Pressed => "One.AccentPressed", _ => "One.Accent"
                }
                : sample.State switch
                {
                    SelectionVisualState.Hover => "One.NeutralHover",
                    SelectionVisualState.Pressed => "One.NeutralPressed", _ => "One.Subtle"
                };
            glyph = "One.IconOnAccent";
        }
        else
        {
            var toggle = (ToggleSwitch)control;
            background = Part<Grid>(toggle, "RootGrid").Background;
            Check(Colour(Part<Grid>(toggle, "SwitchAreaGrid").Background)?.A == 0,
                $"{label}: switch area has no state background plate");
            indicator = Part<Border>(toggle, selected ? "SwitchKnobBounds" : "OuterBorder").Background;
            Check(
                !disabled || Part<Border>(toggle, selected ? "SwitchKnobBounds" : "OuterBorder").BorderThickness.Left >
                0,
                $"{label}: disabled tracks retain an outline against a matching surface");
            mark = Part<Ellipse>(toggle, selected ? "SwitchKnobOn" : "SwitchKnobOff").Fill;
            foreground =
                (selected
                    ? Part<ContentPresenter>(toggle, "PART_OnContentPresenter")
                    : Part<ContentPresenter>(toggle, "PART_OffContentPresenter")).Foreground;
            fill = disabled ? "One.DisabledSelection" : selected ? "One.Accent" : "One.Line";
            glyph = disabled
                ? "One.PrimaryDisabled"
                : sample.State switch
                {
                    SelectionVisualState.Hover => "One.SurfaceHover",
                    SelectionVisualState.Pressed => "One.SurfacePressed", _ => "One.Surface"
                };
        }

        var text = disabled ? "One.DisabledText" : "One.Text";
        Check(
            Colour(background)?.A == 0 && Colour(indicator) == Resource(control, fill) &&
            Colour(mark) == Resource(control, glyph) && Colour(foreground) == Resource(control, text),
            $"{label}: native semantic templates keep transparent rows, visible indicators and readable labels");
    }

    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        {
            var (window, _) = Open(1240, 2100, theme);
            EnhancementsPreview.Save(window,
                System.IO.Path.Combine(output,
                    $"selection-comparison-{theme.Key!.ToString()!.ToLowerInvariant()}.png"));
            window.Close();
        }

        var (compact, _) = Open(760, 1060, ThemeVariant.Light);
        EnhancementsPreview.Save(compact, System.IO.Path.Combine(output, "selection-comparison-compact.png"));
        compact.Close();
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