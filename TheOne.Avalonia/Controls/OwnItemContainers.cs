using System.Collections.Specialized;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.LogicalTree;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

// Generated containers belong to the native recycle pool. Caller-owned controls
// may be cached only while they remain in Items; retired controls must lose both parents.
internal sealed class OwnItemContainers(VirtualizingPanel panel, Action<Control> remove)
{
    private ItemsControl? _owner;
    private HashSet<Control>? _owned;
    public bool NeedsSelectionSync { get; private set; }

    public void SetOwner(ItemsControl? owner)
    {
        if (ReferenceEquals(_owner, owner)) return;
        if (_owner is not null) _owner.ContainerPrepared -= Prepared;
        _owned?.Clear();
        NeedsSelectionSync = false;
        _owner = owner;
        if (_owner is not null) _owner.ContainerPrepared += Prepared;
    }

    private void Prepared(object? sender, ContainerPreparedEventArgs args)
    {
        if (_owner is not null && args.Index >= 0 && args.Index < _owner.ItemCount &&
            ReferenceEquals(_owner.Items[args.Index], args.Container)) Remember(args.Container);
    }

    public void Remember(Control control) => (_owned ??= new(ReferenceEqualityComparer.Instance)).Add(control);

    public void Restored(Control control)
    {
        Remember(control);
        NeedsSelectionSync = true;
    }

    public void SynchronizeSelection(IEnumerable<Control> containers)
    {
        NeedsSelectionSync = false;
        if (_owner is null || _owned is null) return;
        var tree = FindTree();
        foreach (var control in containers)
        {
            if (!_owned.Contains(control)) continue;
            var index = _owner.IndexFromContainer(control);
            if (index < 0 || index >= _owner.ItemCount || !ReferenceEquals(_owner.Items[index], control)) continue;
            if (control is TreeViewItem node && tree is not null)
                node.SetCurrentValue(TreeViewItem.IsSelectedProperty, tree.SelectedItems.Contains(node));
            else if (_owner is SelectingItemsControl)
                _owner.ItemContainerGenerator.ItemContainerIndexChanged(control, index, index);
        }
    }

    public void CollectionChanged(IReadOnlyList<object?> items, NotifyCollectionChangedEventArgs args)
    {
        if (_owned is not { Count: > 0 }) return;
        NeedsSelectionSync = true;
        if (args.Action == NotifyCollectionChangedAction.Reset)
        {
            HashSet<Control>? current = null;
            foreach (var item in items)
                if (item is Control control) (current ??= new(ReferenceEqualityComparer.Instance)).Add(control);
            foreach (var control in _owned.ToArray())
                if (current?.Contains(control) != true) Retire(control);
        }
        else if (args.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Replace && args.OldItems is { } oldItems)
        {
            foreach (var item in oldItems)
                if (item is Control control && args.NewItems?.Cast<object?>().Any(next => ReferenceEquals(next, control)) != true)
                    Retire(control);
        }
    }

    private void Retire(Control control)
    {
        if (_owned?.Remove(control) != true) return;
        if (control is TreeViewItem node && FindTree() is { } tree)
            RemoveRetiredSelection(tree, node);
        if (ReferenceEquals(control.GetVisualParent(), panel) || ReferenceEquals(control.Parent, _owner)) remove(control);
    }

    private TreeView? FindTree() => _owner as TreeView ?? _owner?.GetVisualAncestors().OfType<TreeView>().FirstOrDefault()
        ?? _owner?.GetLogicalAncestors().OfType<TreeView>().FirstOrDefault();

    private static void RemoveRetiredSelection(TreeView tree, TreeViewItem node)
    {
        if (tree.SelectedItems.Count == 0) return;
        tree.SelectedItems.Remove(node);
        for (var index = 0; index < node.ItemCount && tree.SelectedItems.Count > 0; index++)
        {
            var item = node.Items[index];
            tree.SelectedItems.Remove(item);
            // Walk only the retired branch, including materialized model descendants.
            if (item is TreeViewItem ownChild) RemoveRetiredSelection(tree, ownChild);
            else if (node.ContainerFromIndex(index) is TreeViewItem child) RemoveRetiredSelection(tree, child);
        }
    }
}
