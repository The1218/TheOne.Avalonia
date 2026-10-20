# LoginWindow

[English](../en/TheOne.Avalonia.Controls.LoginWindow.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.LoginWindow.md) · [API index](README.md)

`TheOne.Avalonia.Controls.LoginWindow` · Class · `Avalonia.Controls.Window`

A themed native Window whose caller-owned content performs authentication.

[Source / 声明](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L13)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L35) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void LoginWindow.ShowErrorToast(string message, TimeSpan? expiration = null)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L81) | Shows an error toast in this login window on the UI thread. |
| [`void LoginWindow.ShowErrorNotification(string message, string title = "登录失败", TimeSpan? expiration = null)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L87) | Shows an error notification in this login window on the UI thread. |
| [`Task<OneDialogResult> LoginWindow.ShowErrorDialog(string message, string title = "登录失败", CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L93) | Shows an error dialog in this login window; dispatches to the UI thread. |
| [`bool LoginWindow.CompleteLogin(OneWindow mainWindow)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L107) | Installs the supplied fresh, unopened OneWindow as the desktop MainWindow and all message owners, then closes login. |
| [`Task<bool> LoginWindow.CompleteLoginAsync(OneWindow mainWindow)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L114) | Dispatches the handoff of the supplied OneWindow to the UI thread. Construct the window on the UI thread. |
| [`bool LoginWindow.CompleteLogin(Func<OneWindow> mainWindowFactory)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L130) | After successful authentication, replaces this desktop MainWindow with a fresh OneWindow. Returns false if Closing cancels the transition or a completion is already in progress. Repeated calls after success return true without invoking the factory again. |
| [`Task<bool> LoginWindow.CompleteLoginAsync(Func<OneWindow> mainWindowFactory)`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L221) | Dispatches completion, including window construction, to the UI thread. |
| [`LoginWindow.LoginWindow()`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L27) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly DirectProperty<LoginWindow, bool> LoginWindow.IsDialogOpenProperty`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.OneDialogResult`](TheOne.Avalonia.Controls.OneDialogResult.md)
- [`TheOne.Avalonia.Controls.OneWindow`](TheOne.Avalonia.Controls.OneWindow.md)
