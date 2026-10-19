# WorkflowNode

[English](../en/TheOne.Avalonia.Workflow.WorkflowNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowNode.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowNode` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[Inherited additional members: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L112)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Content`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L198) | `object?` | writable; CLR / n/a | `null` | Optional host presentation data, never serialized. Restore it from TypeId after loading. |
| [`Description`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L156) | `string` | writable; CLR / n/a | `""` | Descriptive text supplied by the host; omitted values keep their initial state. |
| [`Id`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L135) | `string` | writable; CLR / init-only | `Guid.NewGuid().ToString("N")` | Stable model identifier for persistence/references; avoid duplicates in one document. |
| [`IsAnnotation`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L138) | `bool` | writable; CLR / init-only | `false` | Marks an annotation node rather than an executable business step. |
| [`Ports`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L204) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowPort>` | read-only; CLR / n/a | `[]` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`Properties`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L205) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowProperty>` | read-only; CLR / n/a | `[]` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`Shape`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L137) | `TheOne.Avalonia.Workflow.WorkflowNodeShape` | writable; CLR / init-only | `0 / default` | Node appearance enum, Card/Pill/Note, independent of TypeId. Values: `Card`, `Pill`, `Note` |
| [`State`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L182) | `TheOne.Avalonia.Workflow.WorkflowNodeState` | writable; CLR / n/a | `0 / default` | Model state enum; see its values and owning control for persistence/behavior semantics. Values: `Pending`, `Running`, `Completed`, `Failed`, `Skipped` |
| [`StateText`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L191) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L150) | `string` | writable; CLR / n/a | `""` | Display title, distinct from business identity or a localization key. |
| [`TypeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L136) | `string` | writable; CLR / init-only | `"task"` | Stable host node-definition key used to restore definitions/nonserialized content after loading, without CLR type lookup. |
| [`Width`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L140) | `double` | writable; CLR / n/a | `180` | Width configuration; units follow the owning model, without changing data-coordinate meaning. |
| [`X`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L162) | `double` | writable; CLR / n/a | `0 / default` | Horizontal model coordinate/value in its chart, workflow or report coordinate system. |
| [`Y`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L172) | `double` | writable; CLR / n/a | `0 / default` | Vertical model coordinate/value in its chart, workflow or report coordinate system. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowNode.WorkflowNode()`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L121) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowNodeShape`](TheOne.Avalonia.Workflow.WorkflowNodeShape.md)
- [`TheOne.Avalonia.Workflow.WorkflowNodeState`](TheOne.Avalonia.Workflow.WorkflowNodeState.md)
- [`TheOne.Avalonia.Workflow.WorkflowPort`](TheOne.Avalonia.Workflow.WorkflowPort.md)
- [`TheOne.Avalonia.Workflow.WorkflowProperty`](TheOne.Avalonia.Workflow.WorkflowProperty.md)
