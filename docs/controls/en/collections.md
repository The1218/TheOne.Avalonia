# Lists, trees, tabs, and grids

[简体中文](../zh-cn/collections.md) · **English**

[Back to control documentation](README.md). Bind views to a ViewModel with `x:DataType`; extensions use `xmlns:one="using:TheOne.Avalonia.Controls"`.

## ItemsControl and ListBox

```xml
<Grid RowDefinitions="Auto,*" Height="420">
  <TextBlock Text="记录" />
  <ListBox Grid.Row="1" ItemsSource="{Binding Records}"
           SelectedItem="{Binding SelectedRecord, Mode=TwoWay}">
    <ListBox.ItemTemplate>
      <DataTemplate x:DataType="vm:Record">
        <TextBlock Text="{Binding Name}" />
      </DataTemplate>
    </ListBox.ItemTemplate>
  </ListBox>
</Grid>
```

`Records` contains models with display text in `Record.Name`. ListBox adds selection / keyboard navigation; use `SelectionMode="Multiple,Toggle"` and native SelectedItems for multiple selection. Use ItemsControl when selection is unnecessary:

```xml
<ItemsControl Height="320" ItemsSource="{Binding Records}">
  <ItemsControl.ItemTemplate>
    <DataTemplate x:DataType="vm:Record">
      <TextBlock Text="{Binding Name}" Margin="8" />
    </DataTemplate>
  </ItemsControl.ItemTemplate>
</ItemsControl>
```

The theme gives ordinary ItemsControl a scrolling viewport, and both default to VirtualizingStackPanel. Lists need bounded visible areas. Avoid large lists in infinite-height outer StackPanel / ScrollViewer and do not replace ItemsPanel with ordinary StackPanel / WrapPanel.

## TreeView

```xml
<TreeView x:Name="ProjectTree" Height="360"
          ItemsSource="{Binding Roots}"
          SelectedItem="{Binding SelectedNode, Mode=TwoWay}">
  <TreeView.Styles>
    <Style Selector="TreeViewItem" x:DataType="vm:TreeNode">
      <Setter Property="IsExpanded" Value="{Binding IsExpanded, Mode=TwoWay}" />
    </Style>
  </TreeView.Styles>
  <TreeView.ItemTemplate>
    <TreeDataTemplate x:DataType="vm:TreeNode" ItemsSource="{Binding Children}">
      <TextBlock Text="{Binding Name}" />
    </TreeDataTemplate>
  </TreeView.ItemTemplate>
</TreeView>
```

Nodes provide `Name`, `Children`, and `IsExpanded`, with change notifications; see [PreviewTreeNode.cs](../../../design/PreviewHost/Models/NavigationLayout/PreviewTreeNode.cs). Nodes need distinct identities even when text repeats. To retain expansion across reloads, restore model IsExpanded by stable IDs and synchronize containers with TwoWay bindings.

Roots and every child level recycle by viewport. The theme enables `Virtualization.Enabled` and adds navigation across unrealized nodes, Home / End, Ctrl focus movement, and Shift range selection. The theme handles expansion motion; disable Motion per subtree if needed.

On the UI thread after attachment, locate distant nodes with zero-based index paths:

```csharp
using TheOne.Avalonia.Controls;

var container = Virtualization.BringIntoView(ProjectTree, 0, 9000);
if (container is not null)
    ProjectTree.SelectedItem = container.DataContext;
```

The path addresses root 0's 9,001st child. Only path-required containers are created, ancestors expand, and the target scrolls into view; invalid paths return null. `Virtualization.SelectAll(tree)` in multiselect trees selects all nodes in expanded branches, including offscreen nodes, excluding descendants of collapsed branches.

`GetRealizedContainers()` returns only current containers and helps observe virtualization. Traverse models for complete data, export, or recursive business operations.

## TabControl and Card tabs

