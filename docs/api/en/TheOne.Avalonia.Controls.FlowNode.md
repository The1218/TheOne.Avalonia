# FlowNode

[English](../en/TheOne.Avalonia.Controls.FlowNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowNode.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowNode` · Class · `Avalonia.Controls.ContentControl`

A flow marker with independently templated content and an explicit business state.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowNode.cs#L17)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ContentSpacing`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L83) | `double` | writable; OneWay | `12` | 10 / 12 / 16 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`DefaultContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L90) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Fallback for FlowNodeContent; OneTheme supplies the default four-field template. |
| [`EffectiveContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L96) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; OneWay | `computed / state` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`EffectiveStateText`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L97) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IndicatorSize`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L77) | `double` | writable; OneWay | `30` | 24 / 30 / 36 Validation: `value => double.IsFinite(value) && value > 0` |
| [`IsCompleted`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L99) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsInProgress`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L100) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsVertical`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L98) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Orientation`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L71) | `Avalonia.Layout.Orientation` | writable; OneWay | `Orientation.Horizontal` | Horizontal/vertical arrangement; updates node, connector and content layout without changing business order. Validation: `Enum.IsDefined` Values: `Horizontal`, `Vertical` |
| [`Status`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L65) | `TheOne.Avalonia.Controls.FlowNodeStatus` | writable; OneWay | `0 / default` | Marker Validation: `Enum.IsDefined` Values: `Pending`, `InProgress`, `Completed` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`FlowNode.FlowNode()`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L57) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<FlowNodeStatus> FlowNode.StatusProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Orientation> FlowNode.OrientationProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FlowNode.IndicatorSizeProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FlowNode.ContentSpacingProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> FlowNode.DefaultContentTemplateProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<FlowNode, IDataTemplate?> FlowNode.EffectiveContentTemplateProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<FlowNode, string> FlowNode.EffectiveStateTextProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L40) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<FlowNode, bool> FlowNode.IsVerticalProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L43) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<FlowNode, bool> FlowNode.IsCompletedProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L46) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<FlowNode, bool> FlowNode.IsInProgressProperty`](../../../TheOne.Avalonia/Controls/FlowNode.cs#L49) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.FlowNodeContent`](TheOne.Avalonia.Controls.FlowNodeContent.md)
- [`TheOne.Avalonia.Controls.FlowNodeItem`](TheOne.Avalonia.Controls.FlowNodeItem.md)
- [`TheOne.Avalonia.Controls.FlowNodeStatus`](TheOne.Avalonia.Controls.FlowNodeStatus.md)
