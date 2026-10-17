// TheOne extension of the native collection view; see LICENSE.md.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Avalonia.Collections;

/// <summary>Presentation state for an original model in a local tree.</summary>
public readonly record struct DataGridTreeNodeState(int Depth, object? Parent, bool HasChildren, bool IsExpanded)
{
    public bool IsForcedExpanded { get; init; }
    public bool IsFilterMatch { get; init; }
}

/// <summary>A cancellable change to the visible tree. Item is null for a source refresh.</summary>
public sealed class DataGridTreeChangingEventArgs(object? item, bool? expanded) : CancelEventArgs
{
    public object? Item { get; } = item;
    public bool? Expanded { get; } = expanded;
}

/// <summary>Non-generic tree presentation contract; row objects remain the original models.</summary>
public interface IDataGridTreeCollectionView
{
    event EventHandler<DataGridTreeChangingEventArgs>? TreeChanging;
    event EventHandler? TreeChanged;
    bool TryGetNodeState(object item, out DataGridTreeNodeState state);
    bool TrySetExpanded(object item, bool expanded);
}

/// <summary>Optional query/summary contract, separate from the existing tree presentation API.</summary>
public interface IDataGridTreeQueryView : IDataGridTreeCollectionView
{
    IEnumerable LoadedItems { get; }
    event EventHandler? LoadedItemsChanged;
    bool IsFilterMatch(object item);
    bool IsLeaf(object item);
}

