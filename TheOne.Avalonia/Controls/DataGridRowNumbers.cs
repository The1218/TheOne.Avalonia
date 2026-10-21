using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>原生 DataGrid 行头序号，支持分页偏移、虚拟化复用和关闭后恢复原设置。</summary>
public static class DataGridRowNumbers
{
    /// <summary>显式启用行号，不改变未配置此能力的表格。</summary>
    public static readonly AttachedProperty<bool> IsVisibleProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, bool>("IsVisible", typeof(DataGridRowNumbers));
    /// <summary>从 1 开始的页码，与 Pagination 的 long 页码一致。</summary>
    public static readonly AttachedProperty<long> PageIndexProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, long>("PageIndex", typeof(DataGridRowNumbers), 1,
            validate: value => value >= 1);
    /// <summary>每页记录数，必须大于零。</summary>
    public static readonly AttachedProperty<int> PageSizeProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, int>("PageSize", typeof(DataGridRowNumbers), 50,
            validate: value => value > 0);
    /// <summary>行头宽度，不接受零、无穷或 NaN。</summary>
    public static readonly AttachedProperty<double> HeaderWidthProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, double>("HeaderWidth", typeof(DataGridRowNumbers), 56,
            validate: value => double.IsFinite(value) && value > 0);
    private static readonly AttachedProperty<State?> StateProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, State?>("State", typeof(DataGridRowNumbers));

    static DataGridRowNumbers()
    {
        IsVisibleProperty.Changed.AddClassHandler<DataGrid>((grid, _) => Update(grid));
        PageIndexProperty.Changed.AddClassHandler<DataGrid>((grid, _) => Update(grid));
        PageSizeProperty.Changed.AddClassHandler<DataGrid>((grid, _) => Update(grid));
        HeaderWidthProperty.Changed.AddClassHandler<DataGrid>((grid, _) => Update(grid));
    }

    /// <summary>读取行号启用状态。</summary>
    public static bool GetIsVisible(DataGrid grid) => grid.GetValue(IsVisibleProperty);
    /// <summary>切换行号；关闭后解除事件并恢复原行头设置。</summary>
    public static void SetIsVisible(DataGrid grid, bool value) => grid.SetValue(IsVisibleProperty, value);
    /// <summary>读取页码。</summary>
    public static long GetPageIndex(DataGrid grid) => grid.GetValue(PageIndexProperty);
    /// <summary>设置页码。</summary>
    public static void SetPageIndex(DataGrid grid, long value) => grid.SetValue(PageIndexProperty, value);
    /// <summary>读取页大小。</summary>
    public static int GetPageSize(DataGrid grid) => grid.GetValue(PageSizeProperty);
    /// <summary>设置页大小。</summary>
    public static void SetPageSize(DataGrid grid, int value) => grid.SetValue(PageSizeProperty, value);
    /// <summary>读取行头宽度。</summary>
    public static double GetHeaderWidth(DataGrid grid) => grid.GetValue(HeaderWidthProperty);
    /// <summary>设置行头宽度。</summary>
    public static void SetHeaderWidth(DataGrid grid, double value) => grid.SetValue(HeaderWidthProperty, value);

    /// <summary>仅为已启用的表格维护事件状态，关闭后不残留行容器引用。</summary>
    private static void Update(DataGrid grid)
    {
        var state = grid.GetValue(StateProperty);
        if (GetIsVisible(grid))
        {
            if (state is null) grid.SetValue(StateProperty, state = new State(grid));
            state.Refresh();
        }
        else if (state is not null)
        {
            state.Dispose();
            grid.ClearValue(StateProperty);
        }
    }

    private sealed class State : IDisposable
    {
        private readonly DataGrid _grid;
        private readonly DataGridHeadersVisibility _headers;
        private readonly double _width;
        private readonly Dictionary<DataGridRow, object?> _original = [];

        /// <summary>启用时保留原设置；行卸载和关闭时释放行容器引用。</summary>
        public State(DataGrid grid)
        {
            _grid = grid;
            _headers = grid.HeadersVisibility;
            _width = grid.RowHeaderWidth;
            grid.LoadingRow += Loading;
            grid.UnloadingRow += Unloading;
            grid.LayoutUpdated += LayoutUpdated;
        }

        /// <summary>索引因排序或容器复用变化时，只刷新实际可见行。</summary>
        private void LayoutUpdated(object? sender, EventArgs e) => RefreshRows();
        /// <summary>新建或复用行时写入对应索引。</summary>
        private void Loading(object? sender, DataGridRowEventArgs e) => SetHeader(e.Row);
        /// <summary>卸载行时恢复宿主原行头并释放引用。</summary>
        private void Unloading(object? sender, DataGridRowEventArgs e)
        {
            if (_original.Remove(e.Row, out var header)) e.Row.SetCurrentValue(DataGridRow.HeaderProperty, header);
        }

        /// <summary>更新可见性、宽度和分页序号，不重建原生行模板。</summary>
        public void Refresh()
        {
            _grid.SetCurrentValue(DataGrid.HeadersVisibilityProperty, _grid.HeadersVisibility | DataGridHeadersVisibility.Row);
            _grid.SetCurrentValue(DataGrid.RowHeaderWidthProperty, GetHeaderWidth(_grid));
            RefreshRows();
        }

        /// <summary>布局后校正可见行的显示索引，避免排序和筛选后沿用旧序号。</summary>
        private void RefreshRows()
        {
            foreach (var row in _grid.GetVisualDescendants().OfType<DataGridRow>()) SetHeader(row);
        }

        /// <summary>使用十进制运算避免 long 页码与页大小乘积溢出；序号不依赖业务数据或反射。</summary>
        private void SetHeader(DataGridRow row)
        {
            if (row.Index < 0) return;
            _original.TryAdd(row, row.Header);
            var number = ((decimal)(GetPageIndex(_grid) - 1) * GetPageSize(_grid) + row.Index + 1)
                .ToString(CultureInfo.InvariantCulture);
            if (!Equals(row.Header, number)) row.SetCurrentValue(DataGridRow.HeaderProperty, number);
        }

        /// <summary>停止管理并恢复启用前的可见性、宽度及仍存在的行头。</summary>
        public void Dispose()
        {
            _grid.LoadingRow -= Loading;
            _grid.UnloadingRow -= Unloading;
            _grid.LayoutUpdated -= LayoutUpdated;
            foreach (var (row, header) in _original) row.SetCurrentValue(DataGridRow.HeaderProperty, header);
            _original.Clear();
            _grid.SetCurrentValue(DataGrid.HeadersVisibilityProperty, _headers);
            _grid.SetCurrentValue(DataGrid.RowHeaderWidthProperty, _width);
        }
    }
}
