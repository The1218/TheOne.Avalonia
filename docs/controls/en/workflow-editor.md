# WorkflowEditor

[简体中文](../zh-cn/workflow-editor.md) · [Index](README.md)

WorkflowEditor edits a bound WorkflowDocument: nodes, named ports, edges and business data references. It includes default templates; the host supplies factories, data, persistence and execution, and can replace individual templates. There is no built-in approval, HTTP or script engine.

## Contracts

Models live in TheOne.Avalonia.Workflow, independently of visual controls.

| Type | Responsibility |
| --- | --- |
| WorkflowDocument | Observable Nodes / Connections, title, revision and reversible edits |
| WorkflowNodeDefinition | Registered TypeId, title, description, category and CreateNode factory |
| WorkflowNode | Stable Id / TypeId, title, description, logical coordinates, width / shape, annotation flag, ports, properties, state and presentation Content |
| WorkflowPort | Node-local ID, direction, DataType, maximum connections; nonpositive means unlimited |
| WorkflowConnection | Stable edge ID, source/target node and port IDs, label |
| WorkflowProperty | Key, title, DataType, Text/Number/Boolean kind, required flag, string literal or reference |
| WorkflowDataReference | NodeId + OutputPortId + FieldPath, independent of display titles |
| WorkflowOutputField | Host-declared field path, title and data type |

Number/Boolean literals are stored as strings and validated using invariant numeric/boolean semantics. DataType is a host token such as email, never a reflected CLR type. Treat port/output records as immutable; replace Ports entries when changing metadata. Renaming and sorting do not change IDs.

## Consumer binding

```xml
<Grid>
<one:WorkflowEditor Document="{Binding Document, Mode=TwoWay}"
                    NodeDefinitions="{Binding NodeDefinitions}"
                    SelectedNode="{Binding SelectedNode, Mode=TwoWay}"
                    SaveCommand="{Binding SaveCommand}" />
</Grid>
```

Use concrete notifying ViewModels and correct x:DataType declarations for compiled binding. A single document keeps nodes and edges consistent during replacement. The editor stretches to a finite parent by default, without a fixed width/height or minimum height. Use a Grid or DockPanel viewport; an unconstrained StackPanel or scrolling parent cannot provide a height to fill.

```csharp
var document = new WorkflowDocument();
var source = new WorkflowNode { TypeId = "request", Title = "Submit" };
source.Ports.Add(new("out", "Request", WorkflowPortDirection.Output,
    "flow", 0, [new("applicant.email", "Applicant email", "email")]));
var target = new WorkflowNode { TypeId = "notification", Title = "Notify", X = 300 };
target.Ports.Add(new("in", "Input", WorkflowPortDirection.Input));
target.Properties.Add(new() {
    Key = "recipient", Title = "Recipient", DataType = "email", IsRequired = true,
    Reference = new(source.Id, "out", "applicant.email")
});
document.AddNode(source);
document.AddNode(target);
document.TryConnect(source.Id, "out", target.Id, "in", out var error);
```

The host registers NodeDefinitions. Each factory returns an independent instance with a fresh ID and matching TypeId. Search the category-grouped library, click/drag items, or call AddNode(definition, position). NodeDefinitionTemplate receives a definition for custom library icons and content. The demo supplies the approval types and icons as consumer data.

## Associate business data

Edges represent graph relationships, without automatically transmitting every output. GetReferenceOptions(node, property) filters declared fields by upstream reachability and matching DataType (any accepts all field types). The host resolves the stable triple against output tables or explicit compiled getters. FieldPath is not evaluated as an expression or reflection path.

The demo's reference-resolution action uses a host dictionary to provide request amount and applicant email to approval amount and notification recipient. Resolving again reads current source values.

## Templates

- The control includes its shell, header, node library, canvas, inspector, toolbar and minimap. No host templates are required.
- HeaderTemplate receives WorkflowEditor; the default displays document title, validation and save. Set it to null to hide the header or supply an IDataTemplate to replace it.
- NodeTemplate receives WorkflowNode; the default displays title, description and state. Ports and selection chrome remain outside it.
- NodeDefinitionTemplate receives WorkflowNodeDefinition; the default displays title and description in the node library.
- ContentTemplate receives typed host Content. Declare the concrete x:DataType. Content is not persisted; the host restores it by TypeId after loading.
- PropertyTemplate receives the selected node. The default uses native TabStrip tabs for basic metadata, data and ports; it edits name, description, literals and upstream references through document undo history, and respects IsReadOnly. SelectedNodeEditor exposes WorkflowNodeEditor and WorkflowPropertyEditor adapters for custom templates. A host can supply specialized business UI or hide the panel and bind SelectedNode externally. Templates own their scroll container; delete stays outside the default scrolling region.
- SelectedConnection exposes the selected edge for external label or business-condition editing. The default inspector displays nodes.

