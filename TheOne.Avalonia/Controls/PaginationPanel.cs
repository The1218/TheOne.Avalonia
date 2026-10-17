using Avalonia;
using Avalonia.Controls;

namespace TheOne.Avalonia.Controls;

/// <summary>Places three standard groups in a wrapping row, or navigation above a compact summary.</summary>
public class PaginationPanel : Panel
{
    private const double Gap = 20;

    public static readonly StyledProperty<bool> IsCompactProperty =
        AvaloniaProperty.Register<PaginationPanel, bool>(nameof(IsCompact));

    static PaginationPanel()
    {
        AffectsMeasure<PaginationPanel>(IsCompactProperty);
    }

    public bool IsCompact
    {
        get => GetValue(IsCompactProperty);
        set => SetValue(IsCompactProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count != 3) return base.MeasureOverride(availableSize);
        if (!IsCompact) Children[1].Measure(new Size(double.PositiveInfinity, availableSize.Height));
        if (IsCompact)
        {
            Children[0].Measure(availableSize);
            Children[1].Measure(availableSize);
            Children[2].Measure(default);
            return new Size(Math.Max(Children[0].DesiredSize.Width, Children[1].DesiredSize.Width),
                Children[1].DesiredSize.Height + (Children[0].IsVisible ? Children[0].DesiredSize.Height + 12 : 0));
        }

        double x = 0, height = 0, rowHeight = 0, width = 0;
        foreach (var child in Children.Where(child => child.IsVisible))
        {
            if (child != Children[1]) child.Measure(availableSize);
            if (x > 0 && x + Gap + child.DesiredSize.Width > availableSize.Width)
            {
                height += rowHeight + 12;
                x = 0;
                rowHeight = 0;
            }

            if (x > 0) x += Gap;
            x += child.DesiredSize.Width;
            width = Math.Max(width, x);
            rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);
        }

        return new Size(width, height + rowHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count != 3) return base.ArrangeOverride(finalSize);
        if (IsCompact)
        {
            var height = Children[1].DesiredSize.Height;
            Children[1].Arrange(new Rect(0, 0, finalSize.Width, height));
            if (Children[0].IsVisible)
                Children[0].Arrange(new Rect(0, height + 12, finalSize.Width, Children[0].DesiredSize.Height));
            return finalSize;
        }

        if (Children.All(child => child.IsVisible) &&
            Children.Sum(child => child.DesiredSize.Width) + Gap * 2 <= finalSize.Width)
        {
            var summary = Children[0];
            var navigation = Children[1];
            var jump = Children[2];
            var left = summary.DesiredSize.Width + Gap;
            var right = finalSize.Width - jump.DesiredSize.Width - Gap;
            summary.Arrange(new Rect(0, (finalSize.Height - summary.DesiredSize.Height) / 2, summary.DesiredSize.Width,
                summary.DesiredSize.Height));
            navigation.Arrange(new Rect(left + (right - left - navigation.DesiredSize.Width) / 2,
                (finalSize.Height - navigation.DesiredSize.Height) / 2, navigation.DesiredSize.Width,
                navigation.DesiredSize.Height));
            jump.Arrange(new Rect(finalSize.Width - jump.DesiredSize.Width,
                (finalSize.Height - jump.DesiredSize.Height) / 2, jump.DesiredSize.Width, jump.DesiredSize.Height));
            return finalSize;
        }

        double x = 0, y = 0, rowHeight = 0;
        foreach (var child in Children.Where(child => child.IsVisible))
        {
            if (x > 0 && x + Gap + child.DesiredSize.Width > finalSize.Width)
            {
                y += rowHeight + 12;
                x = 0;
                rowHeight = 0;
            }

            if (x > 0) x += Gap;
            child.Arrange(new Rect(x, y, child.DesiredSize.Width, child.DesiredSize.Height));
            x += child.DesiredSize.Width;
            rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);
        }

        return finalSize;
    }
}