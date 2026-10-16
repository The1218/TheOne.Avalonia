# LoginWindow and main-window handoff

[简体中文](../zh-cn/login-window.md) · **English**

See [windows](windows.md) for themes and shared services.

See [window owners and message services](window-owners.md) for explicit login owners, error helpers, Toast / Notification / Dialog lifetimes, and message routing after handoff.

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

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost labels example Classes, sizes and states directly on each control page. Description components explain public properties, APIs/events and model fields, including property types, defaults, binding modes, validation and API signatures. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### LoginWindow

[API / model reference](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L20) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### OneWindow

[API / model reference](../../api/en/TheOne.Avalonia.Controls.OneWindow.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Current`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L39) | `TheOne.Avalonia.Controls.OneWindow?` | read-only; CLR / n/a | `accessor-defined / state` | UI thread. |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L38) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`LeftContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L30) | `object?` | writable; OneWay | `null` | Additional content on the left side of the library window title bar. |
| [`LeftContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L32) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`RightContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L31) | `object?` | writable; OneWay | `null` | Additional right title-bar content; avoid covering window-operation hit areas. |
| [`RightContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L33) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`ShowThemeButton`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L34) | `bool` | writable; OneWay | `true` | Shows the built-in brightness toggle; palette resources remain controlled by ThemeColorPicker/OneTheme. |
| [`ShowTitleBar`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L36) | `bool` | writable; OneWay | `true` | Shows the library title bar without changing window lifetime or ownership. |
| [`ThemeButtonScope`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L35) | `TheOne.Avalonia.Controls.ThemeButtonScope` | writable; OneWay | `0 / default` | Theme-toggle scope for the built-in button, distinguishing application and window via enum. Values: `Window`, `Application` |
| [`TitleBarHeight`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L37) | `double` | writable; OneWay | `48` | Title-bar height in DIPs; retain sufficient title and window-button hit areas. Validation: `value => double.IsFinite(value) && value >= 0` |

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
| LoginWindow | `one-large` | automatic template class; do not bind manually | one&#124;LoginWindow.one-large => FontSize=16 |
| LoginWindow | `one-normal` | automatic template class; do not bind manually | one&#124;LoginWindow.one-normal => FontSize=14 |
| LoginWindow | `one-small` | automatic template class; do not bind manually | one&#124;LoginWindow.one-small => FontSize=12 |
| OneWindow | `one-large` | automatic template class; do not bind manually | one&#124;OneWindow.one-large => TitleBarHeight=56; FontSize=16 |
| OneWindow | `one-normal` | automatic template class; do not bind manually | one&#124;OneWindow.one-normal => TitleBarHeight=48; FontSize=14 |
| OneWindow | `one-small` | automatic template class; do not bind manually | one&#124;OneWindow.one-small => TitleBarHeight=42; FontSize=12 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.LoginWindow`](../../api/en/TheOne.Avalonia.Controls.LoginWindow.md) | Class | 1 | 8 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogResult`](../../api/en/TheOne.Avalonia.Controls.OneDialogResult.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneWindow`](../../api/en/TheOne.Avalonia.Controls.OneWindow.md) | Class | 10 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/en/TheOne.Avalonia.Controls.ThemeButtonScope.md) | Enum | 2 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
