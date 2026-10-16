# DataGridColumn

[English](../en/Avalonia.Controls.DataGridColumn.md) · [简体中文](../zh-cn/Avalonia.Controls.DataGridColumn.md) · [API index](README.md)

`Avalonia.Controls.DataGridColumn` · Class · `Avalonia.AvaloniaObject`

TheOne additions to the native column; inherited/upstream APIs are omitted.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/DataGridColumn.cs#L28)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`SortPriority`](../../../vendor/Avalonia.Controls.DataGrid/DataGridColumn.cs#L743) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Gets the one-based priority, or zero if this column is not sorted. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void DataGridColumn.CycleSort(KeyModifiers modifiers = KeyModifiers.None)`](../../../vendor/Avalonia.Controls.DataGrid/DataGridColumn.cs#L734) | Cycles unsorted, ascending, descending, then unsorted through the native edit pipeline. |
