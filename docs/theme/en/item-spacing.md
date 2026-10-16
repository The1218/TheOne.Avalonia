# Item surface spacing

[简体中文](../zh-cn/item-spacing.md) · **English**

Selectable list and navigation surfaces keep at least 4px clear of their enclosing frame and 4px between neighboring surfaces. Existing larger popup padding is retained. Horizontal strips use 2px on either side of each item to create a 4px gap; their parent provides the edge gutter. Apply spacing to the visible header rather than an entire nested branch when margins would accumulate with depth.

| Family | Implementation |
| --- | --- |
| ListBoxItem / ComboBoxItem | `Margin="4,2"` |
| TreeViewItem / NavMenuItem | Header surface `PART_LayoutRoot` uses `Margin="4,2"`; child indentation stays native |
| AutoCompleteBox / MultiSelectBox results | Inherit ListBoxItem spacing |
| Vertical MenuItem | `Margin="0,2"`; ContextMenu and submenu frames retain 6px padding |
| Menu / horizontal menu flyout | Items use `Margin="2"`; the menu bar adds 2px parent padding, flyouts retain 6px padding |
| MenuFlyoutPresenter | Default 6px padding is forwarded to its border; explicit Padding overrides remain effective |
| Standard TabControl / TabItem | Header scroll area has 2px padding and items have 2px margin, including Top / Bottom / Left / Right placement |
| TabStripItem | `Margin="2"` supports horizontal and vertical panels; TabStrip retains its existing 4px padding |

The menu bar has a minimum height and grows with its items, allowing all three densities to retain their gutters. Explicit item Margin and presenter Padding can override defaults.

The audit also checked these separate layouts:

- Calendar cells already have 2px margins, producing 4px gaps on their two-dimensional grid.
- Pagination buttons have 6px spacing; FlowStep has explicit node spacing. VerificationStep uses indicator connectors rather than adjacent filled selection surfaces.
- Card tabs preserve their separator and selected-tab seam. DataGrid cells preserve continuous grid/data alignment. Gantt's derived row theme explicitly sets zero Margin to avoid inheriting list gaps and shifting the timeline grid. Carousel pages preserve viewport paging. These structures do not receive list margins.
- Generic ItemsControl has caller-owned content and no default selected surface; its template does not add arbitrary item margins. WorkflowEditor uses the shared TabStrip theme and explicit spacing for its node library.

PreviewHost provides independent checks; run only affected families:

```text
--verify-menu-item-spacing [output]
--verify-tab-item-spacing [output]
--verify-tabstrip-item-spacing [output]
--verify-gantt [output]
```

