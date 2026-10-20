# ReportField

[English](../en/TheOne.Avalonia.Reporting.ReportField_T_.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportField_T_.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportField<T>` · Class · `object`

Register the actual row accessor at compile time, including any nested property access.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Getter`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L12) | `System.Func<T, object?>` | writable; CLR / init-only | `required constructor argument` | Explicit typed field getter that reads original rows without string-based reflection lookup. |
| [`Key`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L10) | `string` | writable; CLR / init-only | `required constructor argument` | Field/parameter key used to resolve business values, distinct from display names. |
| [`Kind`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L13) | `TheOne.Avalonia.Reporting.ReportValueKind` | writable; CLR / init-only | `ReportValueKind.Text` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Text`, `Number`, `Date` |
| [`Label`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L11) | `string` | writable; CLR / init-only | `required constructor argument` | Display label, not a substitute for model identity, field paths or serialized identifiers. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportField<T>.ReportField(string Key, string Label, Func<T, object?> Getter, ReportValueKind Kind = ReportValueKind.Text)`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L9) | Register the actual row accessor at compile time, including any nested property access. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportValueKind`](TheOne.Avalonia.Reporting.ReportValueKind.md)
