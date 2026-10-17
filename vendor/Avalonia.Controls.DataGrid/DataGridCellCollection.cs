// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Avalonia.Controls;

internal class DataGridCellCollection
{
    private readonly List<DataGridCell> _cells;
    private readonly DataGridRow _owningRow;

    public DataGridCellCollection(DataGridRow owningRow)
    {
        _owningRow = owningRow;
        _cells = new List<DataGridCell>();
    }

    public int Count => _cells.Count;

    public DataGridCell this[int index]
    {
        get
        {
            if (index < 0 || index >= _cells.Count)
                throw DataGridError.DataGrid.ValueMustBeBetween("index", "Index", 0, true, _cells.Count, false);
            return _cells[index];
        }
    }

    internal event EventHandler<DataGridCellEventArgs>? CellAdded;
    internal event EventHandler<DataGridCellEventArgs>? CellRemoved;

    public IEnumerator GetEnumerator()
    {
        return _cells.GetEnumerator();
    }

    public void Insert(int cellIndex, DataGridCell cell)
    {
        Debug.Assert(cellIndex >= 0 && cellIndex <= _cells.Count);

        cell.OwningRow = _owningRow;
        _cells.Insert(cellIndex, cell);

        CellAdded?.Invoke(this, new DataGridCellEventArgs(cell));
    }

    public void RemoveAt(int cellIndex)
    {
        var dataGridCell = _cells[cellIndex];
        _cells.RemoveAt(cellIndex);
        dataGridCell.OwningRow = null;
        CellRemoved?.Invoke(this, new DataGridCellEventArgs(dataGridCell));
    }
}