```xml
<TabControl Classes="card" Height="320"
            ItemsSource="{Binding Documents}"
            SelectedItem="{Binding SelectedDocument, Mode=TwoWay}"
            one:TabFeatures.CanClose="True"
            one:TabFeatures.CanReorder="True">
  <TabControl.ItemTemplate>
    <DataTemplate x:DataType="vm:Document">
      <TextBlock Text="{Binding Title}" />
    </DataTemplate>
  </TabControl.ItemTemplate>
  <TabControl.ContentTemplate>
    <DataTemplate x:DataType="vm:Document">
      <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
    </DataTemplate>
  </TabControl.ContentTemplate>
  <TabControl.Styles>
    <Style Selector="TabItem" x:DataType="vm:Document">
      <Setter Property="one:TabFeatures.IsClosable" Value="{Binding CanClose}" />
    </Style>
  </TabControl.Styles>
</TabControl>
```

`Document` provides `Title`, `Description`, and `CanClose`. Use `ObservableCollection<Document>` for ItemsSource, ItemTemplate for headings, and ContentTemplate for the selected page. Remove `card` for ordinary tab appearance.

Card supports closing and reordering. Headers have backgrounds only; content retains a border. Overflow shows scroll arrows; headings and selected content are created on demand. `TabStripPlacement` supports Top / Bottom / Left / Right.

After dragging beyond 6 px, the heading follows the pointer and neighboring headings ease aside. Release commits one reorder and preserves selection. Edge autoscrolling and recycling-safe previews are supported. Esc, capture loss, disabling reordering / the control, external collection changes, or unloading cancels and cleans up without delayed commits.

Displacement / placement uses `One.Motion.Fast` (120 ms) and CubicEaseOut. `Motion.Enabled=False` or zero duration applies immediately. Set `TabFeatures.CanReorder=True` for ordinary / pills tabs. Native virtual panels remain; only the dragged heading has a preview.

| API | Purpose |
| --- | --- |
| `TabFeatures.CanClose` | Enables closing; explicit False supported |
| `TabFeatures.CanReorder` | Enables pointer / keyboard ordering within a TabControl |
| `TabFeatures.IsClosable` | Per-TabItem closing; False pins the tab |
| `TabFeatures.CloseCommand` | Host command receives the data item and updates the collection when provided |
| `TabFeatures.TabClosingEvent` | Cancellable pre-close event with Item / Cancel |
| `TabFeatures.Close(tabItem)` | Attempts to close the container's data item |
| `TabFeatures.Move(tabs, from, to)` | Attempts reordering by zero-based indices |

Cancel closing unsaved content in the view (`DocumentsTabs` is a named TabControl):

```csharp
DocumentsTabs.AddHandler(TabFeatures.TabClosingEvent, (_, args) =>
{
    if (args.Item is Document document && document.HasUnsavedChanges)
        args.Cancel = true;
});
```

`HasUnsavedChanges` is a business-model property. For asynchronous confirmation, cancel the current close first, then let the host confirm and update collections. Default close / reorder needs mutable IList; readonly or fixed-size collections cannot be deleted / reordered by default. CloseCommand can update their source instead.

Ctrl / macOS ⌘ + W closes current; Ctrl / ⌘ + Shift + ← / → reorders; Esc cancels dragging. Closing selects an adjacent tab; reordering retains the selected model. Override One.CardTabStrip, One.CardTabBorder, and One.CardTabHover for colors. Examples: [ControlGallery.axaml](../../../design/PreviewHost/Verification/Fixtures/LegacyControlGallery.axaml), [CardTabPreview.cs](../../../design/PreviewHost/Verification/Basic/CardTabPreview.cs).

## DataGrid

The library references native layout extensions based on `Avalonia.Controls.DataGrid` 12.1.2. Load OneTheme to use them; core Avalonia remains 12.1.4.

```xml
<DataGrid Height="420" ItemsSource="{Binding Records}"
          AutoGenerateColumns="False" MaxColumnWidth="320"
          CanUserSortColumns="True" CanUserResizeColumns="True"
          CanUserReorderColumns="True" FrozenColumnCount="1">
  <DataGrid.Columns>
    <DataGridTextColumn Header="编号" Binding="{Binding Id}"
                        IsReadOnly="True" />
    <DataGridTextColumn Header="名称" Binding="{Binding Name, Mode=TwoWay}" />
    <DataGridCheckBoxColumn Header="启用" Binding="{Binding Enabled, Mode=TwoWay}" />
  </DataGrid.Columns>
</DataGrid>
```

