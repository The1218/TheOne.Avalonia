using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;
using TheOne.PrintingTools.Avalonia;

namespace TheOne.DesignPreview;

internal static class PreviewPrintingPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 1120, 520 })
        {
            var viewer = PreviewPrinting.CreatePreviewSample();
            var owner = new Window { Width = 900, Height = 700, Content = viewer, RequestedThemeVariant = theme };
            Motion.SetEnabled(owner, false); ControlSizing.SetSize(owner, size); owner.Show(); Flush();
            var backend = new Backend(); var service = new PrintService(backend);
            var path = ""; var label = $"PreviewPrinting/{theme}/{size}/{width}";
            viewer.PrintHandler = (pdf, options, token) => { path = pdf; return PreviewPrinting.PrintReportAsync(viewer, pdf, options, token, service); };
            try
            {
                Pump(() => viewer.Layout is not null);
                var styleCount = Application.Current!.Styles.Count;
                var printing = viewer.PrintAsync(new ReportPrintOptions(Copies: 2));
                Pump(() => Dialog() is not null || printing.IsCompleted);
                if (printing.IsFaulted) printing.GetAwaiter().GetResult();
                var dialog = Dialog()!;
                check(dialog is not null && File.Exists(path), $"{label}: report snapshot opens the package preview while retaining its PDF");
                dialog!.Width = width; dialog.Height = 850; Flush();
                check(dialog.Preview.Document.PageCount == viewer.Layout!.Pages.Count && dialog.Preview.Document.PageCount > 1,
                    $"{label}: package imports every report PDF page");
                check(dialog.ActualThemeVariant == theme && dialog.Preview.Density == (size == ControlSize.Small ? PrintDensity.Small : size == ControlSize.Large ? PrintDensity.Large : PrintDensity.Normal)
                    && dialog.Styles.OfType<PrintingToolsTheme>().Count() == 1 && Application.Current.Styles.Count == styleCount,
                    $"{label}: print theme and density remain local to the owned dialog");
                check(!viewer.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "PrintButton").IsEnabled && backend.Submissions == 0,
                    $"{label}: preview alone submits no job and prevents duplicate report printing");
                Pump(() => dialog.Preview.GetVisualDescendants().OfType<Button>().Any(b => b.Content as string == "打印" && b.IsEffectivelyEnabled));
                var narrowTab = dialog.Preview.GetVisualDescendants().OfType<ComboBox>().FirstOrDefault(c => c.Name == "PrintNarrowTab" && c.IsVisible);
                if (narrowTab is not null) { narrowTab.SelectedIndex = 1; Flush(); }
                Pump(() => dialog.Preview.GetVisualDescendants().OfType<Image>().Any(i => i.Name == "PrintSheet" && i.Source is not null));
                check(dialog.Preview.GetVisualDescendants().OfType<Image>().Any(i => i.Name == "PrintSheet" && i.Bounds.Width > 100 && i.Bounds.Height > 100),
                    $"{label}: the package renders the report PDF onto an actual preview sheet");
                var sheet = dialog.Preview.GetVisualDescendants().OfType<Image>().Single(i => i.Name == "PrintSheet");
                check(sheet.Source is Bitmap bitmap && PreviewRenderedPixels.UniqueColors(bitmap) > 16,
                    $"{label}: rendered PDF pixels contain report content instead of a blank sheet");
                if (output != null)
                {
                    Directory.CreateDirectory(output);
                    using var frame = dialog.CaptureRenderedFrame()!;
                    frame.Save(Path.Combine(output, $"printing-{theme}-{size}-{width}.png"), PngBitmapEncoderOptions.Default);
                }
                var submit = dialog.Preview.GetVisualDescendants().OfType<Button>().Single(b => b.Content as string == "打印");
                submit.BringIntoView(); Flush();
                var point = submit.TranslatePoint(new Point(submit.Bounds.Width / 2, submit.Bounds.Height / 2), dialog)!.Value;
                dialog.MouseMove(point); dialog.MouseDown(point, MouseButton.Left); dialog.MouseUp(point, MouseButton.Left);
                Pump(() => printing.IsCompleted);
                var result = printing.GetAwaiter().GetResult();
                check(backend.Submissions == 1 && backend.Copies == 2 && !result.KeepDocument && !File.Exists(path),
                    $"{label}: real confirmation submits once to the test backend and releases the PDF");
                using var cancellation = new CancellationTokenSource();
                var cancelled = viewer.PrintAsync(cancellationToken: cancellation.Token);
                Pump(() => Dialog() is not null || cancelled.IsCompleted);
                cancellation.Cancel(); Pump(() => cancelled.IsCompleted);
                check(cancelled.IsCanceled && !File.Exists(path) && Dialog() is null && backend.Submissions == 1,
                    $"{label}: caller cancellation closes preview and cleans its snapshot without submission");
                viewer.PrintHandler = (_, _, _) => throw new InvalidOperationException("test printer error");
                var failed = viewer.PrintAsync(); Pump(() => failed.IsCompleted);
                check(failed.IsFaulted && viewer.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "PrintButton").IsEnabled,
                    $"{label}: a host printer failure restores the report's print action");
                _ = failed.Exception;
            }
            finally { Dialog()?.Close(); owner.Close(); Flush(); }
        }
    }
    private static PrintPreviewWindow? Dialog() => (Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.Windows.OfType<PrintPreviewWindow>().FirstOrDefault(w => w.IsVisible);
    private static void Pump(Func<bool> ready)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        while (!ready() && watch.ElapsedMilliseconds < 15000)
        {
            var frame = new DispatcherFrame(); var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; }; timer.Start();
            try { Dispatcher.UIThread.PushFrame(frame); } finally { timer.Stop(); }
            Flush();
        }
        Flush(); if (!ready()) throw new TimeoutException("PrintingTools preview did not settle.");
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private sealed class Backend : IPrintBackend
    {
        public int Submissions, Copies;
        public Task<IReadOnlyList<PrintPrinter>> GetPrintersAsync(CancellationToken token) => Task.FromResult<IReadOnlyList<PrintPrinter>>([new("test", "Verification queue", true)]);
        public Task<PrinterCapabilities> GetCapabilitiesAsync(string id, CancellationToken token) => Task.FromResult(new PrinterCapabilities([PrintPaper.A4], true, true, PrintMargins.Zero));
        public Task<PrintResult> PrintAsync(PrintDocument document, PrintSettings settings, PrinterCapabilities capabilities, CancellationToken token)
        { token.ThrowIfCancellationRequested(); Submissions++; Copies = settings.Copies; return Task.FromResult(new PrintResult("test-1", "测试队列已接收")); }
    }
}
