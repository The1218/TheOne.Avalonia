# Pie, doughnut, and gauge charts

[简体中文](../zh-cn/chart-radial.md) · **English**

See [charts](charts.md) for models and setup.

## Pie and doughnut

```xml
<one:DoughnutChart Title="营收构成"
           SeriesSource="{CompiledBinding Revenue}"
           InnerRadius="0.62" ShowDataLabels="True"
           ValueFormat="0.##" Unit=" 万元" Height="300" />
```

Only the first nonempty `IsVisible=True` series is used; later series are not merged. Points contribute to shares, legends, and selection only when X / Y are finite and `Y > 0`. Zero, negative, and nonfinite values are ignored. `Label` names categories. If the first visible nonempty series has no valid positive values, `EmptyText` is shown instead of trying later series.

`InnerRadius` is a ratio in `[0, 1)` affecting doughnut only; `Pie` stays solid. `ShowDataLabels=True` displays fitting sector percentages; tooltips retain values for small sectors. For many categories, aggregate major categories and an other group in business code; legend paging browses remaining entries.

## Gauges

```xml
<one:GaugeChart Title="目标完成率"
           Minimum="0" Maximum="100" Value="72"
           ShowGaugeNeedle="True" ShowGaugeTicks="True"
           GaugeMajorTickCount="5" GaugeMinorTickCount="2"
           ValueFormat="0" Unit="%" Height="280" />
```

`Minimum`, `Maximum`, and `Value` must be finite. Supply `Maximum > Minimum`; invalid ranges display the Chinese message that maximum must exceed minimum. Arcs clamp to the range while text retains actual `Value`. Compiled bindings can connect `Value` to notifying ViewModel properties. Gauges have no point selection or legend.

For original `GaugeStyle="Semicircle"`, `ShowGaugeNeedle` / `ShowGaugeTicks` both default to `False`. Needles / arcs share the range and clamped value, stopping at endpoints while text retains raw values. `GaugeMajorTickCount` defaults to 5 in `[2, 25]`, including both endpoints. `GaugeMinorTickCount` defaults to 2 in `[0, 10]`, limiting minor ticks between majors; compact layouts reduce subdivision by arc length. Legacy semicircle major / minor ticks are round-ended lines outside bands, differentiated by length / thickness rather than crossing them. Tick text uses `ValueFormat`; long ranges use scientific notation and colliding intermediate labels are omitted. Values and units use equal font size, baseline alignment, and centered layout. Both shrink together when needed; numbers can use compact formatting and long units ellipsize. Model values / units remain unchanged.

### Enum-selected layouts

`GaugeStyle` selects structure through the `GaugeStyle` enum; properties still control range, colors, units, and formatting. Default `Semicircle` keeps legacy drawing. Basic gauge numeric animation uses existing `IsAnimationEnabled` / `AnimationDuration` and `Motion.Enabled`, without an additional animation enum.

| GaugeStyle | Structure | Default start / clockwise sweep | Theme needle / ticks / major count |
| --- | --- | --- | --- |
| Semicircle | Legacy semicircle progress | 180° / 180° | False / False / 5 |
| Basic | Thin track, triangular needle | 135° / 270° | True / True / 11 |
| Speed | Semicircle speedometer, progress arc, needle anchor | 180° / 180° | True / True / 9 |
| Progress | 270° progress arc, line needle | 135° / 270° | True / True / 11 |
| StageSpeed | Segmented bands, needle in the current band's color | 135° / 270° | True / True / 11 |
| Grade | Semicircle bands, segment labels, short triangular marker | 180° / 180° | True / True / 9 |
| Temperature | No needle, two progress layers, center value | 160° / 220° | False / True / 13 |
| Barometer | Outer main track, independent inner scale, long line needle | 135° / 270° | True / True / 11 |

ControlTheme styles supply defaults; explicit values / compiled bindings take precedence. Changing the enum does not overwrite local settings, `Minimum`, `Maximum`, `Value`, `Unit`, or business colors. Without TheOne themes, needle, ticks, and major count retain ChartBase defaults.

```xml
<one:GaugeChart GaugeStyle="Temperature" Title="温度"
                Minimum="0" Maximum="60" Value="35.78" Unit="°C"
                AccentBrush="#FFAB91" SecondaryBrush="#FD7347" />

<one:GaugeChart GaugeStyle="Grade" Value="70" GaugeCaption="Grade Rating">
  <one:GaugeChart.GaugeSegments>
    <one:GaugeSegment Fraction="0.25" Brush="#FF6E76" Label="Grade D" />
    <one:GaugeSegment Fraction="0.5" Brush="#FDDD60" Label="Grade C" />
    <one:GaugeSegment Fraction="0.75" Brush="#58D9F9" Label="Grade B" />
    <one:GaugeSegment Fraction="1" Brush="#7CFFB2" Label="Grade A" />
  </one:GaugeChart.GaugeSegments>
</one:GaugeChart>
```

| Property | Default / purpose |
| --- | --- |
| GaugeStartAngle / GaugeSweepAngle | NaN follows the enum; start −360°–360°, clockwise sweep `(0, 360]`. 0° points right, 90° down. Full circles supported |
| ShowGaugeNeedle / ShowGaugeTicks | Enum-based theme defaults; explicit False hides elements. Barometer tick visibility applies to both scales |
| ShowGaugeProgress | null follows the enum; True / False forces progress visibility |
| ShowGaugeAnchor | null follows the enum; controls the center anchor only when the needle is shown |
| ShowGaugeValue / ShowGaugeTickLabels | Default True; controls value / tick text without changing data |
| GaugeNeedleShape | Auto follows the enum; Triangle / Line / Marker overrides shape |
| GaugeNeedleLength / GaugeNeedleWidth | Length defaults to 0.72 radius, range `(0, 1]`; width NaN follows shape, explicit pixels `(0, 50]` |
| GaugeTickPlacement | Auto follows the enum; Inside / Outside overrides major placement; barometer secondary ticks remain inside |
| GaugeBandRatio | Follows three theme densities; Basic, Progress, Grade, Barometer scale bands by structure; ratio is overridable |
| GaugeGradeLabelRotation / GaugeGradeLabelGap | Grade text follows a readable tangent by default; rotation NaN is automatic, `[−90, 90]` explicit degrees (0 horizontal). Gap from bands is NaN for font-based sizing or `[0, 100]` explicit pixels |
| GaugeCaption / GaugeCaptionOffset | Caption defaults to null; vertical top offset in radius units `[−1, 1]`, NaN follows the enum; separate from top Title |
| GaugeValueFontSize / GaugeValueOffset | NaN follows the enum; font `(0, 200]`, vertical radius-relative offset `[−1, 1]`, positive downward. Fonts shrink to fit |
| GaugeTrackBrush | Background track color, default GridBrush |
| GaugeNeedleBrush / GaugeTickBrush / GaugeValueBrush | Overrides needle, major ticks, and center value; otherwise theme / band colors |
| GaugeSegments | Independent empty collection per instance, supporting XAML children or binding replacement. Ordered bands with cumulative Fraction `(0, 1]` of Minimum–Maximum; Brush sets color, Label is used by Grade |
| GaugeSecondaryMinimum / GaugeSecondaryMaximum | Barometer secondary range, default 0–60, independent of main value / needle. Invalid ranges hide the secondary scale |
| GaugeSecondaryTickCount / GaugeSecondaryBrush | Secondary count defaults to 7 in `[2, 12]`; color defaults to Foreground |

StageSpeed defaults to three bands and Grade to four, using theme series colors. Set actual labels / business colors through `GaugeSegments`. Supply increasing Fraction values; nonincreasing entries are skipped and tails below 1 retain track fill. `ObservableCollection<GaugeSegment>` changes and segment-property updates redraw automatically. Reassign plain lists after additions / removals. Segment events are detached on visual-tree exit.

New properties also work with `Semicircle`: explicit use selects general gauge layout, while leaving them unset keeps legacy layout. All presets draw directly, animate value updates, and export stable target-value PNGs preserving style, bands, and overrides. Narrow layouts prioritize endpoints, omit colliding intermediate ticks, and fall back to value text at tiny sizes.

