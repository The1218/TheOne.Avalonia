# DataGridSummaryPresenter

[English](../en/TheOne.Avalonia.Controls.DataGridSummaryPresenter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridSummaryPresenter.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridSummaryPresenter` · Class · `Avalonia.Controls.Control`

One drawn footer, aligned and clipped with native DataGrid column geometry.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L14)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L22) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 背景画刷；主题相关背景使用 DynamicResource，避免固定颜色破坏明暗切换。 |
| [`Foreground`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L23) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 前景画刷；自定义时保持正文、状态与禁用对比度。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override void DataGridSummaryPresenter.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L67) | Avalonia 绘制回调；使用提供的 DrawingContext 绘制当前状态，业务代码通常不直接调用。 |
| [`DataGridSummaryPresenter.DataGridSummaryPresenter()`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L24) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> DataGridSummaryPresenter.LineBrushProperty`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> DataGridSummaryPresenter.BackgroundProperty`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> DataGridSummaryPresenter.ForegroundProperty`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
