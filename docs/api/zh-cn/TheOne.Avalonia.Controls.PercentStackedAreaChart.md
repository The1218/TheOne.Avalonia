# PercentStackedAreaChart

[English](../en/TheOne.Avalonia.Controls.PercentStackedAreaChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.PercentStackedAreaChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.PercentStackedAreaChart` · Class · `TheOne.Avalonia.Controls.CartesianChartBase`

[继承的额外成员: CartesianChartBase](TheOne.Avalonia.Controls.CartesianChartBase.md)

A fixed PercentStackedArea chart control with its own theme and visual state.

[Source / 声明](../../../TheOne.Avalonia/Controls/PercentStackedAreaChart.cs#L4)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PercentStackedAreaChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`PercentStackedAreaChart.PercentStackedAreaChart()`](../../../TheOne.Avalonia/Controls/PercentStackedAreaChart.cs#L4) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
