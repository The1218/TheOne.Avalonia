using global::Avalonia;
using global::Avalonia.Controls;

namespace TheOne.Avalonia.Controls;

/// <summary>A naturally sized list of labelled values, with automatic stacking in narrow layouts.</summary>
public class Description : ItemsControl
{
    public static readonly StyledProperty<double> LabelWidthProperty =
        AvaloniaProperty.Register<Description, double>(nameof(LabelWidth), 180, inherits: true,
            validate: value => double.IsFinite(value) && value >= 0);

    public static readonly StyledProperty<double> StackWidthProperty =
        AvaloniaProperty.Register<Description, double>(nameof(StackWidth), 520, inherits: true,
            validate: value => double.IsFinite(value) && value >= 0);

    /// <summary>Label column width in DIPs. Individual rows may override the inherited value.</summary>
    public double LabelWidth { get => GetValue(LabelWidthProperty); set => SetValue(LabelWidthProperty, value); }

    /// <summary>Rows narrower than this width place their label above the value. Zero keeps two columns.</summary>
    public double StackWidth { get => GetValue(StackWidthProperty); set => SetValue(StackWidthProperty, value); }

    protected override Type StyleKeyOverride => typeof(Description);

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) => new DescriptionItem();

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) =>
        NeedsContainer<DescriptionItem>(item, out recycleKey);

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is DescriptionItem row) row.SetFirst(index == 0);
    }

    protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
    {
        base.ContainerIndexChangedOverride(container, oldIndex, newIndex);
        if (container is DescriptionItem row) row.SetFirst(newIndex == 0);
    }
}
