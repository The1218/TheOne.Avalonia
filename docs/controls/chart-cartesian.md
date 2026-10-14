# 坐标图与采样

数据模型与接入见[图表入口](charts.md)。

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
