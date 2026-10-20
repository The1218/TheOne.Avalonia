# Timeline

[English](../en/TheOne.Avalonia.Controls.Timeline.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Timeline.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Timeline` · Class · `Avalonia.Controls.ItemsControl`

A naturally sized vertical event history. Collection order is caller-owned.

[Source / 声明](../../../TheOne.Avalonia/Controls/Timeline.cs#L15)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ItemSpacing`](../../../TheOne.Avalonia/Controls/Timeline.cs#L34) | `double` | 可写; OneWay; 向后代继承 | `16` | 16 DIP ; 继承到 TimelineItem 的下方留白，有限非负，可逐项覆盖；主题三档值为 12/16/20 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Mode`](../../../TheOne.Avalonia/Controls/Timeline.cs#L30) | `TheOne.Avalonia.Controls.TimelineMode` | 可写; OneWay | `0 / default` | Left ; Left：轴在左、内容在右；Right：轴在右、内容在左；Alternate：轴居中，按视觉顺序交错，首项内容在右 校验：`Enum.IsDefined` 可选值：`Left`, `Right`, `Alternate` |
| [`Reverse`](../../../TheOne.Avalonia/Controls/Timeline.cs#L32) | `bool` | 可写; OneWay; 向后代继承 | `false` | false ; 仅反转视觉排列，不修改数据源；交错方向与末项尾线随之更新 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Timeline.Timeline()`](../../../TheOne.Avalonia/Controls/Timeline.cs#L27) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<TimelineMode> Timeline.ModeProperty`](../../../TheOne.Avalonia/Controls/Timeline.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> Timeline.ReverseProperty`](../../../TheOne.Avalonia/Controls/Timeline.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> Timeline.ItemSpacingProperty`](../../../TheOne.Avalonia/Controls/Timeline.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.TimelineItem`](TheOne.Avalonia.Controls.TimelineItem.md)
- [`TheOne.Avalonia.Controls.TimelineItemModel`](TheOne.Avalonia.Controls.TimelineItemModel.md)
- [`TheOne.Avalonia.Controls.TimelineMode`](TheOne.Avalonia.Controls.TimelineMode.md)
- [`TheOne.Avalonia.Controls.TimelineStatus`](TheOne.Avalonia.Controls.TimelineStatus.md)
