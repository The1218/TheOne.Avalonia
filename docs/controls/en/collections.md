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

The `card` appearance stays inside the native TabControl theme instead of replacing `Theme` from a class selector. Cached pages can leave and rejoin the visual/logical tree with headers, selection and native virtualization intact; hosts do not need to recreate the tab control.

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


See [Carousel / Refresh](collection-content.md), and [preview entry points](../../../design/PreviewHost/Program.cs) for checks.

## Appearance classes

`ListBox Classes="cards"` uses separate rounded items and selected borders; `borderless` suits embedded layouts. `TabControl Classes="pills"` uses pill headers; `card` keeps a continuous outline. Native panels and bounded viewports remain, with ItemsSource / compiled templates. Choose one appearance per control.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost's Control Laboratory (top-right action on each page) groups live examples by styles, properties, APIs/events and models; complex models use the complete business samples.

### TabFeatures

[API / model reference](../../api/en/TheOne.Avalonia.Controls.TabFeatures.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanClose`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | `bool` | writable; OneWay; attached | `false` | Enables closing; explicit False supported |
| [`CanReorder`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L22) | `bool` | writable; OneWay; attached | `false` | Enables pointer / keyboard ordering within a TabControl |
| [`CloseCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | `System.Windows.Input.ICommand?` | writable; OneWay; attached | `null` | Host command receives the data item and updates the collection when provided |
| [`CloseTabCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L35) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `new CloseTab()` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`IsClosable`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L23) | `bool` | writable; OneWay; attached | `true` | Per-TabItem closing; False pins the tab |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | `bool` | writable; OneWay; attached | `false` | Shows the close button; the host and associated commands retain control over closing. |

### Virtualization

[API / model reference](../../api/en/TheOne.Avalonia.Controls.Virtualization.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L15) | `bool` | writable; OneWay; attached | `false` | Enables supported virtualization; still requires a bounded viewport and model source rather than precreated controls. |

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
| ListBox | `borderless` | explicit Classes; combine with density | ListBox.borderless, DataGrid.borderless => BorderThickness=0 |
| ListBox | `cards` | explicit Classes; combine with density | ListBox.cards => Background={DynamicResource One.Subtle}; BorderThickness=0; Padding=8; ListBox.cards > ListBoxItem => Margin=0,3; CornerRadius={DynamicResource One.Radius.Control}; BorderThickness=1; BorderBrush={DynamicResource One.Line}; ListBox.cards > ListBoxItem /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Surface}; ListBox.cards > ListBoxItem:pointerover /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Subtle}; ListBox.cards > ListBoxItem:selected /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentSoft}; BorderBrush={DynamicResource One.Accent}; Foreground={DynamicResource One.Text}; ListBox.cards > ListBoxItem:disabled /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.DisabledSurface}; Foreground={DynamicResource One.DisabledText}; BorderBrush={DynamicResource One.Line} |
| ListBox | `one-large` | automatic template class; do not bind manually | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| ListBox | `one-normal` | automatic template class; do not bind manually | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| ListBox | `one-small` | automatic template class; do not bind manually | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |
| TabControl | `card` | explicit Classes; combine with density | ^.card => BorderBrush={DynamicResource One.CardTabBorder}; BorderThickness=1; CornerRadius={DynamicResource One.Radius.TabContent}; Padding=16; ItemContainerTheme={StaticResource One.CardTabItem}; ItemsPanel=template/resource; Template={StaticResource One.CardTabControlTemplate}; TabControl.card => one:TabFeatures.CanClose=True; one:TabFeatures.CanReorder=True |
| TabControl | `one-large` | automatic template class; do not bind manually | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| TabControl | `one-normal` | automatic template class; do not bind manually | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| TabControl | `one-small` | automatic template class; do not bind manually | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |
| TabControl | `pills` | explicit Classes; combine with density | TabControl.pills > TabItem => CornerRadius=999; Margin=0,0,8,0; TabControl.pills > TabItem /template/ Border#PART_SelectedPipe => IsVisible=False; TabControl.pills > TabItem:selected /template/ Border#PART_LayoutRoot => Background={DynamicResource One.Accent}; TabControl.pills > TabItem:selected /template/ ContentPresenter#PART_ContentPresenter => Foreground={DynamicResource One.OnAccent}; TabControl.pills > TabItem:selected:pointerover /template/ Border#PART_LayoutRoot => Background={DynamicResource One.AccentHover}; TabControl.pills > TabItem:selected:pressed /template/ Border#PART_LayoutRoot => Background={DynamicResource One.AccentPressed}; TabControl.pills > TabItem:disabled /template/ Border#PART_LayoutRoot => Background={DynamicResource One.DisabledSurface}; TabControl.pills > TabItem:disabled /template/ ContentPresenter#PART_ContentPresenter => Foreground={DynamicResource One.DisabledText} |
| TabStrip | `one-large` | automatic template class; do not bind manually | TabStrip.one-large, TabStripItem.one-large => FontSize=16 |
| TabStrip | `one-normal` | automatic template class; do not bind manually | TabStrip.one-normal, TabStripItem.one-normal => FontSize=14 |
| TabStrip | `one-small` | automatic template class; do not bind manually | TabStrip.one-small, TabStripItem.one-small => FontSize=12 |
| TreeView | `one-large` | automatic template class; do not bind manually | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| TreeView | `one-normal` | automatic template class; do not bind manually | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| TreeView | `one-small` | automatic template class; do not bind manually | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |
| TreeViewItem | `one-large` | automatic template class; do not bind manually | :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, ButtonSpinner.one-large, CheckBox.one-large, RadioButton.one-large, ToggleSwitch.one-large, ListBoxItem.one-large, TreeViewItem.one-large, MenuItem.one-large, TabItem.one-large, CalendarDatePicker.one-large, DatePicker.one-large, TimePicker.one-large, Expander.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16; TreeViewItem.one-large => Padding=0,4,14,4; TreeViewItem.one-large /template/ ToggleButton#PART_ExpandCollapseChevron => Width=44; Height=44 |
| TreeViewItem | `one-no-motion` | automatic template class; do not bind manually | ToggleSwitch.one-no-motion /template/ Thumb, Expander.one-no-motion /template/ Path, TreeViewItem.one-no-motion /template/ Path, one&#124;NavMenuItem.one-no-motion /template/ Path => Transitions={x:Null} |
| TreeViewItem | `one-normal` | automatic template class; do not bind manually | :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, ButtonSpinner.one-normal, CheckBox.one-normal, RadioButton.one-normal, ToggleSwitch.one-normal, ListBoxItem.one-normal, TreeViewItem.one-normal, MenuItem.one-normal, TabItem.one-normal, CalendarDatePicker.one-normal, DatePicker.one-normal, TimePicker.one-normal, Expander.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14; TreeViewItem.one-normal => Padding=0,4,10,4; TreeViewItem.one-normal /template/ ToggleButton#PART_ExpandCollapseChevron => Width=36; Height=36 |
| TreeViewItem | `one-small` | automatic template class; do not bind manually | :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, ButtonSpinner.one-small, CheckBox.one-small, RadioButton.one-small, ToggleSwitch.one-small, ListBoxItem.one-small, TreeViewItem.one-small, MenuItem.one-small, TabItem.one-small, CalendarDatePicker.one-small, DatePicker.one-small, TimePicker.one-small, Expander.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12; TreeViewItem.one-small => Padding=0,4,10,4; TreeViewItem.one-small /template/ ToggleButton#PART_ExpandCollapseChevron => Width=30; Height=30 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.TabFeatures`](../../api/en/TheOne.Avalonia.Controls.TabFeatures.md) | Class | 6 | 22 | 0 |
| [`TheOne.Avalonia.Controls.Virtualization`](../../api/en/TheOne.Avalonia.Controls.Virtualization.md) | Class | 1 | 5 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
