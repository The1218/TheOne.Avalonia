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

`WorkflowCanvas` and `WorkflowMiniMap` are drawing/interaction helpers in the WorkflowEditor template. Integrate business pages through WorkflowEditor and the extension points below, rather than treating these helpers as standalone editors. When replacing the full template, retain `PART_Canvas` and bind the minimap's `Editor` to the current WorkflowEditor; consult the same-version [WorkflowEditor.axaml](../../../TheOne.Avalonia/Themes/Controls/WorkflowEditor.axaml).

- The control includes its shell, header, node library, canvas, inspector, toolbar and minimap. No host templates are required.
- HeaderTemplate receives WorkflowEditor; the default displays document title, validation and save. Set it to null to hide the header or supply an IDataTemplate to replace it.
- NodeTemplate receives WorkflowNode; the default displays title, description and state. Ports and selection chrome remain outside it.
- NodeDefinitionTemplate receives WorkflowNodeDefinition; the default displays title and description in the node library.
- ContentTemplate receives typed host Content. Declare the concrete x:DataType. Content is not persisted; the host restores it by TypeId after loading.
- PropertyTemplate receives the selected node. The default uses native TabStrip tabs for basic metadata, data and ports; it edits name, description, literals and upstream references through document undo history, and respects IsReadOnly. SelectedNodeEditor exposes WorkflowNodeEditor and WorkflowPropertyEditor adapters for custom templates. A host can supply specialized business UI or hide the panel and bind SelectedNode externally. Templates own their scroll container; delete stays outside the default scrolling region.
- SelectedConnection exposes the selected edge for external label or business-condition editing. The default inspector displays nodes.

Node and connection selection are exclusive, including external `SelectedNode` / `SelectedConnection` assignments. Selecting a connection clears all selected nodes and the node inspector; selecting a node clears the selected connection. Models outside the current document normalize to null, even when they share an ID with a document model.

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

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost keeps complete demos on their own pages, with styles, additional properties, APIs/events and models on each page. Only Button, Input and Select share comparison pages.

### WorkflowCanvas

[API / model reference](../../api/en/TheOne.Avalonia.Controls.WorkflowCanvas.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### WorkflowEditor

[API / model reference](../../api/en/TheOne.Avalonia.Controls.WorkflowEditor.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`AddNodeCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L95) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`AlignCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L104) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`AllowCycles`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L88) | `bool` | writable; OneWay | `false` | Allows cyclic connections; otherwise editor validation rejects cycles. Runners still own business-loop semantics. |
| [`ArrangeCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L103) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ConnectionValidator`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L109) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, string?>?` | writable; CLR / n/a | `null` | Host can reject business-specific connections in addition to structural checks. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L69) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`DeleteCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L98) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`Document`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L64) | `TheOne.Avalonia.Workflow.WorkflowDocument?` | writable; TwoWay | `null` | Current business document model; editor documentation defines subscription, selection and history behavior on replacement. |
| [`FitCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L99) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`FitPadding`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L92) | `Avalonia.Thickness` | writable; OneWay | `new Thickness(32)` | DIP padding reserved when fitting content to the viewport. Validation: `p => new[] { p.Left, p.Top, p.Right, p.Bottom }.All(x => double.IsFinite(x) && x >= 0)` |
| [`GraphSummary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L83) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HeaderTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L73) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Header content receives this editor. The default exposes document title, validation and SaveCommand. |
| [`IsPanMode`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L78) | `bool` | writable; OneWay | `false` | Uses pointer dragging to pan the canvas, following workflow selection/edit gesture rules. |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L85) | `bool` | writable; OneWay | `false` | Whether editing is blocked; the control's read-only contract determines remaining display, selection and navigation behavior. |
| [`LibraryGroups`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L81) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Workflow.WorkflowNodeGroup>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`LocateIssueCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L107) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`NodeDefinitionTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L71) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`NodeDefinitions`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L65) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Workflow.WorkflowNodeDefinition>?` | writable; OneWay | `null` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`NodeTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L68) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`Pan`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L91) | `Avalonia.Vector` | writable; TwoWay | `new Vector(24, 24)` | Canvas pan vector, affecting the viewport without rewriting node coordinates. Validation: `x => double.IsFinite(x.X) && double.IsFinite(x.Y)` |
| [`PropertyTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L70) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`RedoCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L97) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`SaveCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L75) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Host-owned persistence command. The default save button passes Document as its parameter. |
| [`SearchText`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L77) | `string` | writable; OneWay | `""` | Current search text, independent of selection and model identity. |
| [`SelectedConnection`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L67) | `TheOne.Avalonia.Workflow.WorkflowConnection?` | writable; TwoWay | `null` | Selected connection model, following the editor's node/connection selection rules. |
| [`SelectedNode`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L66) | `TheOne.Avalonia.Workflow.WorkflowNode?` | writable; TwoWay | `null` | Selected node model; null clears node selection. |
| [`SelectedNodeEditor`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L76) | `TheOne.Avalonia.Workflow.WorkflowNodeEditor?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectedNodes`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L63) | `System.Collections.ObjectModel.ReadOnlyObservableCollection<TheOne.Avalonia.Workflow.WorkflowNode>` | read-only; CLR / n/a | `accessor-defined / state` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`ShowNodeLibrary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L86) | `bool` | writable; OneWay | `true` | Shows the library panel of node definitions available to add. |
| [`ShowProperties`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L87) | `bool` | writable; OneWay | `true` | Shows the selected object's property editor panel. |
| [`ShowRuntimePreview`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L79) | `bool` | writable; OneWay | `false` | Shows runtime preview; the host runner owns actual business execution. |
| [`ShowValidation`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L80) | `bool` | writable; OneWay | `false` | Shows document validation results; hiding the panel does not bypass validation. |
| [`SnapToGrid`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L89) | `bool` | writable; OneWay | `true` | Snaps moved nodes to the grid; existing coordinates remain until edited. |
| [`Status`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L93) | `string` | read-only; OneWay | `computed / state` | Business node-state enum; styles derive from state rather than storing state in classes. |
| [`ToggleLibraryCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L105) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`TogglePropertiesCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L106) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`UndoCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L96) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ValidateCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L102) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ValidationIssues`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L82) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Workflow.WorkflowValidationIssue>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ValidationSummary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L84) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Zoom`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L90) | `double` | writable; TwoWay | `1` | View scale factor, where 1 means 100%; does not modify report millimetres or workflow coordinates. Validation: `x => double.IsFinite(x) && x >= .25 && x <= 2` |
| [`ZoomInCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L100) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ZoomOutCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L101) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ZoomText`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L94) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### WorkflowMiniMap

[API / model reference](../../api/en/TheOne.Avalonia.Controls.WorkflowMiniMap.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Editor`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L13) | `TheOne.Avalonia.Controls.WorkflowEditor?` | writable; OneWay | `null` | WorkflowEditor associated with the minimap, sharing document/viewport rather than creating independent workflow state. |

