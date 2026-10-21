# WorkflowEditor

[English](../en/TheOne.Avalonia.Controls.WorkflowEditor.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) · [API index](README.md)

`TheOne.Avalonia.Controls.WorkflowEditor` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

Template-based graph editing. Persistence, execution and business validation belong to the host.

[Source / 声明](../../../TheOne.Avalonia/Controls/WorkflowEditor.Panels.cs#L6)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`AddNodeCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L356) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`AlignCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L365) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`AllowCycles`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L324) | `bool` | writable; OneWay | `false` | Allows cyclic connections; otherwise editor validation rejects cycles. Runners still own business-loop semantics. |
| [`ArrangeCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L364) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ChoiceItemAdapter`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L41) | `TheOne.Avalonia.Workflow.WorkflowChoiceAdapter?` | writable; OneWay | `null` | Typed key and label projections from WorkflowChoiceAdapter.Create<T>, without reflection member paths. When omitted, records must be WorkflowChoice instances. |
| [`ChoiceItemsSource`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L35) | `System.Collections.IEnumerable?` | writable; OneWay | `null` | Default choice catalog for multi-select fields; supports two-way binding and collection notifications. ChoiceItemAdapter supplies stable keys and labels; the host loads people or roles. |
| [`ConnectionFactory`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L374) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, TheOne.Avalonia.Workflow.WorkflowConnection?>?` | writable; CLR / n/a | `null` | Creates connection metadata before its single history edit. Endpoints must match the requested ports; null rejects creation. |
| [`ConnectionPropertyTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L249) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Optional connection inspector; its data context is the original SelectedConnection. |
| [`ConnectionValidator`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L371) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, string?>?` | writable; CLR / n/a | `null` | Host can reject business-specific connections in addition to structural checks. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L236) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`DeleteCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L359) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`Document`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L206) | `TheOne.Avalonia.Workflow.WorkflowDocument?` | writable; TwoWay | `null` | Current business document model; editor documentation defines subscription, selection and history behavior on replacement. |
| [`FitCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L360) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`FitPadding`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L348) | `Avalonia.Thickness` | writable; OneWay | `new Thickness(32)` | DIP padding reserved when fitting content to the viewport. Validation: `p => new[] { p.Left, p.Top, p.Right, p.Bottom }.All(x => double.IsFinite(x) && x >= 0)` |
| [`GraphSummary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L303) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HeaderTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L262) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Header content receives this editor. The default exposes document title, validation and SaveCommand. |
| [`IsPanMode`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L283) | `bool` | writable; OneWay | `false` | Uses pointer dragging to pan the canvas, following workflow selection/edit gesture rules. |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L306) | `bool` | writable; OneWay | `false` | Whether editing is blocked; the control's read-only contract determines remaining display, selection and navigation behavior. |
| [`LibraryGroups`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L301) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Workflow.WorkflowNodeGroup>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`LocateIssueCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L368) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`NodeCreationValidator`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L377) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode, string?>?` | writable; CLR / n/a | `null` | Optional host policy for adding and pasting nodes. A non-null message rejects that candidate. |
| [`NodeDefinitionTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L255) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`NodeDefinitions`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L212) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Workflow.WorkflowNodeDefinition>?` | writable; OneWay | `null` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`NodeDeletionValidator`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L380) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode, string?>?` | writable; CLR / n/a | `null` | Optional host policy for deletion. Rejected nodes are retained while other selected nodes may be removed. |
| [`NodeTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L230) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`Pan`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L342) | `Avalonia.Vector` | writable; TwoWay | `new Vector(24, 24)` | Canvas pan vector, affecting the viewport without rewriting node coordinates. Validation: `x => double.IsFinite(x.X) && double.IsFinite(x.Y)` |
| [`PropertyTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L242) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`RedoCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L358) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`SaveCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L269) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Host-owned persistence command. The default save button passes Document as its parameter. |
| [`SearchText`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L277) | `string` | writable; OneWay | `""` | Current search text, independent of selection and model identity. |
| [`SelectedConnection`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L224) | `TheOne.Avalonia.Workflow.WorkflowConnection?` | writable; TwoWay | `null` | Selected connection model, following the editor's node/connection selection rules. |
| [`SelectedNode`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L218) | `TheOne.Avalonia.Workflow.WorkflowNode?` | writable; TwoWay | `null` | Selected node model; null clears node selection. |
| [`SelectedNodeEditor`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L275) | `TheOne.Avalonia.Workflow.WorkflowNodeEditor?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectedNodes`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L204) | `System.Collections.ObjectModel.ReadOnlyObservableCollection<TheOne.Avalonia.Workflow.WorkflowNode>` | read-only; CLR / n/a | `accessor-defined / state` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`ShowNodeLibrary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L312) | `bool` | writable; OneWay | `true` | Shows the library panel of node definitions available to add. |
| [`ShowProperties`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L318) | `bool` | writable; OneWay | `true` | Shows the selected object's property editor panel. |
| [`ShowRuntimePreview`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L289) | `bool` | writable; OneWay | `false` | Shows runtime preview; the host runner owns actual business execution. |
| [`ShowValidation`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L295) | `bool` | writable; OneWay | `false` | Shows document validation results; hiding the panel does not bypass validation. |
| [`SnapToGrid`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L330) | `bool` | writable; OneWay | `true` | Snaps moved nodes to the grid; existing coordinates remain until edited. |
| [`Status`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L354) | `string` | read-only; OneWay | `computed / state` | Business node-state enum; styles derive from state rather than storing state in classes. |
| [`ToggleLibraryCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L366) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`TogglePropertiesCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L367) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`UndoCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L357) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ValidateCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L363) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ValidationIssues`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L302) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Workflow.WorkflowValidationIssue>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ValidationSummary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L304) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Zoom`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L336) | `double` | writable; TwoWay | `1` | View scale factor, where 1 means 100%; does not modify report millimetres or workflow coordinates. Validation: `x => double.IsFinite(x) && x >= .25 && x <= 2` |
| [`ZoomInCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L361) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ZoomOutCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L362) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ZoomText`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L355) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowNode? WorkflowEditor.AddNode(WorkflowNodeDefinition definition, Point position)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L596) | Adds a workflow node through document history so the edit can be undone. |
| [`void WorkflowEditor.SelectNode(WorkflowNode? node, bool additive = false)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L620) | Selects an original workflow node under the editor's connection-selection rules. |
| [`bool WorkflowEditor.Connect(string sourceNode, string sourcePort, string targetNode, string targetPort)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L642) | Attempts a port connection using document validation/history. |
| [`void WorkflowEditor.DeleteSelection()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L680) | Deletes selected nodes/connections through undo history. |
| [`void WorkflowEditor.FitToView()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L695) | Adjusts zoom/pan to fit document nodes without changing model coordinates. |
| [`void WorkflowEditor.Locate(string? nodeId)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L709) | Locates a node/validation issue in the current viewport. |
| [`void WorkflowEditor.ArrangeNodes()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L717) | Arranges node layout while recording coordinate changes in document history. |
| [`void WorkflowEditor.CopySelection()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L733) | Copies the selection into a supported editor fragment under serialization boundaries. |
| [`void WorkflowEditor.PasteSelection()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L748) | Pastes a copied fragment with independent identities and undoable edits. |
| [`WorkflowEditor.WorkflowEditor()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L155) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler? WorkflowEditor.DocumentEdited`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L385) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<WorkflowDocument?> WorkflowEditor.DocumentProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IEnumerable<WorkflowNodeDefinition>?> WorkflowEditor.NodeDefinitionsProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IEnumerable?> WorkflowEditor.ChoiceItemsSourceProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<WorkflowChoiceAdapter?> WorkflowEditor.ChoiceItemAdapterProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<WorkflowNode?> WorkflowEditor.SelectedNodeProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L47) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<WorkflowConnection?> WorkflowEditor.SelectedConnectionProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L51) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.NodeTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L55) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L58) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.PropertyTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L61) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.ConnectionPropertyTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L64) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.NodeDefinitionTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L67) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.HeaderTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L70) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ICommand?> WorkflowEditor.SaveCommandProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L73) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, WorkflowNodeEditor?> WorkflowEditor.SelectedNodeEditorProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L76) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> WorkflowEditor.SearchTextProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L80) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.IsPanModeProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L83) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowRuntimePreviewProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L86) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowValidationProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L89) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, IReadOnlyList<WorkflowNodeGroup>> WorkflowEditor.LibraryGroupsProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L92) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.GraphSummaryProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L96) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.ValidationSummaryProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L99) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, IReadOnlyList<WorkflowValidationIssue>> WorkflowEditor.ValidationIssuesProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L103) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.IsReadOnlyProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L107) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowNodeLibraryProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L110) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowPropertiesProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L113) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.AllowCyclesProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L116) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> WorkflowEditor.SnapToGridProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L119) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> WorkflowEditor.ZoomProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L122) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Vector> WorkflowEditor.PanProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L126) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Thickness> WorkflowEditor.FitPaddingProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L130) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.StatusProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L134) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.ZoomTextProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L137) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

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
