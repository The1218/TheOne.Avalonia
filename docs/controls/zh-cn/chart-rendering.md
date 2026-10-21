# 图表更新、导出与尺寸

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

**简体中文** · [English](../en/chart-rendering.md)

数据模型与接入见[图表入口](charts.md)。

## 数据更新动画

全部 21 个继承 `ChartBase` 的图表控件统一使用以下动画设置，包括 Heatmap、Radar、Funnel、BoxPlot、Candlestick、Treemap 和 Sunburst。`AnimationStyle` 是经过枚举值校验的 `ChartAnimationStyle` 属性，默认 `Auto`。

| AnimationStyle | 首次非空展示 | 后续数据变化 |
| --- | --- | --- |
| `Auto` | 从左向右显露已准备的绘图区 | 兼容的坐标帧插值坐标，结构变化交叉淡化缓存数据层；饼图／环形图过渡角度，仪表过渡数值、弧线与指针；全部专用统计图和层级图淡入已准备的目标绘图区。 |
| `None` | 立即显示目标帧 | 立即显示目标帧 |
| `Fade` | 淡入已准备的绘图区 | 淡入已准备的目标绘图区 |
| `Reveal` | 从左向右显露 | 从左向右显露已准备的目标绘图区 |

```xml
<!-- 所在视图通过 x:DataType 声明 Revenue 属性的数据类型。 -->
<one:LineChart IsSmooth="True"
           SeriesSource="{CompiledBinding Revenue}"
           AnimationStyle="Reveal" AnimationDuration="0:0:0.2" />

<!-- 也可以通过主题类选择相同的动效。 -->
<one:LineChart SeriesSource="{CompiledBinding Revenue}"
           Classes="animation-fade" />
```

公开动画类为 `animation-fade`、`animation-reveal` 和 `animation-none`。同时添加多个类时，优先级固定为 **animation-none > animation-reveal > animation-fade**，与 `Classes` 中的书写顺序无关。显式设置的局部 `AnimationStyle` 值优先于这些类的 Setter；需要由 Classes 控制时，请去掉局部属性值。不指定动画类时使用默认 `Auto`。

`IsAnimationEnabled` 默认 `True`，`AnimationDuration` 为 `TimeSpan`，默认 200 ms，可设为零至 10 秒。首次非空绘图区只播放一次初始动效，卸载后重新挂载不会仅因重建订阅而重播；首次绘制之前的坐标或图例查询只准备目标坐标，不消耗初始动效。笛卡尔图的 `Auto` 数据过渡直接显示目标坐标轴；专用图的 `Auto` 更新和 `Fade` 一样淡入完整目标绘图区（包括坐标轴），`Reveal` 则从左向右裁剪显露已经准备的绘图区。标题与图例保持稳定。连续更新合并准备，不修改模型值、原始索引、抽样、命中、选择或视口。自动数据过渡中，径向扇区按原始索引对应角度，新增扇区从零角度长出、移除扇区直接消失；需要跨重排保留业务身份时，由业务层先保持数据顺序。

动画预算是整张图最多 **6,000 个已准备／绘制项**，不是 6,000 行原始数据，也不是每个系列各自拥有该预算。`Auto` 捕获旧笛卡尔／径向层时也检查其标记数；超出预算直接显示目标帧。单边逻辑尺寸超过 **4,096** 或逻辑面积超过 **4,194,304** 的图表同样跳过动画。抽样可减少实际绘制项，但不代表原始数据准备没有成本。

设置 `AnimationStyle=None`、关闭 `IsAnimationEnabled`、时长为零、`Motion.Enabled=False`、禁用或隐藏图表时，正在执行的动画立即完成到目标帧。脱离视觉树后停止回调并释放临时动画状态，旧过渡排队中的回调不能重新启动动画。动态切换动画样式会先结束当前过渡，下一次数据变化使用新样式。尺寸、主题、样式、视口和导航变化直接应用。只读 `IsAnimating` / `AnimationProgress` 提供状态，进度通知在 UI 派发中发出，避免绘制过程中同步触发绑定修改。

动画期间暂不显示悬停覆盖层。指针／键盘输入或调用 `GetDataPointPosition`、`HitTestDataPoint`、`GetSharedDataPoints` 等几何查询，会先完成到目标帧，再按目标数据处理，确保命中与显示一致；查询本身不改变数据选择。需要逐帧读取数据坐标的业务应关闭更新动画。

动画复用已经准备的绘制数据，不重新建立数据订阅。兼容笛卡尔帧的源点查找、Frame、点数组和标签锚点在每次过渡开始时准备一次，tick 只原位更新坐标；同一进度被重复绘制时复用已经准备的路径。标签保留目标布局并插值已有锚点，不在每个 tick 重算标签避让；Scatter 的文字排版缓存保留当前插值 Frame。饼图／环形图复用最近一次扇区几何和格式化标签，缓存数量受当前扇区数量限制；Fade／Reveal 复用目标几何，动画帧回调也只在每次过渡创建一次。这减少重复准备和分配，不消除可见标记的栅格工作，也不承诺固定帧耗时。PNG 导出始终绘制独立的**目标状态**快照，不受当前动画进度影响。

