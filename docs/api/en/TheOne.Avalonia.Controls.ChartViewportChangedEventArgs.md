# ChartViewportChangedEventArgs

[English](../en/TheOne.Avalonia.Controls.ChartViewportChangedEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartViewportChangedEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartViewportChangedEventArgs` · Class · `System.EventArgs`

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L16)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`NewViewport`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L19) | `Avalonia.Rect` | read-only; CLR / n/a | `newViewport` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`OldViewport`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L18) | `Avalonia.Rect` | read-only; CLR / n/a | `oldViewport` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ChartViewportChangedEventArgs.ChartViewportChangedEventArgs(Rect oldViewport, Rect newViewport)`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L16) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
