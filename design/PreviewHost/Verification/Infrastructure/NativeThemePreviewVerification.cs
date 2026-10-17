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

/// <summary>Verifies only the compact default-theme and project-feature presentations added to PreviewHost.</summary>
internal static class NativeThemePreviewVerification
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        var defaults = PreviewPageContentPolicy.All.Where(plan => plan.Mode == PreviewPageContentMode.DefaultTheme)
            .ToArray();
        check(defaults.Length == 17 && defaults.All(plan => !plan.ShowOriginalDemo && !plan.ShowStyleShowcase),
            "Native theme/policy: native-only pages contain one default preview instead of full native API demonstrations");
        foreach (var plan in defaults)
            VerifySample(plan.Name, NativeThemePreview.Create(plan.Name), false, check, output);
        VerifySample("TabControl", NativeThemePreview.CreateProjectFeature("TabControl"), true, check, output);
        VerifyTabClosing(check);
    }

    private static void VerifySample(string name, NativeThemePreview preview, bool projectFeature,
        Action<bool, string> check, string? output)
    {
        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = preview
        };
        var window = new Window
        {
            Width = 720, Height = 720, WindowDecorations = WindowDecorations.None,
            Content = new Border { Padding = new Thickness(16), Child = scroll }
        };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        try
        {
            var frame = (Border)preview.Content!;
            var sections = (StackPanel)frame.Child!;
            var heading = (Border)sections.Children[0];
            var body = (Border)sections.Children[1];
            var sample = body.Child!;
            var headingText = ((TextBlock)heading.Child!).Text;
            check(headingText is not null &&
                  headingText.Contains(projectFeature ? "TabFeatures" : "默认主题预览", StringComparison.Ordinal) &&
                  (projectFeature || sample.Name == "DefaultTheme_" + name),
                $"Native theme/{name}/identity: the labelled card contains its real default or project-feature example");
            var captions = preview.GetVisualDescendants().OfType<TextBlock>().Select(text => text.Text ?? "")
                .Where(text => text.Contains("=\"", StringComparison.Ordinal)).ToArray();
            check(projectFeature
                    ? captions.Length == 1 &&
                      captions[0].StartsWith("Classes=\"card\" · TabFeatures.", StringComparison.Ordinal) &&
                      !captions[0].Contains("IsChecked", StringComparison.Ordinal)
                    : captions.Length == 0,
                $"Native theme/{name}/captions: only project-specific Classes and TabFeatures have code captions");

            foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            foreach (var size in Enum.GetValues<ControlSize>())
            foreach (var width in new[] { 720, 360 })
            {
                window.RequestedThemeVariant = theme;
                ControlSizing.SetSize(window, size);
                window.Width = width;
                scroll.Offset = default;
                Flush();
                var label = $"Native theme/{name}/{theme.Key}/{size}/{width}";
                check(double.IsNaN(preview.Height) && double.IsPositiveInfinity(preview.MaxHeight) &&
                      double.IsNaN(frame.Height) && double.IsPositiveInfinity(frame.MaxHeight) &&
                      preview.Bounds.Height >= preview.DesiredSize.Height - 1 &&
                      frame.Bounds.Height >= frame.DesiredSize.Height - 1,
                    label + ": the card grows naturally without fixing its overall height");
                check(Fits(preview, scroll, false) && Fits(frame, preview) && Fits(heading, sections) &&
                      Fits(body, sections) && Fits(sample, body) &&
                      heading.Bounds.Bottom <= body.Bounds.Top + 1 &&
                      sections.Children.All(child => Fits(child, sections)),
                    label + ": heading and actual sample fit the viewport without clipping or overlapping each other");
                check(
                    sample.Bounds.Width > 0 && sample.Bounds.Height > 0 && heading.Child!.Bounds.Height > 0 &&
                    ControlSizing.GetSize(sample) == size,
                    label + ": the default visual remains nonempty and inherits the active density");
                using var pixels = window.CaptureRenderedFrame();
                check(pixels is not null && PreviewRenderedPixels.UniqueColors(pixels) > 24,
                    label + ": the real themed sample and its title draw pixels");
                if (output is not null && pixels is not null && size == ControlSize.Normal &&
                    ((name == "Calendar" && theme == ThemeVariant.Light && width == 360) ||
                     (name == "ScrollViewer" && theme == ThemeVariant.Dark && width == 360) ||
                     (projectFeature && theme == ThemeVariant.Light && width == 720)))
                    pixels.Save(Path.Combine(output, $"native-theme-{name}-{theme.Key}-{width}.png"),
                        PngBitmapEncoderOptions.Default);
            }
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyTabClosing(Action<bool, string> check)
    {
        var preview = NativeThemePreview.CreateProjectFeature("TabControl");
        var window = new Window { Width = 720, Height = 520, Content = new ScrollViewer { Content = preview } };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        try
        {
            var tabs = preview.GetVisualDescendants().OfType<TabControl>().Single();
            var original = tabs.Items.Cast<TabItem>().ToArray();
            check(
                original.Length == 3 && TabFeatures.GetCanClose(tabs) && TabFeatures.GetCanReorder(tabs) &&
                Virtualization.GetEnabled(tabs),
                "Native theme/TabFeatures: the focused example enables only the library's close, reorder and viewport features");
            tabs.SelectedIndex = 1;
            Flush();
            var selected = (TabItem)tabs.ContainerFromIndex(1)!;
            var close = selected.GetVisualDescendants().OfType<Button>()
                .Single(button => button.Name == "PART_CloseButton");
            check(close.IsEffectivelyVisible && close.IsEffectivelyEnabled && selected.IsSelected,
                "Native theme/TabFeatures: the selected real card header exposes a working close button");
            var point = close.TranslatePoint(new Point(close.Bounds.Width / 2, close.Bounds.Height / 2), window)!.Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Flush();
            check(tabs.Items.Count == 2 && !tabs.Items.Contains(original[1]) &&
                  ReferenceEquals(tabs.Items[0], original[0]) && ReferenceEquals(tabs.Items[1], original[2]),
                "Native theme/TabFeatures: a real pointer click removes only the selected item and preserves surviving instances");
            check(tabs.SelectedItem is TabItem remaining && tabs.Items.Contains(remaining) && remaining.IsSelected &&
                  ((TextBlock)remaining.Content!).Text is { Length: > 0 },
                "Native theme/TabFeatures: closing keeps valid native selection and displays the surviving item's content");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static bool Fits(Control child, Control parent, bool height = true)
    {
        var top = child.TranslatePoint(default, parent);
        var bottom = child.TranslatePoint(new Point(child.Bounds.Width, child.Bounds.Height), parent);
        return top is { } start && bottom is { } end && double.IsFinite(end.X) && double.IsFinite(end.Y) &&
               start.X >= -1 && end.X <= parent.Bounds.Width + 1 &&
               (!height || (start.Y >= -1 && end.Y <= parent.Bounds.Height + 1));
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}