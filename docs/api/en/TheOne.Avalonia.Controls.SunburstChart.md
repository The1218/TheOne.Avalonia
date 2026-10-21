# SunburstChart

[English](../en/TheOne.Avalonia.Controls.SunburstChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.SunburstChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.SunburstChart` · Class · `TheOne.Avalonia.Controls.HierarchyChartBase`

[Inherited additional members: HierarchyChartBase](TheOne.Avalonia.Controls.HierarchyChartBase.md)

A cached hierarchical annular chart with exact sector hit testing.

[Source / 声明](../../../TheOne.Avalonia/Controls/SunburstChart.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/SunburstChart.cs#L22) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`RingGap`](../../../TheOne.Avalonia/Controls/SunburstChart.cs#L24) | `double` | writable; OneWay | `2` | Separation between rings and adjacent sectors, in logical pixels. Validation: `value => double.IsFinite(value) && value >= 0` |
| [`StartAngle`](../../../TheOne.Avalonia/Controls/SunburstChart.cs#L26) | `double` | writable; OneWay | `-90` | Clockwise start angle in degrees. -90 starts at the top. Validation: `double.IsFinite` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`SunburstChart.SunburstChart()`](../../../TheOne.Avalonia/Controls/SunburstChart.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<double> SunburstChart.RingGapProperty`](../../../TheOne.Avalonia/Controls/SunburstChart.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> SunburstChart.StartAngleProperty`](../../../TheOne.Avalonia/Controls/SunburstChart.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
