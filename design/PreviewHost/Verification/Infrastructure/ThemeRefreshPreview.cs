using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using TheOne.Avalonia.Themes;
using TheOne.Avalonia.Workflow;

namespace TheOne.DesignPreview;

internal static class ThemeRefreshPreview
{
    internal static void Verify(Action<bool, string> check, string? output)
    {
        var theme = Application.Current!.Styles.OfType<OneTheme>().Single(); var original = theme.ColorTheme;
        if (output is not null) Directory.CreateDirectory(output);
        try
        {
            foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            foreach (var size in Enum.GetValues<ControlSize>())
            foreach (var width in new[] { 900, 320 })
            {
                var start = new DateOnly(2026, 10, 1);
                var task = new GanttTask { Title = "主题任务", Start = start.AddDays(1), EndExclusive = start.AddDays(6), Progress = .6 };
                var gantt = new GanttChart { Height = 160, ViewStart = start, ViewEnd = start.AddDays(10), ItemsSource = new[] { task }, TaskListWidth = 120, DayWidth = 22 };
                var doc = new WorkflowDocument(); var node = new WorkflowNode { Title = "节点", X = 10, Y = 10, Width = 100 };
                doc.Nodes.Add(node); doc.Nodes.Add(new WorkflowNode { Title = "便签", X = 140, Y = 10, Width = 90, IsAnnotation = true });
                var editor = new WorkflowEditor { Document = doc, Height = 330, ShowNodeLibrary = false, ShowProperties = false };
                var swatches = new WrapPanel(); var gradients = new List<Border>();
                var caption = new TextBlock { Text = "Palette 12345", FontSize = 20, FontWeight = FontWeight.Bold };
                caption.Bind(TextBlock.ForegroundProperty, new global::Avalonia.Markup.Xaml.MarkupExtensions.DynamicResourceExtension("One.Text"));
                foreach (var category in new[] { "Subtle", "Vivid" })
                foreach (var name in new[] { "Brand", "Cool", "Natural", "Warm" })
                {
                    var border = new Border { Width = 60, Height = 24, Margin = new Thickness(2), Tag = $"One.Gradient.{category}.{name}" };
                    border.Bind(Border.BackgroundProperty, new global::Avalonia.Markup.Xaml.MarkupExtensions.DynamicResourceExtension(border.Tag));
                    gradients.Add(border); swatches.Children.Add(border);
                }
                var window = new Window { Width = width, Height = 650, RequestedThemeVariant = variant,
                    Content = new StackPanel { Margin = new Thickness(12), Spacing = 8, Children = { caption, gantt, editor, swatches } } };
                ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); ThemeAppearancePreview.Flush();
                editor.SelectedNode = node; ThemeAppearancePreview.Flush();
                var canvas = editor.GetVisualDescendants().OfType<WorkflowCanvas>().Single();
                var minimap = editor.GetVisualDescendants().OfType<WorkflowMiniMap>().Single();
                var nodeBorder = canvas.GetVisualDescendants().OfType<Border>().First(b => ReferenceEquals(b.Tag, node));
                var selectedEditor = editor.SelectedNodeEditor; var revision = doc.Revision; var pan = editor.Pan; var zoom = editor.Zoom;
                try
                {
                    foreach (var color in OneTheme.ColorThemes)
                    {
                        OneTheme.SetColor(color); ThemeAppearancePreview.Flush();
                        var label = $"ThemeRefresh/{color}/{variant}/{size}/{width}";
                        check(Colour(gantt.ProgressBrush) == Colour(Resource(gantt, "One.Accent")) && Colour(gantt.TaskBrush) == Colour(Resource(gantt, "One.AccentSoft")), label + ": Gantt aliases follow the active palette");
                        check(ReferenceEquals(selectedEditor, editor.SelectedNodeEditor) && doc.Revision == revision && editor.Pan == pan && editor.Zoom == zoom &&
                            canvas.GetVisualDescendants().OfType<Border>().Any(b => ReferenceEquals(b, nodeBorder)), label + ": recoloring retains workflow node controls, selection, document and viewport");
                        using var frame = window.CaptureRenderedFrame()!;
                        check(Count(frame, window, caption, Colour(Resource(caption, "One.Text"))!.Value) > 3, label + ": existing text glyphs repaint the live foreground");
                        check(Count(frame, window, gantt, Colour(gantt.ProgressBrush)!.Value) > 10, label + ": Gantt progress paints current pixels");
                        check(Count(frame, window, canvas, Colour(Resource(canvas, "One.Workflow.Canvas"))!.Value) > 300, label + ": workflow canvas repaints current pixels");
                        check(Count(frame, window, minimap, Colour(Resource(minimap, "One.AccentSoft"))!.Value) > 2, label + ": minimap repaints current palette");
                        foreach (var border in gradients)
                        {
                            var key = (string)border.Tag!;
                            check(border.Background is ILinearGradientBrush brush && brush.GradientStops.Count == 2 && brush.GradientStops[0].Color != brush.GradientStops[1].Color &&
                                window.TryFindResource(key + ".StartColor", variant, out var first) && Equals(first, brush.GradientStops[0].Color) &&
                                window.TryFindResource(key + ".EndColor", variant, out var last) && Equals(last, brush.GradientStops[1].Color), label + ": " + key + " supplies matching brush and color stops");
                        }
                        if (output is not null) frame.Save(Path.Combine(output, $"refresh-{color}-{variant}-{size}-{width}.png"), PngBitmapEncoderOptions.Default);
                    }
                }
                finally { window.Close(); }
            }
            VerifyEditor(check);
        }
        finally { OneTheme.SetColor(original); ThemeAppearancePreview.Flush(); }
    }

    private static void VerifyEditor(Action<bool, string> check)
    {
        var block = new PreviewCodeBlock { Text = "public class ThemeSample { }", CodeType = CodeType.CSharp };
        var window = new Window { Width = 460, Height = 180, Content = block, RequestedThemeVariant = ThemeVariant.Light };
        window.Show(); ThemeAppearancePreview.Flush();
        try
        {
            var count = block.EditorPaletteRefreshes;
            foreach (var color in OneTheme.ColorThemes) { OneTheme.SetColor(color); ThemeAppearancePreview.Flush(); }
            check(block.EditorPaletteRefreshes == count, "ThemeRefresh/editor: palette-only switches skip unrelated syntax palette resets");
            var resources = Application.Current!.Resources; const string key = "CodeEditor.Keyword";
            var had = resources.TryGetValue(key, out var old); var custom = Color.Parse("#C214D8");
            try
            {
                resources[key] = new SolidColorBrush(custom); ThemeAppearancePreview.Flush();
                using var frame = window.CaptureRenderedFrame()!;
                check(block.EditorPaletteRefreshes > count && Count(frame, window, block, custom) > 3,
                    "ThemeRefresh/editor: custom syntax resource overrides still repaint actual glyphs");
            }
            finally { if (had) resources[key] = old; else resources.Remove(key); ThemeAppearancePreview.Flush(); }
            var light = Colour(block.Editor.Foreground); window.RequestedThemeVariant = ThemeVariant.Dark; ThemeAppearancePreview.Flush();
            check(Colour(block.Editor.Foreground) != light && Colour(block.Editor.Background) == Colour(Resource(block, "One.Surface")),
                "ThemeRefresh/editor: Dark mode still updates text and One.Surface background");
        }
        finally { window.Close(); }
    }
    private static IBrush Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? (IBrush)value! : throw new InvalidOperationException(key);
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static int Count(Bitmap frame, Window window, Control control, Color color)
    {
        var origin = control.TranslatePoint(default, window)!.Value;
        var length = frame.PixelSize.Width * frame.PixelSize.Height * 4; var memory = Marshal.AllocHGlobal(length);
        try
        {
            var data = new byte[length]; frame.CopyPixels(new PixelRect(frame.PixelSize), memory, length, frame.PixelSize.Width * 4); Marshal.Copy(memory, data, 0, length);
            var bgra = frame.Format == PixelFormat.Bgra8888; var count = 0;
            for (var y = Math.Max(0, (int)origin.Y); y < Math.Min(frame.PixelSize.Height, origin.Y + control.Bounds.Height); y++)
            for (var x = Math.Max(0, (int)origin.X); x < Math.Min(frame.PixelSize.Width, origin.X + control.Bounds.Width); x++)
            { var i = (y * frame.PixelSize.Width + x) * 4; if (data[i] == (bgra ? color.B : color.R) && data[i + 1] == color.G && data[i + 2] == (bgra ? color.R : color.B)) count++; }
            return count;
        }
        finally { Marshal.FreeHGlobal(memory); }
    }
}