参考 [ECharts 数据过渡](https://echarts.apache.org/handbook/en/how-to/animation/transition/)。下方历史性能表保留原始日期与测量条件，不代表新增动画样式的测量结果。

## PreviewHost 创建与可见内容

每个图表保留独立页面和带名称的配置变体网格。首个样例立即创建，其他变体、离屏动画区及代码高亮示例仅在预留区域与真实可见视口相交时同步创建。滚动后仍可看到所有示例，不替换为单一选择器，也不删减展示内容。尚未创建时只在测量阶段估算占位，创建后使用内容的实际期望尺寸，不保留强制占位高度；已经创建的内容在滚动与重新挂载时复用。

静态小样例不再为每张图创建完整交互编辑器 ViewModel。每张静态图持有独立的 Series 包装，图例显隐互不影响；动画示例使用单独的点模型。交互编辑器仍在显式展开时创建。首次进入可见区的高亮编辑器仍有实际创建与布局成本，按需创建减少无关的离屏工作，不承诺消除可见内容的渲染成本，也不将工作挪到 Dispatcher 回调中。

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

`ChartPoint`、`ChartSeries` 继承 `AvaloniaObject`，其属性修改可自动刷新已挂载图表。可观察的 `SeriesSource` 与系列 `ItemsSource` 按集合成员差量维护订阅：Add / Remove / Replace 只处理变化的引用，Move 保留现有订阅；Reset 核对发出通知的集合，保留未变化模型的订阅。替换系列 `ItemsSource` 只读取该系列的新数据源；替换 `SeriesSource` 或显式刷新时核对当前系列及点集合，保留共享引用。脱离视觉树时释放订阅，再次挂载后读取最新数据。

同一个系列、点或点集合可以重复出现。每张图对同一模型只订阅一次，每个点集合只有一个集合事件处理器；最后一个持有者移除后才释放订阅。已移除的数据源和模型不再触发更新。订阅按引用身份计数，选择仍按源索引解析。

创建及修改图表模型、集合和调用 `RefreshData()` 均在 UI 线程执行。后台获取数据后，切回 UI 线程再应用：

```csharp
using Avalonia.Threading;

await Dispatcher.UIThread.InvokeAsync(() =>
{
    viewModel.Revenue[0].ItemsSource!.Add(new ChartPoint(5, 81, "五月"));
    viewModel.Revenue[0].ItemsSource![0].Y = 45;
});
```

普通 `List<ChartPoint>` 也可作为数据源；修改内容后需调用 `RevenueChart.RefreshData()`，点属性通知仍自动生效。批量加载可一次替换 `ItemsSource`，或修改普通列表后统一刷新，减少通知与失效次数。笛卡尔图表将数据范围、按索引堆积与绘制帧分开缓存：坐标修改只更新受影响列，集合结构或数据源变化时重建成员索引。若选择需要显露离屏点，仍可能在通知期间同步准备几何。

选择强调复用已准备的点、柱条和扇区位置。抽样的笛卡尔图表缓存原始坐标及不受选择影响的基础样本，未入样点只加入对应片段，不重新计算坐标轴、范围、堆积或命中索引。切换／清空选择会移除原来的额外样本，不累积历次选中点，并为当前样本重新准备标签。显露离屏点仍可能改变视口并重建坐标映射。选择操作先结束进行中的数据动画，再呈现目标数据上的选中效果。

该控件没有逐点 UI 容器，因此集合控件的容器回收面板不适用。数据准备、原始点定位索引与模型内存仍随数据量增长；采样减少绘制顶点，不代替分页或远程数据虚拟化。业务层应控制数据量，并为图表提供有限宽高。

### 第一轮订阅优化记录（2026-10-18）

以下为引入后文范围／堆积／绘制缓存之前的第一轮历史结果。范围为 13 类系列图表，Gauge 不使用系列订阅。公共属性和业务 API 保持不变。基线使用 `e083204` 中的图表实现，加入同一测量工具。环境为 macOS 27.0.1 / ARM64、.NET 10.0.0、Release、Skia headless；视口 900 × 420，3 个系列，每系列 1,000 或 10,000 点；关闭动画、标记和图例。普通场景关闭抽样，抽样场景使用 MinMax、阈值 100、预算 128。每场景预热 8 次、采样 32 次。下表两个进程均设置 `DOTNET_TieredCompilation=0`，这是测量条件，不是应用配置建议。

每格为**优化前 → 优化后**。通知时间为同步集合修改／选择耗时；完整周期包含该通知、Dispatcher、布局及 headless 渲染 tick，不代表 GPU 或屏幕呈现延迟。分配量为同步操作期间当前 UI 线程的中位字节数。增删场景每次操作产生两次通知，临时系列包含 8 个点；选择场景使用默认视口内的点。

| 每系列点数 | 场景 | 通知中位 ms | 完整周期中位 ms | 通知分配 B |
| ---: | --- | ---: | ---: | ---: |
| 1,000 | `point-add-remove` | 0.3640 → 0.0118 | 4.257 → 5.487 | 1,320,616 → 1,312 |
| 1,000 | `point-replace` | 0.2399 → 0.0159 | 4.408 → 6.008 | 660,512 → 1,048 |
| 1,000 | `point-move` | 0.2133 → 0.0098 | 4.031 → 5.949 | 660,488 → 600 |
| 1,000 | `series-add-remove` | 0.3532 → 0.0212 | 3.977 → 5.995 | 1,322,168 → 3,880 |
| 1,000 | `select-line` | 1.5376 → 0.0117 | 3.253 → 2.784 | 1,331,304 → 936 |
| 1,000 | `select-bar` | 1.4792 → 0.0239 | 4.868 → 6.562 | 1,577,112 → 936 |
| 1,000 | `select-sampled-endpoints` | 1.6316 → 0.0121 | 2.734 → 1.992 | 1,367,536 → 968 |
| 1,000 | `select-sampled-interior` | 1.5876 → 2.8806 | 2.675 → 5.094 | 1,367,632 → 1,367,664 |
| 10,000 | `point-add-remove` | 10.5573 → 0.0257 | 35.440 → 36.817 | 12,884,568 → 1,312 |
| 10,000 | `point-replace` | 2.7012 → 0.0212 | 29.985 → 30.110 | 6,442,488 → 1,048 |
| 10,000 | `point-move` | 2.9542 → 0.0192 | 31.798 → 29.950 | 6,442,464 → 600 |
| 10,000 | `series-add-remove` | 12.2339 → 0.0299 | 37.938 → 31.290 | 12,886,120 → 3,880 |
| 10,000 | `select-line` | 8.7050 → 0.0252 | 24.042 → 19.489 | 6,813,888 → 936 |
| 10,000 | `select-bar` | 12.6793 → 0.0328 | 60.736 → 42.757 | 6,821,840 → 936 |
| 10,000 | `select-sampled-endpoints` | 12.1532 → 0.0090 | 17.767 → 4.450 | 6,947,976 → 968 |
| 10,000 | `select-sampled-interior` | 11.0652 → 11.6682 | 16.240 → 16.860 | 6,948,064 → 6,948,096 |

32 次点 Replace / Move 操作中，原有源集合的枚举次数由 128 降为 0，集合事件处理器的添加／移除次数由 128/128 降为 0/0。点增删和系列增删场景中的原有集合也没有订阅重建；初始化临时系列的新数据源不计入这些计数。普通 Line / Bar 选择及已入样端点选择的几何失效、笛卡尔帧替换均由 32 次降为 0；选择未入样原始点仍产生 32 次重建。

第一轮降低了同步通知和分配量，完整绘制并非每个场景都更快。两轮默认分层 JIT 测量也有波动：每系列 10,000 点的 Bar 选择，完整周期中位数优化前为 31.91–36.98 ms，优化后为 25.73–42.10 ms；每系列 1,000 点的未入样点选择，优化前为 2.89–3.23 ms，优化后为 5.24–5.27 ms。这些历史结果是继续优化缓存的依据，不能据此宣称第一轮在这些路径上提升了帧率。

测量入口记录中位数、p95、分配量、源枚举、集合事件处理器变更、几何失效及帧替换次数。前后 JSON、默认 JIT 复测、日志和对比表保存在 `.preview/chart-refresh/`；上述正文表格为仓库内记录。结果是本机 headless 测量，不代表 Windows / Linux 实跑、NativeAOT 结果或生产延迟承诺。

### 笛卡尔图表的范围、堆积与绘制缓存

本轮影响 11 类笛卡尔图表：Line、Area、Scatter、Bar、HorizontalBar、三类 Stacked 和三类 PercentStacked。不改变 Pie、Doughnut、Gauge 的绘制，也不新增属性、Classes 或业务 API。

- 范围树按原始索引列保存 X 及各轴 Y 极值。单点坐标变化只重算受影响列及其聚合祖先；堆积／百分比仍按轴和正负方向独立计算，保留标准 X 缺口、零值与溢出处理。共享／重复点的同一列只更新一次，极值向内移动也无需扫描其他所有列。
- Y 更新保持有效性、片段拓扑、坐标轴映射及可见柱条槽位时，直接修补现有帧中的原始位置、命中桶／柱条和对应路径；堆积几何只修改受影响轴上的标记。数值抽样片段使用下文的桶内增量索引，无需重新扫描原始片段。
- 选择未入样原始点时，从已缓存原始坐标二分查找并插入基础样本，无需重新抽样全部原始点。只刷新原来／新选中片段的路径及抽样标签；清空选择恢复基础样本。指针、键盘、原始模型身份与离屏显露保持既有行为。
- 线与面积的 `StreamGeometry` 复用至对应片段变化；笛卡尔文字绘制使用每帧最多 512 项的缓存。样式、视口、布局和字体变化会生成新几何，文字和画笔继续跟随主题。不引入逐点控件或屏幕位图缓存。

以下情况保留回退：X 变化、缺口有效性变化、堆积面积正负分段变化、柱条零／非零切换、轴范围变化，以及裁剪柱条出现／消失时，重建绘制帧；范围和堆积缓存仍只更新受影响列。集合 Add / Remove / Move / Replace / Reset、源替换、显式 `RefreshData()`、可见性／轴归属变化会重建数据成员索引；卸载释放模型与范围缓存。开启 motion 且当前帧标记数不超过 6,000 时，保留既有有界数据动画的完整动画帧准备；原本因精确数据量过大而跳过动画的帧可走局部更新。

缓存保留模型成员、范围聚合及原始坐标，增加随数据量增长的常驻内存和首次准备工作。这是 CPU／内存复用，不是数据虚拟化；抽样与 `ShowMarkers="False"` 可减少绘制工作，但长路径栅格化或大量柱条绘制仍有成本。主题、字体和布局变化会使绘制缓存失效，但不会丢弃未变的数据聚合。局部／全量准备像素对照还覆盖明暗主题、三档尺寸下的 260 × 240 窄视口。

```csharp
// 在 UI 线程修改现有可观察模型，不需要新增 API。
series.ItemsSource![index].Y = nextValue;
chart.SelectedIndex = originalIndex; // 未入样点复用已准备的帧。
// 仅在静默修改普通列表内容后调用 RefreshData()。
```

### 第二轮几何缓存性能记录（2026-10-18）

本轮基线是开始本轮优化时已具备增量订阅的实现，冻结在 `before-artifacts`，不是前一表的 `e083204` 实现。前后均使用 macOS 27.0.1 ARM64、.NET 10.0.0、Release、Skia headless、`DOTNET_TieredCompilation=0`、900 × 420、3 个系列和相同正弦数据；关闭动画、标记和图例。除 `select-omitted` 使用 MinMax（阈值 100、预算 128）外，关闭抽样。每场景预热 16 次、测量 32 次，记录中位数和最近秩 p95。普通 Y 更新在现有范围内交替设置 21/22；`line-extremum` 交替设置 50/60，使自动范围变化；`line-x` 修改内部 X；`select-paint` 切换已有端点，不重新抽样。

每格为**优化前 → 优化后**。同步时间包含属性通知及其间同步选择准备；完整周期还包含 Dispatcher、布局及 headless 渲染。完整周期分配量为整个区间当前 UI 线程的字节数，不包含 GPU／屏幕呈现。帧替换统计 32 次操作中完整帧对象变化次数，不等同于底层栅格绘制次数。

| 每系列点数 | 场景 | 同步中位 ms | 完整周期中位 ms | 完整周期 p95 ms | 完整周期分配 B | 帧替换次数 |
| ---: | --- | ---: | ---: | ---: | ---: | ---: |
| 1,000 | `line-y` | 0.0056 → 0.0031 | 3.7089 → 1.3365 | 7.7179 → 1.6640 | 1,417,888 → 77,488 | 32 → 0 |
| 1,000 | `line-x` | 0.0026 → 0.0038 | 3.6178 → 3.3457 | 6.9697 → 7.2635 | 1,358,896 → 1,418,904 | 32 → 32 |
| 1,000 | `line-extremum` | 0.0028 → 0.0074 | 3.4599 → 3.2195 | 7.9621 → 6.8861 | 1,452,976 → 1,455,592 | 32 → 32 |
| 1,000 | `area-y` | 0.0028 → 0.0048 | 4.0124 → 1.6716 | 7.1083 → 2.2310 | 1,319,872 → 78,256 | 32 → 0 |
| 1,000 | `bar-y` | 0.0025 → 0.0058 | 5.2390 → 3.4257 | 6.7759 → 3.8791 | 1,703,552 → 90,992 | 32 → 0 |
| 1,000 | `stacked-bar-y` | 0.0025 → 0.0051 | 3.9355 → 1.4012 | 7.4242 → 1.8276 | 2,094,176 → 97,632 | 32 → 0 |
| 1,000 | `stacked-area-y` | 0.0023 → 0.0048 | 4.3504 → 2.0432 | 6.4923 → 4.4865 | 1,863,008 → 88,088 | 32 → 0 |
| 1,000 | `percent-bar-y` | 0.0035 → 0.0033 | 4.2589 → 1.4847 | 7.8183 → 1.8405 | 2,070,536 → 101,008 | 32 → 0 |
| 1,000 | `percent-area-y` | 0.0019 → 0.0036 | 3.6225 → 0.8801 | 7.5747 → 1.1873 | 1,847,512 → 91,512 | 32 → 0 |
| 1,000 | `select-omitted` | 1.7453 → 0.0030 | 2.9552 → 1.0034 | 6.4562 → 1.2988 | 1,463,832 → 80,064 | 32 → 0 |
| 1,000 | `select-paint` | 0.0028 → 0.0026 | 1.6558 → 1.3064 | 2.1664 → 1.5165 | 96,960 → 77,040 | 0 → 0 |
| 10,000 | `line-y` | 0.0075 → 0.0052 | 23.4702 → 11.9880 | 26.8788 → 12.4250 | 6,972,216 → 58,288 | 32 → 0 |
| 10,000 | `line-x` | 0.0060 → 0.0094 | 22.7644 → 20.0450 | 27.0378 → 22.4590 | 6,925,272 → 6,905,864 | 32 → 32 |
| 10,000 | `line-extremum` | 0.0060 → 0.0087 | 19.4625 → 16.8521 | 23.9716 → 19.6971 | 6,893,480 → 6,835,360 | 32 → 32 |
| 10,000 | `area-y` | 0.0101 → 0.0055 | 27.5882 → 13.8933 | 29.9900 → 14.2987 | 6,872,384 → 59,056 | 32 → 0 |
| 10,000 | `bar-y` | 0.0121 → 0.0057 | 43.6016 → 27.2284 | 52.2981 → 28.0128 | 7,057,032 → 220,456 | 32 → 0 |
| 10,000 | `stacked-bar-y` | 0.0118 → 0.0050 | 26.4908 → 9.0752 | 34.0675 → 9.8523 | 10,536,488 → 229,688 | 32 → 0 |
| 10,000 | `stacked-area-y` | 0.0079 → 0.0130 | 32.9540 → 15.4129 | 37.1892 → 15.8615 | 10,373,240 → 68,992 | 32 → 0 |
| 10,000 | `percent-bar-y` | 0.0070 → 0.0052 | 27.5493 → 9.5905 | 34.6526 → 10.4497 | 10,547,024 → 233,064 | 32 → 0 |
| 10,000 | `percent-area-y` | 0.0077 → 0.0050 | 25.1682 → 4.4891 | 30.7451 → 4.6916 | 10,282,248 → 72,192 | 32 → 0 |
| 10,000 | `select-omitted` | 9.4645 → 0.0033 | 13.4702 → 3.0072 | 20.5185 → 3.1740 | 6,981,264 → 61,920 | 32 → 0 |
| 10,000 | `select-paint` | 0.0083 → 0.0046 | 15.2876 → 11.3818 | 15.7846 → 11.7171 | 71,480 → 57,864 | 0 → 0 |

上述即时绘制场景中，内部 Y 更新和未入样点选择的整帧替换均由 32 次降为 0。每系列 10,000 点时，Line 的 Y 更新完整周期由 23.470 降至 11.988 ms，分配由 6,972,216 降至 58,288 B；StackedBar 的 Y 更新由 26.491 降至 9.075 ms。未入样点选择由 13.470 降至 3.007 ms，分配由 6,981,264 降至 61,920 B。X 和自动极值更新仍替换 32 帧，仅复用数据聚合，不承诺常数时间坐标映射；柱条／长路径栅格化仍反映在完整周期中。结果不是帧率保证，也不代表 Windows / Linux、GPU 或 NativeAOT 实跑。

JSON、前后构建产物、日志与包含同步分配及 p95 的完整对照保存在 `.preview/chart-geometry/`，上表为仓库内长期记录。保留的第一轮表格用于历史比较，其中未入样点导致整帧重建的行为已由本轮更新。

<a id="incremental-sampling-buckets"></a>
### 抽样桶内增量维护

本轮仅影响 **LineChart、AreaChart、ScatterChart、StackedAreaChart、PercentStackedAreaChart**，不新增公共属性、Classes 或 API。

- 完整准备仍使用线性 MinMax 抽样，同时缓存 X 单调片段的可见区间（包含两侧邻点）、按源顺序排列的桶边界，以及顶部／基线极值索引树。映射不变的 Y 更新只修补变化叶子的祖先、查询受影响桶，其他桶复用原代表点。共享模型会先完成全部变化叶子的维护，再查询桶。
- 保留每桶首尾、顶部和基线的最小／最大值。并列极值仍取源中最早的项，降序和相同 X 也遵守此规则。基础抽样之后再保留所选原始点；清空选择恢复基础样本。缺口／正负分段、按片段比例分配的**系列软预算**均不变；碎片很多时仍允许超出预算。
- 缓存区间外的原始点复用基础样本。关闭抽样、未达阈值、短片段和非单调片段直接复用精确原始点，不在每次 Y 更新时重复检查资格。初始未减点的抽样仍建立索引，后续极值变化可以正确减少样本。映射坐标非有限时保留线性回退。
- 缓存 V 个原始点、B 个非空桶时，k 个变化偏移、q 个受影响桶的抽样维护为 O(k log V + q log V + B log B)，无需扫描整个片段。最后合并样本、重绘受影响路径仍与保留样本量有关；堆积更新可能影响多个系列。这是抽样维护的成本，不代表整帧或 GPU 耗时。
- 首次准备增加 O(V) 索引工作及内存。树节点保存四个 32 位原始偏移，数组占 `32 × nextPowerOfTwo(V)` 字节（单片段 100,000 个可见原始点约 4 MiB），另有桶／对象开销及既有原始坐标缓存。未抽样片段没有索引树。X／拓扑／范围、视口／布局及抽样配置变化会重新准备索引；集合／数据源替换和卸载沿用既有缓存生命周期。不创建逐点控件或常驻位图。

### 第三轮桶内增量抽样性能记录（2026-10-18）

当前宿主的 Release 构建及 **3,212** 项专项检查通过；此次为私有缓存改动，未扩大运行无关图表／控件回归，也未执行原生 AOT 发布。

基线为本节之前的范围／堆积／绘制缓存实现，优化前二进制冻结在 `.preview/chart-sampling/before-artifacts`。前后均为 macOS 27.0.1 ARM64、.NET 10.0.0、Release、Skia headless、`DOTNET_TieredCompilation=0`、900 × 420、3 个系列，每系列 10,000／100,000 点，相同正弦数据。MinMax 阈值 100、每系列预算 128，关闭动画、标记及图例，固定 Y 范围 0–150，内部点 Y 交替 21/22；`offscreen-y` 为 LineChart，在 8 倍缩放后修改第 10 个原始点。每场景预热 16 次、测量 32 次；索引初建发生在预热前，不包含在单点更新耗时内。

每格为**优化前 → 优化后**。同步为属性通知耗时；完整周期包含 Dispatcher／布局／headless 绘制，分配为该区间当前 UI 线程字节数。p95 取最近秩。最后一列为每次操作在完整抽样扫描循环中执行的迭代数（X 顺序检查及可见桶扫描），不是索引查询读取次数。12 个场景前后均无整帧替换。

| 每系列点数 | 场景 | 同步中位 ms | 同步 p95 ms | 完整周期中位 ms | 完整周期 p95 ms | 完整周期分配 B | 完整扫描迭代／次 |
| ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| 10,000 | `line-y` | 0.1687 → 0.0090 | 0.2113 → 0.0202 | 1.1218 → 0.9095 | 1.9954 → 1.5663 | 72,048 → 80,968 | 19,999 → 0 |
| 10,000 | `area-y` | 0.1644 → 0.0136 | 0.1784 → 0.0505 | 1.3065 → 1.2529 | 1.7880 → 1.8137 | 72,824 → 69,360 | 19,999 → 0 |
| 10,000 | `scatter-y` | 0.1580 → 0.0130 | 0.2007 → 0.0462 | 1.7535 → 1.7287 | 4.8027 → 4.3322 | 464,416 → 460,952 | 19,999 → 0 |
| 10,000 | `stacked-area-y` | 0.4743 → 0.0270 | 0.5143 → 0.0808 | 2.6304 → 2.3020 | 3.1013 → 2.6702 | 97,704 → 87,232 | 59,997 → 0 |
| 10,000 | `percent-area-y` | 0.4605 → 0.0144 | 0.4998 → 0.0528 | 0.9243 → 0.4941 | 1.3244 → 0.7506 | 94,584 → 97,872 | 59,997 → 0 |
| 10,000 | `offscreen-y` | 0.0697 → 0.0022 | 0.0826 → 0.0094 | 0.4120 → 0.3362 | 0.5005 → 0.4303 | 62,152 → 53,880 | 11,252 → 0 |
| 100,000 | `line-y` | 1.4753 → 0.0084 | 1.5100 → 0.0196 | 2.3470 → 0.8684 | 2.5056 → 0.9485 | 71,040 → 67,576 | 199,999 → 0 |
| 100,000 | `area-y` | 1.4826 → 0.0083 | 1.5325 → 0.0194 | 2.6150 → 1.1129 | 2.7682 → 1.3202 | 71,816 → 68,352 | 199,999 → 0 |
| 100,000 | `scatter-y` | 1.4755 → 0.0184 | 1.5067 → 0.0620 | 3.0988 → 1.8026 | 3.4598 → 4.0338 | 462,520 → 459,056 | 199,999 → 0 |
| 100,000 | `stacked-area-y` | 4.4181 → 0.0296 | 4.5017 → 0.0924 | 6.5371 → 2.2821 | 6.7417 → 2.6422 | 96,696 → 86,224 | 599,997 → 0 |
| 100,000 | `percent-area-y` | 4.4163 → 0.0142 | 4.5329 → 0.0396 | 4.9335 → 0.4670 | 5.1659 → 0.7693 | 105,656 → 95,184 | 599,997 → 0 |
| 100,000 | `offscreen-y` | 0.6460 → 0.0031 | 0.7860 → 0.0382 | 1.0432 → 0.3682 | 1.4949 → 0.6863 | 64,784 → 54,176 | 112,502 → 0 |

<a id="scatter-rendering-tail"></a>
### Scatter 绘制尾延迟

本轮只修改 **ScatterChart**，不新增公共属性、Classes 或 API。抽样、极值／并列值／缺口、系列软预算、原始点命中、所选点保留及显露均沿用现有数据路径。`ShowMarkers=False` 仍不会隐藏散点。

上一轮 32 次样本的 p95 单值不足以证明稳定回退，本轮冻结当前控件库，每场景测量三轮、每轮 256 次。预算 2000 时，基线三个系列准备 3,990 个点，逐点记录椭圆，并为每个可见点创建可变 Pen。基线三轮 p95 的中位值：更新通知 0.2109 ms，Dispatcher／布局阶段（包含绘制命令记录）104.4647 ms，渲染 Tick 阶段 18.6441 ms。合并样本中最慢的 5% 共 39 次，全部观察到 GC。这些数据支持“命令记录与分配压力是主要尾延迟来源”，不代表所有慢帧都只能归因于 GC。

- 内置纯色填充及空／纯色描边，每个非空片段记录一条自定义绘制操作，共享不可变描边和缓存坐标。合成器只读取不可变坐标／画刷，不读取可变模型列表；映射不变的选择现在复用基础坐标，按源顺序插入所选标记，详见下文选择 p99 优化。
- 仍按原始系列／点顺序逐个绘制椭圆，不合并路径、不量化坐标、不合并透明重叠。保持既有外层裁剪、透明度／禁用态、半径／描边设置及 PNG 导出结果。
- 内置可变纯色画刷按不同实例订阅一次；原地修改颜色／透明度会刷新绘制，绘制后解除不再使用的画刷订阅，动画结束也会清理，卸载时全部释放。渐变、VisualBrush、平铺和其他自定义画刷保留原生椭圆路径及其资源行为。
- 优化减少命令记录和逐点托管分配，不消除光栅化工作。当前基础坐标／索引数组额外占每个保留样本容量槽约 20 字节，另有少量片段／操作对象开销（最初批量实现只有坐标，为 16 字节）；关闭抽样时容量可能等于完整原始片段。原始索引、空间命中桶和抽样树不变。大量可见散点仍需逐点填充／描边，不引入位图缓存或逐点控件。

#### Scatter 性能记录（2026-10-18）

基线为本次修改前冻结的当前工作区控件库，已包含上一轮桶内增量抽样。前后使用相同工作负载：macOS 27.0.1 ARM64、.NET 10.0.0、Release、Skia headless、`DOTNET_TieredCompilation=0`、900 × 420、3 个相同正弦数据系列；关闭动画、图例和可选标记修饰，ScatterChart 本身仍绘制散点。MinMax 阈值 100，预算 128／2000，固定 Y 范围 0–150。`y` 场景将中间原始点的 Y 在 21/22 间交替；选择场景切换未入样索引 50,013／50,017，或已入样端点 0／99,999。

每场景三轮，每轮预热 32 次后测量 256 次；强制 GC 在每轮计时前执行，不计构建、预热、JSON 写入和 PNG 截图。下表的中位、p95、p99 和分配中位按每场景合并的 768 次样本计算，使用最近秩分位数；GC 列为样本区间内观察到的 0／1／2 代收集次数总和，各代计数相互包含。全部前后对照均保持相同已准备点数，抽样完整扫描和整帧替换次数均为 0。

每格为**优化前 → 优化后**，耗时单位 ms，分配为当前 UI 线程字节数。完整周期包含属性更新、Dispatcher／布局、绘制命令记录和 headless 渲染 Tick，不代表 GPU／屏幕延迟，也不保证其他设备的帧率。

| 场景 | 已准备点数 | 完整周期中位 ms | 完整周期 p95 ms | 完整周期 p99 ms | UI 分配中位 B | GC0/1/2 |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| `10k-y-128` | 252 | 1.6340 → 1.4590 | 4.1231 → 2.3327 | 5.5977 → 6.9196 | 460,912 → 66,568 | 62/40/20 → 23/22/18 |
| `100k-y-128` | 252 | 1.6133 → 1.3719 | 2.4394 → 1.6246 | 5.3314 → 4.5009 | 459,016 → 73,664 | 45/45/3 → 9/9/3 |
| `100k-y-2000` | 3,990 | 24.4545 → 17.0141 | 104.2413 → 17.7369 | 189.1705 → 18.5633 | 6,157,152 → 182,360 | 592/260/103 → 24/15/9 |
| `100k-select-omitted` | 253 | 1.5998 → 1.3731 | 2.4083 → 1.5557 | 4.3437 → 4.4983 | 457,112 → 70,336 | 45/21/3 → 9/6/3 |
| `100k-select-existing` | 252 | 1.5800 → 1.3619 | 2.4603 → 1.4900 | 4.2790 → 4.4625 | 452,304 → 65,448 | 42/18/3 → 6/3/3 |

`100k-y-128` 三轮 p95 范围从 2.4165–2.6290 降至 1.6031–1.6448 ms；预算 2000 从 104.2413–105.2264 降至 17.6292–17.8710 ms。后者 Dispatcher／布局 p95 的三轮中位值从 104.4647 降至 0.1480 ms，但渲染 Tick p95 仍为 17.2819 ms，区分了已消除的记录／分配瓶颈与仍存在的逐点光栅化成本。并非所有指标都改善：10k Y 与 100k 选择场景的 p99 仍有波动或上升，表中保留实测值，不承诺所有极端帧均下降。

<a id="scatter-selection-p99"></a>
### Scatter 选择 p99 与分配复用

本次仅修改 **ScatterChart**，不新增公共属性、Classes 或 API。基线是上一轮批量椭圆实现，不是更早的逐点创建 Pen 实现。绘制计数显示：前后每次完成选择均为三个系列各一批，包括慢帧；连续八次选择也只在最终绘制三批。重复赋相同索引的记录／光栅批数和测得分配均为 0，未发现可以删去的重复整帧绘制。

仍可避免的工作集中在准备阶段：复制选择样本和坐标、重建未变的 Pen／画刷操作，以及坐标文字反复排版。分配采样观察到字形数组、已排版文本运行和文本行对象。Avalonia 的 [FormattedText.Draw](https://github.com/AvaloniaUI/Avalonia/blob/12.1.4/src/Avalonia.Base/Media/FormattedText.cs) 在绘制时枚举并格式化文本行，仅缓存 FormattedText 对象没有保留这些行。因此改用 [TextLayout](https://github.com/AvaloniaUI/Avalonia/blob/12.1.4/src/Avalonia.Base/Media/TextFormatting/TextLayout.cs) 保留坐标／数据标签的排版结果，布局决策仍沿用原 FormattedText 度量。分配采样独立于性能计时；分配 Tick 是采样证据，不能当作逐类型精确字节统计。

- 未入样的选中点使用基础样本上的不可变单点拼接视图，数量、源索引顺序、同步抽样诊断及选择事件保持不变。绘制端保留基础坐标和源索引，在原始绘制位置插入所选标记，单纯切换选择无需复制基础坐标数组。清空选择恢复同一基础样本；缺口、并列极值、软预算溢出和原始命中桶均不变。
- 复用未变的标记画刷、绘制操作，以及坐标轴／选中环／焦点边框的四个 Pen 槽。可变 Pen 画刷继续按原生资源规则更新；颜色、透明度、半径、描边、源或映射变化会刷新对应状态。切换已入样点可复用全部标记操作实例，切换额外保留点只替换对应操作。仍按源顺序绘制所有需要的椭圆，保留透明重叠。
- 坐标／数据标签的 TextLayout 每帧最多缓存 512 项，显式保留文化、字体、字号及文字方向；数据标签度量也复用缓存。淘汰、旧帧不再参与动画、卸载和导出 finally 路径都会 Dispose；当前帧、有效动画源及复用的插值帧保留缓存，不再使用的插值帧会被清理。标题／图例文字仍走原路径。
- 标记缓存容量约每槽 20 字节（坐标及源索引），额外选择状态为常量大小，替代随样本数增长的复制。文字缓存增加有界字形资源及首次排版工作；512 项不是任意超长标签的字节上限。不引入位图缓存，不跳过合法散点，不创建逐点控件。

#### 选择性能记录（2026-10-18）

前后均使用 macOS 27.0.1 ARM64、.NET 10.0.0、Release Skia headless、`DOTNET_TieredCompilation=0`、900 × 420 视口、三个各 100,000 个正弦数据点的系列。MinMax 阈值为 100，预算为 128，名称带 2000 的场景预算为 2000；固定 Y 范围 0–150，关闭图例和动画。已入样选择交替选择两个端点；未入样选择交替选择原始索引 50013/50017；burst 在一次绘制前连续改变八次选择；noop 重复设置索引 0。每场景三轮，每轮预热 32 次、测量 256 次。表格合并**每场景全部 768 次操作**，采用最近秩分位数，并非取三轮 p99 的中位数。GC 列累计三轮计时区间内观察到的收集次数，轮前主动 GC 不计入。

每格为**优化前 → 优化后**。完整周期包括同步更新、Dispatcher／布局和渲染 Tick；分配为该区间当前 UI 线程字节数的中位数，不包含其他线程和原生内存。入样点数为三个系列合计，前后完全相同；预算是每系列软上限，不保证恰好保留该数量。

| 场景 | 入样点数 | 完整周期中位 ms | 完整周期 p95 ms | 完整周期 p99 ms | 完整周期分配 B | GC 0/1/2 |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| `100k-select-omitted` | 253 | 1.3835 → 1.3026 | 1.6037 → 1.4222 | 4.5730 → 1.6409 | 70,360 → 13,144 | 9/9/3 → 3/3/3 |
| `100k-select-existing` | 252 | 1.4172 → 1.3518 | 1.7393 → 1.6572 | 2.3352 → 2.0820 | 65,472 → 12,960 | 6/3/3 → 3/3/3 |
| `100k-select-omitted-2000` | 3,991 | 17.0943 → 16.8964 | 17.9505 → 17.7998 | 19.0080 → 19.7188 | 140,400 → 13,408 | 21/15/9 → 9/9/9 |
| `100k-select-existing-2000` | 3,990 | 17.0967 → 16.9578 | 18.2200 → 17.5817 | 20.3327 → 18.0221 | 65,736 → 13,224 | 12/9/9 → 9/9/9 |
| `100k-select-noop` | 252 | 0.0000 → 0.0000 | 0.0001 → 0.0001 | 0.0001 → 0.0002 | 0 → 0 | 0/0/0 → 0/0/0 |
| `100k-select-burst` | 253 | 1.4085 → 1.3114 | 1.7252 → 1.4407 | 5.1392 → 1.7403 | 97,160 → 16,360 | 9/6/3 → 3/3/3 |

预算 128 的未入样选择 p99 降低 64.1%，分配中位数降低 81.3%；连续八次选择分别降低 66.1% 和 83.2%。全部场景前后整帧替换、重新扫描抽样点均为 0。未入样选择／连续选择的坐标快照构建由每操作一次降至 0；完成一次选择仍是三批记录、三批光栅绘制，noop 均为 0。这些计数证明减少了准备工作，不能据此声称减少了实际光栅批次。

**预算 2000 的未入样选择 p99 仍未解决**：虽然分配降低 90.5%，p99 从 19.0080 升至 19.7188 ms。优化后三轮 p99 分别为 17.7902、17.7183、30.0700 ms；最慢八次操作合计 99.64% 的周期位于渲染 Tick。最长三次的椭圆循环内测得分配和 GC 均为 0，仍为三批绘制，快照构建和整帧替换均为 0。椭圆循环自身 p99 从 18.1675 升至 19.0575 ms。剩余尾延迟因此定位在实际光栅工作及其调度区间；当前计时无法区分原生执行和操作系统调度暂停，也未证明这部分尾延迟来自重复绘制或托管分配。

光栅阶段仍可观察到偶发分配：优化后预算 2000 未入样场景中，9/768 次操作的椭圆循环分配非零，中位数为 0 B、p99 为 2,024 B、最大 6,264 B；优化后所有场景最大为 6,896 B。整帧并非零分配。独立分配采样仍观察到 Dispatcher／绘制上下文和 Skia 相关对象，尚未取得这些罕见分配的调用栈归因。因此不承诺所有 p99 均下降、无 GC 或生产帧率。

审计后的原始数据和完整对照在 `.preview/scatter-selection/audited-before.json`、`audited-after.json`、`comparison.md`。前后宿主使用相同的实体 PreviewHost 入口程序集；控件库 SHA-256 前缀分别为 `A63960DF4F35181C`／`0AE51A90D0C051B4`。JSON 保存完整哈希、路径和模块 ID，入口核验已加载模块与文件一致。中途 `confirm-*.json` 和 `final-confirm-*.json` 使用符号链接入口，可能解析至错误控件库，已作为无效证据保留并从表格中排除。较早的实体宿主 `before.json`／`after.json` 保留为独立观察，不替换本次不利样本。

Release 构建 0 错误、2 条既有无关可空性警告；**1,004 项 ScatterChart 专项检查通过**，包括 143 张原生参考 PNG 精确对照。参考图通过实体宿主重新加载原始逐椭圆控件库（`7441CE01E5708203…`）生成，独立于两个性能测量版本；完整身份在 `capture-audited.log`，优化版身份在 `verify-audited.log`。未运行全图表／控件回归或 AOT 发布。这些结果仅代表当前宿主 headless 实跑，GPU／屏幕呈现延迟及其他系统尚未验证。

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

图表使用独立于标准表单控件的紧凑字号。仪表刻度字号为 9 / 10 / 11，中心数值为 20 / 22 / 24；标题在基础字号上增加 2，图例使用基础字号。PreviewHost 图表样式的可用宽度随尺寸为 240 / 300 / 360，不再固定为同一宽度。

这些值可在单个图表上覆盖。`DefaultChartWidth` / `DefaultChartHeight` 只提供默认期望尺寸；明确设置 `Width` / `Height` 或由 Grid 拉伸时，以消费布局为准。固定相同宽高下，三档仍改变图表内部密度。径向图会为扇区抬起预留边缘空间；坐标图按字号与留白重新计算绘图区及可容纳的刻度、标签。

## 验证场景

下表保留原专项渲染场景的历史记录，对应执行入口已移除。

| 需求 | 渲染名称 |
| --- | --- |
| 视口／双轴／采样 | ZoomedLineChart、PannedBarChart、DualYAxisChart、LargeSamplingChart |
| 准线／提示／图例 | CrosshairChart、SharedTooltipChart、InteractiveLegendChart |
| 标签／平滑／动画 | LabeledLineChart、SmoothLineChart、AnimatedChart |
| 悬停／仪表 | PieHoverChart、DoughnutHoverChart、BarHoverChart、NeedleGaugeChart、PrecisionGaugeChart |

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### ChartBase

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`AccentBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L297) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 主序列／强调色画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`AnimationDuration`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L52) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(200)` | 数据过渡的持续时间；是否启用动画还受控件开关和 Motion.Enabled 约束。 校验：`value => value >= TimeSpan.Zero && value <= TimeSpan.FromSeconds(10)` |
| [`AnimationProgress`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L66) | `double` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`AnimationStyle`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L59) | `TheOne.Avalonia.Controls.ChartAnimationStyle` | 可写; OneWay | `0 / default` | 首次非空展示 ; 后续数据变化 校验：`value => Enum.IsDefined(value)` 可选值：`Auto`, `None`, `Fade`, `Reveal` |
| [`Background`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L273) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 背景画刷；主题相关背景使用 DynamicResource，避免固定颜色破坏明暗切换。 |
| [`ChartPadding`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L53) | `double` | 可写; OneWay | `12` | 8 ; 12 ; 16 校验：`value => double.IsFinite(value) && value >= 0` |
| [`CrosshairPosition`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L54) | `Avalonia.Point?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`DefaultChartHeight`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L47) | `double` | 可写; OneWay | `280` | 320 / 224 ; 400 / 280 ; 480 / 336 校验：`value => double.IsFinite(value) && value > 0` |
| [`DefaultChartWidth`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L40) | `double` | 可写; OneWay | `400` | 320 / 224 ; 400 / 280 ; 480 / 336 校验：`value => double.IsFinite(value) && value > 0` |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L255) | `string` | 可写; OneWay | `"暂无数据"` | `暂无数据` |
| [`FifthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L321) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第五序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`FontFamily`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L333) | `Avalonia.Media.FontFamily` | 可写; OneWay | `inherited registration` | 文本绘制使用的字体族；字体回退和实际度量由 Avalonia 文本系统处理。 |
| [`FontSize`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L339) | `double` | 可写; OneWay | `inherited registration` | 10 ; 11 ; 12 |
| [`Foreground`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L279) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 前景画刷；自定义时保持正文、状态与禁用对比度。 |
| [`GaugeBandRatio`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L83) | `double` | 可写; OneWay | `0.18` | 沿用三档主题密度；基础、进度、等级、气压表按结构缩放带宽，可显式调整该比例 校验：`value => double.IsFinite(value) && value > 0 && value <= 0.5` |
| [`GaugeMajorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L34) | `int` | 可写; OneWay | `5` | Number of major ticks, including the minimum and maximum. 校验：`value => value >= 2 && value <= 25` |
| [`GaugeMinorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L41) | `int` | 可写; OneWay | `2` | Maximum minor marks between consecutive major ticks. Compact gauges reduce them to keep readable spacing. 校验：`value => value >= 0 && value <= 10` |
| [`GridBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L285) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 绘图区网格线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`HoveredPoints`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L55) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.ChartSelection>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`InnerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L207) | `double` | 可写; OneWay | `0.62` | 径向图内半径比例；控制中空区域，保留扇区数值和比例含义。 校验：`value => double.IsFinite(value) && value >= 0 && value < 1` |
| [`IsAnimating`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L65) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsAnimationEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L46) | `bool` | 可写; OneWay | `true` | 启用该图表的数据过渡；全局 Motion.Enabled=False 时仍关闭动画。 |
| [`IsLegendInteractive`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L27) | `bool` | 可写; OneWay | `true` | `True`，允许坐标图图例切换系列及键盘操作 |
| [`IsPanEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L67) | `bool` | 可写; OneWay | `false` | 默认 `False`；启用鼠标左键拖动及键盘平移 |
| [`IsSmooth`](../../../TheOne.Avalonia/Controls/ChartBase.Smoothing.cs#L12) | `bool` | 可写; OneWay | `false` | `False`，折线／普通面积的单调平滑路径 |
| [`IsZoomEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L60) | `bool` | 可写; OneWay | `false` | 默认 `False`；启用滚轮及键盘缩放 |
| [`Kind`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L163) | `TheOne.Avalonia.Controls.ChartKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`LegendPageCount`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L42) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读分页状态，页索引从 0 开始 |
| [`LegendPageIndex`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L33) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读分页状态，页索引从 0 开始 |
| [`MarkerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L59) | `double` | 可写; OneWay | `3.5` | 3 ; 3.5 ; 4.25 校验：`value => double.IsFinite(value) && value > 0` |
| [`Maximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L231) | `double` | 可写; OneWay | `100` | 主数值轴上界；空值使用自动范围，不通过裁剪更改数据本身。 校验：`double.IsFinite` |
| [`Minimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L225) | `double` | 可写; OneWay | `0 / default` | 主数值轴下界；空值由数据自动推导，显式范围必须与数据及 Maximum 协调。 校验：`double.IsFinite` |
| [`MutedBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L291) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 次要文字与辅助标记画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`NavigationAxes`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L73) | `TheOne.Avalonia.Controls.ChartNavigationAxes` | 可写; OneWay | `ChartNavigationAxes.Both` | 默认 `Both`；可设为 `None`、`Horizontal`、`Vertical`，限制导航轴 校验：`value => (value & ~ChartNavigationAxes.Both) == 0` 可选值：`None`, `Horizontal`, `Vertical`, `Both` |
| [`QuaternaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L315) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第四序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`RenderedPointCount`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L113) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SamplingActive`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L114) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SamplingMode`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L94) | `TheOne.Avalonia.Controls.ChartSamplingMode` | 可写; OneWay | `ChartSamplingMode.MinMax` | 密集序列的采样算法；可选策略见枚举，采样影响绘制点数，不替换原始数据。 校验：`value => value is ChartSamplingMode.None or ChartSamplingMode.MinMax` 可选值：`None`, `MinMax` |
| [`SamplingPointBudget`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L107) | `int` | 可写; OneWay | `2000` | A per-series sampling target; gap boundaries and selected originals can exceed it. 校验：`value => value >= 8` |
| [`SamplingThreshold`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L100) | `int` | 可写; OneWay | `2000` | 启用采样的点数阈值；结合视口与 SamplingMode 调整大数据绘制成本。 校验：`value => value >= 2` |
| [`SecondaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L303) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第二序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`SecondaryUnit`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L60) | `string?` | 可写; OneWay | `null` | 第二数值轴的单位后缀，与主轴单位独立。 |
| [`SecondaryValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L54) | `string` | 可写; OneWay | `"0.##"` | 第二数值轴的数值格式字符串；使用符合 .NET 数值格式的值。 |
| [`SecondaryYAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L48) | `string?` | 可写; OneWay | `null` | 第二数值轴标题；不改变序列使用哪一条数值轴。 |
| [`SecondaryYMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L36) | `double` | 可写; OneWay | `double.NaN` | 第二数值轴上界；空值自动计算，与 SecondaryYMinimum 配合使用。 校验：`value => !double.IsInfinity(value)` |
| [`SecondaryYMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L30) | `double` | 可写; OneWay | `double.NaN` | 第二数值轴下界；空值自动计算，与 SecondaryYMaximum 配合使用。 校验：`value => !double.IsInfinity(value)` |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L267) | `int` | 可写; TwoWay | `-1` | 零基系列 / 点索引，支持双向绑定；点索引 `-1` 清除选择 |
| [`SelectedSeriesIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L261) | `int` | 可写; TwoWay | `0 / default` | 零基系列 / 点索引，支持双向绑定；点索引 `-1` 清除选择 |
| [`Selection`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L345) | `TheOne.Avalonia.Controls.ChartSelection?` | 只读; OneWay | `computed / state` | 只读 `ChartSelection?`，包含索引以及原始 `Series` / `Point` 引用 |
| [`SelectionThickness`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L65) | `double` | 可写; OneWay | `2` | 1.5 ; 2 ; 2.5 校验：`value => double.IsFinite(value) && value > 0` |
| [`SeriesSource`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L165) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartSeries>?` | 可写; OneWay | `null` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`ShowCrosshair`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L42) | `bool` | 可写; OneWay | `false` | `False`，开启坐标图指针十字准线 |
| [`ShowDataLabels`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L195) | `bool` | 可写; OneWay | `false` | `False`，坐标图显示数据值，饼图／环形图显示可容纳的百分比 |
| [`ShowGaugeNeedle`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L21) | `bool` | 可写; OneWay | `false` | 主题按枚举提供默认值；显式 False 隐藏对应元素。气压表的刻度开关同时作用于两套轴 |
| [`ShowGaugeTicks`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L27) | `bool` | 可写; OneWay | `false` | 主题按枚举提供默认值；显式 False 隐藏对应元素。气压表的刻度开关同时作用于两套轴 |
| [`ShowGrid`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L183) | `bool` | 可写; OneWay | `true` | `True`，坐标图网格 |
| [`ShowLegend`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L177) | `bool` | 可写; OneWay | `true` | `True`，坐标图为系列图例，饼图／环形图为类别图例 |
| [`ShowMarkers`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L189) | `bool` | 可写; OneWay | `true` | `True`，折线／面积图的数据标记 |
| [`ShowSecondaryAxis`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L66) | `bool` | 可写; OneWay | `true` | 显示第二数值轴；序列轴归属仍由序列模型配置。 |
| [`SixthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L327) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第六序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`SliceHoverOffset`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L71) | `double` | 可写; OneWay | `6` | 4 ; 6 ; 8 校验：`value => double.IsFinite(value) && value >= 0` |
| [`SliceHoverScale`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L77) | `double` | 可写; OneWay | `1.025` | 1.02 ; 1.025 ; 1.03 校验：`value => double.IsFinite(value) && value >= 1 && value <= 1.2` |
| [`StrokeThickness`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L201) | `double` | 可写; OneWay | `2.5` | `2.5`，必须大于零且有限 校验：`value => double.IsFinite(value) && value > 0` |
| [`TertiaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L309) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 第三序列画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`Title`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L171) | `string?` | 可写; OneWay | `null` | 可选标题 |
| [`TooltipMode`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L48) | `TheOne.Avalonia.Controls.ChartTooltipMode` | 可写; OneWay | `0 / default` | `ChartTooltipMode.Single`，可设为 `Shared` 查看同位置的多个系列 校验：`value => value is ChartTooltipMode.Single or ChartTooltipMode.Shared` 可选值：`Single`, `Shared` |
| [`Unit`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L249) | `string?` | 可写; OneWay | `null` | `0.##` / 空，使用当前区域设置格式化数值 |
| [`Value`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L237) | `double` | 可写; OneWay | `0 / default` | 当前值；单位、范围及空值含义以所属模型和校验条件为准。 校验：`double.IsFinite` |
| [`ValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L243) | `string` | 可写; OneWay | `"0.##"` | `0.##` / 空，使用当前区域设置格式化数值 |
| [`Viewport`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L53) | `Avalonia.Rect` | 可写; TwoWay | `new Rect(0, 0, 1, 1)` | A normalized window into full data extents, bounded to [0,1] and at most 1,000× zoom. 校验：`value => double.IsFinite(value.X) && double.IsFinite(value.Y) && double.IsFinite(value.Width) && double.IsFinite(value.Height) && value.Width > 0 && value.Height > 0` |
| [`YAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L42) | `string?` | 可写; OneWay | `null` | 主数值轴标题，参与实际文本度量与轴区域布局。 |
| [`YMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L219) | `double` | 可写; OneWay | `double.NaN` | `NaN` 表示自动范围，可设置有限 Y 轴范围 校验：`value => !double.IsInfinity(value)` |
| [`YMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L213) | `double` | 可写; OneWay | `double.NaN` | `NaN` 表示自动范围，可设置有限 Y 轴范围 校验：`value => !double.IsInfinity(value)` |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

- `ChartBase`: `Classes="animation-fade"` / `Classes="animation-none"` / `Classes="animation-reveal"`

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `double? ChartBase.GetStackPercentage(int seriesIndex, int pointIndex)` | A signed share of the matching axis/category/sign total in a percentage stack, or null otherwise. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.ExportPng(Stream stream, Size? size = null, double dpi = 96, bool transparentBackground = false)` | Render a stable target-state PNG on the UI thread without altering this chart. Size is logical pixels; DPI scales the output. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.ExportPng(string path, Size? size = null, double dpi = 96, bool transparentBackground = false)` | 将当前图表绘制为 PNG；目标尺寸、缩放与输出流参数见签名，调用方负责流生命周期。 |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `IReadOnlyList<ChartSelection> ChartBase.GetSharedDataPoints(Point point)` | Nearest visible numeric X (exact matches per series), or the pointed categorical index. No interpolation. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `Rect? ChartBase.GetDataLabelBounds(int seriesIndex, int pointIndex)` | Returns the target bounds of a visible numeric data label, or null when it is omitted or labels are disabled. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `Rect? ChartBase.GetLegendItemBounds(int seriesIndex)` | 当前页系列图例的局部 `Rect?` |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `int? ChartBase.HitTestLegend(Point point)` | 指针下的系列索引；没有命中时为 `null` |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `Rect? ChartBase.GetLegendPageNavigationBounds(bool next)` | 获取下一页（True）或上一页（False）区域的局部 `Rect?` |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `bool ChartBase.PageLegend(int delta)` | 按页移动，返回是否改变当前页 |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `bool ChartBase.ToggleSeriesVisibility(int seriesIndex)` | Toggle model visibility, retaining the series' binding. Independent of input/legend flags. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.Zoom(double factor, Point? anchor = null)` | Zoom by a positive factor around fractional coordinates in the current view. Factors above one zoom in. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.Pan(double horizontal, double vertical)` | Pan by fractions of the visible spans. Positive deltas move towards larger axis values/category indices. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.ResetViewport()` | 恢复完整数据视图 |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `bool ChartBase.BringDataPointIntoView(int seriesIndex, int pointIndex)` | Reveal a valid data mark without changing the current zoom. This explicit operation can move both axes. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.RefreshData()` | Refresh a plain IList after modifying its contents. Use observable collections for automatic updates. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `Point? ChartBase.GetDataPointPosition(int seriesIndex, int pointIndex)` | The center of a visible mark in local coordinates; null for a missing or clipped point. |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `ChartSelection? ChartBase.HitTestDataPoint(Point point)` | Find the data mark under a local-coordinate pointer. |

### 事件订阅

按需订阅下列通知，并在宿主释放时解除订阅；这里只列事件入口，不列路由事件标识字段。

| 类型 | 事件 | 用途 |
| --- | --- | --- |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `ViewportChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`ChartBase`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md) | `SelectionChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.ChartAnimationStyle`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartAnimationStyle.md) | 4 |
| [`TheOne.Avalonia.Controls.ChartHierarchyNode`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartHierarchyNode.md) | 3 |
| [`TheOne.Avalonia.Controls.ChartKind`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartKind.md) | 21 |
| [`TheOne.Avalonia.Controls.ChartNavigationAxes`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartNavigationAxes.md) | 4 |
| [`TheOne.Avalonia.Controls.ChartPoint`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartPoint.md) | 3 |
| [`TheOne.Avalonia.Controls.ChartSamplingMode`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSamplingMode.md) | 2 |
| [`TheOne.Avalonia.Controls.ChartSelection`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSelection.md) | 4 |
| [`TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs.md) | 1 |
| [`TheOne.Avalonia.Controls.ChartSeries`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartSeries.md) | 7 |
| [`TheOne.Avalonia.Controls.ChartTooltipMode`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartTooltipMode.md) | 2 |
| [`TheOne.Avalonia.Controls.ChartViewportChangedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartViewportChangedEventArgs.md) | 2 |
| [`TheOne.Avalonia.Controls.ChartYAxis`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartYAxis.md) | 2 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
