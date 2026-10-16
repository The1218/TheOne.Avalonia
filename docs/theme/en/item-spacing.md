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
## Additional properties, styles and complete API

This section lists only Classes, properties, extension APIs/events and related models added by this project, without repeating inherited Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost visibly compares project-added styles and uses Description components for control-specific properties, API signatures and model fields. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### MultiSelectBox

[API / model reference](../../api/en/TheOne.Avalonia.Controls.MultiSelectBox.md)

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

[API / model reference](../../api/en/TheOne.Avalonia.Controls.NavMenu.md)

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

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| ComboBox | `filled` | explicit Classes; combine with density | TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled => Background={DynamicResource One.Subtle}; ComboBox.filled /template/ Border#HighlightBackground => Background={DynamicResource One.Subtle} |
| ComboBox | `one-large` | automatic template class; do not bind manually | :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, ButtonSpinner.one-large, CheckBox.one-large, RadioButton.one-large, ToggleSwitch.one-large, ListBoxItem.one-large, TreeViewItem.one-large, MenuItem.one-large, TabItem.one-large, CalendarDatePicker.one-large, DatePicker.one-large, TimePicker.one-large, Expander.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16; :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, CalendarDatePicker.one-large, ListBoxItem.one-large, MenuItem.one-large => Padding=16,10 |
| ComboBox | `one-normal` | automatic template class; do not bind manually | :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, ButtonSpinner.one-normal, CheckBox.one-normal, RadioButton.one-normal, ToggleSwitch.one-normal, ListBoxItem.one-normal, TreeViewItem.one-normal, MenuItem.one-normal, TabItem.one-normal, CalendarDatePicker.one-normal, DatePicker.one-normal, TimePicker.one-normal, Expander.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14; :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, CalendarDatePicker.one-normal, ListBoxItem.one-normal, MenuItem.one-normal => Padding=12,7 |
| ComboBox | `one-small` | automatic template class; do not bind manually | :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, ButtonSpinner.one-small, CheckBox.one-small, RadioButton.one-small, ToggleSwitch.one-small, ListBoxItem.one-small, TreeViewItem.one-small, MenuItem.one-small, TabItem.one-small, CalendarDatePicker.one-small, DatePicker.one-small, TimePicker.one-small, Expander.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12; :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, CalendarDatePicker.one-small, ListBoxItem.one-small, MenuItem.one-small => Padding=10,4 |
| ComboBox | `pagination-size` | automatic template class; do not bind manually | ComboBox.pagination-size => Padding=10,0,6,0; MinHeight={Binding $parent[one:Pagination].ItemHeight} |
| ComboBox | `underlined` | explicit Classes; combine with density | TextBox.underlined, ComboBox.underlined, NumericUpDown.underlined, AutoCompleteBox.underlined => Background={DynamicResource One.Surface}; BorderThickness=0,0,0,1; CornerRadius=0; ComboBox.underlined:focus-within, NumericUpDown.underlined:focus-within, AutoCompleteBox.underlined:focus-within => BorderThickness=0,0,0,2; ComboBox.underlined /template/ TextBox#PART_EditableTextBox => MinHeight=0; FocusAdorner={x:Null}; ComboBox.underlined /template/ TextBox#PART_EditableTextBox /template/ Border#PART_BorderElement => Background=Transparent; BorderBrush=Transparent; BorderThickness=0; CornerRadius=0; ComboBox.underlined:focus-within /template/ Border#Background, ComboBox.underlined:focus-within /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Focus}; ComboBox.underlined:focus-within /template/ Border#Background, ComboBox.underlined:focus-within /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Focus}; ComboBox.underlined:error /template/ Border#Background, ComboBox.underlined:error /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Danger}; ComboBox.underlined:error /template/ Border#Background, ComboBox.underlined:error /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Danger}; TextBox.underlined:disabled /template/ Border#PART_BorderElement, AutoCompleteBox.underlined:disabled /template/ TextBox#PART_TextBox /template/ Border#PART_BorderElement, ComboBox.underlined:disabled /template/ Border#Background, ComboBox.underlined:disabled /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.InputLine}; TextBox.underlined:disabled /template/ Border#PART_BorderElement, AutoCompleteBox.underlined:disabled /template/ TextBox#PART_TextBox /template/ Border#PART_BorderElement, ComboBox.underlined:disabled /template/ Border#Background, ComboBox.underlined:disabled /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.InputLine} |
| ListBox | `borderless` | explicit Classes; combine with density | ListBox.borderless, DataGrid.borderless => BorderThickness=0 |
| ListBox | `cards` | explicit Classes; combine with density | ListBox.cards => Background={DynamicResource One.Subtle}; BorderThickness=0; Padding=8; ListBox.cards > ListBoxItem => Margin=0,3; CornerRadius={DynamicResource One.Radius.Control}; BorderThickness=1; BorderBrush={DynamicResource One.Line}; ListBox.cards > ListBoxItem /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Surface}; ListBox.cards > ListBoxItem:pointerover /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Subtle}; ListBox.cards > ListBoxItem:selected /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentSoft}; BorderBrush={DynamicResource One.Accent}; Foreground={DynamicResource One.Text}; ListBox.cards > ListBoxItem:disabled /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.DisabledSurface}; Foreground={DynamicResource One.DisabledText}; BorderBrush={DynamicResource One.Line} |
| ListBox | `one-large` | automatic template class; do not bind manually | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| ListBox | `one-normal` | automatic template class; do not bind manually | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| ListBox | `one-small` | automatic template class; do not bind manually | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |
| MenuItem | `one-large` | automatic template class; do not bind manually | :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, ButtonSpinner.one-large, CheckBox.one-large, RadioButton.one-large, ToggleSwitch.one-large, ListBoxItem.one-large, TreeViewItem.one-large, MenuItem.one-large, TabItem.one-large, CalendarDatePicker.one-large, DatePicker.one-large, TimePicker.one-large, Expander.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16; :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, CalendarDatePicker.one-large, ListBoxItem.one-large, MenuItem.one-large => Padding=16,10 |
| MenuItem | `one-normal` | automatic template class; do not bind manually | :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, ButtonSpinner.one-normal, CheckBox.one-normal, RadioButton.one-normal, ToggleSwitch.one-normal, ListBoxItem.one-normal, TreeViewItem.one-normal, MenuItem.one-normal, TabItem.one-normal, CalendarDatePicker.one-normal, DatePicker.one-normal, TimePicker.one-normal, Expander.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14; :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, CalendarDatePicker.one-normal, ListBoxItem.one-normal, MenuItem.one-normal => Padding=12,7 |
| MenuItem | `one-small` | automatic template class; do not bind manually | :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, ButtonSpinner.one-small, CheckBox.one-small, RadioButton.one-small, ToggleSwitch.one-small, ListBoxItem.one-small, TreeViewItem.one-small, MenuItem.one-small, TabItem.one-small, CalendarDatePicker.one-small, DatePicker.one-small, TimePicker.one-small, Expander.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12; :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, CalendarDatePicker.one-small, ListBoxItem.one-small, MenuItem.one-small => Padding=10,4 |
| MultiSelectBox | `one-large` | automatic template class; do not bind manually | ^.one-large => FontSize=16 |
| MultiSelectBox | `one-normal` | automatic template class; do not bind manually | ^.one-normal => FontSize=14 |
| MultiSelectBox | `one-small` | automatic template class; do not bind manually | ^.one-small => FontSize=12 |
| NavMenu | `one-large` | automatic template class; do not bind manually | one&#124;NavMenu.one-large => CollapsedWidth=60; SubmenuWidth=320; SubmenuMaxHeight=400; FontSize=16; IconSize=20; IconSpacing=12; ItemHeight=44; Indent=20 |
| NavMenu | `one-normal` | automatic template class; do not bind manually | one&#124;NavMenu.one-normal => CollapsedWidth=52; SubmenuWidth=280; SubmenuMaxHeight=320; FontSize=14; IconSize=18; IconSpacing=10; ItemHeight=36; Indent=16 |
| NavMenu | `one-small` | automatic template class; do not bind manually | one&#124;NavMenu.one-small => CollapsedWidth=44; SubmenuWidth=240; SubmenuMaxHeight=240; FontSize=12; IconSize=16; IconSpacing=8; ItemHeight=30; Indent=12 |
| TabControl | `card` | explicit Classes; combine with density | ^.card => BorderBrush={DynamicResource One.CardTabBorder}; BorderThickness=1; CornerRadius={DynamicResource One.Radius.TabContent}; Padding=16; ItemContainerTheme={StaticResource One.CardTabItem}; ItemsPanel=template/resource; Template={StaticResource One.CardTabControlTemplate}; TabControl.card => one:TabFeatures.CanClose=True; one:TabFeatures.CanReorder=True |
| TabControl | `one-large` | automatic template class; do not bind manually | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| TabControl | `one-normal` | automatic template class; do not bind manually | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| TabControl | `one-small` | automatic template class; do not bind manually | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |
| TabControl | `pills` | explicit Classes; combine with density | TabControl.pills > TabItem => CornerRadius=999; Margin=0,0,8,0; TabControl.pills > TabItem /template/ Border#PART_SelectedPipe => IsVisible=False; TabControl.pills > TabItem:selected /template/ Border#PART_LayoutRoot => Background={DynamicResource One.Accent}; TabControl.pills > TabItem:selected /template/ ContentPresenter#PART_ContentPresenter => Foreground={DynamicResource One.OnAccent}; TabControl.pills > TabItem:selected:pointerover /template/ Border#PART_LayoutRoot => Background={DynamicResource One.AccentHover}; TabControl.pills > TabItem:selected:pressed /template/ Border#PART_LayoutRoot => Background={DynamicResource One.AccentPressed}; TabControl.pills > TabItem:disabled /template/ Border#PART_LayoutRoot => Background={DynamicResource One.DisabledSurface}; TabControl.pills > TabItem:disabled /template/ ContentPresenter#PART_ContentPresenter => Foreground={DynamicResource One.DisabledText} |
| TreeView | `one-large` | automatic template class; do not bind manually | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| TreeView | `one-normal` | automatic template class; do not bind manually | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| TreeView | `one-small` | automatic template class; do not bind manually | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |

`:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without added appearance classes use their default theme and control-specific properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.MultiSelectBox`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBox.md) | Class | 25 | 6 | 1 |
| [`TheOne.Avalonia.Controls.MultiSelectBoxItem`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) | Class | 10 | 0 | 1 |
| [`TheOne.Avalonia.Controls.MultiSelectItemAdapter`](../../api/en/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) | Class | 0 | 3 | 0 |
| [`TheOne.Avalonia.Controls.NavMenu`](../../api/en/TheOne.Avalonia.Controls.NavMenu.md) | Class | 35 | 8 | 1 |
| [`TheOne.Avalonia.Controls.NavMenuFilterBox`](../../api/en/TheOne.Avalonia.Controls.NavMenuFilterBox.md) | Class | 0 | 1 | 1 |
| [`TheOne.Avalonia.Controls.NavMenuFilterResult`](../../api/en/TheOne.Avalonia.Controls.NavMenuFilterResult.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs`](../../api/en/TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs.md) | Class | 1 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
