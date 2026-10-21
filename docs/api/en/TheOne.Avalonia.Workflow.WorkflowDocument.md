# WorkflowDocument

[English](../en/TheOne.Avalonia.Workflow.WorkflowDocument.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowDocument` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[Inherited additional members: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

Observable UI-thread document. Editor operations are reversible; direct host changes reset history.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowDocument.Index.cs#L3)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanRedo`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L45) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanUndo`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L44) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Connections`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L43) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowConnection>` | read-only; CLR / n/a | `[]` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`Nodes`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L42) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowNode>` | read-only; CLR / n/a | `[]` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`Revision`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L46) | `long` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L33) | `string` | writable; CLR / n/a | `"新建流程"` | Display title, distinct from business identity or a localization key. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void WorkflowDocument.Edit(string title, Action redo, Action undo)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L114) | Apply a host edit to the same history as graph edits. Actions must be deterministic and not throw. |
| [`void WorkflowDocument.Undo()`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L133) | Undoes the most recent undoable document edit. |
| [`void WorkflowDocument.Redo()`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L152) | Redoes the most recently undone document edit. |
| [`void WorkflowDocument.ClearHistory()`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L171) | Clears undo/redo history while retaining current document data. |
| [`void WorkflowDocument.AddNode(WorkflowNode node)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L179) | Adds a workflow node through document history so the edit can be undone. |
| [`void WorkflowDocument.Remove(IEnumerable<WorkflowNode> nodes, WorkflowConnection? connection = null)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L187) | Removes a workflow object/associated relationships through document history. |
| [`void WorkflowDocument.MoveNodes(IEnumerable<(WorkflowNode Node, double X, double Y)> positions)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L207) | Updates node coordinates in a batch recorded as an undoable move. |
| [`void WorkflowDocument.SetProperty(WorkflowProperty property, string value, WorkflowDataReference? reference)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L231) | Edits workflow literal/reference properties through the document to preserve undo/notifications. |
| [`void WorkflowDocument.SetConnectionProperty(WorkflowConnection connection, WorkflowProperty property, string value, WorkflowDataReference? reference = null)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L248) | 修改本图中指定连线的固定值或引用，保留原字段的撤销记录；不允许编辑其他文档的字段。 |
| [`void WorkflowDocument.SetSelectedKeys(WorkflowProperty property, IEnumerable<string> keys)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L259) | One undoable multi-selection edit, preserving stable keys and their order. |
| [`string? WorkflowDocument.CheckConnection(string sourceNode, string sourcePort, string targetNode, string targetPort, bool allowCycles = false)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L278) | Validates a candidate port connection and reports failures without creating it. |
| [`bool WorkflowDocument.TryConnect(string sourceNode, string sourcePort, string targetNode, string targetPort, out string? error, bool allowCycles = false)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L302) | Validates and attempts a connection, returning success and associated results. |
| [`bool WorkflowDocument.TryConnect(WorkflowConnection connection, out string? error, bool allowCycles = false)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L313) | Adds a host-created connection in one undoable edit after validating identity and topology. |
| [`bool WorkflowDocument.IsReachable(string from, string to)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L326) | Checks graph reachability for connection/cycle decisions. |
| [`IReadOnlyList<WorkflowReferenceOption> WorkflowDocument.GetReferenceOptions(WorkflowNode target, WorkflowProperty property)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L342) | Lists compatible upstream references for the current node property. |
| [`IReadOnlyList<WorkflowValidationIssue> WorkflowDocument.Validate(bool allowCycles = false)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L355) | Validates workflow structure, ports and references, returning locatable issues. |
| [`WorkflowDocument.WorkflowDocument()`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L17) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler? WorkflowDocument.Changed`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L47) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowConnection`](TheOne.Avalonia.Workflow.WorkflowConnection.md)
- [`TheOne.Avalonia.Workflow.WorkflowDataReference`](TheOne.Avalonia.Workflow.WorkflowDataReference.md)
- [`TheOne.Avalonia.Workflow.WorkflowNode`](TheOne.Avalonia.Workflow.WorkflowNode.md)
- [`TheOne.Avalonia.Workflow.WorkflowProperty`](TheOne.Avalonia.Workflow.WorkflowProperty.md)
- [`TheOne.Avalonia.Workflow.WorkflowReferenceOption`](TheOne.Avalonia.Workflow.WorkflowReferenceOption.md)
- [`TheOne.Avalonia.Workflow.WorkflowValidationIssue`](TheOne.Avalonia.Workflow.WorkflowValidationIssue.md)
