# WorkflowNodeEditor

[English](../en/TheOne.Avalonia.Workflow.WorkflowNodeEditor.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeEditor.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowNodeEditor` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[Inherited additional members: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

Undo-aware presentation for the default inspector; usable in custom templates.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L6)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Description`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L12) | `string` | writable; CLR / n/a | `accessor-defined / state` | Descriptive text supplied by the host; omitted values keep their initial state. |
| [`Document`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L8) | `TheOne.Avalonia.Workflow.WorkflowDocument` | read-only; CLR / n/a | `accessor-defined / state` | Current business document model; editor documentation defines subscription, selection and history behavior on replacement. |
| [`Fields`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L10) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowPropertyEditor>` | read-only; CLR / n/a | `[]` | Output-field metadata exposed by a port to describe referenceable data. |
| [`Node`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L9) | `TheOne.Avalonia.Workflow.WorkflowNode` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L11) | `string` | writable; CLR / n/a | `accessor-defined / state` | Display title, distinct from business identity or a localization key. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void WorkflowNodeEditor.Refresh()`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L20) | Refresh after document edits, including undo and upstream metadata changes. |
| [`WorkflowNodeEditor.WorkflowNodeEditor(WorkflowDocument document, WorkflowNode node)`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L14) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowDocument`](TheOne.Avalonia.Workflow.WorkflowDocument.md)
- [`TheOne.Avalonia.Workflow.WorkflowNode`](TheOne.Avalonia.Workflow.WorkflowNode.md)
- [`TheOne.Avalonia.Workflow.WorkflowPropertyEditor`](TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md)
