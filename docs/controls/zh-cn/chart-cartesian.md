# 坐标图与采样

**简体中文** · [English](../en/chart-cartesian.md)

数据模型与接入见[图表入口](charts.md)。

## X 轴标签与窄视口

折线、柱状和其他坐标图按实际文字宽度计算标签数量，不使用固定 36px 最小槽位跳过仍能容纳的短标签。Small 下 240px 宽的月份示例可完整显示六个月份；标签按屏幕位置检查间距，递减 X 数据也适用。确实放不下时减少标签数量，数据点、坐标和提示信息保持完整。

标签边缘在可用文本区域内对齐，使用整个轴标签区域裁剪，长文字不再被固定 80px 窗口截断。底部空间同时考虑实际文字高度，与图例分开。单个标签比整个图表还宽时，业务端应提供较短名称或更宽视口。

`--verify-chart-axis-labels [输出目录]` 使用独立绘制的未裁剪文字作像素对照，覆盖折线／柱状、明暗主题、三档尺寸、六个月份、长中文标签、递减 X 和图例开关。托管及原生调用见[预览专项入口](../../../design/PreviewHost/Program.cs)。

## 平滑曲线

```xml
<one:LineChart IsSmooth="True"
           SeriesSource="{CompiledBinding Revenue}" Height="320" />
```

`IsSmooth` 默认 `False`，仅用于 `Line` 与普通 `Area`。开启后，对 X 严格递增或递减、没有重复 X 的连续段使用单调三次插值；局部极值保留在输入点，缺口仍断开。重复或回头 X 的段保持直线连接，不重排源数据。普通面积的上沿使用同一曲线；堆叠面积、百分比堆叠面积保持直边，避免上下边界独立插值后交叉。

