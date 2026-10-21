# SpecializedChartBase

[English](../en/TheOne.Avalonia.Controls.SpecializedChartBase.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.SpecializedChartBase.md) · [API index](README.md)

`TheOne.Avalonia.Controls.SpecializedChartBase` · Class · `TheOne.Avalonia.Controls.ChartBase`

[继承的额外成员: ChartBase](TheOne.Avalonia.Controls.ChartBase.md)

Shared direct-drawing infrastructure for non-Cartesian statistical charts.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartBase.Specialized.cs#L54)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |
