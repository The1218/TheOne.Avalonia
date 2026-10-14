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
