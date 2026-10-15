// TheOne extension of the native collection view; see LICENSE.md.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls.Utils;

namespace Avalonia.Collections;

public sealed partial class DataGridTreeCollectionView<T> where T : class
{
    private sealed record Projection(List<T> Visible, HashSet<object> Matches, HashSet<object> Included,
        HashSet<object> ForcedExpanded, HashSet<INotifyPropertyChanged> Observers);
    private HashSet<object> _matches = new(ReferenceEqualityComparer.Instance);
    private HashSet<object> _included = new(ReferenceEqualityComparer.Instance);
    private HashSet<object> _forcedExpanded = new(ReferenceEqualityComparer.Instance);
    private readonly HashSet<INotifyPropertyChanged> _queryObservers = new(ReferenceEqualityComparer.Instance);
    private DataGridSortDescriptionCollection? _treeSorts;
    private DataGridSortDescription[] _appliedSorts = [];
    private Func<object, bool>? _treeFilter, _appliedFilter;
    private int _queryDeferLevel;
    private bool _queryDirty, _restoringQuery, _queryPending;

    /// <summary>Failure of a live query or sort change. The last valid projection is retained.</summary>
    public string? QueryError { get; private set; }

    public override DataGridSortDescriptionCollection SortDescriptions
    {
        get
        {
            if (_treeSorts is null)
            {
                _treeSorts = new();
                _treeSorts.Validate = description =>
                {
                    if (_restoringQuery) return;
                    VerifyQueryChange();
                    ArgumentNullException.ThrowIfNull(description);
                    if (description.HasPropertyPath && typeof(T).GetNestedPropertyType(description.PropertyPath) is null)
                        throw new ArgumentException($"Unknown registered sort path: {description.PropertyPath}.", nameof(description));
                    description.Initialize(typeof(T));
                };
                _treeSorts.CollectionChanged += (_, _) =>
                {
                    if (_restoringQuery) return;
                    _queryDirty = true;
                    if (_queryDeferLevel == 0) CompleteQuery();
                };
            }
            return _treeSorts;
        }
    }

    /// <summary>Direct match predicate. Ancestors of matches remain visible without matching this predicate.</summary>
    public override Func<object, bool> Filter
    {
        get => _treeFilter!;
        set
        {
            VerifyQueryChange();
            if (ReferenceEquals(_treeFilter, value)) return;
            var previous = _treeFilter;
            _treeFilter = value;
            if (_queryDeferLevel > 0) { _queryDirty = true; return; }
            try
            {
                if (!ApplyQuery()) throw new InvalidOperationException("The tree query was cancelled.");
            }
            catch { _treeFilter = previous; throw; }
        }
    }

    public override IDisposable DeferRefresh()
    {
        VerifyQueryChange();
        _queryDeferLevel++;
        return new QueryDeferral(this);
    }

    private sealed class QueryDeferral(DataGridTreeCollectionView<T> owner) : IDisposable
    {
        private DataGridTreeCollectionView<T>? _owner = owner;
        public void Dispose()
        {
            if (_owner is not { } view) return;
            view.VerifyAccess(); _owner = null;
            if (--view._queryDeferLevel == 0) { view.CompleteQuery(); view.QueuePending(); }
        }
    }

    private void VerifyQueryChange()
    {
        VerifyAccess();
        if (_changing || IsEditingItem || IsAddingNew)
            throw new InvalidOperationException("Finish the current tree change or row edit before changing the query.");
    }

    private void CompleteQuery()
    {
        if (!_queryDirty) return;
        _queryDirty = false;
        try
        {
            VerifyQueryChange();
            if (_pending ? RefreshTree() : ApplyQuery()) return;
            RestoreQuery("The tree query was cancelled.");
        }
        catch (Exception exception) { RestoreQuery(exception.Message); }
    }

    private void RestoreQuery(string error)
    {
        _treeFilter = _appliedFilter;
        _restoringQuery = true;
        try { SortDescriptions.Clear(); SortDescriptions.AddRange(_appliedSorts); }
        finally { _restoringQuery = false; }
        SetQueryError(error);
        OnPropertyChanged(nameof(SortDescriptions));
    }

