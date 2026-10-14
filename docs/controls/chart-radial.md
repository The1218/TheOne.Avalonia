# 饼图、环形图与仪表

数据模型与接入见[图表入口](charts.md)。

## 饼图与环形图

```xml
<one:DoughnutChart Title="营收构成"
           SeriesSource="{CompiledBinding Revenue}"
           InnerRadius="0.62" ShowDataLabels="True"
           ValueFormat="0.##" Unit=" 万元" Height="300" />
```

饼图／环形图只使用首个 `IsVisible=True` 且非空的系列；其余系列不合并。只有 X / Y 均有限且 `Y > 0` 的点参与比例、图例与选择，零值、负值及非有限值被忽略。类别名称使用 `Label`。若首个可见非空系列没有有效正值，显示 `EmptyText`，不会自动跳到后续系列。

`InnerRadius` 为 `[0, 1)` 的比例，只影响环形图；`Pie` 保持实心。`ShowDataLabels=True` 显示可容纳文字的扇区百分比，小扇区仍可通过提示查看实际值。大量类别可先在业务层汇总为主要类别与“其他”，图例可分页浏览其余类别。

## 仪表图

```xml
<one:GaugeChart Title="目标完成率"
           Minimum="0" Maximum="100" Value="72"
           ShowGaugeNeedle="True" ShowGaugeTicks="True"
           GaugeMajorTickCount="5" GaugeMinorTickCount="2"
           ValueFormat="0" Unit="%" Height="280" />
```

`Minimum`、`Maximum`、`Value` 必须为有限值。业务层应提供 `Maximum > Minimum` 的范围；无效范围显示“最大值必须大于最小值”。仪表的弧线按范围限制显示，数值文本保留实际 `Value`；可以用编译绑定把 `Value` 连接到带变化通知的 ViewModel 属性。仪表不提供数据点选择与图例。

原有 `GaugeStyle="Semicircle"` 的 `ShowGaugeNeedle` / `ShowGaugeTicks` 默认均为 `False`；开启后显示指针与刻度。指针和弧线使用同一范围及受限数值，超出范围时停在端点，文本仍保留原始值。`GaugeMajorTickCount` 默认 5，范围 `[2, 25]`，包含最小值与最大值；`GaugeMinorTickCount` 默认 2，范围 `[0, 10]`，表示两个主刻度之间的小刻度数量上限，紧凑布局按弧长减少次刻度，避免拥挤。原有半圆的主／次刻度采用色带外侧的圆头短线，以长度及粗细区分，不切穿色带。刻度文字沿用 `ValueFormat`，过长范围文字使用科学计数，空间不足时省略会碰撞的中间标签。数值与单位使用相同字号，同行、基线对齐并整体居中；空间不足时两者一起缩小，数值标题放不下时改用紧凑格式并调整字号，过长单位省略显示；模型数值及单位不改变。

### 用枚举切换仪表

`GaugeStyle` 使用 `GaugeStyle` 枚举选择结构；数值范围、颜色、单位和格式仍由属性控制。默认 `Semicircle` 保持原有半圆绘制。基础仪表的数字动画使用现有 `IsAnimationEnabled` / `AnimationDuration` 与 `Motion.Enabled`，不新增动画专用枚举。

| GaugeStyle | 结构 | 默认起点 / 顺时针角度 | 主题默认指针 / 刻度 / 主刻度数 |
| --- | --- | --- | --- |
| Semicircle | 半圆进度，兼容原有默认 | 180° / 180° | False / False / 5 |
| Basic | 薄轴线、三角指针 | 135° / 270° | True / True / 11 |
| Speed | 半圆速度表、进度弧、指针锚点 | 180° / 180° | True / True / 9 |
| Progress | 270° 进度弧、线形指针 | 135° / 270° | True / True / 11 |
| StageSpeed | 分段色带，指针跟随所属分段颜色 | 135° / 270° | True / True / 11 |
| Grade | 半圆色带、分段标签、短三角标记 | 180° / 180° | True / True / 9 |
| Temperature | 无指针、双层进度弧、中央数值 | 160° / 220° | False / True / 13 |
| Barometer | 外侧主轴、内侧独立刻度、长线指针 | 135° / 270° | True / True / 11 |

主题默认值通过 ControlTheme 样式提供，显式属性值和编译绑定优先；切换枚举不会覆盖本地设置。枚举不会改写 `Minimum`、`Maximum`、`Value`、`Unit` 或业务颜色。未载入 TheOne 主题时，指针、刻度和主刻度数仍使用 ChartBase 的默认值。

```xml
<one:GaugeChart GaugeStyle="Temperature" Title="温度"
                Minimum="0" Maximum="60" Value="35.78" Unit="°C"
                AccentBrush="#FFAB91" SecondaryBrush="#FD7347" />

<one:GaugeChart GaugeStyle="Grade" Value="70" GaugeCaption="Grade Rating">
  <one:GaugeChart.GaugeSegments>
    <one:GaugeSegment Fraction="0.25" Brush="#FF6E76" Label="Grade D" />
    <one:GaugeSegment Fraction="0.5" Brush="#FDDD60" Label="Grade C" />
    <one:GaugeSegment Fraction="0.75" Brush="#58D9F9" Label="Grade B" />
    <one:GaugeSegment Fraction="1" Brush="#7CFFB2" Label="Grade A" />
  </one:GaugeChart.GaugeSegments>
</one:GaugeChart>
```

