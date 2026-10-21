# HeatmapChart

[English](../en/TheOne.Avalonia.Controls.HeatmapChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.HeatmapChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.HeatmapChart` · Class · `TheOne.Avalonia.Controls.SpecializedChartBase`

[继承的额外成员: SpecializedChartBase](TheOne.Avalonia.Controls.SpecializedChartBase.md)

A native continuous-color heatmap with coordinate-based cells and cached color batches.

[Source / 声明](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CellGap`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L47) | `double` | 可写; OneWay | `2` | The visual gap between cells, in logical pixels. 校验：`value => double.IsFinite(value) && value >= 0` |
| [`CellHeight`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L45) | `double` | 可写; OneWay | `1` | The cell height in Y coordinate units. 校验：`value => double.IsFinite(value) && value > 0` |
| [`CellWidth`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L43) | `double` | 可写; OneWay | `1` | The cell width in X coordinate units. 校验：`value => double.IsFinite(value) && value > 0` |
| [`HighColor`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L52) | `Avalonia.Media.Color?` | 可写; OneWay | `null` | HeatmapChart 的 HighColor 配置字段；值类型和初值如表，通过 HighColorProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Kind`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L38) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`LowColor`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L50) | `Avalonia.Media.Color?` | 可写; OneWay | `null` | HeatmapChart 的 LowColor 配置字段；值类型和初值如表，通过 LowColorProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`MidColor`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L51) | `Avalonia.Media.Color?` | 可写; OneWay | `null` | HeatmapChart 的 MidColor 配置字段；值类型和初值如表，通过 MidColorProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`ValueMaximum`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L49) | `double` | 可写; OneWay | `double.NaN` | HeatmapChart 的 ValueMaximum 配置字段；值类型和初值如表，通过 ValueMaximumProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 校验：`value => !double.IsInfinity(value)` |
| [`ValueMinimum`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L48) | `double` | 可写; OneWay | `double.NaN` | HeatmapChart 的 ValueMinimum 配置字段；值类型和初值如表，通过 ValueMinimumProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 校验：`value => !double.IsInfinity(value)` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`HeatmapChart.HeatmapChart()`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L7) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<double> HeatmapChart.CellWidthProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L9) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> HeatmapChart.CellHeightProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> HeatmapChart.CellGapProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> HeatmapChart.ValueMinimumProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> HeatmapChart.ValueMaximumProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Color?> HeatmapChart.LowColorProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Color?> HeatmapChart.MidColorProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Color?> HeatmapChart.HighColorProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
