# ReportPdfExporter

[English](../en/TheOne.Avalonia.Reporting.ReportPdfExporter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPdfExporter.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPdfExporter` · Class · `object`

Exports an immutable definition snapshot through the same drawing path as ReportPreview.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPdfExporter.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static Task ReportPdfExporter.ExportAsync(ReportDocument definition, ReportDataSource? source, Stream destination, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/ReportPdfExporter.cs#L12) | Exports a paginated report PDF asynchronously with caller-supplied destination/cancellation/file lifetime. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
