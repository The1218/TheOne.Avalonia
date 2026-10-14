# 内容过渡与通知

菜单、浮层与布局见[布局入口](layout.md)。

## TransitioningContentControl 内容切换

使用官方控件与数据模板，将当前业务页面绑定到 Content：

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

`vm` 为消费项目的 ViewModel 命名空间。页面模型和动画开关由业务实现；输入状态放在模型中，避免模板释放后丢失。可覆盖 ContentTemplate，或直接使用 Control 内容；返回同一个 Control 时保留原生单父级规则。

默认采用 200 ms CrossFade。可在 PageTransition 中配置原生 PageSlide、CrossFade、CompositePageTransition 或支持取消的自定义 IPageTransition；IsTransitionReversed 保留官方方向语义，PageTransition=null 始终直接切换。

```xml
<TransitioningContentControl Content="{Binding CurrentPage}">
  <TransitioningContentControl.PageTransition>
    <PageSlide Duration="0:0:0.2" Orientation="Horizontal"
               SlideInEasing="CubicEaseOut" SlideOutEasing="CubicEaseOut" />
  </TransitioningContentControl.PageTransition>
</TransitioningContentControl>
```

三档密度通过 Classes="small|normal|large" 或 ControlSizing.Size 设置，并向内容继承。small / normal / large 的内边距为 12 / 16 / 20，正文字号为 12 / 14 / 16；高度由布局和内容决定，示例的 400 仅用于有界预览。外框固定，内容在圆角范围内过渡；Foreground、Background、BorderBrush、BorderThickness、CornerRadius、Padding 与对齐属性均可覆盖。

Motion.Enabled 可在窗口、容器或控件上继承。False 保留新内容、输入和原生事件，播放中关闭会取消过渡并立即复位透明度／位移；重新开启后，下一次内容变化恢复动画。主题通过 ContentTransitionOptions.UseMotionSettings 默认接入，对原生 PageTransition 配置使用取消适配器，SetCurrentValue 保留单向绑定。读取用户配置可调用 ContentTransitionOptions.GetConfiguredTransition(control)；设 UseMotionSettings=False 可退出适配。自定义 IPageTransition 须响应传入的 CancellationToken。

TransitionCompleted 的 From / To 保持原生内容对象；HasRunToCompletion 仅在动画完整结束时为 True，直接切换、取消与被后续内容替代时为 False。不要将此事件等同于业务保存成功。过渡期间仅最新内容可被鼠标点击；旧模板在完成后释放，最多保留两个原生呈现器。

该控件本身不是列表。内容包含大数据时，仍需使用有界 ListBox / DataGrid 等虚拟化控件。PreviewHost 的“内容切换 TransitioningContentControl”页使用三个页面模型，其中记录页包含 1,000 条数据，验证远距离定位、选择与实际容器数量，并支持草稿输入、反向、快速往返、空内容、禁用和动画中关闭动效。

## NotificationCard 通知

通过官方 WindowNotificationManager 在已显示的窗口上创建通知；以下代码放在窗口 Loaded 后的处理逻辑中，管理器作为窗口字段保留：

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

加载 OneTheme 后，管理器创建的原生 NotificationCard 自动使用 TheOne 主题：共同的 Surface / Text / Line，四种类型分别使用对应的语义图标与侧边标记。关闭按钮、点击正文、Expiration 自动关闭、Close(content)、CloseAll 与原生 NotificationClosed 回调均保留。

small / normal / large 使用比标准表单更紧凑的消息密度：字号 11 / 12 / 13，内边距 8 / 10 / 12，图标底座及关闭按钮 20 / 24 / 28，图标 12 / 14 / 16。通过父窗口的 Classes 或 ControlSizing.Size 继承，也可为单张卡片设置尺寸。默认 normal 宽度 320，small / large 为 280 / 360；Width、Padding、Foreground、Background、BorderBrush、BorderThickness、CornerRadius 与 ContentTemplate 可覆盖。自定义 Control 内容仍由原生 ContentPresenter 承载。

Motion.Enabled 从窗口、管理器或卡片继承。默认入场 200 ms、离场 140 ms，只动画透明度与位移。播放中关闭动效会复位显示状态，正在关闭的通知则直接完成移除；自动关闭计时和手动关闭继续工作。重新开启后，后续通知及关闭操作恢复动画，不重复播放已显示卡片的入场。可通过 One.Motion.Notification.Enter / Exit 动态资源，或 NotificationMotion.EnterDuration / ExitDuration 覆盖时长。

通知是短时消息，不是历史列表。使用有限 MaxItems 控制活动卡片数量；离场动画期间可能短暂保留正在关闭的卡片。PreviewHost 连续投递 1,000 条通知，验证最多 4 条活动卡片和关闭后的释放；需要长期通知历史时，另用绑定模型的有界虚拟化 ListBox。侧栏“Notification”提供四种类型、自动关闭延迟、手动关闭、六种位置、动效切换、回调计数与重置，沿用全局主题和三档尺寸。
