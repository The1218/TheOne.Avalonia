# BoxPlotPoint

[English](../en/TheOne.Avalonia.Controls.BoxPlotPoint.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.BoxPlotPoint.md) · [API index](README.md)

`TheOne.Avalonia.Controls.BoxPlotPoint` · Class · `TheOne.Avalonia.Controls.ChartPoint`

[继承的额外成员: ChartPoint](TheOne.Avalonia.Controls.ChartPoint.md)

Precomputed five-number summary. Median and inherited Y refer to the same observable value.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`LowerQuartile`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L26) | `double` | 可写; OneWay | `0 / default` | BoxPlotPoint 的 LowerQuartile 配置字段；值类型和初值如表，通过 LowerQuartileProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Maximum`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L29) | `double` | 可写; OneWay | `0 / default` | BoxPlotPoint 的 Maximum 配置字段；值类型和初值如表，通过 MaximumProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Median`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L27) | `double` | 可写; OneWay | `computed / state` | BoxPlotPoint 的 Median 配置字段；值类型和初值如表，通过 MedianProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Minimum`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L25) | `double` | 可写; OneWay | `0 / default` | BoxPlotPoint 的 Minimum 配置字段；值类型和初值如表，通过 MinimumProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Outliers`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L30) | `System.Collections.Generic.IList<double>?` | 可写; OneWay | `null` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`UpperQuartile`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L28) | `double` | 可写; OneWay | `0 / default` | BoxPlotPoint 的 UpperQuartile 配置字段；值类型和初值如表，通过 UpperQuartileProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`BoxPlotPoint.BoxPlotPoint()`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L17) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
| [`BoxPlotPoint.BoxPlotPoint(double x, double minimum, double lowerQuartile, double median, double upperQuartile, double maximum, string? label = null)`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L18) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<double> BoxPlotPoint.MinimumProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L9) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> BoxPlotPoint.LowerQuartileProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L10) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<BoxPlotPoint, double> BoxPlotPoint.MedianProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> BoxPlotPoint.UpperQuartileProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> BoxPlotPoint.MaximumProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IList<double>?> BoxPlotPoint.OutliersProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
