# 图表更新、导出与尺寸

数据模型与接入见[图表入口](charts.md)。

## 数据更新动画

```xml
<one:LineChart IsSmooth="True"
           SeriesSource="{CompiledBinding Revenue}"
           IsAnimationEnabled="True" AnimationDuration="0:0:0.2" />
```

`IsAnimationEnabled` 默认 `True`，`AnimationDuration` 为 `TimeSpan`，默认 200 ms，可设为零至 10 秒。动画用于已挂载图表的数据更新：坐标图在原始点身份及顺序稳定时过渡数据层坐标，结构变化时交叉淡化已缓存的新旧数据层；饼图／环形图过渡扇区角度，仪表过渡数值、弧线与指针。坐标轴、标题与图例直接显示目标状态。连续更新合并准备，动画不修改模型值、源索引、选择或视口。

新旧数据层任一超过 6,000 个已准备绘制项时直接显示结果，避免在大数据场景逐帧重建完整数据；单边逻辑尺寸超过 4,096 或面积超过 4,194,304 的图表也跳过动画。径向扇区按原始索引对应角度，新增扇区从零角度长出、移除扇区直接消失；需要跨重排保留业务身份时，由业务层先保持数据顺序。

关闭 `IsAnimationEnabled`、将时长设为零或通过 `Motion.Enabled=False` 关闭主题动效时，立即显示目标帧；脱离视觉树后停止动画。尺寸、样式、视口和导航变化直接应用。只读 `IsAnimating` / `AnimationProgress` 提供状态，进度通知在 UI 派发中发出，避免绘制过程中同步触发绑定修改。

动画期间暂不显示悬停覆盖层。指针／键盘输入或调用 `GetDataPointPosition`、`HitTestDataPoint`、`GetSharedDataPoints` 等几何查询，会先完成到目标帧，再按目标数据处理，确保命中与显示一致；查询本身不改变数据选择。需要逐帧读取数据坐标的业务应关闭更新动画。

