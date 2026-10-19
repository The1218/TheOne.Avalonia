# WorkflowMiniMap

[English](../en/TheOne.Avalonia.Controls.WorkflowMiniMap.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.WorkflowMiniMap.md) · [API index](README.md)

`TheOne.Avalonia.Controls.WorkflowMiniMap` · Class · `Avalonia.Controls.Control`

[Source / 声明](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Editor`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L24) | `TheOne.Avalonia.Controls.WorkflowEditor?` | writable; OneWay | `null` | WorkflowEditor associated with the minimap, sharing document/viewport rather than creating independent workflow state. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void WorkflowMiniMap.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L97) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`WorkflowMiniMap.WorkflowMiniMap()`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L19) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<WorkflowEditor?> WorkflowMiniMap.EditorProperty`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L10) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.WorkflowEditor`](TheOne.Avalonia.Controls.WorkflowEditor.md)
