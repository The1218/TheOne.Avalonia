# ReportPaginator

[English](../en/TheOne.Avalonia.Reporting.ReportPaginator.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPaginator.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPaginator` · Class · `object`

Single table pagination: repeated title/header, atomic wrapped rows, last-page totals and page numbers.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L59)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static ReportLayout ReportPaginator.Paginate(ReportDocument document, ReportDataSource? source = null)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L98) | Validates the document and computes immutable paginated layout shared by preview/printing. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`const double ReportPaginator.DipsPerMillimetre`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L67) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
- [`TheOne.Avalonia.Reporting.ReportLayout`](TheOne.Avalonia.Reporting.ReportLayout.md)
