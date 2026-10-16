# WorkflowDataReference

[English](../en/TheOne.Avalonia.Workflow.WorkflowDataReference.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowDataReference.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowDataReference` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L17)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`FieldPath`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L17) | `string` | writable; CLR / init-only | `""` | Stable upstream output-field path key defined by the host, without CLR reflection lookup. |
| [`NodeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L17) | `string` | writable; CLR / init-only | `required constructor argument` | Node Id associated with a reference/validation issue; nullable values can indicate no specific node. |
| [`OutputPortId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L17) | `string` | writable; CLR / init-only | `required constructor argument` | Upstream output-port Id used by a data reference. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowDataReference.WorkflowDataReference(string NodeId, string OutputPortId, string FieldPath = "")`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L17) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
