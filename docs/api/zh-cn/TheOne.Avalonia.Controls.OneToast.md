# OneToast

[English](../en/TheOne.Avalonia.Controls.OneToast.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneToast.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneToast` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/OneMessages.cs#L154)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L159) | `Avalonia.Controls.Window?` | 可写; CLR / n/a | `accessor-defined / state` | `child.Show(parent)` / `child.ShowDialog<T>(parent)` 建立的原生窗口所属关系，影响平台窗口生命周期。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static void OneToast.Show(string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L165) | UI 线程。 |
| [`static void OneToast.Show(INotification notification, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L175) | Shows the notification's message and status icon, ignoring Title, while retaining expiration and click/close callbacks. |
| [`static Task OneToast.ShowAsync(string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L183) | 内部调度至 UI 线程。 |
| [`static void OneToast.CloseAll(Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L189) | UI 线程。 |
