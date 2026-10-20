# OneMessageBox

[English](../en/TheOne.Avalonia.Controls.OneMessageBox.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneMessageBox.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneMessageBox` · Class · `object`

Simple, command-driven confirmation using the existing themed window dialog host.

[Source / 声明](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L19)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static Task<bool> OneMessageBox.ConfirmAsync(string title, string message, MessageBoxOptions? options = null, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L25) | Returns true only after explicit confirmation. Cancel, Escape and owner closure return false. Cancellation of cancels the returned task. |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.MessageBoxOptions`](TheOne.Avalonia.Controls.MessageBoxOptions.md)
