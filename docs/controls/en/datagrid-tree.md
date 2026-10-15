# DataGrid tree view prototype

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

```bash
dotnet build design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/datagrid-tree-stage2/artifacts
dotnet .preview/datagrid-tree-stage2/artifacts/bin/PreviewHost/debug/PreviewHost.dll --verify-datagrid-tree-query .preview/datagrid-tree-stage2/frames
dotnet .preview/datagrid-tree-stage2/artifacts/bin/PreviewHost/debug/PreviewHost.dll --verify-datagrid-tree
dotnet .preview/datagrid-tree-stage2/artifacts/bin/PreviewHost/debug/PreviewHost.dll --datagrid-tree
```

The query suite covers stable sibling sorting, deferred multi-sort, nested notifications, failure rollback, actual headers/filter popups, all summary scopes, leaves, native editing during active queries and expansion automation. Stage-one tree checks stay separate. Rendering covers both themes, three sizes and wide/narrow viewports. Run only DataGrid entry points affected by the change.

```bash
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 --artifacts-path .preview/datagrid-tree-stage2/aot-artifacts -o .preview/datagrid-tree-stage2/native
.preview/datagrid-tree-stage2/native/NativeAotSmoke --verify-datagrid-tree-query --require-aot
```

This is the native consumer entry point for the current macOS host; it does not establish Windows/Linux runtime coverage. Existing ordinary-grid regression entry points remain in [Program.cs](../../../design/PreviewHost/Program.cs).
