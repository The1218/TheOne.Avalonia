# ChartAnimationStyle

[English](../en/TheOne.Avalonia.Controls.ChartAnimationStyle.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartAnimationStyle.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartAnimationStyle` · Enum · `System.Enum`

Controls the presentation of chart data changes without changing target data or hit testing.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L4)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ChartAnimationStyle.ChartAnimationStyle()`](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L4) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 枚举值

| 完整签名／值 | 说明 |
| --- | --- |
| [`Auto = 0`](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L7) | Reveal the first frame; interpolate compatible data changes, otherwise crossfade. |
| [`None = 1`](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L9) | Present the complete target frame immediately. |
| [`Fade = 2`](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L11) | Fade in the prepared target plot while keeping its geometry cached. |
| [`Reveal = 3`](../../../TheOne.Avalonia/Controls/ChartAnimationStyle.cs#L13) | Reveal the prepared target plot from left to right. |
