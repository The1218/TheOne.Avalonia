# BoxPlot and Candlestick

**English** · [简体中文](../zh-cn/chart-statistical.md) · [Chart overview](charts.md)

`BoxPlotChart` and `CandlestickChart` inherit `StatisticalChartBase`. They draw cached geometry, axes and text directly, without a control per observation. Semantics reference [ECharts boxplot](https://echarts.apache.org/en/llms-documents/option-parts/option.series-boxplot.md) and [candlestick](https://echarts.apache.org/en/llms-documents/option-parts/option.series-candlestick.md); the API uses this library's typed models with no ECharts runtime dependency.

## Shared configuration

Load `OneTheme` for Light / Dark, three sizes, `SeriesSource`, legends, single-point tooltips, selection and PNG export. Common configuration is covered in [Chart](charts.md), animation in [rendering](chart-rendering.md). This page describes the differences.

| Property | Default | Meaning |
| --- | --- | --- |
| `PointWidthRatio` | `0.6` | Body width relative to the smallest adjacent X interval; finite and `0 < value <= 1`. Visible series share the slot side by side |
| `YMinimum` / `YMaximum` | `NaN` | Explicit bounds of the single numeric Y axis; NaN uses automatic range. A maximum no greater than the minimum produces no geometry |
| `Viewport` | `(0,0,1,1)` | Normalized numeric viewport, bindable TwoWay. Reuses `Zoom`, `Pan`, `ResetViewport`, `BringDataPointIntoView`; enable gestures through `IsZoomEnabled` / `IsPanEnabled` |

X retains numeric distance regardless of source order. `Label` provides X tick and tooltip text. For dates, use timestamps as X and formatted dates as labels. There is no trading calendar or automatic category compression: your X values determine whether missing trading days leave space. Duplicate X values remain separate observations; the last painted observation wins a same-series overlap hit.

Both controls use one primary Y axis. Secondary `ChartSeries.YAxis` configuration, stacking, shared-X tooltips and sampling do not apply. Invalid observations are neither drawn nor hit and do not contribute to automatic ranges; source indices remain unchanged. Zoom clips visuals while retaining original values. X-indexed hit lookup checks actual bodies, whiskers, wicks and outliers.

## BoxPlotChart

| Added property | Type / default | Meaning |
| --- | --- | --- |
| `ShowOutliers` | `bool`, true | Draw finite outliers and include them in the automatic Y range; false excludes both contributions |
| `OutlierRadius` | `double`, 3 | Outlier radius in DIP, finite and `0 < value <= 32` |

Use `BoxPlotPoint` in `ChartSeries.ItemsSource`. It inherits `ChartPoint`: `X` is position, `Label` the optional name; `Minimum`, `LowerQuartile`, `Median`, `UpperQuartile`, `Maximum` form a precomputed summary. `Median` aliases inherited `Y`; setting either notifies both properties, while the chart invalidates its data only once.

Require `Minimum <= LowerQuartile <= Median <= UpperQuartile <= Maximum`, finite coordinates and finite summary values. Negative and equal values are valid. Reversed or NaN values are not silently sorted or repaired. Whisker endpoints are supplied by the caller; there is no built-in 1.5×IQR rule or raw-sample quantile calculation. Use a consistent statistical convention in your application.

`Outliers` is `IList<double>?`, defaulting to a separate observable empty collection per point. All finite entries are drawn as supplied, without testing whether they lie beyond the whiskers; NaN / Infinity are skipped. The chart draws the quartile body, median and whisker caps, and reports all five summary values in tooltips.

A distant outlier does not pull keyboard tooltips or the `GetDataPointPosition()` anchor into empty space. The visible body / median is preferred, falling back to a visible whisker or outlier after zooming. Drawn outliers count toward the shared animation safety budget, so a large outlier list cannot bypass it.

```csharp
public ObservableCollection<ChartSeries> Distribution { get; } =
[
    new()
    {
        Name = "Latency", Unit = " ms", ValueFormat = "0.0",
        ItemsSource = new ObservableCollection<ChartPoint>
        {
            new BoxPlotPoint(1, 12, 18, 24, 32, 43, "Service A")
            { Outliers = new ObservableCollection<double> { 5, 56 } },
            new BoxPlotPoint(2, 9, 15, 21, 29, 38, "Service B")
        }
    }
];
```

Declare the actual view-model `x:DataType` at the view root and enable compiled bindings:

```xml
<one:BoxPlotChart SeriesSource="{CompiledBinding Distribution}"
                  PointWidthRatio="0.6" ShowOutliers="True" OutlierRadius="3"
                  IsZoomEnabled="True" IsPanEnabled="True"
                  AnimationStyle="Reveal" MinHeight="280" />
```

## CandlestickChart

| Added property | Type / default | Meaning |
| --- | --- | --- |
| `UpBrush` | `IBrush?`, themed `One.Success` | Color when `Close >= Open`; explicit null falls back to theme series color 0 |
| `DownBrush` | `IBrush?`, themed `One.Danger` | Color when `Close < Open`; explicit null falls back to theme series color 1 |
| `IsHollowUp` | `bool`, false | Hollow rising bodies; transparent export leaves no wick through the hollow body |

`CandlestickPoint` inherits `ChartPoint`, adding `Open`, `Close`, `Low`, `High`. `Close` aliases observable `Y`. Constructor order is **x, open, close, low, high, label**.

All values must be finite, with `Low <= min(Open, Close)` and `High >= max(Open, Close)`. Negative prices, unchanged closes and zero-height intervals are valid. An invalid OHLC observation is skipped as a whole; reversed high/low values are not repaired. Direction compares Close with Open. Flat candles use UpBrush and a horizontal body line. Configure the brushes explicitly for your market's color convention.

```csharp
public ObservableCollection<ChartSeries> Prices { get; } =
[
    new()
    {
        Name = "Price", ValueFormat = "0.00",
        ItemsSource = new ObservableCollection<ChartPoint>
        {
            new CandlestickPoint(1, 102, 108, 99, 110, "Oct 19"),
            new CandlestickPoint(2, 108, 104, 101, 111, "Oct 20")
        }
    }
];
```

```xml
<one:CandlestickChart SeriesSource="{CompiledBinding Prices}"
                      IsHollowUp="True" PointWidthRatio="0.7"
                      UpBrush="{DynamicResource One.Danger}"
                      DownBrush="{DynamicResource One.Success}"
                      IsZoomEnabled="True" IsPanEnabled="True" />
```

## Updates, caches and export

Observable series, point collections, point properties and Outliers update automatically. Replacing Outliers or ItemsSource releases old subscriptions; shared outlier collections use reference-counted single subscriptions. Unloading unsubscribes; reattaching reads the current models. Call `RefreshData()` after in-place changes to a plain IList, including a plain outlier list. Mutate Avalonia models on the UI thread.

Scalar updates retain shared data subscriptions and invalidate this chart's geometry. Layout computes ranges, the X index, whiskers / wicks, text and pens. Unchanged redraws reuse the caches without enumerating source data. A point update still recalculates this chart's range and layout; this is not O(1) incremental geometry.

`AnimationStyle="Auto"` reveals on first presentation and fades subsequent data updates, using shared animation safety budgets. Large data presents the target frame directly. `None`, disabled Motion or zero duration complete immediately. `ExportPng` creates an independent snapshot of the same concrete type, specialized models and configuration, always exporting the complete target frame without advancing on-screen animation or sharing mutable drawing caches.

PreviewHost has separate pages with configuration comparisons, live updates, property / model descriptions and short code samples. See [second-batch performance](chart-next-batch-performance-20261020.md) for measured results and targeted coverage.