/// <summary>
///     Local tree projection with sibling sorting, ancestor-preserving filtering,
///     native editing and currency. Grouping, paging and view-based structural edits are unsupported.
/// </summary>
public sealed partial class DataGridTreeCollectionView<T> : DataGridCollectionView, IEnumerable<T>, IList,
    IDataGridTreeQueryView, IDisposable where T : class
{
    private readonly Func<T, IEnumerable<T>?> _children;
    private readonly HashSet<INotifyCollectionChanged> _collections = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<object> _expanded = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<INotifyPropertyChanged> _models = new(ReferenceEqualityComparer.Instance);
    private readonly IEnumerable<T> _roots;
    private readonly ObservableCollection<T> _visible;
    private bool _changing, _pending, _queued, _disposed;
    private Dictionary<object, Node> _nodes = new(ReferenceEqualityComparer.Instance);
    private List<Node> _order = [];

    public DataGridTreeCollectionView(IEnumerable<T> roots, Func<T, IEnumerable<T>?> childrenSelector)
        : this(roots, childrenSelector, new ObservableCollection<T>())
    {
    }

    private DataGridTreeCollectionView(IEnumerable<T> roots, Func<T, IEnumerable<T>?> childrenSelector,
        ObservableCollection<T> visible)
        : base(new ReadOnlyObservableCollection<T>(visible))
    {
        _roots = roots ?? throw new ArgumentNullException(nameof(roots));
        _children = childrenSelector ?? throw new ArgumentNullException(nameof(childrenSelector));
        _visible = visible;
        DataGridRowMetadata.RegisterCollection<DataGridTreeCollectionView<T>, T>();
        _ = SortDescriptions;
        GroupDescriptions.Validate = _ => throw new NotSupportedException("Tree grouping is not supported.");
        PropertyChanged += EditStateChanged;
        RefreshTree();
    }

    public override bool CanAddNew => false;
    public override bool CanRemove => false;
    public override bool CanSort => true;
    public override bool CanFilter => true;
    protected override bool IsSourcePreprocessed => true;
    public override bool CanGroup => false;
    public override bool CanChangePage => false;

    public override int PageSize
    {
        get => 0;
        set
        {
            if (value != 0) throw new NotSupportedException("Tree paging is not supported.");
        }
    }

    public event EventHandler<DataGridTreeChangingEventArgs>? TreeChanging;
    public event EventHandler? TreeChanged;
    public event EventHandler? LoadedItemsChanged;
    public IEnumerable LoadedItems => _order.Select(node => node.Item);

    public bool IsFilterMatch(object item)
    {
        return _matches.Contains(item);
    }

    public bool IsLeaf(object item)
    {
        return _nodes.TryGetValue(item, out var node) && node.Children.Length == 0;
    }

    public bool TryGetNodeState(object item, out DataGridTreeNodeState state)
    {
        if (_nodes.TryGetValue(item, out var node))
        {
            state = new DataGridTreeNodeState(node.Depth, node.Parent,
                    node.Children.Any(child => _included.Contains(child)),
                    _expanded.Contains(item) || _forcedExpanded.Contains(item))
                { IsForcedExpanded = _forcedExpanded.Contains(item), IsFilterMatch = _matches.Contains(item) };
            return true;
        }

        state = default;
        return false;
    }

    public bool TrySetExpanded(object item, bool expanded)
    {
        VerifyAccess();
        if (_changing || _queryDeferLevel > 0 || (!expanded && _forcedExpanded.Contains(item)) ||
            !_nodes.TryGetValue(item, out var node) || (expanded && node.Children.Length == 0)) return false;
        if (expanded && _forcedExpanded.Contains(item)) return true;
        if (_expanded.Contains(item) == expanded) return true;
        _changing = true;
        try
        {
            var args = new DataGridTreeChangingEventArgs(item, expanded);
            TreeChanging?.Invoke(this, args);
            if (args.Cancel || IsEditingItem || IsAddingNew) return false;
            if (expanded) _expanded.Add(item);
            else _expanded.Remove(item);
            try
            {
                PublishProjection(CreateProjection(_nodes, _order));
            }
            catch
            {
                if (expanded) _expanded.Remove(item);
                else _expanded.Add(item);
                throw;
            }

            TreeChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }
        finally
        {
            _changing = false;
            QueuePending();
        }
    }

    public void Dispose()
    {
        Dispatcher.UIThread.VerifyAccess();
        if (_disposed) return;
        if (_changing || _queryDeferLevel > 0 || IsEditingItem || IsAddingNew)
            throw new InvalidOperationException("Finish editing and query deferrals before disposing the tree view.");
        _disposed = true;
        PropertyChanged -= EditStateChanged;
        foreach (var source in _collections) source.CollectionChanged -= SourceChanged;
        foreach (var model in _models) model.PropertyChanged -= ModelChanged;
        _collections.Clear();
        _models.Clear();
        foreach (var observer in _queryObservers) observer.PropertyChanged -= QueryModelChanged;
        _queryObservers.Clear();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        var iterator = base.GetEnumerator();
        try
        {
            while (iterator.MoveNext()) yield return (T)iterator.Current;
        }
        finally
        {
            (iterator as IDisposable)?.Dispose();
        }
    }

    // DataGrid gates model editing through IList.IsReadOnly. The projection stays
    // structurally read-only; model edits still use the native editable-view contract.
    bool IList.IsReadOnly => false;

    /// <summary>
    ///     Refreshes plain collections or reconnects replaced child collections.
    ///     Returns false when an edit or a cancellable presentation change prevents refresh.
    /// </summary>
    public bool RefreshTree()
    {
        VerifyAccess();
        if (_changing || _queryDeferLevel > 0)
        {
            _pending = true;
            return false;
        }

        _changing = true;
        var published = false;
        try
        {
            var args = new DataGridTreeChangingEventArgs(null, null);
            TreeChanging?.Invoke(this, args);
            if (args.Cancel || IsEditingItem || IsAddingNew)
            {
                _pending = true;
                return false;
            }

            var nodes = new Dictionary<object, Node>(ReferenceEqualityComparer.Instance);
            var order = new List<Node>();
            var collections = new HashSet<INotifyCollectionChanged>(ReferenceEqualityComparer.Instance);
            if (_roots is INotifyCollectionChanged rootChanges) collections.Add(rootChanges);
            var stack = new Stack<(T Item, T? Parent, int Depth)>();
            foreach (var root in _roots.Reverse()) stack.Push((root, null, 0));
            while (stack.TryPop(out var entry))
            {
                if (entry.Item is null || nodes.ContainsKey(entry.Item))
                    throw new InvalidOperationException(
                        "A tree cannot contain null nodes, cycles or shared model instances.");
                var source = _children(entry.Item);
                var children = source?.ToArray() ?? [];
                var node = new Node(entry.Item, entry.Parent, entry.Depth, source, children);
                nodes.Add(entry.Item, node);
                order.Add(node);
                if (source is INotifyCollectionChanged changes) collections.Add(changes);
                for (var index = children.Length - 1; index >= 0; index--)
                    stack.Push((children[index], entry.Item, entry.Depth + 1));
            }

            var projection = CreateProjection(nodes, order);
            var loadedChanged = order.Count != _order.Count || order.Where((node, index) => index >= _order.Count ||
                !ReferenceEquals(node.Item, _order[index].Item) ||
                !ReferenceEquals(node.Parent, _order[index].Parent) ||
                !ReferenceEquals(node.Source, _order[index].Source)).Any();
            _nodes = nodes;
            _order = order;
            _expanded.RemoveWhere(item => !nodes.ContainsKey(item));
            Reconnect(collections);
            _pending = _queryPending = false;
            PublishProjection(projection);
            var filterChanged = !ReferenceEquals(_appliedFilter, _treeFilter);
            _appliedSorts = SortDescriptions.ToArray();
            _appliedFilter = _treeFilter;
            SetQueryError(null);
            published = true;
            if (loadedChanged) LoadedItemsChanged?.Invoke(this, EventArgs.Empty);
            TreeChanged?.Invoke(this, EventArgs.Empty);
            if (filterChanged) OnPropertyChanged(nameof(Filter));
            return true;
        }
        finally
        {
            _changing = false;
            if (published) QueuePending();
        }
    }

    private void PublishProjection(Projection projection)
    {
        _matches = projection.Matches;
        _included = projection.Included;
        _forcedExpanded = projection.ForcedExpanded;
        ReconnectQueryObservers(projection.Observers);
        var next = projection.Visible;
        int prefix = 0, suffix = 0;
        while (prefix < _visible.Count && prefix < next.Count &&
               ReferenceEquals(_visible[prefix], next[prefix])) prefix++;
        while (suffix < _visible.Count - prefix && suffix < next.Count - prefix &&
               ReferenceEquals(_visible[_visible.Count - 1 - suffix], next[next.Count - 1 - suffix])) suffix++;
        // Native DataGrid accepts single-item deltas. Do not send range notifications or Reset.
        for (var index = _visible.Count - suffix - 1; index >= prefix; index--) _visible.RemoveAt(index);
        for (var index = prefix; index < next.Count - suffix; index++) _visible.Insert(index, next[index]);
    }

    private void Reconnect(HashSet<INotifyCollectionChanged> collections)
    {
        foreach (var source in _collections.Where(source => !collections.Contains(source)).ToArray())
        {
            source.CollectionChanged -= SourceChanged;
            _collections.Remove(source);
        }

        foreach (var source in collections)
            if (_collections.Add(source))
                source.CollectionChanged += SourceChanged;
        var models = new HashSet<INotifyPropertyChanged>(_nodes.Keys.OfType<INotifyPropertyChanged>(),
            ReferenceEqualityComparer.Instance);
        foreach (var model in _models.Where(model => !models.Contains(model)).ToArray())
        {
            model.PropertyChanged -= ModelChanged;
            _models.Remove(model);
        }

        foreach (var model in models)
            if (_models.Add(model))
                model.PropertyChanged += ModelChanged;
    }

    private void SourceChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        RefreshTree();
    }

    private void ModelChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (sender is T item && _nodes.TryGetValue(item, out var node))
        {
            if (!ReferenceEquals(node.Source, _children(item)))
            {
                RefreshTree();
            }
            else if (_treeFilter is not null || SortDescriptions.Count > 0)
            {
                _queryPending = true;
                QueuePending();
            }
        }
    }

    private void EditStateChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is nameof(IsEditingItem) or nameof(IsAddingNew))
        {
            QueuePending();
        }
        else if (args.PropertyName == nameof(Culture))
        {
            _queryPending = true;
            QueuePending();
        }
    }

    private void QueuePending()
    {
        if ((!_pending && !_queryPending) || _queued || _disposed || _queryDeferLevel > 0 || IsEditingItem ||
            IsAddingNew) return;
        _queued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _queued = false;
            if (_disposed || IsEditingItem || IsAddingNew || _queryDeferLevel > 0) return;
            try
            {
                if (_pending)
                {
                    RefreshTree();
                }
                else if (_queryPending)
                {
                    _queryPending = false;
                    ApplyQuery();
                }
            }
            catch (Exception exception)
            {
                _pending = _queryPending = false;
                SetQueryError(exception.Message);
            }
        });
    }

    private void VerifyAccess()
    {
        Dispatcher.UIThread.VerifyAccess();
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private sealed record Node(T Item, T? Parent, int Depth, IEnumerable<T>? Source, T[] Children);
}