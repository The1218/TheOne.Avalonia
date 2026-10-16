# NotificationMotion

[English](../en/TheOne.Avalonia.Controls.NotificationMotion.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NotificationMotion.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NotificationMotion` · Class · `Avalonia.AvaloniaObject`

Drives native NotificationCard open/close lifecycle with cancellable, inherited motion.

[Source / 声明](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L22)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`EnterDuration`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L26) | `System.TimeSpan` | 可写; OneWay; 附加属性 | `TimeSpan.FromMilliseconds(200)` | 通知入场持续时间；遵守共享动效开关。 校验：`value => value >= TimeSpan.Zero` |
| [`ExitDuration`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L28) | `System.TimeSpan` | 可写; OneWay; 附加属性 | `TimeSpan.FromMilliseconds(140)` | 通知退场持续时间；遵守共享动效开关。 校验：`value => value >= TimeSpan.Zero` |
| [`UseMotionSettings`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L24) | `bool` | 可写; OneWay; 附加属性 | `false` | 使用库的共享动效配置；全局关闭或目标控件关闭时不强制播放。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool NotificationMotion.GetUseMotionSettings(NotificationCard card)`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L35) | 读取附加属性。使用库的共享动效配置；全局关闭或目标控件关闭时不强制播放。 |
| [`static void NotificationMotion.SetUseMotionSettings(NotificationCard card, bool value)`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L36) | 设置附加属性。使用库的共享动效配置；全局关闭或目标控件关闭时不强制播放。 |
| [`static TimeSpan NotificationMotion.GetEnterDuration(NotificationCard card)`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L37) | 读取附加属性。通知入场持续时间；遵守共享动效开关。 |
| [`static void NotificationMotion.SetEnterDuration(NotificationCard card, TimeSpan value)`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L38) | 设置附加属性。通知入场持续时间；遵守共享动效开关。 |
| [`static TimeSpan NotificationMotion.GetExitDuration(NotificationCard card)`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L39) | 读取附加属性。通知退场持续时间；遵守共享动效开关。 |
| [`static void NotificationMotion.SetExitDuration(NotificationCard card, TimeSpan value)`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L40) | 设置附加属性。通知退场持续时间；遵守共享动效开关。 |
| [`NotificationMotion.NotificationMotion()`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L22) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<bool> NotificationMotion.UseMotionSettingsProperty`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<TimeSpan> NotificationMotion.EnterDurationProperty`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<TimeSpan> NotificationMotion.ExitDurationProperty`](../../../TheOne.Avalonia/Controls/NotificationMotion.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
