# DialogContext

[English](../en/TheOne.Avalonia.Controls.DialogContext.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DialogContext.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DialogContext` · Class · `object`

Optional base for dialog view models. Does not depend on a window or dispatcher.

[Source / 声明](../../../TheOne.Avalonia/Controls/DialogContext.cs#L12)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void DialogContext.RequestClose(OneDialogResult result = OneDialogResult.Ok)`](../../../TheOne.Avalonia/Controls/DialogContext.cs#L20) | Requests completion of this dialog, including when it is still queued. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event PropertyChangedEventHandler? DialogContext.PropertyChanged`](../../../TheOne.Avalonia/Controls/DialogContext.cs#L16) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`event EventHandler<DialogCloseRequestedEventArgs>? DialogContext.CloseRequested`](../../../TheOne.Avalonia/Controls/DialogContext.cs#L17) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs`](TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs.md)
- [`TheOne.Avalonia.Controls.OneDialogResult`](TheOne.Avalonia.Controls.OneDialogResult.md)
