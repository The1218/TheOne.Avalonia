# OneNotification

[English](../en/TheOne.Avalonia.Controls.OneNotification.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneNotification.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneNotification` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/OneMessages.cs#L82)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L86) | `Avalonia.Controls.Window?` | 可写; CLR / n/a | `accessor-defined / state` | `child.Show(parent)` / `child.ShowDialog<T>(parent)` 建立的原生窗口所属关系，影响平台窗口生命周期。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static void OneNotification.Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L88) | Shows on the UI thread. The owner is optional; the registered default owner or most recently activated visible themed window is used. |
| [`static void OneNotification.Show(INotification notification, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L90) | UI 线程。 |
| [`static Task OneNotification.ShowAsync(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L95) | 内部调度至 UI 线程。 |
| [`static void OneNotification.CloseAll(Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L97) | UI 线程。 |
