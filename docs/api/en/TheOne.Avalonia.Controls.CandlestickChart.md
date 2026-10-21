# CandlestickChart

[English](../en/TheOne.Avalonia.Controls.CandlestickChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CandlestickChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CandlestickChart` · Class · `TheOne.Avalonia.Controls.StatisticalChartBase`

[Inherited additional members: StatisticalChartBase](TheOne.Avalonia.Controls.StatisticalChartBase.md)

Cached OHLC candles on numeric X coordinates, with theme-aware rise/fall colors.

[Source / 声明](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`DownBrush`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L18) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | DownBrush configuration on CandlestickChart; type and initial value are listed here. DownBrushProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`IsHollowUp`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L19) | `bool` | writable; OneWay | `false` | IsHollowUp configuration on CandlestickChart; type and initial value are listed here. IsHollowUpProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Kind`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L16) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |
| [`UpBrush`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L17) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | UpBrush configuration on CandlestickChart; type and initial value are listed here. UpBrushProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`CandlestickChart.CandlestickChart()`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> CandlestickChart.UpBrushProperty`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> CandlestickChart.DownBrushProperty`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L10) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> CandlestickChart.IsHollowUpProperty`](../../../TheOne.Avalonia/Controls/CandlestickChart.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
