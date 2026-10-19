# ReportDocument

[English](../en/TheOne.Avalonia.Reporting.ReportDocument.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportDocument.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportDocument` · Class · `TheOne.Avalonia.Reporting.ReportObservable`

[Inherited additional members: ReportObservable](TheOne.Avalonia.Reporting.ReportObservable.md)

Mutable single-table report definition. Geometry is in millimetres; text and row sizes are in DIPs.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L234)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`BottomMargin`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L366) | `double` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Columns`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L383) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Reporting.ReportColumn>` | read-only; CLR / n/a | `[]` | Report column definitions or paginated snapshots; definitions are editable while snapshots serve rendering, as defined by their element types. |
| [`DataBodyPointer`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L357) | `string` | writable; CLR / n/a | `""` | JSON Pointer to the record array in an HTTP JSON response; empty means the response root. |
| [`DataSourceAuthentication`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L338) | `TheOne.Avalonia.Reporting.ReportApiAuthentication?` | writable; CLR / n/a | `null` | Login configuration for source requests, without persisting runtime tokens in the document. |
| [`DataSourceMethod`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L320) | `string` | writable; CLR / n/a | `"GET"` | Report-source HTTP method, GET by default. |
| [`DataSourceName`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L344) | `string` | writable; CLR / n/a | `"接口数据"` | Source label in the designer's data catalog, default “接口数据”; saved with the template without changing the API URL or field keys. |
| [`DataSourceParameterMode`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L332) | `TheOne.Avalonia.Reporting.ReportApiParameterMode` | writable; CLR / n/a | `0 / default` | Parameter-encoding enum for the report source. Values: `Json`, `Form` |
| [`DataSourceParameters`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L326) | `string` | writable; CLR / n/a | `""` | Report-source parameter text, interpreted with its encoding mode. |
| [`DataSourceUrl`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L314) | `string` | writable; CLR / n/a | `""` | Remote report JSON source URL, following report replacement/refresh loading rules. |
| [`Description`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L248) | `string` | writable; CLR / n/a | `""` | Descriptive text supplied by the host; omitted values keep their initial state. |
| [`Elements`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L386) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Reporting.ReportElement>` | read-only; CLR / n/a | `[]` | Free report element definitions or paginated snapshots; source geometry uses millimetres and layout snapshots use DIPs. |
| [`FontSize`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L370) | `double` | writable; CLR / n/a | `12` | Report font size in DIPs, with document/element range validation performed by the paginator. |
| [`Footer`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L254) | `string` | writable; CLR / n/a | `""` | Footer content, customized independently of body/header. |
| [`FooterHeightMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L308) | `double` | writable; CLR / n/a | `18` | FooterHeightMm: page/element geometry in millimetres. Nullable margins fall back to MarginMm. Pagination converts to DIPs; do not preapply view zoom. |
| [`FooterTop`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L368) | `double` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HeaderBottom`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L367) | `double` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HeaderHeightMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L302) | `double` | writable; CLR / n/a | `32` | HeaderHeightMm: page/element geometry in millimetres. Nullable margins fall back to MarginMm. Pagination converts to DIPs; do not preapply view zoom. |
| [`LeftMargin`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L363) | `double` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`MarginBottomMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L296) | `double?` | writable; CLR / n/a | `null` | MarginBottomMm: page/element geometry in millimetres. Nullable margins fall back to MarginMm. Pagination converts to DIPs; do not preapply view zoom. |
| [`MarginLeftMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L278) | `double?` | writable; CLR / n/a | `null` | MarginLeftMm: page/element geometry in millimetres. Nullable margins fall back to MarginMm. Pagination converts to DIPs; do not preapply view zoom. |
| [`MarginMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L272) | `double` | writable; CLR / n/a | `14` | MarginMm: page/element geometry in millimetres. Nullable margins fall back to MarginMm. Pagination converts to DIPs; do not preapply view zoom. |
| [`MarginRightMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L290) | `double?` | writable; CLR / n/a | `null` | MarginRightMm: page/element geometry in millimetres. Nullable margins fall back to MarginMm. Pagination converts to DIPs; do not preapply view zoom. |
| [`MarginTopMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L284) | `double?` | writable; CLR / n/a | `null` | MarginTopMm: page/element geometry in millimetres. Nullable margins fall back to MarginMm. Pagination converts to DIPs; do not preapply view zoom. |
| [`MinimumRowHeight`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L376) | `double` | writable; CLR / n/a | `30` | Minimum report-detail row height in DIPs; wrapped content may increase actual height. |
| [`PageHeightMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L266) | `double` | writable; CLR / n/a | `297` | PageHeightMm: page/element geometry in millimetres. Nullable margins fall back to MarginMm. Pagination converts to DIPs; do not preapply view zoom. |
| [`PageWidthMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L260) | `double` | writable; CLR / n/a | `210` | PageWidthMm: page/element geometry in millimetres. Nullable margins fall back to MarginMm. Pagination converts to DIPs; do not preapply view zoom. |
| [`RightMargin`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L365) | `double` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectedDataPointers`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L351) | `string[]?` | writable; CLR / n/a | `null` | Designer item selection, independent of the data used by existing report bindings. |
| [`Subtitle`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L242) | `string` | writable; CLR / n/a | `"DETAIL REPORT"` | Report subtitle displayed independently of the main title. |
| [`Title`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L236) | `string` | writable; CLR / n/a | `"新建报表"` | Display title, distinct from business identity or a localization key. |
| [`TopMargin`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L364) | `double` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`string ReportDocument.Save()`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L389) | Returns a portable JSON definition including public API/image URLs, but no fetched records. |
| [`static ReportDocument ReportDocument.Load(string json)`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L394) | Loads a typed business document from JSON under its version/field/structure validation contract. |
| [`ReportDocument.ReportDocument()`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L234) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportApiAuthentication`](TheOne.Avalonia.Reporting.ReportApiAuthentication.md)
- [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](TheOne.Avalonia.Reporting.ReportApiParameterMode.md)
- [`TheOne.Avalonia.Reporting.ReportColumn`](TheOne.Avalonia.Reporting.ReportColumn.md)
- [`TheOne.Avalonia.Reporting.ReportElement`](TheOne.Avalonia.Reporting.ReportElement.md)
