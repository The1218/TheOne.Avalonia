# DataGrid tree view prototype

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

**English** · [简体中文](../zh-cn/datagrid-tree.md)

Stage-two prototype: sibling sorting, ancestor-preserving filtering and tree-aware summaries reuse native DataGrid editing, selection and recycling. These APIs are provisional.

```csharp
DataGridAot.PreserveRowType<Node>();
var tree = new DataGridTreeCollectionView<Node>(roots, node => node.Children);
// DataGridTreeCollectionView belongs to Avalonia.Collections.
```

```xml
<DataGrid ItemsSource="{Binding Tree}" AutoGenerateColumns="False"
          one:DataGridTreeFeatures.IsEnabled="True"
          one:DataGridFeatures.ShowColumnFilters="True"
          one:DataGridFeatures.ShowSummaryRow="True"
          one:DataGridFeatures.SummaryScope="FilteredItems"
          one:DataGridFeatures.SummaryLeavesOnly="True">
  <DataGrid.Columns>
    <one:DataGridTreeTextColumn Header="Name"
        Binding="{CompiledBinding Name, DataType={x:Type vm:Node}, Mode=TwoWay}" />
    <DataGridTextColumn Header="Quantity"
        Binding="{CompiledBinding Quantity, DataType={x:Type vm:Node}}"
        one:DataGridColumnOptions.SummaryEnabled="True" />
  </DataGrid.Columns>
</DataGrid>
```

