using System;
using Avalonia.Media;

namespace Avalonia.Controls;

public readonly record struct DataGridColumnLayout(Rect Bounds, Rect VisibleBounds);

public partial class DataGrid
{
    /// <summary>Raised after native headers arrange, including width/order/scroll/frozen-region changes.</summary>
    public event EventHandler? ColumnLayoutChanged;
    internal void NotifyColumnLayoutChanged() => ColumnLayoutChanged?.Invoke(this, EventArgs.Empty);
    public double LayoutRowHeaderWidth => ActualRowHeaderWidth;

    /// <summary>Shared native geometry for aligned extensions such as the summary footer.</summary>
    public DataGridColumnLayout GetColumnLayout(DataGridColumn column, double height)
    {
        if (column.OwningGrid != this) throw new ArgumentException("Column does not belong to this grid.", nameof(column));
        double x, left, right;
        if (FrozenRightColumnCount > 0)
        {
            x = GetTwoSidedColumnX(column);
            (left, right) = GetColumnViewport(column);
        }
        else
        {
            x = column.IsFrozen ? 0 : -HorizontalOffset;
            foreach (var previous in ColumnsInternal.GetVisibleColumns())
            {
                if (previous == column) break;
                if (!column.IsFrozen || previous.IsFrozen) x += previous.ActualWidth;
            }
            left = column.IsFrozen ? 0 : Math.Min(CellsWidth, LeftFrozenWidth);
            right = column.IsFrozen ? Math.Min(CellsWidth, LeftFrozenWidth) : CellsWidth;
        }
        var bounds = new Rect(ActualRowHeaderWidth + x, 0, column.ActualWidth, Math.Max(0, height));
        var viewport = new Rect(ActualRowHeaderWidth + left, 0, Math.Max(0, right - left), Math.Max(0, height));
        return new(bounds, column.IsVisible ? bounds.Intersect(viewport) : default);
    }
}