| 属性 | 默认值与用途 |
| --- | --- |
| GaugeStartAngle / GaugeSweepAngle | NaN 表示随枚举；起点允许 −360°–360°，顺时针角度 `(0, 360]`。坐标系 0° 指向右、90° 指向下。支持整圆 |
| ShowGaugeNeedle / ShowGaugeTicks | 主题按枚举提供默认值；显式 False 隐藏对应元素。气压表的刻度开关同时作用于两套轴 |
| ShowGaugeProgress | null 随枚举；True / False 强制显示／隐藏进度弧 |
| ShowGaugeAnchor | null 随枚举；控制指针中心锚点，仅指针开启时生效 |
| ShowGaugeValue / ShowGaugeTickLabels | 默认 True；分别控制数值和刻度标签，不影响数据 |
| GaugeNeedleShape | Auto 随枚举；Triangle / Line / Marker 单独覆盖指针形状 |
| GaugeNeedleLength / GaugeNeedleWidth | 长度默认半径的 0.72，范围 `(0, 1]`；宽度 NaN 随形状，显式像素值 `(0, 50]` |
| GaugeTickPlacement | Auto 随枚举；Inside / Outside 覆盖主刻度位置，气压表副刻度保持内侧 |
| GaugeBandRatio | 沿用三档主题密度；基础、进度、等级、气压表按结构缩放带宽，可显式调整该比例 |
| GaugeGradeLabelRotation / GaugeGradeLabelGap | 等级标签默认沿切线旋转并保持可读；旋转角 NaN 自动、`[−90, 90]` 显式角度（0 水平）；标签到色带的间距 NaN 随字号、`[0, 100]` 显式像素 |
| GaugeCaption / GaugeCaptionOffset | 说明文字默认 null；说明顶端的垂直偏移为半径比例 `[−1, 1]`，NaN 随枚举，与顶部 Title 分开 |
| GaugeValueFontSize / GaugeValueOffset | NaN 随枚举；字号 `(0, 200]`，垂直偏移为半径比例 `[−1, 1]`，正数向下。字号按实际空间缩小 |
| GaugeTrackBrush | 背景轴线／轨道颜色，默认 GridBrush |
| GaugeNeedleBrush / GaugeTickBrush / GaugeValueBrush | 显式覆盖指针、主刻度和中心数值颜色；未设置时使用主题或所属色带颜色 |
| GaugeSegments | 每个实例默认独立空集合，支持直接 XAML 子项与绑定替换；有序色带列表；每项 Fraction 是相对于 Minimum–Maximum 的累计结束比例 `(0, 1]`，Brush 控制颜色，Label 用于 Grade |
| GaugeSecondaryMinimum / GaugeSecondaryMaximum | 气压表副刻度范围，默认 0–60；不改变主值或指针所代表的范围。无效副范围隐藏副轴 |
| GaugeSecondaryTickCount / GaugeSecondaryBrush | 气压表副刻度数默认 7（范围 `[2, 12]`），颜色默认 Foreground |

默认分段速度表使用三个分段，等级表使用四个分段，颜色来自现有主题系列色。实际等级名称和业务色值通过 `GaugeSegments` 指定。按 Fraction 递增提供色带；非递增项跳过，末尾不足 1 的剩余区间显示轨道。`ObservableCollection<GaugeSegment>` 的增删和每个分段属性的变更会自动重绘；普通列表增删后重新赋值列表。分段事件在离开视觉树时解除。

新增属性也可用于 `Semicircle`；显式使用这些属性时采用通用仪表布局。不配置新增属性时保留原有布局。所有样式均直接绘制、支持数值更新动画和稳定目标值 PNG 导出；导出保留样式、色带及属性覆盖。窄视口优先保留端点，中间刻度标签相撞时省略，极小尺寸回退为数值。

参考：[Apache ECharts 仪表盘示例](https://echarts.apache.org/examples/zh/index.html#chart-type-gauge)。PreviewHost 的 GaugeChart 页同时展示八种样式，展开“交互与数据实验”可通过枚举下拉框在同一实例上切换结构。

等级标签按实际文字的旋转边界参与仪表尺寸计算，放在色带外侧；可见文字不会压到色带或刻度。窄视口按旋转边界检查标签之间的碰撞，省略无法容纳的标签，不把文字向内挤到弧线上。

数值绘制请求字体的等宽数字特性（`tnum`，字体支持时生效），字号根据范围和目标值预留，单位位置跟随目标布局，不跟随每帧插值宽度。默认 `ValueFormat="0.##"` 的整数动画使用整数显示；连续拖动出现小数后在这段动画中保留两位小数，经过整数目标也不反复改变精度，结束后恢复 `ValueFormat` 的最终显示。显式数值格式继续优先。指针和进度弧保留连续数值动画。

### 精度与尺寸

默认 `ValueFormat="0.##"` 时，刻度文字根据主刻度间隔保留必要小数位。例如范围 0–1、5 个主刻度显示 0、0.25、0.5、0.75、1。自定义 `ValueFormat` 仍优先；整数格式会省略小数。高数量级窄范围保留更多有效数字，避免相邻刻度显示为相同文字。

下表描述兼容半圆布局；其他枚举按结构调整带宽与数值字号。

| 配置 | Small | Normal | Large |
| --- | --- | --- | --- |
| 默认期望宽／高 | 320 / 224 | 400 / 280 | 480 / 336 |
| 基础字号 | 10 | 11 | 12 |
| 刻度字号 | 9 | 10 | 11 |
| 中心数值字号 | 20 | 22 | 24 |
| GaugeBandRatio | 0.14 | 0.18 | 0.22 |

中心数值与单位会在空间不足时缩小。端点标签优先，中间标签相撞时省略；尺寸更小时回退为数值提示。不要把三个尺寸的 Width / Height 固定为相同值：显式尺寸和 Grid 拉伸决定外部大小，尺寸主题只改变内部密度。PreviewHost 八种仪表样式的可用宽度为 240 / 300 / 360，随顶部尺寸设置变化。

折线／面积／散点的标签规则见[坐标图与采样](chart-cartesian.md)。