更新动效的可选关闭方式参考 [Chart.js 更新图表](https://www.chartjs.org/docs/latest/developers/updates.html)；图表的数据层过渡与稳定导出参考 [ECharts 数据过渡](https://echarts.apache.org/handbook/en/how-to/animation/transition/)。

## PNG 图片导出

在 UI 线程调用 `ExportPng`，可写入可写流或文件路径。指定的 `Size` 使用逻辑像素，输出像素尺寸为 `ceil(逻辑尺寸 × dpi / 96)`：

```csharp
using Avalonia;

// 800 × 560 逻辑像素，192 DPI，输出 1600 × 1120 PNG。
using var output = File.Create("revenue.png");
RevenueChart.ExportPng(output, new Size(800, 560), 192);

// 透明画布，保留系列、标签和图例的主题颜色。
RevenueChart.ExportPng("revenue-transparent.png", new Size(800, 560), 96, true);
```

两个重载的参数均为 `ExportPng(destination, Size? size = null, double dpi = 96, bool transparentBackground = false)`，`destination` 为 `Stream` 或路径。省略尺寸时使用当前有效宽高，尚未布局时使用 `DefaultChartWidth` / `DefaultChartHeight`。导出使用独立的最终数据快照，动画未结束时也导出目标帧；保留视口及当前图例页，不包含十字准线、悬停／选择强调、焦点框或原生提示浮层，不修改原图的尺寸、视口、动画、选择或绑定。透明模式只跳过画布背景填充，保留数据标签所需的语义画笔。调用方负责所传流的生命周期。

逻辑宽高必须有限且大于零，DPI 必须在 `[16, 600]`；最终单边像素不能超过 8,192，总像素不能超过 16,777,216。尺寸检查在位图分配前执行，超出范围抛出 `ArgumentOutOfRangeException`；像素上限约对应 64 MiB 的 RGBA 数据，不代表导出过程的总内存上限。流必须非空且可写。导出为同步操作，大图片应在用户显式导出时调用。

文件选择可使用 Avalonia `StorageProvider.SaveFilePickerAsync` 并将 `IStorageFile.OpenWriteAsync()` 返回的流传入导出，适用于没有本地路径的存储提供程序。

## 数据更新与性能边界

`ChartPoint`、`ChartSeries` 继承 `AvaloniaObject`，其属性修改可自动刷新已挂载图表。`SeriesSource` 与每个系列的 `ItemsSource` 使用 `ObservableCollection<T>` 时，增删、替换和重排也自动刷新。替换 `SeriesSource` / `ItemsSource` 会重新订阅新数据；脱离视觉树时释放订阅，再次挂载后读取最新数据。

创建及修改图表模型、集合和调用 `RefreshData()` 均在 UI 线程执行。后台获取数据后，切回 UI 线程再应用：

```csharp
using Avalonia.Threading;

await Dispatcher.UIThread.InvokeAsync(() =>
{
    viewModel.Revenue[0].ItemsSource!.Add(new ChartPoint(5, 81, "五月"));
    viewModel.Revenue[0].ItemsSource![0].Y = 45;
});
```

普通 `List<ChartPoint>` 也可作为数据源；修改其内容后需调用 `RevenueChart.RefreshData()`，点属性通知仍自动生效。大量数据批量加载时，可一次替换 `ItemsSource`，或修改普通列表后统一刷新，避免逐项集合通知重复建立订阅。

该控件没有逐点 UI 容器，因此集合控件的容器回收面板不适用。数据准备、原始点定位索引与模型内存仍随数据量增长；采样减少绘制顶点，不代替分页或远程数据虚拟化。业务层应控制数据量，并为图表提供有限宽高。

## 主题与三档尺寸

`OneTheme` 设置背景、文字、网格及六色系列画笔；可通过 `Background`、`Foreground`、`GridBrush`、`MutedBrush`、`AccentBrush`、`SecondaryBrush`、`TertiaryBrush`、`QuaternaryBrush`、`FifthBrush`、`SixthBrush` 覆盖。默认系列色来自 `One.Chart.Series1` 至 `One.Chart.Series6`，按六色循环，随主题动态变化；`ChartSeries.Brush` 优先，导出保留六个画笔属性。`Classes="small"` / `normal` / `large` 或可继承的 `ControlSizing.Size` 同时改变期望宽高、字号、留白、线条、标记及仪表弧带，不只改变文字。

| 属性 | Small | Normal | Large |
| --- | --- | --- | --- |
| `DefaultChartWidth` / `DefaultChartHeight` | 320 / 224 | 400 / 280 | 480 / 336 |
| `FontSize` | 10 | 11 | 12 |
| `ChartPadding` | 8 | 12 | 16 |
| `StrokeThickness` | 2 | 2.5 | 3 |
| `MarkerRadius` | 3 | 3.5 | 4.25 |
| `SelectionThickness` | 1.5 | 2 | 2.5 |
| `SliceHoverOffset` | 4 | 6 | 8 |
| `SliceHoverScale` | 1.02 | 1.025 | 1.03 |
| `GaugeBandRatio` | 0.14 | 0.18 | 0.22 |

图表使用独立于标准表单控件的紧凑字号。仪表刻度字号为 9 / 10 / 11，中心数值为 20 / 22 / 24；标题在基础字号上增加 2，图例使用基础字号。PreviewHost 三种样式的可用宽度随尺寸为 240 / 300 / 360，不再固定为同一宽度。

这些值可在单个图表上覆盖。`DefaultChartWidth` / `DefaultChartHeight` 只提供默认期望尺寸；明确设置 `Width` / `Height` 或由 Grid 拉伸时，以消费布局为准。固定相同宽高下，三档仍改变图表内部密度。径向图会为扇区抬起预留边缘空间；坐标图按字号与留白重新计算绘图区及可容纳的刻度、标签。

## 验证场景

具体图表类型均有同名渲染入口，抽象基类没有。按改动选择必要场景，执行方式见[测试策略](../workflow/testing.md)：

| 需求 | 渲染名称 |
| --- | --- |
| 视口／双轴／采样 | ZoomedLineChart、PannedBarChart、DualYAxisChart、LargeSamplingChart |
| 准线／提示／图例 | CrosshairChart、SharedTooltipChart、InteractiveLegendChart |
| 标签／平滑／动画 | LabeledLineChart、SmoothLineChart、AnimatedChart |
| 悬停／仪表 | PieHoverChart、DoughnutHoverChart、BarHoverChart、NeedleGaugeChart、PrecisionGaugeChart |

PNG 导出 API 单独验证，不依赖截图命令。NativeAOT 接入见[对应文档](../native-aot.md)。