Enable compiled bindings by default; columns infer row models from typed Records collections without individual DataType / CompiledBindingExtension settings. List<Record>, ObservableCollection<Record>, and IEnumerable<Record> infer rows; DataGridCollectionView<Record> adds filtering / grouping without column x:DataType. Non-generic DataGridCollectionView still needs explicit row types. Records items expose `Id`, `Name`, and `Enabled`. Editable properties must be writable and notify changes; validation uses Avalonia-supported mechanisms. `IsReadOnly` applies to grids or columns. Use DataGridTemplateColumn CellTemplate / CellEditingTemplate for custom cells.

Call `DataGridAot.PreserveRowType<Record>()` before creating views to register edit / sort / group metadata. Set SortMemberPath for path sorting; see [DataGrid NativeAOT](datagrid-binding.md#nativeaot) for managed / native contracts.

ColumnWidth defaults to Auto with 64 px minimum. Fixed, SizeToCells, SizeToHeader, and star widths are available. Auto widths grow from displayed content rather than scanning the whole grid. RowHeight remains Auto and can grow for long content; densities follow shared conventions.

Set finite `MaxColumnWidth` before creating columns, as in the XAML example. Native 12.1.2 can throw when changing an infinite maximum to finite after visible columns exist. In code, set it before adding columns, or set each column MaxWidth before insertion.

Keep native DataGridRowsPresenter / DataGridCellsPresenter, frozen columns, and row recycling; do not replace them with VirtualizingStackPanel. Provide a bounded viewport. Row recycling is not pagination or a guarantee that all cell containers in realized rows are created on demand. Limit columns and page remote data.

The preview has 10,000 rows, sorting, width / order changes, a frozen ID column, F2 editing, Enter commit, Esc undo, filtering, and first / middle / last navigation. Examples: [DataGridDemoView.axaml](../../../design/PreviewHost/Views/DataDisplay/DataGridDemoView.axaml), [DataGridDemoModel.cs](../../../design/PreviewHost/Models/DataDisplay/GridRecord.cs).

See [DataGrid extension contracts](datagrid-extensions.md) for header sorting / filtering, text / numeric matching, combined filters, and frozen / summary APIs.

## Large-data verification

PreviewHost's large-data page shows ten-thousand-item lists, roots / children, suggestions, tabs, and context menus with realized-container counts. Choose distant-operation, recycling, navigation, and selection checks from [preview entry points](../../../design/PreviewHost/Program.cs); DataGrid has dedicated checks. Inspect real counts and distant operations rather than just ItemsPanel types when maintaining templates.


## Standalone TabStrip

```xml
<Grid RowDefinitions="Auto,*">
  <TabStrip Name="SectionTabs" Classes="normal"
            ItemsSource="{Binding Sections}"
            SelectedItem="{Binding ActiveSection, Mode=TwoWay}">
    <TabStrip.ItemTemplate>
      <DataTemplate x:DataType="vm:Section">
        <TextBlock Text="{Binding Title}" />
      </DataTemplate>
    </TabStrip.ItemTemplate>
  </TabStrip>
  <ContentControl Grid.Row="1" Content="{Binding ActiveSection}" />
</Grid>
```

Use native `Avalonia.Controls.Primitives.TabStrip` and `TabStripItem`. Default heights are 30 / 36 / 44 and fonts 12 / 14 / 16. One item remains selected by default; clicks, arrows, Home / End, and access keys are supported. `AutoScrollToSelectedItem=False` disables automatic selected-item reveal.

Headings use horizontal virtualization / recycling by default; `ScrollIntoView(index)` locates explicitly. Bound outer width and bind headings to data instead of prebuilding many controls or using WrapPanel. Bind model `IsEnabled` in `ItemContainerTheme` for individual disabled states. TabStrip supplies navigation only; the host creates page content through bindings / templates.

The standalone tab-navigation preview has 10,000 actual items, container counts, target / last navigation, and enabled-state changes.


See [Carousel / Refresh](collection-content.md), and [preview entry points](../../../design/PreviewHost/Program.cs) for checks.

## Appearance classes

`ListBox Classes="cards"` uses separate rounded items and selected borders; `borderless` suits embedded layouts. `TabControl Classes="pills"` uses pill headers; `card` keeps a continuous outline. Native panels and bounded viewports remain, with ItemsSource / compiled templates. Choose one appearance per control.
