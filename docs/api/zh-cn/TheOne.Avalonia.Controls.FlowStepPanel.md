# FlowStepPanel

[English](../en/TheOne.Avalonia.Controls.FlowStepPanel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowStepPanel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowStepPanel` · Class · `Avalonia.Controls.Panel`

Equal horizontal slots or natural vertical rows, with rails between actual markers.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`MinNodeWidth`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L23) | `double` | 可写; OneWay; 向后代继承 | `180` | 150 / 180 / 210 校验：`value => double.IsFinite(value) && value > 0` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L22) | `double` | 可写; OneWay; 向后代继承 | `24` | 20 / 24 / 32 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L21) | `Avalonia.Layout.Orientation` | 可写; OneWay; 向后代继承 | `Orientation.Horizontal` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |
| [`ViewportWidth`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L24) | `double` | 可写; OneWay | `0` | 宿主提供给流程布局的可用宽度；用于节点排布，通常由模板维护。 校验：`value => double.IsFinite(value) && value >= 0` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`FlowStepPanel.FlowStepPanel()`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L10) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<Orientation> FlowStepPanel.OrientationProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> FlowStepPanel.NodeSpacingProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> FlowStepPanel.MinNodeWidthProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> FlowStepPanel.ViewportWidthProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
