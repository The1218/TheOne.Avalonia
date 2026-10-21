# WorkflowConnection

[English](../en/TheOne.Avalonia.Workflow.WorkflowConnection.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowConnection.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowConnection` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[Inherited additional members: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L213)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Id`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L232) | `string` | writable; CLR / init-only | `Guid.NewGuid().ToString("N")` | Stable model identifier for persistence/references; avoid duplicates in one document. |
| [`Label`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L238) | `string` | writable; CLR / n/a | `""` | Display label, not a substitute for model identity, field paths or serialized identifiers. |
| [`Properties`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L245) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowProperty>` | read-only; CLR / n/a | `[]` | 宿主定义的可持久化字段，例如分支类型、表达式和优先级；不从显示标签推断业务语义。 |
| [`SourceNodeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L233) | `string` | writable; CLR / init-only | `""` | Stable source-node Id of a connection, matching the document node collection. |
| [`SourcePortId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L234) | `string` | writable; CLR / init-only | `""` | Source output-port Id, belonging to the SourceNodeId node. |
| [`TargetNodeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L235) | `string` | writable; CLR / init-only | `""` | Stable target-node Id of a connection, matching the document node collection. |
| [`TargetPortId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L236) | `string` | writable; CLR / init-only | `""` | Target input-port Id, belonging to the TargetNodeId node. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowConnection.WorkflowConnection()`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L219) | 跟踪连线业务字段；字段变化与节点编辑共用文档修订和撤销边界。 |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowProperty`](TheOne.Avalonia.Workflow.WorkflowProperty.md)
