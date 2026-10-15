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
