using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace TheOne.Avalonia.Controls;

/// <summary>A label and value row. Header and Content accept text, controls or typed data templates.</summary>
[PseudoClasses(":first", ":stacked")]
public class DescriptionItem : HeaderedContentControl
{
    public static readonly StyledProperty<double> LabelWidthProperty =
        Description.LabelWidthProperty.AddOwner<DescriptionItem>();

    public static readonly StyledProperty<double> StackWidthProperty =
        Description.StackWidthProperty.AddOwner<DescriptionItem>();

    static DescriptionItem()
    {
        AffectsMeasure<DescriptionItem>(LabelWidthProperty, StackWidthProperty);
    }

    /// <summary>Inherited label column width in DIPs; applies while the row uses two columns.</summary>
    public double LabelWidth
    {
        get => GetValue(LabelWidthProperty);
        set => SetValue(LabelWidthProperty, value);
    }

    /// <summary>Inherited width below which the label is placed above the content.</summary>
    public double StackWidth
    {
        get => GetValue(StackWidthProperty);
        set => SetValue(StackWidthProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(DescriptionItem);

    internal void SetFirst(bool value)
    {
        PseudoClasses.Set(":first", value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        PseudoClasses.Set(":stacked", availableSize.Width < StackWidth);
        return base.MeasureOverride(availableSize);
    }
}