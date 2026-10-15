using global::Avalonia;
using global::Avalonia.Media;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed partial class ReportPageSurface
{
    private void DrawCheckBox(DrawingContext context, ReportElement element, string value, Rect area, IBrush brush)
    {
        var size = Math.Max(1, Math.Min(Math.Min(area.Height - 4, area.Width - 4), Math.Max(14, element.FontSize)));
        var box = new Rect(area.X + 2, area.Y + (area.Height - size) / 2, size, size);
        using (context.PushClip(area))
        {
            context.DrawRectangle(Brushes.White, new Pen(brush, 1), box);
            if (element.FieldKey.Length > 0 ? ReportBarcode.CheckedValue(value, element.IsChecked) : element.IsChecked)
            {
                var pen = new Pen(brush, Math.Max(1.5, size / 10));
                var middle = new Point(box.X + size * .43, box.Y + size * .73);
                context.DrawLine(pen, new Point(box.X + size * .19, box.Y + size * .48), middle);
                context.DrawLine(pen, middle, new Point(box.X + size * .83, box.Y + size * .24));
            }
            if (area.Width > size + 10) ElementText(context, element.Text, new Rect(box.Right + 6, area.Y + 2, area.Width - size - 10, area.Height - 4), brush, element);
        }
    }

    private void DrawBarcode(DrawingContext context, ReportElement element, string value, Rect area, IBrush brush)
    {
        string runs;
        try { runs = ReportBarcode.Encode(value); }
        catch (ArgumentException exception)
        {
            if (!_preview) Text(context, exception.Message, area.Deflate(2), Brushes.DarkRed, 10);
            return;
        }
        var textHeight = element.BarcodeShowText ? element.FontSize + 6 : 0;
        var unit = Math.Max(0, area.Width - 4) / (runs.Sum(run => run - '0') + 20);
        var x = area.X + 2 + unit * 10;
        using (context.PushClip(area))
        {
            context.DrawRectangle(Brushes.White, null, area);
            for (var i = 0; i < runs.Length; i++)
            {
                var width = (runs[i] - '0') * unit;
                if (i % 2 == 0) context.DrawRectangle(brush, null, new Rect(x, area.Y + 2, width, Math.Max(1, area.Height - textHeight - 4)));
                x += width;
            }
            if (element.BarcodeShowText) ElementText(context, value, new Rect(area.X + 2, area.Bottom - textHeight, area.Width - 4, textHeight), brush, element);
        }
    }
}
