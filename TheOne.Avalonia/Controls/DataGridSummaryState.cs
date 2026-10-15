using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using global::Avalonia;
using global::Avalonia.Collections;
using global::Avalonia.Controls;
using global::Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

internal sealed class DataGridSummaryState
{
    private readonly DataGrid _grid;
    private readonly Dictionary<DataGridColumn, DataGridSummaryResult> _results = new();
    private readonly Dictionary<DataGridColumn, string> _text = new();
    private readonly HashSet<DataGridColumn> _columns = new();
    private readonly HashSet<INotifyPropertyChanged> _observed = new(ReferenceEqualityComparer.Instance);
    private IDataGridCollectionView? _view;
    private INotifyCollectionChanged? _source;
    private bool _active, _queued;
    public event EventHandler? Updated;
    public DataGridSummaryState(DataGrid grid)
    {
        _grid = grid;
        grid.AttachedToVisualTree += (_, _) => Synchronize();
        grid.DetachedFromVisualTree += (_, _) => Stop();
    }
    public DataGridSummaryResult? GetResult(DataGridColumn column) => _results.GetValueOrDefault(column);
    public string GetText(DataGridColumn column) => _text.GetValueOrDefault(column, "");
    public void Synchronize()
    {
        if (!DataGridFeatures.GetShowSummaryRow(_grid)) { Stop(); return; }
        if (_active) { Queue(); return; }
        _active = true;
        _grid.PropertyChanged += GridChanged;
        _grid.Columns.CollectionChanged += ColumnsChanged;
        _grid.RowEditEnded += EditEnded;
        BindColumns(); ConnectView(); Queue();
    }
    private void GridChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == DataGrid.CollectionViewProperty) { ConnectView(); Queue(); }
        else if (args.Property == DataGridFeatures.SummaryScopeProperty || args.Property == DataGridFeatures.SummaryLeavesOnlyProperty || args.Property == DataGridFeatures.SummaryEmptyTextProperty || args.Property == DataGridFeatures.SummaryLabelProperty) Queue();
    }
    private void ColumnsChanged(object? sender, NotifyCollectionChangedEventArgs args) { BindColumns(); Queue(); }
    private void EditEnded(object? sender, DataGridRowEditEndedEventArgs args) => Queue();
    private void ColumnChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == DataGridColumnOptions.SummaryEnabledProperty || args.Property == DataGridColumnOptions.SummaryKindProperty || args.Property == DataGridColumnOptions.SummaryFormatProperty || args.Property == DataGridColumnOptions.ValueAccessorProperty || args.Property == DataGridColumn.IsVisibleProperty) Queue();
    }
    private void ModelBindingChanged(object? sender, EventArgs args) => Queue();
    private void BindColumns()
    {
        foreach (var old in _columns.Where(column => !_grid.Columns.Contains(column)).ToArray()) { Unhook(old); _columns.Remove(old); }
        foreach (var column in _grid.Columns)
            if (_columns.Add(column))
            {
                column.PropertyChanged += ColumnChanged;
                if (column is DataGridBoundColumn bound) bound.ModelBindingChanged += ModelBindingChanged;
            }
    }
    private void Unhook(DataGridColumn column)
    {
        column.PropertyChanged -= ColumnChanged;
        if (column is DataGridBoundColumn bound) bound.ModelBindingChanged -= ModelBindingChanged;
    }
    private void ConnectView()
    {
        if (ReferenceEquals(_view, _grid.CollectionView)) return;
        DisconnectView(); _view = _grid.CollectionView;
        if (_view is null) return;
        _view.CollectionChanged += CollectionChanged;
        if (_view is IDataGridTreeQueryView tree) tree.TreeChanged += TreeChanged;
        if (_view is INotifyPropertyChanged notifying) notifying.PropertyChanged += ViewChanged;
        _source = _view.SourceCollection as INotifyCollectionChanged;
        if (_source is not null) _source.CollectionChanged += CollectionChanged;
    }
    private void TreeChanged(object? sender, EventArgs args) => Queue();
    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs args) => Queue();
    private void ViewChanged(object? sender, PropertyChangedEventArgs args) => Queue();
    private void RowChanged(object? sender, PropertyChangedEventArgs args) => Queue();
    private void DisconnectView()
    {
        if (_view is not null)
        {
            _view.CollectionChanged -= CollectionChanged;
            if (_view is IDataGridTreeQueryView tree) tree.TreeChanged -= TreeChanged;
            if (_view is INotifyPropertyChanged notifying) notifying.PropertyChanged -= ViewChanged;
        }
        if (_source is not null) _source.CollectionChanged -= CollectionChanged;
        _view = null; _source = null;
        foreach (var item in _observed) item.PropertyChanged -= RowChanged;
        _observed.Clear();
    }
    public void Queue()
    {
        if (!_active || _queued) return;
        _queued = true;
        Dispatcher.UIThread.Post(() => { _queued = false; if (_active) Recalculate(); }, DispatcherPriority.Background);
    }
    private sealed record Entry(DataGridColumn Column, IDataGridValueAccessor Field, DataGridSummaryAccumulator Accumulator);
    private void Recalculate()
    {
        var culture = _view?.Culture ?? CultureInfo.CurrentCulture;
        var entries = new List<Entry>();
        var next = new Dictionary<DataGridColumn, DataGridSummaryResult>();
        var observers = new HashSet<INotifyPropertyChanged>(ReferenceEqualityComparer.Instance);
        void Observe(object instance) { if (instance is INotifyPropertyChanged notifying) observers.Add(notifying); }
        foreach (var column in _grid.Columns.Where(column => column.IsVisible && DataGridColumnOptions.GetSummaryEnabled(column)))
        {
            var field = DataGridAutoFields.Resolve(column);
            if (DataGridFilterParsing.Kind(field, DataGridFilterKind.Auto) != DataGridFilterKind.Number)
                next[column] = new(null, 0, "统计仅支持数值／可空数值实体属性。");
            else entries.Add(new(column, field!, new(field!.ValueType, DataGridColumnOptions.GetSummaryKind(column))));
        }
        try
        {
            var tree = _view as IDataGridTreeQueryView;
            var scope = DataGridFeatures.GetSummaryScope(_grid);
            var visible = scope == DataGridSummaryScope.VisibleItems && _view is not null
                ? new HashSet<object>(_view.Cast<object>(), ReferenceEqualityComparer.Instance) : null;
            if (_view is not null && entries.Count > 0)
                foreach (var item in tree?.LoadedItems ?? _view.SourceCollection)
                {
                    if (item is null) continue;
                    Observe(item);
                    bool match = scope switch
                    {
                        DataGridSummaryScope.SourceItems => true,
                        DataGridSummaryScope.VisibleItems => visible!.Contains(item),
                        _ => tree?.IsFilterMatch(item) ?? (_view.Filter?.Invoke(item) ?? true)
                    };
                    if (tree is not null && DataGridFeatures.GetSummaryLeavesOnly(_grid) && !tree.IsLeaf(item)) match = false;
                    foreach (var entry in entries)
                    {
                        try
                        {
                            var value = entry.Field is DataGridAutoFields.AutomaticAccessor auto ? auto.GetValue(item, Observe) : entry.Field.GetValue(item);
                            if (match) entry.Accumulator.Add(value);
                        }
                        catch (Exception exception) { if (match) entry.Accumulator.Fail("字段读取失败：" + exception.Message); }
                    }
                }
        }
        catch (Exception exception) { foreach (var entry in entries) entry.Accumulator.Fail("数据源读取失败：" + exception.Message); }
        foreach (var entry in entries) next[entry.Column] = entry.Accumulator.Result();
        _text.Clear();
        foreach (var column in next.Keys.ToArray())
        {
            var result = next[column];
            try
            {
                _text[column] = result.Error is not null ? "!" : result.Value is null ? DataGridFeatures.GetSummaryEmptyText(_grid) :
                    result.Value is IFormattable formatted ? formatted.ToString(DataGridColumnOptions.GetSummaryFormat(column), culture) : result.Value.ToString() ?? "";
            }
            catch (FormatException exception) { next[column] = result with { Error = "统计格式无效：" + exception.Message }; _text[column] = "!"; }
        }
        foreach (var item in _observed.Where(item => !observers.Contains(item)).ToArray()) { item.PropertyChanged -= RowChanged; _observed.Remove(item); }
        foreach (var item in observers) if (_observed.Add(item)) item.PropertyChanged += RowChanged;
        _results.Clear(); foreach (var pair in next) _results.Add(pair.Key, pair.Value);
        Updated?.Invoke(this, EventArgs.Empty);
        _grid.RaiseEvent(new DataGridSummaryUpdatedEventArgs());
    }
    private void Stop()
    {
        if (!_active) return;
        _active = false;
        _grid.PropertyChanged -= GridChanged; _grid.Columns.CollectionChanged -= ColumnsChanged; _grid.RowEditEnded -= EditEnded;
        foreach (var column in _columns) Unhook(column);
        _columns.Clear(); DisconnectView(); _results.Clear(); _text.Clear();
        Updated?.Invoke(this, EventArgs.Empty);
    }
}
