# TreemapChart

[English](../en/TheOne.Avalonia.Controls.TreemapChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TreemapChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TreemapChart` · Class · `TheOne.Avalonia.Controls.HierarchyChartBase`

[Inherited additional members: HierarchyChartBase](TheOne.Avalonia.Controls.HierarchyChartBase.md)

A stable slice-and-dice treemap with nested branch headers.

[Source / 声明](../../../TheOne.Avalonia/Controls/TreemapChart.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Gap`](../../../TheOne.Avalonia/Controls/TreemapChart.cs#L23) | `double` | writable; OneWay | `3` | Gap configuration on TreemapChart; type and initial value are listed here. GapProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => double.IsFinite(value) && value >= 0` |
| [`HeaderHeight`](../../../TheOne.Avalonia/Controls/TreemapChart.cs#L25) | `double` | writable; OneWay | `20` | Maximum parent header height; labels and padding are reserved inside the allocated tile. Validation: `value => double.IsFinite(value) && value >= 0` |
| [`Kind`](../../../TheOne.Avalonia/Controls/TreemapChart.cs#L22) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea`, `Heatmap`, `Radar`, `Funnel`, `BoxPlot`, `Candlestick`, `Treemap`, `Sunburst` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`TreemapChart.TreemapChart()`](../../../TheOne.Avalonia/Controls/TreemapChart.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<double> TreemapChart.GapProperty`](../../../TheOne.Avalonia/Controls/TreemapChart.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> TreemapChart.HeaderHeightProperty`](../../../TheOne.Avalonia/Controls/TreemapChart.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
