# ChartPoint

[English](../en/TheOne.Avalonia.Controls.ChartPoint.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartPoint.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartPoint` · Class · `Avalonia.AvaloniaObject`

A mutable, observable data point. Non-finite coordinates are treated as gaps.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartData.cs#L41)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Label`](../../../TheOne.Avalonia/Controls/ChartData.cs#L72) | `string?` | writable; OneWay | `null` | Display label, not a substitute for model identity, field paths or serialized identifiers. |
| [`X`](../../../TheOne.Avalonia/Controls/ChartData.cs#L60) | `double` | writable; OneWay | `0 / default` | Horizontal model coordinate/value in its chart, workflow or report coordinate system. |
| [`Y`](../../../TheOne.Avalonia/Controls/ChartData.cs#L66) | `double` | writable; OneWay | `0 / default` | Vertical model coordinate/value in its chart, workflow or report coordinate system. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ChartPoint.ChartPoint()`](../../../TheOne.Avalonia/Controls/ChartData.cs#L49) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
| [`ChartPoint.ChartPoint(double x, double y, string? label = null)`](../../../TheOne.Avalonia/Controls/ChartData.cs#L53) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<double> ChartPoint.XProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L43) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> ChartPoint.YProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L44) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> ChartPoint.LabelProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L46) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
