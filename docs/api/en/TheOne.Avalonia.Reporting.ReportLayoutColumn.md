# ReportLayoutColumn

[English](../en/TheOne.Avalonia.Reporting.ReportLayoutColumn.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportLayoutColumn.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportLayoutColumn` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`FieldKey`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7) | `string` | writable; CLR / init-only | `required constructor argument` | Explicitly registered field key rather than a reflection path; must match a source field. |
| [`Header`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7) | `string` | writable; CLR / init-only | `required constructor argument` | Header content, as text or a model; HeaderTemplate controls custom presentation. |
| [`IsNumeric`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7) | `bool` | writable; CLR / init-only | `required constructor argument` | Whether the layout column is numeric, affecting alignment/summary formatting. |
| [`Width`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7) | `double` | writable; CLR / init-only | `required constructor argument` | Width configuration; units follow the owning model, without changing data-coordinate meaning. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportLayoutColumn.ReportLayoutColumn(string Header, string FieldKey, double Width, bool IsNumeric)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
