# TextFilterCondition

[English](../en/TheOne.Avalonia.Controls.TextFilterCondition.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TextFilterCondition.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TextFilterCondition` · Class · `TheOne.Avalonia.Controls.DataGridFilterCondition`

[Inherited additional members: DataGridFilterCondition](TheOne.Avalonia.Controls.DataGridFilterCondition.md)

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L57)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Comparison`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L67) | `System.StringComparison` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. Values: `CurrentCulture`, `CurrentCultureIgnoreCase`, `InvariantCulture`, `InvariantCultureIgnoreCase`, `Ordinal`, `OrdinalIgnoreCase` |
| [`IsEmpty`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L68) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Text`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L66) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Input/display text; notifications and validation follow the model/native editor contract. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override bool TextFilterCondition.Matches(object? value)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L70) | Checks a value against this filter; null/comparison semantics follow the condition type. |
| [`TextFilterCondition.TextFilterCondition(string? text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L59) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
