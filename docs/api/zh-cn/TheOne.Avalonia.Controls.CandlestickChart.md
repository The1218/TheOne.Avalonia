# CandlestickChart

[English](../en/TheOne.Avalonia.Controls.CandlestickChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CandlestickChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CandlestickChart` · Class · `TheOne.Avalonia.Controls.StatisticalChartBase`

[继承的额外成员: StatisticalChartBase](TheOne.Avalonia.Controls.StatisticalChartBase.md)

Cached OHLC candles on numeric X coordinates, with theme-aware rise/fall colors.

[Source / 声明](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`DownBrush`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L18) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | CandlestickChart 的 DownBrush 配置字段；值类型和初值如表，通过 DownBrushProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`IsHollowUp`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L19) | `bool` | 可写; OneWay | `false` | CandlestickChart 的 IsHollowUp 配置字段；值类型和初值如表，通过 IsHollowUpProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Kind`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L16) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`UpBrush`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L17) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | CandlestickChart 的 UpBrush 配置字段；值类型和初值如表，通过 UpBrushProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`CandlestickChart.CandlestickChart()`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L7) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> CandlestickChart.UpBrushProperty`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L9) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> CandlestickChart.DownBrushProperty`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L10) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> CandlestickChart.IsHollowUpProperty`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
