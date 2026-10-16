# FlowNode

[English](../en/TheOne.Avalonia.Controls.FlowNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowNode.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowNode` · Class · `Avalonia.Controls.ContentControl`

A flow marker with independently templated content and an explicit business state.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowNode.cs#L17)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentSpacing`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L40) | `double` | 可写; OneWay | `12` | 10 / 12 / 16 校验：`value => double.IsFinite(value) && value >= 0` |
| [`DefaultContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L42) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | Fallback for FlowNodeContent; OneTheme supplies the default four-field template. |
| [`EffectiveContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L43) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; OneWay | `computed / state` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`EffectiveStateText`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L44) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IndicatorSize`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L39) | `double` | 可写; OneWay | `30` | 24 / 30 / 36 校验：`value => double.IsFinite(value) && value > 0` |
| [`IsCompleted`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L46) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsInProgress`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L47) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsVertical`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L45) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L38) | `Avalonia.Layout.Orientation` | 可写; OneWay; 向后代继承 | `Orientation.Horizontal` | 横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |
| [`Status`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L37) | `TheOne.Avalonia.Controls.FlowNodeStatus` | 可写; OneWay | `FlowNodeStatus.Pending` | 标记 校验：`Enum.IsDefined` 可选值：`Pending`, `InProgress`, `Completed` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`FlowNode.FlowNode()`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L36) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<FlowNodeStatus> FlowNode.StatusProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Orientation> FlowNode.OrientationProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> FlowNode.IndicatorSizeProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> FlowNode.ContentSpacingProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> FlowNode.DefaultContentTemplateProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<FlowNode, IDataTemplate?> FlowNode.EffectiveContentTemplateProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<FlowNode, string> FlowNode.EffectiveStateTextProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<FlowNode, bool> FlowNode.IsVerticalProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<FlowNode, bool> FlowNode.IsCompletedProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<FlowNode, bool> FlowNode.IsInProgressProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.FlowNodeContent`](TheOne.Avalonia.Controls.FlowNodeContent.md)
- [`TheOne.Avalonia.Controls.FlowNodeItem`](TheOne.Avalonia.Controls.FlowNodeItem.md)
- [`TheOne.Avalonia.Controls.FlowNodeStatus`](TheOne.Avalonia.Controls.FlowNodeStatus.md)
