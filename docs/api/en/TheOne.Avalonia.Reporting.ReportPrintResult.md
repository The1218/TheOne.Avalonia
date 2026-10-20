# ReportPrintResult

[English](../en/TheOne.Avalonia.Reporting.ReportPrintResult.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPrintResult.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPrintResult` · Class · `object`

Submission is accepted by the OS handler/spooler, not confirmation that paper was printed.

[Source / 声明](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L17)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`DocumentPath`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L20) | `string?` | writable; CLR / init-only | `null` | Temporary PDF retained for an asynchronous Windows handler, if applicable. |
| [`KeepDocument`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L17) | `bool` | writable; CLR / init-only | `required constructor argument` | Whether to retain the generated document after printing, following the receiver's disposal contract. |
| [`Message`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L17) | `string` | writable; CLR / init-only | `required constructor argument` | Result message for users/diagnostics; status fields still determine the operation outcome. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportPrintResult.ReportPrintResult(string Message, bool KeepDocument)`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L17) | Submission is accepted by the OS handler/spooler, not confirmation that paper was printed. |
