# CardTabStrip

[English](../en/TheOne.Avalonia.Controls.CardTabStrip.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CardTabStrip.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CardTabStrip` · Class · `Avalonia.Controls.DockPanel`

Header scrolling and adjacent-tab separators for the card theme.

[Source / 声明](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L12)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanScrollBackward`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L41) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanScrollForward`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L42) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasOverflow`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L40) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`CardTabStrip.CardTabStrip()`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L34) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly DirectProperty<CardTabStrip, bool> CardTabStrip.HasOverflowProperty`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<CardTabStrip, bool> CardTabStrip.CanScrollBackwardProperty`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<CardTabStrip, bool> CardTabStrip.CanScrollForwardProperty`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
