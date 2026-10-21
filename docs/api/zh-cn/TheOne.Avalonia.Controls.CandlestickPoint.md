# CandlestickPoint

[English](../en/TheOne.Avalonia.Controls.CandlestickPoint.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CandlestickPoint.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CandlestickPoint` · Class · `TheOne.Avalonia.Controls.ChartPoint`

[继承的额外成员: ChartPoint](TheOne.Avalonia.Controls.ChartPoint.md)

OHLC observation at a numeric X coordinate. Close and inherited Y are the same value.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L40)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Close`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L55) | `double` | 可写; OneWay | `computed / state` | CandlestickPoint 的 Close 配置字段；值类型和初值如表，通过 CloseProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`High`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L57) | `double` | 可写; OneWay | `0 / default` | CandlestickPoint 的 High 配置字段；值类型和初值如表，通过 HighProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Low`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L56) | `double` | 可写; OneWay | `0 / default` | CandlestickPoint 的 Low 配置字段；值类型和初值如表，通过 LowProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Open`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L54) | `double` | 可写; OneWay | `0 / default` | CandlestickPoint 的 Open 配置字段；值类型和初值如表，通过 OpenProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`CandlestickPoint.CandlestickPoint()`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L48) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
| [`CandlestickPoint.CandlestickPoint(double x, double open, double close, double low, double high, string? label = null)`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L49) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<double> CandlestickPoint.OpenProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L42) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<CandlestickPoint, double> CandlestickPoint.CloseProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L43) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> CandlestickPoint.LowProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L45) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> CandlestickPoint.HighProperty`](../../../TheOne.Avalonia/Controls/ChartStatisticalData.cs#L46) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
