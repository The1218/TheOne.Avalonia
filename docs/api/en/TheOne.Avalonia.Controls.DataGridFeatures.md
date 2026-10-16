# DataGridFeatures

[English](../en/TheOne.Avalonia.Controls.DataGridFeatures.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridFeatures.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridFeatures` · Class · `Avalonia.AvaloniaObject`

Column filtering and native right freezing; preserves the data view and presenters.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L27)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`FrozenRightColumnCount`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L29) | `int` | writable; OneWay; attached | `inherited registration` | Number of frozen columns on the right, sharing available width with left-frozen columns. |
| [`ShowColumnFilters`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L33) | `bool` | writable; OneWay; attached | `false` | Enables column-filter entry points; each column must also permit filtering. |
| [`ShowSummaryRow`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L36) | `bool` | writable; OneWay; attached | `false` | bool / false ; Separate summary row; disabling stops calculation / subscriptions but retains column settings |
| [`SummaryEmptyText`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L40) | `string` | writable; OneWay; attached | `"—"` | string / — ; Sum / Average / Min / Max text when no numeric values exist; Count is 0 |
| [`SummaryLabel`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L39) | `string` | writable; OneWay; attached | `"合计"` | string / 合计 ; Default label means total; shown in the native row-header slot when visible, without replacing the first result |
| [`SummaryLeavesOnly`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L38) | `bool` | writable; OneWay; attached | `false` | bool / false ; Only structural tree leaves, preventing parent/child double counting; ordinary rows are all leaves |
| [`SummaryScope`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L37) | `TheOne.Avalonia.Controls.DataGridSummaryScope` | writable; OneWay; attached | `DataGridSummaryScope.FilteredItems` | DataGridSummaryScope / FilteredItems ; Locally loaded models after business / column filters and before paging; SourceItems ignores filters, VisibleItems enumerates the current view; tree FilteredItems counts direct matches only Validation: `Enum.IsDefined` Values: `FilteredItems`, `SourceItems`, `VisibleItems` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static int DataGridFeatures.GetFrozenRightColumnCount(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L30) | Reads the attached property. Number of frozen columns on the right, sharing available width with left-frozen columns. |
| [`static void DataGridFeatures.SetFrozenRightColumnCount(DataGrid grid, int count)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L31) | Sets the attached property. Number of frozen columns on the right, sharing available width with left-frozen columns. |
| [`static bool DataGridFeatures.GetShowSummaryRow(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L43) | Reads the attached property. bool / false ; Separate summary row; disabling stops calculation / subscriptions but retains column settings |
| [`static void DataGridFeatures.SetShowSummaryRow(DataGrid grid, bool value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L44) | Sets the attached property. bool / false ; Separate summary row; disabling stops calculation / subscriptions but retains column settings |
| [`static DataGridSummaryScope DataGridFeatures.GetSummaryScope(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L45) | Reads the attached property. DataGridSummaryScope / FilteredItems ; Locally loaded models after business / column filters and before paging; SourceItems ignores filters, VisibleItems enumerates the current view; tree FilteredItems counts direct matches only |
| [`static void DataGridFeatures.SetSummaryScope(DataGrid grid, DataGridSummaryScope value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L46) | Sets the attached property. DataGridSummaryScope / FilteredItems ; Locally loaded models after business / column filters and before paging; SourceItems ignores filters, VisibleItems enumerates the current view; tree FilteredItems counts direct matches only |
| [`static bool DataGridFeatures.GetSummaryLeavesOnly(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L47) | Reads the attached property. bool / false ; Only structural tree leaves, preventing parent/child double counting; ordinary rows are all leaves |
| [`static void DataGridFeatures.SetSummaryLeavesOnly(DataGrid grid, bool value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L48) | Sets the attached property. bool / false ; Only structural tree leaves, preventing parent/child double counting; ordinary rows are all leaves |
| [`static string DataGridFeatures.GetSummaryLabel(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L49) | Reads the attached property. string / 合计 ; Default label means total; shown in the native row-header slot when visible, without replacing the first result |
| [`static void DataGridFeatures.SetSummaryLabel(DataGrid grid, string value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L50) | Sets the attached property. string / 合计 ; Default label means total; shown in the native row-header slot when visible, without replacing the first result |
| [`static string DataGridFeatures.GetSummaryEmptyText(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L51) | Reads the attached property. string / — ; Sum / Average / Min / Max text when no numeric values exist; Count is 0 |
| [`static void DataGridFeatures.SetSummaryEmptyText(DataGrid grid, string value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L52) | Sets the attached property. string / — ; Sum / Average / Min / Max text when no numeric values exist; Count is 0 |
| [`static DataGridSummaryResult? DataGridFeatures.GetSummaryResult(DataGrid grid, DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L53) | Reads current column aggregates, including valid count, result and errors. |
| [`static bool DataGridFeatures.GetShowColumnFilters(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L62) | Reads the attached property. Enables column-filter entry points; each column must also permit filtering. |
| [`static void DataGridFeatures.SetShowColumnFilters(DataGrid grid, bool value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L63) | Sets the attached property. Enables column-filter entry points; each column must also permit filtering. |
| [`static DataGridFilterApplyResult DataGridFeatures.ApplyFilter(DataGrid grid, DataGridColumn column, DataGridFilterCondition? condition)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L64) | Applies a column filter and returns its result; inspect result/filter-error state on failure. |
| [`static DataGridFilterApplyResult DataGridFeatures.ClearFilter(DataGrid grid, DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L65) | Clears the specified column filter while preserving other conditions. |
| [`static DataGridFilterApplyResult DataGridFeatures.ClearAllFilters(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L66) | Clears all column filters and refreshes visible rows. |
| [`DataGridFeatures.DataGridFeatures()`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L27) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<int> DataGridFeatures.FrozenRightColumnCountProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> DataGridFeatures.ShowColumnFiltersProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<DataGridFilterChangingEventArgs> DataGridFeatures.FilterChangingEvent`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<DataGridFilterChangedEventArgs> DataGridFeatures.FilterChangedEvent`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> DataGridFeatures.ShowSummaryRowProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<DataGridSummaryScope> DataGridFeatures.SummaryScopeProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L37) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> DataGridFeatures.SummaryLeavesOnlyProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<string> DataGridFeatures.SummaryLabelProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L39) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<string> DataGridFeatures.SummaryEmptyTextProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L40) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<DataGridSummaryUpdatedEventArgs> DataGridFeatures.SummaryUpdatedEvent`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.DataGridFilterApplyResult`](TheOne.Avalonia.Controls.DataGridFilterApplyResult.md)
- [`TheOne.Avalonia.Controls.DataGridFilterCondition`](TheOne.Avalonia.Controls.DataGridFilterCondition.md)
- [`TheOne.Avalonia.Controls.DataGridSummaryResult`](TheOne.Avalonia.Controls.DataGridSummaryResult.md)
- [`TheOne.Avalonia.Controls.DataGridSummaryScope`](TheOne.Avalonia.Controls.DataGridSummaryScope.md)
