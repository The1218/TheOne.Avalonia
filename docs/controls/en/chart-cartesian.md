# Cartesian charts and sampling

[简体中文](../zh-cn/chart-cartesian.md) · **English**

See [charts](charts.md) for models and setup.

## X-axis labels and narrow viewports

Line, column, and other Cartesian charts use actual text widths to determine label counts rather than skipping short labels because of a fixed 36 px minimum slot. The Small 240 px month example shows all six labels. Spacing is checked in screen order, including decreasing X data. Genuine crowding reduces labels while retaining all points, coordinates, and tooltip information.

Labels align within the available text area and clip against the whole axis-label region, removing the fixed 80 px window that truncated long text. Bottom space accounts for actual text height separately from legends. If one label exceeds the entire chart width, provide a shorter name or wider viewport.

`--verify-chart-axis-labels [输出目录]` compares pixels with independently drawn unclipped text. It covers line / columns, themes, sizes, six month names, long Chinese labels, decreasing X, and legend visibility. The optional argument is an output directory. See [preview entry points](../../../design/PreviewHost/Program.cs) for managed / native calls.

## Smooth curves

```xml
<one:LineChart IsSmooth="True"
           SeriesSource="{CompiledBinding Revenue}" Height="320" />
```

`IsSmooth` defaults to `False` and applies only to `Line` and ordinary `Area`. Continuous segments with strictly increasing or decreasing X and no duplicate X use monotone cubic interpolation. Local extrema stay at input points and gaps remain. Duplicate or reversing X segments keep straight connections without reordering data. Ordinary area upper edges use the same curve; stacked / percent stacked areas retain straight edges so independently interpolated boundaries cannot cross.