Reference: [Apache ECharts gauges](https://echarts.apache.org/examples/zh/index.html#chart-type-gauge). PreviewHost shows all eight presets simultaneously; expand the interaction / data experiment to switch the enum on the same instance.

Rotated text bounds participate in Grade sizing and keep labels outside bands. Visible labels do not cover bands or ticks. Narrow viewports check collisions using rotated bounds and omit unplaceable labels rather than pushing them onto the arc.

Numeric text requests tabular digits (`tnum`, when supported by the font). Font fitting reserves space from ranges and targets; units follow target layout rather than each interpolated width. With default `ValueFormat="0.##"`, integer animations show integers. Once continuous dragging includes decimals, that animation retains two digits even across integer targets, restoring final `ValueFormat` when finished. Explicit formats take precedence. Needle / progress motion remains continuous.

### Precision and sizing

Default `ValueFormat="0.##"` keeps fractional tick precision required by major spacing: range 0–1 with five majors shows 0, 0.25, 0.5, 0.75, 1. Explicit `ValueFormat` wins; integer formats omit fractions. Narrow ranges at large magnitudes keep additional significant digits to distinguish adjacent ticks.

The table describes compatible semicircle layout; other enum values adjust bands and numeric fonts by structure.

| Setting | Small | Normal | Large |
| --- | --- | --- | --- |
| Preferred width / height | 320 / 224 | 400 / 280 | 480 / 336 |
| Base font | 10 | 11 | 12 |
| Tick font | 9 | 10 | 11 |
| Center value font | 20 | 22 | 24 |
| GaugeBandRatio | 0.14 | 0.18 | 0.22 |

Values and units shrink when needed. Endpoints take priority; colliding intermediate labels are omitted, and smaller layouts use value-only fallback. Do not force identical Width / Height for all sizes: explicit dimensions / Grid stretching determine outer size, while size themes change internal density. PreviewHost's eight presets use widths 240 / 300 / 360 according to the top size selector.

See [Cartesian charts and sampling](chart-cartesian.md) for line / area / scatter label rules.

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

### DoughnutChart

[API / model reference](../../api/en/TheOne.Avalonia.Controls.DoughnutChart.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/DoughnutChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### GaugeChart

[API / model reference](../../api/en/TheOne.Avalonia.Controls.GaugeChart.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`GaugeCaption`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L74) | `string?` | writable; OneWay | `null` | Caption defaults to null; vertical top offset in radius units `[−1, 1]`, NaN follows the enum; separate from top Title |
| [`GaugeCaptionOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L69) | `double` | writable; OneWay | `double.NaN` | Caption defaults to null; vertical top offset in radius units `[−1, 1]`, NaN follows the enum; separate from top Title Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`GaugeGradeLabelGap`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L73) | `double` | writable; OneWay | `double.NaN` | Grade text follows a readable tangent by default; rotation NaN is automatic, `[−90, 90]` explicit degrees (0 horizontal). Gap from bands is NaN for font-based sizing or `[0, 100]` explicit pixels Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= 0 && value <= 100)` |
| [`GaugeGradeLabelRotation`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L71) | `double` | writable; OneWay | `double.NaN` | Grade text follows a readable tangent by default; rotation NaN is automatic, `[−90, 90]` explicit degrees (0 horizontal). Gap from bands is NaN for font-based sizing or `[0, 100]` explicit pixels Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -90 && value <= 90)` |
| [`GaugeNeedleBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L76) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Overrides needle, major ticks, and center value; otherwise theme / band colors |
| [`GaugeNeedleLength`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L64) | `double` | writable; OneWay | `0.72` | Length defaults to 0.72 radius, range `(0, 1]`; width NaN follows shape, explicit pixels `(0, 50]` Validation: `value => double.IsFinite(value) && value > 0 && value <= 1` |
| [`GaugeNeedleShape`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L62) | `TheOne.Avalonia.Controls.GaugeNeedleShape` | writable; OneWay | `GaugeNeedleShape.Auto` | Auto follows the enum; Triangle / Line / Marker overrides shape Validation: `Enum.IsDefined` Values: `Auto`, `Triangle`, `Line`, `Marker` |
| [`GaugeNeedleWidth`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L65) | `double` | writable; OneWay | `double.NaN` | Length defaults to 0.72 radius, range `(0, 1]`; width NaN follows shape, explicit pixels `(0, 50]` Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 50)` |
| [`GaugeSecondaryBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L83) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Secondary count defaults to 7 in `[2, 12]`; color defaults to Foreground |
| [`GaugeSecondaryMaximum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L81) | `double` | writable; OneWay | `60` | Barometer secondary range, default 0–60, independent of main value / needle. Invalid ranges hide the secondary scale Validation: `double.IsFinite` |
| [`GaugeSecondaryMinimum`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L80) | `double` | writable; OneWay | `0` | Barometer secondary range, default 0–60, independent of main value / needle. Invalid ranges hide the secondary scale Validation: `double.IsFinite` |
| [`GaugeSecondaryTickCount`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L82) | `int` | writable; OneWay | `7` | Secondary count defaults to 7 in `[2, 12]`; color defaults to Foreground Validation: `value => value >= 2 && value <= 12` |
| [`GaugeSegments`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L79) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.GaugeSegment>?` | writable; OneWay | `null` | Independent empty collection per instance, supporting XAML children or binding replacement. Ordered bands with cumulative Fraction `(0, 1]` of Minimum–Maximum; Brush sets color, Label is used by Grade |
| [`GaugeStartAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L56) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; start −360°–360°, clockwise sweep `(0, 360]`. 0° points right, 90° down. Full circles supported Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -360 && value <= 360)` |
| [`GaugeStyle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L55) | `TheOne.Avalonia.Controls.GaugeStyle` | writable; OneWay | `GaugeStyle.Semicircle` | Structure ; Default start / clockwise sweep ; Theme needle / ticks / major count Validation: `Enum.IsDefined` Values: `Semicircle`, `Basic`, `Speed`, `Progress`, `StageSpeed`, `Grade`, `Temperature`, `Barometer` |
| [`GaugeSweepAngle`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L57) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; start −360°–360°, clockwise sweep `(0, 360]`. 0° points right, 90° down. Full circles supported Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 360)` |
| [`GaugeTickBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L77) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Overrides needle, major ticks, and center value; otherwise theme / band colors |
| [`GaugeTickPlacement`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L63) | `TheOne.Avalonia.Controls.GaugeTickPlacement` | writable; OneWay | `GaugeTickPlacement.Auto` | Auto follows the enum; Inside / Outside overrides major placement; barometer secondary ticks remain inside Validation: `Enum.IsDefined` Values: `Auto`, `Inside`, `Outside` |
| [`GaugeTrackBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L75) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Background track color, default GridBrush |
| [`GaugeValueBrush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L78) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Overrides needle, major ticks, and center value; otherwise theme / band colors |
| [`GaugeValueFontSize`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L66) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; font `(0, 200]`, vertical radius-relative offset `[−1, 1]`, positive downward. Fonts shrink to fit Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value > 0 && value <= 200)` |
| [`GaugeValueOffset`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L67) | `double` | writable; OneWay | `double.NaN` | NaN follows the enum; font `(0, 200]`, vertical radius-relative offset `[−1, 1]`, positive downward. Fonts shrink to fit Validation: `value => double.IsNaN(value) &#124;&#124; (double.IsFinite(value) && value >= -1 && value <= 1)` |
| [`Kind`](../../../TheOne.Avalonia/Controls/GaugeChart.cs#L9) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |
| [`ShowGaugeAnchor`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L59) | `bool?` | writable; OneWay | `null` | null follows the enum; controls the center anchor only when the needle is shown |
| [`ShowGaugeProgress`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L58) | `bool?` | writable; OneWay | `null` | null follows the enum; True / False forces progress visibility |
| [`ShowGaugeTickLabels`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L61) | `bool` | writable; OneWay | `true` | Default True; controls value / tick text without changing data |
| [`ShowGaugeValue`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L60) | `bool` | writable; OneWay | `true` | Default True; controls value / tick text without changing data |

### PieChart

[API / model reference](../../api/en/TheOne.Avalonia.Controls.PieChart.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PieChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

### RadialChartBase

[API / model reference](../../api/en/TheOne.Avalonia.Controls.RadialChartBase.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

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
| [`TheOne.Avalonia.Controls.DoughnutChart`](../../api/en/TheOne.Avalonia.Controls.DoughnutChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GaugeChart`](../../api/en/TheOne.Avalonia.Controls.GaugeChart.md) | Class | 27 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GaugeNeedleShape`](../../api/en/TheOne.Avalonia.Controls.GaugeNeedleShape.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GaugeSegment`](../../api/en/TheOne.Avalonia.Controls.GaugeSegment.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GaugeStyle`](../../api/en/TheOne.Avalonia.Controls.GaugeStyle.md) | Enum | 8 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GaugeTickPlacement`](../../api/en/TheOne.Avalonia.Controls.GaugeTickPlacement.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.PieChart`](../../api/en/TheOne.Avalonia.Controls.PieChart.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.RadialChartBase`](../../api/en/TheOne.Avalonia.Controls.RadialChartBase.md) | Class | 0 | 0 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
