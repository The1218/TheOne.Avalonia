using System.Diagnostics;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;
using TheOne.Avalonia.Themes;

namespace TheOne.DesignPreview;

internal static class ThemeSwitchPerformancePreview
{
    internal static void Verify(Action<bool, string> check, string? output)
    {
        var app = Application.Current!;
        var theme = app.Styles.OfType<OneTheme>().Single();
        var original = theme.ColorTheme;
        var window = new MainWindow { Width = 1100, Height = 800 };
        Motion.SetEnabled(window, false);
        window.Show();
        ThemeAppearancePreview.Flush();
        var colors = Enum.GetValues<ThemeColor>();
        var samples = new List<Sample>();
        var blocks = window.GetVisualDescendants().OfType<PreviewCodeBlock>().ToArray();
        var resourceChanges = 0;

        void Changed(object? sender, ResourcesChangedEventArgs args)
        {
            resourceChanges++;
        }

        try
        {
            foreach (var color in colors)
            {
                theme.ColorTheme = color;
                ThemeAppearancePreview.Flush();
            }

            var editorRefreshes = blocks.Sum(block => block.EditorPaletteRefreshes);
            app.ResourcesChanged += Changed;
            for (var cycle = 0; cycle < 3; cycle++)
                foreach (var color in colors)
                {
                    resourceChanges = 0;
                    var allocation = GC.GetAllocatedBytesForCurrentThread();
                    var timer = Stopwatch.StartNew();
                    OneTheme.SetColor(color);
                    var applyMs = timer.Elapsed.TotalMilliseconds;
                    ThemeAppearancePreview.Flush();
                    samples.Add(new Sample(color.ToString(), applyMs, timer.Elapsed.TotalMilliseconds,
                        resourceChanges, GC.GetAllocatedBytesForCurrentThread() - allocation));
                }

            check(window.FindControl<ControlGallery>("Gallery")!.CurrentView is HomePage,
                "ThemeColor/performance: repeated switches keep the live Home view");
            check(blocks.Sum(block => block.EditorPaletteRefreshes) == editorRefreshes,
                "ThemeColor/performance: 30 palette switches cause zero unrelated editor palette resets");
            if (output is not null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
                using var stream = File.Create(output);
                using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                writer.WriteStartArray();
                foreach (var sample in samples)
                {
                    writer.WriteStartObject();
                    writer.WriteString("Color", sample.Color);
                    writer.WriteNumber("ApplyMs", sample.ApplyMs);
                    writer.WriteNumber("FrameMs", sample.FrameMs);
                    writer.WriteNumber("ResourceChanges", sample.ResourceChanges);
                    writer.WriteNumber("AllocatedBytes", sample.AllocatedBytes);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }

            Console.WriteLine($"ThemeColor/performance: {samples.Count} measured switches; output={output}");
        }
        finally
        {
            app.ResourcesChanged -= Changed;
            window.Close();
            theme.ColorTheme = original;
            ThemeAppearancePreview.Flush();
        }
    }

    private sealed record Sample(
        string Color,
        double ApplyMs,
        double FrameMs,
        int ResourceChanges,
        long AllocatedBytes);
}