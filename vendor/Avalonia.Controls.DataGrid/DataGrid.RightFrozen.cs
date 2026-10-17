using System;
using System.Linq;
using Avalonia.Media;

namespace Avalonia.Controls;

// TheOne extension: all geometry stays in the native layout/scrolling pipeline.
public partial class DataGrid
{
    public static readonly AttachedProperty<int> FrozenRightColumnCountProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, DataGrid, int>(nameof(FrozenRightColumnCount),
            validate: value => value >= 0, coerce: (owner, value) => ((DataGrid)owner).ValidateRightFrozenCount(value));

    private Thickness? _rightFrozenScrollBarMargin;

    public int FrozenRightColumnCount
    {
        get => GetValue(FrozenRightColumnCountProperty);
        set => SetValue(FrozenRightColumnCountProperty, value);
    }

    internal double LeftFrozenWidth => ColumnsInternal.GetVisibleColumns().Where(c => c.IsFrozen && !c.IsFrozenRight)
        .Sum(c => c.ActualWidth);

    internal double RightFrozenWidth =>
        ColumnsInternal.GetVisibleColumns().Where(c => c.IsFrozenRight).Sum(c => c.ActualWidth);

    internal double MiddleViewportLeft => Math.Min(CellsWidth, LeftFrozenWidth);
    internal double MiddleViewportRight => Math.Max(MiddleViewportLeft, CellsWidth - RightFrozenWidth);

    private void UpdateRightFrozenScrollBarMargin()
    {
        if (FrozenRightColumnCount > 0)
        {
            _rightFrozenScrollBarMargin ??= HorizontalScrollBar.Margin;
            var original = _rightFrozenScrollBarMargin.Value;
            HorizontalScrollBar.SetCurrentValue(MarginProperty,
                new Thickness(original.Left, original.Top, original.Right + RightFrozenWidth, original.Bottom));
        }
        else if (_rightFrozenScrollBarMargin is { } original)
        {
            HorizontalScrollBar.SetCurrentValue(MarginProperty, original);
            _rightFrozenScrollBarMargin = null;
        }
    }

    private int ValidateRightFrozenCount(int value)
    {
        // XAML can set counts before columns exist. Later collection/visibility
        // changes temporarily clamp effective regions without overwriting bindings.
        if (value > 0 && Columns.Count > 0 && value + (long)FrozenColumnCount > Columns.Count(c => c.IsVisible))
            throw new ArgumentOutOfRangeException(nameof(value),
                "Left and right frozen regions cannot overlap or exceed visible columns.");
        return value;
    }

    private int ValidateLeftFrozenCount(int value)
    {
        if (FrozenRightColumnCount > 0 && Columns.Count > 0 &&
            value + (long)FrozenRightColumnCount > Columns.Count(c => c.IsVisible))
            throw new ArgumentOutOfRangeException(nameof(value), "Left and right frozen regions cannot overlap.");
        return value;
    }

    internal (double Left, double Right) GetColumnViewport(DataGridColumn column)
    {
        if (column.IsFrozenRight) return (MiddleViewportRight, CellsWidth);
        if (column.IsFrozen) return (0, MiddleViewportLeft);
        return (MiddleViewportLeft, MiddleViewportRight);
    }

    internal double GetTwoSidedColumnX(DataGridColumn column)
    {
        var x = column.IsFrozenRight ? CellsWidth - RightFrozenWidth :
            column.IsFrozen ? 0 : LeftFrozenWidth - HorizontalOffset;
        foreach (var previous in ColumnsInternal.GetVisibleColumns())
        {
            if (previous == column) break;
            if (previous.IsFrozenRight == column.IsFrozenRight && previous.IsFrozen == column.IsFrozen)
                x += previous.ActualWidth;
        }

        return x;
    }

    internal RectangleGeometry GetTwoSidedClip(DataGridColumn column, double x, double width, double height)
    {
        var region = GetColumnViewport(column);
        var left = Math.Clamp(region.Left - x, 0, width);
        var right = Math.Clamp(region.Right - x, 0, width);
        return new RectangleGeometry(new Rect(left, 0, Math.Max(0, right - left), height));
    }

