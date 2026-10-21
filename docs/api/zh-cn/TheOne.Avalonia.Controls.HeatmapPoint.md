# HeatmapPoint

[English](../en/TheOne.Avalonia.Controls.HeatmapPoint.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.HeatmapPoint.md) · [API index](README.md)

`TheOne.Avalonia.Controls.HeatmapPoint` · Class · `TheOne.Avalonia.Controls.ChartPoint`

[继承的额外成员: ChartPoint](TheOne.Avalonia.Controls.ChartPoint.md)

A heatmap cell. X and Y locate the cell; Value controls its color.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L6)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Value`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L19) | `double` | 可写; OneWay | `0 / default` | Non-finite values leave a gap without changing the cell coordinates. |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`HeatmapPoint.HeatmapPoint()`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L11) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
| [`HeatmapPoint.HeatmapPoint(double x, double y, double value, string? label = null)`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L13) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<double> HeatmapPoint.ValueProperty`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L8) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
