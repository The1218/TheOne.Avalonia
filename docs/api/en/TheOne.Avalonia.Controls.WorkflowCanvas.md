# WorkflowCanvas

[English](../en/TheOne.Avalonia.Controls.WorkflowCanvas.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.WorkflowCanvas.md) · [API index](README.md)

`TheOne.Avalonia.Controls.WorkflowCanvas` · Class · `Avalonia.Controls.Control`

WorkflowEditor template part. Graph coordinates are independent of zoom and pan.

[Source / 声明](../../../TheOne.Avalonia/Controls/WorkflowCanvas.Rendering.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`Point WorkflowCanvas.ToWorld(Point point)`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L97) | Converts viewport coordinates into document-world coordinates, accounting for zoom/pan. |
| [`override void WorkflowCanvas.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L214) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`void WorkflowCanvas.CancelGesture()`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L487) | Cancels the current canvas gesture and clears drag/connection previews. |
| [`WorkflowCanvas.WorkflowCanvas()`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L32) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
