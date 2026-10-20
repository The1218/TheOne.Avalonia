# GaugeSegment

[English](../en/TheOne.Avalonia.Controls.GaugeSegment.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GaugeSegment.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GaugeSegment` · Class · `Avalonia.AvaloniaObject`

An ordered band ending at Fraction of the gauge range. Labels are used by Grade.

[Source / 声明](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L37)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Brush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L55) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 当前序列／分段画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`Fraction`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L49) | `double` | 可写; OneWay | `1` | 仪表分段占比；配合其他分段定义绘制范围，不以像素宽度代替数值。 校验：`value => double.IsFinite(value) && value > 0 && value <= 1` |
| [`Label`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L61) | `string?` | 可写; OneWay | `null` | 显示标签文本；不作为模型身份、字段路径或序列化标识。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`GaugeSegment.GaugeSegment()`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L37) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<double> GaugeSegment.FractionProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L39) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GaugeSegment.BrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L43) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> GaugeSegment.LabelProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L46) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
