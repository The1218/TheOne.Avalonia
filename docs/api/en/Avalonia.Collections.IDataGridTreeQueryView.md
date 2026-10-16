# IDataGridTreeQueryView

[English](../en/Avalonia.Collections.IDataGridTreeQueryView.md) · [简体中文](../zh-cn/Avalonia.Collections.IDataGridTreeQueryView.md) · [API index](README.md)

`Avalonia.Collections.IDataGridTreeQueryView` · Interface

Optional query/summary contract, separate from the existing tree presentation API.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L38)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`LoadedItems`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L41) | `System.Collections.IEnumerable` | read-only; CLR / n/a | `accessor-defined / state` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`bool IDataGridTreeQueryView.IsFilterMatch(object item)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L42) | Checks whether an original node directly matches the current filter. |
| [`bool IDataGridTreeQueryView.IsLeaf(object item)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L43) | Checks whether an original node is a leaf with no children. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler? IDataGridTreeQueryView.LoadedItemsChanged`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L40) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
