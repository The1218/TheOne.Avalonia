# Lists, trees, tabs, and grids

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

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

## Dynamic collections and container lifetime

The default theme fixes caller-owned item controls disappearing after reorder or removal/reinsertion, and stale data after Replace, a single Reset or switching ItemsSource. Prefer model collections with compiled templates for dynamic applications. Use ObservableCollection or correct collection notifications; mutating a non-notifying backing list does not refresh the UI automatically.

After a source change, selection may refer only to items in the current collection. To preserve business selection across reloads, resolve a stable business identifier to an instance in the new collection from the ViewModel; do not retain old containers or model references. Keep tree expansion and selection in models so recycled containers rebind correctly.

Dropping a TabControl heading beyond the last heading moves it to the end. Release/cancel clears drag-preview offsets. Closing, reordering or resetting a collection requires no manual visual-tree removal of TabItem, and generated containers should not become the data source.

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

Content defaults to `Padding="8"` for standard, `card` and `One.CardTabControl` themes, reduced from Card's previous 16 DIP. Header-item padding is unchanged. Explicit local Padding takes precedence; use `Padding="0"` for edge-to-edge workspaces. Preview content no longer adds another 16/24 DIP margin.

When switching the `card` Class or `One.CardTabControl` theme at runtime, the old content presenters release their visual children before native TabControl mounts them in the new presenters, preventing an already-parented exception. The selected tab, control content instances and edited text are retained. Content presenters follow the current Padding without accumulating old spacing when changing theme variants, density or tabs; clearing local Padding restores 8 DIP. Wrap long content in a ScrollViewer as needed; Padding does not add automatic scrolling.

After replacing the template or header panel, realized caller-owned TabItems are reattached with the current container theme and inherited density, preventing stale header heights from squeezing the content area. This refresh only touches realized caller-owned headers; it does not recreate pages or clear their Header / Content or explicit consumer property values.

Tab icons and context menus are available. `TabFeatures.ShowItemIcon` on TabControl (default True) toggles icons. `TabFeatures.Icon` on TabItem accepts Geometry / text by default; IconTemplate customizes rendering. `TabFeatures.IsPinned=True` pins left and prevents closing. IsClosable=False only prevents closing.

```xml
<TabControl Classes="card" one:TabFeatures.CanClose="True"
            one:TabFeatures.CanReorder="True" one:TabFeatures.ShowItemIcon="True">
  <TabItem Header="Home" one:TabFeatures.Icon="{StaticResource HomeGeometry}"
           one:TabFeatures.IsPinned="True"><TextBlock Text="Home" /></TabItem>
  <TabItem Header="Editor"><TextBox Text="Draft" /></TabItem>
</TabControl>
```

Menus offer close current, pin left / unpin and close all. Close all skips pinned, nonclosable and canceled items. Consumer ContextMenus are preserved. Pinned headers display a pushpin and cannot be dragged or reordered by keyboard or Move. Ordinary drag previews and drop positions cannot cross the pinned region. ShowItemIcon only controls the item icon, not the pushpin. Buttons, shortcuts and APIs share close rules. Bound item pin state follows model identity rather than recycled containers.

Compact tab padding accompanies a 14 DIP item icon, 12 DIP pushpin and 10 DIP close glyph. The close button retains its 24 DIP hit target. The three density levels still follow shared sizing.

OneTheme enables menus by default. Set `one:TabFeatures.EnableContextMenu="False"` to disable built-in menus; consumer ContextMenus remain consumer-owned.

```csharp
TabFeatures.Pin(tabItem);        // pin left while preserving selection
TabFeatures.Pin(tabItem, false); // unpin
int closed = TabFeatures.CloseAll(tabs); // honor cancellation and CloseCommand
```

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

The `card` appearance stays inside the native TabControl theme instead of replacing `Theme` from a class selector. Cached pages can leave and rejoin the visual/logical tree with headers, selection and native virtualization intact; hosts do not need to recreate the tab control.

