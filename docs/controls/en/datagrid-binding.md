# DataGrid binding, sorting, and cell density

[简体中文](../zh-cn/datagrid-binding.md) · **English**

DataGridColumnHeader, DataGridRowHeader, and DataGridRowGroupHeader are native header / grouping template structures, already themed by OneTheme. Prefer public extension points such as HeaderTemplate. Complete replacement templates must preserve PART_*, column identity, and native layout contracts; do not create these containers separately.

Declare the ViewModel x:DataType on the view root and enable compiled bindings by default. When ItemsSource binds List<Record>, ObservableCollection<Record>, or IEnumerable<Record>, native Binding / CellTemplate / CellEditingTemplate metadata infers Record. Columns and cell templates need not repeat x:DataType. Runtime DataContext assignments cannot provide static types; object or non-generic IEnumerable sources cannot infer them either.

For native filtering, sorting, and grouping, use a generic collection view:

```csharp
using Avalonia.Collections;
using TheOne.Avalonia.Controls;
// 在创建视图前注册；托管和原生使用同一元数据路径。
_ = DataGridAot.PreserveRowType<Record>();
var view = new DataGridCollectionView<Record>(records);
```

```xml
<DataGrid ItemsSource="{Binding View}" AutoGenerateColumns="False" MaxColumnWidth="320">
  <DataGrid.Columns>
    <DataGridTextColumn Header="名称" Binding="{Binding Name, Mode=TwoWay}" />
    <DataGridCheckBoxColumn Header="启用" Binding="{Binding Enabled, Mode=TwoWay}" />
  </DataGrid.Columns>
</DataGrid>
```

DataGridCollectionView<T> keeps native implementation and enumeration order. T supplies only the row type; data is not copied and controls are not prebuilt. ObservableCollection<T> changes notify the native view. List<T> has no change notifications; use ObservableCollection<T> or explicitly refresh for dynamic updates. Managed and NativeAOT callers register DataGridAot.PreserveRowType<Record>() before creating views. Register nested models and custom comparison values separately. Empty custom collections need RegisterCollection<TCollection, Record>(); see [NativeAOT](#nativeaot) below.

Every column header includes DataGridSortButton: unsorted shows symmetric arrows; ascending / descending uses an accent direction mark. Click, Enter, or Space cycles unsorted → ascending → descending → unsorted, retaining native edit commit and Sorting events. Shift keeps existing sorts and adds a column; activation after descending removes only that column. Multicolumn sorting shows one-based priorities inside buttons, preserves order on direction changes, and renumbers on removal. Platform Ctrl / ⌘ clears all sorts; combined with Shift, it clears only the current column.

CanUserSortColumns and column CanUserSort control availability. Native header clicks, dragging, and resizing remain. CycleSort(KeyModifiers) is the three-state entry point; Sort(), Sort(KeyModifiers), and Sort(ListSortDirection) retain native behavior. Column SortDirection / SortPriority come from the view; button Direction / SortPriority are read-only DirectProperty values supporting compiled bindings. External sort-collection changes and source replacement update indicators. Unsorted priority is 0; a single sort has no number. CanUserSortChanged notifies dynamic availability changes.

The DataGrid preview ViewModel and GridRecord inherit ReactiveObject directly and use .NET 10 field properties with RaiseAndSetIfChanged, without extra notification wrappers. Row-edit cancellation, validation, and derived properties for selection remain supported.

Centered text uses the full content width, avoiding clipping to stale measurements after sorting / recycling while retaining other controls' alignment. Small cells reserve full height for fixed 20 px checkmarks. Button / ToggleButton / RepeatButton use cell-density padding and minimum heights. Auto row height follows content without shrinking fonts or forced truncation. Consumers must adjust explicitly undersized RowHeight.

PreviewHost's DataGrid page has a separate collection-operation area and real thousand-item recycling checks. Use --verify-datagrid-binding and the DataGridBinding render target. No comparison page was added.

[Filtering, frozen columns, and summaries](datagrid-extensions.md).

## NativeAOT

Library templates use TemplateBinding or compiled bindings. Declare correct `x:DataType` on consumer views / data templates; code-behind DataContext assignment cannot provide static types. Non-generic collection-view columns need explicit row types, and nested binding scopes need their model type. Do not bypass checks by disabling compiled binding or using ReflectionBinding.

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  <IsAotCompatible>true</IsAotCompatible>
  <!-- 仅可执行应用设置；类库不设置 PublishAot。 -->
  <PublishAot>true</PublishAot>
</PropertyGroup>
```

`DataGridAot.PreserveRowType<T>()` registers concrete row types before views / grids are created. It preserves only that type's public instance properties, accessors, and public parameterless constructor, without scanning or retaining entire assemblies. Rows without that constructor still display and edit but do not support AddNew. Register each model along nested paths and custom `IComparable<T>` field types. Sorting uses compile-time-instantiated `Comparer<T>.Default`.

Automatic columns use compiled accessors generated from registered properties. Basic numeric, nullable, date/time, and Guid values use explicit conversion APIs; invalid formats and overflow produce binding errors. Enums, custom TypeConverter, Parse, and implicit conversions require explicit column Converter settings. Registered properties support single-argument int / string indexers. Filtering and summaries reuse compiled getters.

The registry supplies empty-source row types for `List<T>`, `ObservableCollection<T>`, `T[]`, and `DataGridCollectionView<T>`. Other empty collection shapes use `DataGridRowMetadata.RegisterCollection<MyCollection, Record>()`. Nonempty sources can identify types from the first item, but that does not preserve properties. Unregistered paths fail explicitly rather than silently losing editing or sorting after trimming.

Set `SortMemberPath` explicitly for path sorting and a finite `MaxColumnWidth` before creating columns:

```xml
<DataGrid ItemsSource="{Binding Records}" AutoGenerateColumns="False" MaxColumnWidth="320">
  <DataGrid.Columns>
    <DataGridTextColumn Header="名称" SortMemberPath="Name"
        Binding="{Binding Name, Mode=TwoWay}" />
  </DataGrid.Columns>
</DataGrid>
```

This library replaces the official DataGrid package while preserving native assembly identity, types, and presenters. Do not also reference official `Avalonia.Controls.DataGrid`. Sources and licenses: [vendor README](../../../vendor/Avalonia.Controls.DataGrid/README.md). Independent compiled-binding / native consumption: [NativeAotSmoke](../../../design/PreviewHost/Consumers/NativeAotSmoke/Program.cs). Package-only consumption: [DataGridPackageConsumer](../../../design/PreviewHost/Consumers/DataGridPackageConsumer/Program.cs).
