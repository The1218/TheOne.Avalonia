# PaginationPageSize

[English](../en/TheOne.Avalonia.Controls.PaginationPageSize.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.PaginationPageSize.md) · [API index](README.md)

`TheOne.Avalonia.Controls.PaginationPageSize` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L16)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Text`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L16) | `string` | writable; CLR / init-only | `required constructor argument` | Input/display text; notifications and validation follow the model/native editor contract. |
| [`Value`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L16) | `int` | writable; CLR / init-only | `required constructor argument` | Current value; units, range and null semantics are defined by its model and validation constraints. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`PaginationPageSize.PaginationPageSize(int Value, string Text)`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L16) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
