# WorkflowEditor

[English](../en/TheOne.Avalonia.Controls.WorkflowEditor.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) · [API index](README.md)

`TheOne.Avalonia.Controls.WorkflowEditor` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

Template-based graph editing. Persistence, execution and business validation belong to the host.

[Source / 声明](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L18)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`AddNodeCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L344) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`AlignCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L353) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`AllowCycles`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L312) | `bool` | writable; OneWay | `false` | Allows cyclic connections; otherwise editor validation rejects cycles. Runners still own business-loop semantics. |
| [`ArrangeCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L352) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ChoiceItemAdapter`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L39) | `TheOne.Avalonia.Workflow.WorkflowChoiceAdapter?` | writable; OneWay | `null` | Typed key and label projections from WorkflowChoiceAdapter.Create<T>, without reflection member paths. When omitted, records must be WorkflowChoice instances. |
| [`ChoiceItemsSource`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L33) | `System.Collections.IEnumerable?` | writable; OneWay | `null` | Default choice catalog for multi-select fields; supports two-way binding and collection notifications. ChoiceItemAdapter supplies stable keys and labels; the host loads people or roles. |
| [`ConnectionValidator`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L359) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, string?>?` | writable; CLR / n/a | `null` | Host can reject business-specific connections in addition to structural checks. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L231) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`DeleteCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L347) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`Document`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L201) | `TheOne.Avalonia.Workflow.WorkflowDocument?` | writable; TwoWay | `null` | Current business document model; editor documentation defines subscription, selection and history behavior on replacement. |
| [`FitCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L348) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`FitPadding`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L336) | `Avalonia.Thickness` | writable; OneWay | `new Thickness(32)` | DIP padding reserved when fitting content to the viewport. Validation: `p => new[] { p.Left, p.Top, p.Right, p.Bottom }.All(x => double.IsFinite(x) && x >= 0)` |
| [`GraphSummary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L291) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HeaderTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L250) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Header content receives this editor. The default exposes document title, validation and SaveCommand. |
| [`IsPanMode`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L271) | `bool` | writable; OneWay | `false` | Uses pointer dragging to pan the canvas, following workflow selection/edit gesture rules. |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L294) | `bool` | writable; OneWay | `false` | Whether editing is blocked; the control's read-only contract determines remaining display, selection and navigation behavior. |
| [`LibraryGroups`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L289) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Workflow.WorkflowNodeGroup>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`LocateIssueCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L356) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`NodeDefinitionTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L243) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`NodeDefinitions`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L207) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Workflow.WorkflowNodeDefinition>?` | writable; OneWay | `null` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`NodeTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L225) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`Pan`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L330) | `Avalonia.Vector` | writable; TwoWay | `new Vector(24, 24)` | Canvas pan vector, affecting the viewport without rewriting node coordinates. Validation: `x => double.IsFinite(x.X) && double.IsFinite(x.Y)` |
| [`PropertyTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L237) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`RedoCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L346) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`SaveCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L257) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Host-owned persistence command. The default save button passes Document as its parameter. |
| [`SearchText`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L265) | `string` | writable; OneWay | `""` | Current search text, independent of selection and model identity. |
| [`SelectedConnection`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L219) | `TheOne.Avalonia.Workflow.WorkflowConnection?` | writable; TwoWay | `null` | Selected connection model, following the editor's node/connection selection rules. |
| [`SelectedNode`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L213) | `TheOne.Avalonia.Workflow.WorkflowNode?` | writable; TwoWay | `null` | Selected node model; null clears node selection. |
| [`SelectedNodeEditor`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L263) | `TheOne.Avalonia.Workflow.WorkflowNodeEditor?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectedNodes`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L199) | `System.Collections.ObjectModel.ReadOnlyObservableCollection<TheOne.Avalonia.Workflow.WorkflowNode>` | read-only; CLR / n/a | `accessor-defined / state` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`ShowNodeLibrary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L300) | `bool` | writable; OneWay | `true` | Shows the library panel of node definitions available to add. |
| [`ShowProperties`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L306) | `bool` | writable; OneWay | `true` | Shows the selected object's property editor panel. |
| [`ShowRuntimePreview`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L277) | `bool` | writable; OneWay | `false` | Shows runtime preview; the host runner owns actual business execution. |
| [`ShowValidation`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L283) | `bool` | writable; OneWay | `false` | Shows document validation results; hiding the panel does not bypass validation. |
| [`SnapToGrid`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L318) | `bool` | writable; OneWay | `true` | Snaps moved nodes to the grid; existing coordinates remain until edited. |
| [`Status`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L342) | `string` | read-only; OneWay | `computed / state` | Business node-state enum; styles derive from state rather than storing state in classes. |
| [`ToggleLibraryCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L354) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`TogglePropertiesCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L355) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`UndoCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L345) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ValidateCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L351) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ValidationIssues`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L290) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Workflow.WorkflowValidationIssue>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ValidationSummary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L292) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Zoom`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L324) | `double` | writable; TwoWay | `1` | View scale factor, where 1 means 100%; does not modify report millimetres or workflow coordinates. Validation: `x => double.IsFinite(x) && x >= .25 && x <= 2` |
| [`ZoomInCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L349) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ZoomOutCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L350) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ZoomText`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L343) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowNode? WorkflowEditor.AddNode(WorkflowNodeDefinition definition, Point position)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L571) | Adds a workflow node through document history so the edit can be undone. |
| [`void WorkflowEditor.SelectNode(WorkflowNode? node, bool additive = false)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L590) | Selects an original workflow node under the editor's connection-selection rules. |
| [`bool WorkflowEditor.Connect(string sourceNode, string sourcePort, string targetNode, string targetPort)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L612) | Attempts a port connection using document validation/history. |
| [`void WorkflowEditor.DeleteSelection()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L635) | Deletes selected nodes/connections through undo history. |
| [`void WorkflowEditor.FitToView()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L642) | Adjusts zoom/pan to fit document nodes without changing model coordinates. |
| [`void WorkflowEditor.Locate(string? nodeId)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L656) | Locates a node/validation issue in the current viewport. |
| [`void WorkflowEditor.ArrangeNodes()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L664) | Arranges node layout while recording coordinate changes in document history. |
| [`void WorkflowEditor.CopySelection()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L680) | Copies the selection into a supported editor fragment under serialization boundaries. |
| [`void WorkflowEditor.PasteSelection()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L695) | Pastes a copied fragment with independent identities and undoable edits. |
| [`WorkflowEditor.WorkflowEditor()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L150) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler? WorkflowEditor.DocumentEdited`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L364) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<WorkflowDocument?> WorkflowEditor.DocumentProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IEnumerable<WorkflowNodeDefinition>?> WorkflowEditor.NodeDefinitionsProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IEnumerable?> WorkflowEditor.ChoiceItemsSourceProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<WorkflowChoiceAdapter?> WorkflowEditor.ChoiceItemAdapterProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<WorkflowNode?> WorkflowEditor.SelectedNodeProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L45) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<WorkflowConnection?> WorkflowEditor.SelectedConnectionProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L49) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.NodeTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L53) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L56) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.PropertyTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L59) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.NodeDefinitionTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L62) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.HeaderTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L65) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ICommand?> WorkflowEditor.SaveCommandProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L68) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, WorkflowNodeEditor?> WorkflowEditor.SelectedNodeEditorProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L71) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> WorkflowEditor.SearchTextProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L75) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.IsPanModeProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L78) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowRuntimePreviewProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L81) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowValidationProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L84) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, IReadOnlyList<WorkflowNodeGroup>> WorkflowEditor.LibraryGroupsProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L87) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.GraphSummaryProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L91) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.ValidationSummaryProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L94) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, IReadOnlyList<WorkflowValidationIssue>> WorkflowEditor.ValidationIssuesProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L98) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.IsReadOnlyProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L102) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowNodeLibraryProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L105) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowPropertiesProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L108) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.AllowCyclesProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L111) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.SnapToGridProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L114) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> WorkflowEditor.ZoomProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L117) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Vector> WorkflowEditor.PanProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L121) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Thickness> WorkflowEditor.FitPaddingProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L125) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.StatusProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L129) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.ZoomTextProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L132) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowChoiceAdapter`](TheOne.Avalonia.Workflow.WorkflowChoiceAdapter.md)
- [`TheOne.Avalonia.Workflow.WorkflowConnection`](TheOne.Avalonia.Workflow.WorkflowConnection.md)
- [`TheOne.Avalonia.Workflow.WorkflowDocument`](TheOne.Avalonia.Workflow.WorkflowDocument.md)
- [`TheOne.Avalonia.Workflow.WorkflowNode`](TheOne.Avalonia.Workflow.WorkflowNode.md)
- [`TheOne.Avalonia.Workflow.WorkflowNodeDefinition`](TheOne.Avalonia.Workflow.WorkflowNodeDefinition.md)
- [`TheOne.Avalonia.Workflow.WorkflowNodeEditor`](TheOne.Avalonia.Workflow.WorkflowNodeEditor.md)
- [`TheOne.Avalonia.Workflow.WorkflowNodeGroup`](TheOne.Avalonia.Workflow.WorkflowNodeGroup.md)
- [`TheOne.Avalonia.Workflow.WorkflowPort`](TheOne.Avalonia.Workflow.WorkflowPort.md)
- [`TheOne.Avalonia.Workflow.WorkflowValidationIssue`](TheOne.Avalonia.Workflow.WorkflowValidationIssue.md)
