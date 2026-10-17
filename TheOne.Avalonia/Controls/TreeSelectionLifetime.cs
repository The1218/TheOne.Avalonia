using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Utilities;

namespace TheOne.Avalonia.Controls;

// Model ownership outlives recycled containers, but only selected paths need it.
internal static class TreeSelectionLifetime
{
    private static readonly ConditionalWeakTable<TreeViewItem, ContainerState> Containers = new();
    private static readonly ConditionalWeakTable<TreeView, SelectionState> Trees = new();

    internal static void Prepared(ItemsControl owner, Control container, int index)
    {
        if (container is not TreeViewItem node || index < 0 || index >= owner.ItemCount) return;
        var state = Containers.GetValue(node, static _ => new ContainerState());
        state.Data = owner.Items[index];
        state.Index = index;
        if (state.Owner is null) state.Owner = new WeakReference<ItemsControl>(owner);
        else state.Owner.SetTarget(owner);
        if (FindTree(owner) is { } tree)
            Trees.GetValue(tree, static current => new SelectionState(current)).Prepared(owner, node, state.Data);
    }

    internal static void IndexChanged(ItemsControl owner, Control container, int index)
    {
        if (container is TreeViewItem node && Containers.TryGetValue(node, out var state) &&
            state.Owner?.TryGetTarget(out var previous) == true &&
            ReferenceEquals(previous, owner)) state.Index = index;
    }

    internal static bool ShouldResetOnPreparing(ItemsControl owner, Control container)
    {
        return container is TreeViewItem node && Containers.TryGetValue(node, out var state) &&
               state.Data is { } data &&
               !ReferenceEquals(data, node) && state.Owner?.TryGetTarget(out var previous) == true &&
               ReferenceEquals(previous, owner) &&
               state.Index >= 0 && state.Index < owner.ItemCount && ReferenceEquals(owner.Items[state.Index], data) &&
               FindTree(owner) is { } tree && Trees.TryGetValue(tree, out var selection) &&
               selection.HasSelectedDescendant(data);
    }

    internal static void CollectionChanged(ItemsControl owner, NotifyCollectionChangedEventArgs args)
    {
        if (args.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Move) return;
        if (FindTree(owner) is { } tree && Trees.TryGetValue(tree, out var state)) state.OwnerChanged(owner);
    }

    private static TreeView? FindTree(ItemsControl owner)
    {
        return owner as TreeView ?? owner.GetLogicalAncestors().OfType<TreeView>().FirstOrDefault();
    }

    private static object? Data(ItemsControl owner)
    {
        return owner is TreeViewItem node && Containers.TryGetValue(node, out var state) ? state.Data : null;
    }

    private static bool Contains(IEnumerable source, object data)
    {
        foreach (var item in source)
            if (ReferenceEquals(item, data))
                return true;
        return false;
    }

    private sealed class ContainerState
    {
        public object? Data;
        public int Index;
        public WeakReference<ItemsControl>? Owner;
    }

    private sealed record Link(WeakReference<ItemsControl> Owner, object? ParentData, IEnumerable Source, object Data);

    private sealed class SelectionState : IWeakEventSubscriber<NotifyCollectionChangedEventArgs>
    {
        private readonly Dictionary<object, List<Link>> _paths = new(ReferenceEqualityComparer.Instance);
        private readonly HashSet<INotifyCollectionChanged> _sources = new(ReferenceEqualityComparer.Instance);
        private readonly TreeView _tree;

        public SelectionState(TreeView tree)
        {
            _tree = tree;
            tree.SelectionChanged += SelectionChanged;
        }

        public void OnEvent(object? sender, WeakEvent ev, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Move ||
                sender is not IEnumerable source) return;
            RemoveInvalid(link => ReferenceEquals(link.Source, sender), source);
        }

        public bool HasSelectedDescendant(object data)
        {
            foreach (var path in _paths)
                if (!ReferenceEquals(path.Key, data))
                    foreach (var link in path.Value)
                        if (ReferenceEquals(link.Data, data))
                            return true;
            return false;
        }

        public void Prepared(ItemsControl owner, TreeViewItem container, object? data)
        {
            OwnerChanged(container);
            if (data is not null && Contains(_tree.SelectedItems, data)) Capture(data, container, owner);
        }

        private void SelectionChanged(object? sender, SelectionChangedEventArgs args)
        {
            if (!ReferenceEquals(args.Source, _tree)) return;
            foreach (var item in args.RemovedItems)
                if (item is not null)
                    _paths.Remove(item);
            foreach (var item in args.AddedItems)
                if (item is not null)
                    Capture(item);
            UpdateSources();
        }

        private void Capture(object data, TreeViewItem? container = null, ItemsControl? owner = null)
        {
            container ??= data as TreeViewItem ?? _tree.TreeContainerFromItem(data) as TreeViewItem;
            owner ??= container?.Parent as ItemsControl;
            if (container is null || owner is null) return;
            var path = new List<Link>();
            var child = data;
            while (owner is not null)
            {
                var parent = Data(owner);
                path.Add(new Link(new WeakReference<ItemsControl>(owner), parent, owner.ItemsSource ?? owner.Items,
                    child));
                if (ReferenceEquals(owner, _tree)) break;
                if (owner is not TreeViewItem parentNode || parent is null) return;
                child = parent;
                owner = parentNode.Parent as ItemsControl;
            }

            if (owner is null) return;
            _paths[data] = path;
            UpdateSources();
        }

        public void OwnerChanged(ItemsControl owner)
        {
            var parent = Data(owner);

            bool Matches(Link link)
            {
                return ReferenceEquals(link.ParentData, parent) && (parent is not null ||
                                                                    (link.Owner.TryGetTarget(out var target) &&
                                                                     ReferenceEquals(target, owner)));
            }

            if (!_paths.Values.Any(path => path.Any(Matches))) return;
            RemoveInvalid(Matches, owner.Items);
            foreach (var path in _paths.Values)
                for (var index = 0; index < path.Count; index++)
                    if (Matches(path[index]))
                        path[index] = path[index] with
                        {
                            Owner = new WeakReference<ItemsControl>(owner),
                            Source = owner.ItemsSource ?? owner.Items
                        };
            UpdateSources();
        }

        private void RemoveInvalid(Func<Link, bool> matches, IEnumerable current)
        {
            var missing = new HashSet<object>(ReferenceEqualityComparer.Instance);
            foreach (var path in _paths.Values)
            foreach (var link in path)
                if (matches(link))
                    missing.Add(link.Data);
            if (missing.Count == 0) return;
            foreach (var item in current)
            {
                if (item is not null) missing.Remove(item);
                if (missing.Count == 0) return;
            }

            foreach (var path in _paths.ToArray())
                if (path.Value.Any(link => matches(link) && missing.Contains(link.Data)))
                    _tree.SelectedItems.Remove(path.Key);
        }

        private void UpdateSources()
        {
            var current = new HashSet<INotifyCollectionChanged>(ReferenceEqualityComparer.Instance);
            foreach (var path in _paths.Values)
            foreach (var link in path)
                if (link.Source is INotifyCollectionChanged source)
                    current.Add(source);
            foreach (var source in _sources.ToArray())
                if (!current.Contains(source))
                {
                    WeakEvents.CollectionChanged.Unsubscribe(source, this);
                    _sources.Remove(source);
                }

            foreach (var source in current)
                if (_sources.Add(source))
                    WeakEvents.CollectionChanged.Subscribe(source, this);
        }
    }
}