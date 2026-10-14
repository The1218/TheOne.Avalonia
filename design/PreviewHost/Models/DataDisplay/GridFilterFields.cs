using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public static class GridFilterFields
{
    public static IDataGridValueAccessor Id { get; } = DataGridValueAccessor.Create<GridRecord, int>(row => row.Id);
    public static IDataGridValueAccessor Name { get; } = DataGridValueAccessor.Create<GridRecord, string>(row => row.Name);
    public static IDataGridValueAccessor Warehouse { get; } = DataGridValueAccessor.Create<GridRecord, string>(row => row.Warehouse);
    public static IDataGridValueAccessor Quantity { get; } = DataGridValueAccessor.Create<GridRecord, int>(row => row.Quantity);
    public static IDataGridValueAccessor Price { get; } = DataGridValueAccessor.Create<GridRecord, decimal>(row => row.Price);
    public static IDataGridValueAccessor Enabled { get; } = DataGridValueAccessor.Create<GridRecord, bool>(row => row.Enabled);
    public static IDataGridValueAccessor Notes { get; } = DataGridValueAccessor.Create<GridRecord, string>(row => row.Notes);
}
