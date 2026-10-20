# ChartSelectionChangedEventArgs

[English](../en/TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs` · Class · `System.EventArgs`

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartData.cs#L150)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Selection`](../../../TheOne.Avalonia/Controls/ChartData.cs#L152) | `TheOne.Avalonia.Controls.ChartSelection?` | read-only; CLR / n/a | `selection` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ChartSelectionChangedEventArgs.ChartSelectionChangedEventArgs(ChartSelection? selection)`](../../../TheOne.Avalonia/Controls/ChartData.cs#L150) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartSelection`](TheOne.Avalonia.Controls.ChartSelection.md)
