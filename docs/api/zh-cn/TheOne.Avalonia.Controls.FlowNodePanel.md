# FlowNodePanel

[English](../en/TheOne.Avalonia.Controls.FlowNodePanel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowNodePanel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowNodePanel` · Class · `Avalonia.Controls.Panel`

Positions a marker above horizontal content or to the left of vertical content.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L95)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentSpacing`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L114) | `double` | 可写; OneWay | `12` | 10 / 12 / 16 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L108) | `Avalonia.Layout.Orientation` | 可写; OneWay | `Orientation.Horizontal` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`FlowNodePanel.FlowNodePanel()`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L95) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<Orientation> FlowNodePanel.OrientationProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L97) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> FlowNodePanel.ContentSpacingProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L100) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
