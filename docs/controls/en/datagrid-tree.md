# DataGrid tree view prototype

**English** · [简体中文](../zh-cn/datagrid-tree.md)

Stage-one prototype: local tree expansion and collapse reuse native DataGrid cells, row recycling, editing transactions and currency. These APIs are provisional.

```csharp
DataGridAot.PreserveRowType<Node>();
var tree = new DataGridTreeCollectionView<Node>(roots, node => node.Children);
// DataGridTreeCollectionView belongs to Avalonia.Collections.
```

```xml
<DataGrid ItemsSource="{Binding Tree}" AutoGenerateColumns="False"
          one:DataGridTreeFeatures.IsEnabled="True">
  <DataGrid.Columns>
    <one:DataGridTreeTextColumn Header="Name"
        Binding="{CompiledBinding Name, DataType={x:Type vm:Node}, Mode=TwoWay}" />
  </DataGrid.Columns>
</DataGrid>
```

Rows remain the original models, without an `Item` wrapper. The generic view supplies the compiled row type and explicitly registers its collection shape, including empty sources. Register concrete row and nested property types before creating the view; see [NativeAOT](datagrid-binding.md#nativeaot). A typed delegate reads Children without property-name reflection. Bind each tree view to only one attached grid; create separate views for independent state.

## Stage-one behavior

- Initially collapsed. `TrySetExpanded(node, true/false)` returns success; `TryGetNodeState` reports depth, parent, children and expansion state.
- A local, homogeneous tree must contain non-null reference-type nodes, without cycles or shared model instances. Identity uses references. The index covers all loaded nodes; only viewport rows create native UI containers.
- Collapse retains descendant expansion state. Re-expansion restores it. Projection changes publish single-item additions/removals, without replacing ItemsSource, range events or Reset.
- With `DataGridTreeFeatures.IsEnabled`, changes first commit the native row edit. Validation failure cancels the change and retains the editor. A standalone view refuses expansion changes while editing.
- Collapsing a selected descendant moves single selection and currency to the collapsed node. Multiple selection drops hidden descendants and retains unrelated selections. Expansion does not restore hidden selections.
- Outside editing, Right expands or enters the first child in the tree column; Left collapses or selects the parent. Editors and other columns retain native navigation.
- Root and child `INotifyCollectionChanged` collections remain observed when collapsed. Model notifications detect replacement child collections. Call `RefreshTree()` after changing plain collections. Structural source changes during an unfinished edit keep the old projection and refresh after editing ends.
- `TreeChanging` is cancellable; `TreeChanged` follows successful application. Item/Expanded are null for source refreshes. Invalid sources are rejected before publishing a new index; the caller must repair its source.
- The consumer owns the view: detach/close the grid, finish editing, then `Dispose()` to remove source and model subscriptions. Use the UI thread for all operations.

## Limitations

Sorting, column/business filtering, grouping, paging, AddNew and view-based removal are disabled. Modify business root/child collections to add or remove nodes. `SourceCollection` is the read-only visible projection, not the entire tree; do not enable existing summary rows yet. Tree-aware filtering, sibling sorting, complete summary scopes, async loading, drag-and-drop and full accessibility expansion patterns are outside this stage.

Text editing retains the native TextBox as the binding target; row cancellation follows the native `IEditableObject` contract. `Indent` defaults to 16 and accepts finite non-negative values. Configure it when creating the column in this prototype.

## PreviewHost verification

```bash
dotnet build TheOne.Avalonia.slnx
dotnet run --project design/PreviewHost --no-build -- --verify-datagrid-tree .preview/datagrid-tree/frames
dotnet run --project design/PreviewHost --no-build -- --datagrid-tree
```

The suite covers source updates, cycle/shared-instance rejection, compiled binding, pointer/keyboard expansion, commit/cancel/validation, single/multiple selection, recycling, both themes, three sizes and narrow viewport screenshots.

```bash
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 -o .preview/datagrid-tree/native
.preview/datagrid-tree/native/NativeAotSmoke --verify-datagrid-tree --require-aot
```

This is the native consumer entry point for the current macOS host; it does not establish Windows/Linux runtime coverage. Existing ordinary-grid regression entry points remain in [Program.cs](../../../design/PreviewHost/Program.cs).
