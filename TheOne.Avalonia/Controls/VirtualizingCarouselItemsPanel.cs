using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

// Preserve native carousel gestures, transitions and generated-container pooling,
// while releasing retired own Controls and remounting a reused own instance.
internal sealed class VirtualizingCarouselItemsPanel : VirtualizingCarouselPanel
{
    private readonly OwnItemContainers _ownItems;

    public VirtualizingCarouselItemsPanel()
    {
        _ownItems = new OwnItemContainers(this, RemoveInternalChild);
    }

    protected override Type StyleKeyOverride => typeof(VirtualizingCarouselPanel);

    protected override void OnItemsControlChanged(ItemsControl? oldValue)
    {
        base.OnItemsControlChanged(oldValue);
        _ownItems.SetOwner(ItemsControl);
    }

    protected override void OnItemsChanged(IReadOnlyList<object?> items, NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(items, e);
        _ownItems.CollectionChanged(items, e);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var result = base.MeasureOverride(availableSize);
        if (GetRealizedContainers() is not { } containers) return result;
        List<Control>? detached = null;
        foreach (var container in containers)
        {
            var index = IndexFromContainer(container);
            if (index < 0 || index >= Items.Count || !ReferenceEquals(Items[index], container)) continue;
            _ownItems.Remember(container);
            if (container.GetVisualParent() is null && !Children.Contains(container) &&
                detached?.Contains(container) != true)
                (detached ??= []).Add(container);
        }

        if (detached is not null)
        {
            foreach (var container in detached)
            {
                AddInternalChild(container);
                _ownItems.Restored(container);
            }

            // Reattachment restores inherited resources and can change desired size.
            result = base.MeasureOverride(availableSize);
        }

        if (_ownItems.NeedsSelectionSync && GetRealizedContainers() is { } current)
            _ownItems.SynchronizeSelection(current);
        return result;
    }
}