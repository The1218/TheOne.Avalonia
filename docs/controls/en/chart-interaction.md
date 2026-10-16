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

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section lists only Classes, properties, extension APIs/events and related models added by this project, without repeating inherited Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost visibly compares project-added styles and uses Description components for control-specific properties, API signatures and model fields. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### ChartBase

[API / model reference](../../api/en/TheOne.Avalonia.Controls.ChartBase.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`AccentBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L78) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Primary-series/accent brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`AnimationDuration`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L18) | `System.TimeSpan` | writable; OneWay | `TimeSpan.FromMilliseconds(200)` | Duration of data transitions, also gated by the control animation switch and Motion.Enabled. Validation: `value => value >= TimeSpan.Zero && value <= TimeSpan.FromSeconds(10)` |
| [`AnimationProgress`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L20) | `double` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Background`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L74) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Background brush; use DynamicResource for theme-dependent surfaces. |
| [`ChartPadding`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L20) | `double` | writable; OneWay | `12` | 8 ; 12 ; 16 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`CrosshairPosition`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L20) | `Avalonia.Point?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`DefaultChartHeight`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L19) | `double` | writable; OneWay | `280` | 320 / 224 ; 400 / 280 ; 480 / 336 Validation: `value => double.IsFinite(value) && value > 0` |
| [`DefaultChartWidth`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L17) | `double` | writable; OneWay | `400` | 320 / 224 ; 400 / 280 ; 480 / 336 Validation: `value => double.IsFinite(value) && value > 0` |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L71) | `string` | writable; OneWay | `"暂无数据"` | `暂无数据`, the default Chinese no-data text |
| [`FifthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L82) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Fifth-series brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`FontFamily`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L84) | `Avalonia.Media.FontFamily` | writable; OneWay | `inherited registration` | Font family for text rendering; Avalonia handles fallback and actual text metrics. |
| [`FontSize`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L85) | `double` | writable; OneWay | `inherited registration` | 10 ; 11 ; 12 |
| [`Foreground`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L75) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Foreground brush; retain text, status and disabled contrast when overriding. |
| [`GaugeBandRatio`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L25) | `double` | writable; OneWay | `0.18` | Follows three theme densities; Basic, Progress, Grade, Barometer scale bands by structure; ratio is overridable Validation: `value => double.IsFinite(value) && value > 0 && value <= 0.5` |
| [`GaugeMajorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L16) | `int` | writable; OneWay | `5` | Number of major ticks, including the minimum and maximum. Validation: `value => value >= 2 && value <= 25` |
| [`GaugeMinorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L18) | `int` | writable; OneWay | `2` | Maximum minor marks between consecutive major ticks. Compact gauges reduce them to keep readable spacing. Validation: `value => value >= 0 && value <= 10` |
| [`GridBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L76) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Plot grid-line brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`HoveredPoints`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L21) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.ChartSelection>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`InnerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L63) | `double` | writable; OneWay | `0.62` | Inner-radius ratio for radial charts, controlling the hole without changing sector values/proportions. Validation: `value => double.IsFinite(value) && value >= 0 && value < 1` |
| [`IsAnimating`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L19) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsAnimationEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L17) | `bool` | writable; OneWay | `true` | Enables chart data transitions; global Motion.Enabled=False still disables them. |
| [`IsLegendInteractive`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L11) | `bool` | writable; OneWay | `true` | `True`; Cartesian series toggling and legend keyboard input |
| [`IsPanEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L34) | `bool` | writable; OneWay | `false` | Default `False`; enables left-pointer dragging / keyboard pan |
| [`IsSmooth`](../../../TheOne.Avalonia/Controls/ChartBase.Smoothing.cs#L10) | `bool` | writable; OneWay | `false` | `False`; monotone smoothing for line / ordinary area |
| [`IsZoomEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L32) | `bool` | writable; OneWay | `false` | Default `False`; enables wheel / keyboard zoom |
| [`Kind`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L55) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |
| [`LegendPageCount`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L13) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Read-only paging state; zero-based page index |
| [`LegendPageIndex`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L12) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Read-only paging state; zero-based page index |
| [`MarkerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L21) | `double` | writable; OneWay | `3.5` | 3 ; 3.5 ; 4.25 Validation: `value => double.IsFinite(value) && value > 0` |
| [`Maximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L67) | `double` | writable; OneWay | `100` | Primary value-axis maximum; null uses automatic bounds without changing the underlying data. Validation: `double.IsFinite` |
| [`Minimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L66) | `double` | writable; OneWay | `0` | Primary value-axis minimum; null derives it from data. Explicit ranges must be consistent with the data and Maximum. Validation: `double.IsFinite` |
| [`MutedBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L77) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Muted text/helper-mark brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`NavigationAxes`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L35) | `TheOne.Avalonia.Controls.ChartNavigationAxes` | writable; OneWay | `ChartNavigationAxes.Both` | Default `Both`; `None`, `Horizontal`, `Vertical` restrict navigation axes Validation: `value => (value & ~ChartNavigationAxes.Both) == 0` Values: `None`, `Horizontal`, `Vertical`, `Both` |
| [`QuaternaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L81) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Fourth-series brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`RenderedPointCount`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L19) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SamplingActive`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L20) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SamplingMode`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L15) | `TheOne.Avalonia.Controls.ChartSamplingMode` | writable; OneWay | `ChartSamplingMode.MinMax` | Sampling strategy for dense series; see enum choices. It reduces rendered points without replacing source data. Validation: `value => value is ChartSamplingMode.None or ChartSamplingMode.MinMax` Values: `None`, `MinMax` |
| [`SamplingPointBudget`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L18) | `int` | writable; OneWay | `2000` | A per-series sampling target; gap boundaries and selected originals can exceed it. Validation: `value => value >= 8` |
| [`SamplingThreshold`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L16) | `int` | writable; OneWay | `2000` | Point threshold for sampling, combined with the viewport and SamplingMode to bound rendering work. Validation: `value => value >= 2` |
| [`SecondaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L79) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Second-series brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`SecondaryUnit`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L19) | `string?` | writable; OneWay | `null` | Unit suffix for the secondary value axis, independent of the primary unit. |
| [`SecondaryValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L18) | `string` | writable; OneWay | `"0.##"` | Numeric format string for the secondary value axis, using .NET numeric formatting. |
| [`SecondaryYAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L17) | `string?` | writable; OneWay | `null` | Secondary value-axis title; it does not reassign series to that axis. |
| [`SecondaryYMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L15) | `double` | writable; OneWay | `double.NaN` | Secondary value-axis maximum; null is automatic, paired with SecondaryYMinimum. Validation: `value => !double.IsInfinity(value)` |
| [`SecondaryYMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L14) | `double` | writable; OneWay | `double.NaN` | Secondary value-axis minimum; null is automatic, paired with SecondaryYMaximum. Validation: `value => !double.IsInfinity(value)` |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L73) | `int` | writable; TwoWay | `-1` | Zero-based series / point indices with TwoWay binding; point index `-1` clears selection |
| [`SelectedSeriesIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L72) | `int` | writable; TwoWay | `0` | Zero-based series / point indices with TwoWay binding; point index `-1` clears selection |
| [`Selection`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L86) | `TheOne.Avalonia.Controls.ChartSelection?` | read-only; OneWay | `computed / state` | Read-only `ChartSelection?`, with indices and original `Series` / `Point` references |
| [`SelectionThickness`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L22) | `double` | writable; OneWay | `2` | 1.5 ; 2 ; 2.5 Validation: `value => double.IsFinite(value) && value > 0` |
| [`SeriesSource`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L56) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartSeries>?` | writable; OneWay | `null` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`ShowCrosshair`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L18) | `bool` | writable; OneWay | `false` | `False`; enables pointer crosshairs in Cartesian charts |
| [`ShowDataLabels`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L61) | `bool` | writable; OneWay | `false` | `False`; values for Cartesian charts, fitting percentages for pie / doughnut |
| [`ShowGaugeNeedle`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L13) | `bool` | writable; OneWay | `false` | Enum-based theme defaults; explicit False hides elements. Barometer tick visibility applies to both scales |
| [`ShowGaugeTicks`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L14) | `bool` | writable; OneWay | `false` | Enum-based theme defaults; explicit False hides elements. Barometer tick visibility applies to both scales |
| [`ShowGrid`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L59) | `bool` | writable; OneWay | `true` | `True`; Cartesian grid |
| [`ShowLegend`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L58) | `bool` | writable; OneWay | `true` | `True`; series legend for Cartesian charts, category legend for pie / doughnut |
| [`ShowMarkers`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L60) | `bool` | writable; OneWay | `true` | `True`; point marks for line / area |
| [`ShowSecondaryAxis`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L20) | `bool` | writable; OneWay | `true` | Shows the secondary value axis; series models still determine axis assignment. |
| [`SixthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L83) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Sixth-series brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`SliceHoverOffset`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L23) | `double` | writable; OneWay | `6` | 4 ; 6 ; 8 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`SliceHoverScale`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L24) | `double` | writable; OneWay | `1.025` | 1.02 ; 1.025 ; 1.03 Validation: `value => double.IsFinite(value) && value >= 1 && value <= 1.2` |
| [`StrokeThickness`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L62) | `double` | writable; OneWay | `2.5` | `2.5`; finite and greater than zero Validation: `value => double.IsFinite(value) && value > 0` |
| [`TertiaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L80) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Third-series brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`Title`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L57) | `string?` | writable; OneWay | `null` | Optional title |
| [`TooltipMode`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L19) | `TheOne.Avalonia.Controls.ChartTooltipMode` | writable; OneWay | `ChartTooltipMode.Single` | `ChartTooltipMode.Single`; `Shared` shows multiple series at the same position Validation: `value => value is ChartTooltipMode.Single or ChartTooltipMode.Shared` Values: `Single`, `Shared` |
| [`Unit`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L70) | `string?` | writable; OneWay | `null` | `0.##` / empty; numbers use current culture |
| [`Value`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L68) | `double` | writable; OneWay | `0` | Current value; units, range and null semantics are defined by its model and validation constraints. Validation: `double.IsFinite` |
| [`ValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L69) | `string` | writable; OneWay | `"0.##"` | `0.##` / empty; numbers use current culture |
| [`Viewport`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L30) | `Avalonia.Rect` | writable; TwoWay | `new Rect(0, 0, 1, 1)` | ZoomedLineChart, PannedBarChart, DualYAxisChart, LargeSamplingChart Validation: `value => double.IsFinite(value.X) && double.IsFinite(value.Y) && double.IsFinite(value.Width) && double.IsFinite(value.Height) && value.Width > 0 && value.Height > 0` |
| [`YAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L16) | `string?` | writable; OneWay | `null` | Primary value-axis title, included in text measurement and axis layout. |
| [`YMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L65) | `double` | writable; OneWay | `double.NaN` | `NaN` for automatic ranges; finite overrides allowed Validation: `value => !double.IsInfinity(value)` |
| [`YMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L64) | `double` | writable; OneWay | `double.NaN` | `NaN` for automatic ranges; finite overrides allowed Validation: `value => !double.IsInfinity(value)` |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| ChartBase | `one-large` | automatic template class; do not bind manually | ^.one-large => FontSize=12; DefaultChartWidth=480; DefaultChartHeight=336; ChartPadding=16; StrokeThickness=3; MarkerRadius=4.25; SelectionThickness=2.5; SliceHoverOffset=8; SliceHoverScale=1.03; GaugeBandRatio=0.22 |
| ChartBase | `one-small` | automatic template class; do not bind manually | ^.one-small => FontSize=10; DefaultChartWidth=320; DefaultChartHeight=224; ChartPadding=8; StrokeThickness=2; MarkerRadius=3; SelectionThickness=1.5; SliceHoverOffset=4; SliceHoverScale=1.02; GaugeBandRatio=0.14 |

`:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without added appearance classes use their default theme and control-specific properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | Class | 72 | 18 | 2 |
| [`TheOne.Avalonia.Controls.ChartKind`](../../api/en/TheOne.Avalonia.Controls.ChartKind.md) | Enum | 14 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartNavigationAxes`](../../api/en/TheOne.Avalonia.Controls.ChartNavigationAxes.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartPoint`](../../api/en/TheOne.Avalonia.Controls.ChartPoint.md) | Class | 3 | 2 | 0 |
| [`TheOne.Avalonia.Controls.ChartSamplingMode`](../../api/en/TheOne.Avalonia.Controls.ChartSamplingMode.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartSelection`](../../api/en/TheOne.Avalonia.Controls.ChartSelection.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs`](../../api/en/TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartSeries`](../../api/en/TheOne.Avalonia.Controls.ChartSeries.md) | Class | 7 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartTooltipMode`](../../api/en/TheOne.Avalonia.Controls.ChartTooltipMode.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartViewportChangedEventArgs`](../../api/en/TheOne.Avalonia.Controls.ChartViewportChangedEventArgs.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ChartYAxis`](../../api/en/TheOne.Avalonia.Controls.ChartYAxis.md) | Enum | 2 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