They cover light/dark, Small/Normal/Large, wide/narrow viewports, actual surface/gap pixels, disabled items, and native key/pointer interaction. Menu checks include ContextMenu, vertical/horizontal flyouts, and nested submenus. TabControl checks all four placements; TabStrip checks both panel orientations.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### MultiSelectBox

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.MultiSelectBox.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanClear`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L76) | `bool` | read-only; OneWay | `computed / state` | Read-only empty-results state, editable-selection availability, and density minimum height |
| [`ClearText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L62) | `string` | writable; OneWay | `"清空可编辑选择"` | Accessible button names; removal format defaults to `移除 {0}`, with model text as its argument |
| [`DisplayTags`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L70) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.MultiSelectBoxItem>` | read-only; OneWay | `computed / state` | Read-only template data; `MultiSelectBoxItem.Item` is the original model, with text, enabled/selected state, and template content |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L61) | `string` | writable; OneWay | `"无匹配选项"` | Selection placeholder, search placeholder, and empty-results text; override for the host language |
| [`HasHiddenSelection`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L74) | `bool` | read-only; OneWay | `computed / state` | Read-only count/state for selections without visible tags |
| [`HasNoResults`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L75) | `bool` | read-only; OneWay | `computed / state` | Read-only empty-results state, editable-selection availability, and density minimum height |
| [`HasSelection`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L73) | `bool` | read-only; OneWay | `computed / state` | Read-only distinct selection count and nonempty state |
| [`HiddenSelectedCount`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L72) | `int` | read-only; OneWay | `computed / state` | Read-only count/state for selections without visible tags |
| [`IsDropDownOpen`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L66) | `bool` | writable; TwoWay | `false` | Selection readonly state and two-way popup state; disabling/detaching closes the popup |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L65) | `bool` | writable; OneWay | `false` | Selection readonly state and two-way popup state; disabling/detaching closes the popup |
| [`ItemAdapter`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L55) | `TheOne.Avalonia.Controls.MultiSelectItemAdapter?` | writable; OneWay | `null` | Explicit text and availability getters; text also drives search and default tags |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L77) | `double` | read-only; OneWay | `computed / state` | Read-only empty-results state, editable-selection availability, and density minimum height |
| [`ItemTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L56) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Templates for original result/tag models; declare their corresponding `x:DataType` |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L53) | `System.Collections.IEnumerable?` | writable; OneWay | `null` | Optional model `IEnumerable`; do not populate it with pre-created controls |
| [`MaxDropDownHeight`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L68) | `double` | writable; OneWay | `320` | Defaults to `320`, finite and at least 80; popup bounds also follow the host Validation: `value => double.IsFinite(value) && value >= 80` |
| [`MaxVisibleTags`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L67) | `int` | writable; OneWay | `3` | Defaults to `3`, range `0–100`; remaining selection is represented by its count Validation: `value => value is >= 0 and <= 100` |
| [`OpenText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L63) | `string` | writable; OneWay | `"展开或收起选项"` | Accessible button names; removal format defaults to `移除 {0}`, with model text as its argument |
| [`PlaceholderText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L59) | `string` | writable; OneWay | `"请选择"` | Selection placeholder, search placeholder, and empty-results text; override for the host language |
| [`RemoveTextFormat`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L64) | `string` | writable; OneWay | `"移除 {0}"` | Accessible button names; removal format defaults to `移除 {0}`, with model text as its argument |
| [`SearchPlaceholderText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L60) | `string` | writable; OneWay | `"搜索选项"` | Selection placeholder, search placeholder, and empty-results text; override for the host language |
| [`SearchText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L58) | `string` | writable; TwoWay | `""` | Defaults to empty, supports two-way binding; closing retains the query |
| [`SelectedCount`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L71) | `int` | read-only; OneWay | `computed / state` | Read-only distinct selection count and nonempty state |
| [`SelectedItemTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L57) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Templates for original result/tag models; declare their corresponding `x:DataType` |
| [`SelectedItems`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L54) | `System.Collections.IList?` | writable; TwoWay | `null` | Optional `IList`, with default two-way binding and synchronization in both directions |
| [`VisibleItems`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L69) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.MultiSelectBoxItem>` | read-only; OneWay | `computed / state` | Read-only template data; `MultiSelectBoxItem.Item` is the original model, with text, enabled/selected state, and template content |

### NavMenu

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.NavMenu.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ChildrenBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L68) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Native binding for menu children; use concrete compiled model bindings rather than reflection paths. |
| [`CollapsedWidth`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L53) | `double` | writable; OneWay | `52` | 44 ; 52 ; 60 Validation: `Positive` |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L67) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`DisplayIconTemplate`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L60) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; OneWay | `computed / state` | The icon template including the compact-mode fallback glyph. |
| [`ExpandedBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L69) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Menu expansion binding; stable model state preserves expansion across recycling. |
| [`FilterBox`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L21) | `TheOne.Avalonia.Controls.NavMenuFilterBox?` | read-only; CLR / n/a | `null` | The template's native auto-complete input, available after template application. |
| [`FilterPlaceholder`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L18) | `string` | writable; OneWay | `"搜索菜单"` | Placeholder text for the menu's built-in filter input. |
| [`FilterResults`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L19) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.NavMenuFilterResult>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HoverCloseDelay`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L23) | `System.TimeSpan` | writable; OneWay | `TimeSpan.FromMilliseconds(300)` | 00:00:00.300 ; Closes after leaving the anchor and popup chain. Entering any popup or returning to the anchor cancels pending closure. Validation: `ValidDelay` |
| [`HoverOpenDelay`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L22) | `System.TimeSpan` | writable; OneWay | `TimeSpan.FromMilliseconds(100)` | 00:00:00.100 ; Passing an icon briefly does not open it; remaining opens it after the delay. Zero is allowed. Validation: `ValidDelay` |
| [`IconSize`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L73) | `double` | writable; OneWay | `18` | Icon drawing size in DIPs, independent of font size and button hit area. Validation: `Positive` |
| [`IconSpacing`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L74) | `double` | writable; OneWay | `10` | DIP spacing between the icon and adjacent text/content. Validation: `value => double.IsFinite(value) && value >= 0` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L65) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`Indent`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L75) | `double` | writable; OneWay | `16` | Additional DIP indentation per hierarchy level, without changing parent/child relationships. Validation: `value => double.IsFinite(value) && value >= 0` |
| [`IsCollapsed`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L52) | `bool` | writable; TwoWay | `false` | Collapses the menu to an icon rail; children use hover popups while current selection is retained. |
| [`IsCompactSubmenu`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L48) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsFilterOpen`](../../../TheOne.Avalonia/Controls/NavMenu.FilterPopup.cs#L15) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsGroupBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L70) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Binding for whether an item is a group; groups differ from ordinary navigable items. |
| [`IsItemEnabledBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L71) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Binding for item availability; disabled items do not respond to user navigation selection. |
| [`IsSearching`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L26) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsSubmenuOpen`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L56) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L72) | `double` | writable; OneWay | `36` | Item/wheel-cell height in DIPs; theme density or explicit values may override registration defaults. Validation: `Positive` |
| [`ItemInvokedCommand`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L76) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`OpenOnHover`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L21) | `bool` | writable; OneWay | `true` | true ; Enables hover opening for collapsed root icons and popup branches. false still permits clicks, keyboard input, and APIs. |
| [`SearchMatchCount`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L27) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SearchOwner`](../../../TheOne.Avalonia/Controls/NavMenu.SearchInput.cs#L11) | `TheOne.Avalonia.Controls.NavMenu?` | writable; OneWay; attached | `null` | Menu instance associated with search results, used to locate and activate original items. |
| [`SearchText`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L24) | `string?` | writable; TwoWay | `null` | Current search text, independent of selection and model identity. |
| [`SearchTextBinding`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L25) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Compiled binding that reads item search text; display templates remain independently customizable. |
| [`ShowFilter`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L17) | `bool` | writable; OneWay | `false` | Shows the menu's built-in filter; the host may provide a separate search UI when hidden. |
| [`Submenu`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L58) | `TheOne.Avalonia.Controls.NavMenu?` | read-only; OneWay | `computed / state` | The currently open, virtualized submenu. Null when closed. |
| [`SubmenuMaxHeight`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L55) | `double` | writable; OneWay | `320` | 240 ; 320 ; 400 Validation: `Positive` |
| [`SubmenuOpensLeft`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L49) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SubmenuWidth`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L54) | `double` | writable; OneWay | `280` | 240 ; 280 ; 320 Validation: `Positive` |
| [`TransitionDuration`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L19) | `System.TimeSpan` | writable; OneWay | `TimeSpan.FromMilliseconds(200)` | Expansion/transition duration; no animation runs when Motion.Enabled=False. Validation: `value => value >= TimeSpan.Zero` |
| [`TransitionExtent`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L20) | `double` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

- `ComboBox`: `Classes="filled"` / `Classes="underlined"`
- `ListBox`: `Classes="borderless"` / `Classes="cards"`
- `TabControl`: `Classes="card"` / `Classes="pills"`
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`MultiSelectBox`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBox.md) | `bool MultiSelectBox.OpenDropDown()` | Opens and focuses search, or closes and returns focus to the field; opening requires attachment and enabled state |
| [`MultiSelectBox`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBox.md) | `void MultiSelectBox.CloseDropDown()` | Opens and focuses search, or closes and returns focus to the field; opening requires attachment and enabled state |
| [`MultiSelectBox`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBox.md) | `void MultiSelectBox.RefreshItems()` | Refreshes plain collections and models that do not provide change notifications. |
| [`MultiSelectBox`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBox.md) | `bool MultiSelectBox.ToggleItem(object item)` | Toggles an editable value; adding requires source membership, removal also supports selected external values; returns `false` if unchanged |
| [`MultiSelectBox`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBox.md) | `bool MultiSelectBox.ClearSelection()` | Removes editable selections, retaining selected disabled items. |
| [`MultiSelectItemAdapter`](../../api/en/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) | `abstract string MultiSelectItemAdapter.GetText(object item)` | Reads original-model display/search text through the configured typed adapter. |
| [`MultiSelectItemAdapter`](../../api/en/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) | `virtual bool MultiSelectItemAdapter.IsEnabled(object item)` | Uses the adapter to determine whether an original model permits user selection. |
| [`NavMenu`](../../api/en/TheOne.Avalonia.Controls.NavMenu.md) | `bool NavMenu.OpenSubmenu(int rootIndex)` | Open a collapsed root's children without selecting or expanding its model. |
| [`NavMenu`](../../api/en/TheOne.Avalonia.Controls.NavMenu.md) | `void NavMenu.CloseSubmenu()` | Closes the current cascading submenu and clears popup state while retaining selection. |
| [`NavMenu`](../../api/en/TheOne.Avalonia.Controls.NavMenu.md) | `bool NavMenu.OpenFilter()` | Focuses the inline filter, or opens its compact search popup. |
| [`NavMenu`](../../api/en/TheOne.Avalonia.Controls.NavMenu.md) | `void NavMenu.CloseFilter()` | Dismisses compact search and restores the pre-search navigation state. |
| [`NavMenu`](../../api/en/TheOne.Avalonia.Controls.NavMenu.md) | `bool NavMenu.SelectPath(params int[] path)` | Realize only an ancestor path, then select its target data item. |

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`MultiSelectBox`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBox.md) | `SelectionChanged` | Bubbling event with original models in AddedItems / RemovedItems; batch user edits produce one event |
| [`MultiSelectBoxItem`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) | `PropertyChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`NavMenu`](../../api/en/TheOne.Avalonia.Controls.NavMenu.md) | `ItemInvoked` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`NavMenuFilterBox`](../../api/en/TheOne.Avalonia.Controls.NavMenuFilterBox.md) | `ResultCommitted` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.MultiSelectBoxItem`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) | 10 |
| [`TheOne.Avalonia.Controls.NavMenuFilterResult`](../../api/en/TheOne.Avalonia.Controls.NavMenuFilterResult.md) | 4 |
| [`TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs`](../../api/en/TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs.md) | 1 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
