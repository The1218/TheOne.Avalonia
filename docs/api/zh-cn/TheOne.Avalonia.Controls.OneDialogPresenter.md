# OneDialogPresenter

[English](../en/TheOne.Avalonia.Controls.OneDialogPresenter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneDialogPresenter.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneDialogPresenter` · Class · `Avalonia.Controls.Primitives.HeaderedContentControl`

[Source / 声明](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Buttons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L44) | `TheOne.Avalonia.Controls.OneDialogButtons` | 可写; OneWay | `0 / default` | 对话框按钮配置集合／标志；由选项类型和关闭结果契约解释。 可选值：`Ok`, `OkCancel`, `YesNo`, `YesNoCancel` |
| [`CloseOnEscape`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L68) | `bool` | 可写; OneWay | `true` | 允许 Esc 关闭对话框；结果与取消语义按对话框选项处理。 |
| [`ShowButtons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L62) | `bool` | 可写; OneWay | `true` | 显示默认按钮区；自定义内容中的按钮不受此开关控制。 |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L56) | `bool` | 可写; OneWay | `true` | 显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`ShowTitle`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L50) | `bool` | 可写; OneWay | `true` | 显示对话框标题区；标题值仍保留在选项中。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`OneDialogPresenter.OneDialogPresenter()`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L29) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<OneDialogButtons> OneDialogPresenter.ButtonsProperty`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> OneDialogPresenter.ShowTitleProperty`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> OneDialogPresenter.ShowCloseButtonProperty`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> OneDialogPresenter.ShowButtonsProperty`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> OneDialogPresenter.CloseOnEscapeProperty`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.OneDialogButtons`](TheOne.Avalonia.Controls.OneDialogButtons.md)
