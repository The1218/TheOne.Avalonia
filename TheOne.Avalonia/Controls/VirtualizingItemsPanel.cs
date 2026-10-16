using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

// Keep native virtualization, including own Control containers. Avalonia 12.1.4
// remembers an own container after removal but skips reattaching it on reinsertion.
internal sealed class VirtualizingItemsPanel : VirtualizingStackPanel
{
    protected override Type StyleKeyOverride => typeof(VirtualizingStackPanel);

    protected override Control? ScrollIntoView(int index)
    {
        // A cached own item may not be realized yet. Restore its parent before the
        // native panel measures it and raises BringIntoView through the visual tree.
        if (IsEffectivelyVisible && this.IsAttachedToVisualTree() && index >= 0 && index < Items.Count &&
            Items[index] is Control item && item.GetVisualParent() is null && !Children.Contains(item) &&
            ReferenceEquals(base.ContainerFromIndex(index), item))
            AddInternalChild(item);
        return base.ScrollIntoView(index);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var result = base.MeasureOverride(availableSize);
        if (GetRealizedContainers() is not { } containers) return result;
        List<Control>? detached = null;
        foreach (var container in containers)
        {
            if (container.GetVisualParent() is not null) continue;
            var index = IndexFromContainer(container);
            if (index < 0 || index >= Items.Count || !ReferenceEquals(Items[index], container) || Children.Contains(container)) continue;
            (detached ??= []).Add(container);
        }
        if (detached is null) return result;
        foreach (var container in detached) AddInternalChild(container);
        // Reattachment restores inherited resources and may invalidate measurement.
        return base.MeasureOverride(availableSize);
    }
}
