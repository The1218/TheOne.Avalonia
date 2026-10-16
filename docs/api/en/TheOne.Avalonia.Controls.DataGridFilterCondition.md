# DataGridFilterCondition

[English](../en/TheOne.Avalonia.Controls.DataGridFilterCondition.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridFilterCondition.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridFilterCondition` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L29)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsEmpty`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L31) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`abstract bool DataGridFilterCondition.Matches(object? value)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L32) | Checks a value against this filter; null/comparison semantics follow the condition type. |
