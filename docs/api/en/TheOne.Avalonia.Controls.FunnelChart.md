# FunnelChart

[English](../en/TheOne.Avalonia.Controls.FunnelChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FunnelChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FunnelChart` · Class · `TheOne.Avalonia.Controls.SpecializedChartBase`

[Inherited additional members: SpecializedChartBase](TheOne.Avalonia.Controls.SpecializedChartBase.md)

A native funnel chart. Source order is retained unless sorting is explicitly requested.

[Source / 声明](../../../TheOne.Avalonia/Controls/FunnelChart.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Gap`](../../../TheOne.Avalonia/Controls/FunnelChart.cs#L30) | `double` | writable; OneWay | `4` | The vertical gap between stages, in logical pixels. Validation: `value => double.IsFinite(value) && value >= 0` |
| [`Kind`](../../../TheOne.Avalonia/Controls/FunnelChart.cs#L25) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`MinimumWidthRatio`](../../../TheOne.Avalonia/Controls/FunnelChart.cs#L32) | `double` | writable; OneWay | `0 / default` | Optional minimum stage width relative to the widest stage. Zero preserves the value ratio. Validation: `value => double.IsFinite(value) && value >= 0 && value <= 1` |
| [`SortMode`](../../../TheOne.Avalonia/Controls/FunnelChart.cs#L28) | `TheOne.Avalonia.Controls.FunnelSortMode` | writable; OneWay | `0 / default` | SortMode configuration on FunnelChart; type and initial value are listed here. SortModeProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => value is FunnelSortMode.None or FunnelSortMode.Ascending or FunnelSortMode.Descending` Values: `None`, `Ascending`, `Descending` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`FunnelChart.FunnelChart()`](../../../TheOne.Avalonia/Controls/FunnelChart.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<FunnelSortMode> FunnelChart.SortModeProperty`](../../../TheOne.Avalonia/Controls/FunnelChart.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FunnelChart.GapProperty`](../../../TheOne.Avalonia/Controls/FunnelChart.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FunnelChart.MinimumWidthRatioProperty`](../../../TheOne.Avalonia/Controls/FunnelChart.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
- [`TheOne.Avalonia.Controls.FunnelSortMode`](TheOne.Avalonia.Controls.FunnelSortMode.md)
