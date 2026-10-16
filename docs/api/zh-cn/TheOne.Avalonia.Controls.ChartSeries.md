# ChartSeries

[English](../en/TheOne.Avalonia.Controls.ChartSeries.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartSeries.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartSeries` · Class · `Avalonia.AvaloniaObject`

One series. Observable collections and point properties update attached charts automatically.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartData.cs#L27)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Brush`](../../../TheOne.Avalonia/Controls/ChartData.cs#L39) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 当前序列／分段画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`IsVisible`](../../../TheOne.Avalonia/Controls/ChartData.cs#L40) | `bool` | 可写; OneWay | `true` | 是否参与该序列的绘制和可见数据范围计算。 |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/ChartData.cs#L38) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartPoint>?` | 可写; OneWay | `null` | 绑定原始模型集合，不预建大量 Control；动态增删使用带通知的集合，并提供有限视口。 |
| [`Name`](../../../TheOne.Avalonia/Controls/ChartData.cs#L37) | `string` | 可写; OneWay | `"Series"` | 名称字段，用于业务展示或标识；与显示模板、键值字段保持一致。 |
| [`Unit`](../../../TheOne.Avalonia/Controls/ChartData.cs#L44) | `string?` | 可写; OneWay | `null` | Optional tooltip/data-label unit; null inherits the assigned chart axis unit. |
| [`ValueFormat`](../../../TheOne.Avalonia/Controls/ChartData.cs#L46) | `string?` | 可写; OneWay | `null` | Optional raw-value numeric format; null inherits the assigned chart axis format. |
| [`YAxis`](../../../TheOne.Avalonia/Controls/ChartData.cs#L42) | `TheOne.Avalonia.Controls.ChartYAxis` | 可写; OneWay | `ChartYAxis.Primary` | The independent value scale used by this series in Cartesian charts. 校验：`value => value is ChartYAxis.Primary or ChartYAxis.Secondary` 可选值：`Primary`, `Secondary` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ChartSeries.ChartSeries()`](../../../TheOne.Avalonia/Controls/ChartData.cs#L47) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string> ChartSeries.NameProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IList<ChartPoint>?> ChartSeries.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> ChartSeries.BrushProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> ChartSeries.IsVisibleProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ChartYAxis> ChartSeries.YAxisProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> ChartSeries.UnitProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> ChartSeries.ValueFormatProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L36) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.ChartPoint`](TheOne.Avalonia.Controls.ChartPoint.md)
- [`TheOne.Avalonia.Controls.ChartYAxis`](TheOne.Avalonia.Controls.ChartYAxis.md)
