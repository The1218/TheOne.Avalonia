using global::Avalonia;
using global::Avalonia.Skia.Helpers;
using global::Avalonia.Threading;
using SkiaSharp;
using TheOne.Avalonia.Controls;

namespace TheOne.Avalonia.Reporting;

/// <summary>Exports an immutable definition snapshot through the same drawing path as ReportPreview.</summary>
public static class ReportPdfExporter
{
    public static async Task ExportAsync(ReportDocument definition, ReportDataSource? source, Stream destination, System.Net.Http.HttpClient? client = null, CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess(); cancellationToken.ThrowIfCancellationRequested();
        var document = Snapshot(definition);
        await ReportResources.LoadImagesAsync(document, client, cancellationToken, source);
        var layout = ReportPaginator.Paginate(document, source);
        await RenderAsync(document, layout, destination, cancellationToken);
    }
    /// <summary>Exports the exact loaded records and pictures already displayed by the viewer.</summary>
    internal static Task ExportSnapshotAsync(ReportDocument definition, ReportLayout layout, Stream destination, CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess(); cancellationToken.ThrowIfCancellationRequested();
        var document = Snapshot(definition);
        if (document.Elements.Any(e => e.Kind == ReportElementKind.Image && e.ImageUrl.Length > 0 &&
            (e.ResolvedImageUrl != e.ImageUrl || e.ResolvedImageData.Length == 0)))
            throw new InvalidOperationException("报表图片尚未完成加载，无法导出当前预览。");
        return RenderAsync(document, layout, destination, cancellationToken);
    }
    internal static ReportDocument Snapshot(ReportDocument definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        var snapshot = ReportDocument.Load(definition.Save());
        for (var i = 0; i < definition.Elements.Count; i++)
        {
            var element = definition.Elements[i]; var copy = snapshot.Elements[i];
            if (element.Kind == ReportElementKind.Image && element.ImageUrl.Length > 0 &&
                element.ResolvedImageUrl == element.ImageUrl && element.ResolvedImageData.Length > 0)
            { copy.ResolvedImageUrl = element.ResolvedImageUrl; copy.ResolvedImageData = element.ResolvedImageData; }
        }
        return snapshot;
    }
    private static async Task RenderAsync(ReportDocument document, ReportLayout layout, Stream destination, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.CanWrite) throw new ArgumentException("PDF 目标流不可写。", nameof(destination));
        using var buffer = new MemoryStream();
        using (var pdf = SKDocument.CreatePdf(buffer) ?? throw new InvalidOperationException("无法创建 PDF 文档。"))
        {
            var surface = new ReportPageSurface { IsPrinting = true }; surface.SetDocument(document, null);
            try
            {
                for (var i = 0; i < layout.Pages.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // The helper resets SKCanvas transforms; put DIP-to-point scaling in the visual itself.
                    surface.Update(layout, true, i, -1, .75, null);
                    surface.Measure(new Size(surface.Width, surface.Height)); surface.Arrange(new Rect(surface.DesiredSize));
                    var canvas = pdf.BeginPage((float)(layout.PageWidth * .75), (float)(layout.PageHeight * .75));
                    await DrawingContextHelper.RenderAsync(canvas, surface);
                    pdf.EndPage();
                }
                pdf.Close();
            }
            finally { surface.ClearImages(); }
        }
        cancellationToken.ThrowIfCancellationRequested();
        buffer.Position = 0; if (destination.CanSeek) { destination.Position = 0; destination.SetLength(0); }
        await buffer.CopyToAsync(destination, cancellationToken);
    }
}
