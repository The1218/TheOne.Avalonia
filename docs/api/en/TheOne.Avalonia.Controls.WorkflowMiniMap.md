# WorkflowMiniMap

[English](../en/TheOne.Avalonia.Controls.WorkflowMiniMap.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.WorkflowMiniMap.md) · [API index](README.md)

`TheOne.Avalonia.Controls.WorkflowMiniMap` · Class · `Avalonia.Controls.Control`

[Source / 声明](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Editor`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L13) | `TheOne.Avalonia.Controls.WorkflowEditor?` | writable; OneWay | `null` | WorkflowEditor associated with the minimap, sharing document/viewport rather than creating independent workflow state. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void WorkflowMiniMap.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L28) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`WorkflowMiniMap.WorkflowMiniMap()`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L11) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<WorkflowEditor?> WorkflowMiniMap.EditorProperty`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.WorkflowEditor`](TheOne.Avalonia.Controls.WorkflowEditor.md)
