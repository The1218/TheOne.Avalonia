# ChartPoint

[English](../en/TheOne.Avalonia.Controls.ChartPoint.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartPoint.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartPoint` · Class · `Avalonia.AvaloniaObject`

A mutable, observable data point. Non-finite coordinates are treated as gaps.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartData.cs#L41)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Label`](../../../TheOne.Avalonia/Controls/ChartData.cs#L72) | `string?` | 可写; OneWay | `null` | 显示标签文本；不作为模型身份、字段路径或序列化标识。 |
| [`X`](../../../TheOne.Avalonia/Controls/ChartData.cs#L60) | `double` | 可写; OneWay | `0 / default` | 模型横向坐标／数值；按所属图表、流程或报表坐标系统解释。 |
| [`Y`](../../../TheOne.Avalonia/Controls/ChartData.cs#L66) | `double` | 可写; OneWay | `0 / default` | 模型纵向坐标／数值；按所属图表、流程或报表坐标系统解释。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ChartPoint.ChartPoint()`](../../../TheOne.Avalonia/Controls/ChartData.cs#L49) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
| [`ChartPoint.ChartPoint(double x, double y, string? label = null)`](../../../TheOne.Avalonia/Controls/ChartData.cs#L53) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<double> ChartPoint.XProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L43) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> ChartPoint.YProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L44) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> ChartPoint.LabelProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L46) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