Rows remain the original models, without an `Item` wrapper. The generic view supplies the compiled row type and explicitly registers its collection shape, including empty sources. Register concrete row and nested property types before creating the view; see [NativeAOT](datagrid-binding.md#nativeaot). A typed delegate reads Children without property-name reflection. Bind each tree view to only one attached grid; create separate views for independent state.

## Tree and editing contract

- Initially collapsed. `TrySetExpanded(node, true/false)` returns success; `TryGetNodeState` reports depth, parent, filtered children and effective expansion state, with `IsFilterMatch` / `IsForcedExpanded`.
- A local, homogeneous tree must contain non-null reference-type nodes, without cycles or shared model instances. Identity uses references. The index covers all loaded nodes; only viewport rows create native UI containers.
- Collapse retains descendant expansion state. Re-expansion restores it. Projection changes publish single-item additions/removals, without replacing ItemsSource, range events or Reset.
- With `DataGridTreeFeatures.IsEnabled`, changes first commit the native row edit. Validation failure cancels the change and retains the editor. A standalone view refuses expansion changes while editing.
- Collapsing a selected descendant moves single selection and currency to the collapsed node. Multiple selection drops hidden descendants and retains unrelated selections. Expansion does not restore hidden selections.
- Outside editing, Right expands or enters the first child in the tree column; Left collapses or selects the parent. Editors and other columns retain native navigation.
- Root and child `INotifyCollectionChanged` collections remain observed when collapsed. Model notifications detect replacement child collections. Call `RefreshTree()` after changing plain collections. Structural source changes during an unfinished edit keep the old projection and refresh after editing ends.
- `TreeChanging` is cancellable; `TreeChanged` follows successful application. Item/Expanded are null for source refreshes and query changes. `LoadedItemsChanged` reports structural changes without firing again for expansion or query updates. Invalid sources are rejected before publishing a new index; the caller must repair its source.
- The consumer owns the view: detach/close the grid, finish editing and query deferrals, then `Dispose()` to remove source and model subscriptions. Use the UI thread for all operations.

## Sibling sorting and filtering

Native headers retain three-state sorting, Shift multi-sort priorities and Ctrl/Command clearing. Roots and each sibling collection sort stably and independently, with every parent preceding its subtree. Clearing sorting restores each collection's original order without modifying business collections. Use `using (tree.DeferRefresh())` to publish multiple descriptions as one projection. Invalid sort paths are rejected before inserting a description. Sorting reuses native registered metadata and comparers, including nested paths and nested notifications/replacement.

`tree.Filter` is the direct-match predicate. Existing column filters use `DataGridFeatures.ApplyFilter` and combine with business predicates using AND. Queries evaluate all loaded nodes, including collapsed branches. Matches and their ancestors remain visible; a parent match does not automatically match children. Matching paths are temporarily forced open and cannot collapse until the filter clears, restoring the user's expansion state. `IsFilterMatch(item)` distinguishes direct matches from context ancestors.

Pending native edits remain in place until commit, then sort/filter again. Invalid edits block header queries. Standalone views require completed edits before changing predicates or sorting. A throwing predicate setter preserves the previous predicate; failed/cancelled sorts restore the previous descriptions and report `QueryError`. Failed live queries retain the last valid projection; the next successful refresh clears the error. Arbitrary business delegates cannot reveal their dependencies: declared column bindings and sort paths observe nested objects; call `RefreshTree()` when other external dependencies change.

## Tree summaries

| Setting | Scope |
| --- | --- |
| `SummaryScope=SourceItems` | All loaded nodes, ignoring filtering and collapse |
| `SummaryScope=FilteredItems` (default) | Direct business/column matches, excluding context-only ancestors and independent of collapse |
| `SummaryScope=VisibleItems` | Currently expanded visible nodes, including context ancestors; changes with collapse |
| `SummaryLeavesOnly=True` (default false) | Excludes structural parents within the selected scope, preventing parent totals being added to their children again |

Existing `SummaryEnabled` / `SummaryKind` / `SummaryFormat` and compiled getters still apply. Leaves refer to the underlying tree: hiding every child through filtering does not turn a parent into a leaf. Every ordinary flat-grid row is treated as a leaf; flat `VisibleItems` uses the native view's current enumeration, including paging.

`SourceCollection` remains the read-only visible projection used by the native grid, not the entire tree. Use `LoadedItems` for business enumeration through the separate optional `IDataGridTreeQueryView` contract; existing tree-presentation implementations gain no required members. Tree text cells expose expansion automation, names and hierarchy help text, including state-change notifications. Native row/column automation structure remains intact. Provider tests do not establish screen-reader runtime coverage.

## Limitations

Grouping, paging, AddNew, view-based removal, async loading and cross-parent drag/drop remain unsupported. Modify business root/child collections instead. All loaded models have index entries and required subscriptions; row virtualization is not data virtualization. Use `RefreshTree()` for plain collections/external query dependencies rather than inherited native `Refresh()`.

Text editing retains the native TextBox as the binding target; row cancellation follows the native `IEditableObject` contract. `Indent` defaults to 16 and accepts finite non-negative values. Configure it when creating the column in this prototype.

## PreviewHost verification

Start PreviewHost normally and open **数据展示 → DataGridTree**, or search for `DataGridTree`. The page reuses the tree grid verification fixture and preserves expansion and selection when switching pages.

The query suite covers stable sibling sorting, deferred multi-sort, nested notifications, failure rollback, actual headers/filter popups, all summary scopes, leaves, native editing during active queries and expansion automation. Stage-one tree checks stay separate. Rendering covers both themes, three sizes and wide/narrow viewports. Run only DataGrid entry points affected by the change.

Historical macOS native-consumer results do not establish Windows/Linux runtime coverage; validate the consuming application on its target platform.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### DataGridTreeChangingEventArgs

[Complete source contract](../../api/en/Avalonia.Collections.DataGridTreeChangingEventArgs.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Expanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L26) | `bool?` | read-only; CLR / n/a | `expanded` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Item`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L25) | `object?` | read-only; CLR / n/a | `item` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### DataGridTreeCollectionView

[Complete source contract](../../api/en/Avalonia.Collections.DataGridTreeCollectionView_T_.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanAddNew`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L83) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanChangePage`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L89) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanFilter`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L86) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanGroup`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L88) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanRemove`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L84) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanSort`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L85) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Filter`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L57) | `System.Func<object, bool>` | writable; CLR / n/a | `accessor-defined / state` | Direct match predicate. Ancestors of matches remain visible without matching this predicate. |
| [`LoadedItems`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L103) | `System.Collections.IEnumerable` | read-only; CLR / n/a | `accessor-defined / state` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`PageSize`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L91) | `int` | writable; CLR / n/a | `accessor-defined / state` | Rows per page; hierarchical views do not support paging and enforce their own restrictions. |
| [`QueryError`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L24) | `string?` | read-only; CLR / n/a | `accessor-defined / state` | Failure of a live query or sort change. The last valid projection is retained. |
| [`SortDescriptions`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L26) | `Avalonia.Collections.DataGridSortDescriptionCollection` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### DataGridTreeNodeState

