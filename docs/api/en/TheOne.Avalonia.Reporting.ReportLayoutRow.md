# ReportLayoutRow

[English](../en/TheOne.Avalonia.Reporting.ReportLayoutRow.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportLayoutRow.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportLayoutRow` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Cells`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L8) | `System.Collections.Generic.IReadOnlyList<string>` | writable; CLR / init-only | `required constructor argument` | Final cell texts in column order; this snapshot is not a reverse lookup of original data. |
| [`Foregrounds`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L10) | `System.Collections.Generic.IReadOnlyList<string>` | writable; CLR / init-only | `[]` | Conditional foreground colors in cell order, aligned with Cells indices. |
| [`Height`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L8) | `double` | writable; CLR / init-only | `required constructor argument` | Height configuration; preserve content/text/data contracts; fixed values can override theme density. |
| [`SourceIndex`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L8) | `int` | writable; CLR / init-only | `required constructor argument` | Zero-based source-data index for locating the original record. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportLayoutRow.ReportLayoutRow(int SourceIndex, IReadOnlyList<string> Cells, double Height)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L8) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
