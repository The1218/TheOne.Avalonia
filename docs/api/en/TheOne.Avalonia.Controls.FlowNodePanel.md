# FlowNodePanel

[English](../en/TheOne.Avalonia.Controls.FlowNodePanel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowNodePanel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowNodePanel` · Class · `Avalonia.Controls.Panel`

Positions a marker above horizontal content or to the left of vertical content.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L59)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ContentSpacing`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L64) | `double` | writable; OneWay | `inherited registration` | 10 / 12 / 16 |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L63) | `Avalonia.Layout.Orientation` | writable; OneWay | `inherited registration` | Horizontal/vertical arrangement; updates node, connector and content layout without changing business order. Values: `Horizontal`, `Vertical` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`FlowNodePanel.FlowNodePanel()`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L59) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<Orientation> FlowNodePanel.OrientationProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L61) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FlowNodePanel.ContentSpacingProperty`](../../../TheOne.Avalonia/Controls/FlowStepPanel.cs#L62) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
