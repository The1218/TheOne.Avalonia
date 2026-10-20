# OneWindow, LoginWindow, and window services

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

[简体中文](../zh-cn/windows.md) · **English**

OneWindow inherits native Window and is available after OneTheme loads. It keeps Content / ContentTemplate, ownership, Closing, state, and platform lifecycle. Title bars, messages, and dialogs live in templates without wrapping and reassigning business Content.

Use `OneWindow` for the main workspace and [LoginWindow](login-window.md) for a sign-in window that hands off to it. Both already have dedicated documentation and preserve native window ownership. PreviewHost opens on Home; its live controls and usage snippets lead into the full samples.

```xml
<one:OneWindow xmlns="https://github.com/avaloniaui"
               xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
               xmlns:one="using:TheOne.Avalonia.Controls"
               xmlns:vm="using:YourApp.ViewModels"
               x:Class="YourApp.MainWindow" x:DataType="vm:MainViewModel"
               Title="我的工作区" Width="1000" Height="720">
  <one:OneWindow.LeftContent>
    <TextBlock Text="{Binding WorkspaceName}" />
  </one:OneWindow.LeftContent>
  <one:OneWindow.RightContent>
    <StackPanel Orientation="Horizontal" Spacing="8">
      <TextBox Width="160" Text="{Binding Search, Mode=TwoWay}" />
      <Button Classes="ghost" Content="设置" Command="{Binding SettingsCommand}" />
    </StackPanel>
  </one:OneWindow.RightContent>
  <Grid><!-- 正常业务内容 --></Grid>
</one:OneWindow>
```

The code-behind must inherit the same window type: `public partial class MainWindow : OneWindow`. Set `desktop.MainWindow = new MainWindow();` during desktop startup. Register `OneTheme` once in App.axaml; ordinary controls inside the window use the application's theme and density.

Code-behind must also inherit OneWindow. LeftContent / RightContent accept objects or Control, with LeftContentTemplate / RightContentTemplate; object templates need x:DataType. Controls inherit window DataContext, and buttons / inputs do not initiate title-bar dragging.

See [LoginWindow](login-window.md) for login UI, authenticated handoff, and process shutdown.

## Title bar and theme

ThemeButton is an icon-only ToggleButton. By default it switches Light / Dark. Set the inherited `IsThreeState="True"` to cycle Default → Dark → Light → Default; `IsChecked` is null / true / false respectively. Default displays a system icon. The independent [ThemeColorPicker](../../theme/en/theme-color-picker.md) selects color resources.

ShowTitleBar / ShowThemeButton default to True; TitleBarHeight is overridable. small / normal / large title heights are 42 / 48 / 56, with body / dialog fonts 12 / 14 / 16. Classes or ControlSizing.Size follow window theme and motion inheritance.

Blank title-bar areas drag; double-click maximizes / restores. Buttons use native WindowState, CanMinimize, CanMaximize, and Close; native Closing can cancel. Defaults use BorderOnly and extended client area with platform borders. The title body declares User decoration role; OneWindow centralizes dragging / double-click so macOS native hit testing and managed events do not both toggle state. System buttons retain native decoration roles. Verify actual dragging, resizing, snapping, and DPI on the target desktop.

ThemeButton switches the nearest window's effective Light / Dark by default; icons, tooltips, and accessible names update. OneWindow.ThemeButtonScope="Application" switches globally; explicitly themed windows retain local settings. Standalone buttons can appear elsewhere:

```xml
<one:ThemeButton Scope="Application" Classes="small" />
<one:ThemeButton Scope="Application" IsThreeState="True" />
<one:OneWindow ThemeButtonScope="Application" ThemeButtonIsThreeState="True" />
```

Application Default follows system theme changes automatically. Window Default restores inheritance from the application. Keep buttons controlling the same UI in the same scope: a window override takes precedence over the application. Buttons observe their target's requested and actual theme, so Default remains distinguishable even when its effective brightness does not change.

Clicks use SetCurrentValue to preserve bindings. The shortcut toggles light / dark; RequestedThemeVariant=Default still follows system settings. PreviewHost uses Application scope synchronized with its theme dropdown.

## Automatic message hosts

See [window owners and message services](window-owners.md) for owner resolution, native ownership versus message targets, threading, Dialog queues, and login handoff.

Show OneWindow, then use the services without finding TopLevel, creating managers, or registering hosts:

```csharp
using Avalonia.Controls.Notifications;
using TheOne.Avalonia.Controls;

OneNotification.Show("保存成功", "资料已保存。", NotificationType.Success);
OneToast.Show("更新完成", NotificationType.Success);
var result = await OneDialog.ShowAsync("确认", "是否继续？", OneDialogButtons.YesNoCancel);
```

