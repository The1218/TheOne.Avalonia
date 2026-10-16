# DataGrid filtering, frozen columns, and summaries

[简体中文](../zh-cn/datagrid-extensions.md) · **English**

Native DataGrid, column types, virtualized presenters, and keyboard behavior are retained, with automatic entity-property filters, native right frozen columns, and numeric summaries.


See [DataGrid binding and sorting](datagrid-binding.md) for type inference, generic collection views, header sort buttons, and cell sizing.

## Column filtering

Compiled column bindings identify entity properties automatically. Enable grid filtering and register `DataGridAot.PreserveRowType<StockRecord>()` before creating the grid / collection view. Display and edit bindings continue to use CompiledBinding.

```xml
<DataGrid ItemsSource="{Binding Records}" AutoGenerateColumns="False"
          MaxColumnWidth="320" one:DataGridFeatures.ShowColumnFilters="True">
  <DataGrid.Columns>
    <DataGridTextColumn Header="名称"
        Binding="{CompiledBinding Name, DataType={x:Type vm:StockRecord}}" />
    <DataGridTextColumn Header="数量"
        Binding="{CompiledBinding Quantity, DataType={x:Type vm:StockRecord}}" />
  </DataGrid.Columns>
</DataGrid>
```

After OneTheme loads, filterable columns have a funnel entry point, with accent colors for active conditions. Text supports contains matching; numbers support exact values and inclusive ranges. Either range endpoint can be omitted; empty conditions clear the column. Boolean, date, action-template, and unidentified-property columns have no filter button. `FilterEnabled=False` or `FilterKind=None` also hides a column's entry point. Runtime changes update the header and close an unavailable filter popup.

```csharp
var result = DataGridFeatures.ApplyFilter(grid, grid.Columns[0], new TextFilterCondition("显示器"));
if (!result.Succeeded) { /* 显示 result.Error */ }
DataGridFeatures.ApplyFilter(grid, grid.Columns[1],
    new NumericFilterCondition<int>(NumericFilterMode.Range, minimum: 5, maximum: 20));
DataGridFeatures.ClearFilter(grid, grid.Columns[0]);
DataGridFeatures.ClearAllFilters(grid);
```

Conditions combine with AND and with the existing business Filter. Clearing all column conditions does not clear business filtering, sorting, or grouping. Hiding all entry points retains conditions; disabling a column's filtering clears its condition. Operations return DataGridFilterApplyResult: check Succeeded and call on the UI thread.

`FilterChangingEvent` can cancel changes; `FilterChangedEvent` exposes column and old / new conditions. Cancelling any ClearAll change leaves the whole batch unchanged. `FilterCondition` defaults to TwoWay. Columns are not ordinary DataContext containers, so explicitly provide the compiled binding source when binding ViewModel conditions:

```csharp
column.Bind(DataGridColumnOptions.FilterConditionProperty,
    CompiledBinding.Create<StockViewModel, DataGridFilterCondition?>(
        vm => vm.NameCondition, source: model, mode: BindingMode.TwoWay));
```

Incompatible condition bindings restore the last valid value; query errors with `DataGridColumnOptions.GetFilterError(column)`. Popup drafts do not change data. Apply / Enter commits; Esc, cancel, or outside dismissal discards drafts and returns to the entry point. Invalid numbers, reversed ranges, or failed native edit validation retain the old condition.

### Collection views and lifecycle

Native DataGridCollectionView and its [DataGridTreeCollectionView](datagrid-tree.md) derivative support column filtering; native DataGrid wraps ordinary IEnumerable ItemsSource in the flat view too. It exposes edit state and Filter notifications without reflecting into internal editable interfaces. Custom IDataGridCollectionView does not yet support column filters; entry points remain disabled with a reason.

Only one grid can manage active column conditions on a CollectionView at a time. For independent filtering, create one CollectionView per grid over the same model collection. Later business Filter replacements are preserved. On unload, the behavior removes only its own predicate; restoration waits for active editing to finish, and callbacks do not retain unloaded UI.

