# DataGridTreeChangingEventArgs

[English](../en/Avalonia.Collections.DataGridTreeChangingEventArgs.md) · [简体中文](../zh-cn/Avalonia.Collections.DataGridTreeChangingEventArgs.md) · [API index](README.md)

`Avalonia.Collections.DataGridTreeChangingEventArgs` · Class · `System.ComponentModel.CancelEventArgs`

A cancellable change to the visible tree. Item is null for a source refresh.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L23)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Expanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L26) | `bool?` | read-only; CLR / n/a | `expanded` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Item`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L25) | `object?` | read-only; CLR / n/a | `item` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`DataGridTreeChangingEventArgs.DataGridTreeChangingEventArgs(object? item, bool? expanded)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L23) | A cancellable change to the visible tree. Item is null for a source refresh. |
