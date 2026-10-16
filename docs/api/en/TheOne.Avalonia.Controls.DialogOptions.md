# DialogOptions

[English](../en/TheOne.Avalonia.Controls.DialogOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DialogOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DialogOptions` · Class · `object`

Immutable settings for a modal dialog inside OneWindow.

[Source / 声明](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L4)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Buttons`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L14) | `TheOne.Avalonia.Controls.OneDialogButtons` | writable; CLR / init-only | `OneDialogButtons.OkCancel` | Dialog button configuration/flags, interpreted according to the options type and close-result contract. Values: `Ok`, `OkCancel`, `YesNo`, `YesNoCancel` |
| [`CloseOnEscape`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L12) | `bool` | writable; CLR / init-only | `true` | Allows Escape to close a dialog, following its cancellation/result semantics. |
| [`CloseOnOverlayClick`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L11) | `bool` | writable; CLR / init-only | `false` | Allows overlay clicks to close the dialog, following cancellation/result options. |
| [`MotionEnabled`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L7) | `bool?` | writable; CLR / init-only | `null` | Dialog-specific motion switch, also respecting inherited Motion.Enabled. |
| [`ShowButtons`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L13) | `bool` | writable; CLR / init-only | `false` | Shows the default button area; does not control buttons in custom content. |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L10) | `bool` | writable; CLR / init-only | `true` | Shows the close button; the host and associated commands retain control over closing. |
| [`ShowTitle`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L9) | `bool` | writable; CLR / init-only | `true` | Shows the dialog title area while retaining its configured title value. |
| [`Size`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L6) | `TheOne.Avalonia.Styling.ControlSize?` | writable; CLR / init-only | `null` | Dialog size enum using the library's supported size presets. Values: `Small`, `Normal`, `Large` |
| [`Title`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L8) | `string?` | writable; CLR / init-only | `null` | Display title, distinct from business identity or a localization key. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`DialogOptions.DialogOptions()`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L4) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.OneDialogButtons`](TheOne.Avalonia.Controls.OneDialogButtons.md)
- [`TheOne.Avalonia.Styling.ControlSize`](TheOne.Avalonia.Styling.ControlSize.md)
