using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>Equal horizontal slots or natural vertical rows, with rails between actual markers.</summary>
public class FlowStepPanel : Panel
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        FlowStep.OrientationProperty.AddOwner<FlowStepPanel>();

    public static readonly StyledProperty<double> NodeSpacingProperty =
        FlowStep.NodeSpacingProperty.AddOwner<FlowStepPanel>();

    public static readonly StyledProperty<double> MinNodeWidthProperty =
        FlowStep.MinNodeWidthProperty.AddOwner<FlowStepPanel>();

    public static readonly StyledProperty<double> ViewportWidthProperty =
        AvaloniaProperty.Register<FlowStepPanel, double>(nameof(ViewportWidth),
            validate: value => double.IsFinite(value) && value >= 0);

    private double _slotWidth;

    static FlowStepPanel()
    {
        AffectsMeasure<FlowStepPanel>(OrientationProperty, NodeSpacingProperty, MinNodeWidthProperty,
            ViewportWidthProperty);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public double NodeSpacing
    {
        get => GetValue(NodeSpacingProperty);
        set => SetValue(NodeSpacingProperty, value);
    }

    public double MinNodeWidth
    {
        get => GetValue(MinNodeWidthProperty);
        set => SetValue(MinNodeWidthProperty, value);
    }

    public double ViewportWidth
    {
        get => GetValue(ViewportWidthProperty);
        set => SetValue(ViewportWidthProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var children = Children.Where(child => child.IsVisible).ToArray();
        if (children.Length == 0) return default;
        var gaps = NodeSpacing * (children.Length - 1);
        if (Orientation == Orientation.Horizontal)
        {
            var width = double.IsFinite(availableSize.Width) ? availableSize.Width : ViewportWidth;
            _slotWidth = Math.Max(MinNodeWidth, (width - gaps) / children.Length);
            foreach (var child in children) child.Measure(new Size(_slotWidth, availableSize.Height));
            return new Size(_slotWidth * children.Length + gaps, children.Max(child => child.DesiredSize.Height));
        }

        foreach (var child in children) child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
        return new Size(children.Max(child => child.DesiredSize.Width),
            children.Sum(child => child.DesiredSize.Height) + gaps);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = Children.Where(child => child.IsVisible).ToArray();
        if (children.Length == 0) return finalSize;
        var width = Orientation == Orientation.Horizontal
            ? Math.Max(_slotWidth, (finalSize.Width - NodeSpacing * (children.Length - 1)) / children.Length)
            : finalSize.Width;
        double offset = 0;
        foreach (var child in children)
        {
            if (Orientation == Orientation.Horizontal) child.Arrange(new Rect(offset, 0, width, finalSize.Height));
            else child.Arrange(new Rect(0, offset, width, child.DesiredSize.Height));
            offset += (Orientation == Orientation.Horizontal ? width : child.DesiredSize.Height) + NodeSpacing;
        }

        this.GetVisualAncestors().OfType<FlowStep>().FirstOrDefault()?.InvalidateConnectors();
        return finalSize;
    }
}

/// <summary>Positions a marker above horizontal content or to the left of vertical content.</summary>
public class FlowNodePanel : Panel
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        FlowStep.OrientationProperty.AddOwner<FlowNodePanel>();

    public static readonly StyledProperty<double> ContentSpacingProperty =
        FlowNode.ContentSpacingProperty.AddOwner<FlowNodePanel>();

    static FlowNodePanel()
    {
        AffectsMeasure<FlowNodePanel>(OrientationProperty, ContentSpacingProperty);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public double ContentSpacing
    {
        get => GetValue(ContentSpacingProperty);
        set => SetValue(ContentSpacingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count != 2) return base.MeasureOverride(availableSize);
        var marker = Children[0];
        var content = Children[1];
        marker.Measure(availableSize);
        var gap = content.IsVisible ? ContentSpacing : 0;
        content.Measure(new Size(
            Math.Max(0,
                availableSize.Width - (Orientation == Orientation.Vertical ? marker.DesiredSize.Width + gap : 0)),
            double.PositiveInfinity));
        return Orientation == Orientation.Horizontal
            ? new Size(Math.Max(marker.DesiredSize.Width, content.DesiredSize.Width),
                marker.DesiredSize.Height + gap + content.DesiredSize.Height)
            : new Size(marker.DesiredSize.Width + gap + content.DesiredSize.Width,
                Math.Max(marker.DesiredSize.Height, content.DesiredSize.Height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count != 2) return base.ArrangeOverride(finalSize);
        var marker = Children[0];
        var content = Children[1];
        var size = marker.DesiredSize;
        var gap = content.IsVisible ? ContentSpacing : 0;
        if (Orientation == Orientation.Horizontal)
        {
            marker.Arrange(new Rect((finalSize.Width - size.Width) / 2, 0, size.Width, size.Height));
            content.Arrange(new Rect(0, size.Height + gap, finalSize.Width,
                Math.Max(0, finalSize.Height - size.Height - gap)));
        }
        else
        {
            marker.Arrange(new Rect(0, 0, size.Width, size.Height));
            content.Arrange(new Rect(size.Width + gap, 0, Math.Max(0, finalSize.Width - size.Width - gap),
                finalSize.Height));
        }

        return finalSize;
    }
}