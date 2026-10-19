using System.Collections.Specialized;

namespace TheOne.Avalonia.Controls;

public partial class GanttChart
{
    [Flags]
    private enum TaskChange { Presentation = 1, Values = 2, Expansion = 4, Structure = 8 }

    private readonly Dictionary<GanttTask, TaskChange> _dirtyTasks = new(ReferenceEqualityComparer.Instance);
    private bool _structureDirty;

    private void ObserveSource(GanttTask[] source)
    {
        if (!_active) return;
        var collection = ItemsSource as INotifyCollectionChanged;
        if (!ReferenceEquals(_collection, collection))
        {
            if (_collection is not null) _collection.CollectionChanged -= OnCollection;
            _collection = collection;
            if (_collection is not null) _collection.CollectionChanged += OnCollection;
        }
        var current = new HashSet<GanttTask>(source.Where(task => task is not null), ReferenceEqualityComparer.Instance);
        foreach (var task in _observed.Where(task => !current.Contains(task)).ToArray())
        {
            task.PropertyChanged -= OnTaskChanged;
            _observed.Remove(task);
        }
        foreach (var task in current)
            if (_observed.Add(task)) task.PropertyChanged += OnTaskChanged;
    }

    private Row? SelectedRow() => SelectedTask is { } task ? _taskRows.GetValueOrDefault(task) : null;

    private bool IsRowVisible(Row row) => row.Index >= 0 && row.Index < _rows.Count && ReferenceEquals(_rows[row.Index], row);

    private static string? CalculateValues(Row row)
    {
        var task = row.Task;
        if (row.Children.Count > 0)
        {
            if (task.IsMilestone) return "A milestone cannot have children.";
            row.Start = row.Children.Min(child => child.Start);
            row.End = row.Children.Max(child => child.End);
            row.Weight = row.Children.Sum(child => child.Weight);
            row.CompletedWeight = row.Children.Sum(child => child.CompletedWeight);
            row.Progress = row.Weight > 0 ? row.CompletedWeight / row.Weight : 0;
        }
        else
        {
            if (!double.IsFinite(task.Progress) || task.Progress < 0 || task.Progress > 1 ||
                (!task.IsMilestone && task.EndExclusive <= task.Start))
                return "Leaf tasks need Progress in [0, 1] and EndExclusive after Start.";
            row.Start = task.Start;
            row.End = task.IsMilestone
                ? task.Start < DateOnly.MaxValue ? task.Start.AddDays(1) : task.Start
                : task.EndExclusive;
            row.Progress = task.Progress;
            row.Weight = task.IsMilestone ? 0 : row.End.DayNumber - row.Start.DayNumber;
            row.CompletedWeight = row.Weight * row.Progress;
        }
        return null;
    }

    private void ApplyPendingUpdates()
    {
        if (_structureDirty)
        {
            RefreshData();
            return;
        }
        if (_dirtyTasks.Count == 0) return;
        var changes = _dirtyTasks.ToArray();
        _dirtyTasks.Clear();
        var painted = new HashSet<Row>();
        var projectionChanged = false;
        var restoreFocus = _list?.IsKeyboardFocusWithin == true;
        foreach (var (task, change) in changes)
        {
            if (!_taskRows.TryGetValue(task, out var row)) continue;
            painted.Add(row);
            if ((change & TaskChange.Values) != 0 && !UpdateValues(row, painted))
            {
                // Keep the established all-or-nothing validation and recovery contract.
                RefreshData();
                return;
            }
            if ((change & TaskChange.Expansion) != 0) projectionChanged |= UpdateExpansion(row, painted);
        }
        if (projectionChanged)
        {
            SetAndRaise(VisibleTaskCountProperty, ref _visibleTaskCount, _rows.Count);
            var selected = SelectedRow();
            while (selected is not null && !IsRowVisible(selected)) selected = selected.Parent;
            SetCurrentValue(SelectedTaskProperty, selected?.Task);
            SyncSelection();
            UpdateEmpty();
            if (restoreFocus && _list is { SelectedIndex: >= 0 } list)
                list.ContainerFromIndex(list.SelectedIndex)?.Focus();
        }
        else UpdateSelectionInfo();
        foreach (var presenter in Presenters())
            if (painted.Contains(presenter.Data)) presenter.RefreshTask();
    }

    private static bool UpdateValues(Row row, HashSet<Row> painted)
    {
        if (row.Children.Count > 0) return !row.Task.IsMilestone; // Summary model dates/progress are ignored.
        var before = Values(row);
        if (CalculateValues(row) is not null) return false;
        while (row.Parent is { } parent && before != Values(row))
        {
            var oldParent = Values(parent);
            parent.Weight += row.Weight - before.Weight;
            parent.CompletedWeight = parent.Weight > 0
                ? Math.Clamp(parent.CompletedWeight + (row.CompletedWeight - before.CompletedWeight), 0, parent.Weight)
                : 0;
            parent.Progress = parent.Weight > 0 ? parent.CompletedWeight / parent.Weight : 0;
            // Rescan direct siblings only when an old extremum moves inward.
            parent.Start = row.Start < parent.Start ? row.Start
                : before.Start == parent.Start && row.Start > before.Start ? parent.Children.Min(child => child.Start)
                : parent.Start;
            parent.End = row.End > parent.End ? row.End
                : before.End == parent.End && row.End < before.End ? parent.Children.Max(child => child.End)
                : parent.End;
            if (oldParent != Values(parent)) painted.Add(parent);
            row = parent;
            before = oldParent;
        }
        return true;
    }

    private bool UpdateExpansion(Row row, HashSet<Row> painted)
    {
        if (row.Expanded == row.Task.IsExpanded) return false;
        row.Expanded = row.Task.IsExpanded;
        if (!IsRowVisible(row) || row.Children.Count == 0) return false;
        _selecting = true;
        try
        {
            var index = row.Index + 1;
            if (!row.Expanded)
                while (index < _rows.Count && _rows[index].Depth > row.Depth)
                {
                    _rows[index].Index = -1;
                    _rows.RemoveAt(index);
                }
            else
            {
                var stack = new Stack<Row>(row.Children.AsEnumerable().Reverse());
                while (stack.TryPop(out var child))
                {
                    _rows.Insert(index++, child);
                    if (child.Task.IsExpanded)
                        for (var i = child.Children.Count - 1; i >= 0; i--) stack.Push(child.Children[i]);
                }
            }
            for (var i = row.Index + 1; i < _rows.Count; i++)
            {
                _rows[i].Index = i;
                painted.Add(_rows[i]); // The displayed row number changes, not the row identity.
            }
        }
        finally { _selecting = false; }
        return true;
    }

    private static RowValues Values(Row row) => new(row.Start, row.End, row.Progress, row.Weight, row.CompletedWeight);
    private readonly record struct RowValues(DateOnly Start, DateOnly End, double Progress, double Weight, double CompletedWeight);
}
