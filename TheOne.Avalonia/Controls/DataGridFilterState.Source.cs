using System.Collections.Specialized;
using System.ComponentModel;
using global::Avalonia.Collections;
using global::Avalonia.Controls;
using global::Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

internal sealed partial class DataGridFilterState
{
    private readonly HashSet<INotifyPropertyChanged> _rows = new(ReferenceEqualityComparer.Instance);
    private INotifyCollectionChanged? _source;
    private static bool Editing(IDataGridCollectionView view) => view is DataGridCollectionView edit && (edit.IsEditingItem || edit.IsAddingNew);
    private void ConnectView()
    {
        if (ReferenceEquals(_view, _grid.CollectionView)) return;
        DisconnectView(); _view = _grid.CollectionView;
        if (_view is null) return;
        _baseFilter = _view.Filter;
        if (_view is INotifyPropertyChanged notifying) notifying.PropertyChanged += ViewChanged;
        _view.CollectionChanged += ViewCollectionChanged;
        _source = _view.SourceCollection as INotifyCollectionChanged;
        if (_source is not null) _source.CollectionChanged += SourceChanged;
    }
    private void ViewChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (_writingView || !_active) return;
        if (args.PropertyName == "Filter") { _baseFilter = _view!.Filter; _installed = null; QueueRebuild(); }
        else if (args.PropertyName is "IsEditingItem" or "IsAddingNew") QueueRebuild();
    }
    private void ViewCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (!_writingView && !_busy && _installed is not null) BindRows();
    }
    private void SourceChanged(object? sender, NotifyCollectionChangedEventArgs args) { BindRows(); QueueRebuild(); }
    private void RowChanged(object? sender, PropertyChangedEventArgs args) => QueueRebuild();
    private void BindRows()
    {
        var current = new HashSet<INotifyPropertyChanged>(ReferenceEqualityComparer.Instance);
        if (_installed is not null && _view is not null)
        {
            var automatic = _applied.Where(pair => pair.Value is not null).Select(pair => DataGridAutoFields.Resolve(pair.Key)).OfType<DataGridAutoFields.AutomaticAccessor>().ToArray();
            void Observe(object item) { if (item is INotifyPropertyChanged notifying) current.Add(notifying); }
            foreach (var item in _view.SourceCollection)
            {
                if (item is null) continue;
                Observe(item);
                foreach (var field in automatic)
                    try { field.GetValue(item, Observe); } catch { /* ApplyCandidate publishes getter errors. */ }
            }
        }
        foreach (var row in _rows.Where(row => !current.Contains(row)).ToArray()) { row.PropertyChanged -= RowChanged; _rows.Remove(row); }
        foreach (var row in current) if (_rows.Add(row)) row.PropertyChanged += RowChanged;
    }
    private void QueueRebuild()
    {
        _needRebuild = true;
        if (_queued || !_active) return;
        _queued = true;
        Dispatcher.UIThread.Post(() =>
        {
            _queued = false;
            if (!_active || !_needRebuild || _busy || _view is null || Editing(_view)) return;
            _needRebuild = false;
            var candidate = new Dictionary<DataGridColumn, DataGridFilterCondition?>(_applied);
            foreach (var column in _grid.Columns)
            {
                var condition = candidate.GetValueOrDefault(column);
                if (Validate(column, condition) is { } error)
                {
                    candidate[column] = null; Publish(column, null);
                    column.SetValue(DataGridColumnOptions.FilterErrorProperty, error);
                }
            }
            var result = ApplyCandidate(candidate, false);
            if (!result.Succeeded)
                foreach (var (column, condition) in candidate)
                    if (condition is not null) column.SetValue(DataGridColumnOptions.FilterErrorProperty, result.Error);
        }, DispatcherPriority.Background);
    }
    private void DisconnectView()
    {
        foreach (var row in _rows) row.PropertyChanged -= RowChanged;
        _rows.Clear();
        if (_source is not null) _source.CollectionChanged -= SourceChanged;
        _source = null;
        var view = _view;
        if (view is null) return;
        view.CollectionChanged -= ViewCollectionChanged;
        if (view is INotifyPropertyChanged notify) notify.PropertyChanged -= ViewChanged;
        if (_installed is not null && ReferenceEquals(view.Filter, _installed))
        {
            var installed = _installed; var business = _baseFilter;
            if (!Editing(view)) view.Filter = business!;
            else if (view is INotifyPropertyChanged pending)
            {
                // A deferred restore captures only the data view and predicates, never the detached grid.
                PropertyChangedEventHandler? restore = null;
                restore = (_, _) =>
                {
                    if (!ReferenceEquals(view.Filter, installed)) { pending.PropertyChanged -= restore; return; }
                    if (Editing(view)) return;
                    pending.PropertyChanged -= restore; view.Filter = business!;
                };
                pending.PropertyChanged += restore;
            }
        }
        if (Owners.TryGetValue(view, out var owner) && owner.TryGetTarget(out var state) && state == this) Owners.Remove(view);
        _view = null; _baseFilter = _installed = null;
    }
    private void Stop()
    {
        if (!_active) return;
        _active = false;
        _grid.PropertyChanged -= GridChanged; _grid.Columns.CollectionChanged -= ColumnsChanged; _grid.RowEditEnded -= EditEnded;
        foreach (var column in _columns)
        {
            column.PropertyChanged -= ColumnChanged;
            if (column is DataGridBoundColumn bound) bound.ModelBindingChanged -= ModelBindingChanged;
        }
        _columns.Clear(); DisconnectView();
    }
}
