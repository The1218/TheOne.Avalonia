using Avalonia;
using Avalonia.Controls;

namespace TheOne.Avalonia.Controls;

public enum IconPlacement
{
    Left,
    Right,
    Top,
    Bottom
}

/// <summary>Template layout: the first two children are icon and label, the third is the loading replacement.</summary>
public sealed class IconButtonPanel : Panel
{
    public static readonly StyledProperty<IconPlacement> IconPlacementProperty =
        AvaloniaProperty.Register<IconButtonPanel, IconPlacement>(nameof(IconPlacement));

    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<IconButtonPanel, double>(nameof(Spacing), 8);

    static IconButtonPanel()
    {
        AffectsMeasure<IconButtonPanel>(IconPlacementProperty, SpacingProperty);
    }

    public IconPlacement IconPlacement
    {
        get => GetValue(IconPlacementProperty);
        set => SetValue(IconPlacementProperty, value);
    }

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    private bool Vertical => IconPlacement is IconPlacement.Top or IconPlacement.Bottom;

    private Control[] VisibleChildren()
    {
        return Children.Where(child => child.IsVisible).OrderBy(child => Children.IndexOf(child) == 1 ? 1 : 0)
            .ToArray();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var children = VisibleChildren();
        double used = 0;
        foreach (var child in children)
        {
            child.Measure(Vertical
                ? new Size(availableSize.Width, Math.Max(0, availableSize.Height - used))
                : new Size(Math.Max(0, availableSize.Width - used), availableSize.Height));
            used += (Vertical ? child.DesiredSize.Height : child.DesiredSize.Width) + Spacing;
        }

        var gap = children.Length > 1 ? Spacing * (children.Length - 1) : 0;
        return Vertical
            ? new Size(children.Select(child => child.DesiredSize.Width).DefaultIfEmpty().Max(),
                children.Sum(child => child.DesiredSize.Height) + gap)
            : new Size(children.Sum(child => child.DesiredSize.Width) + gap,
                children.Select(child => child.DesiredSize.Height).DefaultIfEmpty().Max());
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = VisibleChildren();
        if (IconPlacement is IconPlacement.Right or IconPlacement.Bottom) Array.Reverse(children);
        double offset = 0;
        foreach (var child in children)
        {
            var size = child.DesiredSize;
            child.Arrange(Vertical
                ? new Rect((finalSize.Width - size.Width) / 2, offset, size.Width, size.Height)
                : new Rect(offset, (finalSize.Height - size.Height) / 2, size.Width, size.Height));
            offset += (Vertical ? size.Height : size.Width) + Spacing;
        }

        return finalSize;
    }
}