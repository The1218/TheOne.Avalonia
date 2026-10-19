# WorkflowConnection

[English](../en/TheOne.Avalonia.Workflow.WorkflowConnection.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowConnection.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowConnection` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[Inherited additional members: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L213)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Id`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L216) | `string` | writable; CLR / init-only | `Guid.NewGuid().ToString("N")` | Stable model identifier for persistence/references; avoid duplicates in one document. |
| [`Label`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L222) | `string` | writable; CLR / n/a | `""` | Display label, not a substitute for model identity, field paths or serialized identifiers. |
| [`SourceNodeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L217) | `string` | writable; CLR / init-only | `""` | Stable source-node Id of a connection, matching the document node collection. |
| [`SourcePortId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L218) | `string` | writable; CLR / init-only | `""` | Source output-port Id, belonging to the SourceNodeId node. |
| [`TargetNodeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L219) | `string` | writable; CLR / init-only | `""` | Stable target-node Id of a connection, matching the document node collection. |
| [`TargetPortId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L220) | `string` | writable; CLR / init-only | `""` | Target input-port Id, belonging to the TargetNodeId node. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowConnection.WorkflowConnection()`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L213) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
