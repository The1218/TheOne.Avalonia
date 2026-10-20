# ReportPrintOptions

[English](../en/TheOne.Avalonia.Reporting.ReportPrintOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPrintOptions.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPrintOptions` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L14)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Copies`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L14) | `int` | writable; CLR / init-only | `1` | Copy count, validated/submitted by the print handler to the selected printer. |
| [`PrinterName`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L14) | `string?` | writable; CLR / init-only | `null` | Selected printer name; an empty value delegates default behavior to the print handler. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportPrintOptions.ReportPrintOptions(string? PrinterName = null, int Copies = 1)`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L14) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
