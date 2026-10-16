# FlowStep

[English](../en/TheOne.Avalonia.Controls.FlowStep.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowStep.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowStep` · Class · `Avalonia.Controls.ItemsControl`

A linear, non-selecting flow display with native content containers.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowStep.cs#L11)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L38) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`MinNodeWidth`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L37) | `double` | writable; OneWay; inherits | `180` | 150 / 180 / 210 Validation: `value => double.IsFinite(value) && value > 0` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L36) | `double` | writable; OneWay; inherits | `24` | 20 / 24 / 32 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L35) | `Avalonia.Layout.Orientation` | writable; OneWay; inherits | `Orientation.Horizontal` | Horizontal/vertical arrangement; updates node, connector and content layout without changing business order. Validation: `Enum.IsDefined` Values: `Horizontal`, `Vertical` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`FlowStep.FlowStep()`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L26) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<Orientation> FlowStep.OrientationProperty`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FlowStep.NodeSpacingProperty`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FlowStep.MinNodeWidthProperty`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> FlowStep.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/FlowStep.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.FlowNodeContent`](TheOne.Avalonia.Controls.FlowNodeContent.md)
- [`TheOne.Avalonia.Controls.FlowNodeItem`](TheOne.Avalonia.Controls.FlowNodeItem.md)
- [`TheOne.Avalonia.Controls.FlowNodeStatus`](TheOne.Avalonia.Controls.FlowNodeStatus.md)
