// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace Avalonia.Controls;

/// <summary>
///     Defines modes that indicates how DataGrid content is copied to the Clipboard.
/// </summary>
#if !DATAGRID_INTERNAL
public
#endif
    enum DataGridClipboardCopyMode
{
    /// <summary>
    ///     Disable the DataGrid's ability to copy selected items as text.
    /// </summary>
    None,

    /// <summary>
    ///     Enable the DataGrid's ability to copy selected items as text, but do not include
    ///     the column header content as the first line in the text that gets copied to the Clipboard.
    /// </summary>
    ExcludeHeader,

    /// <summary>
    ///     Enable the DataGrid's ability to copy selected items as text, and include
    ///     the column header content as the first line in the text that gets copied to the Clipboard.
    /// </summary>
    IncludeHeader
}

/// <summary>
///     This structure encapsulate the cell information necessary when clipboard content is prepared.
/// </summary>
#if !DATAGRID_INTERNAL
public
#endif
    struct DataGridClipboardCellContent
{
    /// <summary>
    ///     Creates a new DataGridClipboardCellValue structure containing information about a DataGrid cell.
    /// </summary>
    /// <param name="item">DataGrid row item containing the cell.</param>
    /// <param name="column">DataGridColumn containing the cell.</param>
    /// <param name="content">DataGrid cell value.</param>
    public DataGridClipboardCellContent(object item, DataGridColumn column, object content)
    {
        Item = item;
        Column = column;
        Content = content;
    }

    /// <summary>
    ///     DataGridColumn containing the cell.
    /// </summary>
    public DataGridColumn Column { get; }

    /// <summary>
    ///     Cell content.
    /// </summary>
    public object Content { get; }

    /// <summary>
    ///     DataGrid row item containing the cell.
    /// </summary>
    public object Item { get; }

    /// <summary>
    ///     Field-by-field comparison to avoid reflection-based ValueType.Equals.
    /// </summary>
    /// <param name="obj">DataGridClipboardCellContent to compare.</param>
    /// <returns>True iff this and data are equal</returns>
    public override bool Equals(object? obj)
    {
        if (obj is DataGridClipboardCellContent content)
            return Column == content.Column && Content == content.Content && Item == content.Item;

        return false;
    }

    /// <summary>
    ///     Returns a deterministic hash code.
    /// </summary>
    /// <returns>Hash value.</returns>
    public override int GetHashCode()
    {
        return Column.GetHashCode() ^ Content.GetHashCode() ^ Item.GetHashCode();
    }

    /// <summary>
    ///     Field-by-field comparison to avoid reflection-based ValueType.Equals.
    /// </summary>
    /// <param name="clipboardCellContent1">The first DataGridClipboardCellContent.</param>
    /// <param name="clipboardCellContent2">The second DataGridClipboardCellContent.</param>
    /// <returns>True iff clipboardCellContent1 and clipboardCellContent2 are equal.</returns>
    public static bool operator ==(DataGridClipboardCellContent clipboardCellContent1,
        DataGridClipboardCellContent clipboardCellContent2)
    {
        return clipboardCellContent1.Column == clipboardCellContent2.Column &&
               clipboardCellContent1.Content == clipboardCellContent2.Content &&
               clipboardCellContent1.Item == clipboardCellContent2.Item;
    }

    /// <summary>
    ///     Field-by-field comparison to avoid reflection-based ValueType.Equals.
    /// </summary>
    /// <param name="clipboardCellContent1">The first DataGridClipboardCellContent.</param>
    /// <param name="clipboardCellContent2">The second DataGridClipboardCellContent.</param>
    /// <returns>True iff clipboardCellContent1 and clipboardCellContent2 are NOT equal.</returns>
    public static bool operator !=(DataGridClipboardCellContent clipboardCellContent1,
        DataGridClipboardCellContent clipboardCellContent2)
    {
        if (clipboardCellContent1.Column == clipboardCellContent2.Column &&
            clipboardCellContent1.Content == clipboardCellContent2.Content)
            return clipboardCellContent1.Item != clipboardCellContent2.Item;
        return true;
    }
}

/// <summary>
///     This class encapsulates a selected row's information necessary for the CopyingRowClipboardContent event.
/// </summary>
#if !DATAGRID_INTERNAL
public
#endif
    class DataGridRowClipboardEventArgs : EventArgs
{
    private List<DataGridClipboardCellContent>? _clipboardRowContent;

    /// <summary>
    ///     Creates a DataGridRowClipboardEventArgs object and initializes the properties.
    /// </summary>
    /// <param name="item">The row's associated data item.</param>
    /// <param name="isColumnHeadersRow">Whether or not this EventArgs is for the column headers.</param>
    internal DataGridRowClipboardEventArgs(object item, bool isColumnHeadersRow)
    {
        IsColumnHeadersRow = isColumnHeadersRow;
        Item = item;
    }

    /// <summary>
    ///     This list should be used to modify, add ot remove a cell content before it gets stored into the clipboard.
    /// </summary>
    public List<DataGridClipboardCellContent> ClipboardRowContent
    {
        get
        {
            if (_clipboardRowContent == null) _clipboardRowContent = new List<DataGridClipboardCellContent>();
            return _clipboardRowContent;
        }
    }

    /// <summary>
    ///     This property is true when the ClipboardRowContent represents column headers, in which case the Item is null.
    /// </summary>
    public bool IsColumnHeadersRow { get; }

    /// <summary>
    ///     DataGrid row item used for preparing the ClipboardRowContent.
    /// </summary>
    public object Item { get; }
}