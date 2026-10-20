# TimelineItemModel

[English](../en/TheOne.Avalonia.Controls.TimelineItemModel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TimelineItemModel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TimelineItemModel` · Class · `Avalonia.AvaloniaObject`

Optional observable event data; Key remains caller-owned identity. Timestamp is displayed, never sorted or formatted implicitly.

[Source / 声明](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L6)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Content`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L16) | `object?` | 可写; OneWay | `null` | 宿主内容或业务展示对象；具体模板作用域见内容模板契约，不把视图对象当作持久化数据。 |
| [`Dot`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L18) | `object?` | 可写; OneWay | `null` | null ; 自定义节点内容及类型模板；非 null 替代状态圆点，恢复 null 后显示当前 Status |
| [`Header`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L15) | `object?` | 可写; OneWay | `null` | 标题区内容，可为文本或模型；自定义显示由 HeaderTemplate 负责。 |
| [`Key`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L14) | `object?` | 可写; OneWay | `null` | 字段／参数键，用于关联业务值；与显示名称分开维护。 |
| [`Status`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L19) | `TheOne.Avalonia.Controls.TimelineStatus` | 可写; OneWay | `0 / default` | Default ; Default（强调色）、Success、Warning、Error、Pending；Pending 为静态空心节点，无循环动画 校验：`Enum.IsDefined` 可选值：`Default`, `Success`, `Warning`, `Error`, `Pending` |
| [`Timestamp`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L17) | `object?` | 可写; OneWay | `null` | null ; 时间内容及类型模板；null 隐藏。左右模式位于正文上方，交错模式位于轴另一侧 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`TimelineItemModel.TimelineItemModel()`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L6) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<object?> TimelineItemModel.KeyProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L8) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> TimelineItemModel.HeaderProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L9) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> TimelineItemModel.ContentProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L10) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> TimelineItemModel.TimestampProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> TimelineItemModel.DotProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TimelineStatus> TimelineItemModel.StatusProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.TimelineStatus`](TheOne.Avalonia.Controls.TimelineStatus.md)