平滑只改变绘制路径，选择、采样、提示与共享 X 查询仍对应原始数据点，不生成可查询的插值点。该取舍参考 [Chart.js 的单调插值说明](https://www.chartjs.org/docs/latest/charts/line.html)与 [ECharts 的平滑折线开关](https://echarts.apache.org/handbook/en/how-to/chart-types/line/smooth-line/)。

## 堆叠图

```xml
<one:StackedBarChart Title="月度营收构成"
           SeriesSource="{CompiledBinding Revenue}"
           IsZoomEnabled="True" IsPanEnabled="True" Height="300" />
```

`StackedBar` / `StackedHorizontalBar` 按源列表索引对齐，同轴可见系列的正值从零向正方向累加，负值从零向负方向累加。主、次轴独立累加，并在每个类别中使用不同槽位。缺失、非有限数据不参与累加；不同长度的系列可以使用，但业务层仍需保证同一索引表示同一类别。数据标签与提示显示单个贡献值，选择返回原始系列及数据点。

`StackedArea` 也按索引对齐；每个轴、每个索引以同轴首个可见系列中有限的 X 作为该位置的标准 X。其他同轴系列该索引的 X 不匹配时按缺口处理，不会按 X 搜索或重新排列；各系列应预先对齐采样位置。缺失数据及正负方向变化会分开填充带，正、负贡献分别累加。坐标轴按堆叠后的范围计算；同方向累计超出 `double` 的有限范围时限制到 `±double.MaxValue`，避免生成非有限绘制几何。

同一索引的多个零值贡献可能共享基线标记，可通过键盘分别选择原始数据点。累计值达到有限上限后，后续贡献可能不再有可见厚度；没有可见标记时 `GetDataPointPosition` 返回 `null`，定位接口也可能返回 `false`。

## 百分比堆叠

```xml
<one:PercentStackedBarChart Title="月度营收占比"
           SeriesSource="{CompiledBinding Revenue}"
           ShowDataLabels="True" Height="300" />
```

三种 `PercentStacked*` 保留对应堆叠图的索引与 X 对齐规则。每个轴、每个索引的正值按正值总和归一化到 100%，负值按负值绝对总和归一化到 -100%；两种方向独立计算，不能将正负贡献相抵后作为分母。零值占比为 0，无效数据不参与分母；计算先缩放再求和，极大有限数值不会使百分比总和溢出。

百分比数值轴使用固定百分比范围，忽略 `YMinimum` / `YMaximum` 和次轴的数值范围覆盖；可用视口继续缩放和平移。数据标签显示一位小数百分比，提示同时包含原始值、系列单位及有符号占比。`GetStackPercentage(seriesIndex, pointIndex)` 返回 `double?` 的有符号比例（例如 `0.25` 为 25%，`-0.25` 为 -25%）；非百分比图形或无效贡献返回 `null`。

```csharp
var share = RevenueChart.GetStackPercentage(0, 3);
if (share is { } ratio) Console.WriteLine($"{ratio:P1}");
```

## 双 Y 轴与系列格式

将系列的 `YAxis` 设置为 `ChartYAxis.Primary`（默认）或 `ChartYAxis.Secondary`。X / 类别位置共用，两个数值轴分别按所属系列计算范围；柱状图的主、次轴系列仍按类别并列，堆叠只在同轴内累加。普通坐标图的次轴位于右侧，横向条形图的第二个数值轴位于顶部。

```csharp
viewModel.Revenue[1].Name = "转化率";
viewModel.Revenue[1].YAxis = ChartYAxis.Secondary;
viewModel.Revenue[1].ValueFormat = "0.0";
viewModel.Revenue[1].Unit = "%";
viewModel.Revenue[1].ItemsSource = new[]
{
    new ChartPoint(1, 3.5, "一月"), new ChartPoint(2, 4.6, "二月"),
    new ChartPoint(3, 4.9, "三月"), new ChartPoint(4, 6.1, "四月")
};
```

```xml
<one:LineChart Title="营收与转化率"
           SeriesSource="{CompiledBinding Revenue}"
           YAxisTitle="营收（万元）" Unit=" 万元"
           SecondaryYAxisTitle="转化率（%）"
           SecondaryValueFormat="0.0" SecondaryUnit="%"
           SecondaryYMinimum="0" Height="320" />
```

`SecondaryYMinimum` / `SecondaryYMaximum` 默认为 `NaN`，表示自动范围；有限值覆盖次轴范围。主轴使用原有 `YMinimum` / `YMaximum`、`ValueFormat` / `Unit`，次轴使用 `SecondaryValueFormat`（默认 `0.##`）/ `SecondaryUnit`。`YAxisTitle` / `SecondaryYAxisTitle` 设置轴标题。

`ShowSecondaryAxis` 默认 `True`，存在 `IsVisible=True` 的有效次轴系列时自动显示次轴；设为 `False` 隐藏次轴刻度和标题，但仍保留其独立缩放。某个轴的最终上限不大于下限时，仅该轴不绘制数据；有效的另一轴仍可显示。两轴共用 `Viewport` 的归一化窗口，缩放相同比例会分别映射到各自的数值范围。系列格式仅覆盖提示／数据标签的原始值，不更改轴刻度格式；百分比标签仍使用百分比格式。径向图不使用双轴缩放。

## 大数据采样

```xml
<one:LineChart SeriesSource="{CompiledBinding Revenue}"
           SamplingMode="MinMax" SamplingThreshold="2000"
           SamplingPointBudget="2000" IsZoomEnabled="True"
           IsPanEnabled="True" Height="320" />
```

`SamplingMode` 为 `ChartSamplingMode.None` 或 `MinMax`，默认 `MinMax`；`SamplingThreshold` 与 `SamplingPointBudget` 均默认 2,000，分别至少为 2 / 8。折线、面积、散点、普通／百分比堆叠面积可采样，类别柱状／条形图保持精确绘制。MinMax 针对按 X 单调排列的连续段保留上下沿局部极值、边界与当前选择；不按 X 重排源数据，非单调段退回精确绘制。

预算是**每个系列的软目标**：按连续段长度分配，段边界、缺口、符号变化和所选原始点需要额外保留，碎片很多或不能采样的段可能超过目标。它不是所有系列共享的上限，也不是严格的顶点数量限制。缩放后会按当前视图重新准备几何；`SamplingMode=None` 关闭采样。

`RenderedPointCount` 为只读的坐标图已准备绘制点数，汇总所有系列的数值曲线采样点和可见柱形／条形数量；饼图、环形图及仪表为 0。它不是原始数据数量、Control 数量或底层绘制命令数。`SamplingActive` 表示已准备结果是否实际减少了点；两者在布局及几何准备后更新。绘制过程中的诊断通知会合并并延后到下一次 UI 派发，避免绑定或监听器在绘制中修改图表；通过几何 API 在绘制外准备时同步更新。

渲染几何与原始点空间命中索引缓存，鼠标查询复用已准备数据；数据、坐标轴、视口、布局或采样配置变化后重新准备。`GetDataPointPosition`、命中、选择和提示仍使用原始系列与点索引，未进入绘制采样结果的可见原始点也可定位与选择。命中测试查询指针附近的空间桶；大量重叠或高度密集的数据仍会增加局部候选扫描成本，不保证固定的查询耗时。

```csharp
Console.WriteLine($"绘制顶点：{RevenueChart.RenderedPointCount}，采样：{RevenueChart.SamplingActive}");
RevenueChart.SelectedIndex = 99_999; // 数据源须包含该项；选择仍指向原始模型。
```

采样不保证保留每个点的视觉细节；需要逐点审查时关闭采样或放大视口。预览提供 100,000 点示例及实际绘制计数，性能边界以目标设备和数据形状为准。

## 刻度与标签精度

折线、面积及散点的数值标签跨系列统一避让，先尝试数据点上方，再尝试下方或侧方。候选上限 2,048 个，实际绘制上限 512 个；无法安全放置的标签省略，提示和选择仍可读取原值。`GetDataLabelBounds(seriesIndex, pointIndex)` 返回已显示标签的目标边界；关闭标签、点未进入采样或无法放置时返回 `null`，查询会先完成当前动画。

数值轴根据可见范围和可用高度（横向图按宽度）选择 1、2、2.5、5 乘以 10 的整数幂作为间隔，绘制范围内的整倍数刻度。例如数据范围 34–92 会显示 40、60、80，而不是把端点强制等分成 53.33、72.67。刻度不改变数据范围、显式 YMinimum / YMaximum 或选点坐标；缩放后按当前视口重新计算。百分比图保持百分比文本。极端或接近浮点精度边界的范围使用安全插值。

默认 ValueFormat="0.##" 时，轴刻度按间隔增加必要的小数位；微小数值使用有效数字，高数量级的窄范围保留更高精度，避免不同刻度显示相同标签。显式设置其他 ValueFormat / SecondaryValueFormat 时遵循用户格式；如指定整数格式，则小数信息会被舍去。原始数据提示与数据标签仍按各系列的格式显示。

X 轴根据标签实际宽度与间距决定可容纳数量，优先保留可见范围两端；短标签可显示更多项，长标签会抽样。标签显示与数据采样相互独立，不修改原始索引或选择。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节只列出本项目增加的 Classes、属性、扩展 API／事件及关联模型，不重复 Avalonia 原生继承 API。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用可见示例对照本项目附加样式，用 Description 组件说明专属属性、API 签名与模型字段；ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### AreaChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.AreaChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/AreaChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### BarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.BarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/BarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### CartesianChartBase

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.CartesianChartBase.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### ChartBase

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`AccentBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L78) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 主序列／强调色画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`AnimationDuration`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L18) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(200)` | 数据过渡的持续时间；是否启用动画还受控件开关和 Motion.Enabled 约束。 校验：`value => value >= TimeSpan.Zero && value <= TimeSpan.FromSeconds(10)` |
| [`AnimationProgress`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L20) | `double` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Background`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L74) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 背景画刷；主题相关背景使用 DynamicResource，避免固定颜色破坏明暗切换。 |
| [`ChartPadding`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L20) | `double` | 可写; OneWay | `12` | 8 ; 12 ; 16 校验：`value => double.IsFinite(value) && value >= 0` |
| [`CrosshairPosition`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L20) | `Avalonia.Point?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`DefaultChartHeight`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L19) | `double` | 可写; OneWay | `280` | 320 / 224 ; 400 / 280 ; 480 / 336 校验：`value => double.IsFinite(value) && value > 0` |
| [`DefaultChartWidth`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L17) | `double` | 可写; OneWay | `400` | 320 / 224 ; 400 / 280 ; 480 / 336 校验：`value => double.IsFinite(value) && value > 0` |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L71) | `string` | 可写; OneWay | `"暂无数据"` | `暂无数据` |
| [`FifthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L82) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第五序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`FontFamily`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L84) | `Avalonia.Media.FontFamily` | 可写; OneWay | `inherited registration` | 文本绘制使用的字体族；字体回退和实际度量由 Avalonia 文本系统处理。 |
| [`FontSize`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L85) | `double` | 可写; OneWay | `inherited registration` | 10 ; 11 ; 12 |
| [`Foreground`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L75) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 前景画刷；自定义时保持正文、状态与禁用对比度。 |
| [`GaugeBandRatio`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L25) | `double` | 可写; OneWay | `0.18` | 沿用三档主题密度；基础、进度、等级、气压表按结构缩放带宽，可显式调整该比例 校验：`value => double.IsFinite(value) && value > 0 && value <= 0.5` |
| [`GaugeMajorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L16) | `int` | 可写; OneWay | `5` | Number of major ticks, including the minimum and maximum. 校验：`value => value >= 2 && value <= 25` |
| [`GaugeMinorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L18) | `int` | 可写; OneWay | `2` | Maximum minor marks between consecutive major ticks. Compact gauges reduce them to keep readable spacing. 校验：`value => value >= 0 && value <= 10` |
| [`GridBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L76) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 绘图区网格线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`HoveredPoints`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L21) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.ChartSelection>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`InnerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L63) | `double` | 可写; OneWay | `0.62` | 径向图内半径比例；控制中空区域，保留扇区数值和比例含义。 校验：`value => double.IsFinite(value) && value >= 0 && value < 1` |
| [`IsAnimating`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L19) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsAnimationEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L17) | `bool` | 可写; OneWay | `true` | 启用该图表的数据过渡；全局 Motion.Enabled=False 时仍关闭动画。 |
| [`IsLegendInteractive`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L11) | `bool` | 可写; OneWay | `true` | `True`，允许坐标图图例切换系列及键盘操作 |
| [`IsPanEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L34) | `bool` | 可写; OneWay | `false` | 默认 `False`；启用鼠标左键拖动及键盘平移 |
| [`IsSmooth`](../../../TheOne.Avalonia/Controls/ChartBase.Smoothing.cs#L10) | `bool` | 可写; OneWay | `false` | `False`，折线／普通面积的单调平滑路径 |
| [`IsZoomEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L32) | `bool` | 可写; OneWay | `false` | 默认 `False`；启用滚轮及键盘缩放 |
| [`Kind`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L55) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |
| [`LegendPageCount`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L13) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读分页状态，页索引从 0 开始 |
| [`LegendPageIndex`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L12) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读分页状态，页索引从 0 开始 |
| [`MarkerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L21) | `double` | 可写; OneWay | `3.5` | 3 ; 3.5 ; 4.25 校验：`value => double.IsFinite(value) && value > 0` |
| [`Maximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L67) | `double` | 可写; OneWay | `100` | 主数值轴上界；空值使用自动范围，不通过裁剪更改数据本身。 校验：`double.IsFinite` |
| [`Minimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L66) | `double` | 可写; OneWay | `0` | 主数值轴下界；空值由数据自动推导，显式范围必须与数据及 Maximum 协调。 校验：`double.IsFinite` |
| [`MutedBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L77) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 次要文字与辅助标记画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`NavigationAxes`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L35) | `TheOne.Avalonia.Controls.ChartNavigationAxes` | 可写; OneWay | `ChartNavigationAxes.Both` | 默认 `Both`；可设为 `None`、`Horizontal`、`Vertical`，限制导航轴 校验：`value => (value & ~ChartNavigationAxes.Both) == 0` 可选值：`None`, `Horizontal`, `Vertical`, `Both` |
| [`QuaternaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L81) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第四序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`RenderedPointCount`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L19) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SamplingActive`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L20) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SamplingMode`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L15) | `TheOne.Avalonia.Controls.ChartSamplingMode` | 可写; OneWay | `ChartSamplingMode.MinMax` | 密集序列的采样算法；可选策略见枚举，采样影响绘制点数，不替换原始数据。 校验：`value => value is ChartSamplingMode.None or ChartSamplingMode.MinMax` 可选值：`None`, `MinMax` |
| [`SamplingPointBudget`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L18) | `int` | 可写; OneWay | `2000` | A per-series sampling target; gap boundaries and selected originals can exceed it. 校验：`value => value >= 8` |
| [`SamplingThreshold`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L16) | `int` | 可写; OneWay | `2000` | 启用采样的点数阈值；结合视口与 SamplingMode 调整大数据绘制成本。 校验：`value => value >= 2` |
| [`SecondaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L79) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第二序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`SecondaryUnit`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L19) | `string?` | 可写; OneWay | `null` | 第二数值轴的单位后缀，与主轴单位独立。 |
| [`SecondaryValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L18) | `string` | 可写; OneWay | `"0.##"` | 第二数值轴的数值格式字符串；使用符合 .NET 数值格式的值。 |
| [`SecondaryYAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L17) | `string?` | 可写; OneWay | `null` | 第二数值轴标题；不改变序列使用哪一条数值轴。 |
| [`SecondaryYMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L15) | `double` | 可写; OneWay | `double.NaN` | 第二数值轴上界；空值自动计算，与 SecondaryYMinimum 配合使用。 校验：`value => !double.IsInfinity(value)` |
| [`SecondaryYMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L14) | `double` | 可写; OneWay | `double.NaN` | 第二数值轴下界；空值自动计算，与 SecondaryYMaximum 配合使用。 校验：`value => !double.IsInfinity(value)` |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L73) | `int` | 可写; TwoWay | `-1` | 零基系列 / 点索引，支持双向绑定；点索引 `-1` 清除选择 |
| [`SelectedSeriesIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L72) | `int` | 可写; TwoWay | `0` | 零基系列 / 点索引，支持双向绑定；点索引 `-1` 清除选择 |
| [`Selection`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L86) | `TheOne.Avalonia.Controls.ChartSelection?` | 只读; OneWay | `computed / state` | 只读 `ChartSelection?`，包含索引以及原始 `Series` / `Point` 引用 |
| [`SelectionThickness`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L22) | `double` | 可写; OneWay | `2` | 1.5 ; 2 ; 2.5 校验：`value => double.IsFinite(value) && value > 0` |
| [`SeriesSource`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L56) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartSeries>?` | 可写; OneWay | `null` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`ShowCrosshair`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L18) | `bool` | 可写; OneWay | `false` | `False`，开启坐标图指针十字准线 |
| [`ShowDataLabels`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L61) | `bool` | 可写; OneWay | `false` | `False`，坐标图显示数据值，饼图／环形图显示可容纳的百分比 |
| [`ShowGaugeNeedle`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L13) | `bool` | 可写; OneWay | `false` | 主题按枚举提供默认值；显式 False 隐藏对应元素。气压表的刻度开关同时作用于两套轴 |
| [`ShowGaugeTicks`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L14) | `bool` | 可写; OneWay | `false` | 主题按枚举提供默认值；显式 False 隐藏对应元素。气压表的刻度开关同时作用于两套轴 |
| [`ShowGrid`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L59) | `bool` | 可写; OneWay | `true` | `True`，坐标图网格 |
| [`ShowLegend`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L58) | `bool` | 可写; OneWay | `true` | `True`，坐标图为系列图例，饼图／环形图为类别图例 |
| [`ShowMarkers`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L60) | `bool` | 可写; OneWay | `true` | `True`，折线／面积图的数据标记 |
| [`ShowSecondaryAxis`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L20) | `bool` | 可写; OneWay | `true` | 显示第二数值轴；序列轴归属仍由序列模型配置。 |
| [`SixthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L83) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第六序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`SliceHoverOffset`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L23) | `double` | 可写; OneWay | `6` | 4 ; 6 ; 8 校验：`value => double.IsFinite(value) && value >= 0` |
| [`SliceHoverScale`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L24) | `double` | 可写; OneWay | `1.025` | 1.02 ; 1.025 ; 1.03 校验：`value => double.IsFinite(value) && value >= 1 && value <= 1.2` |
| [`StrokeThickness`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L62) | `double` | 可写; OneWay | `2.5` | `2.5`，必须大于零且有限 校验：`value => double.IsFinite(value) && value > 0` |
| [`TertiaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L80) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第三序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`Title`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L57) | `string?` | 可写; OneWay | `null` | 可选标题 |
| [`TooltipMode`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L19) | `TheOne.Avalonia.Controls.ChartTooltipMode` | 可写; OneWay | `ChartTooltipMode.Single` | `ChartTooltipMode.Single`，可设为 `Shared` 查看同位置的多个系列 校验：`value => value is ChartTooltipMode.Single or ChartTooltipMode.Shared` 可选值：`Single`, `Shared` |
| [`Unit`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L70) | `string?` | 可写; OneWay | `null` | `0.##` / 空，使用当前区域设置格式化数值 |
| [`Value`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L68) | `double` | 可写; OneWay | `0` | 当前值；单位、范围及空值含义以所属模型和校验条件为准。 校验：`double.IsFinite` |
| [`ValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L69) | `string` | 可写; OneWay | `"0.##"` | `0.##` / 空，使用当前区域设置格式化数值 |
| [`Viewport`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L30) | `Avalonia.Rect` | 可写; TwoWay | `new Rect(0, 0, 1, 1)` | A normalized window into full data extents, bounded to [0,1] and at most 1,000× zoom. 校验：`value => double.IsFinite(value.X) && double.IsFinite(value.Y) && double.IsFinite(value.Width) && double.IsFinite(value.Height) && value.Width > 0 && value.Height > 0` |
| [`YAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L16) | `string?` | 可写; OneWay | `null` | 主数值轴标题，参与实际文本度量与轴区域布局。 |
| [`YMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L65) | `double` | 可写; OneWay | `double.NaN` | `NaN` 表示自动范围，可设置有限 Y 轴范围 校验：`value => !double.IsInfinity(value)` |
| [`YMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L64) | `double` | 可写; OneWay | `double.NaN` | `NaN` 表示自动范围，可设置有限 Y 轴范围 校验：`value => !double.IsInfinity(value)` |

### HorizontalBarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.HorizontalBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/HorizontalBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### LineChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.LineChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/LineChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### PercentStackedAreaChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedAreaChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PercentStackedAreaChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### PercentStackedBarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PercentStackedBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### PercentStackedHorizontalBarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedHorizontalBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PercentStackedHorizontalBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### ScatterChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.ScatterChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/ScatterChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### StackedAreaChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StackedAreaChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/StackedAreaChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### StackedBarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StackedBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/StackedBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### StackedHorizontalBarChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StackedHorizontalBarChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/StackedHorizontalBarChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| ChartBase | `one-large` | 模板自动类；不手动绑定 | ^.one-large => FontSize=12; DefaultChartWidth=480; DefaultChartHeight=336; ChartPadding=16; StrokeThickness=3; MarkerRadius=4.25; SelectionThickness=2.5; SliceHoverOffset=8; SliceHoverScale=1.03; GaugeBandRatio=0.22 |
| ChartBase | `one-small` | 模板自动类；不手动绑定 | ^.one-small => FontSize=10; DefaultChartWidth=320; DefaultChartHeight=224; ChartPadding=8; StrokeThickness=2; MarkerRadius=3; SelectionThickness=1.5; SliceHoverOffset=4; SliceHoverScale=1.02; GaugeBandRatio=0.14 |

`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出附加外观类的控件使用默认主题及其专属属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.AreaChart`](../../api/zh-cn/TheOne.Avalonia.Controls.AreaChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.BarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.BarChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.CartesianChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.CartesianChartBase.md) | Class | 0 | 0 | 0 |
| [`TheOne.Avalonia.Controls.ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | Class | 72 | 18 | 2 |
| [`TheOne.Avalonia.Controls.ChartKind`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartKind.md) | Enum | 14 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartNavigationAxes`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartNavigationAxes.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartPoint`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartPoint.md) | Class | 3 | 2 | 0 |
| [`TheOne.Avalonia.Controls.ChartSamplingMode`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSamplingMode.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartSelection`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSelection.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartSeries`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSeries.md) | Class | 7 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartTooltipMode`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartTooltipMode.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartViewportChangedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartViewportChangedEventArgs.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartYAxis`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartYAxis.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.HorizontalBarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.HorizontalBarChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.LineChart`](../../api/zh-cn/TheOne.Avalonia.Controls.LineChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.PercentStackedAreaChart`](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedAreaChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.PercentStackedBarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedBarChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.PercentStackedHorizontalBarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.PercentStackedHorizontalBarChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ScatterChart`](../../api/zh-cn/TheOne.Avalonia.Controls.ScatterChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StackedAreaChart`](../../api/zh-cn/TheOne.Avalonia.Controls.StackedAreaChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StackedBarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.StackedBarChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StackedHorizontalBarChart`](../../api/zh-cn/TheOne.Avalonia.Controls.StackedHorizontalBarChart.md) | Class | 1 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