    private void SetQueryError(string? error)
    {
        if (QueryError == error) return;
        QueryError = error; OnPropertyChanged(nameof(QueryError));
    }

    private bool ApplyQuery()
    {
        VerifyAccess();
        if (_changing || IsEditingItem || IsAddingNew) { _queryPending = true; return false; }
        _changing = true;
        try
        {
            var args = new DataGridTreeChangingEventArgs(null, null);
            TreeChanging?.Invoke(this, args);
            if (args.Cancel || IsEditingItem || IsAddingNew) return false;
            var projection = CreateProjection(_nodes, _order);
            bool filterChanged = !ReferenceEquals(_appliedFilter, _treeFilter);
            _appliedFilter = _treeFilter; _appliedSorts = SortDescriptions.ToArray();
            _queryPending = false;
            PublishProjection(projection);
            SetQueryError(null);
            TreeChanged?.Invoke(this, EventArgs.Empty);
            if (filterChanged) OnPropertyChanged(nameof(Filter));
            OnPropertyChanged(nameof(SortDescriptions));
            return true;
        }
        finally { _changing = false; QueuePending(); }
    }

    private Projection CreateProjection(Dictionary<object, Node> nodes, List<Node> order)
    {
        var matches = new HashSet<object>(ReferenceEqualityComparer.Instance);
        foreach (var node in order)
            if (_treeFilter?.Invoke(node.Item) ?? true) matches.Add(node.Item);
        var included = new HashSet<object>(matches, ReferenceEqualityComparer.Instance);
        var forced = new HashSet<object>(ReferenceEqualityComparer.Instance);
        if (_treeFilter is not null)
            for (int index = order.Count - 1; index >= 0; index--)
                if (included.Contains(order[index].Item) && order[index].Parent is { } parent)
                { included.Add(parent); forced.Add(parent); }

        var observers = new HashSet<INotifyPropertyChanged>(ReferenceEqualityComparer.Instance);
        var descriptions = SortDescriptions.ToArray();
        foreach (var description in descriptions)
        {
            description.Initialize(typeof(T));
            if (!description.HasPropertyPath) continue;
            var parts = TypeHelper.SplitPropertyPath(description.PropertyPath);
            foreach (var node in order)
            {
                object? current = node.Item;
                foreach (var part in parts)
                {
                    current = TypeHelper.GetNestedPropertyValue(current, part);
                    if (current is INotifyPropertyChanged notifying && !nodes.ContainsKey(current)) observers.Add(notifying);
                }
            }
        }
        var comparers = descriptions.Select(description => description.Comparer).ToArray();
        IEnumerable<T> Sort(IEnumerable<T> items)
        {
            if (comparers.Length == 0) return items;
            // LINQ's stable ordering preserves source order for ties and after clearing sorts.
            var sorted = items.OrderBy(item => item, comparers[0]);
            for (int index = 1; index < comparers.Length; index++) sorted = sorted.ThenBy(item => item, comparers[index]);
            return sorted;
        }
        var next = new List<T>();
        var stack = new Stack<T>(Sort(order.Where(node => node.Parent is null).Select(node => node.Item)).Reverse());
        while (stack.TryPop(out var item))
        {
            if (!included.Contains(item)) continue;
            next.Add(item);
            if (!_expanded.Contains(item) && !forced.Contains(item)) continue;
            foreach (var child in Sort(nodes[item].Children).Reverse()) stack.Push(child);
        }
        // Read every sibling comparison even in collapsed branches: invalid hidden values
        // must not turn a later expansion into a partially published query.
        if (comparers.Length > 0)
            foreach (var node in order) _ = Sort(node.Children).ToArray();
        return new(next, matches, included, forced, observers);
    }

    private void ReconnectQueryObservers(HashSet<INotifyPropertyChanged> observers)
    {
        foreach (var observer in _queryObservers.Where(observer => !observers.Contains(observer)).ToArray())
        { observer.PropertyChanged -= QueryModelChanged; _queryObservers.Remove(observer); }
        foreach (var observer in observers)
            if (_queryObservers.Add(observer)) observer.PropertyChanged += QueryModelChanged;
    }
    private void QueryModelChanged(object? sender, PropertyChangedEventArgs args) { _queryPending = true; QueuePending(); }
}
