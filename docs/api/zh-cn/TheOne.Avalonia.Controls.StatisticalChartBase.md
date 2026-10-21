# StatisticalChartBase

[English](../en/TheOne.Avalonia.Controls.StatisticalChartBase.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.StatisticalChartBase.md) · [API index](README.md)

`TheOne.Avalonia.Controls.StatisticalChartBase` · Class · `TheOne.Avalonia.Controls.SpecializedChartBase`

[继承的额外成员: SpecializedChartBase](TheOne.Avalonia.Controls.SpecializedChartBase.md)

Cached numeric axes, viewport navigation and indexed hits for range-valued observations.

[Source / 声明](../../../TheOne.Avalonia/Controls/StatisticalChartBase.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`PointWidthRatio`](../../../TheOne.Avalonia/Controls/StatisticalChartBase.cs#L21) | `double` | 可写; OneWay | `.6` | StatisticalChartBase 的 PointWidthRatio 配置字段；值类型和初值如表，通过 PointWidthRatioProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 校验：`value => double.IsFinite(value) && value > 0 && value <= 1` |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<double> StatisticalChartBase.PointWidthRatioProperty`](../../../TheOne.Avalonia/Controls/StatisticalChartBase.cs#L9) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |
