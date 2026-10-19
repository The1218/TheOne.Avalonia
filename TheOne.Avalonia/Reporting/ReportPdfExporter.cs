using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Skia.Helpers;
using Avalonia.Threading;
using SkiaSharp;
using TheOne.Avalonia.Controls;

namespace TheOne.Avalonia.Reporting;

/// <summary>Exports an immutable definition snapshot through the same drawing path as ReportPreview.</summary>
public static class ReportPdfExporter
{
    private static readonly ConditionalWeakTable<Stream, DestinationState> Destinations = new();

    private sealed class DestinationState
    {
        internal int InUse;
    }

    private sealed class DestinationLease(DestinationState state) : IDisposable
    {
        public void Dispose() => Volatile.Write(ref state.InUse, 0);
    }

    private static DestinationLease AcquireDestination(Stream destination)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.CanWrite) throw new ArgumentException("PDF 目标流不可写。", nameof(destination));
        var state = Destinations.GetValue(destination, static _ => new DestinationState());
        if (Interlocked.CompareExchange(ref state.InUse, 1, 0) != 0)
            throw new InvalidOperationException("同一目标流已有 PDF 导出任务；请等待完成或为并发任务使用独立流。");
        return new DestinationLease(state);
    }

    /// <summary>Stages a complete PDF; concurrent jobs require separate destination streams.</summary>
    /// <remarks>Cancellation stops before commit. Seekable destinations roll back on write failure;
    /// overwriting nonempty content requires read access. Nonseekable writes cannot be rolled back.</remarks>
    public static async Task ExportAsync(ReportDocument definition, ReportDataSource? source, Stream destination,
        HttpClient? client = null, CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess();
        cancellationToken.ThrowIfCancellationRequested();
        using var lease = AcquireDestination(destination);
        var document = Snapshot(definition);
        // Capture getter values and QR geometry before yielding for network images.
        // Each export keeps its own payloads even if the host edits shared rows meanwhile.
        var layout = ReportPaginator.Paginate(document, source);
        await ReportResources.LoadImagesAsync(document, client, cancellationToken, source);
        await RenderAsync(document, layout, destination, cancellationToken);
    }

    /// <summary>Exports the exact loaded records and pictures already displayed by the viewer.</summary>
    internal static async Task ExportSnapshotAsync(ReportDocument definition, ReportLayout layout, Stream destination,
        CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess();
        cancellationToken.ThrowIfCancellationRequested();
        using var lease = AcquireDestination(destination);
        var document = Snapshot(definition);
        if (document.Elements.Any(e => e.Kind == ReportElementKind.Image && e.ImageUrl.Length > 0 &&
                                       (e.ResolvedImageUrl != e.ImageUrl || e.ResolvedImageData.Length == 0)))
            throw new InvalidOperationException("报表图片尚未完成加载，无法导出当前预览。");
        await RenderAsync(document, layout, destination, cancellationToken);
    }

    internal static ReportDocument Snapshot(ReportDocument definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        var snapshot = ReportDocument.Load(definition.Save());
        for (var i = 0; i < definition.Elements.Count; i++)
        {
            var element = definition.Elements[i];
            var copy = snapshot.Elements[i];
            if (element.Kind == ReportElementKind.Image && element.ImageUrl.Length > 0 &&
                element.ResolvedImageUrl == element.ImageUrl && element.ResolvedImageData.Length > 0)
            {
                copy.ResolvedImageUrl = element.ResolvedImageUrl;
                copy.ResolvedImageData = element.ResolvedImageData;
            }
        }

        return snapshot;
    }

    private static async Task RenderAsync(ReportDocument document, ReportLayout layout, Stream destination,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.CanWrite) throw new ArgumentException("PDF 目标流不可写。", nameof(destination));
        ReportPaginator.RequireQRCodes(layout);
        using var buffer = new MemoryStream();
        using (var pdf = SKDocument.CreatePdf(buffer) ?? throw new InvalidOperationException("无法创建 PDF 文档。"))
        {
            var surface = new ReportPageSurface { IsPrinting = true };
            surface.SetDocument(document, null);
            try
            {
                for (var i = 0; i < layout.Pages.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // The helper resets SKCanvas transforms; put DIP-to-point scaling in the visual itself.
                    surface.Update(layout, true, i, -1, .75, null);
                    surface.Measure(new Size(surface.Width, surface.Height));
                    surface.Arrange(new Rect(surface.DesiredSize));
                    var canvas = pdf.BeginPage((float)(layout.PageWidth * .75), (float)(layout.PageHeight * .75));
                    await DrawingContextHelper.RenderAsync(canvas, surface);
                    pdf.EndPage();
                }

                pdf.Close();
            }
            finally
            {
                surface.ClearImages();
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        await CommitAsync(buffer, destination, cancellationToken);
    }

    private static async Task CommitAsync(MemoryStream buffer, Stream destination, CancellationToken cancellationToken)
    {
        var seekable = destination.CanSeek;
        var position = seekable ? destination.Position : 0;
        var length = seekable ? destination.Length : 0;
        // A nonempty seekable target needs a readable backup to undo a partial write.
        if (seekable && length > 0 && !destination.CanRead)
            throw new NotSupportedException("覆盖非空 PDF 目标需要可读写的流，以便写入失败时恢复原内容。");
        using var previous = seekable && destination.CanRead ? new MemoryStream() : null;
        if (previous is not null)
        {
            try
            {
                destination.Position = 0;
                await destination.CopyToAsync(previous, cancellationToken);
            }
            finally
            {
                destination.Position = position;
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            if (seekable) destination.Position = 0;
            buffer.Position = 0;
            // Cancellation is honored through staging/backup. Once committing, finish the
            // complete PDF instead of reporting cancellation after leaving a partial file.
            await buffer.CopyToAsync(destination, CancellationToken.None);
            if (seekable) destination.SetLength(buffer.Length);
            await destination.FlushAsync(CancellationToken.None);
        }
        catch (Exception writeError)
        {
            // Nonseekable streams cannot undo bytes already accepted by their destination.
            if (!seekable) throw;
            try
            {
                destination.Position = 0;
                if (previous is not null)
                {
                    previous.Position = 0;
                    await previous.CopyToAsync(destination, CancellationToken.None);
                }
                destination.SetLength(length);
                await destination.FlushAsync(CancellationToken.None);
                destination.Position = position;
            }
            catch (Exception restoreError)
            {
                throw new AggregateException("PDF 写入失败，原目标内容也未能恢复；请勿使用该目标中的数据。", writeError, restoreError);
            }
            throw;
        }
    }

    internal static async Task WriteStagedAsync(MemoryStream buffer, Stream destination,
        CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess();
        cancellationToken.ThrowIfCancellationRequested();
        using var lease = AcquireDestination(destination);
        await CommitAsync(buffer, destination, cancellationToken);
    }
}