Notifications use an internal native WindowNotificationManager: top-right, at most four active, four-second expiration. Toast has a separate manager: top-center, at most three, 2.5 seconds. TimeSpan.Zero lasts until manual close. Both retain the native NotificationCard lifecycle, semantic types and Motion.Enabled. CloseAll clears the requested category. Notification keeps its title and close button, widths 280 / 320 / 360, fonts 11 / 12 / 13 and padding 8 / 10 / 12. Toast uses the dedicated One.ToastCard template with only a status icon and message: no title, close button, decorative stripe or icon badge. INotification.Title is ignored visually. Short messages size naturally; long messages wrap at maximum widths 280 / 320 / 360 and respect narrower window space. The icon and the complete text block align vertically at their centers, without a fixed message height or ellipsis. Toast fonts are 12 / 14 / 16, icons 16 / 18 / 20 and padding 12,8 / 16,10 / 20,12. Persistent toasts can be dismissed by clicking the body or calling CloseAll; click / close callbacks still run exactly once.

Without owner arguments, services use Owner. Defaults update on OneWindow / LoginWindow opening or activation and can be set on the UI thread. Hidden, closed, or unopened defaults fall back to the most recently activated visible themed window. Pass owners explicitly for multiple windows; unavailable explicit owners fail instead of silently redirecting. OneWindow.Current exposes the most recent visible OneWindow on the UI thread, using weak registration removed on close.

Synchronous Show / CloseAll run on the UI thread. Background work uses async entry points; dialog APIs always dispatch automatically:

```csharp
await OneNotification.ShowAsync("处理完成", "可继续操作。", owner: mainWindow);
await OneToast.ShowAsync("同步完成", owner: mainWindow);
```

Without an open OneWindow / LoginWindow, APIs report lifecycle errors rather than losing messages. Ordinary Window does not acquire a host automatically.

## Dialog results and lifecycle

OneDialogButtons supports Ok, OkCancel, YesNo, YesNoCancel. Results are Ok, Cancel, Yes, No, or Closed. Escape returns Cancel; window closure returns Closed. CancellationToken cancels Tasks and closes active popups, or independently cancels queued requests.

Dialogs queue in call order per window; only active requests create OneDialogPresenter. Background content, title-bar content, and theme buttons are disabled during modality. Primary buttons receive initial focus, Tab cycles in the dialog, and closing restores previous focus. Long content scrolls within finite viewports while actions stay visible.

```csharp
using var cancellation = new CancellationTokenSource();
var editor = new TextBox { Text = "工作区" };
var result = await OneDialog.ShowContentAsync(editor, "编辑名称",
    OneDialogButtons.OkCancel, owner: mainWindow, cancellationToken: cancellation.Token);
```

ShowContentAsync supports models and explicit contentTemplate. Control content must have no parent; closing detaches it while preserving input state for reuse. Large forms still need bounded virtualized lists; hosts do not virtualize arbitrary business collections.

Closing cleans active / queued requests to avoid hanging awaits; cancelled closure retains hosts. Custom OneWindow / LoginWindow templates must keep PART_VisualLayerManager, PART_ContentPresenter, PART_Notifications, PART_Toasts, PART_DialogShield, PART_DialogLayer, and relevant title-bar PART_*. Missing message parts cause API errors.

## Generic ViewModel dialogs

```csharp
var vm = new EditorViewModel();
var result = await OneDialog.ShowAsync<EditorView, EditorViewModel>(vm,
    new DialogOptions
    {
        Title = "编辑资料",
        ShowTitle = true,
        ShowCloseButton = true,
        CloseOnOverlayClick = false,
        CloseOnEscape = true
    });
if (result == OneDialogResult.Ok)
{
    // 使用 vm 中保存的数据，更新父页面。
}
```

These are modal overlays within OneWindow, blocking background input, trapping keyboard focus, and restoring it on close. TView must inherit UserControl with a public parameterless constructor. TViewModel can be a plain class or DialogContext. Generic construction creates the view and assigns DataContext without ViewLocator, runtime type lookup, or reflection. Enable compiled AXAML bindings with the matching ViewModel x:DataType.

DialogOptions is an immutable record; omitted options create fresh defaults, not mutable global state. Defaults: ShowTitle=true (empty Title hides it), ShowCloseButton=true, CloseOnOverlayClick=false, CloseOnEscape=true, ShowButtons=false, Buttons=OkCancel. ShowButtons=true enables generic bottom actions; Buttons selects their combination. Optional Size / MotionEnabled override the overlay, otherwise inheriting the parent. Disabling both title / close button removes header space. String / ShowContentAsync APIs retain generic bottom actions.