Smoothing changes only drawing paths. Selection, sampling, tooltips, and shared-X queries still refer to original points; interpolated points are not queryable. This follows the tradeoff described by [Chart.js monotone interpolation](https://www.chartjs.org/docs/latest/charts/line.html) and [ECharts smooth lines](https://echarts.apache.org/handbook/en/how-to/chart-types/line/smooth-line/).

## Stacked charts

```xml
<one:StackedBarChart Title="月度营收构成"
           SeriesSource="{CompiledBinding Revenue}"
           IsZoomEnabled="True" IsPanEnabled="True" Height="300" />
```

`StackedBar` / `StackedHorizontalBar` align by source index. Visible same-axis positive values accumulate upward from zero; negatives accumulate downward. Primary / secondary axes stack independently and occupy separate category slots. Missing / nonfinite data is excluded. Different lengths are accepted, but each index should represent the same category. Labels / tooltips show individual contributions; selection returns original series and points.

`StackedArea` also aligns by index. At each axis / index, the first visible same-axis series with finite X defines the standard X. Mismatching X in other same-axis series becomes a gap; no searching or reordering occurs. Align sample positions beforehand. Missing data and sign changes split fill bands, with positive / negative contributions accumulated separately. Axes use stacked extents. Same-sign totals exceeding finite `double` range clamp to `±double.MaxValue` to keep geometry finite.

Multiple zero contributions at an index can share a baseline mark but remain separately selectable by keyboard. After totals reach finite limits, further contributions can have no visible thickness. Without a visible mark, `GetDataPointPosition` returns `null`, and location APIs can return `false`.

## Percent stacking

```xml
<one:PercentStackedBarChart Title="月度营收占比"
           SeriesSource="{CompiledBinding Revenue}"
           ShowDataLabels="True" Height="300" />
```

The three `PercentStacked*` types retain their stacking counterpart's index / X alignment. At each axis / index, positives normalize by positive totals to 100%, and negatives by absolute negative totals to -100%. Directions are independent; do not net opposite signs for the denominator. Zero contributes 0; invalid data is excluded. Values are scaled before summing to avoid overflow with large finite inputs.

Percentage axes use fixed percentage ranges, ignoring `YMinimum` / `YMaximum` and secondary numeric-range overrides. Viewport zoom / pan remains available. Labels show one decimal percentage; tooltips include raw values, series units, and signed shares. `GetStackPercentage(seriesIndex, pointIndex)` returns a signed `double?` ratio: `0.25` means 25%, `-0.25` means -25%. Other chart types or invalid contributions return `null`.

```csharp
var share = RevenueChart.GetStackPercentage(0, 3);
if (share is { } ratio) Console.WriteLine($"{ratio:P1}");
```

## Dual Y axes and series formatting

Set series `YAxis` to `ChartYAxis.Primary` (default) or `ChartYAxis.Secondary`. X / categories are shared; each value axis calculates its range from its own series. Columns still group by category across axes, and stacking occurs only within an axis. Secondary axes appear on the right for ordinary Cartesian charts and at the top for horizontal bars.

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

`SecondaryYMinimum` / `SecondaryYMaximum` default to `NaN` for automatic ranges; finite values override them. Primary settings are `YMinimum` / `YMaximum`, `ValueFormat` / `Unit`. Secondary settings are `SecondaryValueFormat` (default `0.##`) / `SecondaryUnit`. `YAxisTitle` / `SecondaryYAxisTitle` set titles.

`ShowSecondaryAxis` defaults to `True` and shows a secondary axis when valid `IsVisible=True` series use it. `False` hides its ticks / title while retaining independent scaling. An invalid final range suppresses only that axis's data; the other can remain visible. Both share normalized `Viewport`, mapping the same zoom fractions to their own numeric ranges. Series formatting overrides raw tooltip / label values, not tick formats; percentage labels remain percentages. Radial charts do not use dual-axis scaling.

## Large-data sampling

```xml
<one:LineChart SeriesSource="{CompiledBinding Revenue}"
           SamplingMode="MinMax" SamplingThreshold="2000"
           SamplingPointBudget="2000" IsZoomEnabled="True"
           IsPanEnabled="True" Height="320" />
```

`SamplingMode` is `ChartSamplingMode.None` or `MinMax`, default `MinMax`. `SamplingThreshold` and `SamplingPointBudget` both default to 2,000, with minimums 2 / 8 respectively. Line, area, scatter, and ordinary / percent stacked areas support sampling; categorical columns / bars remain exact. MinMax keeps local upper / lower extrema, boundaries, and selection in monotonic-X segments. It does not reorder input; nonmonotonic segments fall back to exact drawing.

The budget is a **soft target per series**, allocated by segment length. Boundaries, gaps, sign changes, and selected original points need additional retention; fragmented or unsampleable segments can exceed it. It is neither a shared all-series limit nor a strict vertex cap. Zoom prepares geometry for the current view; `SamplingMode=None` disables sampling.

Read-only `RenderedPointCount` totals prepared numeric-curve sample points and visible column / bar marks across series; pie, doughnut, and gauge report 0. It is not original data count, Control count, or low-level draw-command count. `SamplingActive` indicates whether prepared output actually reduces points. Both update after layout / geometry preparation. Notifications during drawing coalesce and defer to the next UI dispatch to prevent listeners / bindings from mutating charts while rendering; preparation outside rendering through geometry APIs updates synchronously.

Drawing geometry and the original-point spatial hit index are cached and reused by pointer queries. Data, axes, viewport, layout, or sampling changes rebuild them. `GetDataPointPosition`, hit testing, selection, and tooltips retain original series / point indices; visible original points excluded from sampled geometry can still be located and selected. Hit testing scans spatial buckets near the pointer; highly dense or overlapping data can increase candidate scans, so fixed query time is not guaranteed.

```csharp
Console.WriteLine($"绘制顶点：{RevenueChart.RenderedPointCount}，采样：{RevenueChart.SamplingActive}");
RevenueChart.SelectedIndex = 99_999; // 数据源须包含该项；选择仍指向原始模型。
```

Sampling does not preserve every point's visual detail. Disable it or zoom for point-by-point inspection. The preview includes 100,000 points and actual draw counts; performance depends on target hardware and data shape.

## Tick and label precision

Line / area / scatter labels avoid collisions across series, trying above, then below or beside points. At most 2,048 candidates and 512 drawn labels are considered. Unplaceable labels are omitted, but tooltips / selection retain values. `GetDataLabelBounds(seriesIndex, pointIndex)` returns the displayed label's target bounds; disabled labels, unsampled points, or unplaceable labels return `null`. Queries complete active animation first.

Numeric axes choose steps of 1, 2, 2.5, or 5 times an integer power of 10 according to visible range and available height, or width for horizontal charts. Ticks are in-range multiples: range 34–92 yields 40, 60, 80 rather than forcing divisions at 53.33 / 72.67. Ticks do not change data ranges, explicit YMinimum / YMaximum, or point positions. Zoom recalculates them. Percentage charts keep percentage text. Extreme / precision-boundary ranges use safe interpolation.

With default ValueFormat="0.##", ticks add fractional digits as required by step size. Tiny values use significant digits; narrow ranges at large magnitudes retain precision so distinct ticks do not share labels. Other explicit ValueFormat / SecondaryValueFormat settings are honored; integer formats discard decimals. Raw tooltips and data labels still use series formats.

X-axis capacity depends on measured label widths and spacing, preferring visible endpoints. Short labels allow more entries; long labels are thinned. Label density is independent of data sampling and never changes source indices or selection.
