# Chart labels and interaction

[简体中文](../zh-cn/chart-interaction.md) · **English**

See [charts](charts.md) for models and setup.

Scatter selection preserves synchronous events and original model identity while reusing base samples, pens and shaped text. See [selection p99 and allocation reuse](chart-rendering.md#scatter-selection-p99) for the measured behavior and limits.

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

Replace, Reset, and source replacement resolve `Selection` synchronously against the current models. If an index becomes invalid, `Selection` becomes `null` while the bound indices remain unchanged. Selection emphasis normally reuses cached geometry; see [data updates and performance](chart-rendering.md#data-updates-and-performance) for sampling, viewport reveal, and animation exceptions.

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
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### ChartBase

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.ChartBase.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`AccentBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L297) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Primary-series/accent brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`AnimationDuration`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L52) | `System.TimeSpan` | writable; OneWay | `TimeSpan.FromMilliseconds(200)` | Duration of data transitions, also gated by the control animation switch and Motion.Enabled. Validation: `value => value >= TimeSpan.Zero && value <= TimeSpan.FromSeconds(10)` |
| [`AnimationProgress`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L66) | `double` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`AnimationStyle`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L59) | `TheOne.Avalonia.Controls.ChartAnimationStyle` | writable; OneWay | `0 / default` | First nonempty presentation ; Subsequent data changes Validation: `value => Enum.IsDefined(value)` Values: `Auto`, `None`, `Fade`, `Reveal` |
| [`Background`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L273) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Background brush; use DynamicResource for theme-dependent surfaces. |
| [`ChartPadding`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L53) | `double` | writable; OneWay | `12` | 8 ; 12 ; 16 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`CrosshairPosition`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L54) | `Avalonia.Point?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`DefaultChartHeight`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L47) | `double` | writable; OneWay | `280` | 320 / 224 ; 400 / 280 ; 480 / 336 Validation: `value => double.IsFinite(value) && value > 0` |
| [`DefaultChartWidth`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L40) | `double` | writable; OneWay | `400` | 320 / 224 ; 400 / 280 ; 480 / 336 Validation: `value => double.IsFinite(value) && value > 0` |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L255) | `string` | writable; OneWay | `"暂无数据"` | `暂无数据`, the default Chinese no-data text |
| [`FifthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L321) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Fifth-series brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`FontFamily`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L333) | `Avalonia.Media.FontFamily` | writable; OneWay | `inherited registration` | Font family for text rendering; Avalonia handles fallback and actual text metrics. |
| [`FontSize`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L339) | `double` | writable; OneWay | `inherited registration` | 10 ; 11 ; 12 |
| [`Foreground`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L279) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | Foreground brush; retain text, status and disabled contrast when overriding. |
| [`GaugeBandRatio`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L83) | `double` | writable; OneWay | `0.18` | Follows three theme densities; Basic, Progress, Grade, Barometer scale bands by structure; ratio is overridable Validation: `value => double.IsFinite(value) && value > 0 && value <= 0.5` |
| [`GaugeMajorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L34) | `int` | writable; OneWay | `5` | Number of major ticks, including the minimum and maximum. Validation: `value => value >= 2 && value <= 25` |
| [`GaugeMinorTickCount`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L41) | `int` | writable; OneWay | `2` | Maximum minor marks between consecutive major ticks. Compact gauges reduce them to keep readable spacing. Validation: `value => value >= 0 && value <= 10` |
| [`GridBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L285) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Plot grid-line brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`HoveredPoints`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L55) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.ChartSelection>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`InnerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L207) | `double` | writable; OneWay | `0.62` | Inner-radius ratio for radial charts, controlling the hole without changing sector values/proportions. Validation: `value => double.IsFinite(value) && value >= 0 && value < 1` |
| [`IsAnimating`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L65) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsAnimationEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Animation.cs#L46) | `bool` | writable; OneWay | `true` | Enables chart data transitions; global Motion.Enabled=False still disables them. |
| [`IsLegendInteractive`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L27) | `bool` | writable; OneWay | `true` | `True`; Cartesian series toggling and legend keyboard input |
| [`IsPanEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L67) | `bool` | writable; OneWay | `false` | Default `False`; enables left-pointer dragging / keyboard pan |
| [`IsSmooth`](../../../TheOne.Avalonia/Controls/ChartBase.Smoothing.cs#L12) | `bool` | writable; OneWay | `false` | `False`; monotone smoothing for line / ordinary area |
| [`IsZoomEnabled`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L60) | `bool` | writable; OneWay | `false` | Default `False`; enables wheel / keyboard zoom |
| [`Kind`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L163) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`LegendPageCount`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L42) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Read-only paging state; zero-based page index |
| [`LegendPageIndex`](../../../TheOne.Avalonia/Controls/ChartBase.Legend.cs#L33) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Read-only paging state; zero-based page index |
| [`MarkerRadius`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L59) | `double` | writable; OneWay | `3.5` | 3 ; 3.5 ; 4.25 Validation: `value => double.IsFinite(value) && value > 0` |
| [`Maximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L231) | `double` | writable; OneWay | `100` | Primary value-axis maximum; null uses automatic bounds without changing the underlying data. Validation: `double.IsFinite` |
| [`Minimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L225) | `double` | writable; OneWay | `0 / default` | Primary value-axis minimum; null derives it from data. Explicit ranges must be consistent with the data and Maximum. Validation: `double.IsFinite` |
| [`MutedBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L291) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Muted text/helper-mark brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`NavigationAxes`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L73) | `TheOne.Avalonia.Controls.ChartNavigationAxes` | writable; OneWay | `ChartNavigationAxes.Both` | Default `Both`; `None`, `Horizontal`, `Vertical` restrict navigation axes Validation: `value => (value & ~ChartNavigationAxes.Both) == 0` Values: `None`, `Horizontal`, `Vertical`, `Both` |
| [`QuaternaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L315) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Fourth-series brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`RenderedPointCount`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L113) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SamplingActive`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L114) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SamplingMode`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L94) | `TheOne.Avalonia.Controls.ChartSamplingMode` | writable; OneWay | `ChartSamplingMode.MinMax` | Sampling strategy for dense series; see enum choices. It reduces rendered points without replacing source data. Validation: `value => value is ChartSamplingMode.None or ChartSamplingMode.MinMax` Values: `None`, `MinMax` |
| [`SamplingPointBudget`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L107) | `int` | writable; OneWay | `2000` | A per-series sampling target; gap boundaries and selected originals can exceed it. Validation: `value => value >= 8` |
| [`SamplingThreshold`](../../../TheOne.Avalonia/Controls/ChartBase.Sampling.cs#L100) | `int` | writable; OneWay | `2000` | Point threshold for sampling, combined with the viewport and SamplingMode to bound rendering work. Validation: `value => value >= 2` |
| [`SecondaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L303) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Second-series brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`SecondaryUnit`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L60) | `string?` | writable; OneWay | `null` | Unit suffix for the secondary value axis, independent of the primary unit. |
| [`SecondaryValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L54) | `string` | writable; OneWay | `"0.##"` | Numeric format string for the secondary value axis, using .NET numeric formatting. |
| [`SecondaryYAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L48) | `string?` | writable; OneWay | `null` | Secondary value-axis title; it does not reassign series to that axis. |
| [`SecondaryYMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L36) | `double` | writable; OneWay | `double.NaN` | Secondary value-axis maximum; null is automatic, paired with SecondaryYMinimum. Validation: `value => !double.IsInfinity(value)` |
| [`SecondaryYMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L30) | `double` | writable; OneWay | `double.NaN` | Secondary value-axis minimum; null is automatic, paired with SecondaryYMaximum. Validation: `value => !double.IsInfinity(value)` |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L267) | `int` | writable; TwoWay | `-1` | Zero-based series / point indices with TwoWay binding; point index `-1` clears selection |
| [`SelectedSeriesIndex`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L261) | `int` | writable; TwoWay | `0 / default` | Zero-based series / point indices with TwoWay binding; point index `-1` clears selection |
| [`Selection`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L345) | `TheOne.Avalonia.Controls.ChartSelection?` | read-only; OneWay | `computed / state` | Read-only `ChartSelection?`, with indices and original `Series` / `Point` references |
| [`SelectionThickness`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L65) | `double` | writable; OneWay | `2` | 1.5 ; 2 ; 2.5 Validation: `value => double.IsFinite(value) && value > 0` |
| [`SeriesSource`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L165) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartSeries>?` | writable; OneWay | `null` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`ShowCrosshair`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L42) | `bool` | writable; OneWay | `false` | `False`; enables pointer crosshairs in Cartesian charts |
| [`ShowDataLabels`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L195) | `bool` | writable; OneWay | `false` | `False`; values for Cartesian charts, fitting percentages for pie / doughnut |
| [`ShowGaugeNeedle`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L21) | `bool` | writable; OneWay | `false` | Enum-based theme defaults; explicit False hides elements. Barometer tick visibility applies to both scales |
| [`ShowGaugeTicks`](../../../TheOne.Avalonia/Controls/ChartBase.Gauge.cs#L27) | `bool` | writable; OneWay | `false` | Enum-based theme defaults; explicit False hides elements. Barometer tick visibility applies to both scales |
| [`ShowGrid`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L183) | `bool` | writable; OneWay | `true` | `True`; Cartesian grid |
| [`ShowLegend`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L177) | `bool` | writable; OneWay | `true` | `True`; series legend for Cartesian charts, category legend for pie / doughnut |
| [`ShowMarkers`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L189) | `bool` | writable; OneWay | `true` | `True`; point marks for line / area |
| [`ShowSecondaryAxis`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L66) | `bool` | writable; OneWay | `true` | Shows the secondary value axis; series models still determine axis assignment. |
| [`SixthBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L327) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Sixth-series brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`SliceHoverOffset`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L71) | `double` | writable; OneWay | `6` | 4 ; 6 ; 8 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`SliceHoverScale`](../../../TheOne.Avalonia/Controls/ChartBase.Sizing.cs#L77) | `double` | writable; OneWay | `1.025` | 1.02 ; 1.025 ; 1.03 Validation: `value => double.IsFinite(value) && value >= 1 && value <= 1.2` |
| [`StrokeThickness`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L201) | `double` | writable; OneWay | `2.5` | `2.5`; finite and greater than zero Validation: `value => double.IsFinite(value) && value > 0` |
| [`TertiaryBrush`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L309) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Third-series brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`Title`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L171) | `string?` | writable; OneWay | `null` | Optional title |
| [`TooltipMode`](../../../TheOne.Avalonia/Controls/ChartBase.Interaction.cs#L48) | `TheOne.Avalonia.Controls.ChartTooltipMode` | writable; OneWay | `0 / default` | `ChartTooltipMode.Single`; `Shared` shows multiple series at the same position Validation: `value => value is ChartTooltipMode.Single or ChartTooltipMode.Shared` Values: `Single`, `Shared` |
| [`Unit`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L249) | `string?` | writable; OneWay | `null` | `0.##` / empty; numbers use current culture |
| [`Value`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L237) | `double` | writable; OneWay | `0 / default` | Current value; units, range and null semantics are defined by its model and validation constraints. Validation: `double.IsFinite` |
| [`ValueFormat`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L243) | `string` | writable; OneWay | `"0.##"` | `0.##` / empty; numbers use current culture |
| [`Viewport`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L53) | `Avalonia.Rect` | writable; TwoWay | `new Rect(0, 0, 1, 1)` | ZoomedLineChart, PannedBarChart, DualYAxisChart, LargeSamplingChart Validation: `value => double.IsFinite(value.X) && double.IsFinite(value.Y) && double.IsFinite(value.Width) && double.IsFinite(value.Height) && value.Width > 0 && value.Height > 0` |
| [`YAxisTitle`](../../../TheOne.Avalonia/Controls/ChartBase.Advanced.cs#L42) | `string?` | writable; OneWay | `null` | Primary value-axis title, included in text measurement and axis layout. |
| [`YMaximum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L219) | `double` | writable; OneWay | `double.NaN` | `NaN` for automatic ranges; finite overrides allowed Validation: `value => !double.IsInfinity(value)` |
| [`YMinimum`](../../../TheOne.Avalonia/Controls/ChartBase.cs#L213) | `double` | writable; OneWay | `double.NaN` | `NaN` for automatic ranges; finite overrides allowed Validation: `value => !double.IsInfinity(value)` |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

- `ChartBase`: `Classes="animation-fade"` / `Classes="animation-none"` / `Classes="animation-reveal"`
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `double? ChartBase.GetStackPercentage(int seriesIndex, int pointIndex)` | A signed share of the matching axis/category/sign total in a percentage stack, or null otherwise. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.ExportPng(Stream stream, Size? size = null, double dpi = 96, bool transparentBackground = false)` | Render a stable target-state PNG on the UI thread without altering this chart. Size is logical pixels; DPI scales the output. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.ExportPng(string path, Size? size = null, double dpi = 96, bool transparentBackground = false)` | Renders the chart to PNG; see size/scale/stream parameters in the signature. The caller owns stream lifetime. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `IReadOnlyList<ChartSelection> ChartBase.GetSharedDataPoints(Point point)` | Nearest visible numeric X (exact matches per series), or the pointed categorical index. No interpolation. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `Rect? ChartBase.GetDataLabelBounds(int seriesIndex, int pointIndex)` | Returns the target bounds of a visible numeric data label, or null when it is omitted or labels are disabled. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `Rect? ChartBase.GetLegendItemBounds(int seriesIndex)` | Current-page local `Rect?` for a series |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `int? ChartBase.HitTestLegend(Point point)` | Hit series index, or `null` |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `Rect? ChartBase.GetLegendPageNavigationBounds(bool next)` | Local `Rect?` for next (True) / previous (False) navigation |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `bool ChartBase.PageLegend(int delta)` | Moves by pages and reports whether the page changed |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `bool ChartBase.ToggleSeriesVisibility(int seriesIndex)` | Toggle model visibility, retaining the series' binding. Independent of input/legend flags. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.Zoom(double factor, Point? anchor = null)` | Zoom by a positive factor around fractional coordinates in the current view. Factors above one zoom in. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.Pan(double horizontal, double vertical)` | Pan by fractions of the visible spans. Positive deltas move towards larger axis values/category indices. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.ResetViewport()` | Restores full data view |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `bool ChartBase.BringDataPointIntoView(int seriesIndex, int pointIndex)` | Reveal a valid data mark without changing the current zoom. This explicit operation can move both axes. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `void ChartBase.RefreshData()` | Refresh a plain IList after modifying its contents. Use observable collections for automatic updates. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `Point? ChartBase.GetDataPointPosition(int seriesIndex, int pointIndex)` | The center of a visible mark in local coordinates; null for a missing or clipped point. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `ChartSelection? ChartBase.HitTestDataPoint(Point point)` | Find the data mark under a local-coordinate pointer. |

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `ViewportChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`ChartBase`](../../api/en/TheOne.Avalonia.Controls.ChartBase.md) | `SelectionChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.ChartAnimationStyle`](../../api/en/TheOne.Avalonia.Controls.ChartAnimationStyle.md) | 4 |
| [`TheOne.Avalonia.Controls.ChartHierarchyNode`](../../api/en/TheOne.Avalonia.Controls.ChartHierarchyNode.md) | 3 |
| [`TheOne.Avalonia.Controls.ChartKind`](../../api/en/TheOne.Avalonia.Controls.ChartKind.md) | 21 |
| [`TheOne.Avalonia.Controls.ChartNavigationAxes`](../../api/en/TheOne.Avalonia.Controls.ChartNavigationAxes.md) | 4 |
| [`TheOne.Avalonia.Controls.ChartPoint`](../../api/en/TheOne.Avalonia.Controls.ChartPoint.md) | 3 |
| [`TheOne.Avalonia.Controls.ChartSamplingMode`](../../api/en/TheOne.Avalonia.Controls.ChartSamplingMode.md) | 2 |
| [`TheOne.Avalonia.Controls.ChartSelection`](../../api/en/TheOne.Avalonia.Controls.ChartSelection.md) | 4 |
| [`TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs`](../../api/en/TheOne.Avalonia.Controls.ChartSelectionChangedEventArgs.md) | 1 |
| [`TheOne.Avalonia.Controls.ChartSeries`](../../api/en/TheOne.Avalonia.Controls.ChartSeries.md) | 7 |
| [`TheOne.Avalonia.Controls.ChartTooltipMode`](../../api/en/TheOne.Avalonia.Controls.ChartTooltipMode.md) | 2 |
| [`TheOne.Avalonia.Controls.ChartViewportChangedEventArgs`](../../api/en/TheOne.Avalonia.Controls.ChartViewportChangedEventArgs.md) | 2 |
| [`TheOne.Avalonia.Controls.ChartYAxis`](../../api/en/TheOne.Avalonia.Controls.ChartYAxis.md) | 2 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
