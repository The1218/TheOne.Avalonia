# DataGridFilterChangingEventArgs

[English](../en/TheOne.Avalonia.Controls.DataGridFilterChangingEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridFilterChangingEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridFilterChangingEventArgs` · Class · `Avalonia.Interactivity.CancelRoutedEventArgs`

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Column`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L16) | `Avalonia.Controls.DataGridColumn` | read-only; CLR / n/a | `column` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`NewCondition`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L18) | `TheOne.Avalonia.Controls.DataGridFilterCondition?` | read-only; CLR / n/a | `newCondition` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`OldCondition`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L17) | `TheOne.Avalonia.Controls.DataGridFilterCondition?` | read-only; CLR / n/a | `oldCondition` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`DataGridFilterChangingEventArgs.DataGridFilterChangingEventArgs(DataGridColumn column, DataGridFilterCondition? oldCondition, DataGridFilterCondition? newCondition)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L10) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.DataGridFilterCondition`](TheOne.Avalonia.Controls.DataGridFilterCondition.md)
