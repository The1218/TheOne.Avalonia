using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

/// <summary>Checks the new comparison cards and their labels, not the controls' internal themes.</summary>
internal static class ControlStyleShowcasePreview
{
    private static readonly string[] Names =
    [
        "Button", "RepeatButton", "ToggleButton", "IconButton", "SplitButton", "ToggleSplitButton",
        "TextBox", "NumericUpDown", "AutoCompleteBox", "ComboBox", "ListBox", "MultiSelectBox",
        "CheckBox", "RadioButton", "ToggleSwitch", "Slider", "ProgressBar", "DatePicker", "CalendarDatePicker", "TimePicker",
        "Label", "SelectableTextBlock", "InfoCard", "TabControl", "Tag", "Description", "DataGrid"
    ];

    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        check(ControlStyleShowcase.Create("GanttChart") is null && ControlStyleShowcase.Create("WorkflowEditor") is null,
            "Style showcase/scope: complex controls retain their complete original demonstrations");
        foreach (var name in Names)
        {
            var root = (StackPanel)ControlStyleShowcase.Create(name)!;
            var scroll = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = root
            };
            var window = new Window
            {
                Width = 1000, Height = 720, WindowDecorations = WindowDecorations.None,
                Content = new Border { Padding = new Thickness(16), Child = scroll }
            };
            Motion.SetEnabled(window, false);
            window.Show(); Flush();
            try
            {
                var samples = root.GetVisualDescendants().OfType<Control>()
                    .Where(control => control.Name?.StartsWith("Variant_", StringComparison.Ordinal) == true).ToArray();
                check(samples.Length >= 3 && samples.All(sample => sample.Name!.StartsWith("Variant_" + name + "_", StringComparison.Ordinal)),
                    $"Style showcase/{name}/identity: comparison cells contain the named real control type");
                check(samples.All(ConfigurationMatches) && ControlStyleShowcase.PublicStyles(name).All(style =>
                    samples.Any(sample => sample.Classes.Contains(style.Name) && Configuration(sample) == "Classes=\"" + style.Name + "\"")),
                    $"Style showcase/{name}/classes: each public class is directly labelled on its real independent example");
                check(Enum.GetValues<ControlSize>().All(size => samples.Any(sample =>
                        Configuration(sample) == "ControlSizing.Size=\"" + size + "\"" && ControlSizing.GetSize(sample) == size)),
                    $"Style showcase/{name}/sizes: Small, Normal and Large are simultaneously present with explicit size values");

                foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
                foreach (var size in Enum.GetValues<ControlSize>())
                foreach (var width in new[] { 1000, 360 })
                {
                    window.RequestedThemeVariant = theme; ControlSizing.SetSize(window, size); window.Width = width;
                    scroll.Offset = default; Flush();
                    var label = $"Style showcase/{name}/{theme.Key}/{size}/{width}";
                    var sections = root.Children.OfType<Border>().ToArray();
                    check(double.IsNaN(root.Height) && double.IsPositiveInfinity(root.MaxHeight) && root.Bounds.Height >= root.DesiredSize.Height - 1 &&
                        sections.All(section => double.IsNaN(section.Height) && double.IsPositiveInfinity(section.MaxHeight) && section.Bounds.Height >= section.DesiredSize.Height - 1) &&
                        sections.Zip(sections.Skip(1)).All(pair => pair.First.Bounds.Bottom <= pair.Second.Bounds.Top + .5),
                        label + ": comparison cards determine natural height without caps or overlapping sections");
                    check(Fits(root, scroll, height: false) && sections.All(section => Fits(section, root)) && samples.All(SampleFits),
                        label + ": every sample and wrapping cell stays inside its measured page width and height");
                    check(samples.All(sample => sample.Bounds.Width > 0 && sample.Bounds.Height > 0) &&
                        Enum.GetValues<ControlSize>().All(explicitSize => samples.Any(sample =>
                            Configuration(sample) == "ControlSizing.Size=\"" + explicitSize + "\"" && ControlSizing.GetSize(sample) == explicitSize)),
                        label + ": all variants have visible bounds and explicit sizes survive inherited density changes");
                    using var frame = window.CaptureRenderedFrame();
                    check(frame is not null && PreviewRenderedPixels.UniqueColors(frame) > 24,
                        label + ": comparison labels and actual examples draw themed pixels");
                    if (output is not null && frame is not null && size == ControlSize.Normal &&
                        (name is "Button" or "TextBox" or "MultiSelectBox") && (width == 360 || theme == ThemeVariant.Light))
                        frame.Save(Path.Combine(output, $"style-{name}-{theme.Key}-{width}.png"), PngBitmapEncoderOptions.Default);
                }
            }
            finally { window.Close(); Flush(); }
        }
    }

    private static string Configuration(Control sample) => ((StackPanel)sample.Parent!).Children.OfType<TextBlock>().ElementAt(1).Text ?? "";

    private static bool ConfigurationMatches(Control sample)
    {
        var text = Configuration(sample);
        if (!text.StartsWith("Classes=\"", StringComparison.Ordinal)) return true;
        var name = text[9..^1];
        return name.Length == 0 || sample.Classes.Contains(name);
    }

    private static bool SampleFits(Control sample)
    {
        if (sample.Parent is not StackPanel cell || cell.Parent is not Decorator wrapper || wrapper.Parent is not WrapPanel wrap) return false;
        return Fits(sample, cell) && Fits(cell, wrapper) && Fits(wrapper, wrap) &&
            cell.Children.All(child => Fits(child, cell)) && double.IsNaN(wrapper.Height) && double.IsPositiveInfinity(wrapper.MaxHeight);
    }

    private static bool Fits(Control child, Control parent, bool height = true)
    {
        var topLeft = child.TranslatePoint(default, parent);
        var bottomRight = child.TranslatePoint(new Point(child.Bounds.Width, child.Bounds.Height), parent);
        return topLeft is { } top && bottomRight is { } bottom && double.IsFinite(bottom.X) && double.IsFinite(bottom.Y) &&
            top.X >= -1 && bottom.X <= parent.Bounds.Width + 1 &&
            (!height || top.Y >= -1 && bottom.Y <= parent.Bounds.Height + 1);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs();
    }
}
