# OneWindow, LoginWindow, and window services

[简体中文](../zh-cn/windows.md) · **English**

OneWindow inherits native Window and is available after OneTheme loads. It keeps Content / ContentTemplate, ownership, Closing, state, and platform lifecycle. Title bars, messages, and dialogs live in templates without wrapping and reassigning business Content.

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

Code-behind must also inherit OneWindow. LeftContent / RightContent accept objects or Control, with LeftContentTemplate / RightContentTemplate; object templates need x:DataType. Controls inherit window DataContext, and buttons / inputs do not initiate title-bar dragging.

See [LoginWindow](login-window.md) for login UI, authenticated handoff, and process shutdown.

## Title bar and theme

ShowTitleBar / ShowThemeButton default to True; TitleBarHeight is overridable. small / normal / large title heights are 42 / 48 / 56, with body / dialog fonts 12 / 14 / 16. Classes or ControlSizing.Size follow window theme and motion inheritance.

Blank title-bar areas drag; double-click maximizes / restores. Buttons use native WindowState, CanMinimize, CanMaximize, and Close; native Closing can cancel. Defaults use BorderOnly and extended client area with platform borders. The title body declares User decoration role; OneWindow centralizes dragging / double-click so macOS native hit testing and managed events do not both toggle state. System buttons retain native decoration roles. Verify actual dragging, resizing, snapping, and DPI on the target desktop.

ThemeButton switches the nearest window's effective Light / Dark by default; icons, tooltips, and accessible names update. OneWindow.ThemeButtonScope="Application" switches globally; explicitly themed windows retain local settings. Standalone buttons can appear elsewhere:

```xml
<one:ThemeButton Scope="Application" Classes="small" />
```

Clicks use SetCurrentValue to preserve bindings. The shortcut toggles light / dark; RequestedThemeVariant=Default still follows system settings. PreviewHost uses Application scope synchronized with its theme dropdown.

## Automatic message hosts

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

Completion, cancellation, and host closure detach events, allowing sequential ViewModel reuse. A DialogContext cannot serve concurrent requests across windows, avoiding one close event ending multiple dialogs; concurrent use returns a faulted Task. A thousand queued generic requests create only the active View, retaining request data for the rest. PreviewHost's OneWindow form changes title / close / overlay / Escape settings and verifies results through compiled save / cancel commands.

For Toast, build TheOne.Avalonia.slnx and run PreviewHost with `--verify-buttons-messages`. NativeAotSmoke accepts `--require-aot --verify-toast .preview/toast/native-pixels` to check both themes, three densities, four states, multiline alignment in narrow windows, callbacks and expiration.
