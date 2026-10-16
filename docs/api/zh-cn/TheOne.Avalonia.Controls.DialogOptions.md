# DialogOptions

[English](../en/TheOne.Avalonia.Controls.DialogOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DialogOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DialogOptions` · Class · `object`

Immutable settings for a modal dialog inside OneWindow.

[Source / 声明](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L4)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Buttons`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L14) | `TheOne.Avalonia.Controls.OneDialogButtons` | 可写; CLR / init-only | `OneDialogButtons.OkCancel` | 对话框按钮配置集合／标志；由选项类型和关闭结果契约解释。 可选值：`Ok`, `OkCancel`, `YesNo`, `YesNoCancel` |
| [`CloseOnEscape`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L12) | `bool` | 可写; CLR / init-only | `true` | 允许 Esc 关闭对话框；结果与取消语义按对话框选项处理。 |
| [`CloseOnOverlayClick`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L11) | `bool` | 可写; CLR / init-only | `false` | 允许点击遮罩关闭弹窗；取消／返回结果遵循对话框选项。 |
| [`MotionEnabled`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L7) | `bool?` | 可写; CLR / init-only | `null` | 该弹窗的动效开关；同时遵守继承的 Motion.Enabled。 |
| [`ShowButtons`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L13) | `bool` | 可写; CLR / init-only | `false` | 显示默认按钮区；自定义内容中的按钮不受此开关控制。 |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L10) | `bool` | 可写; CLR / init-only | `true` | 显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`ShowTitle`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L9) | `bool` | 可写; CLR / init-only | `true` | 显示对话框标题区；标题值仍保留在选项中。 |
| [`Size`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L6) | `TheOne.Avalonia.Styling.ControlSize?` | 可写; CLR / init-only | `null` | 对话框尺寸枚举；采用库提供的尺寸档位。 可选值：`Small`, `Normal`, `Large` |
| [`Title`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L8) | `string?` | 可写; CLR / init-only | `null` | 显示标题；不等于业务主键或本地化资源键。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`DialogOptions.DialogOptions()`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L4) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.OneDialogButtons`](TheOne.Avalonia.Controls.OneDialogButtons.md)
- [`TheOne.Avalonia.Styling.ControlSize`](TheOne.Avalonia.Styling.ControlSize.md)
