# RadarAxis

[English](../en/TheOne.Avalonia.Controls.RadarAxis.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RadarAxis.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RadarAxis` · Class · `Avalonia.AvaloniaObject`

The name and independent scale of a radar dimension.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L27)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Maximum`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L48) | `double` | writable; OneWay | `100` | Maximum configuration on RadarAxis; type and initial value are listed here. MaximumProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `double.IsFinite` |
| [`Minimum`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L42) | `double` | writable; OneWay | `0 / default` | Minimum configuration on RadarAxis; type and initial value are listed here. MinimumProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `double.IsFinite` |
| [`Name`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L36) | `string` | writable; OneWay | `"Axis"` | Name used for presentation/identity; keep it consistent with templates and keys. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`RadarAxis.RadarAxis()`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L27) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string> RadarAxis.NameProperty`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RadarAxis.MinimumProperty`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> RadarAxis.MaximumProperty`](../../../TheOne.Avalonia/Controls/ChartSpecializedData.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
