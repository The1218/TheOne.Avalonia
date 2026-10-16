# FlowStep

[English](../en/TheOne.Avalonia.Controls.FlowStep.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowStep.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowStep` · Class · `Avalonia.Controls.ItemsControl`

A linear, non-selecting flow display with native content containers.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowStep.cs#L11)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L38) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`MinNodeWidth`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L37) | `double` | 可写; OneWay; 向后代继承 | `180` | 150 / 180 / 210 校验：`value => double.IsFinite(value) && value > 0` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L36) | `double` | 可写; OneWay; 向后代继承 | `24` | 20 / 24 / 32 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L35) | `Avalonia.Layout.Orientation` | 可写; OneWay; 向后代继承 | `Orientation.Horizontal` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`FlowStep.FlowStep()`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L26) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<Orientation> FlowStep.OrientationProperty`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> FlowStep.NodeSpacingProperty`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> FlowStep.MinNodeWidthProperty`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> FlowStep.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.FlowNodeContent`](TheOne.Avalonia.Controls.FlowNodeContent.md)
- [`TheOne.Avalonia.Controls.FlowNodeItem`](TheOne.Avalonia.Controls.FlowNodeItem.md)
- [`TheOne.Avalonia.Controls.FlowNodeStatus`](TheOne.Avalonia.Controls.FlowNodeStatus.md)
