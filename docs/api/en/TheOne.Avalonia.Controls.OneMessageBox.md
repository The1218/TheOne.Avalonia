# OneMessageBox

[English](../en/TheOne.Avalonia.Controls.OneMessageBox.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneMessageBox.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneMessageBox` · Class · `object`

Simple, command-driven confirmation using the existing themed window dialog host.

[Source / 声明](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L19)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static Task<bool> OneMessageBox.ConfirmAsync(string title, string message, MessageBoxOptions? options = null, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L25) | Returns true only after explicit confirmation. Cancel, Escape and owner closure return false. Cancellation of cancels the returned task. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.MessageBoxOptions`](TheOne.Avalonia.Controls.MessageBoxOptions.md)
