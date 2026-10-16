# FlowStepPanel

[English](../en/TheOne.Avalonia.Controls.FlowStepPanel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowStepPanel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowStepPanel` · Class · `Avalonia.Controls.Panel`

Equal horizontal slots or natural vertical rows, with rails between actual markers.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`MinNodeWidth`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L23) | `double` | writable; OneWay; inherits | `180` | 150 / 180 / 210 Validation: `value => double.IsFinite(value) && value > 0` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L22) | `double` | writable; OneWay; inherits | `24` | 20 / 24 / 32 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L21) | `Avalonia.Layout.Orientation` | writable; OneWay; inherits | `Orientation.Horizontal` | Horizontal/vertical arrangement; updates node, connector and content layout without changing business order. Validation: `Enum.IsDefined` Values: `Horizontal`, `Vertical` |
| [`ViewportWidth`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L24) | `double` | writable; OneWay | `0` | Available width supplied to flow layout, normally maintained by the template. Validation: `value => double.IsFinite(value) && value >= 0` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`FlowStepPanel.FlowStepPanel()`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L10) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<Orientation> FlowStepPanel.OrientationProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FlowStepPanel.NodeSpacingProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FlowStepPanel.MinNodeWidthProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FlowStepPanel.ViewportWidthProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
