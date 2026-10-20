# PrintTool

[English](../en/TheOne.Avalonia.Reporting.PrintTool.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.PrintTool.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.PrintTool` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L23)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CurrentPlatform`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L27) | `TheOne.Avalonia.Reporting.ReportPrintPlatform` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. Values: `Windows`, `Linux`, `MacOS` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static ProcessStartInfo PrintTool.CreateStartInfo(string pdfPath, ReportPrintPlatform platform, ReportPrintOptions? options = null)`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L33) | Builds shell-free CUPS arguments, or the Windows registered PDF print verb. |
| [`static Task<ReportPrintResult> PrintTool.PrintPdfAsync(string pdfPath, ReportPrintOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L66) | Passes an existing PDF to the desktop print adapter and returns its result; see platform/file-retention rules. |
| [`static void PrintTool.CleanupTemporaryDocuments()`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L171) | Removes print PDFs older than 24 hours, including files left by a previous process. Call at application startup if printing is infrequent. Only this library's temporary folders are inspected. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportPrintOptions`](TheOne.Avalonia.Reporting.ReportPrintOptions.md)
- [`TheOne.Avalonia.Reporting.ReportPrintPlatform`](TheOne.Avalonia.Reporting.ReportPrintPlatform.md)
- [`TheOne.Avalonia.Reporting.ReportPrintResult`](TheOne.Avalonia.Reporting.ReportPrintResult.md)