[Complete source contract](../../api/en/Avalonia.Collections.DataGridTreeNodeState.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Depth`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L16) | `int` | writable; CLR / init-only | `required constructor argument` | Node depth starting at zero for roots, used to calculate indentation. |
| [`HasChildren`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L16) | `bool` | writable; CLR / init-only | `required constructor argument` | Whether a node has children, controlling expansion affordances independently of current expansion. |
| [`IsExpanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L16) | `bool` | writable; CLR / init-only | `required constructor argument` | Current node expansion; bind to a stable model when it must survive recycling or reload. |
| [`IsFilterMatch`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L19) | `bool` | writable; CLR / init-only | `false` | Whether the node directly matches the tree filter, distinct from retaining ancestors. |
| [`IsForcedExpanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L18) | `bool` | writable; CLR / init-only | `false` | Whether filtering forces expansion to reveal matching descendants, distinct from saved user expansion. |
| [`Parent`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L16) | `object?` | writable; CLR / init-only | `required constructor argument` | Direct parent model of a tree node; null for roots, preserving original identity. |

### IDataGridTreeCollectionView

[Complete source contract](../../api/en/Avalonia.Collections.IDataGridTreeCollectionView.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### IDataGridTreeQueryView

[Complete source contract](../../api/en/Avalonia.Collections.IDataGridTreeQueryView.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`LoadedItems`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L41) | `System.Collections.IEnumerable` | read-only; CLR / n/a | `accessor-defined / state` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |

### DataGridColumnOptions

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.DataGridColumnOptions.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`FilterCondition`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L21) | `TheOne.Avalonia.Controls.DataGridFilterCondition?` | writable; TwoWay; attached | `null` | DataGridFilterCondition? / null ; Applied immutable condition; TwoWay supported; null means none; popup drafts are separate |
| [`FilterEnabled`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L15) | `bool` | writable; OneWay; attached | `true` | bool / true ; Disable an individual entry point after grid filtering is enabled; disabling filtering clears that column's condition |
| [`FilterError`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L25) | `string?` | read-only; OneWay; attached | `null` | string? / null ; Read-only phase-2 query property describing condition / field errors; read with GetFilterError |
| [`FilterKind`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L18) | `TheOne.Avalonia.Controls.DataGridFilterKind` | writable; OneWay; attached | `0 / default` | DataGridFilterKind / Auto ; `Auto`, `Text`, `Number`, `None`; Auto uses ValueAccessor's raw field type Values: `Auto`, `Text`, `Number`, `None` |
| [`SummaryEnabled`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L28) | `bool` | writable; OneWay; attached | `false` | bool / false ; Explicitly enable the column; numeric or nullable numeric properties only |
| [`SummaryFormat`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L35) | `string?` | writable; OneWay; attached | `null` | string? / null ; .NET numeric format, independent of display Converter, using the view Culture |
| [`SummaryKind`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L31) | `TheOne.Avalonia.Controls.DataGridSummaryKind` | writable; OneWay; attached | `0 / default` | DataGridSummaryKind / Sum ; Sum, Average, Min, Max, Count; one per column Validation: `Enum.IsDefined` Values: `Sum`, `Average`, `Min`, `Max`, `Count` |
| [`ValueAccessor`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L11) | `TheOne.Avalonia.Controls.IDataGridValueAccessor?` | writable; OneWay; attached | `null` | IDataGridValueAccessor? / null ; Optional override; ordinary bound columns infer it, while template columns or computed fields can supply a typed delegate |

### DataGridFeatures

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.DataGridFeatures.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`FrozenRightColumnCount`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L35) | `int` | writable; OneWay; attached | `0 / default` | Number of frozen columns on the right, sharing available width with left-frozen columns. Validation: `value => value >= 0` |
| [`ShowColumnFilters`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L38) | `bool` | writable; OneWay; attached | `false` | Enables column-filter entry points; each column must also permit filtering. |
| [`ShowSummaryRow`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L47) | `bool` | writable; OneWay; attached | `false` | bool / false ; Separate summary row; disabling stops calculation / subscriptions but retains column settings |
| [`SummaryEmptyText`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L60) | `string` | writable; OneWay; attached | `"—"` | string / — ; Sum / Average / Min / Max text when no numeric values exist; Count is 0 |
| [`SummaryLabel`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L57) | `string` | writable; OneWay; attached | `"合计"` | string / 合计 ; Default label means total; shown in the native row-header slot when visible, without replacing the first result |
| [`SummaryLeavesOnly`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L54) | `bool` | writable; OneWay; attached | `false` | bool / false ; Only structural tree leaves, preventing parent/child double counting; ordinary rows are all leaves |
| [`SummaryScope`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L50) | `TheOne.Avalonia.Controls.DataGridSummaryScope` | writable; OneWay; attached | `0 / default` | DataGridSummaryScope / FilteredItems ; Locally loaded models after business / column filters and before paging; SourceItems ignores filters, VisibleItems enumerates the current view; tree FilteredItems counts direct matches only Validation: `Enum.IsDefined` Values: `FilteredItems`, `SourceItems`, `VisibleItems` |

### DataGridTreeFeatures

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.DataGridTreeFeatures.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L13) | `bool` | writable; OneWay; attached | `false` | Business availability; selection, removal and commands follow the control's disabled-item rules. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

- `DataGrid`: `Classes="borderless"`
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`DataGridTreeCollectionView`](../../api/en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) | `override IDisposable DataGridTreeCollectionView<T>.DeferRefresh()` | Defers collection refresh to batch changes; dispose the returned token at the end of a using scope. |
| [`DataGridTreeCollectionView`](../../api/en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) | `bool DataGridTreeCollectionView<T>.IsFilterMatch(object item)` | Checks whether an original node directly matches the current filter. |
| [`DataGridTreeCollectionView`](../../api/en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) | `bool DataGridTreeCollectionView<T>.IsLeaf(object item)` | Checks whether an original node is a leaf with no children. |
| [`DataGridTreeCollectionView`](../../api/en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) | `bool DataGridTreeCollectionView<T>.TryGetNodeState(object item, out DataGridTreeNodeState state)` | Attempts to read hierarchy/parent/expansion state; false means the node was not found. |
| [`DataGridTreeCollectionView`](../../api/en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) | `bool DataGridTreeCollectionView<T>.TrySetExpanded(object item, bool expanded)` | Attempts to change original-node expansion; the result indicates success under tree-state restrictions. |
| [`DataGridTreeCollectionView`](../../api/en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) | `bool DataGridTreeCollectionView<T>.RefreshTree()` | Refreshes plain collections or reconnects replaced child collections. Returns false when an edit or a cancellable presentation change prevents refresh. |
| [`IDataGridTreeCollectionView`](../../api/en/Avalonia.Collections.IDataGridTreeCollectionView.md) | `bool IDataGridTreeCollectionView.TryGetNodeState(object item, out DataGridTreeNodeState state)` | Attempts to read hierarchy/parent/expansion state; false means the node was not found. |
| [`IDataGridTreeCollectionView`](../../api/en/Avalonia.Collections.IDataGridTreeCollectionView.md) | `bool IDataGridTreeCollectionView.TrySetExpanded(object item, bool expanded)` | Attempts to change original-node expansion; the result indicates success under tree-state restrictions. |
| [`IDataGridTreeQueryView`](../../api/en/Avalonia.Collections.IDataGridTreeQueryView.md) | `bool IDataGridTreeQueryView.IsFilterMatch(object item)` | Checks whether an original node directly matches the current filter. |
| [`IDataGridTreeQueryView`](../../api/en/Avalonia.Collections.IDataGridTreeQueryView.md) | `bool IDataGridTreeQueryView.IsLeaf(object item)` | Checks whether an original node is a leaf with no children. |
| [`DataGridFeatures`](../../api/en/TheOne.Avalonia.Controls.DataGridFeatures.md) | `static DataGridSummaryResult? DataGridFeatures.GetSummaryResult(DataGrid grid, DataGridColumn column)` | Reads current column aggregates, including valid count, result and errors. |
| [`DataGridFeatures`](../../api/en/TheOne.Avalonia.Controls.DataGridFeatures.md) | `static DataGridFilterApplyResult DataGridFeatures.ApplyFilter(DataGrid grid, DataGridColumn column, DataGridFilterCondition? condition)` | Applies a column filter and returns its result; inspect result/filter-error state on failure. |
| [`DataGridFeatures`](../../api/en/TheOne.Avalonia.Controls.DataGridFeatures.md) | `static DataGridFilterApplyResult DataGridFeatures.ClearFilter(DataGrid grid, DataGridColumn column)` | Clears the specified column filter while preserving other conditions. |
| [`DataGridFeatures`](../../api/en/TheOne.Avalonia.Controls.DataGridFeatures.md) | `static DataGridFilterApplyResult DataGridFeatures.ClearAllFilters(DataGrid grid)` | Clears all column filters and refreshes visible rows. |
| [`DataGridFilterCondition`](../../api/en/TheOne.Avalonia.Controls.DataGridFilterCondition.md) | `abstract bool DataGridFilterCondition.Matches(object? value)` | Checks a value against this filter; null/comparison semantics follow the condition type. |
| [`IDataGridValueAccessor`](../../api/en/TheOne.Avalonia.Controls.IDataGridValueAccessor.md) | `object? IDataGridValueAccessor.GetValue(object item)` | Reads an original-row value through an explicit field key/typed getter, without reflection paths. |

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`DataGridTreeCollectionView`](../../api/en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) | `TreeChanging` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`DataGridTreeCollectionView`](../../api/en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) | `TreeChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`DataGridTreeCollectionView`](../../api/en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) | `LoadedItemsChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`IDataGridTreeCollectionView`](../../api/en/Avalonia.Collections.IDataGridTreeCollectionView.md) | `TreeChanging` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`IDataGridTreeCollectionView`](../../api/en/Avalonia.Collections.IDataGridTreeCollectionView.md) | `TreeChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`IDataGridTreeQueryView`](../../api/en/Avalonia.Collections.IDataGridTreeQueryView.md) | `LoadedItemsChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`Avalonia.Collections.DataGridTreeChangingEventArgs`](../../api/en/Avalonia.Collections.DataGridTreeChangingEventArgs.md) | 2 |
| [`Avalonia.Collections.DataGridTreeCollectionView<T>`](../../api/en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) | 11 |
| [`Avalonia.Collections.DataGridTreeNodeState`](../../api/en/Avalonia.Collections.DataGridTreeNodeState.md) | 6 |
| [`Avalonia.Collections.IDataGridTreeQueryView`](../../api/en/Avalonia.Collections.IDataGridTreeQueryView.md) | 1 |
| [`TheOne.Avalonia.Controls.DataGridColumnOptions`](../../api/en/TheOne.Avalonia.Controls.DataGridColumnOptions.md) | 8 |
| [`TheOne.Avalonia.Controls.DataGridFeatures`](../../api/en/TheOne.Avalonia.Controls.DataGridFeatures.md) | 7 |
| [`TheOne.Avalonia.Controls.DataGridFilterApplyResult`](../../api/en/TheOne.Avalonia.Controls.DataGridFilterApplyResult.md) | 2 |
| [`TheOne.Avalonia.Controls.DataGridFilterCondition`](../../api/en/TheOne.Avalonia.Controls.DataGridFilterCondition.md) | 1 |
| [`TheOne.Avalonia.Controls.DataGridFilterKind`](../../api/en/TheOne.Avalonia.Controls.DataGridFilterKind.md) | 4 |
| [`TheOne.Avalonia.Controls.DataGridSummaryKind`](../../api/en/TheOne.Avalonia.Controls.DataGridSummaryKind.md) | 5 |
| [`TheOne.Avalonia.Controls.DataGridSummaryResult`](../../api/en/TheOne.Avalonia.Controls.DataGridSummaryResult.md) | 3 |
| [`TheOne.Avalonia.Controls.DataGridSummaryScope`](../../api/en/TheOne.Avalonia.Controls.DataGridSummaryScope.md) | 3 |
| [`TheOne.Avalonia.Controls.DataGridTreeFeatures`](../../api/en/TheOne.Avalonia.Controls.DataGridTreeFeatures.md) | 1 |
| [`TheOne.Avalonia.Controls.IDataGridValueAccessor`](../../api/en/TheOne.Avalonia.Controls.IDataGridValueAccessor.md) | 1 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
