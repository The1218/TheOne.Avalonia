# ReportDesign

[English](../en/TheOne.Avalonia.Controls.ReportDesign.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ReportDesign.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ReportDesign` · Class · `Avalonia.Controls.UserControl`

A single-table report workbench with typed field binding and paginated A4 preview.

[Source / 声明](../../../TheOne.Avalonia/Controls/ReportDesign.Commands.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ConditionIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L21) | `int` | writable; OneWay | `computed / state` | Current conditional-format editor option index, synchronized with the selected column. |
| [`CurrentPageNumber`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L39) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`DataSource`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L23) | `TheOne.Avalonia.Reporting.ReportDataSource?` | writable; OneWay | `null` | Explicit data source supplied by the host according to the model contract; templates do not replace data adaptation. |
| [`Document`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L22) | `TheOne.Avalonia.Reporting.ReportDocument?` | writable; OneWay | `null` | Current business document model; editor documentation defines subscription, selection and history behavior on replacement. |
| [`FontFamilyName`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L30) | `string` | writable; OneWay | `computed / state` | Report text font-family name; use fonts available on the target platform and check fallback. |
| [`FontSizeValue`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L25) | `double` | writable; OneWay | `computed / state` | Current font-size editor value, synchronized with the selected report element/document. |
| [`FormatIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L31) | `int` | writable; OneWay | `computed / state` | Current option index in the report numeric-format editor. |
| [`ImageFitIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L39) | `int` | writable; OneWay | `computed / state` | Current option index in the report image-fit editor. |
| [`IsPreview`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L38) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Layout`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L36) | `TheOne.Avalonia.Reporting.ReportLayout?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SaveCommand`](../../../TheOne.Avalonia/Controls/ReportDesign.Commands.cs#L26) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Consumer save command. Its parameter is the current ReportDocument; call document.Save() to obtain JSON. |
| [`SelectedColumn`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L24) | `TheOne.Avalonia.Reporting.ReportColumn?` | writable; TwoWay | `null` | Selected report column model; null means no selection. |
| [`SelectedElement`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L19) | `TheOne.Avalonia.Reporting.ReportElement?` | writable; TwoWay | `null` | Selected report element model; null means no selection. |
| [`SelectedField`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L26) | `TheOne.Avalonia.Reporting.ReportFieldInfo?` | writable; OneWay | `computed / state` | Selected available report field, used for insertion/binding edits. |
| [`SourceFields`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L25) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportFieldInfo>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SumIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L40) | `int` | writable; OneWay | `computed / state` | Current option index in the report summary-operation editor. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L37) | `string?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Zoom`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L40) | `double` | writable; CLR / n/a | `accessor-defined / state` | View scale factor, where 1 means 100%; does not modify report millimetres or workflow coordinates. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void ReportDesign.MarkSaved()`](../../../TheOne.Avalonia/Controls/ReportDesign.Commands.cs#L28) | Call after the consumer's server save succeeds; executing SaveCommand alone does not imply success. |
| [`Task ReportDesign.DropDataNodeAsync(ReportDataNode node, double x, double y)`](../../../TheOne.Avalonia/Controls/ReportDesign.Data.cs#L100) | Drops an actual JSON node or typed source field at a paper position in millimetres. |
| [`ReportElement ReportDesign.AddElement(ReportElementKind kind, double x, double y)`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L54) | Adds an unbound component at a page position in millimetres. A report supports one repeating table. |
| [`void ReportDesign.DropField(string fieldKey, double x, double y)`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L115) | Binds a dropped field to the hit table detail cell or text element; empty paper creates a bound text element. |
| [`Task ReportDesign.FetchApiDataAsync(string url, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L136) | GET a JSON endpoint and expose its actual object, scalar and collection nodes for binding. |
| [`Task ReportDesign.FetchApiDataAsync(ReportApiRequest request, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L139) | Fetches a configured API request and imports its response tree without changing existing bindings. |
| [`void ReportDesign.SetImage(ReportElement element, ReadOnlySpan<byte> bytes, string name = "图片")`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L49) | Assigns a validated, embedded raster image as one undoable edit. |
| [`string ReportDesign.SaveTemplate()`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L61) | Serializes only the report definition, bindings and embedded images. |
| [`void ReportDesign.LoadTemplate(string json)`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L63) | Validates a template, replaces the document and automatically restores its optional API data. |
| [`Task ReportDesign.ExportPdfAsync(Stream destination, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L72) | Exports every preview page as a vector PDF. Must be called on the UI thread. |
| [`Task ReportDesign.LoadTemplateAsync(string json, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportDesign.Loading.cs#L103) | Loads a saved definition and awaits its optional API data and bound images. |
| [`void ReportDesign.RefreshReport()`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L184) | Re-evaluates registered getters and repaginates. Source row membership is a creation-time snapshot. |
| [`void ReportDesign.ShowPreview()`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L185) | Switches the report designer to preview mode while retaining its document. |
| [`void ReportDesign.ShowDesign()`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L186) | Switches the report designer back to design mode. |
| [`void ReportDesign.GoToPage(int pageNumber)`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L187) | Navigates to a requested page; paging/report contracts define indexing and range handling. |
| [`void ReportDesign.BindField(string fieldKey)`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L192) | Binds a selected report element/column to an explicitly registered data field. |
| [`ReportDesign.ReportDesign()`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L54) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<ICommand?> ReportDesign.SaveCommandProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Commands.cs#L25) | Consumer save command. Its parameter is the current ReportDocument; call document.Save() to obtain JSON. |
| [`static readonly StyledProperty<ReportElement?> ReportDesign.SelectedElementProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ReportDesign, int> ReportDesign.ImageFitIndexProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ReportDesign, int> ReportDesign.SumIndexProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ReportDesign, int> ReportDesign.ConditionIndexProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ReportDesign, string> ReportDesign.FontFamilyNameProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ReportDesign, double> ReportDesign.FontSizeValueProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ReportDocument?> ReportDesign.DocumentProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ReportDataSource?> ReportDesign.DataSourceProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ReportColumn?> ReportDesign.SelectedColumnProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ReportDesign, IReadOnlyList<ReportFieldInfo>> ReportDesign.SourceFieldsProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ReportDesign, ReportFieldInfo?> ReportDesign.SelectedFieldProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ReportDesign, int> ReportDesign.FormatIndexProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportApiRequest`](TheOne.Avalonia.Reporting.ReportApiRequest.md)
- [`TheOne.Avalonia.Reporting.ReportColumn`](TheOne.Avalonia.Reporting.ReportColumn.md)
- [`TheOne.Avalonia.Reporting.ReportDataNode`](TheOne.Avalonia.Reporting.ReportDataNode.md)
- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
- [`TheOne.Avalonia.Reporting.ReportElement`](TheOne.Avalonia.Reporting.ReportElement.md)
- [`TheOne.Avalonia.Reporting.ReportElementKind`](TheOne.Avalonia.Reporting.ReportElementKind.md)
- [`TheOne.Avalonia.Reporting.ReportFieldInfo`](TheOne.Avalonia.Reporting.ReportFieldInfo.md)
- [`TheOne.Avalonia.Reporting.ReportLayout`](TheOne.Avalonia.Reporting.ReportLayout.md)
