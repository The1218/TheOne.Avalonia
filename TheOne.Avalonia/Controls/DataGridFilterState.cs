using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using global::Avalonia;
using global::Avalonia.Collections;
using global::Avalonia.Controls;
using global::Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

internal sealed partial class DataGridFilterState
{
    private static readonly ConditionalWeakTable<IDataGridCollectionView, WeakReference<DataGridFilterState>> Owners = new();
    private readonly DataGrid _grid;
    private readonly Dictionary<DataGridColumn, DataGridFilterCondition?> _applied = new();
    private readonly HashSet<DataGridColumn> _columns = new();
    private IDataGridCollectionView? _view;
    private Func<object, bool>? _baseFilter, _installed;
    private bool _active, _busy, _publishing, _writingView, _queued, _needRebuild;

    public DataGridFilterState(DataGrid grid)
    {
        _grid = grid;
        grid.AttachedToVisualTree += (_, _) => Start();
        grid.DetachedFromVisualTree += (_, _) => Stop();
    }
    public void Start()
    {
        if (_active) return;
        _active = true;
        _grid.PropertyChanged += GridChanged;
        _grid.Columns.CollectionChanged += ColumnsChanged;
        _grid.RowEditEnded += EditEnded;
        BindColumns(); ConnectView(); QueueRebuild();
    }
    private void GridChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == DataGrid.CollectionViewProperty) { ConnectView(); QueueRebuild(); }
    }
    private void EditEnded(object? sender, DataGridRowEditEndedEventArgs args) => QueueRebuild();
    private void ColumnsChanged(object? sender, NotifyCollectionChangedEventArgs args) { BindColumns(); QueueRebuild(); }
    private void BindColumns()
    {
        foreach (var removed in _applied.Keys.Where(column => !_grid.Columns.Contains(column)).ToArray()) _applied.Remove(removed);
        foreach (var old in _columns.Where(column => !_grid.Columns.Contains(column)).ToArray())
        {
            old.PropertyChanged -= ColumnChanged;
            if (old is DataGridBoundColumn oldBound) oldBound.ModelBindingChanged -= ModelBindingChanged; _columns.Remove(old); _applied.Remove(old);
        }
        foreach (var column in _grid.Columns)
            if (_columns.Add(column))
            {
                column.PropertyChanged += ColumnChanged;
                if (column is DataGridBoundColumn bound) bound.ModelBindingChanged += ModelBindingChanged;
                _applied[column] = Normalize(DataGridColumnOptions.GetFilterCondition(column));
            }
    }
    private void ModelBindingChanged(object? sender, EventArgs args) => QueueRebuild();
    private void ColumnChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (_publishing || sender is not DataGridColumn column) return;
        if (args.Property == DataGridColumnOptions.FilterConditionProperty)
        {
            var result = Apply(column, DataGridColumnOptions.GetFilterCondition(column));
            if (!result.Succeeded)
            {
                Publish(column, _applied.GetValueOrDefault(column));
                column.SetValue(DataGridColumnOptions.FilterErrorProperty, result.Error);
            }
        }
        else if (args.Property == DataGridColumnOptions.ValueAccessorProperty || args.Property == DataGridColumnOptions.FilterKindProperty || args.Property == DataGridColumnOptions.FilterEnabledProperty)
            QueueRebuild();
    }
    private static DataGridFilterCondition? Normalize(DataGridFilterCondition? value) => value?.IsEmpty == true ? null : value;
    private void Publish(DataGridColumn column, DataGridFilterCondition? condition)
    {
        _publishing = true;
        try { column.SetCurrentValue(DataGridColumnOptions.FilterConditionProperty, condition); }
        finally { _publishing = false; }
    }
    public DataGridFilterApplyResult Apply(DataGridColumn column, DataGridFilterCondition? condition)
    {
        Start();
        if (!_grid.Columns.Contains(column)) return new(false, "筛选列不属于当前表格。");
        var candidate = new Dictionary<DataGridColumn, DataGridFilterCondition?>(_applied) { [column] = Normalize(condition) };
        var result = ApplyCandidate(candidate, true);
        column.SetValue(DataGridColumnOptions.FilterErrorProperty, result.Error);
        return result;
    }
    public DataGridFilterApplyResult ClearAll()
    {
        Start();
        return ApplyCandidate(_grid.Columns.ToDictionary(column => column, _ => (DataGridFilterCondition?)null), true);
    }
    private static string? Validate(DataGridColumn column, DataGridFilterCondition? condition)
    {
        if (condition is null) return null;
        var kind = DataGridColumnOptions.EffectiveKind(column);
        var field = DataGridAutoFields.Resolve(column);
        if (kind == DataGridFilterKind.None || field is null) return "该列没有可读取的文本或数值实体属性，或已关闭筛选。";
        if (!condition.Supports(field.ValueType) || (kind == DataGridFilterKind.Text) != (condition is TextFilterCondition)) return "筛选条件与字段类型不匹配。";
        return null;
    }
    private DataGridFilterApplyResult ApplyCandidate(Dictionary<DataGridColumn, DataGridFilterCondition?> candidate, bool notify)
    {
        if (_busy) return new(false, "正在处理筛选变更，请稍后重试。");
        foreach (var (column, condition) in candidate)
            if (Validate(column, condition) is { } error) return new(false, error);
        if (_view is not DataGridCollectionView || !_view.CanFilter) return new(false, "当前数据视图尚未就绪；列筛选需要官方 DataGridCollectionView。");
        if (Owners.TryGetValue(_view, out var owner) && owner.TryGetTarget(out var state) && state != this && state._active)
            return new(false, "此 CollectionView 已由另一个表格管理筛选，请为表格使用独立视图。");
        if (!_grid.CommitEdit(DataGridEditingUnit.Row, true) || Editing(_view)) return new(false, "请先完成当前编辑；验证失败的内容不会被筛选丢弃。");
        _busy = true;
        try
        {
            var oldConditions = new Dictionary<DataGridColumn, DataGridFilterCondition?>(_applied);
            var changes = candidate.Where(pair => !Equals(pair.Value, _applied.GetValueOrDefault(pair.Key))).ToArray();
            if (notify)
                foreach (var (column, condition) in changes)
                {
                    var changing = new DataGridFilterChangingEventArgs(column, _applied.GetValueOrDefault(column), condition);
                    _grid.RaiseEvent(changing);
                    if (changing.Cancel) return new(false, "筛选变更已取消。");
                }
            // Events may change column configuration. Validate again before installing a predicate.
            foreach (var (column, condition) in candidate)
                if (Validate(column, condition) is { } error) return new(false, error);
            if (!ReferenceEquals(_view.Filter, _installed)) _baseFilter = _view.Filter;
            var clauses = candidate.Where(pair => pair.Value is not null).Select(pair => new Clause(DataGridAutoFields.Resolve(pair.Key)!, pair.Value!)).ToArray();
            var business = _baseFilter;
            bool Predicate(object item) => (business?.Invoke(item) ?? true) && clauses.All(clause => clause.Match(item));
            // Accessor failures must not leave a half-refreshed view or overwrite valid conditions.
            try
            {
                foreach (var item in _view.SourceCollection) { _ = Predicate(item!); }
                if (clauses.FirstOrDefault(clause => clause.Error is not null)?.Error is { } error) return new(false, error);
            }
            catch (Exception exception) { return new(false, "无法读取筛选数据：" + exception.Message); }
            var previous = _view.Filter;
            var next = clauses.Length == 0 ? business : Predicate;
            var ownedBefore = Owners.TryGetValue(_view, out var priorOwner) ? priorOwner : null;
            Owners.Remove(_view); Owners.Add(_view, new(this));
            _writingView = true;
            try { _view.Filter = next!; }
            catch (Exception exception)
            {
                _view.Filter = previous;
                Owners.Remove(_view);
                if (ownedBefore is not null) Owners.Add(_view, ownedBefore);
                return new(false, "数据视图无法应用筛选：" + exception.Message);
            }
            finally { _writingView = false; }
            _installed = clauses.Length == 0 ? null : next;
            Owners.Remove(_view);
            if (_installed is not null) Owners.Add(_view, new(this));
            _applied.Clear();
            foreach (var (column, condition) in candidate)
            {
                _applied[column] = condition; Publish(column, condition);
                column.SetValue(DataGridColumnOptions.FilterErrorProperty, null);
            }
            BindRows();
            if (notify)
                foreach (var (column, condition) in changes)
                    _grid.RaiseEvent(new DataGridFilterChangedEventArgs(column, oldConditions.GetValueOrDefault(column), condition));
            return new(true);
        }
        finally { _busy = false; }
    }
    // A predicate holds only immutable field/condition data, never a grid, column or UI container.
    private sealed class Clause(IDataGridValueAccessor field, DataGridFilterCondition condition)
    {
        public string? Error { get; private set; }
        public bool Match(object item)
        {
            try
            {
                var value = field.GetValue(item);
                if (value is not null && value.GetType() != (Nullable.GetUnderlyingType(field.ValueType) ?? field.ValueType))
                    throw new ArgumentException("accessor 返回值与声明的字段类型不匹配。");
                return condition.Matches(value);
            }
            catch (Exception exception) { Error = "字段读取失败：" + exception.Message; return false; }
        }
    }
}