```csharp
public sealed class EditorViewModel : DialogContext
{
    private string _name = "新资料";
    public string Name { get => _name; set => SetProperty(ref _name, value); }
    // 在绑定的 SaveCommand / CancelCommand 中调用：
    public void Save() => RequestClose(OneDialogResult.Ok);
    public void Cancel() => RequestClose(OneDialogResult.Cancel);
}
```

RequestClose raises CloseRequested; the coordinator closes only requests associated with that ViewModel and resolves the parent's awaited OneDialogResult. It supports background triggering and queued ViewModels can end before their View is created, without affecting active dialogs. Close buttons / optional overlay clicks return Closed; Escape returns Cancel; cancellation cancels Tasks; host closure returns Closed.

Completion, cancellation, and host closure detach events, allowing sequential ViewModel reuse. A DialogContext cannot serve concurrent requests across windows, avoiding one close event ending multiple dialogs; concurrent use returns a faulted Task. A thousand queued generic requests create only the active View, retaining request data for the rest. PreviewHost's Messages and dialogs → Dialog form changes title / close / overlay / Escape settings and verifies results through compiled save / cancel commands.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### DialogContext

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.DialogContext.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### DialogOptions

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.DialogOptions.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Buttons`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L16) | `TheOne.Avalonia.Controls.OneDialogButtons` | writable; CLR / init-only | `OneDialogButtons.OkCancel` | Dialog button configuration/flags, interpreted according to the options type and close-result contract. Values: `Ok`, `OkCancel`, `YesNo`, `YesNoCancel` |
| [`CloseOnEscape`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L14) | `bool` | writable; CLR / init-only | `true` | Allows Escape to close a dialog, following its cancellation/result semantics. |
| [`CloseOnOverlayClick`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L13) | `bool` | writable; CLR / init-only | `false` | Allows overlay clicks to close the dialog, following cancellation/result options. |
| [`MotionEnabled`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L9) | `bool?` | writable; CLR / init-only | `null` | Dialog-specific motion switch, also respecting inherited Motion.Enabled. |
| [`ShowButtons`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L15) | `bool` | writable; CLR / init-only | `false` | Shows the default button area; does not control buttons in custom content. |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L12) | `bool` | writable; CLR / init-only | `true` | Shows the close button; the host and associated commands retain control over closing. |
| [`ShowTitle`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L11) | `bool` | writable; CLR / init-only | `true` | Shows the dialog title area while retaining its configured title value. |
| [`Size`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L8) | `TheOne.Avalonia.Styling.ControlSize?` | writable; CLR / init-only | `null` | Dialog size enum using the library's supported size presets. Values: `Small`, `Normal`, `Large` |
| [`Title`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L10) | `string?` | writable; CLR / init-only | `null` | Display title, distinct from business identity or a localization key. |

### OneDialog

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.OneDialog.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L217) | `Avalonia.Controls.Window?` | writable; CLR / n/a | `accessor-defined / state` | The native relationship established by `child.Show(parent)` / `child.ShowDialog<T>(parent)`, affecting platform window lifetime. |

### OneDialogPresenter

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.OneDialogPresenter.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Buttons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L44) | `TheOne.Avalonia.Controls.OneDialogButtons` | writable; OneWay | `0 / default` | Dialog button configuration/flags, interpreted according to the options type and close-result contract. Values: `Ok`, `OkCancel`, `YesNo`, `YesNoCancel` |
| [`CloseOnEscape`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L68) | `bool` | writable; OneWay | `true` | Allows Escape to close a dialog, following its cancellation/result semantics. |
| [`ShowButtons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L62) | `bool` | writable; OneWay | `true` | Shows the default button area; does not control buttons in custom content. |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L56) | `bool` | writable; OneWay | `true` | Shows the close button; the host and associated commands retain control over closing. |
| [`ShowTitle`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L50) | `bool` | writable; OneWay | `true` | Shows the dialog title area while retaining its configured title value. |

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

### ThemeButton

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.ThemeButton.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Scope`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L30) | `TheOne.Avalonia.Controls.ThemeButtonScope` | writable; OneWay | `0 / default` | Scope of Light/Dark switching; does not change the ThemeColor palette enum. Values: `Window`, `Application` |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`DialogContext`](../../api/en/TheOne.Avalonia.Controls.DialogContext.md) | `void DialogContext.RequestClose(OneDialogResult result = OneDialogResult.Ok)` | Requests completion of this dialog, including when it is still queued. |
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
