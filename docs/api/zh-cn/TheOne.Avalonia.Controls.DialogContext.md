# DialogContext

[English](../en/TheOne.Avalonia.Controls.DialogContext.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DialogContext.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DialogContext` · Class · `object`

Optional base for dialog view models. Does not depend on a window or dispatcher.

[Source / 声明](../../../TheOne.Avalonia/Controls/DialogContext.cs#L12)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void DialogContext.RequestClose(OneDialogResult result = OneDialogResult.Ok)`](../../../TheOne.Avalonia/Controls/DialogContext.cs#L20) | Requests completion of this dialog, including when it is still queued. |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event PropertyChangedEventHandler? DialogContext.PropertyChanged`](../../../TheOne.Avalonia/Controls/DialogContext.cs#L16) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`event EventHandler<DialogCloseRequestedEventArgs>? DialogContext.CloseRequested`](../../../TheOne.Avalonia/Controls/DialogContext.cs#L17) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs`](TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs.md)
- [`TheOne.Avalonia.Controls.OneDialogResult`](TheOne.Avalonia.Controls.OneDialogResult.md)
