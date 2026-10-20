# IDataGridValueAccessor

[English](../en/TheOne.Avalonia.Controls.IDataGridValueAccessor.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IDataGridValueAccessor.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IDataGridValueAccessor` · Interface

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L20)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ValueType`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L22) | `System.Type` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`object? IDataGridValueAccessor.GetValue(object item)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L23) | Reads an original-row value through an explicit field key/typed getter, without reflection paths. |
