# Chart labels and interaction

[简体中文](../zh-cn/chart-interaction.md) · **English**

See [charts](charts.md) for models and setup.

## Display and selection

| Property | Default / purpose |
| --- | --- |
| `Title` | Optional title |
| `ShowLegend` | `True`; series legend for Cartesian charts, category legend for pie / doughnut |
| `ShowGrid` | `True`; Cartesian grid |
| `ShowCrosshair` | `False`; enables pointer crosshairs in Cartesian charts |
| `TooltipMode` | `ChartTooltipMode.Single`; `Shared` shows multiple series at the same position |
| `IsLegendInteractive` | `True`; Cartesian series toggling and legend keyboard input |
| `ShowMarkers` | `True`; point marks for line / area |
| `IsSmooth` | `False`; monotone smoothing for line / ordinary area |
| `ShowDataLabels` | `False`; values for Cartesian charts, fitting percentages for pie / doughnut |
| `StrokeThickness` | `2.5`; finite and greater than zero |
| `YMinimum` / `YMaximum` | `NaN` for automatic ranges; finite overrides allowed |
| `ValueFormat` / `Unit` | `0.##` / empty; numbers use current culture |
| `EmptyText` | `暂无数据`, the default Chinese no-data text |
| `SelectedSeriesIndex` / `SelectedIndex` | Zero-based series / point indices with TwoWay binding; point index `-1` clears selection |
| `Selection` | Read-only `ChartSelection?`, with indices and original `Series` / `Point` references |

Column / bar labels appear only if the visible segment fits the text, using contrast against default fill colors. Small segments and zero values remain accessible through tooltips / selection.