Item INotifyPropertyChanged, source additions / removals, and source replacement coalesce filtering updates, including unrealized models. Without active column conditions, filter-specific row subscriptions are removed. Failed field reads reject application with an error; display strings and implicit conversion do not replace raw values. Automatic access uses Avalonia's generated compiled-path getter, ignoring display Converter / StringFormat without name reflection or row / probe controls. Nested paths support intermediate null, child notifications, and replacement. Replacing Binding identifies the field again.

Custom HeaderTemplate content keeps native contracts. Fully replacing ColumnHeaderTheme / ControlTemplate requires embedding DataGridFilterButton. It gets column identity through public GetColumnContainingElement rather than matching header text.

## Properties

DataGridFeatures.ShowColumnFilters defaults to false. DataGridColumnOptions configures columns:

| Property | Type / default | Contract |
| --- | --- | --- |
| `ValueAccessor` | IDataGridValueAccessor? / null | Optional override; ordinary bound columns infer it, while template columns or computed fields can supply a typed delegate |
| `FilterEnabled` | bool / true | Disable an individual entry point after grid filtering is enabled; disabling filtering clears that column's condition |
| `FilterKind` | DataGridFilterKind / Auto | `Auto`, `Text`, `Number`, `None`; Auto uses ValueAccessor's raw field type |
| `FilterCondition` | DataGridFilterCondition? / null | Applied immutable condition; TwoWay supported; null means none; popup drafts are separate |
| `FilterError` | string? / null | Read-only phase-2 query property describing condition / field errors; read with GetFilterError |


Automatic identification applies to compiled property chains targeting the current entity. Template columns with multiple fields do not infer identity; override ValueAccessor or disable FilterEnabled. Supported numeric types are byte / sbyte / short / ushort / int / uint / long / ulong / float / double / decimal and Nullable. Parsing uses the raw type and CurrentCulture. Exact comparison adds no floating tolerance; null / NaN / Infinity do not match valid conditions.

Use `--verify-datagrid-filters` and render target DataGridFilters; see [preview entry points](../../../design/PreviewHost/Program.cs). Native layout package consumption is demonstrated by [DataGridPackageConsumer](../../../design/PreviewHost/Consumers/DataGridPackageConsumer/Program.cs).

## Native right frozen columns

```xml
<DataGrid ItemsSource="{CompiledBinding Records}" AutoGenerateColumns="False"
          MaxColumnWidth="320" FrozenColumnCount="1"
          one:DataGridFeatures.FrozenRightColumnCount="1">
  <!-- 显式编译绑定列；最后一个可见列进入右侧区。 -->
</DataGrid>
```

FrozenRightColumnCount is an int defaulting to 0, supporting bindings and Style; C# uses GetFrozenRightColumnCount / SetFrozenRightColumnCount. The last N visible columns by DisplayIndex form the right region. Native FrozenColumnCount controls the left; the center scrolls and clips independently. Native IsFrozen is true in both frozen regions and does not change with scrolling.

Negative values, counts beyond existing visible columns, or left / right overlap are rejected, retaining the old value. XAML can set counts before columns exist. If hiding / removal leaves too few columns, the effective right region shrinks to remaining non-left-frozen visible columns while retaining the configured count; restoration recalculates it. Very narrow windows prioritize the left, clip the right from its left edge, and can reduce the center to zero width; widening restores layout.

Pointer reordering stays within a frozen region; center columns cannot drag into frozen areas. Programmatic DisplayIndex changes recalculate the right region by new trailing positions. Right-column ScrollIntoView preserves horizontal offset; center scrolling excludes both frozen widths. Column width, auto row height, native editing / sorting / grouping, filtering, and recycling retain native paths.

This requires native layout extensions. The library references the [12.1.2 source project](../../../vendor/Avalonia.Controls.DataGrid/README.md), preserving native assembly identity, types, presenters, and PART_* while replacing official DataGrid NuGet. Consumers must not add the official package too; upgrades must maintain this local patch. Core Avalonia remains 12.1.4.

PreviewHost's DataGrid page changes right frozen count, default 0. `--verify-datagrid-right` checks right freezing, existing filters, and native left-only behavior; DataGridRightFrozen is the render target. See [preview entry points](../../../design/PreviewHost/Program.cs) for commands and platform context.

