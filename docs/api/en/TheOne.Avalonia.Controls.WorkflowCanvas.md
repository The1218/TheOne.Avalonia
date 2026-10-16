# WorkflowCanvas

[English](../en/TheOne.Avalonia.Controls.WorkflowCanvas.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.WorkflowCanvas.md) · [API index](README.md)

`TheOne.Avalonia.Controls.WorkflowCanvas` · Class · `Avalonia.Controls.Control`

WorkflowEditor template part. Graph coordinates are independent of zoom and pan.

[Source / 声明](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L16)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`Point WorkflowCanvas.ToWorld(Point point)`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L71) | Converts viewport coordinates into document-world coordinates, accounting for zoom/pan. |
| [`override void WorkflowCanvas.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L133) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`void WorkflowCanvas.CancelGesture()`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L276) | Cancels the current canvas gesture and clears drag/connection previews. |
| [`WorkflowCanvas.WorkflowCanvas()`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L31) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |
