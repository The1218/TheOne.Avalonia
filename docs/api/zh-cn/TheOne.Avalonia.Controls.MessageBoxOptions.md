# MessageBoxOptions

[English](../en/TheOne.Avalonia.Controls.MessageBoxOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.MessageBoxOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.MessageBoxOptions` · Class · `object`

Presentation options for a simple confirm/cancel message box.

[Source / 声明](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CancelText`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L11) | `string` | 可写; CLR / init-only | `"取消"` | 取消按钮的本地化文本；不改变取消返回值。 |
| [`ConfirmText`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L10) | `string` | 可写; CLR / init-only | `"确认"` | 确认按钮的本地化文本；不自动执行宿主业务操作。 |
| [`IsDangerous`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L12) | `bool` | 可写; CLR / init-only | `false` | 将确认操作显示为危险语义样式；授权和执行仍由业务决定。 |
| [`Size`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L14) | `TheOne.Avalonia.Styling.ControlSize?` | 可写; CLR / init-only | `null` | Overrides the owner window's inherited density when specified. 可选值：`Small`, `Normal`, `Large` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`MessageBoxOptions.MessageBoxOptions()`](../../../TheOne.Avalonia/Controls/OneMessageBox.cs#L8) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Styling.ControlSize`](TheOne.Avalonia.Styling.ControlSize.md)
