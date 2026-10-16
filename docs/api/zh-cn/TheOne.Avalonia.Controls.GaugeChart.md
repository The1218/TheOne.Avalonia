# GaugeChart

[English](../en/TheOne.Avalonia.Controls.GaugeChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GaugeChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GaugeChart` · Class · `TheOne.Avalonia.Controls.ChartBase`

[继承的额外成员: ChartBase](TheOne.Avalonia.Controls.ChartBase.md)

A directly drawn gauge with enum-selected layouts and property-based visual overrides.

[Source / 声明](../../../TheOne.Avalonia/Controls/GaugeChart.GradeLabels.cs#L6)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`GaugeCaption`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L74) | `string?` | 可写; OneWay | `null` | 说明文字默认 null；说明顶端的垂直偏移为半径比例 `[−1, 1]`，NaN 随枚举，与顶部 Title 分开 |
| [`GaugeCaptionOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L69) | `double` | 可写; OneWay | `double.NaN` | 说明文字默认 null；说明顶端的垂直偏移为半径比例 `[−1, 1]`，NaN 随枚举，与顶部 Title 分开 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`GaugeGradeLabelGap`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L73) | `double` | 可写; OneWay | `double.NaN` | 等级标签默认沿切线旋转并保持可读；旋转角 NaN 自动、`[−90, 90]` 显式角度（0 水平）；标签到色带的间距 NaN 随字号、`[0, 100]` 显式像素 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= 0 && value <= 100)` |
| [`GaugeGradeLabelRotation`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L71) | `double` | 可写; OneWay | `double.NaN` | 等级标签默认沿切线旋转并保持可读；旋转角 NaN 自动、`[−90, 90]` 显式角度（0 水平）；标签到色带的间距 NaN 随字号、`[0, 100]` 显式像素 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -90 && value <= 90)` |
| [`GaugeNeedleBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L76) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 显式覆盖指针、主刻度和中心数值颜色；未设置时使用主题或所属色带颜色 |
| [`GaugeNeedleLength`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L64) | `double` | 可写; OneWay | `0.72` | 长度默认半径的 0.72，范围 `(0, 1]`；宽度 NaN 随形状，显式像素值 `(0, 50]` 校验：`value => double.IsFinite(value) && value > 0 && value <= 1` |
| [`GaugeNeedleShape`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L62) | `TheOne.Avalonia.Controls.GaugeNeedleShape` | 可写; OneWay | `GaugeNeedleShape.Auto` | Auto 随枚举；Triangle / Line / Marker 单独覆盖指针形状 校验：`Enum.IsDefined` 可选值：`Auto`, `Triangle`, `Line`, `Marker` |
| [`GaugeNeedleWidth`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L65) | `double` | 可写; OneWay | `double.NaN` | 长度默认半径的 0.72，范围 `(0, 1]`；宽度 NaN 随形状，显式像素值 `(0, 50]` 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 50)` |
| [`GaugeSecondaryBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L83) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 气压表副刻度数默认 7（范围 `[2, 12]`），颜色默认 Foreground |
| [`GaugeSecondaryMaximum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L81) | `double` | 可写; OneWay | `60` | 气压表副刻度范围，默认 0–60；不改变主值或指针所代表的范围。无效副范围隐藏副轴 校验：`double.IsFinite` |
| [`GaugeSecondaryMinimum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L80) | `double` | 可写; OneWay | `0` | 气压表副刻度范围，默认 0–60；不改变主值或指针所代表的范围。无效副范围隐藏副轴 校验：`double.IsFinite` |
| [`GaugeSecondaryTickCount`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L82) | `int` | 可写; OneWay | `7` | 气压表副刻度数默认 7（范围 `[2, 12]`），颜色默认 Foreground 校验：`value => value >= 2 && value <= 12` |
| [`GaugeSegments`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L79) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.GaugeSegment>?` | 可写; OneWay | `null` | 每个实例默认独立空集合，支持直接 XAML 子项与绑定替换；有序色带列表；每项 Fraction 是相对于 Minimum–Maximum 的累计结束比例 `(0, 1]`，Brush 控制颜色，Label 用于 Grade |
| [`GaugeStartAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L56) | `double` | 可写; OneWay | `double.NaN` | NaN 表示随枚举；起点允许 −360°–360°，顺时针角度 `(0, 360]`。坐标系 0° 指向右、90° 指向下。支持整圆 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -360 && value <= 360)` |
| [`GaugeStyle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L55) | `TheOne.Avalonia.Controls.GaugeStyle` | 可写; OneWay | `GaugeStyle.Semicircle` | 结构 ; 默认起点 / 顺时针角度 ; 主题默认指针 / 刻度 / 主刻度数 校验：`Enum.IsDefined` 可选值：`Semicircle`, `Basic`, `Speed`, `Progress`, `StageSpeed`, `Grade`, `Temperature`, `Barometer` |
| [`GaugeSweepAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L57) | `double` | 可写; OneWay | `double.NaN` | NaN 表示随枚举；起点允许 −360°–360°，顺时针角度 `(0, 360]`。坐标系 0° 指向右、90° 指向下。支持整圆 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 360)` |
| [`GaugeTickBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L77) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 显式覆盖指针、主刻度和中心数值颜色；未设置时使用主题或所属色带颜色 |
| [`GaugeTickPlacement`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L63) | `TheOne.Avalonia.Controls.GaugeTickPlacement` | 可写; OneWay | `GaugeTickPlacement.Auto` | Auto 随枚举；Inside / Outside 覆盖主刻度位置，气压表副刻度保持内侧 校验：`Enum.IsDefined` 可选值：`Auto`, `Inside`, `Outside` |
| [`GaugeTrackBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L75) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 背景轴线／轨道颜色，默认 GridBrush |
| [`GaugeValueBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L78) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 显式覆盖指针、主刻度和中心数值颜色；未设置时使用主题或所属色带颜色 |
| [`GaugeValueFontSize`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L66) | `double` | 可写; OneWay | `double.NaN` | NaN 随枚举；字号 `(0, 200]`，垂直偏移为半径比例 `[−1, 1]`，正数向下。字号按实际空间缩小 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 200)` |
| [`GaugeValueOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L67) | `double` | 可写; OneWay | `double.NaN` | NaN 随枚举；字号 `(0, 200]`，垂直偏移为半径比例 `[−1, 1]`，正数向下。字号按实际空间缩小 校验：`value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`Kind`](../../../TheOne.Avalonia/Controls/GaugeChart.cs#L9) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |
| [`ShowGaugeAnchor`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L59) | `bool?` | 可写; OneWay | `null` | null 随枚举；控制指针中心锚点，仅指针开启时生效 |
| [`ShowGaugeProgress`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L58) | `bool?` | 可写; OneWay | `null` | null 随枚举；True / False 强制显示／隐藏进度弧 |
| [`ShowGaugeTickLabels`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L61) | `bool` | 可写; OneWay | `true` | 默认 True；分别控制数值和刻度标签，不影响数据 |
| [`ShowGaugeValue`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L60) | `bool` | 可写; OneWay | `true` | 默认 True；分别控制数值和刻度标签，不影响数据 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`GaugeChart.GaugeChart()`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L26) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<GaugeStyle> GaugeChart.GaugeStyleProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GaugeChart.GaugeStartAngleProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GaugeChart.GaugeSweepAngleProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool?> GaugeChart.ShowGaugeProgressProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool?> GaugeChart.ShowGaugeAnchorProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> GaugeChart.ShowGaugeValueProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> GaugeChart.ShowGaugeTickLabelsProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<GaugeNeedleShape> GaugeChart.GaugeNeedleShapeProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<GaugeTickPlacement> GaugeChart.GaugeTickPlacementProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L36) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GaugeChart.GaugeNeedleLengthProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GaugeChart.GaugeNeedleWidthProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L38) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GaugeChart.GaugeValueFontSizeProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L39) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GaugeChart.GaugeValueOffsetProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L40) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GaugeChart.GaugeGradeLabelRotationProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L41) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GaugeChart.GaugeGradeLabelGapProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L42) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GaugeChart.GaugeCaptionOffsetProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L43) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> GaugeChart.GaugeCaptionProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L44) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeTrackBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L45) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeNeedleBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L46) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeTickBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L47) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeValueBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L48) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IList<GaugeSegment>?> GaugeChart.GaugeSegmentsProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L49) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GaugeChart.GaugeSecondaryMinimumProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L50) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GaugeChart.GaugeSecondaryMaximumProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L51) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<int> GaugeChart.GaugeSecondaryTickCountProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L52) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GaugeChart.GaugeSecondaryBrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L53) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
- [`TheOne.Avalonia.Controls.GaugeNeedleShape`](TheOne.Avalonia.Controls.GaugeNeedleShape.md)
- [`TheOne.Avalonia.Controls.GaugeSegment`](TheOne.Avalonia.Controls.GaugeSegment.md)
- [`TheOne.Avalonia.Controls.GaugeStyle`](TheOne.Avalonia.Controls.GaugeStyle.md)
- [`TheOne.Avalonia.Controls.GaugeTickPlacement`](TheOne.Avalonia.Controls.GaugeTickPlacement.md)
