# ReportTemplate

[English](../en/TheOne.Avalonia.Reporting.ReportTemplate.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportTemplate.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportTemplate` · Class · `object`

Versioned, portable report templates. Data rows and API responses are never serialized.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportTemplate.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static ReportDocument ReportTemplate.CreateSalesDetail()`](../../../TheOne.Avalonia/Reporting/ReportTemplate.cs#L13) | A source-independent starting template matching the sales report design. |
| [`static string ReportTemplate.Save(ReportDocument document)`](../../../TheOne.Avalonia/Reporting/ReportTemplate.cs#L33) | Saves portable business-document JSON, excluding runtime credentials, visual objects and transient state. |
| [`static ReportDocument ReportTemplate.Load(string json)`](../../../TheOne.Avalonia/Reporting/ReportTemplate.cs#L40) | Loads a typed business document from JSON under its version/field/structure validation contract. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`const int ReportTemplate.MaximumBytes`](../../../TheOne.Avalonia/Reporting/ReportTemplate.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
