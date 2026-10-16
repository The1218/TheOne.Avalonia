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

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost's Control Laboratory (top-right action on each page) groups live examples by styles, properties, APIs/events and models; complex models use the complete business samples.

### DataGridCollectionView

[API / model reference](../../api/en/Avalonia.Collections.DataGridCollectionView_T_.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### DataGridColumn

[API / model reference](../../api/en/Avalonia.Controls.DataGridColumn.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`SortPriority`](../../../vendor/Avalonia.Controls.DataGrid/DataGridColumn.cs#L743) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Gets the one-based priority, or zero if this column is not sorted. |

### DataGridRowMetadata

[API / model reference](../../api/en/Avalonia.Controls.DataGridRowMetadata.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### DataGridAot

[API / model reference](../../api/en/TheOne.Avalonia.Controls.DataGridAot.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### DataGridColumnOptions

[API / model reference](../../api/en/TheOne.Avalonia.Controls.DataGridColumnOptions.md)

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

### DataGridSortButton

[API / model reference](../../api/en/TheOne.Avalonia.Controls.DataGridSortButton.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Direction`](../../../TheOne.Avalonia/Controls/DataGridSortButton.cs#L40) | `System.ComponentModel.ListSortDirection?` | read-only; OneWay | `computed / state` | Reveal/port direction enum; choices are defined by its concrete enum type. Values: `Ascending`, `Descending` |
| [`SortPriority`](../../../TheOne.Avalonia/Controls/DataGridSortButton.cs#L41) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### DataGridValueAccessor

[API / model reference](../../api/en/TheOne.Avalonia.Controls.DataGridValueAccessor.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### Shared attached properties

[ControlSizing](../../api/en/TheOne.Avalonia.Styling.ControlSizing.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | writable; OneWay; attached; inherits | `ControlSize.Normal` | Shared Small/Normal/Large density inherited by descendants; explicit local values/classes can override it. Values: `Small`, `Normal`, `Large` |

[Motion](../../api/en/TheOne.Avalonia.Styling.Motion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | writable; OneWay; attached; inherits | `true` | Inherited motion switch; disabling goes to final layout/state while preserving native input. |

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| DataGrid | `borderless` | explicit Classes; combine with density | ListBox.borderless, DataGrid.borderless => BorderThickness=0 |
| DataGrid | `one-large` | automatic template class; do not bind manually | DataGrid.one-large => FontSize=16 |
| DataGrid | `one-normal` | automatic template class; do not bind manually | DataGrid.one-normal => FontSize=14 |
| DataGrid | `one-small` | automatic template class; do not bind manually | DataGrid.one-small => FontSize=12 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`Avalonia.Collections.DataGridCollectionView<T>`](../../api/en/Avalonia.Collections.DataGridCollectionView_T_.md) | Class | 0 | 2 | 0 |
| [`Avalonia.Controls.DataGridColumn`](../../api/en/Avalonia.Controls.DataGridColumn.md) | Class | 1 | 1 | 0 |
| [`Avalonia.Controls.DataGridRowMetadata`](../../api/en/Avalonia.Controls.DataGridRowMetadata.md) | Class | 0 | 2 | 0 |
| [`TheOne.Avalonia.Controls.DataGridAot`](../../api/en/TheOne.Avalonia.Controls.DataGridAot.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridColumnOptions`](../../api/en/TheOne.Avalonia.Controls.DataGridColumnOptions.md) | Class | 8 | 16 | 0 |
| [`TheOne.Avalonia.Controls.DataGridFilterCondition`](../../api/en/TheOne.Avalonia.Controls.DataGridFilterCondition.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridFilterKind`](../../api/en/TheOne.Avalonia.Controls.DataGridFilterKind.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridSortButton`](../../api/en/TheOne.Avalonia.Controls.DataGridSortButton.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridSummaryKind`](../../api/en/TheOne.Avalonia.Controls.DataGridSummaryKind.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridValueAccessor`](../../api/en/TheOne.Avalonia.Controls.DataGridValueAccessor.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.IDataGridValueAccessor`](../../api/en/TheOne.Avalonia.Controls.IDataGridValueAccessor.md) | Interface | 1 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
