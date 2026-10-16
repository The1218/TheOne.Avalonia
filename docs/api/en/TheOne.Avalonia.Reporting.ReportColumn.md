# ReportColumn

[English](../en/TheOne.Avalonia.Reporting.ReportColumn.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportColumn.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportColumn` · Class · `TheOne.Avalonia.Reporting.ReportObservable`

[Inherited additional members: ReportObservable](TheOne.Avalonia.Reporting.ReportObservable.md)

A detail column. FieldKey identifies an explicitly registered getter, never a reflection path.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L89)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Condition`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L100) | `TheOne.Avalonia.Reporting.ReportCondition` | writable; CLR / n/a | `0 / default` | Report conditional-format comparison enum, paired with ConditionValue. Values: `GreaterThan`, `LessThan`, `Equal` |
| [`ConditionColor`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L102) | `string` | writable; CLR / n/a | `"#C43C3C"` | Color string used when a column condition matches, validated by the paginator. |
| [`ConditionEnabled`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L99) | `bool` | writable; CLR / n/a | `false` | Enables conditional formatting for the column, retaining configuration when disabled. |
| [`ConditionValue`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L101) | `decimal` | writable; CLR / n/a | `0 / default` | Decimal threshold compared using the report Condition operator. |
| [`DecimalPlaces`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L96) | `int` | writable; CLR / n/a | `2` | Displayed decimal places, formatting without changing source precision. |
| [`FieldKey`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L92) | `string` | writable; CLR / n/a | `""` | Explicitly registered field key rather than a reflection path; must match a source field. |
| [`Format`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L95) | `TheOne.Avalonia.Reporting.ReportValueFormat` | writable; CLR / n/a | `0 / default` | Field display-format enum: automatic, text, numeric, currency or date, without changing source values. Values: `Automatic`, `Text`, `Number`, `Currency`, `Date` |
| [`Header`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L91) | `string` | writable; CLR / n/a | `"新列"` | Header content, as text or a model; HeaderTemplate controls custom presentation. |
| [`ShowSum`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L98) | `bool` | writable; CLR / n/a | `false` | Shows the column's numeric total in the summary, following report rules for nonnumeric fields. |
| [`Text`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L93) | `string` | writable; CLR / n/a | `""` | Input/display text; notifications and validation follow the model/native editor contract. |
| [`UseThousandsSeparator`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L97) | `bool` | writable; CLR / n/a | `true` | Enables thousands separators in numeric display text only. |
| [`WidthWeight`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L94) | `double` | writable; CLR / n/a | `1` | Relative table-column width weight, allocated from available table width. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportColumn.ReportColumn()`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L89) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportCondition`](TheOne.Avalonia.Reporting.ReportCondition.md)
- [`TheOne.Avalonia.Reporting.ReportValueFormat`](TheOne.Avalonia.Reporting.ReportValueFormat.md)
