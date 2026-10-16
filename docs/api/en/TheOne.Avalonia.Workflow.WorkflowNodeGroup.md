# WorkflowNodeGroup

[English](../en/TheOne.Avalonia.Workflow.WorkflowNodeGroup.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeGroup.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowNodeGroup` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L105)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Items`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L105) | `TheOne.Avalonia.Workflow.WorkflowNodeDefinition[]` | writable; CLR / init-only | `required constructor argument` | Current item collection; element types and mutation rights follow the signature/control contract. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L105) | `string` | writable; CLR / init-only | `required constructor argument` | Display title, distinct from business identity or a localization key. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowNodeGroup.WorkflowNodeGroup(string Title, WorkflowNodeDefinition[] Items)`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L105) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowNodeDefinition`](TheOne.Avalonia.Workflow.WorkflowNodeDefinition.md)
