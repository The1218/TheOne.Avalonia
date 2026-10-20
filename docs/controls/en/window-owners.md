# Window owners, Toast, Notification, and Dialog

[简体中文](../zh-cn/window-owners.md) · **English**

`OneWindow` and `LoginWindow` each contain independent Toast, Notification, and Dialog hosts. Messages appear inside their target window; Notification here means an in-app notification. Load `OneTheme`, show the window, then call a service. See [OneWindow](windows.md) for templates, themes, and basic usage, and [LoginWindow](login-window.md) for login handoff.

## Four kinds of ownership

| Concept | Purpose |
| --- | --- |
| Native `Window.Owner` | The native relationship established by `child.Show(parent)` / `child.ShowDialog<T>(parent)`, affecting platform window lifetime. |
| `desktop.MainWindow` | The desktop application's main window, used by the `ShutdownMode` exit policy. |
| `OneToast.Owner`, `OneNotification.Owner`, `OneDialog.Owner` | Each service's independent default message target. |
| The `owner:` call argument | Targets this display or cleanup call only; does not change defaults or establish native ownership. |

A detail window owned by the main window can still receive its own messages:

```csharp
using TheOne.Avalonia.Controls;

var detail = new OneWindow { Title = "Details" };
detail.Show(mainWindow); // Its native Owner is mainWindow.
OneToast.Show("Details loaded", owner: detail);
await OneDialog.ShowAsync("Confirm", "Save changes in the main window?", owner: mainWindow);
```

Changing `desktop.MainWindow`, DataContext, or native Owner alone does not add message hosts to an ordinary `Window`. Applying OneTheme to an ordinary Window does not make it a supported service target.

## How owner resolution works

Each call resolves its target in this order:

1. A non-null explicit `owner` must be a registered, open, visible `OneWindow` or `LoginWindow`; otherwise the call throws `InvalidOperationException` without redirecting to another window.
2. An omitted or null owner uses the service's available default `Owner`.
3. If that default is hidden, closed, not yet opened, or no longer referenced, resolution falls back to the most recently opened / activated visible themed window.
4. With no available window, the call throws `InvalidOperationException`.

Availability checks registration, `IsVisible`, and the platform window instance; there is no separate minimized-state exclusion. Do not equate minimizing with `Hide()`, or assume that a default Owner guarantees a foreground window.

All three defaults are weak references and can be assigned independently on the UI thread. Assigning an ordinary `Window` throws `ArgumentException`. Assigning an unopened themed window is allowed, but delivery still checks its availability.

```csharp
OneToast.Owner = mainWindow;
OneNotification.Owner = mainWindow;
OneDialog.Owner = mainWindow;
```

Opening or activating any `OneWindow` / `LoginWindow` automatically updates all three defaults to that window. Manual assignment does not lock routing permanently; use explicit owners for multi-window business operations. On close, only defaults still pointing at the closing window are replaced with a visible fallback, or null if none exists.

Reading `Owner` returns the stored weak-reference target without resolving a fallback. Setting it to null clears that default; it does not disable the service, and later calls may still fall back. `OneWindow.Current` returns the most recent visible `OneWindow`, excluding `LoginWindow`; it does not identify the initiating window of a background operation.

## Multiple windows and background work

Capture the target when the user starts an operation and pass it explicitly after awaiting work, so activating another window does not redirect its result:

```csharp
// In an event handler on the initiating window.
var messageOwner = this; // this is OneWindow or LoginWindow.
await SaveAsync(cancellationToken);
await OneToast.ShowAsync("Saved", owner: messageOwner);
await OneNotification.ShowAsync("Save complete", "Data updated.", owner: messageOwner);
var result = await OneDialog.ShowAsync(
    "Continue", "View the result?", OneDialogButtons.YesNo, owner: messageOwner,
    cancellationToken: cancellationToken);
```

Cancel associated background work when its window closes. If an explicit owner has closed before completion, display fails; do not rely on default routing to deliver login errors or business results elsewhere. Async methods resolve owners when their UI-thread work actually executes.

## Toast and Notification

| Service | Default position / visible limit | Content and default duration | Meaning of `ShowAsync` completion |
| --- | --- | --- | --- |
| `OneToast` | Top center / 3 | Body, no title or close button; 2.5 seconds | The display call ran; does not wait for dismissal. |
| `OneNotification` | Top right / 4 | Title, body, close button; 4 seconds | The display call ran; does not wait for acknowledgement. |
| `OneDialog` | Window modal layer / one active request per window | Confirmation or custom content; no automatic expiration | The request ended, returning a result or canceling the task. |

Positions and limits come from the default window templates and can be customized there. Hosts are independent per window. The `INotification` overload accepts type, expiration, and click / close callbacks; Toast does not display its Title. Use Dialog for a user decision rather than awaiting a Toast / Notification task as confirmation.

