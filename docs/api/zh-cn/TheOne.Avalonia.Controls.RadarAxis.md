# RadarAxis

[English](../en/TheOne.Avalonia.Controls.RadarAxis.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RadarAxis.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RadarAxis` · Class · `Avalonia.AvaloniaObject`

The name and independent scale of a radar dimension.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L27)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Maximum`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L48) | `double` | 可写; OneWay | `100` | RadarAxis 的 Maximum 配置字段；值类型和初值如表，通过 MaximumProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 校验：`double.IsFinite` |
| [`Minimum`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L42) | `double` | 可写; OneWay | `0 / default` | RadarAxis 的 Minimum 配置字段；值类型和初值如表，通过 MinimumProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 校验：`double.IsFinite` |
| [`Name`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L36) | `string` | 可写; OneWay | `"Axis"` | 名称字段，用于业务展示或标识；与显示模板、键值字段保持一致。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`RadarAxis.RadarAxis()`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L27) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string> RadarAxis.NameProperty`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RadarAxis.MinimumProperty`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RadarAxis.MaximumProperty`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
