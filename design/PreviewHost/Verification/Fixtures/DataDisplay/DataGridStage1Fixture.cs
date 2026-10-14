using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

// A diagnostic fixture, not a production implementation of right-frozen columns.
internal sealed class DataGridStage1Fixture
{
    public GridRecord[] Records { get; }
    public DataGrid Grid { get; }
    public DataGridColumn RightColumn => Grid.Columns[^1];
    public DataGridColumnHeader RightHeader => Grid.GetVisualDescendants().OfType<DataGridColumnHeader>()
        .Single(header => Equals(header.Content, RightColumn.Header));
    public DataGridRowsPresenter RowsPresenter => Grid.GetVisualDescendants().OfType<DataGridRowsPresenter>().Single();
    public ScrollBar HorizontalScrollBar => Grid.GetVisualDescendants().OfType<ScrollBar>().Single(bar => bar.Name == "PART_HorizontalScrollbar");
    public DataGridRow[] Rows => Grid.GetVisualDescendants().OfType<DataGridRow>().Where(row => row.IsVisible).ToArray();
    public int CreatedRows { get; private set; }
    public int VisibleRightCells => Rows.Count(row => RightCell(row)?.IsEffectivelyVisible == true);

    public DataGridStage1Fixture(int count = 1000)
    {
        _ = DataGridAot.PreserveRowType<GridRecord>();
        Records = Enumerable.Range(1, count).Select(id => new GridRecord(id, $"设备 {id:00000}", id % 2 == 0 ? "杭州仓" : "上海仓", id % 80 + 1,
            99m + id + 0.5m, true, id == 6 ? "较长备注用于验证自动行高：右侧冻结必须同时处理表头、行高、裁剪、编辑与原生列可见性。" : "标准库存")).ToArray();
        Grid = new DataGrid { ItemsSource = Records, AutoGenerateColumns = false, MaxColumnWidth = 320,
            FrozenColumnCount = 1, CanUserResizeColumns = true, CanUserReorderColumns = true, CanUserSortColumns = true };
        Grid.Columns.Add(new DataGridTextColumn { Header = "编号", Width = new DataGridLength(80), IsReadOnly = true,
            Binding = global::Avalonia.Data.CompiledBinding.Create<GridRecord, int>(row => row.Id) });
        Grid.Columns.Add(new DataGridTextColumn { Header = "设备", Width = new DataGridLength(180),
            Binding = global::Avalonia.Data.CompiledBinding.Create<GridRecord, string>(row => row.Name, mode: global::Avalonia.Data.BindingMode.TwoWay) });
        Grid.Columns.Add(new DataGridTextColumn { Header = "仓库", Width = new DataGridLength(120),
            Binding = global::Avalonia.Data.CompiledBinding.Create<GridRecord, string>(row => row.Warehouse) });
        Grid.Columns.Add(new DataGridTextColumn { Header = "数量", Width = new DataGridLength(100),
            Binding = global::Avalonia.Data.CompiledBinding.Create<GridRecord, int>(row => row.Quantity) });
        Grid.Columns.Add(new DataGridTextColumn { Header = "单价", Width = new DataGridLength(120),
            Binding = global::Avalonia.Data.CompiledBinding.Create<GridRecord, decimal>(row => row.Price) });
        Grid.Columns.Add(new DataGridTextColumn { Header = "备注", Width = new DataGridLength(180),
            Binding = global::Avalonia.Data.CompiledBinding.Create<GridRecord, string>(row => row.Notes) });
        Grid.Columns.Add(new DataGridTextColumn { Header = "右侧价格", Width = new DataGridLength(120),
            Binding = global::Avalonia.Data.CompiledBinding.Create<GridRecord, decimal>(row => row.Price, mode: global::Avalonia.Data.BindingMode.TwoWay) });
        var seen = new HashSet<DataGridRow>(ReferenceEqualityComparer.Instance);
        Grid.LoadingRow += (_, args) => { if (seen.Add(args.Row)) CreatedRows++; };
    }

    public DataGridCell? RightCell(DataGridRow row) => RightColumn.GetCellContent(row)?.GetVisualAncestors().OfType<DataGridCell>().FirstOrDefault();

    // Deliberately uses only public APIs: it changes position, but cannot change the
    // sealed presenter's visibility/measurement and ScrollColumnIntoView decisions.
    public void ApplyPositionOnlyTrial()
    {
        ClearTrial();
        var headers = Grid.GetVisualDescendants().OfType<DataGridColumnHeadersPresenter>().Single();
        var rightX = headers.Bounds.Width - RightColumn.ActualWidth;
        RightHeader.RenderTransform = new TranslateTransform(rightX - RightHeader.Bounds.X, 0);
        var targetX = RightHeader.TranslatePoint(default, Grid)!.Value.X;
        foreach (var row in Rows)
            if (RightCell(row) is { } cell)
                cell.RenderTransform = new TranslateTransform(targetX - cell.TranslatePoint(default, Grid)!.Value.X, 0);
    }

    public void ClearTrial()
    {
        RightHeader.RenderTransform = null;
        foreach (var row in Rows)
            if (RightCell(row) is { } cell) cell.RenderTransform = null;
    }
}
