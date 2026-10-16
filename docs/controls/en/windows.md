# OneWindow, LoginWindow, and window services

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

ThemeButton is an icon-only ToggleButton, checked for Dark, switching Light / Dark only. The independent [ThemeColorPicker](../../theme/en/theme-color-picker.md) selects color resources.

ShowTitleBar / ShowThemeButton default to True; TitleBarHeight is overridable. small / normal / large title heights are 42 / 48 / 56, with body / dialog fonts 12 / 14 / 16. Classes or ControlSizing.Size follow window theme and motion inheritance.

Blank title-bar areas drag; double-click maximizes / restores. Buttons use native WindowState, CanMinimize, CanMaximize, and Close; native Closing can cancel. Defaults use BorderOnly and extended client area with platform borders. The title body declares User decoration role; OneWindow centralizes dragging / double-click so macOS native hit testing and managed events do not both toggle state. System buttons retain native decoration roles. Verify actual dragging, resizing, snapping, and DPI on the target desktop.

ThemeButton switches the nearest window's effective Light / Dark by default; icons, tooltips, and accessible names update. OneWindow.ThemeButtonScope="Application" switches globally; explicitly themed windows retain local settings. Standalone buttons can appear elsewhere:

```xml
<one:ThemeButton Scope="Application" Classes="small" />
```

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

PreviewHost has separate Toast, Dialog, Notification pages with type, body, size, and motion settings. Toast omits title settings; Notification and Dialog retain them. Notification / Toast configure position, limits, duration, auto / manual dismissal, and callbacks. Dialog tests titles, close buttons, action combinations, overlays, Escape, and ViewModel results. OneWindow, LoginWindow, and NotificationCard no longer have catalog display pages; controls and lower-level checks remain. ThemeButton is shown with Button. Independent binding / native examples: [NativeAotSmoke](../../../design/PreviewHost/Consumers/NativeAotSmoke/Program.cs).

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

NativeAotSmoke provides the Toast-only `--verify-toast` check for both themes, three densities, four states, multiline alignment in narrow windows, callbacks and expiration. Omit `--require-aot` for managed execution. Publish and execute a native artifact with `--require-aot` only for affected AOT/binding/metadata changes. Build only the required consumer project with task-specific output directories; see [integration and validation](../../integration/en/README.md#6-implementation-and-acceptance). PreviewHost's `--verify-buttons-messages` covers buttons and messages together rather than Toast alone.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section lists only Classes, properties, extension APIs/events and related models added by this project, without repeating inherited Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost visibly compares project-added styles and uses Description components for control-specific properties, API signatures and model fields. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

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

### OneDialogPresenter

[API / model reference](../../api/en/TheOne.Avalonia.Controls.OneDialogPresenter.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Buttons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L12) | `TheOne.Avalonia.Controls.OneDialogButtons` | writable; OneWay | `0 / default` | Dialog button configuration/flags, interpreted according to the options type and close-result contract. Values: `Ok`, `OkCancel`, `YesNo`, `YesNoCancel` |
| [`CloseOnEscape`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L20) | `bool` | writable; OneWay | `true` | Allows Escape to close a dialog, following its cancellation/result semantics. |
| [`ShowButtons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L18) | `bool` | writable; OneWay | `true` | Shows the default button area; does not control buttons in custom content. |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L16) | `bool` | writable; OneWay | `true` | Shows the close button; the host and associated commands retain control over closing. |
| [`ShowTitle`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L14) | `bool` | writable; OneWay | `true` | Shows the dialog title area while retaining its configured title value. |

### OneNotification

[API / model reference](../../api/en/TheOne.Avalonia.Controls.OneNotification.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L86) | `Avalonia.Controls.Window?` | writable; CLR / n/a | `accessor-defined / state` | The native relationship established by `child.Show(parent)` / `child.ShowDialog<T>(parent)`, affecting platform window lifetime. |

### OneToast

[API / model reference](../../api/en/TheOne.Avalonia.Controls.OneToast.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L103) | `Avalonia.Controls.Window?` | writable; CLR / n/a | `accessor-defined / state` | The native relationship established by `child.Show(parent)` / `child.ShowDialog<T>(parent)`, affecting platform window lifetime. |

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

### ThemeButton

[API / model reference](../../api/en/TheOne.Avalonia.Controls.ThemeButton.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Scope`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L16) | `TheOne.Avalonia.Controls.ThemeButtonScope` | writable; OneWay | `0 / default` | Scope of Light/Dark switching; does not change the ThemeColor palette enum. Values: `Window`, `Application` |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| OneDialogPresenter | `one-large` | automatic template class; do not bind manually | one&#124;OneDialogPresenter.one-large => FontSize=16; Padding=32 |
| OneDialogPresenter | `one-normal` | automatic template class; do not bind manually | one&#124;OneDialogPresenter.one-normal => FontSize=14; Padding=24 |
| OneDialogPresenter | `one-small` | automatic template class; do not bind manually | one&#124;OneDialogPresenter.one-small => FontSize=12; Padding=16 |
| OneWindow | `one-large` | automatic template class; do not bind manually | one&#124;OneWindow.one-large => TitleBarHeight=56; FontSize=16 |
| OneWindow | `one-normal` | automatic template class; do not bind manually | one&#124;OneWindow.one-normal => TitleBarHeight=48; FontSize=14 |
| OneWindow | `one-small` | automatic template class; do not bind manually | one&#124;OneWindow.one-small => TitleBarHeight=42; FontSize=12 |
| ThemeButton | `one-large` | automatic template class; do not bind manually | one&#124;ThemeButton.one-large => Width=44; Height=44 |
| ThemeButton | `one-normal` | automatic template class; do not bind manually | one&#124;ThemeButton.one-normal => Width=36; Height=36 |
| ThemeButton | `one-small` | automatic template class; do not bind manually | one&#124;ThemeButton.one-small => Width=30; Height=30 |

`:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without added appearance classes use their default theme and control-specific properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs`](../../api/en/TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DialogContext`](../../api/en/TheOne.Avalonia.Controls.DialogContext.md) | Class | 0 | 1 | 2 |
| [`TheOne.Avalonia.Controls.DialogOptions`](../../api/en/TheOne.Avalonia.Controls.DialogOptions.md) | Class | 9 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneDialog`](../../api/en/TheOne.Avalonia.Controls.OneDialog.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogButtons`](../../api/en/TheOne.Avalonia.Controls.OneDialogButtons.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogPresenter`](../../api/en/TheOne.Avalonia.Controls.OneDialogPresenter.md) | Class | 5 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogResult`](../../api/en/TheOne.Avalonia.Controls.OneDialogResult.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneNotification`](../../api/en/TheOne.Avalonia.Controls.OneNotification.md) | Class | 1 | 4 | 0 |
| [`TheOne.Avalonia.Controls.OneToast`](../../api/en/TheOne.Avalonia.Controls.OneToast.md) | Class | 1 | 4 | 0 |
| [`TheOne.Avalonia.Controls.OneWindow`](../../api/en/TheOne.Avalonia.Controls.OneWindow.md) | Class | 10 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeButton`](../../api/en/TheOne.Avalonia.Controls.ThemeButton.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/en/TheOne.Avalonia.Controls.ThemeButtonScope.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Styling.ControlSize`](../../api/en/TheOne.Avalonia.Styling.ControlSize.md) | Enum | 3 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
