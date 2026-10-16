# ReportJsonBody

[English](../en/TheOne.Avalonia.Reporting.ReportJsonBody.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportJsonBody.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportJsonBody` · Class · `object`

JSON Pointer identifies the body unambiguously, even when property names contain dots or slashes.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L6)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Label`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L6) | `string` | writable; CLR / init-only | `required constructor argument` | Display label, not a substitute for model identity, field paths or serialized identifiers. |
| [`Pointer`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L6) | `string` | writable; CLR / init-only | `required constructor argument` | JSON Pointer path into source JSON; see source documentation for path/root semantics. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportJsonBody.ReportJsonBody(string Pointer, string Label)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L6) | JSON Pointer identifies the body unambiguously, even when property names contain dots or slashes. |
