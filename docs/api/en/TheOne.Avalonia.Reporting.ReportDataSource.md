# ReportDataSource

[English](../en/TheOne.Avalonia.Reporting.ReportDataSource.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportDataSource.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportDataSource` · Class · `object`

A row snapshot with explicitly typed accessors. Replace the source to refresh changed data.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L12)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Fields`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L21) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportFieldInfo>` | read-only; CLR / n/a | `accessor-defined / state` | Output-field metadata exposed by a port to describe referenceable data. |
| [`Name`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L19) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Name used for presentation/identity; keep it consistent with templates and keys. |
| [`RowCount`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L20) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`object? ReportDataSource.GetValue(int rowIndex, string fieldKey)`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L22) | Reads an original-row value through an explicit field key/typed getter, without reflection paths. |
| [`static ReportDataSource ReportDataSource.Create<T>(string name, IEnumerable<T> rows, params ReportField<T>[] fields)`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L35) | Creates an adapter/source from explicit types, models and delegates; typed getters preserve compile-time information. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportField<T>`](TheOne.Avalonia.Reporting.ReportField_T_.md)
- [`TheOne.Avalonia.Reporting.ReportFieldInfo`](TheOne.Avalonia.Reporting.ReportFieldInfo.md)
