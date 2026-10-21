# LoginWindow and main-window handoff

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

[简体中文](../zh-cn/login-window.md) · **English**

See [windows](windows.md) for themes and shared services.

See [window owners and message services](window-owners.md) for explicit login owners, error helpers, Toast / Notification / Dialog lifetimes, and message routing after handoff.

<!-- visual-guide:start -->
## Appearance and example states

[Capture scope and visual index](visual-index.md). These are actual source-rendered examples; use this page’s usage notes for interaction and binding contracts.

### LoginWindow

![LoginWindow — Login content inside LoginWindow; messaging and main-window handoff have distinct owner rules.](../images/LoginWindow-light.png)

Login content inside LoginWindow; messaging and main-window handoff have distinct owner rules.

[View Dark comparison](../images/LoginWindow-dark.png)

<!-- visual-guide:end -->

## LoginWindow and main-window handoff

LoginWindow inherits native Window with OneTheme semantic colors, fonts, and densities. Defaults retain full platform decorations / title bar, size 440 × 540, padding 24, centered placement, and native close. Resizing / maximization are disabled by default but overridable. The library supplies no accounts, passwords, or authentication rules. Developers provide Content / ContentTemplate as controls or typed x:DataType model templates.

```xml
<one:LoginWindow xmlns="https://github.com/avaloniaui"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:one="using:TheOne.Avalonia.Controls"
                 xmlns:views="using:YourApp.Views"
                 x:Class="YourApp.SignInWindow" Title="登录工作区">
  <views:LoginFormView />
</one:LoginWindow>
```

Code-behind inherits LoginWindow. LoginFormView uses compiled bindings and its own ViewModel type. Set the login as desktop MainWindow at startup; OnMainWindowClose is recommended:

```csharp
public override void OnFrameworkInitializationCompleted()
{
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
        desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
        desktop.MainWindow = new SignInWindow();
    }
    base.OnFrameworkInitializationCompleted();
}
```

In the application's successful authentication callback, provide a new, unshown OneWindow without native Window.Owner. Construct it on the UI thread:

```csharp
// UI 线程认证成功回调；MainWindow 继承 OneWindow。
var mainWindow = new MainWindow();
bool opened = await login.CompleteLoginAsync(mainWindow);
// UI 线程上的同步入口：login.CompleteLogin(mainWindow);
```

CompleteLoginAsync dispatches handoff to the UI thread; the synchronous API requires it. Native login Closing runs first. If accepted, desktop.MainWindow becomes the supplied instance, OneToast.Owner / OneNotification.Owner / OneDialog.Owner update, and the main window opens independently. Owners are checked again before login closes, including changes from login Closed callbacks. The main window is not login-owned, so closing login does not destroy it or its hosts. Applications need no additional Show, Close, or MainWindow assignment.

Cancelled Closing returns false and leaves the supplied window unopened for the caller to retain / dispose. Concurrent handoff calls also return false. Repeating after success returns true without opening another window, even with a different instance; callers own unused instances. Login must be the visible, unowned desktop.MainWindow. Closed, child-owned, or unregistered login windows cannot hand off. The compatible Func<OneWindow> overload invokes the factory on the UI thread only after Closing accepts, allowing delayed construction after background authentication.

Handoff temporarily uses OnExplicitShutdown, restoring the previous ShutdownMode afterward to avoid accidental exit from closing callbacks or failed Show. Show failures throw, restore login MainWindow / prior service owners, and dispose the accepted failed main window and its owned children, allowing form error display / retry. Authentication failures do not call completion. Cancel pending authentication on login Closed and check cancellation before callbacks; late completion after closure is rejected.

Opening / activating LoginWindow registers default Owners for all three services and creates its own Toast, Notification, Dialog layers. owner accepts Window, supporting LoginWindow / OneWindow only. Explicit owners win; omitted owners use available defaults or other themed windows after hiding / closing. Owner uses weak references and UI-thread access. OneWindow.Current still returns only visible OneWindow. The three error helpers explicitly target this login even if another window is active:

```csharp
login.ShowErrorToast("账号或密码错误");
login.ShowErrorNotification("认证服务暂不可用", title: "登录失败");
OneDialogResult result = await login.ShowErrorDialog("请检查账号和密码", title: "登录失败");
```

ShowErrorToast / ShowErrorNotification use NotificationType.Error on the UI thread and accept expiration; TimeSpan.Zero persists. ShowErrorDialog awaits Ok / Closed and supports CancellationToken. Closing login resolves active / queued dialogs as Closed without transferring them to main. New post-handoff requests use OneWindow hosts; Content, DataContext, ContentTemplate, and host instances are not copied.

LoginWindow does not manage dependency injection or .NET Generic Host. Associate shared services / Generic Host with application lifetime and stop / dispose on desktop Exit / Shutdown. Do not attach Host.StopAsync / Dispose to login Closed. Local authentication cancellation / cleanup remains the form's responsibility.

Lifecycle references: [Avalonia desktop lifetimes](https://docs.avaloniaui.net/docs/fundamentals/application-lifetimes) and [12.1.4 Window sources](https://github.com/AvaloniaUI/Avalonia/blob/12.1.4/src/Avalonia.Controls/Window.cs).

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### LoginWindow

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L35) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

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
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `void LoginWindow.ShowErrorToast(string message, TimeSpan? expiration = null)` | Shows an error toast in this login window on the UI thread. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `void LoginWindow.ShowErrorNotification(string message, string title = "登录失败", TimeSpan? expiration = null)` | Shows an error notification in this login window on the UI thread. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `Task<OneDialogResult> LoginWindow.ShowErrorDialog(string message, string title = "登录失败", CancellationToken cancellationToken = default(CancellationToken))` | Shows an error dialog in this login window; dispatches to the UI thread. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `bool LoginWindow.CompleteLogin(OneWindow mainWindow)` | Installs the supplied fresh, unopened OneWindow as the desktop MainWindow and all message owners, then closes login. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `Task<bool> LoginWindow.CompleteLoginAsync(OneWindow mainWindow)` | Dispatches the handoff of the supplied OneWindow to the UI thread. Construct the window on the UI thread. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `bool LoginWindow.CompleteLogin(Func<OneWindow> mainWindowFactory)` | After successful authentication, replaces this desktop MainWindow with a fresh OneWindow. Returns false if Closing cancels the transition or a completion is already in progress. Repeated calls after success return true without invoking the factory again. |
| [`LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | `Task<bool> LoginWindow.CompleteLoginAsync(Func<OneWindow> mainWindowFactory)` | Dispatches completion, including window construction, to the UI thread. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.OneDialogResult`](../../api/en/TheOne.Avalonia.Controls.OneDialogResult.md) | 5 |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/en/TheOne.Avalonia.Controls.ThemeButtonScope.md) | 2 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