Hover / selection emphasizes the entire column / bar, without drawing a dot over its label. Pie / doughnut sectors lift radially and scale slightly, carrying percentage labels with them. `SliceHoverOffset` / `SliceHoverScale` set displacement / scale; selection uses a sector border. Line, area, and scatter emphasize original points. References: [Chart.js bar hover](https://www.chartjs.org/docs/latest/charts/bar.html) and [sector hoverOffset](https://www.chartjs.org/docs/latest/charts/doughnut.html).

Single mode shows a tooltip on hover and selects on click. Tab focuses the chart. Left / Right moves between points, Home / End reaches first / last valid points, Up / Down changes Cartesian series, and Escape clears selection. Navigation reads models directly, including never-hit points.

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

Indices follow source order and are resolved again after deletion / reordering; selection is not guaranteed to track object identity. Hidden series, invalid indices / data, same-axis X mismatches in stacked-area variants, and gauges have `Selection` equal to `null`. `SelectionChanged` reports selection-object changes; selected `ChartPoint` property updates notify through the point itself.

## Crosshairs and shared tooltips

```xml
<one:LineChart SeriesSource="{CompiledBinding Revenue}"
           ShowCrosshair="True" TooltipMode="Shared"
           IsLegendInteractive="True" Height="320" />
```

Crosshairs show actual pointer X / Y within the plot rather than snapping. Read-only `CrosshairPosition` is a local `Point?`, becoming `null` when disabled or outside the plot. Radial charts have no crosshairs and retain single-point tooltips.

Shared mode uses original data. Numeric charts find the nearest X in the current horizontal viewport, then the first valid source point with **exactly equal X** in each visible series. Duplicate X uses the first valid original index; no interpolation or approximate matching occurs. Different series X values can leave only some series in a tooltip. Valid points clipped in Y can still appear in shared tooltips; only plot-visible points receive marks.

Columns / bars share source category indices; horizontal bars use pointer Y to find the category. Tooltips use raw series / axis formats and units, adding signed percentages for percent charts. They show at most 12 series and indicate additional entries with `+N 系列` (additional series). Hover marks are also limited to 12. Long names, labels, and units are truncated; long numbers use a compact round-trip format. Original objects / settings remain unchanged.

Read-only `HoveredPoints` exposes the full current `IReadOnlyList<ChartSelection>`, with original references and indices. `GetSharedDataPoints(Point)` queries the same full results without changing crosshairs, hover, or selection:

```csharp
// pointerPosition 相对于 Chart；绘图区外返回空结果。
var entries = RevenueChart.GetSharedDataPoints(pointerPosition);
foreach (var entry in entries)
    Console.WriteLine($"{entry.Series.Name}: {entry.Point.Y}");
```

Shared lookup uses prepared indices, supports unsorted X, and preserves line connection order. Pointer movement does not traverse all points again. Data, viewport, size, or tooltip configuration changes clear hover; the next movement queries again. Hover pauses during pan dragging and legend keyboard mode.

## Legend interaction and paging

Cartesian legends retain hidden series. Clicking a name or mark toggles `ChartSeries.IsVisible`; another click restores it. Axis ranges, same-axis stacks, and percentage denominators recalculate without removing / reordering sources or resetting normalized viewport. `SetCurrentValue` preserves visibility bindings; use TwoWay compiled bindings if the ViewModel must receive toggles. `IsLegendInteractive=False` retains display / paging but disables user toggles and legend keyboard mode.

Legend page layout metadata is prepared beforehand, pages fit finite width, and only the current entries / navigation regions are drawn. No per-series Control instances are created. Paging / keyboard navigation can reach the last of a thousand series. Clicking legends does not pan; dragging does not accidentally toggle. Pie / doughnut legends represent read-only slices with paging; gauges have no legend.

With focus, F6 enters legend keyboard mode. Arrows move; Home / End reaches first / last series and changes pages automatically; Space / Enter toggles visibility. Escape exits, preserving point selection. Ordinary arrows then resume point navigation.

| API | Purpose |
| --- | --- |
| `GetLegendItemBounds(seriesIndex)` | Current-page local `Rect?` for a series |
| `HitTestLegend(Point)` | Hit series index, or `null` |
| `ToggleSeriesVisibility(seriesIndex)` | Programmatic toggle, returning success independently of legend display / input switches |
| `LegendPageIndex` / `LegendPageCount` | Read-only paging state; zero-based page index |
| `GetLegendPageNavigationBounds(next)` | Local `Rect?` for next (True) / previous (False) navigation |
| `PageLegend(delta)` | Moves by pages and reports whether the page changed |

```csharp
RevenueChart.PageLegend(1);
var legendBounds = RevenueChart.GetLegendItemBounds(3);
RevenueChart.ToggleSeriesVisibility(3);
```

## Zoom, pan, and viewport

`Viewport` is a normalized rectangle within full data bounds, default `Rect(0, 0, 1, 1)`, supporting TwoWay compiled binding. X / Y are origins, Width / Height visible fractions. All values must be finite, with positive dimensions clamped to `[0.001, 1]` and the entire rectangle kept in full view: at most 1,000× zoom per axis. Ordinary Cartesian Y starts at the numeric minimum at the bottom; horizontal-bar Y starts at the first top category. All Cartesian charts, including stacks, use it; pie, doughnut, and gauges do not.

| Property / method | Behavior |
| --- | --- |
| `IsZoomEnabled` | Default `False`; enables wheel / keyboard zoom |
| `IsPanEnabled` | Default `False`; enables left-pointer dragging / keyboard pan |
| `NavigationAxes` | Default `Both`; `None`, `Horizontal`, `Vertical` restrict navigation axes |
| `Zoom(factor, anchor)` | Finite positive factor; `factor > 1` zooms in, `0 < factor < 1` zooms out; anchor is relative to current viewport, default center |
| `Pan(horizontal, vertical)` | Finite arguments move origin by fractions of current dimensions; positive moves toward larger normalized coordinates |
| `ResetViewport()` | Restores full data view |
| `BringDataPointIntoView(seriesIndex, pointIndex)` | Keeps zoom and attempts to reveal a point; returns `true` on success without changing selection |

`NavigationAxes` uses screen directions: Horizontal controls numeric axes for horizontal bars, Vertical controls categories. Input switches restrict users, not direct method calls. `Zoom` / `Pan` respect axis limits; explicit point location can move both, and `ResetViewport` resets all. Business code, model updates, and navigation calls run on the UI thread.

When enabled, wheel zoom anchors at the pointer. Left dragging starts panning after 4 px; shorter clicks still select. Keyboard `+` / `-` zooms, Ctrl / Command + arrows pans, and Ctrl / Command + `0` resets. Ordinary arrows select points. Changing valid selection or reattaching tries to reveal selected points outside the current viewport. `BringDataPointIntoView` returns `false` for invalid points, radial charts, unavailable layout, or points excluded by full axis ranges.

```csharp
using Avalonia;
using TheOne.Avalonia.Controls;

RevenueChart.NavigationAxes = ChartNavigationAxes.Horizontal;
RevenueChart.Zoom(2, new Point(0.75, 0.5)); // 当前视口的 75% 横向位置为锚点。
RevenueChart.Pan(0.25, 0);               // 向较大的 X 移动当前可见宽度的四分之一。
RevenueChart.BringDataPointIntoView(0, 3);
RevenueChart.ResetViewport();
```

Viewport changes do not shrink or reorder sources; full-data extents and stack results stay consistent. Axes, visible marks, and hit regions follow the viewport. Zoomed charts still draw directly without point containers.

`ViewportChanged` exposes `ChartViewportChangedEventArgs.OldViewport` / `NewViewport` for toolbar synchronization or recording views. Pan / zoom alone does not reselect points or trigger automatic reveal for unchanged selection.
