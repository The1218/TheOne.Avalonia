# Content transitions and notifications

[简体中文](../zh-cn/content-feedback.md) · **English**

See [layout](layout.md) for menus and popups.

## TransitioningContentControl

Use the native control and data templates, binding the current business page to Content:

```xml
<TransitioningContentControl Classes="normal"
                             Height="400"
                             Content="{Binding CurrentPage}"
                             styling:Motion.Enabled="{Binding AnimationsEnabled}"
                             TransitionCompleted="OnTransitionCompleted">
  <TransitioningContentControl.DataTemplates>
    <DataTemplate DataType="{x:Type vm:OverviewPage}" x:DataType="vm:OverviewPage">
      <TextBlock Text="{Binding Title}" />
    </DataTemplate>
    <DataTemplate DataType="{x:Type vm:EditorPage}" x:DataType="vm:EditorPage">
      <TextBox Text="{Binding Draft, Mode=TwoWay}" />
    </DataTemplate>
  </TransitioningContentControl.DataTemplates>
</TransitioningContentControl>
```

`vm` is the application's ViewModel namespace. Business code supplies page models and motion state. Store input state in models so releasing templates does not lose it. Override ContentTemplate or use Control content directly; returning the same Control retains the native single-parent rule.

The default transition is a 200 ms CrossFade. PageTransition accepts native PageSlide, CrossFade, CompositePageTransition, or a cancellable custom IPageTransition. IsTransitionReversed keeps native direction semantics. PageTransition=null always switches immediately.

```xml
<TransitioningContentControl Content="{Binding CurrentPage}">
  <TransitioningContentControl.PageTransition>
    <PageSlide Duration="0:0:0.2" Orientation="Horizontal"
               SlideInEasing="CubicEaseOut" SlideOutEasing="CubicEaseOut" />
  </TransitioningContentControl.PageTransition>
</TransitioningContentControl>
```

Set density with Classes="small|normal|large" or ControlSizing.Size; it inherits into content. Padding is 12 / 16 / 20 and body fonts 12 / 14 / 16. Content and layout determine height; 400 in the example only bounds the preview. The outer border stays fixed while content transitions within rounded corners. Foreground, Background, BorderBrush, BorderThickness, CornerRadius, Padding, and alignment are overridable.

Motion.Enabled inherits from windows, containers, or controls. False retains new content, input, and native events. Disabling it mid-transition cancels motion and restores opacity / position immediately; after re-enabling, the next content change animates again. ContentTransitionOptions.UseMotionSettings enables theme adaptation by default, wrapping native PageTransition configuration with cancellation support. SetCurrentValue preserves OneWay bindings. Read original user configuration with ContentTransitionOptions.GetConfiguredTransition(control); UseMotionSettings=False opts out. Custom IPageTransition must honor the supplied CancellationToken.

TransitionCompleted From / To remain the native content objects. HasRunToCompletion is True only for completed animation, and False for immediate switches, cancellation, or replacement by newer content. It is not a business-save confirmation. Only the latest content accepts pointer clicks during transition. Old templates are released afterward; at most two native presenters remain.

This control is not a list. Large content still needs bounded, virtualized ListBox / DataGrid controls. PreviewHost's content-transition page uses three page models, including a thousand-record page, testing distant navigation, selection, container counts, drafts, reversal, rapid switching, null content, disabled state, and disabling motion mid-transition.

## NotificationCard

Create notifications through native WindowNotificationManager after the window is shown. Place the following in logic after Loaded and retain the manager as a window field:

```csharp
using Avalonia.Controls.Notifications;
using TheOne.Avalonia.Styling;

var notifications = new WindowNotificationManager(this)
{
    Position = NotificationPosition.TopRight,
    MaxItems = 4
};
notifications.Show(new Notification("保存成功", "资料已保存。",
    NotificationType.Success, TimeSpan.FromSeconds(3),
    onClose: () => { /* 业务关闭回调 */ }));
// TimeSpan.Zero 表示保留到手动关闭；还支持 Information / Warning / Error。
notifications.CloseAll();
Motion.SetEnabled(notifications, false);
```

After OneTheme loads, manager-created native NotificationCard uses TheOne automatically: shared Surface / Text / Line, with semantic icons and side markers for all four types. Close buttons, body clicks, Expiration auto-close, Close(content), CloseAll, and native NotificationClosed callbacks remain available.

small / normal / large use compact message density: fonts 11 / 12 / 13, padding 8 / 10 / 12, icon bases and close buttons 20 / 24 / 28, icons 12 / 14 / 16. Size inherits from window Classes or ControlSizing.Size, or can be set per card. Default normal width is 320; small / large are 280 / 360. Width, Padding, Foreground, Background, BorderBrush, BorderThickness, CornerRadius, and ContentTemplate are overridable. Native ContentPresenter hosts custom Control content.

Motion.Enabled inherits from the window, manager, or card. Entry defaults to 200 ms and exit to 140 ms, animating opacity and position only. Disabling motion restores displayed state immediately; closing notifications are removed immediately. Automatic timing and manual close still work. Re-enabling animates later notifications / closes without replaying entry on existing cards. Override durations with One.Motion.Notification.Enter / Exit dynamic resources or NotificationMotion.EnterDuration / ExitDuration.

Notifications are temporary messages, not history lists. Limit active cards with finite MaxItems; exiting cards can remain briefly during animation. PreviewHost delivers 1,000 notifications and checks at most four active cards and release on close. Use a separate bounded, model-bound virtualized ListBox for history. The Notification page exposes four types, auto-close delays, manual close, six positions, motion, callback counts, and reset, using global theme / size settings.
