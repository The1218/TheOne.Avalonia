# BoxPlotChart

[English](../en/TheOne.Avalonia.Controls.BoxPlotChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.BoxPlotChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.BoxPlotChart` · Class · `TheOne.Avalonia.Controls.StatisticalChartBase`

[Inherited additional members: StatisticalChartBase](TheOne.Avalonia.Controls.StatisticalChartBase.md)

Precomputed five-number summaries with observable outliers, cached geometry and numeric axes.

[Source / 声明](../../../TheOne.Avalonia/Controls/BoxPlotChart.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/BoxPlotChart.cs#L22) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`OutlierRadius`](../../../TheOne.Avalonia/Controls/BoxPlotChart.cs#L25) | `double` | writable; OneWay | `3` | OutlierRadius configuration on BoxPlotChart; type and initial value are listed here. OutlierRadiusProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => double.IsFinite(value) && value > 0 && value <= 32` |
| [`ShowOutliers`](../../../TheOne.Avalonia/Controls/BoxPlotChart.cs#L24) | `bool` | writable; OneWay | `true` | ShowOutliers configuration on BoxPlotChart; type and initial value are listed here. ShowOutliersProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`BoxPlotChart.BoxPlotChart()`](../../../TheOne.Avalonia/Controls/BoxPlotChart.cs#L8) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<bool> BoxPlotChart.ShowOutliersProperty`](../../../TheOne.Avalonia/Controls/BoxPlotChart.cs#L10) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> BoxPlotChart.OutlierRadiusProperty`](../../../TheOne.Avalonia/Controls/BoxPlotChart.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
