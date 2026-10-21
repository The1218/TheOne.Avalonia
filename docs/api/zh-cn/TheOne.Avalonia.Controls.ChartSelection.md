# ChartSelection

[English](../en/TheOne.Avalonia.Controls.ChartSelection.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartSelection.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartSelection` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartData.cs#L155)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Point`](../../../TheOne.Avalonia/Controls/ChartData.cs#L155) | `TheOne.Avalonia.Controls.ChartPoint` | 可写; CLR / init-only | `required constructor argument` | 命中的原始图表数据点，供选择事件和业务详情读取。 |
| [`PointIndex`](../../../TheOne.Avalonia/Controls/ChartData.cs#L155) | `int` | 可写; CLR / init-only | `required constructor argument` | 选中数据点在所属序列中的零基索引。 |
| [`Series`](../../../TheOne.Avalonia/Controls/ChartData.cs#L155) | `TheOne.Avalonia.Controls.ChartSeries` | 可写; CLR / init-only | `required constructor argument` | 命中的原始图表序列模型。 |
| [`SeriesIndex`](../../../TheOne.Avalonia/Controls/ChartData.cs#L155) | `int` | 可写; CLR / init-only | `required constructor argument` | 选中序列在源集合中的零基索引。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ChartSelection.ChartSelection(int SeriesIndex, int PointIndex, ChartSeries Series, ChartPoint Point)`](../../../TheOne.Avalonia/Controls/ChartData.cs#L155) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.ChartPoint`](TheOne.Avalonia.Controls.ChartPoint.md)
- [`TheOne.Avalonia.Controls.ChartSeries`](TheOne.Avalonia.Controls.ChartSeries.md)
