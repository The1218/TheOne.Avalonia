# Chart updates, export, and sizing

[简体中文](../zh-cn/chart-rendering.md) · **English**

See [charts](charts.md) for models and setup.

## Data update animation

```xml
<one:LineChart IsSmooth="True"
           SeriesSource="{CompiledBinding Revenue}"
           IsAnimationEnabled="True" AnimationDuration="0:0:0.2" />
```

`IsAnimationEnabled` defaults to `True`; `AnimationDuration` is a `TimeSpan`, default 200 ms, accepting zero to 10 seconds. Mounted Cartesian charts interpolate data coordinates when original point identities / order are stable, or crossfade cached layers after structural changes. Pie / doughnut interpolate sector angles; gauges interpolate value, arcs, and needles. Axes, titles, and legends show target states directly. Continuous updates coalesce preparation without changing model values, source indices, selection, or viewport.

If either data layer exceeds 6,000 prepared marks, updates are immediate to avoid rebuilding large geometry per frame. Charts beyond logical side 4,096 or area 4,194,304 also skip animation. Radial sectors match original indices; new sectors grow from zero, removed sectors disappear. Keep source order stable in business code if identities must persist across reordering.

Disabling `IsAnimationEnabled`, setting zero duration, or using `Motion.Enabled=False` immediately shows the target frame. Detaching stops animation. Size, style, viewport, and navigation changes apply directly. Read-only `IsAnimating` / `AnimationProgress` expose state; progress notifications dispatch on the UI thread instead of synchronously changing bindings while drawing.

Hover overlays pause during animation. Pointer / keyboard input and geometry queries such as `GetDataPointPosition`, `HitTestDataPoint`, and `GetSharedDataPoints` first finish to target geometry, ensuring hits match display without changing selection. Disable update animation when business code needs per-frame coordinates.

References for optional motion disabling: [Chart.js updates](https://www.chartjs.org/docs/latest/developers/updates.html). Data-layer transitions and stable export: [ECharts transitions](https://echarts.apache.org/handbook/en/how-to/animation/transition/).

## PNG export

Call `ExportPng` on the UI thread to write a writable stream or file. `Size` uses logical pixels; output pixels equal `ceil(logical size × dpi / 96)`:

```csharp
using Avalonia;

// 800 × 560 逻辑像素，192 DPI，输出 1600 × 1120 PNG。
using var output = File.Create("revenue.png");
RevenueChart.ExportPng(output, new Size(800, 560), 192);

// 透明画布，保留系列、标签和图例的主题颜色。
RevenueChart.ExportPng("revenue-transparent.png", new Size(800, 560), 96, true);
```

Both overloads have `ExportPng(destination, Size? size = null, double dpi = 96, bool transparentBackground = false)`, where `destination` is a `Stream` or path. Omitted size uses valid current dimensions, or `DefaultChartWidth` / `DefaultChartHeight` before layout. An independent final-data snapshot exports the target frame even during animation, preserving viewport / legend page. Crosshairs, hover / selection emphasis, focus borders, and tooltip popups are omitted. Original size, viewport, animation, selection, and bindings remain unchanged. Transparent mode skips canvas background only, retaining semantic brushes needed by labels. Callers own supplied-stream lifetime.

Logical dimensions must be finite and positive; DPI must be `[16, 600]`. Output sides must not exceed 8,192 pixels or total area 16,777,216 pixels. Validation occurs before bitmap allocation and throws `ArgumentOutOfRangeException` for out-of-range requests. The limit represents about 64 MiB RGBA data, not total export memory. Streams must be non-null and writable. Export is synchronous; invoke large exports on an explicit user request.

Use Avalonia `StorageProvider.SaveFilePickerAsync` and pass the stream from `IStorageFile.OpenWriteAsync()` to export, supporting providers without local paths.

## Data updates and performance

`ChartPoint` and `ChartSeries` inherit `AvaloniaObject`; property changes refresh mounted charts. `ObservableCollection<T>` for `SeriesSource` or series `ItemsSource` also refreshes on addition, removal, replacement, and reordering. Replacing `SeriesSource` / `ItemsSource` subscribes to new data. Detaching releases subscriptions; reattaching reads current data.

Create / modify chart models and collections and call `RefreshData()` on the UI thread. After fetching data in the background, dispatch updates back to it:

```csharp
using Avalonia.Threading;

await Dispatcher.UIThread.InvokeAsync(() =>
{
    viewModel.Revenue[0].ItemsSource!.Add(new ChartPoint(5, 81, "五月"));
    viewModel.Revenue[0].ItemsSource![0].Y = 45;
});
```

`List<ChartPoint>` can also supply data; call `RevenueChart.RefreshData()` after list-content changes. Point property notifications remain automatic. For batches, replace `ItemsSource` once or mutate a plain list and refresh once to avoid repeated subscription rebuilding for individual notifications.

Charts have no per-point UI containers, so collection recycling panels do not apply. Preparation, original-point lookup indices, and model memory still grow with data. Sampling reduces drawn vertices but does not replace paging or remote virtualization. Bound data volume and chart dimensions in business code.

## Theme and three sizes

`OneTheme` sets background, text, grid, and six series brushes. Override `Background`, `Foreground`, `GridBrush`, `MutedBrush`, `AccentBrush`, `SecondaryBrush`, `TertiaryBrush`, `QuaternaryBrush`, `FifthBrush`, or `SixthBrush`. Defaults cycle through `One.Chart.Series1` to `One.Chart.Series6` and update with themes. `ChartSeries.Brush` takes precedence; export preserves all six brush properties. `Classes="small"` / `normal` / `large` or inherited `ControlSizing.Size` changes preferred dimensions, fonts, spacing, lines, marks, and gauge bands together.

| Property | Small | Normal | Large |
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

Charts use compact fonts separate from standard forms. Gauge tick fonts are 9 / 10 / 11, center values 20 / 22 / 24. Titles add 2 to the base font; legends use the base. PreviewHost chart-style widths follow size at 240 / 300 / 360 rather than using one fixed width.

Override values per chart. `DefaultChartWidth` / `DefaultChartHeight` are preferred dimensions only; explicit `Width` / `Height` or Grid stretching determines actual size. Even at equal dimensions, density changes internally. Radial charts reserve space for lifted sectors; Cartesian charts recalculate plot area and tick / label capacity from fonts and spacing.

## Verification scenarios

Every concrete chart type has a same-name render entry; abstract bases do not. Choose affected scenarios through [preview entry points](../../../design/PreviewHost/Program.cs):

| Need | Render targets |
| --- | --- |
| Viewport / dual axes / sampling | ZoomedLineChart, PannedBarChart, DualYAxisChart, LargeSamplingChart |
| Crosshairs / tooltips / legends | CrosshairChart, SharedTooltipChart, InteractiveLegendChart |
| Labels / smoothing / animation | LabeledLineChart, SmoothLineChart, AnimatedChart |
| Hover / gauges | PieHoverChart, DoughnutHoverChart, BarHoverChart, NeedleGaugeChart, PrecisionGaugeChart |

PNG export is verified independently of screenshot commands. Chart models need no additional metadata preservation. SeriesSource, selection, Viewport, and gauge values use compiled bindings. Dynamic switching creates concrete controls without changing read-only Kind. Native example: [NativeAotSmoke](../../../design/PreviewHost/Consumers/NativeAotSmoke/Program.cs).
