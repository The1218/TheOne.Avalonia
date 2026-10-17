using Avalonia.Controls;
using Avalonia.Threading;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;
using TheOne.PrintingTools.Avalonia;

namespace TheOne.DesignPreview;

internal static class PreviewPrinting
{
    internal static ReportPreview CreatePreviewSample()
    {
        var viewer = ReportDesignDemo.CreatePreviewSample();
        viewer.PrintHandler = (path, options, token) => PrintReportAsync(viewer, path, options, token);
        return viewer;
    }

    internal static async Task<ReportPrintResult> PrintReportAsync(ReportPreview viewer, string path,
        ReportPrintOptions? options, CancellationToken cancellationToken, PrintService? service = null)
    {
        if (TopLevel.GetTopLevel(viewer) is not Window { IsVisible: true } owner)
            throw new InvalidOperationException("请在报表所属窗口打开后打印。");
        cancellationToken.ThrowIfCancellationRequested();
        using var document = await PrintSource.FromFile(path).CreateDocumentAsync(cancellationToken);
        var dialog = CreateWindow(viewer, document, options, service);
        using var registration = cancellationToken.Register(() => Dispatcher.UIThread.Post(dialog.Cancel));
        var result = await dialog.ShowDialog<PrintResult?>(owner);
        cancellationToken.ThrowIfCancellationRequested();
        return new ReportPrintResult((result ?? PrintResult.Cancelled).Message, false);
    }

    internal static PrintPreviewWindow CreateWindow(ReportPreview viewer, PrintDocument document,
        ReportPrintOptions? options = null, PrintService? service = null)
    {
        var dialog = new PrintPreviewWindow(document, service, new PrintSettings
        {
            Paper = document.Paper, Margins = PrintMargins.Zero, Copies = options?.Copies ?? 1,
            PrinterId = options?.PrinterName
        }) { RequestedThemeVariant = viewer.ActualThemeVariant };
        // Keep this package's Fluent base and print resources local to its dialog.
        dialog.Styles.Add(new PrintingToolsTheme());
        dialog.Preview.Density = ControlSizing.GetSize(viewer) switch
        {
            ControlSize.Small => PrintDensity.Small, ControlSize.Large => PrintDensity.Large, _ => PrintDensity.Normal
        };
        return dialog;
    }
}