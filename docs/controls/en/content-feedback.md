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

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost's Control Laboratory (top-right action on each page) groups live examples by styles, properties, APIs/events and models; complex models use the complete business samples.

### ContentTransitionOptions

[API / model reference](../../api/en/TheOne.Avalonia.Controls.ContentTransitionOptions.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`UseMotionSettings`](../../../TheOne.Avalonia/Controls/ContentTransitionOptions.cs#L17) | `bool` | writable; OneWay; attached | `false` | Uses shared motion configuration without forcing animation when globally/locally disabled. |

### NotificationMotion

[API / model reference](../../api/en/TheOne.Avalonia.Controls.NotificationMotion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`EnterDuration`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L26) | `System.TimeSpan` | writable; OneWay; attached | `TimeSpan.FromMilliseconds(200)` | Notification entrance duration, respecting shared motion settings. Validation: `value => value >= TimeSpan.Zero` |
| [`ExitDuration`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L28) | `System.TimeSpan` | writable; OneWay; attached | `TimeSpan.FromMilliseconds(140)` | Notification exit duration, respecting shared motion settings. Validation: `value => value >= TimeSpan.Zero` |
| [`UseMotionSettings`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L24) | `bool` | writable; OneWay; attached | `false` | Uses shared motion configuration without forcing animation when globally/locally disabled. |

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
| NotificationCard | `one-large` | automatic template class; do not bind manually | n&#124;NotificationCard.one-large => Width={DynamicResource One.Notification.Large.Width}; FontSize=13; Padding={DynamicResource One.Notification.Large.Padding}; n&#124;NotificationCard.one-large /template/ Border#PART_StatusBadge => Width={DynamicResource One.Notification.Large.BadgeSize}; Height={DynamicResource One.Notification.Large.BadgeSize}; n&#124;NotificationCard.one-large /template/ PathIcon#PART_StatusIcon => Width={DynamicResource One.Notification.Large.IconSize}; Height={DynamicResource One.Notification.Large.IconSize}; n&#124;NotificationCard.one-large /template/ Button#PART_CloseButton => Width={DynamicResource One.Notification.Large.CloseSize}; Height={DynamicResource One.Notification.Large.CloseSize} |
| NotificationCard | `one-normal` | automatic template class; do not bind manually | n&#124;NotificationCard.one-normal => Width={DynamicResource One.Notification.Width}; FontSize=12; Padding={DynamicResource One.Notification.Padding} |
| NotificationCard | `one-small` | automatic template class; do not bind manually | n&#124;NotificationCard.one-small => Width={DynamicResource One.Notification.Small.Width}; FontSize=11; Padding={DynamicResource One.Notification.Small.Padding}; n&#124;NotificationCard.one-small /template/ Border#PART_StatusBadge => Width={DynamicResource One.Notification.Small.BadgeSize}; Height={DynamicResource One.Notification.Small.BadgeSize}; n&#124;NotificationCard.one-small /template/ PathIcon#PART_StatusIcon => Width={DynamicResource One.Notification.Small.IconSize}; Height={DynamicResource One.Notification.Small.IconSize}; n&#124;NotificationCard.one-small /template/ Button#PART_CloseButton => Width={DynamicResource One.Notification.Small.CloseSize}; Height={DynamicResource One.Notification.Small.CloseSize} |
| NotificationCard | `one-toast` | automatic template class; do not bind manually | n&#124;NotificationCard.one-toast.one-small => Width=NaN; MaxWidth={DynamicResource One.Notification.Small.Width}; FontSize=12; Padding=12,8; MinHeight=40; n&#124;NotificationCard.one-toast.one-normal => Width=NaN; MaxWidth={DynamicResource One.Notification.Width}; FontSize=14; Padding=16,10; MinHeight=48; n&#124;NotificationCard.one-toast.one-large => Width=NaN; MaxWidth={DynamicResource One.Notification.Large.Width}; FontSize=16; Padding=20,12; MinHeight=56; n&#124;NotificationCard.one-toast.one-small /template/ PathIcon#PART_StatusIcon => Width=16; Height=16; n&#124;NotificationCard.one-toast.one-normal /template/ PathIcon#PART_StatusIcon => Width=18; Height=18; n&#124;NotificationCard.one-toast.one-large /template/ PathIcon#PART_StatusIcon => Width=20; Height=20; n&#124;NotificationCard.one-toast => Theme={StaticResource One.ToastCard} |
| TransitioningContentControl | `one-large` | automatic template class; do not bind manually | TransitioningContentControl.one-large => FontSize=16; Padding={DynamicResource One.ContentTransition.Large.Padding} |
| TransitioningContentControl | `one-normal` | automatic template class; do not bind manually | TransitioningContentControl.one-normal => FontSize=14; Padding={DynamicResource One.ContentTransition.Padding} |
| TransitioningContentControl | `one-small` | automatic template class; do not bind manually | TransitioningContentControl.one-small => FontSize=12; Padding={DynamicResource One.ContentTransition.Small.Padding} |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.ContentTransitionOptions`](../../api/en/TheOne.Avalonia.Controls.ContentTransitionOptions.md) | Class | 1 | 4 | 0 |
| [`TheOne.Avalonia.Controls.NotificationMotion`](../../api/en/TheOne.Avalonia.Controls.NotificationMotion.md) | Class | 3 | 7 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
