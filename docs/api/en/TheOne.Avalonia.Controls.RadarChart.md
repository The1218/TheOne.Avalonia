# RadarChart

[English](../en/TheOne.Avalonia.Controls.RadarChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RadarChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RadarChart` · Class · `TheOne.Avalonia.Controls.SpecializedChartBase`

[Inherited additional members: SpecializedChartBase](TheOne.Avalonia.Controls.SpecializedChartBase.md)

A radar chart whose point indexes map to independently scaled axes.

[Source / 声明](../../../TheOne.Avalonia/Controls/RadarChart.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Axes`](../../../TheOne.Avalonia/Controls/RadarChart.cs#L33) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.RadarAxis>?` | writable; OneWay | `null` | At least three dimensions. Each series point at index i belongs to axis i. |
| [`FillOpacity`](../../../TheOne.Avalonia/Controls/RadarChart.cs#L35) | `double` | writable; OneWay | `.16` | FillOpacity configuration on RadarChart; type and initial value are listed here. FillOpacityProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => double.IsFinite(value) && value >= 0 && value <= 1` |
| [`GridLevels`](../../../TheOne.Avalonia/Controls/RadarChart.cs#L34) | `int` | writable; OneWay | `5` | GridLevels configuration on RadarChart; type and initial value are listed here. GridLevelsProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => value is >= 1 and <= 20` |
| [`Kind`](../../../TheOne.Avalonia/Controls/RadarChart.cs#L31) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`RadarChart.RadarChart()`](../../../TheOne.Avalonia/Controls/RadarChart.cs#L29) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IList<RadarAxis>?> RadarChart.AxesProperty`](../../../TheOne.Avalonia/Controls/RadarChart.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<int> RadarChart.GridLevelsProperty`](../../../TheOne.Avalonia/Controls/RadarChart.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RadarChart.FillOpacityProperty`](../../../TheOne.Avalonia/Controls/RadarChart.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
- [`TheOne.Avalonia.Controls.RadarAxis`](TheOne.Avalonia.Controls.RadarAxis.md)
