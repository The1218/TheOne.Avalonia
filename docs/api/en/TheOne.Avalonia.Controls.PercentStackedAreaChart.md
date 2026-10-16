# PercentStackedAreaChart

[English](../en/TheOne.Avalonia.Controls.PercentStackedAreaChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.PercentStackedAreaChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.PercentStackedAreaChart` · Class · `TheOne.Avalonia.Controls.CartesianChartBase`

[Inherited additional members: CartesianChartBase](TheOne.Avalonia.Controls.CartesianChartBase.md)

A fixed PercentStackedArea chart control with its own theme and visual state.

[Source / 声明](../../../TheOne.Avalonia/Controls/PercentStackedAreaChart.cs#L4)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Kind`](../../../TheOne.Avalonia/Controls/PercentStackedAreaChart.cs#L6) | `TheOne.Avalonia.Controls.ChartKind` | read-only; CLR / n/a | `accessor-defined / state` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Line`, `Area`, `Bar`, `HorizontalBar`, `Scatter`, `Pie`, `Doughnut`, `Gauge`, `StackedBar`, `StackedHorizontalBar`, `StackedArea`, `PercentStackedBar`, `PercentStackedHorizontalBar`, `PercentStackedArea` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`PercentStackedAreaChart.PercentStackedAreaChart()`](../../../TheOne.Avalonia/Controls/PercentStackedAreaChart.cs#L4) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartKind`](TheOne.Avalonia.Controls.ChartKind.md)