After dragging beyond 6 px, the heading follows the pointer and neighboring headings ease aside. Release commits one reorder and preserves selection. Edge autoscrolling and recycling-safe previews are supported. Esc, capture loss, disabling reordering / the control, external collection changes, or unloading cancels and cleans up without delayed commits.

Displacement / placement uses `One.Motion.Fast` (120 ms) and CubicEaseOut. `Motion.Enabled=False` or zero duration applies immediately. Set `TabFeatures.CanReorder=True` for ordinary / pills tabs. Native virtual panels remain; only the dragged heading has a preview.

| API | Purpose |
| --- | --- |
| `TabFeatures.CanClose` | Enables closing; explicit False supported |
| `TabFeatures.CanReorder` | Enables pointer / keyboard ordering within a TabControl |
| `TabFeatures.IsClosable` | Per-TabItem closing; False prevents closing but still allows reordering |
| `TabFeatures.CloseCommand` | Host command receives the data item and updates the collection when provided |
| `TabFeatures.TabClosingEvent` | Cancellable pre-close event with Item / Cancel |
| `TabFeatures.Close(tabItem)` | Attempts to close the container's data item |
| `TabFeatures.Move(tabs, from, to)` | Attempts reordering by zero-based indices |

After `TabClosing`, closing rechecks the container's owner, data identity and current close permission. If the callback replaces the source or container, pins/disables the tab, rejects the command, or already closes the item, the original operation stops without changing the callback's selection.

Cancel closing unsaved content in the view (`DocumentsTabs` is a named TabControl):

```csharp
DocumentsTabs.AddHandler(TabFeatures.TabClosingEvent, (_, args) =>
{
    if (args.Item is Document document && document.HasUnsavedChanges)
        args.Cancel = true;
});
```

`HasUnsavedChanges` is a business-model property. For asynchronous confirmation, cancel the current close first, then let the host confirm and update collections. Default close / reorder needs mutable IList; readonly or fixed-size collections cannot be deleted / reordered by default. CloseCommand can update their source instead.

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

PreviewHost's large-data page shows ten-thousand-item lists, roots / children, suggestions, tabs, and context menus with realized-container counts. Inspect real counts and distant operations rather than just ItemsPanel types when maintaining templates.

## Standalone TabStrip

```xml
<Grid RowDefinitions="Auto,*">
  <TabStrip Name="SectionTabs" Classes="normal"
            ItemsSource="{Binding Sections}"
            SelectedItem="{Binding ActiveSection, Mode=TwoWay}"
            one:TabFeatures.CanClose="True"
            one:TabFeatures.CloseCommand="{Binding CloseSectionCommand}">
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

Closing is opt-in: `TabFeatures.CanClose` defaults to False. With it enabled, native `TabStripItem` headers show a close button; Ctrl / macOS ⌘ + W closes the selected item while the strip has keyboard focus. Set `TabFeatures.IsClosable=False` on a container to pin it. Closing uses the cancellable `TabFeatures.TabClosingEvent`, with the data item in `Item` and a `Cancel` flag, just like TabControl. `CloseSectionCommand` receives that item, respects `CanExecute`, and owns the collection update; omit it to remove from a mutable IList automatically. Readonly/fixed-size sources require a host command. Closing selects an adjacent item and retains native virtualization/recycling.

```csharp
SectionTabs.AddHandler(TabFeatures.TabClosingEvent, (_, args) =>
{
    if (args.Item is Section section && section.HasUnsavedChanges)
        args.Cancel = true;
});
if (SectionTabs.ContainerFromIndex(SectionTabs.SelectedIndex) is TabStripItem item)
    TabFeatures.Close(item);
