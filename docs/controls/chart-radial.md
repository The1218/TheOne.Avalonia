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

`ShowGaugeNeedle` / `ShowGaugeTicks` 默认均为 `False`；开启后显示指针与刻度。指针和弧线使用同一范围及受限数值，超出范围时停在端点，文本仍保留原始值。`GaugeMajorTickCount` 默认 5，范围 `[2, 12]`，包含最小值与最大值；`GaugeMinorTickCount` 默认 2，范围 `[0, 10]`，表示两个主刻度之间的小刻度数量上限，紧凑布局按弧长减少次刻度，避免拥挤。主／次刻度采用色带外侧的圆头短线，以长度及粗细区分，不切穿色带。刻度文字沿用 `ValueFormat`，过长范围文字使用科学计数，空间不足时省略会碰撞的中间标签。数值与单位使用相同字号，同行、基线对齐并整体居中；空间不足时两者一起缩小，数值标题放不下时改用紧凑格式并调整字号，过长单位省略显示；模型数值及单位不改变。

折线、面积及散点的数值标签跨系列统一避让，优先放在数据点上方，发生碰撞时改到下方或侧方；避让范围包括其他标签及标记。首尾标记完整绘制，曲线仍按坐标视口裁剪。空间不足时省略不能安全放置的标签，原始值仍可通过提示及选择读取。布局最多准备 2,048 个候选、绘制 512 个数值标签，不为标注创建控件。`GetDataLabelBounds(seriesIndex, pointIndex)` 返回已显示数值标签的目标边界；关闭标签、原始点未进入采样或未能放置时返回 `null`。该几何查询与其他命中 API 一样先完成当前动画。
