# DataGridColumnOptions

[English](../en/TheOne.Avalonia.Controls.DataGridColumnOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridColumnOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridColumnOptions` · Class · `Avalonia.AvaloniaObject`

Explicit model fields and filter conditions for the official DataGrid column types.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`FilterCondition`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L14) | `TheOne.Avalonia.Controls.DataGridFilterCondition?` | writable; TwoWay; attached | `null` | DataGridFilterCondition? / null ; Applied immutable condition; TwoWay supported; null means none; popup drafts are separate |
| [`FilterEnabled`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L12) | `bool` | writable; OneWay; attached | `true` | bool / true ; Disable an individual entry point after grid filtering is enabled; disabling filtering clears that column's condition |
| [`FilterError`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L15) | `string?` | read-only; OneWay; attached | `null` | string? / null ; Read-only phase-2 query property describing condition / field errors; read with GetFilterError |
| [`FilterKind`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L13) | `TheOne.Avalonia.Controls.DataGridFilterKind` | writable; OneWay; attached | `DataGridFilterKind.Auto` | DataGridFilterKind / Auto ; `Auto`, `Text`, `Number`, `None`; Auto uses ValueAccessor's raw field type Values: `Auto`, `Text`, `Number`, `None` |
| [`SummaryEnabled`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L16) | `bool` | writable; OneWay; attached | `false` | bool / false ; Explicitly enable the column; numeric or nullable numeric properties only |
| [`SummaryFormat`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L18) | `string?` | writable; OneWay; attached | `null` | string? / null ; .NET numeric format, independent of display Converter, using the view Culture |
| [`SummaryKind`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L17) | `TheOne.Avalonia.Controls.DataGridSummaryKind` | writable; OneWay; attached | `DataGridSummaryKind.Sum` | DataGridSummaryKind / Sum ; Sum, Average, Min, Max, Count; one per column Validation: `Enum.IsDefined` Values: `Sum`, `Average`, `Min`, `Max`, `Count` |
| [`ValueAccessor`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L11) | `TheOne.Avalonia.Controls.IDataGridValueAccessor?` | writable; OneWay; attached | `null` | IDataGridValueAccessor? / null ; Optional override; ordinary bound columns infer it, while template columns or computed fields can supply a typed delegate |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool DataGridColumnOptions.GetSummaryEnabled(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L19) | Reads the attached property. bool / false ; Explicitly enable the column; numeric or nullable numeric properties only |
| [`static void DataGridColumnOptions.SetSummaryEnabled(DataGridColumn column, bool value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L20) | Sets the attached property. bool / false ; Explicitly enable the column; numeric or nullable numeric properties only |
| [`static DataGridSummaryKind DataGridColumnOptions.GetSummaryKind(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L21) | Reads the attached property. DataGridSummaryKind / Sum ; Sum, Average, Min, Max, Count; one per column |
| [`static void DataGridColumnOptions.SetSummaryKind(DataGridColumn column, DataGridSummaryKind value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L22) | Sets the attached property. DataGridSummaryKind / Sum ; Sum, Average, Min, Max, Count; one per column |
| [`static string? DataGridColumnOptions.GetSummaryFormat(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L23) | Reads the attached property. string? / null ; .NET numeric format, independent of display Converter, using the view Culture |
| [`static void DataGridColumnOptions.SetSummaryFormat(DataGridColumn column, string? value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L24) | Sets the attached property. string? / null ; .NET numeric format, independent of display Converter, using the view Culture |
| [`static IDataGridValueAccessor? DataGridColumnOptions.GetValueAccessor(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L40) | Reads the attached property. IDataGridValueAccessor? / null ; Optional override; ordinary bound columns infer it, while template columns or computed fields can supply a typed delegate |
| [`static void DataGridColumnOptions.SetValueAccessor(DataGridColumn column, IDataGridValueAccessor? value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L41) | Sets the attached property. IDataGridValueAccessor? / null ; Optional override; ordinary bound columns infer it, while template columns or computed fields can supply a typed delegate |
| [`static bool DataGridColumnOptions.GetFilterEnabled(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L42) | Reads the attached property. bool / true ; Disable an individual entry point after grid filtering is enabled; disabling filtering clears that column's condition |
| [`static void DataGridColumnOptions.SetFilterEnabled(DataGridColumn column, bool value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L43) | Sets the attached property. bool / true ; Disable an individual entry point after grid filtering is enabled; disabling filtering clears that column's condition |
| [`static DataGridFilterKind DataGridColumnOptions.GetFilterKind(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L44) | Reads the attached property. DataGridFilterKind / Auto ; `Auto`, `Text`, `Number`, `None`; Auto uses ValueAccessor's raw field type |
| [`static void DataGridColumnOptions.SetFilterKind(DataGridColumn column, DataGridFilterKind value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L45) | Sets the attached property. DataGridFilterKind / Auto ; `Auto`, `Text`, `Number`, `None`; Auto uses ValueAccessor's raw field type |
| [`static DataGridFilterCondition? DataGridColumnOptions.GetFilterCondition(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L46) | Reads the attached property. DataGridFilterCondition? / null ; Applied immutable condition; TwoWay supported; null means none; popup drafts are separate |
| [`static void DataGridColumnOptions.SetFilterCondition(DataGridColumn column, DataGridFilterCondition? value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L47) | Sets the attached property. DataGridFilterCondition? / null ; Applied immutable condition; TwoWay supported; null means none; popup drafts are separate |
| [`static string? DataGridColumnOptions.GetFilterError(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L48) | Reads the attached property. string? / null ; Read-only phase-2 query property describing condition / field errors; read with GetFilterError |
| [`DataGridColumnOptions.DataGridColumnOptions()`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L9) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<IDataGridValueAccessor?> DataGridColumnOptions.ValueAccessorProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> DataGridColumnOptions.FilterEnabledProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<DataGridFilterKind> DataGridColumnOptions.FilterKindProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<DataGridFilterCondition?> DataGridColumnOptions.FilterConditionProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<string?> DataGridColumnOptions.FilterErrorProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> DataGridColumnOptions.SummaryEnabledProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<DataGridSummaryKind> DataGridColumnOptions.SummaryKindProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<string?> DataGridColumnOptions.SummaryFormatProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.DataGridFilterCondition`](TheOne.Avalonia.Controls.DataGridFilterCondition.md)
- [`TheOne.Avalonia.Controls.DataGridFilterKind`](TheOne.Avalonia.Controls.DataGridFilterKind.md)
- [`TheOne.Avalonia.Controls.DataGridSummaryKind`](TheOne.Avalonia.Controls.DataGridSummaryKind.md)
- [`TheOne.Avalonia.Controls.IDataGridValueAccessor`](TheOne.Avalonia.Controls.IDataGridValueAccessor.md)
