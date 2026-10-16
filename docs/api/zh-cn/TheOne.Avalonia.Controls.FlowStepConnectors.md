# FlowStepConnectors

[English](../en/TheOne.Avalonia.Controls.FlowStepConnectors.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowStepConnectors.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowStepConnectors` · Class · `Avalonia.Controls.Control`

A non-interactive rail layer. Draws one line per adjacent visible node.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L9)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CompletedConnectorBrush`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L17) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 已完成步骤连接线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`ConnectorBrush`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L16) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 未完成步骤连接线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`ConnectorThickness`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L18) | `double` | 可写; OneWay | `2` | 连接线描边宽度，单位 DIP；不改变节点的业务状态。 校验：`value => double.IsFinite(value) && value >= 0` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override void FlowStepConnectors.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L19) | Avalonia 绘制回调；使用提供的 DrawingContext 绘制当前状态，业务代码通常不直接调用。 |
| [`FlowStepConnectors.FlowStepConnectors()`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L9) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> FlowStepConnectors.ConnectorBrushProperty`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> FlowStepConnectors.CompletedConnectorBrushProperty`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> FlowStepConnectors.ConnectorThicknessProperty`](../../../TheOne.Avalonia/Controls/FlowStepConnectors.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |
