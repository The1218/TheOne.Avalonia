using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class HomePreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 1240, 360 })
        {
            var home = new HomePage();
            var scroll = new ScrollViewer { Content = new Border { Padding = new Thickness(20), Child = home } };
            var window = new Window { Width = width, Height = 1100, RequestedThemeVariant = theme, Content = scroll };
            Motion.SetEnabled(window, false); ControlSizing.SetSize(window, size); window.Show(); Flush();
            var label = $"Home/{theme}/{size}/{width}";
            try
            {
                var features = home.FindControl<Grid>("Features")!;
                check(features.Children.Count == 6 && features.ColumnDefinitions.Count == (width == 360 ? 1 : 3), $"{label}: featured controls adapt to the available width");
                check(features.Children.All(c => c.Bounds.Right <= features.Bounds.Width + 1), $"{label}: featured examples fit without horizontal clipping");
                var quick = home.FindControl<Border>("QuickStart")!;
                check(Grid.GetRow(quick) == (width == 360 ? 1 : 0), $"{label}: quick-start content stacks on narrow viewports");
                var input = home.FindControl<TextBox>("WorkspaceInput")!;
                input.BringIntoView(); Flush(); input.Focus(); input.SelectAll(); window.KeyTextInput("首页验证"); Flush();
                var save = home.GetVisualDescendants().OfType<Button>().Single(b => b.Content as string == "保存设置");
                save.BringIntoView(); Flush(); Click(window, save);
                check(home.FindControl<TextBlock>("SampleStatus")!.Text!.Contains("首页验证") && home.FindControl<TextBlock>("SampleStatus")!.Text!.Contains("2 项"),
                    $"{label}: real text input and save pointer action update the sample");
                var requested = -1; home.ControlRequested += (_, id) => requested = id;
                var explore = home.FindControl<Button>("ExploreButton")!;
                explore.BringIntoView(); Flush(); Click(window, explore);
                check(requested == 322, $"{label}: primary action requests the real Form sample");
                check(home.GetVisualDescendants().OfType<PreviewCodeBlock>().Any(t => t.Text?.Contains("OneTheme") == true)
                    && home.GetVisualDescendants().OfType<PreviewCodeBlock>().Any(t => t.Text?.Contains("CompleteLoginAsync") == true),
                    $"{label}: usage snippets include theme setup and window handoff");
                foreach (var button in features.GetVisualDescendants().OfType<Button>().Where(button => button.Content as string == "查看完整示例 →"))
                    check(button.Tag is int id && PreviewPageCatalog.Find(id) is not null, $"{label}: featured link resolves to the catalog");
                using (var pixels = window.CaptureRenderedFrame()!)
                    check(PreviewRenderedPixels.UniqueColors(pixels) > 64, $"{label}: actual pixels contain rendered text and themed controls");
                if (output != null)
                {
                    Directory.CreateDirectory(output); scroll.Offset = default; Flush();
                    using var frame = window.CaptureRenderedFrame()!;
                    frame.Save(Path.Combine(output, $"home-{theme}-{size}-{width}.png"), PngBitmapEncoderOptions.Default);
                    using var full = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(home.Bounds.Width), (int)Math.Ceiling(home.Bounds.Height)), new Vector(96, 96));
                    full.Render(home); full.Save(Path.Combine(output, $"home-full-{theme}-{size}-{width}.png"), PngBitmapEncoderOptions.Default);
                }
            }
            finally { window.Close(); Flush(); }
        }
        var shell = new MainWindow(); Motion.SetEnabled(shell, false); shell.Show(); Flush();
        try
        {
            var gallery = shell.FindControl<ControlGallery>("Gallery")!;
            check(shell.Model.CurrentPage?.Id == 0 && gallery.CurrentView is HomePage, "Home/navigation: new PreviewHost windows open Home by default");
            check(shell.Model.NavigationRoots[0].Category == 0 && !shell.Model.NavigationRoots[0].IsGroup && shell.Model.NavigationRoots[0].Entries.Count == 0,
                "Home/navigation: Home is a standalone top-level menu item");
            var home = (HomePage)gallery.CurrentView!;
            Click(shell, home.FindControl<Button>("ExploreButton")!);
            check(shell.Model.CurrentPage?.Id == 322, "Home/navigation: actual primary click navigates through the shell");
            shell.NavigateToCategory(0); Flush();
            check(ReferenceEquals(gallery.CurrentView, home), "Home/navigation: returning Home preserves the cached live example");
            var report = home.GetVisualDescendants().OfType<Button>().Single(b => b.Content as string == "报表与打印");
            Click(shell, report);
            check(shell.Model.CurrentPage?.Id == 319 && gallery.CurrentView is TheOne.Avalonia.Controls.ReportPreview { PrintHandler: not null },
                "Home/navigation: report entry opens the sample configured for PrintingTools");
            shell.Model.Search = "Home";
            check(shell.Model.SearchResults.Any(p => p.Id == 0), "Home/navigation: global search includes Home");
        }
        finally { shell.Close(); Flush(); }
    }
    private static void Click(Window window, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}

internal static class PreviewRenderedPixels
{
    internal static int UniqueColors(Bitmap bitmap)
    {
        var bytes = new byte[bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4];
        var memory = System.Runtime.InteropServices.Marshal.AllocHGlobal(bytes.Length);
        try
        {
            bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), memory, bytes.Length, bitmap.PixelSize.Width * 4);
            System.Runtime.InteropServices.Marshal.Copy(memory, bytes, 0, bytes.Length);
            var colors = new HashSet<int>();
            for (var index = 0; index < bytes.Length; index += 4) colors.Add(BitConverter.ToInt32(bytes, index));
            return colors.Count;
        }
        finally { System.Runtime.InteropServices.Marshal.FreeHGlobal(memory); }
    }
}
