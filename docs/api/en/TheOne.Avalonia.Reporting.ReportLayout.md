# ReportLayout

[English](../en/TheOne.Avalonia.Reporting.ReportLayout.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportLayout.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportLayout` · Class · `object`

Immutable text and geometry snapshot shared by preview and the designer's drawing surface.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L16)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Columns`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L20) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportLayoutColumn>` | writable; CLR / init-only | `required constructor argument` | Report column definitions or paginated snapshots; definitions are editable while snapshots serve rendering, as defined by their element types. |
| [`ContentWidth`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L23) | `double` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Description`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L17) | `string` | writable; CLR / init-only | `required constructor argument` | Descriptive text supplied by the host; omitted values keep their initial state. |
| [`DetailBottom`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L19) | `double` | writable; CLR / init-only | `required constructor argument` | Bottom coordinate of the report detail region in DIPs, shared by pagination/rendering. |
| [`Elements`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L26) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportLayoutElement>` | writable; CLR / init-only | `[]` | Free report element definitions or paginated snapshots; source geometry uses millimetres and layout snapshots use DIPs. |
| [`FontSize`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L18) | `double` | writable; CLR / init-only | `required constructor argument` | Report font size in DIPs, with document/element range validation performed by the paginator. |
| [`Footer`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L17) | `string` | writable; CLR / init-only | `required constructor argument` | Footer content, customized independently of body/header. |
| [`HeaderHeight`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L19) | `double` | writable; CLR / init-only | `required constructor argument` | Header height in chart DIPs or report layout coordinates; do not confuse it with millimetre source fields. |
| [`Margin`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L18) | `double` | writable; CLR / init-only | `required constructor argument` | Default paginated-layout margin in DIPs, distinct from source-model MarginMm. |
| [`PageHeight`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L18) | `double` | writable; CLR / init-only | `required constructor argument` | Paginated page height in DIPs, already converted from millimetres. |
| [`PageWidth`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L18) | `double` | writable; CLR / init-only | `required constructor argument` | Paginated page width in DIPs, already converted from millimetres. |
| [`Pages`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L21) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportPage>` | writable; CLR / init-only | `required constructor argument` | Page snapshots produced by the paginator, sharing layout between preview and printing. |
| [`Subtitle`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L17) | `string` | writable; CLR / init-only | `required constructor argument` | Report subtitle displayed independently of the main title. |
| [`Summary`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L20) | `System.Collections.Generic.IReadOnlyList<string>` | writable; CLR / init-only | `required constructor argument` | Column-aligned summary display texts calculated by the paginator from source data. |
| [`SummaryHeight`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L19) | `double` | writable; CLR / init-only | `required constructor argument` | Height of the paginated summary region in DIPs. |
| [`TableTop`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L19) | `double` | writable; CLR / init-only | `required constructor argument` | Top coordinate of the paginated table region in DIPs. |
| [`TableWidth`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L25) | `double` | writable; CLR / init-only | `0 / default` | Actual paginated table width in DIPs. |
| [`TableX`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L24) | `double` | writable; CLR / init-only | `Margin` | Horizontal origin of the paginated table in DIPs. |
| [`Title`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L17) | `string` | writable; CLR / init-only | `required constructor argument` | Display title, distinct from business identity or a localization key. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportLayout.ReportLayout(string Title, string Subtitle, string Description, string Footer, double PageWidth, double PageHeight, double Margin, double FontSize, double TableTop, double HeaderHeight, double DetailBottom, double SummaryHeight, IReadOnlyList<ReportLayoutColumn> Columns, IReadOnlyList<string> Summary, IReadOnlyList<ReportPage> Pages)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L16) | Immutable text and geometry snapshot shared by preview and the designer's drawing surface. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportLayoutColumn`](TheOne.Avalonia.Reporting.ReportLayoutColumn.md)
- [`TheOne.Avalonia.Reporting.ReportLayoutElement`](TheOne.Avalonia.Reporting.ReportLayoutElement.md)
- [`TheOne.Avalonia.Reporting.ReportPage`](TheOne.Avalonia.Reporting.ReportPage.md)
