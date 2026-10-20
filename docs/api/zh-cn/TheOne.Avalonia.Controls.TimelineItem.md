# TimelineItem

[English](../en/TheOne.Avalonia.Controls.TimelineItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TimelineItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TimelineItem` · Class · `Avalonia.Controls.Primitives.HeaderedContentControl`

An event with a title, body, timestamp and semantic or custom marker.

[Source / 声明](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L13)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Dot`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L30) | `object?` | 可写; OneWay | `null` | null ; 自定义节点内容及类型模板；非 null 替代状态圆点，恢复 null 后显示当前 Status |
| [`DotTemplate`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L31) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | null ; 自定义节点内容及类型模板；非 null 替代状态圆点，恢复 null 后显示当前 Status |
| [`ItemSpacing`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L33) | `double` | 可写; OneWay; 向后代继承 | `16` | 16 DIP ; 继承到 TimelineItem 的下方留白，有限非负，可逐项覆盖；主题三档值为 12/16/20 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Status`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L32) | `TheOne.Avalonia.Controls.TimelineStatus` | 可写; OneWay | `0 / default` | Default ; Default（强调色）、Success、Warning、Error、Pending；Pending 为静态空心节点，无循环动画 校验：`Enum.IsDefined` 可选值：`Default`, `Success`, `Warning`, `Error`, `Pending` |
| [`Timestamp`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L28) | `object?` | 可写; OneWay | `null` | null ; 时间内容及类型模板；null 隐藏。左右模式位于正文上方，交错模式位于轴另一侧 |
| [`TimestampTemplate`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L29) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | null ; 时间内容及类型模板；null 隐藏。左右模式位于正文上方，交错模式位于轴另一侧 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`TimelineItem.TimelineItem()`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L27) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<object?> TimelineItem.TimestampProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> TimelineItem.TimestampTemplateProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> TimelineItem.DotProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> TimelineItem.DotTemplateProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TimelineStatus> TimelineItem.StatusProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> TimelineItem.ItemSpacingProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.TimelineItemModel`](TheOne.Avalonia.Controls.TimelineItemModel.md)
- [`TheOne.Avalonia.Controls.TimelineStatus`](TheOne.Avalonia.Controls.TimelineStatus.md)