```csharp
using Avalonia.Controls.Notifications;

OneToast.Show("Processing", NotificationType.Information,
    expiration: TimeSpan.Zero, owner: mainWindow);
OneNotification.Show("Attention needed", "Review the import result.", NotificationType.Warning,
    expiration: TimeSpan.Zero, owner: mainWindow);

OneToast.CloseAll(owner: mainWindow);
OneNotification.CloseAll(owner: mainWindow);
```

`TimeSpan.Zero` disables automatic expiration. Click the Toast body or the Notification close button to dismiss, or call the corresponding `CloseAll`. That method clears only its own message category in one resolved window; it does not clear Dialog, change defaults, or visit every window. Closing a window does not migrate existing messages to its fallback.

## UI-thread rules

| Operation | Thread requirement |
| --- | --- |
| Read / assign service `Owner`; read `OneWindow.Current` | UI thread. |
| Synchronous Toast / Notification `Show`, `CloseAll` | UI thread. |
| String-based Toast / Notification `ShowAsync` | Dispatches to the UI thread internally. |
| Dialog `ShowAsync`, generic overload, `ShowContentAsync` | Dispatches to the UI thread internally. |

There is no `CloseAllAsync`. From background work, use `await Dispatcher.UIThread.InvokeAsync(() => OneToast.CloseAll(owner: messageOwner));` with `Avalonia.Threading`. Capture the initiating window on the UI thread before starting background work; do not read static Owner / Current there. Do not block the UI thread with `.Wait()` or `.Result` while awaiting dialogs.

## Dialog queues, results, and dismissal

A Dialog is a modal overlay inside its owner, without a new native Window. Use native `ShowDialog<T>(owner)` when a separate native child window is required. Each window has its own queue and displays one request at a time; different windows can display independently. The owner is resolved before enqueueing; activation and default changes do not move requests. `IsDialogOpen` indicates an active dialog in that window.

| Ending | Outcome |
| --- | --- |
| OK / Cancel / Yes / No button | `Ok` / `Cancel` / `Yes` / `No`. |
| Escape, enabled by default | `Cancel`. |
| Title-bar close button, shown by default | `Closed`. |
| Overlay click, disabled by default | Returns `Closed` when `CloseOnOverlayClick` is enabled. |
| Owner actually closes | Active and queued requests return `Closed`. |
| CancellationToken cancellation | Task is canceled; awaiting throws `OperationCanceledException`, rather than returning the ordinary `Cancel` result. |

`Hide()` does not close a window or automatically finish its dialogs. A vetoed native Closing event also preserves requests. Use a CancellationToken when the caller needs to stop waiting:

```csharp
try
{
    var result = await OneDialog.ShowAsync("Delete", "Delete this item?",
        OneDialogButtons.OkCancel, owner: mainWindow,
        cancellationToken: cancellationToken);
    if (result == OneDialogResult.Ok)
        await DeleteAsync();
}
catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
{
    // Business cancellation is distinct from the user clicking Cancel.
}
```