The demo supplies typed field rows and literal/reference ComboBoxes. SetProperty and document Edit integrate host edits with the same graph undo history. The host rebuilds Content by TypeId after loading or copying rather than persisting presentation objects or sharing mutable business objects with the original node.

## Editing and validation

Update models and controls on the UI thread. Collection/property notifications refresh views; replacing Document detaches old subscriptions and clears selection. Direct host data changes reset stale history. For reversible changes use Edit(title, redo, undo); callbacks must be deterministic and must not throw. Presentation Content and runtime State updates do not change persistence revision or discard history.

Supports dragging, snapping, box/Shift selection, arrow nudges (Shift=10), Delete/Backspace, Ctrl/Cmd+Z / Shift+Z and editor-local Ctrl/Cmd+C/V/D. Copies generate fresh node/edge IDs, remap internal references and retain external references for validation. The clipboard is local to the editor instance.

Drag output to input or use Tab/Enter on native port buttons. Escape cancels. Space+drag or middle-drag pans; pointer-centered wheel zoom ranges from 25% to 200%. Native text controls keep their editing shortcuts.

IsPanMode selects the canvas tool; alignment moves selected nodes to a common top coordinate. Click/drag the minimap to navigate. FitPadding reserves space when fitting, including room for the demo runtime badge. Validation opens a list with clickable issue navigation. IsAnnotation nodes are excluded from the execution-node count.

IsReadOnly blocks editor writes while permitting selection and viewport interaction. Hosts must disable custom inspector writes, as demonstrated; host model updates remain allowed. ShowNodeLibrary / ShowProperties let the host collapse panels for narrow views.

Connections reject missing endpoints, wrong direction/type, duplicates, self-edges, capacity overflow and cycles. AllowCycles explicitly permits cycles; ConnectionValidator adds host constraints. Validate(allowCycles) checks structure, properties and references. Domain validation and permissions belong to the host; no specific business start/end node types are mandatory.

## Persistence

`SaveCommand` is a bindable host `ICommand`. The default save button passes the current `Document` and respects `CanExecute` / `CanExecuteChanged`; it is disabled when no command is supplied. The control never opens a picker or writes a file itself. The PreviewHost ViewModel uses an awaitable command and an injected `IWorkflowEditorFileService`; export and open are commands too. Canceled/failed saves do not mark the document saved, and changes made while an older snapshot is being written remain dirty.

```csharp
string json = WorkflowSerializer.Save(document);
WorkflowDocument loaded = WorkflowSerializer.Load(json);
```

Version 1 JSON uses source-generated System.Text.Json metadata. Nodes, ports, fields, references, coordinates and edges persist; Content, templates, runtime state, services, selection, viewport and history do not. A host envelope DTO may persist additional data.

Unsupported versions or invalid identities/coordinates are rejected. Draft unresolved references/endpoints are retained for Validate to report. Parse a new document before replacing the bound one; the example preserves the current workflow on failure.

## Example and verification

PreviewHost: **Extensions → WorkflowEditor**. The primary view follows the approved seven-node design, with a note, icons, ports and states; inspector tabs use the built-in `TabStrip`. The library, toolbar, inspector and example actions default to Small sizing (body and input font size 12), while canvases retain host density and node proportions. The collapsible consumer section includes default/custom templates, typed Content, configuration, reference selection/resolution, read-only mode, editable JSON and native file save/open. A separate request → approval → notification example demonstrates recipient references.

```sh
dotnet build design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/workflow-editor/artifacts
dotnet .preview/workflow-editor/artifacts/bin/PreviewHost/debug/PreviewHost.dll --verify-workflow .preview/workflow-editor/render
```

NativeAotSmoke supports --verify-workflow. Checks cover contracts, JSON, compiled templates, actual pointer/keyboard input, light/dark, three densities and narrow views with panels folded. Native file dialogs require desktop verification; headless checks cover their shared JSON path.

Initial routing uses directly drawn rounded orthogonal edges and layered arrangement. Full obstacle routing, bend editing and node virtualization are not included. The host owns rule execution and output evaluation.
