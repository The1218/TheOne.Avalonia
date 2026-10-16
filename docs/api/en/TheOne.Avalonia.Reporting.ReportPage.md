# ReportPage

[English](../en/TheOne.Avalonia.Reporting.ReportPage.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPage.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPage` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L13)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Number`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L13) | `int` | writable; CLR / init-only | `required constructor argument` | Page number in the pagination result, used for page-number rendering. |
| [`Rows`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L13) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportLayoutRow>` | writable; CLR / init-only | `required constructor argument` | Atomic detail rows on this page, without splitting rows again during printing. |
| [`ShowSummary`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L13) | `bool` | writable; CLR / init-only | `required constructor argument` | Whether this page renders the summary, normally assigned to the final page by the paginator. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportPage.ReportPage(int Number, IReadOnlyList<ReportLayoutRow> Rows, bool ShowSummary)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L13) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportLayoutRow`](TheOne.Avalonia.Reporting.ReportLayoutRow.md)
