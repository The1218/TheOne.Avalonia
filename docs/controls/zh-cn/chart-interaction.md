# 图表标签与交互

**简体中文** · [English](../en/chart-interaction.md)

数据模型与接入见[图表入口](charts.md)。

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