    internal bool IsTwoSidedColumnVisible(DataGridColumn column)
    {
        if (!column.IsVisible) return false;
        var region = GetColumnViewport(column);
        var x = GetTwoSidedColumnX(column);
        return region.Right > region.Left && x < region.Right && x + column.ActualWidth > region.Left;
    }

    private void CorrectTwoSidedFrozenStates()
    {
        var oldLeftWidth = LeftFrozenWidth;
        var index = 0;
        foreach (var column in ColumnsInternal.GetDisplayedColumns())
        {
            column.IsFrozenRight = false;
            column.IsFrozen = index++ < FrozenColumnCountWithFiller;
        }

        var available = ColumnsInternal.GetVisibleColumns().Where(c => !c.IsFrozen && c != ColumnsInternal.FillerColumn)
            .ToArray();
        var count = Math.Min(FrozenRightColumnCount, available.Length);
        foreach (var column in available.Skip(available.Length - count))
        {
            column.IsFrozenRight = true;
            column.IsFrozen = true;
        }

        UpdateHorizontalOffset(Math.Max(0, HorizontalOffset + oldLeftWidth - LeftFrozenWidth));
        UpdateDisplayedColumns();
        InvalidateRowsMeasure(true);
        InvalidateColumnHeadersMeasure();
    }

    private bool ComputeTwoSidedDisplayedColumns()
    {
        var oldFirst = DisplayData.FirstDisplayedScrollingCol;
        DisplayData.FirstDisplayedScrollingCol = -1;
        DisplayData.LastTotallyDisplayedScrollingCol = -1;
        FirstDisplayedScrollingColumnHiddenWidth = 0;
        var available = MiddleViewportRight - MiddleViewportLeft;
        if (available <= 0) return oldFirst != -1;
        var x = -HorizontalOffset;
        foreach (var column in ColumnsInternal.GetVisibleScrollingColumns())
        {
            var right = x + column.ActualWidth;
            if (right > 0 && x < available)
            {
                if (DisplayData.FirstDisplayedScrollingCol == -1)
                {
                    DisplayData.FirstDisplayedScrollingCol = column.Index;
                    FirstDisplayedScrollingColumnHiddenWidth = Math.Max(0, -x);
                }

                if (x >= 0 && right <= available + 0.001) DisplayData.LastTotallyDisplayedScrollingCol = column.Index;
            }

            x = right;
        }

        return oldFirst != DisplayData.FirstDisplayedScrollingCol;
    }

    private bool ScrollTwoSidedColumnIntoView(int columnIndex)
    {
        var column = ColumnsItemsInternal[columnIndex];
        if (column.IsFrozen || !column.IsVisible) return true;
        var available = MiddleViewportRight - MiddleViewportLeft;
        if (available <= 0) return true;
        var x = GetTwoSidedColumnX(column) - MiddleViewportLeft;
        var offset = HorizontalOffset;
        if (x < 0 || column.ActualWidth > available) offset += x;
        else if (x + column.ActualWidth > available) offset += x + column.ActualWidth - available;
        UpdateHorizontalOffset(Math.Max(0, offset));
        return true;
    }

    private void ScrollTwoSidedColumns(int columns)
    {
        var scrolling = ColumnsInternal.GetVisibleScrollingColumns().ToArray();
        var current = Array.FindIndex(scrolling, c => c.Index == DisplayData.FirstDisplayedScrollingCol);
        if (current < 0) return;
        var target =
            Math.Clamp(current + columns + (columns < 0 && FirstDisplayedScrollingColumnHiddenWidth > 0 ? 1 : 0), 0,
                scrolling.Length - 1);
        UpdateHorizontalOffset(scrolling.Take(target).Sum(c => c.ActualWidth));
    }

    internal bool CanReorderWithinFrozenRegion(DataGridColumn column, int displayIndex)
    {
        var target =
            ColumnsInternal.GetColumnAtDisplayIndex(displayIndex +
                                                    (ColumnsInternal.RowGroupSpacerColumn.IsRepresented ? 1 : 0));
        return target != null && target.IsFrozen == column.IsFrozen && target.IsFrozenRight == column.IsFrozenRight;
    }
}