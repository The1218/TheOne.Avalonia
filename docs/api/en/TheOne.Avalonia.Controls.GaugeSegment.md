# GaugeSegment

[English](../en/TheOne.Avalonia.Controls.GaugeSegment.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GaugeSegment.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GaugeSegment` · Class · `Avalonia.AvaloniaObject`

An ordered band ending at Fraction of the gauge range. Labels are used by Grade.

[Source / 声明](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L14)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Brush`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L20) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Current series/segment brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`Fraction`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L19) | `double` | writable; OneWay | `1` | Gauge segment fraction defining its range with other segments, independent of pixel widths. Validation: `value => double.IsFinite(value) && value > 0 && value <= 1` |
| [`Label`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L21) | `string?` | writable; OneWay | `null` | Display label, not a substitute for model identity, field paths or serialized identifiers. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`GaugeSegment.GaugeSegment()`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L14) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<double> GaugeSegment.FractionProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GaugeSegment.BrushProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> GaugeSegment.LabelProperty`](../../../TheOne.Avalonia/Controls/GaugeChart.Properties.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
