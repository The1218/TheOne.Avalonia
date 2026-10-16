# WorkflowValidationIssue

[English](../en/TheOne.Avalonia.Workflow.WorkflowValidationIssue.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowValidationIssue.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowValidationIssue` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L18)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ConnectionId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L18) | `string?` | writable; CLR / init-only | `null` | Connection Id associated with a validation issue; null means no specific connection. |
| [`Message`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L18) | `string` | writable; CLR / init-only | `required constructor argument` | Result message for users/diagnostics; status fields still determine the operation outcome. |
| [`NodeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L18) | `string?` | writable; CLR / init-only | `null` | Node Id associated with a reference/validation issue; nullable values can indicate no specific node. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowValidationIssue.WorkflowValidationIssue(string Message, string? NodeId = null, string? ConnectionId = null)`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L18) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
