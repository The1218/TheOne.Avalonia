# ReportFieldInfo

[English](../en/TheOne.Avalonia.Reporting.ReportFieldInfo.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportFieldInfo.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportFieldInfo` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L3)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Key`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L3) | `string` | writable; CLR / init-only | `required constructor argument` | Field/parameter key used to resolve business values, distinct from display names. |
| [`Kind`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L3) | `TheOne.Avalonia.Reporting.ReportValueKind` | writable; CLR / init-only | `required constructor argument` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Text`, `Number`, `Date` |
| [`Label`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L3) | `string` | writable; CLR / init-only | `required constructor argument` | Display label, not a substitute for model identity, field paths or serialized identifiers. |
| [`TypeLabel`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L5) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportFieldInfo.ReportFieldInfo(string Key, string Label, ReportValueKind Kind)`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L3) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportValueKind`](TheOne.Avalonia.Reporting.ReportValueKind.md)
