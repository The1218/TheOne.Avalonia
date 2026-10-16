using System.Collections.Specialized;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

// Keep native virtualization, including own Control containers. Avalonia 12.1.4
// remembers an own container after removal but skips reattaching it on reinsertion.
internal sealed class VirtualizingItemsPanel : VirtualizingStackPanel
{
    private readonly OwnItemContainers _own;
    private RecycleTree? _recycleTree;

    private sealed class RecycleTree : TreeView
    {
        public RecycleTree() => DataContext = null;
        protected override Type StyleKeyOverride => typeof(Control);

        public void ResetOwner(TreeViewItem node)
        {
            try { LogicalChildren.Add(node); }
            finally { LogicalChildren.Remove(node); }
        }
    }

    public VirtualizingItemsPanel() => _own = new(this, RemoveInternalChild);

    protected override Type StyleKeyOverride => typeof(VirtualizingStackPanel);

    internal void ReleaseForMenuPanelChange(ItemsControl owner)
    {
        if (!ReferenceEquals(ItemsControl, owner)) return;
        foreach (var child in Children.ToArray()) RemoveInternalChild(child);
    }

    protected override void OnItemsControlChanged(ItemsControl? oldValue)
    {
        if (oldValue is TreeView or TreeViewItem)
        {
            oldValue.ContainerPrepared -= TreeContainerPrepared;
            oldValue.ContainerIndexChanged -= TreeContainerIndexChanged;
            oldValue.PreparingContainer -= TreeContainerPreparing;
        }
        base.OnItemsControlChanged(oldValue);
        _own.SetOwner(ItemsControl);
        if (ItemsControl is TreeView or TreeViewItem)
        {
            ItemsControl.ContainerPrepared += TreeContainerPrepared;
            ItemsControl.ContainerIndexChanged += TreeContainerIndexChanged;
            ItemsControl.PreparingContainer += TreeContainerPreparing;
        }
    }

    private static void TreeContainerPrepared(object? sender, ContainerPreparedEventArgs args)
    {
        if (sender is ItemsControl owner) TreeSelectionLifetime.Prepared(owner, args.Container, args.Index);
    }

    private static void TreeContainerIndexChanged(object? sender, ContainerIndexChangedEventArgs args)
    {
        if (sender is ItemsControl owner) TreeSelectionLifetime.IndexChanged(owner, args.Container, args.NewIndex);
    }

    private void Restored(Control container, int index)
    {
        _own.Restored(container);
        if (ItemsControl is TreeView or TreeViewItem) TreeSelectionLifetime.Prepared(ItemsControl, container, index);
    }

    private void TreeContainerPreparing(object? sender, ContainerPreparedEventArgs args)
    {
        if (sender is not ItemsControl owner || args.Container is not TreeViewItem node ||
            node.Parent is not null || !TreeSelectionLifetime.ShouldResetOnPreparing(owner, node)) return;
        // Native TreeViewItem caches its tree on logical attachment, but never
        // clears it on detachment. Reset that cache before native children rebind,
        // so recycling cannot remove live descendants from the real selection.
        // This temporary logical owner has no visual children or platform window.
        var recycleTree = _recycleTree ??= new RecycleTree();
        LogicalChildren.Add(recycleTree);
        try { recycleTree.ResetOwner(node); }
        finally { LogicalChildren.Remove(recycleTree); }
    }

    protected override void OnItemsChanged(IReadOnlyList<object?> items, NotifyCollectionChangedEventArgs args)
    {
        base.OnItemsChanged(items, args);
        if (ItemsControl is TreeView or TreeViewItem) TreeSelectionLifetime.CollectionChanged(ItemsControl, args);
        _own.CollectionChanged(items, args);
    }

    protected override Control? ScrollIntoView(int index)
    {
        // A cached own item may not be realized yet. Restore its parent before the
        // native panel measures it and raises BringIntoView through the visual tree.
        if (IsEffectivelyVisible && this.IsAttachedToVisualTree() && index >= 0 && index < Items.Count &&
            Items[index] is Control item && item.GetVisualParent() is null && !Children.Contains(item) &&
            ReferenceEquals(base.ContainerFromIndex(index), item))
        {
            AddInternalChild(item);
            Restored(item, index);
        }
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
        if (detached is not null)
        {
            foreach (var container in detached)
            {
                AddInternalChild(container);
                Restored(container, IndexFromContainer(container));
            }
            // Reattachment restores inherited resources and may invalidate measurement.
            result = base.MeasureOverride(availableSize);
        }
        if (_own.NeedsSelectionSync && GetRealizedContainers() is { } current) _own.SynchronizeSelection(current);
        return result;
    }
}
