# LoginWindow and main-window handoff

[简体中文](../zh-cn/login-window.md) · **English**

See [windows](windows.md) for themes and shared services.

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

`dotnet run --project design/PreviewHost -- --login` exercises startup / handoff. Compiled example bindings accept any nonempty account / password as local demo validation, show error Toast for empty input, and provide three error helpers. Closing cancels waits and clears passwords. No authentication service is contacted. The main catalog has no OneWindow / LoginWindow pages; this independent entry demonstrates the full flow.

`--verify-loginwindow` checks interactions. `--verify-login-lifetime <ShutdownMode>` checks shutdown in a real desktop loop; `--verify-login-lifetime-close <ShutdownMode>` checks login Closed immediately closing the new main. Each runs independently with OnMainWindowClose, OnLastWindowClose, and OnExplicitShutdown. Targeted render: `--render-controls .preview/loginwindow LoginWindow`.

Use `dotnet run --project design/PreviewHost -- --login --trace-login` to inspect actual window creation. Optional `LOGIN_TRACE` logs busy state, completion requests, factory call counts, open / close events, and exit window counts without accounts / passwords. Validation delay and shutdown policy retain demo settings.

Lifecycle references: [Avalonia desktop lifetimes](https://docs.avaloniaui.net/docs/fundamentals/application-lifetimes) and [12.1.4 Window sources](https://github.com/AvaloniaUI/Avalonia/blob/12.1.4/src/Avalonia.Controls/Window.cs).
