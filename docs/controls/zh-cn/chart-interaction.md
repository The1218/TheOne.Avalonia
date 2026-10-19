# 图表标签与交互

**简体中文** · [English](../en/chart-interaction.md)

数据模型与接入见[图表入口](charts.md)。

Scatter 选择保持同步事件与原始模型身份，复用基础样本、Pen 和已排版文字；实测行为与边界见[选择 p99 与分配复用](chart-rendering.md#scatter-selection-p99)。

## 显示与选择

| 属性 | 默认值 / 用途 |
| --- | --- |
| `Title` | 可选标题 |
| `ShowLegend` | `True`，坐标图为系列图例，饼图／环形图为类别图例 |
| `ShowGrid` | `True`，坐标图网格 |
| `ShowCrosshair` | `False`，开启坐标图指针十字准线 |
| `TooltipMode` | `ChartTooltipMode.Single`，可设为 `Shared` 查看同位置的多个系列 |
| `IsLegendInteractive` | `True`，允许坐标图图例切换系列及键盘操作 |
| `ShowMarkers` | `True`，折线／面积图的数据标记 |
| `IsSmooth` | `False`，折线／普通面积的单调平滑路径 |
| `ShowDataLabels` | `False`，坐标图显示数据值，饼图／环形图显示可容纳的百分比 |
| `StrokeThickness` | `2.5`，必须大于零且有限 |
| `YMinimum` / `YMaximum` | `NaN` 表示自动范围，可设置有限 Y 轴范围 |
| `ValueFormat` / `Unit` | `0.##` / 空，使用当前区域设置格式化数值 |
| `EmptyText` | `暂无数据` |
| `SelectedSeriesIndex` / `SelectedIndex` | 零基系列 / 点索引，支持双向绑定；点索引 `-1` 清除选择 |
| `Selection` | 只读 `ChartSelection?`，包含索引以及原始 `Series` / `Point` 引用 |

柱形／条形数据标签只在可见段能容纳文字时显示，使用与默认填充色对比的文字颜色；细小段和零值仍可通过提示与选择读取原始值。

悬停或选择柱形／条形时，强调整块柱体，不在数据标签上叠加圆点。悬停饼图／环形图时，扇区沿中心方向抬起并略微放大，百分比随扇区移动；`SliceHoverOffset` / `SliceHoverScale` 可调整偏移与缩放，选择状态通过扇区边框表示。折线、面积和散点仍强调原始数据点。此行为参考 [Chart.js 的整柱悬停样式](https://www.chartjs.org/docs/latest/charts/bar.html)与[扇区 hoverOffset](https://www.chartjs.org/docs/latest/charts/doughnut.html)。

默认 Single 模式悬停数据点显示提示，单击选择数据点。图表可通过 Tab 聚焦；Left / Right 移动数据点，Home / End 到首尾有效点，Up / Down 切换坐标图系列，Escape 清除选择。导航直接读取模型，包括尚未命中过的数据点。

```csharp
RevenueChart.SelectionChanged += (_, e) =>
{
    if (e.Selection is { } selected)
        Console.WriteLine($"{selected.Series.Name}: {selected.Point.Label} = {selected.Point.Y}");
};

// 设置现有模型中的第 1 个系列、第 4 个数据点。
RevenueChart.SelectedSeriesIndex = 0;
RevenueChart.SelectedIndex = 3;

// 坐标均相对于 Chart；没有可见标记时返回 null。
var position = RevenueChart.GetDataPointPosition(0, 3);
var selection = position is { } point ? RevenueChart.HitTestDataPoint(point) : null;
```

索引对应原始列表顺序。删除／重排后会重新解析索引，不承诺按对象身份保留原来的选择。系列隐藏、无效索引、无效数据、两类堆叠面积的同轴 X 不匹配或仪表模式下 `Selection` 为 `null`。`SelectionChanged` 表示选择对象变化；已选中 `ChartPoint` 的属性更新通过数据点自身的属性通知反映。

Replace、Reset 和数据源替换后，`Selection` 会同步解析为当前模型；索引失效时，`Selection` 变为 `null`，绑定的索引值保留。选择强调通常复用几何缓存，抽样、视口显露和动画的处理边界见[数据更新与性能](chart-rendering.md#数据更新与性能边界)。

## 十字准线与多系列提示

```xml
<one:LineChart SeriesSource="{CompiledBinding Revenue}"
           ShowCrosshair="True" TooltipMode="Shared"
           IsLegendInteractive="True" Height="320" />
```

十字准线显示绘图区内指针的真实 X / Y 位置，不吸附数据点。只读 `CrosshairPosition` 为控件局部坐标的 `Point?`；关闭准线或离开绘图区时为 `null`。径向图不显示准线，仍使用单点提示。

Shared 模式使用原始数据：数值图先查找当前横向视口中离指针最近的 X，再取每个可见系列中 X **完全相等**的首个有效源数据点；同一系列存在重复 X 时按原始索引取首个有效项，不插值、不按近似 X 匹配。各系列的 X 不同会使该位置只有部分系列出现在提示中。Y 被视口裁掉的有效项仍可出现在共享提示，图中只标记绘图区内的点。

柱状／条形图按源类别索引共享；横向条形图通过指针 Y 定位类别。提示沿用每个系列／所属轴的原始值格式和单位，百分比图另显示有符号占比。共享提示最多显示 12 个系列，并以 `+N 系列` 提示其余数量；图中悬停标记也最多 12 个。过长名称、标签和单位在共享提示中截短，过长数值改用可往返解析的紧凑格式；原始对象与配置不改变。

只读 `HoveredPoints` 提供当前悬停的完整 `IReadOnlyList<ChartSelection>`，保留原始对象及索引。`GetSharedDataPoints(Point)` 查询同样的完整共享结果，不主动改变准线、悬停或选择状态：

```csharp
// pointerPosition 相对于 Chart；绘图区外返回空结果。
var entries = RevenueChart.GetSharedDataPoints(pointerPosition);
foreach (var entry in entries)
    Console.WriteLine($"{entry.Series.Name}: {entry.Point.Y}");
```

共享定位使用准备后的索引，支持未排序的源 X，保持曲线原有连接顺序；指针移动不重新遍历全部数据点。数据、视口、尺寸及提示配置变化后清理旧悬停状态，下次指针移动重新查询。拖动平移和图例键盘模式中暂停悬停反馈。

## 图例交互与分页

坐标图的图例保留隐藏系列；单击名称或标记可切换 `ChartSeries.IsVisible`，再次单击恢复。隐藏后重新计算轴范围、同轴堆叠与百分比分母，源集合不删除或重排，归一化视口保留。控件使用 `SetCurrentValue` 保留已有可见性绑定；业务 ViewModel 需要接收切换结果时使用双向编译绑定。`IsLegendInteractive=False` 保留图例显示和分页浏览，关闭用户系列切换及图例键盘模式。

图例预先准备各页的布局元数据，按有限宽度分页，只绘制当前页的条目与翻页区域，不创建逐系列 Control。千个系列可通过翻页或键盘到达末项；图例单击不启动数据平移，拖动不会误切换。饼图／环形图的图例表示切片，分页可浏览、切片保持只读；仪表没有图例。

图表获得焦点后，F6 进入图例键盘模式；方向键移动，Home / End 到首尾系列并自动翻页，Space / Enter 切换可见性，Escape 退出图例模式并保留原数据选择。退出后普通方向键继续导航数据点。

| API | 用途 |
| --- | --- |
| `GetLegendItemBounds(seriesIndex)` | 当前页系列图例的局部 `Rect?` |
| `HitTestLegend(Point)` | 指针下的系列索引；没有命中时为 `null` |
| `ToggleSeriesVisibility(seriesIndex)` | 程序切换指定系列，返回是否成功；不依赖图例显示或输入开关 |
| `LegendPageIndex` / `LegendPageCount` | 只读分页状态，页索引从 0 开始 |
| `GetLegendPageNavigationBounds(next)` | 获取下一页（True）或上一页（False）区域的局部 `Rect?` |
| `PageLegend(delta)` | 按页移动，返回是否改变当前页 |

```csharp
RevenueChart.PageLegend(1);
var legendBounds = RevenueChart.GetLegendItemBounds(3);
RevenueChart.ToggleSeriesVisibility(3);
```

## 缩放、平移与视口

`Viewport` 是完整数据范围内的归一化矩形，默认 `Rect(0, 0, 1, 1)`，支持双向编译绑定。X / Y 是起点，Width / Height 是当前可见比例；各值必须有限，宽高必须大于零，最终宽高限制在 `[0.001, 1]` 且整个矩形保持在完整视图内，即每轴最多放大 1,000 倍。普通坐标图的 Y 从数值轴底部的最小值开始；横向条形图的 Y 从顶部第一个类别开始。该属性适用于全部坐标图，包括堆叠图；饼图、环形图和仪表不使用视口。

| 属性 / 方法 | 行为 |
| --- | --- |
| `IsZoomEnabled` | 默认 `False`；启用滚轮及键盘缩放 |
| `IsPanEnabled` | 默认 `False`；启用鼠标左键拖动及键盘平移 |
| `NavigationAxes` | 默认 `Both`；可设为 `None`、`Horizontal`、`Vertical`，限制导航轴 |
| `Zoom(factor, anchor)` | factor 必须有限且大于零；`factor > 1` 放大，`0 < factor < 1` 缩小；anchor 为当前视口内的相对位置，省略时使用中心 |
| `Pan(horizontal, vertical)` | 两个参数必须有限；按当前可见宽高的比例移动起点，正值移向归一化轴的较大位置 |
| `ResetViewport()` | 恢复完整数据视图 |
| `BringDataPointIntoView(seriesIndex, pointIndex)` | 保留缩放比例并尝试显示指定点；成功返回 `true`，不改变数据选择 |

`NavigationAxes` 使用屏幕方向：横向条形图的 Horizontal 控制数值轴，Vertical 控制类别轴。输入开关限制用户操作，程序仍可直接调用缩放和平移方法；`Zoom` / `Pan` 遵循轴限制，显式定位数据点可移动两个轴，`ResetViewport` 恢复所有轴。业务代码、模型更新与导航方法均在 UI 线程执行。

启用后，滚轮以指针位置为中心缩放；左键拖动超过 4 px 开始平移，未构成拖动的单击仍选择数据。键盘 `+` / `-` 缩放，Ctrl / Command + 方向键平移，Ctrl / Command + `0` 恢复完整视图；普通方向键仍用于数据选择。更改有效选择或重新挂载图表后，会尝试把当前视口外的所选点带入视野。显式 `BringDataPointIntoView` 对无效点、径向图、当前没有可用布局或被完整坐标轴范围排除的点返回 `false`。

```csharp
using Avalonia;
using TheOne.Avalonia.Controls;

RevenueChart.NavigationAxes = ChartNavigationAxes.Horizontal;
RevenueChart.Zoom(2, new Point(0.75, 0.5)); // 当前视口的 75% 横向位置为锚点。
RevenueChart.Pan(0.25, 0);               // 向较大的 X 移动当前可见宽度的四分之一。
RevenueChart.BringDataPointIntoView(0, 3);
RevenueChart.ResetViewport();
```

视口改变不会缩减或重排源集合，完整数据的坐标范围与堆叠结果保持一致。坐标轴、可见标记与命中区域跟随视口；缩放后的图表仍直接绘制，不创建数据点容器。

`ViewportChanged` 提供 `ChartViewportChangedEventArgs.OldViewport` / `NewViewport`；可用于同步外部工具栏或记录当前视图。仅平移／缩放不会重新选择数据点，也不会触发未变化选择的自动定位。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### ChartBase

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.ChartBase.md)

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

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

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
| [`TheOne.Avalonia.Controls.ChartKind`](../../api/zh-cn/TheOne.Avalonia.Controls.ChartKind.md) | 14 |
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
