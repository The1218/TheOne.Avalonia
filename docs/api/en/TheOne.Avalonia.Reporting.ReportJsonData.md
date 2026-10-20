# ReportJsonData

[English](../en/TheOne.Avalonia.Reporting.ReportJsonData.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportJsonData.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportJsonData` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool ReportJsonData.IsRootField(string key)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L13) | Checks whether a field key uses the report root-field prefix. |
| [`static object? ReportJsonData.ReadRootValue(string json, string pointer)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L18) | Reads a scalar field from the JSON root using its path. |
| [`static IReadOnlyList<ReportDataNode> ReportJsonData.CreateTree(string json)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L28) | Builds a bounded response tree. Truncating display nodes never truncates the source rows. |
| [`static IReadOnlyList<ReportJsonBody> ReportJsonData.FindBodies(string json)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L79) | Finds JSON arrays and paths that can serve as report detail-record sources. |
| [`static ReportDataSource ReportJsonData.CreateSource(string json, string bodyPointer)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L101) | Creates a report source from a JSON record-array path and explicit field configuration. |
| [`static ReportDataSource ReportJsonData.CreateSource(string json, string bodyPointer, IEnumerable<string>? rootFieldKeys)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L106) | Creates a report source from a JSON record-array path and explicit field configuration. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`const string ReportJsonData.RootFieldPrefix`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportDataNode`](TheOne.Avalonia.Reporting.ReportDataNode.md)
- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportJsonBody`](TheOne.Avalonia.Reporting.ReportJsonBody.md)