### Shared attached properties

[ControlSizing](../../api/en/TheOne.Avalonia.Styling.ControlSizing.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | writable; OneWay; attached; inherits | `ControlSize.Normal` | Shared Small/Normal/Large density inherited by descendants; explicit local values/classes can override it. Values: `Small`, `Normal`, `Large` |

[Motion](../../api/en/TheOne.Avalonia.Styling.Motion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | writable; OneWay; attached; inherits | `true` | Inherited motion switch; disabling goes to final layout/state while preserving native input. |

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| WorkflowEditor | `one-large` | automatic template class; do not bind manually | ^.one-large => FontSize=16 |
| WorkflowEditor | `one-small` | automatic template class; do not bind manually | ^.one-small => FontSize=12 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.WorkflowCanvas`](../../api/en/TheOne.Avalonia.Controls.WorkflowCanvas.md) | Class | 0 | 4 | 0 |
| [`TheOne.Avalonia.Controls.WorkflowEditor`](../../api/en/TheOne.Avalonia.Controls.WorkflowEditor.md) | Class | 44 | 10 | 1 |
| [`TheOne.Avalonia.Controls.WorkflowMiniMap`](../../api/en/TheOne.Avalonia.Controls.WorkflowMiniMap.md) | Class | 1 | 2 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowConnection`](../../api/en/TheOne.Avalonia.Workflow.WorkflowConnection.md) | Class | 6 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowDataReference`](../../api/en/TheOne.Avalonia.Workflow.WorkflowDataReference.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowDocument`](../../api/en/TheOne.Avalonia.Workflow.WorkflowDocument.md) | Class | 6 | 14 | 1 |
| [`TheOne.Avalonia.Workflow.WorkflowNode`](../../api/en/TheOne.Avalonia.Workflow.WorkflowNode.md) | Class | 14 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowNodeDefinition`](../../api/en/TheOne.Avalonia.Workflow.WorkflowNodeDefinition.md) | Class | 6 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowNodeEditor`](../../api/en/TheOne.Avalonia.Workflow.WorkflowNodeEditor.md) | Class | 5 | 2 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowNodeGroup`](../../api/en/TheOne.Avalonia.Workflow.WorkflowNodeGroup.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowNodeShape`](../../api/en/TheOne.Avalonia.Workflow.WorkflowNodeShape.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowNodeState`](../../api/en/TheOne.Avalonia.Workflow.WorkflowNodeState.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowObservable`](../../api/en/TheOne.Avalonia.Workflow.WorkflowObservable.md) | Class | 0 | 0 | 1 |
| [`TheOne.Avalonia.Workflow.WorkflowOutputField`](../../api/en/TheOne.Avalonia.Workflow.WorkflowOutputField.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowPort`](../../api/en/TheOne.Avalonia.Workflow.WorkflowPort.md) | Class | 6 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowPortDirection`](../../api/en/TheOne.Avalonia.Workflow.WorkflowPortDirection.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowProperty`](../../api/en/TheOne.Avalonia.Workflow.WorkflowProperty.md) | Class | 8 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowPropertyEditor`](../../api/en/TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md) | Class | 7 | 2 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowReferenceOption`](../../api/en/TheOne.Avalonia.Workflow.WorkflowReferenceOption.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowSerializer`](../../api/en/TheOne.Avalonia.Workflow.WorkflowSerializer.md) | Class | 0 | 2 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowValidationIssue`](../../api/en/TheOne.Avalonia.Workflow.WorkflowValidationIssue.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Workflow.WorkflowValueKind`](../../api/en/TheOne.Avalonia.Workflow.WorkflowValueKind.md) | Enum | 3 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
