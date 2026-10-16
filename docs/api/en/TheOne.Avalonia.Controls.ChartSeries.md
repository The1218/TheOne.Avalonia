# ChartSeries

[English](../en/TheOne.Avalonia.Controls.ChartSeries.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartSeries.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartSeries` · Class · `Avalonia.AvaloniaObject`

One series. Observable collections and point properties update attached charts automatically.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartData.cs#L27)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Brush`](../../../TheOne.Avalonia/Controls/ChartData.cs#L39) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Current series/segment brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`IsVisible`](../../../TheOne.Avalonia/Controls/ChartData.cs#L40) | `bool` | writable; OneWay | `true` | Whether the series participates in rendering and visible-data range calculation. |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/ChartData.cs#L38) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartPoint>?` | writable; OneWay | `null` | Bind source models rather than precreated controls; use notified collections for changes and a bounded viewport. |
| [`Name`](../../../TheOne.Avalonia/Controls/ChartData.cs#L37) | `string` | writable; OneWay | `"Series"` | Name used for presentation/identity; keep it consistent with templates and keys. |
| [`Unit`](../../../TheOne.Avalonia/Controls/ChartData.cs#L44) | `string?` | writable; OneWay | `null` | Optional tooltip/data-label unit; null inherits the assigned chart axis unit. |
| [`ValueFormat`](../../../TheOne.Avalonia/Controls/ChartData.cs#L46) | `string?` | writable; OneWay | `null` | Optional raw-value numeric format; null inherits the assigned chart axis format. |
| [`YAxis`](../../../TheOne.Avalonia/Controls/ChartData.cs#L42) | `TheOne.Avalonia.Controls.ChartYAxis` | writable; OneWay | `ChartYAxis.Primary` | The independent value scale used by this series in Cartesian charts. Validation: `value => value is ChartYAxis.Primary or ChartYAxis.Secondary` Values: `Primary`, `Secondary` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ChartSeries.ChartSeries()`](../../../TheOne.Avalonia/Controls/ChartData.cs#L47) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string> ChartSeries.NameProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IList<ChartPoint>?> ChartSeries.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> ChartSeries.BrushProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> ChartSeries.IsVisibleProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ChartYAxis> ChartSeries.YAxisProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> ChartSeries.UnitProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> ChartSeries.ValueFormatProperty`](../../../TheOne.Avalonia/Controls/ChartData.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ChartPoint`](TheOne.Avalonia.Controls.ChartPoint.md)
- [`TheOne.Avalonia.Controls.ChartYAxis`](TheOne.Avalonia.Controls.ChartYAxis.md)
