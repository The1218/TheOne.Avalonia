# ReportPdfExporter

[English](../en/TheOne.Avalonia.Reporting.ReportPdfExporter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPdfExporter.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPdfExporter` · Class · `object`

Exports an immutable definition snapshot through the same drawing path as ReportPreview.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPdfExporter.cs#L11)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static Task ReportPdfExporter.ExportAsync(ReportDocument definition, ReportDataSource? source, Stream destination, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/ReportPdfExporter.cs#L38) | Stages a complete paginated PDF before commit. Honors cancellation before destination commit; concurrent jobs need distinct streams. Readable seekable targets restore original bytes/length/position on write failure. |

Overwriting a nonempty seekable write-only stream throws `NotSupportedException`; overlapping jobs on the same stream throw `InvalidOperationException`. Failed restoration throws `AggregateException` with both errors. Nonseekable I/O cannot roll back accepted bytes. See [ReportDesign export safety](../../controls/en/report-design.md#export-cancellation-retries-and-destination-ownership) for retries, cancellation timing and file opening modes.

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
