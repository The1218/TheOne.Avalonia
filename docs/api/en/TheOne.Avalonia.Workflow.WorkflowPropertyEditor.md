# WorkflowPropertyEditor

[English](../en/TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowPropertyEditor` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[Inherited additional members: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

A literal/reference editor backed by the same document undo history.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L58)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ChoiceError`](../../../TheOne.Avalonia/Workflow/WorkflowPropertyEditor.Choices.cs#L22) | `string` | read-only; CLR / n/a | `""` | Error text for empty or duplicate catalog keys or incompatible records; persisted selections are retained. |
| [`Choices`](../../../TheOne.Avalonia/Workflow/WorkflowPropertyEditor.Choices.cs#L21) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Workflow.WorkflowChoice>` | read-only; CLR / n/a | `[]` | Current choice projections identified by key, for display only; Property.SelectedKeys owns persisted selection. |
| [`HasMissingChoices`](../../../TheOne.Avalonia/Workflow/WorkflowPropertyEditor.Choices.cs#L23) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | True when persisted keys are temporarily absent from the catalog; removable tags remain and reconnect when records return. |
| [`IsLiteral`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L84) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsMultiSelect`](../../../TheOne.Avalonia/Workflow/WorkflowPropertyEditor.Choices.cs#L19) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsSingleValue`](../../../TheOne.Avalonia/Workflow/WorkflowPropertyEditor.Choices.cs#L20) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Literal`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L75) | `string` | writable; CLR / n/a | `accessor-defined / state` | Literal text of a workflow property, edited through the document to participate in undo history. |
| [`Options`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L85) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowReferenceOption>` | read-only; CLR / n/a | `[]` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`Property`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L71) | `TheOne.Avalonia.Workflow.WorkflowProperty` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectedChoices`](../../../TheOne.Avalonia/Workflow/WorkflowPropertyEditor.Choices.cs#L26) | `System.Collections.IList` | writable; CLR / n/a | `new ObservableCollection<WorkflowChoice>()` | Two-way item selection. Persisted state belongs to Property.SelectedKeys. |
| [`SelectedOption`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L87) | `TheOne.Avalonia.Workflow.WorkflowReferenceOption?` | writable; CLR / n/a | `accessor-defined / state` | Selected literal/upstream-reference option; switching updates Reference through document history. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L72) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Display title, distinct from business identity or a localization key. |
| [`TypeHint`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L73) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void WorkflowPropertyEditor.Refresh()`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L98) | Resynchronizes the inspector from its model/document, including undo and upstream changes. |
| [`void WorkflowPropertyEditor.ConfigureChoices(IEnumerable? source, WorkflowChoiceAdapter? adapter)`](../../../TheOne.Avalonia/Workflow/WorkflowPropertyEditor.Choices.cs#L44) | Configure host records with typed key/text projections. Collection and label notifications are observed. |
| [`void WorkflowPropertyEditor.Dispose()`](../../../TheOne.Avalonia/Workflow/WorkflowPropertyEditor.Choices.cs#L152) | Detaches subscriptions and releases owned resources; do not reuse it as an active view afterwards. |
| [`WorkflowPropertyEditor.WorkflowPropertyEditor(WorkflowDocument document, WorkflowNode node, WorkflowProperty property)`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L63) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowChoice`](TheOne.Avalonia.Workflow.WorkflowChoice.md)
- [`TheOne.Avalonia.Workflow.WorkflowChoiceAdapter`](TheOne.Avalonia.Workflow.WorkflowChoiceAdapter.md)
- [`TheOne.Avalonia.Workflow.WorkflowDocument`](TheOne.Avalonia.Workflow.WorkflowDocument.md)
- [`TheOne.Avalonia.Workflow.WorkflowNode`](TheOne.Avalonia.Workflow.WorkflowNode.md)
- [`TheOne.Avalonia.Workflow.WorkflowProperty`](TheOne.Avalonia.Workflow.WorkflowProperty.md)
- [`TheOne.Avalonia.Workflow.WorkflowReferenceOption`](TheOne.Avalonia.Workflow.WorkflowReferenceOption.md)
