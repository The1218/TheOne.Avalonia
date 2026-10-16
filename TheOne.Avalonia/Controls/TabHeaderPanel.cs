using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

// Keep native tab virtualization, including own TabItem containers. Avalonia 12.1.4
// remembers an own container after removal but skips reattaching it on reinsertion.
internal sealed class TabHeaderPanel : VirtualizingStackPanel
{
    protected override Type StyleKeyOverride => typeof(VirtualizingStackPanel);

    protected override Size MeasureOverride(Size availableSize)
    {
        var result = base.MeasureOverride(availableSize);
        if (GetRealizedContainers() is not { } containers) return result;
        var restored = false;
        foreach (var header in containers.OfType<TabItem>().ToArray())
        {
            var index = IndexFromContainer(header);
            if (index < 0 || index >= Items.Count || !ReferenceEquals(Items[index], header) ||
                header.GetVisualParent() is not null || Children.Contains(header)) continue;
            AddInternalChild(header);
            restored = true;
        }
        // Reattachment restores inherited resources and may invalidate measurement.
        return restored ? base.MeasureOverride(availableSize) : result;
    }
}