### Column drag feedback

Dragging preserves native drop targets and frozen-region limits. The source header fades; an independent card represents the drag without duplicate action buttons, and insertion hints ease briefly. The drag keeps the initial grab position. Esc, release outside header vertical bounds, or unloading cancels and clears capture. `Motion.Enabled=False` or zero `One.Motion.Fast` disables insertion animation. See [DataGridPackageConsumer](../../../design/PreviewHost/Consumers/DataGridPackageConsumer/Program.cs).

## Numeric summaries



```xml
<DataGrid ItemsSource="{CompiledBinding Records}" AutoGenerateColumns="False"
          MaxColumnWidth="320" one:DataGridFeatures.ShowColumnFilters="True"
          one:DataGridFeatures.ShowSummaryRow="True"
          one:DataGridFeatures.SummaryScope="FilteredItems">
  <DataGrid.Columns>
    <DataGridTextColumn Header="数量" Binding="{CompiledBinding Quantity, DataType={x:Type vm:StockRecord}}"
        one:DataGridColumnOptions.SummaryEnabled="True"
        one:DataGridColumnOptions.SummaryKind="Sum"
        one:DataGridColumnOptions.SummaryFormat="N0" />
    <DataGridTextColumn Header="单价" Binding="{CompiledBinding Price, DataType={x:Type vm:StockRecord}}"
        one:DataGridColumnOptions.SummaryEnabled="True"
        one:DataGridColumnOptions.SummaryKind="Average"
        one:DataGridColumnOptions.SummaryFormat="0.00" />
  </DataGrid.Columns>
</DataGrid>
```

| Setting | Type / default | Behavior |
| --- | --- | --- |
| DataGridFeatures.ShowSummaryRow | bool / false | Separate summary row; disabling stops calculation / subscriptions but retains column settings |
| SummaryScope | DataGridSummaryScope / FilteredItems | Locally loaded models after business / column filters and before paging; SourceItems ignores filters, VisibleItems enumerates the current view; tree FilteredItems counts direct matches only |
| SummaryLeavesOnly | bool / false | Only structural tree leaves, preventing parent/child double counting; ordinary rows are all leaves |
| SummaryLabel | string / 合计 | Default label means total; shown in the native row-header slot when visible, without replacing the first result |
| SummaryEmptyText | string / — | Sum / Average / Min / Max text when no numeric values exist; Count is 0 |
| DataGridColumnOptions.SummaryEnabled | bool / false | Explicitly enable the column; numeric or nullable numeric properties only |
| SummaryKind | DataGridSummaryKind / Sum | Sum, Average, Min, Max, Count; one per column |
| SummaryFormat | string? / null | .NET numeric format, independent of display Converter, using the view Culture |

All 11 basic numeric types and Nullable are supported. null, NaN, and Infinity are excluded from every aggregate; Count includes only valid values. Integer / decimal sums and averages accumulate in decimal; floating values use double. Min / Max retain original types; Count returns long. Overflow, failed reads, nonnumeric configuration, and format errors are reported through result Error and display ! instead of silently showing infinity or wrong numbers.

`DataGridFeatures.GetSummaryResult(grid, column)` returns Value, ValidCount, and Error. SummaryUpdatedEvent publishes coalesced updates. Filtering, editing, unrealized-row PropertyChanged, collection changes / replacement, and nested-object updates recalculate results. Hidden columns stop calculation / listening and recalculate when restored. Summaries do not participate in selection, editing, sorting, or ItemsSource. Widths, reordering, scrolling, and frozen regions share native layout descriptions, retaining narrow-window clipping rules.

Only locally loaded models are summarized, not unloaded server data. Fully custom DataGrid templates must embed DataGridSummaryPresenter; OneTheme already includes it. PreviewHost toggles summaries. `--verify-datagrid-summary` checks aggregates, automatic filters, frozen regions, and native behavior; render targets are DataGridSummary / DataGridFilters. See [preview entry points](../../../design/PreviewHost/Program.cs).
