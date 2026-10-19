# WorkflowNodeDefinition

[English](../en/TheOne.Avalonia.Workflow.WorkflowNodeDefinition.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeDefinition.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowNodeDefinition` · Class · `object`

The host supplies node kinds and factories. Returned instances must have fresh IDs.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L230)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Category`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L235) | `string` | writable; CLR / init-only | `"基础节点"` | Category name organizing node definitions in the library. |
| [`Content`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L236) | `object?` | writable; CLR / init-only | `null` | Host content/presentation object; follow the content-template scope and do not persist visual objects as business data. |
| [`CreateNode`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L237) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode>` | writable; CLR / init-only | `null` | Host factory creating independent node models with their type keys. |
| [`Description`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L234) | `string` | writable; CLR / init-only | `""` | Descriptive text supplied by the host; omitted values keep their initial state. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L233) | `string` | writable; CLR / init-only | `null` | Display title, distinct from business identity or a localization key. |
| [`TypeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L232) | `string` | writable; CLR / init-only | `null` | Stable host node-definition key used to restore definitions/nonserialized content after loading, without CLR type lookup. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowNodeDefinition.WorkflowNodeDefinition()`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L230) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowNode`](TheOne.Avalonia.Workflow.WorkflowNode.md)
