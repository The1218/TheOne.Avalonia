# LoadingIndicator

[English](../en/TheOne.Avalonia.Controls.LoadingIndicator.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.LoadingIndicator.md) · [API index](README.md)

`TheOne.Avalonia.Controls.LoadingIndicator` · Class · `Avalonia.Controls.Control`

A lightweight spinner that becomes a static status glyph when motion is disabled.

[Source / 声明](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L14)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Foreground`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L19) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 前景画刷；自定义时保持正文、状态与禁用对比度。 |
| [`IsActive`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L20) | `bool` | 可写; OneWay | `true` | 指示器是否运行动画；关闭时保留控件布局，全局动效开关仍生效。 |
| [`IsSpinning`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L21) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override void LoadingIndicator.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L36) | Avalonia 绘制回调；使用提供的 DrawingContext 绘制当前状态，业务代码通常不直接调用。 |
| [`LoadingIndicator.LoadingIndicator()`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L26) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> LoadingIndicator.ForegroundProperty`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> LoadingIndicator.IsActiveProperty`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<LoadingIndicator, bool> LoadingIndicator.IsSpinningProperty`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
