# WorkflowPropertyEditor

[English](../en/TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowPropertyEditor` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[Inherited additional members: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

A literal/reference editor backed by the same document undo history.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L33)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsLiteral`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L41) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Literal`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L40) | `string` | writable; CLR / n/a | `accessor-defined / state` | Literal text of a workflow property, edited through the document to participate in undo history. |
| [`Options`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L42) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowReferenceOption>` | read-only; CLR / n/a | `[]` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`Property`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L37) | `TheOne.Avalonia.Workflow.WorkflowProperty` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectedOption`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L43) | `TheOne.Avalonia.Workflow.WorkflowReferenceOption?` | writable; CLR / n/a | `accessor-defined / state` | Selected literal/upstream-reference option; switching updates Reference through document history. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L38) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Display title, distinct from business identity or a localization key. |
| [`TypeHint`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L39) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void WorkflowPropertyEditor.Refresh()`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L57) | Resynchronizes the inspector from its model/document, including undo and upstream changes. |
| [`WorkflowPropertyEditor.WorkflowPropertyEditor(WorkflowDocument document, WorkflowNode node, WorkflowProperty property)`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L53) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowDocument`](TheOne.Avalonia.Workflow.WorkflowDocument.md)
- [`TheOne.Avalonia.Workflow.WorkflowNode`](TheOne.Avalonia.Workflow.WorkflowNode.md)
- [`TheOne.Avalonia.Workflow.WorkflowProperty`](TheOne.Avalonia.Workflow.WorkflowProperty.md)
- [`TheOne.Avalonia.Workflow.WorkflowReferenceOption`](TheOne.Avalonia.Workflow.WorkflowReferenceOption.md)