`DialogOptions` controls the title, close button, confirmation buttons, Escape, overlay dismissal, size, and motion. Defaults include `CloseOnEscape=true` and `CloseOnOverlayClick=false`; generic dialogs default to `ShowButtons=false`. See [generic ViewModel dialogs](windows.md#generic-viewmodel-dialogs) for custom forms and DialogContext commands. A supplied Control must have no existing parent; it is detached when the request ends and can be reused sequentially. The modal layer disables background interaction, cycles keyboard focus, and restores prior focus where possible.

## LoginWindow errors and main-window handoff

LoginWindow directly inherits native Window, has independent message hosts, and is excluded from `OneWindow.Current`. Error helpers explicitly target that login instance even when another window is active:

```csharp
login.ShowErrorToast("Invalid credentials");
login.ShowErrorNotification("Authentication service unavailable", title: "Sign-in failed");
await login.ShowErrorDialog("Please sign in again", title: "Session expired",
    cancellationToken: cancellationToken);
```

The first two synchronous helpers require the UI thread; `ShowErrorDialog` dispatches internally. A hidden or closed login window remains an invalid owner. Cancel authentication work on close so late results do not operate on the old window.

| Handoff phase | Window and service behavior |
| --- | --- |
| Login window opens / activates | All three default Owners become the login window. |
| Normal successful `CompleteLogin` / `CompleteLoginAsync` | Replaces `desktop.MainWindow`, shows an independent OneWindow, closes login, and directs defaults to the new main window while it remains visible. |
| Native Closing veto | Returns false; login hosts remain, main window is not shown. |
| Main-window show failure | Throws, restoring the main window and previous service defaults. |
| Login actually closes | Its active / queued Dialog requests return Closed; existing messages and business Content / DataContext are not transferred. |

```csharp
var mainWindow = new MainWindow(); // OneWindow; never shown and no native Owner.
if (await login.CompleteLoginAsync(mainWindow))
    await OneToast.ShowAsync("Welcome back", owner: mainWindow);
```

This assumes close callbacks do not also close the new main window. If business callbacks close it immediately, end that workflow without sending it another message. Handoff temporarily uses explicit shutdown, then restores the previous ShutdownMode. Setting service Owners and closing login alone does not replace handoff. Do not first call `mainWindow.Show(login)` or `ShowDialog(login)`; the new main window must be independent of the login window that will close. See [LoginWindow](login-window.md) for prerequisites, factory overloads, and failure handling.

## Troubleshooting

| Symptom | Check |
| --- | --- |
| A message appears in another window | Activation updates defaults; capture and pass the initiating owner. |
| Setting Owner still produces a lifecycle error | Check Show, visibility, closure, and supported window type. |
| Messages still display with Owner=null | Null permits fallback; it is not a disable switch. |
| Login exists but Current is null | Current includes only OneWindow; use the login instance or service defaults. |
| Execution continues immediately after awaiting Toast | ShowAsync waits for the display call only; use Dialog for a decision. |
| Custom template loses messages or dialogs | Retain `PART_VisualLayerManager`, `PART_ContentPresenter`, `PART_Notifications`, `PART_Toasts`, `PART_DialogShield`, `PART_DialogLayer`, and the corresponding title-bar PART_*. |

Implementation references: [services and owner resolution](../../../TheOne.Avalonia/Controls/OneMessages.cs), [Dialog lifecycle](../../../TheOne.Avalonia/Controls/DialogCoordinator.cs), [login handoff](../../../TheOne.Avalonia/Controls/LoginWindow.cs).

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### DialogContext

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.DialogContext.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### LoginWindow

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L35) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### OneDialog

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.OneDialog.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L217) | `Avalonia.Controls.Window?` | writable; CLR / n/a | `accessor-defined / state` | The native relationship established by `child.Show(parent)` / `child.ShowDialog<T>(parent)`, affecting platform window lifetime. |

### OneNotification

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.OneNotification.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L120) | `Avalonia.Controls.Window?` | writable; CLR / n/a | `accessor-defined / state` | The native relationship established by `child.Show(parent)` / `child.ShowDialog<T>(parent)`, affecting platform window lifetime. |

### OneToast

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.OneToast.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L159) | `Avalonia.Controls.Window?` | writable; CLR / n/a | `accessor-defined / state` | The native relationship established by `child.Show(parent)` / `child.ShowDialog<T>(parent)`, affecting platform window lifetime. |

### OneWindow

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.OneWindow.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Current`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L123) | `TheOne.Avalonia.Controls.OneWindow?` | read-only; CLR / n/a | `accessor-defined / state` | UI thread. |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L122) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`LeftContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L67) | `object?` | writable; OneWay | `null` | Additional content on the left side of the library window title bar. |
| [`LeftContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L79) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`RightContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L73) | `object?` | writable; OneWay | `null` | Additional right title-bar content; avoid covering window-operation hit areas. |
| [`RightContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L85) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`ShowThemeButton`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L91) | `bool` | writable; OneWay | `true` | Shows the built-in brightness toggle; palette resources remain controlled by ThemeColorPicker/OneTheme. |
| [`ShowTitleBar`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L110) | `bool` | writable; OneWay | `true` | Shows the library title bar without changing window lifetime or ownership. |
| [`ThemeButtonIsThreeState`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L104) | `bool` | writable; OneWay | `false` | Includes Default in the built-in theme button's Light/Dark cycle. |
| [`ThemeButtonScope`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L97) | `TheOne.Avalonia.Controls.ThemeButtonScope` | writable; OneWay | `0 / default` | Theme-toggle scope for the built-in button, distinguishing application and window via enum. Values: `Window`, `Application` |
| [`TitleBarHeight`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L116) | `double` | writable; OneWay | `48` | Title-bar height in DIPs; retain sufficient title and window-button hit areas. Validation: `value => double.IsFinite(value) && value >= 0` |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`DialogContext`](../../api/en/TheOne.Avalonia.Controls.DialogContext.md) | `void DialogContext.RequestClose(OneDialogResult result = OneDialogResult.Ok)` | Requests completion of this dialog, including when it is still queued. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `void LoginWindow.ShowErrorToast(string message, TimeSpan? expiration = null)` | Shows an error toast in this login window on the UI thread. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `void LoginWindow.ShowErrorNotification(string message, string title = "登录失败", TimeSpan? expiration = null)` | Shows an error notification in this login window on the UI thread. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `Task<OneDialogResult> LoginWindow.ShowErrorDialog(string message, string title = "登录失败", CancellationToken cancellationToken = default(CancellationToken))` | Shows an error dialog in this login window; dispatches to the UI thread. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `bool LoginWindow.CompleteLogin(OneWindow mainWindow)` | Installs the supplied fresh, unopened OneWindow as the desktop MainWindow and all message owners, then closes login. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `Task<bool> LoginWindow.CompleteLoginAsync(OneWindow mainWindow)` | Dispatches the handoff of the supplied OneWindow to the UI thread. Construct the window on the UI thread. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `bool LoginWindow.CompleteLogin(Func<OneWindow> mainWindowFactory)` | After successful authentication, replaces this desktop MainWindow with a fresh OneWindow. Returns false if Closing cancels the transition or a completion is already in progress. Repeated calls after success return true without invoking the factory again. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `Task<bool> LoginWindow.CompleteLoginAsync(Func<OneWindow> mainWindowFactory)` | Dispatches completion, including window construction, to the UI thread. |
| [`OneDialog`](../../api/en/TheOne.Avalonia.Controls.OneDialog.md) | `static Task<OneDialogResult> OneDialog.ShowAsync<TView, TViewModel>(TViewModel viewModel, DialogOptions? options = null, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken)) where TView : UserControl, new() where TViewModel : class` | Creates a typed view lazily on the UI thread; compiled bindings and NativeAOT are supported. |
| [`OneDialog`](../../api/en/TheOne.Avalonia.Controls.OneDialog.md) | `static Task<OneDialogResult> OneDialog.ShowAsync(string title, string message, OneDialogButtons buttons = OneDialogButtons.Ok, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken))` | Dispatches to the UI thread internally. |
| [`OneDialog`](../../api/en/TheOne.Avalonia.Controls.OneDialog.md) | `static Task<OneDialogResult> OneDialog.ShowContentAsync(object? content, string? title = null, OneDialogButtons buttons = OneDialogButtons.OkCancel, IDataTemplate? contentTemplate = null, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken))` | Dispatches to the UI thread internally. |
| [`OneNotification`](../../api/en/TheOne.Avalonia.Controls.OneNotification.md) | `static void OneNotification.Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)` | Shows on the UI thread. The owner is optional; the registered default owner or most recently activated visible themed window is used. |
| [`OneNotification`](../../api/en/TheOne.Avalonia.Controls.OneNotification.md) | `static void OneNotification.Show(INotification notification, Window? owner = null)` | UI thread. |
| [`OneNotification`](../../api/en/TheOne.Avalonia.Controls.OneNotification.md) | `static Task OneNotification.ShowAsync(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)` | Dispatches to the UI thread internally. |
| [`OneNotification`](../../api/en/TheOne.Avalonia.Controls.OneNotification.md) | `static void OneNotification.CloseAll(Window? owner = null)` | UI thread. |
| [`OneToast`](../../api/en/TheOne.Avalonia.Controls.OneToast.md) | `static void OneToast.Show(string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)` | UI thread. |
| [`OneToast`](../../api/en/TheOne.Avalonia.Controls.OneToast.md) | `static void OneToast.Show(INotification notification, Window? owner = null)` | Shows the notification's message and status icon, ignoring Title, while retaining expiration and click/close callbacks. |
| [`OneToast`](../../api/en/TheOne.Avalonia.Controls.OneToast.md) | `static Task OneToast.ShowAsync(string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)` | Dispatches to the UI thread internally. |
| [`OneToast`](../../api/en/TheOne.Avalonia.Controls.OneToast.md) | `static void OneToast.CloseAll(Window? owner = null)` | UI thread. |

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`DialogContext`](../../api/en/TheOne.Avalonia.Controls.DialogContext.md) | `PropertyChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`DialogContext`](../../api/en/TheOne.Avalonia.Controls.DialogContext.md) | `CloseRequested` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs`](../../api/en/TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs.md) | 1 |
| [`TheOne.Avalonia.Controls.DialogOptions`](../../api/en/TheOne.Avalonia.Controls.DialogOptions.md) | 9 |
| [`TheOne.Avalonia.Controls.OneDialogButtons`](../../api/en/TheOne.Avalonia.Controls.OneDialogButtons.md) | 4 |
| [`TheOne.Avalonia.Controls.OneDialogResult`](../../api/en/TheOne.Avalonia.Controls.OneDialogResult.md) | 5 |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/en/TheOne.Avalonia.Controls.ThemeButtonScope.md) | 2 |
| [`TheOne.Avalonia.Styling.ControlSize`](../../api/en/TheOne.Avalonia.Styling.ControlSize.md) | 3 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
