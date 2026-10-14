using Avalonia;
using Avalonia.Controls;

namespace TheOne.DesignPreview;

/// <summary>Fixed-width catalogue cells that skip filtered cards when arranging.</summary>
public sealed class CatalogPanel : WrapPanel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        var width = double.IsNaN(ItemWidth) ? availableSize.Width : ItemWidth;
        double x = 0, rowHeight = 0, height = 0;
        foreach (var child in Children.Where(child => child.IsVisible))
        {
            child.Measure(new Size(width, double.PositiveInfinity));
            if (x > 0 && x + width > availableSize.Width + 0.5) { height += rowHeight; x = 0; rowHeight = 0; }
            x += width; rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);
        }
        return new Size(availableSize.Width, height + rowHeight);
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        var width = double.IsNaN(ItemWidth) ? finalSize.Width : ItemWidth;
        double x = 0, y = 0, rowHeight = 0;
        foreach (var child in Children.Where(child => child.IsVisible))
        {
            if (x > 0 && x + width > finalSize.Width + 0.5) { y += rowHeight; x = 0; rowHeight = 0; }
            child.Arrange(new Rect(x, y, width, child.DesiredSize.Height));
            x += width; rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);
        }
        return finalSize;
    }
}
