# WorkflowSerializer

[English](../en/TheOne.Avalonia.Workflow.WorkflowSerializer.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowSerializer.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowSerializer` · Class · `object`

Versioned, source-generated JSON. Templates, Content, delegates and history are not persisted.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static string WorkflowSerializer.Save(WorkflowDocument document)`](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L9) | Saves portable business-document JSON, excluding runtime credentials, visual objects and transient state. |
| [`static WorkflowDocument WorkflowSerializer.Load(string json)`](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L23) | Loads a typed business document from JSON under its version/field/structure validation contract. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowDocument`](TheOne.Avalonia.Workflow.WorkflowDocument.md)
