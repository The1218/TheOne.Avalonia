# LoginWindow

[English](../en/TheOne.Avalonia.Controls.LoginWindow.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.LoginWindow.md) · [API index](README.md)

`TheOne.Avalonia.Controls.LoginWindow` · Class · `Avalonia.Controls.Window`

A themed native Window whose caller-owned content performs authentication.

[Source / 声明](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L13)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L20) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void LoginWindow.ShowErrorToast(string message, TimeSpan? expiration = null)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L71) | Shows an error toast in this login window on the UI thread. |
| [`void LoginWindow.ShowErrorNotification(string message, string title = "登录失败", TimeSpan? expiration = null)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L75) | Shows an error notification in this login window on the UI thread. |
| [`Task<OneDialogResult> LoginWindow.ShowErrorDialog(string message, string title = "登录失败", CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L79) | Shows an error dialog in this login window; dispatches to the UI thread. |
| [`bool LoginWindow.CompleteLogin(OneWindow mainWindow)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L84) | Installs the supplied fresh, unopened OneWindow as the desktop MainWindow and all message owners, then closes login. |
| [`Task<bool> LoginWindow.CompleteLoginAsync(OneWindow mainWindow)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L91) | Dispatches the handoff of the supplied OneWindow to the UI thread. Construct the window on the UI thread. |
| [`bool LoginWindow.CompleteLogin(Func<OneWindow> mainWindowFactory)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L105) | After successful authentication, replaces this desktop MainWindow with a fresh OneWindow. Returns false if Closing cancels the transition or a completion is already in progress. Repeated calls after success return true without invoking the factory again. |
| [`Task<bool> LoginWindow.CompleteLoginAsync(Func<OneWindow> mainWindowFactory)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L188) | Dispatches completion, including window construction, to the UI thread. |
| [`LoginWindow.LoginWindow()`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L29) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly DirectProperty<LoginWindow, bool> LoginWindow.IsDialogOpenProperty`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.OneDialogResult`](TheOne.Avalonia.Controls.OneDialogResult.md)
- [`TheOne.Avalonia.Controls.OneWindow`](TheOne.Avalonia.Controls.OneWindow.md)