```

`Section.HasUnsavedChanges` is an application property. For asynchronous confirmation, cancel first and let the host confirm before removing the item. `CanClose`, `CloseCommand` and `Close(TabStripItem)` add standalone closing without changing existing TabControl APIs; `CanReorder` and `Move` remain TabControl-only.

The standalone tab-navigation preview has 10,000 actual items, container counts, target / last navigation, and enabled-state changes.

## Appearance classes

`ListBox Classes="cards"` uses separate rounded items and selected borders; `borderless` suits embedded layouts. `TabControl Classes="pills"` uses pill headers; `card` keeps a continuous outline. Native panels and bounded viewports remain, with ItemsSource / compiled templates. Choose one appearance per control.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### TabFeatures

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.TabFeatures.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanClose`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | `bool` | writable; OneWay; attached | `false` | Enables closing; explicit False supported |
| [`CanReorder`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | `bool` | writable; OneWay; attached | `false` | Enables pointer / keyboard ordering within a TabControl |
| [`CloseCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | `System.Windows.Input.ICommand?` | writable; OneWay; attached | `null` | Host command receives the data item and updates the collection when provided |
| [`CloseTabCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L66) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `new CloseTab()` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`EnableContextMenu`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L12) | `bool` | writable; OneWay; attached | `false` | EnableContextMenu configuration on TabFeatures; type and initial value are listed here. EnableContextMenuProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Icon`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L18) | `object?` | writable; OneWay; attached | `null` | Icon content/vector geometry as defined by the signature; the host owns external bitmap lifetimes. |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L20) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay; attached | `DefaultIconTemplate` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`IsClosable`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L27) | `bool` | writable; OneWay; attached | `true` | Per-TabItem closing; False prevents closing but still allows reordering |
| [`IsIconVisible`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L24) | `bool` | read-only; OneWay; attached | `false` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsPinned`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L22) | `bool` | writable; OneWay; attached | `false` | IsPinned configuration on TabFeatures; type and initial value are listed here. IsPinnedProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L30) | `bool` | writable; OneWay; attached | `false` | Shows the close button; the host and associated commands retain control over closing. |
| [`ShowItemIcon`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L16) | `bool` | writable; OneWay; attached | `true` | ShowItemIcon configuration on TabFeatures; type and initial value are listed here. ShowItemIconProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |

### Virtualization

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.Virtualization.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L16) | `bool` | writable; OneWay; attached | `false` | Enables supported virtualization; still requires a bounded viewport and model source rather than precreated controls. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

- `DataGrid`: `Classes="borderless"`
- `ListBox`: `Classes="borderless"` / `Classes="cards"`
- `TabControl`: `Classes="card"` / `Classes="pills"`
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`TabFeatures`](../../api/en/TheOne.Avalonia.Controls.TabFeatures.md) | `static bool TabFeatures.Close(TabStripItem item)` | Attempts to close the container's data item |
| [`TabFeatures`](../../api/en/TheOne.Avalonia.Controls.TabFeatures.md) | `static bool TabFeatures.Pin(TabItem item, bool pinned = true)` | Pins a page before all unpinned pages, preserving selection. Does not require drag reordering. |
| [`TabFeatures`](../../api/en/TheOne.Avalonia.Controls.TabFeatures.md) | `static int TabFeatures.CloseAll(TabControl tabs)` | Closes a snapshot of closable pages, honoring cancellation, commands and pin state. |
| [`TabFeatures`](../../api/en/TheOne.Avalonia.Controls.TabFeatures.md) | `static bool TabFeatures.Close(TabItem item)` | Attempts to close the container's data item |
| [`TabFeatures`](../../api/en/TheOne.Avalonia.Controls.TabFeatures.md) | `static bool TabFeatures.Move(TabControl tabs, int from, int to)` | Attempts reordering by zero-based indices |
| [`Virtualization`](../../api/en/TheOne.Avalonia.Controls.Virtualization.md) | `static TreeViewItem? Virtualization.BringIntoView(TreeView tree, params int[] path)` | Materializes only the ancestor path and target of a large tree. |
| [`Virtualization`](../../api/en/TheOne.Avalonia.Controls.Virtualization.md) | `static void Virtualization.SelectAll(TreeView tree)` | Selects all nodes exposed by expanded branches without creating their containers. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.TabFeatures`](../../api/en/TheOne.Avalonia.Controls.TabFeatures.md) | 12 |
| [`TheOne.Avalonia.Controls.Virtualization`](../../api/en/TheOne.Avalonia.Controls.Virtualization.md) | 1 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
