# MessageBox

`OneMessageBox` is a simple confirm/cancel prompt. It reuses the dialog layer installed by `OneWindow` and `LoginWindow`; it does not create another native window. Use `OneDialog` for forms or custom content.

```csharp
// Invoke from a ViewModel's asynchronous ICommand.
var confirmed = await OneMessageBox.ConfirmAsync(
    "Delete this file?",
    "The file and its preview will be permanently deleted.",
    new MessageBoxOptions
    {
        ConfirmText = "Delete file",
        CancelText = "Keep file",
        IsDangerous = true
    });
if (confirmed)
    await fileService.DeleteAsync(fileId);
```

Only the confirm command returns `true`. The cancel command, Escape, or owner closure returns `false`. The cancel button receives initial focus, including for dangerous operations; Tab changes focus and Enter activates the focused button. Clicking the overlay keeps the prompt open. Closing restores the previously focused control through the existing dialog host.

`MessageBoxOptions` provides `ConfirmText` (default `确认`), `CancelText` (default `取消`), `IsDangerous` (default `false`), and an optional `ControlSize? Size`. Omit `Size` to inherit the window's Small / Normal / Large density. Text and actions use `One.*` resources for Light and Dark themes. Title, message and action labels must be non-empty.

The optional `owner` parameter selects an open `OneWindow` or `LoginWindow`. Without it, the registered default owner is used, matching `OneDialog`. Requests share the existing per-window queue. `cancellationToken` cancellation cancels the returned task and releases the dialog; it does not return a user decision.

The PreviewHost MessageBox page binds its launch buttons to ViewModel commands and shows both a configurable confirmation and a dangerous confirmation. It demonstrates awaiting the boolean result before applying business work, with compiled bindings throughout.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost labels example Classes, sizes and states directly on each control page. Description components explain public properties, APIs/events and model fields, including property types, defaults, binding modes, validation and API signatures. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### DialogContext

[API / model reference](../../api/en/TheOne.Avalonia.Controls.DialogContext.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### DialogOptions

[API / model reference](../../api/en/TheOne.Avalonia.Controls.DialogOptions.md)

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

### OneDialog

[API / model reference](../../api/en/TheOne.Avalonia.Controls.OneDialog.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L123) | `Avalonia.Controls.Window?` | writable; CLR / n/a | `accessor-defined / state` | The native relationship established by `child.Show(parent)` / `child.ShowDialog<T>(parent)`, affecting platform window lifetime. |

### Shared attached properties

[ControlSizing](../../api/en/TheOne.Avalonia.Styling.ControlSizing.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | writable; OneWay; attached; inherits | `ControlSize.Normal` | Shared Small/Normal/Large density inherited by descendants; explicit local values/classes can override it. Values: `Small`, `Normal`, `Large` |

[Motion](../../api/en/TheOne.Avalonia.Styling.Motion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | writable; OneWay; attached; inherits | `true` | Inherited motion switch; disabling goes to final layout/state while preserving native input. |

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs`](../../api/en/TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DialogContext`](../../api/en/TheOne.Avalonia.Controls.DialogContext.md) | Class | 0 | 1 | 2 |
| [`TheOne.Avalonia.Controls.DialogOptions`](../../api/en/TheOne.Avalonia.Controls.DialogOptions.md) | Class | 9 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneDialog`](../../api/en/TheOne.Avalonia.Controls.OneDialog.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogButtons`](../../api/en/TheOne.Avalonia.Controls.OneDialogButtons.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogResult`](../../api/en/TheOne.Avalonia.Controls.OneDialogResult.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Styling.ControlSize`](../../api/en/TheOne.Avalonia.Styling.ControlSize.md) | Enum | 3 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
