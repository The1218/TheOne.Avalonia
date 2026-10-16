# MessageBoxOptions

[English](../en/TheOne.Avalonia.Controls.MessageBoxOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.MessageBoxOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.MessageBoxOptions` · Class · `object`

Presentation options for a simple confirm/cancel message box.

[Source / 声明](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CancelText`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L11) | `string` | writable; CLR / init-only | `"取消"` | Localized cancel-button text without changing its result value. |
| [`ConfirmText`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L10) | `string` | writable; CLR / init-only | `"确认"` | Localized confirmation text; it does not execute host business logic itself. |
| [`IsDangerous`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L12) | `bool` | writable; CLR / init-only | `false` | Applies danger semantics to confirmation; the business layer owns authorization/execution. |
| [`Size`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L14) | `TheOne.Avalonia.Styling.ControlSize?` | writable; CLR / init-only | `null` | Overrides the owner window's inherited density when specified. Values: `Small`, `Normal`, `Large` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`MessageBoxOptions.MessageBoxOptions()`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L8) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Styling.ControlSize`](TheOne.Avalonia.Styling.ControlSize.md)
