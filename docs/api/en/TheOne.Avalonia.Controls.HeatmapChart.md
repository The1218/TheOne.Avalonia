# HeatmapChart

[English](../en/TheOne.Avalonia.Controls.HeatmapChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.HeatmapChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.HeatmapChart` · Class · `TheOne.Avalonia.Controls.SpecializedChartBase`

[Inherited additional members: SpecializedChartBase](TheOne.Avalonia.Controls.SpecializedChartBase.md)

A native continuous-color heatmap with coordinate-based cells and cached color batches.

[Source / 声明](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CellGap`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L47) | `double` | writable; OneWay | `2` | The visual gap between cells, in logical pixels. Validation: `value => double.IsFinite(value) && value >= 0` |
| [`CellHeight`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L45) | `double` | writable; OneWay | `1` | The cell height in Y coordinate units. Validation: `value => double.IsFinite(value) && value > 0` |
| [`CellWidth`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L43) | `double` | writable; OneWay | `1` | The cell width in X coordinate units. Validation: `value => double.IsFinite(value) && value > 0` |
| [`HighColor`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L52) | `Avalonia.Media.Color?` | writable; OneWay | `null` | HighColor configuration on HeatmapChart; type and initial value are listed here. HighColorProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Kind`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L38) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`LowColor`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L50) | `Avalonia.Media.Color?` | writable; OneWay | `null` | LowColor configuration on HeatmapChart; type and initial value are listed here. LowColorProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`MidColor`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L51) | `Avalonia.Media.Color?` | writable; OneWay | `null` | MidColor configuration on HeatmapChart; type and initial value are listed here. MidColorProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`ValueMaximum`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L49) | `double` | writable; OneWay | `double.NaN` | ValueMaximum configuration on HeatmapChart; type and initial value are listed here. ValueMaximumProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => !double.IsInfinity(value)` |
| [`ValueMinimum`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L48) | `double` | writable; OneWay | `double.NaN` | ValueMinimum configuration on HeatmapChart; type and initial value are listed here. ValueMinimumProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => !double.IsInfinity(value)` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`HeatmapChart.HeatmapChart()`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<double> HeatmapChart.CellWidthProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> HeatmapChart.CellHeightProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> HeatmapChart.CellGapProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> HeatmapChart.ValueMinimumProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> HeatmapChart.ValueMaximumProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Color?> HeatmapChart.LowColorProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Color?> HeatmapChart.MidColorProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Color?> HeatmapChart.HighColorProperty`](../../../TheOne.Avalonia/Controls/HeatmapChart.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
