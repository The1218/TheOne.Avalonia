# NumericFilterCondition

[English](../en/TheOne.Avalonia.Controls.NumericFilterCondition_TNumber_.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NumericFilterCondition_TNumber_.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NumericFilterCondition<TNumber>` · Class · `TheOne.Avalonia.Controls.DataGridFilterCondition`

[Inherited additional members: DataGridFilterCondition](TheOne.Avalonia.Controls.DataGridFilterCondition.md)

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L81)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsEmpty`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L103) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Maximum`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L101) | `TNumber?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Minimum`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L100) | `TNumber?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Mode`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L98) | `TheOne.Avalonia.Controls.NumericFilterMode` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. Values: `Exact`, `Range` |
| [`Value`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L99) | `TNumber?` | read-only; CLR / n/a | `accessor-defined / state` | Current value; units, range and null semantics are defined by its model and validation constraints. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override bool NumericFilterCondition<TNumber>.Matches(object? value)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L106) | Checks a value against this filter; null/comparison semantics follow the condition type. |
| [`NumericFilterCondition<TNumber>.NumericFilterCondition(NumericFilterMode mode, TNumber? value = null, TNumber? minimum = null, TNumber? maximum = null)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L83) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.NumericFilterMode`](TheOne.Avalonia.Controls.NumericFilterMode.md)
