using global::Avalonia;
using global::Avalonia.Media;
using global::Avalonia.Media.Imaging;
using global::Avalonia.VisualTree;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed partial class ReportPageSurface
{
    private readonly Dictionary<string, Bitmap> _images = new(StringComparer.Ordinal);
    internal void ClearImages() { foreach (var image in _images.Values) image.Dispose(); _images.Clear(); }
    private void PruneImages()
    {
        var active = _document?.Elements.Where(e => e.Kind == ReportElementKind.Image).Select(ImageData).ToHashSet(StringComparer.Ordinal) ?? [];
        foreach (var key in _images.Keys.Where(k => !active.Contains(k)).ToArray()) { _images[key].Dispose(); _images.Remove(key); }
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) { ClearImages(); base.OnDetachedFromVisualTree(e); }
    private void DrawImage(DrawingContext context, ReportElement element, Rect area)
    {
        var data = ImageData(element);
        if (data.Length == 0)
        {
            if (!_preview)
            {
                context.DrawRectangle(Stripe, new Pen(PaperLine, 1, DashStyle.Dash), area);
                Text(context, element.ImageUrl.Length > 0 ? "图片加载中或加载失败" : "图片 · 输入服务器地址", area.Deflate(8), MutedInk, 12);
            }
            return;
        }
        if (!_images.TryGetValue(data, out var image))
        {
            try { image = new Bitmap(new MemoryStream(ReportImageData.Decode(data), false)); _images.Add(data, image); }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
            { if (!_preview) Text(context, "图片无法解码", area.Deflate(8), Brushes.DarkRed, 12); return; }
        }
        var source = new Rect(image.Size); var target = area;
        var scale = element.ImageFit == ReportImageFit.Cover ? Math.Max(area.Width / source.Width, area.Height / source.Height) : Math.Min(area.Width / source.Width, area.Height / source.Height);
        if (element.ImageFit != ReportImageFit.Fill)
        { var size = source.Size * scale; target = new Rect(area.Center - new Vector(size.Width / 2, size.Height / 2), size); }
        using (context.PushClip(area)) context.DrawImage(image, source, target);
    }
    private static string ImageData(ReportElement element) => element.ImageUrl.Length > 0
        ? element.ResolvedImageUrl == element.ImageUrl ? element.ResolvedImageData : "" : element.ImageData;
    private static void ElementText(DrawingContext context, string value, Rect area, IBrush brush, ReportElement element, bool bold = false, bool right = false, bool center = false, double? fontSize = null)
    {
        if (area.Width < 1 || area.Height < 1 || value.Length == 0) return;
        var text = ReportPaginator.MeasureElementText(value, area.Width, element, brush, bold, fontSize);
        var alignment = center ? ReportTextAlignment.Center : right && element.TextAlignment == ReportTextAlignment.Left ? ReportTextAlignment.Right : element.TextAlignment;
        var x = alignment == ReportTextAlignment.Center ? area.X + (area.Width - text.Width) / 2 : alignment == ReportTextAlignment.Right ? area.Right - text.Width : area.X;
        using (context.PushClip(area)) context.DrawText(text, new Point(x, area.Y + Math.Max(0, (area.Height - text.Height) / 2)));
    }
}
