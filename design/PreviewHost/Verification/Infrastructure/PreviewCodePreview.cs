using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class PreviewCodePreview
{
    internal static void Verify(Action<bool, string> check, string? output)
    {
        foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 920, 320 })
        {
            const string text = "<Button Classes=\"primary\" Content=\"保存\" />\n<!-- 可选中并复制的 AXAML 示例 -->";
            var block = new PreviewCodeBlock { Text = text };
            var window = new Window { Width = width, Height = 280, RequestedThemeVariant = variant,
                Content = new Border { Padding = new Thickness(16), Child = block } };
            ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); ThemeAppearancePreview.Flush();
            try
            {
                var editor = block.Editor; var label = $"PreviewCode/{variant}/{size}/{width}";
                check(editor.IsReadOnly && editor.IsSyntaxHighlightingEnabled && !editor.IsDiagnosticsEnabled && editor.CodeType == CodeType.Axaml && editor.WordWrap,
                    $"{label}: package editor enables read-only highlighted AXAML with wrapping");
                check(editor.ActualThemeVariant == variant && editor.Density.ToString() == size.ToString() && double.IsFinite(editor.Bounds.Height) && editor.Bounds.Width <= width - 32 + 1,
                    $"{label}: theme, density and bounded viewport follow the host");
                editor.Focus(); ThemeAppearancePreview.Press(window, Key.A, KeyModifiers.Control); window.KeyTextInput("不可修改"); ThemeAppearancePreview.Flush();
                check(editor.Text == text, $"{label}: real text input cannot modify a read-only example");
                var copy = editor.GetVisualDescendants().OfType<Button>().Single(button => button.Name == "PART_CopyButton");
                ThemeAppearancePreview.Click(window, copy);
                var clipboard = window.Clipboard!.TryGetTextAsync();
                while (!clipboard.IsCompleted) ThemeAppearancePreview.Flush();
                check(clipboard.GetAwaiter().GetResult() == text, $"{label}: actual copy button writes the complete example to the clipboard");
                block.CodeType = CodeType.CSharp; block.Text = "await OneToast.ShowAsync(\"保存完成\");"; ThemeAppearancePreview.Flush();
                check(editor.CodeType == CodeType.CSharp && editor.Text == block.Text && editor.HighlightingError is null, $"{label}: changing files updates C# language and content");
                using var frame = window.CaptureRenderedFrame()!;
                check(PreviewRenderedPixels.UniqueColors(frame) > 64, $"{label}: real rendering includes code syntax and text");
                if (output is not null) { Directory.CreateDirectory(output); frame.Save(Path.Combine(output, $"code-{variant}-{size}-{width}.png"), PngBitmapEncoderOptions.Default); }
            }
            finally { window.Close(); ThemeAppearancePreview.Flush(); }
        }
        VerifySourceNavigation(check, output);
    }
    private static void VerifySourceNavigation(Action<bool, string> check, string? output)
    {
        var window = new MainWindow(); Motion.SetEnabled(window, false); window.Show(); ThemeAppearancePreview.Flush();
        try
        {
            var gallery = window.FindControl<ControlGallery>("Gallery")!;
            foreach (var id in new[] { 0, 1, 0, 322, 319, 0 })
            {
                window.NavigateToCategory(id); ThemeAppearancePreview.Flush();
                var panel = gallery.FindControl<PreviewSourcePanel>("SourcePanel")!;
                var expander = panel.GetVisualDescendants().OfType<Expander>().Single();
                check(!expander.IsExpanded, $"PreviewCode/source/{id}: navigation starts with a collapsed lazy source panel");
                expander.IsExpanded = true; ThemeAppearancePreview.Flush();
                var picker = panel.GetVisualDescendants().OfType<ComboBox>().Single();
                var files = picker.ItemsSource!.Cast<PreviewSourceFile>().ToArray();
                check(files.Length > 0 && files.All(file => file.Text.Length > 0), $"PreviewCode/source/{id}: embedded source is available without repository paths");
                check(files.Any(file => file.Language == CodeType.CSharp), $"PreviewCode/source/{id}: C# source is included");
                if (id is 0 or 322) check(files.Any(file => file.Language == CodeType.Axaml), $"PreviewCode/source/{id}: AXAML source is included");
                picker.SelectedIndex = files.Length - 1; ThemeAppearancePreview.Flush();
                var editor = panel.GetVisualDescendants().OfType<CodeEditor>().Single();
                check(editor.Text == files[^1].Text,
                    $"PreviewCode/source/{id}: file selection updates the actual highlighted editor");
                window.Width = 680; window.Height = 540; ThemeAppearancePreview.Flush();
                check(editor.Bounds.Height <= gallery.Bounds.Height * .45 + 1 && gallery.FindControl<ScrollViewer>("PageScroll")!.Bounds.Height > 0,
                    $"PreviewCode/source/{id}: a short viewport retains both the source and the live preview");
                if (output is not null)
                {
                    using var frame = window.CaptureRenderedFrame()!;
                    frame.Save(Path.Combine(output, $"shell-source-{id}.png"), PngBitmapEncoderOptions.Default);
                }
            }
        }
        finally { window.Close(); ThemeAppearancePreview.Flush(); }
    }
}
