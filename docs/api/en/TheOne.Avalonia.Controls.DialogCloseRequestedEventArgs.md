# DialogCloseRequestedEventArgs

[English](../en/TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs` · Class · `System.EventArgs`

[Source / 声明](../../../TheOne.Avalonia/Controls/DialogContext.cs#L6)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Result`](../../../TheOne.Avalonia/Controls/DialogContext.cs#L8) | `TheOne.Avalonia.Controls.OneDialogResult` | read-only; CLR / n/a | `result` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. Values: `Ok`, `Cancel`, `Yes`, `No`, `Closed` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`DialogCloseRequestedEventArgs.DialogCloseRequestedEventArgs(OneDialogResult result)`](../../../TheOne.Avalonia/Controls/DialogContext.cs#L6) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.OneDialogResult`](TheOne.Avalonia.Controls.OneDialogResult.md)
