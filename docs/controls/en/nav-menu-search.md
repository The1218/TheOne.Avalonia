# NavMenu search

[简体中文](../zh-cn/nav-menu-search.md) · **English**

See [NavMenu](nav-menu.md) for basic setup.

## Multilevel filtering

SearchText supplies the query; SearchTextBinding supplies each node's searchable field. ChildrenBinding still supplies children. Search input can be placed in the page or sidebar; the dedicated NavMenu preview uses compiled TwoWay bindings:

```xml
<TextBox Text="{Binding MenuSearch, Mode=TwoWay}" PlaceholderText="搜索菜单" />
<one:NavMenu Height="480"
             ItemsSource="{Binding MenuRoots}"
             SelectedItem="{Binding SelectedMenu, Mode=TwoWay}"
             SearchText="{Binding MenuSearch, Mode=TwoWay}"
             SearchTextBinding="{Binding Title}"
             ChildrenBinding="{Binding Entries}"
             ExpandedBinding="{Binding IsExpanded, Mode=TwoWay}" />
```

Matching is case-insensitive and all whitespace-separated terms must match the field. Results retain direct matches and complete ancestor paths, expanding those paths automatically. SearchMatchCount counts direct matches only; IsSearching indicates active filtering. Empty, null, or whitespace-only queries exit search; filtering is disabled without SearchTextBinding. Properties support styles and compiled bindings.

Filtering preserves model identity, icon / content templates, disabled states, and group behavior. Source collections are unchanged. The effective ItemsSource is the filtered projection, while the external ViewModel root collection retains all data. Search expansion does not write ExpandedBinding. Clearing restores the expansion, selection, and root viewport captured on first entering search. Editing the query does not reset that snapshot, and a still-matching selection remains selected. If the original selection was removed, restoration selects nothing.

Without ExpandedBinding, expansion snapshots use the model-identity cache and survive recycling. Node renaming via INotifyPropertyChanged and root / child updates via INotifyCollectionChanged refresh active filtering. Unloading removes subscriptions; reattachment refreshes results. Update models on the UI thread and use distinct node identities.

Both inline trees and collapsed cascades use filtered collections and native virtualization panels. Search does not instantiate UI containers for the entire tree. SelectPath indices address the current filtered results. PreviewHost → Extensions → NavMenu provides actions to search the final third-level item, search thousand-item submenus, and clear / restore, with top theme / size switches.

See the [preview entry points](../../../design/PreviewHost/Program.cs) for commands and scope.


## Highlighting and search-box keyboard navigation

Associate input and menu with the `NavMenu.SearchOwner` attached property. Input Text and menu SearchText still bind TwoWay to the same ViewModel field; no view KeyDown handler is needed:

```xml
<TextBox Name="MenuSearch" Text="{Binding MenuSearch, Mode=TwoWay}"
         one:NavMenu.SearchOwner="{Binding #Navigation}" />
<one:NavMenu Name="Navigation" Height="480"
             ItemsSource="{Binding MenuRoots}"
             SearchText="{Binding MenuSearch, Mode=TwoWay}"
             SearchTextBinding="{Binding Title}"
             ChildrenBinding="{Binding Entries}"
             SelectedItem="{Binding SelectedMenu, Mode=TwoWay}">
    <one:NavMenu.ContentTemplate>
        <DataTemplate x:DataType="local:MenuNode">
            <one:NavMenuSearchText SourceText="{Binding Title}" TextTrimming="CharacterEllipsis" />
        </DataTemplate>
    </one:NavMenu.ContentTemplate>
</one:NavMenu>
```

Without ContentTemplate, default text uses SearchTextBinding automatically and highlights matches. Custom templates can bind text through `NavMenuSearchText.SourceText` to inherit search terms. Consumers still control complex templates; the library does not rewrite ordinary TextBlock, buttons, or existing Inlines. SourceText is separate from TextBlock.Text, so rebuilding spans does not break compiled bindings.

Matching uses the same OrdinalIgnoreCase and whitespace-token rules as filtering. All matching spans are bold and underlined; overlaps merge. Text inherits menu foreground, font size, and disabled states across Light / Dark and Small / Normal / Large, preserving selection accent colors. Model text changes, query changes, and recycling rebuild spans; clearing removes highlights. Collapsed popups and headings also highlight.

Unmodified keys in the search box:

| Key | Behavior |
| --- | --- |
| Down / Up | Moves through directly matched, invokable items in the current projection's depth-first order. Skips groups, disabled items, and descendants of disabled ancestors. Initial Down selects the first result; initial Up selects the last; boundary navigation stays on the current item. |
| Enter | Invokes the current result, or the first if none is selected. Keeps the query and fires ItemInvoked / ItemInvokedCommand once. Collapsed mode closes the popup chain. |
| Esc | Clears SearchText, synchronizes input through TwoWay binding, and restores expansion, selection, and the root viewport from before search. |

Navigation realizes native containers only along the target path. Focus and the caret remain in the search box. Collapsed navigation locates items through Transient popups that do not steal focus. Without results, arrows and Enter invoke nothing. Outside search, or with Ctrl / Alt / Shift / Meta, TextBox retains native keyboard behavior.

The dedicated preview demonstrates input and invocation counts. `--verify-navmenu-search` covers real input across three sizes × light / dark, navigation to item 1,000, container counts / recycling, one Enter invocation, Esc restoration, collapsed cascades, token / case / overlapping highlights, and default / custom compiled templates. Full control and native AOT consumption checks reuse these scenarios.

## Built-in autocomplete search

`ShowFilter=True` displays a native AutoCompleteBox above NavMenu. It defaults to False, preserving existing layout. `FilterPlaceholder` sets placeholder text; input inherits the menu's sizes and light / dark themes.

```xml
<one:NavMenu Height="480" ShowFilter="True"
             FilterPlaceholder="搜索菜单"
             ItemsSource="{Binding MenuRoots}"
             SearchText="{Binding MenuSearch, Mode=TwoWay}"
             SearchTextBinding="{Binding Title}"
             ChildrenBinding="{Binding Entries}"
             SelectedItem="{Binding SelectedMenu, Mode=TwoWay}" />
```

Suggestions derive from all original ItemsSource models. ChildrenBinding recursively reads children, and SearchTextBinding reads text; no separate suggestion list is needed. Suggestions show names and ancestor paths, matching case-insensitive whitespace tokens against the full path. Groups, disabled nodes, and descendants of disabled ancestors are not selectable. Model / collection notifications update the index; hiding input or unloading releases subscriptions.

Up / Down preselects suggestions while keeping the typed query. Enter or a pointer click confirms. Confirmation clears the search projection, expands the original ancestor path, selects, and scrolls to the target; collapsed mode locates it through cascades. SelectedItem TwoWay binding receives the original model, and confirmation does not additionally fire ItemInvoked. Esc clears search and restores prior expansion / selection. Expanded mode keeps input focus; collapsed mode returns focus to the search icon after confirmation or cancellation.

Suggestions use a bounded viewport, VirtualizingStackPanel, and native recycling. Only visible suggestions and target-path menu containers are realized. The index contains lightweight model data, not prebuilt controls. The existing external TextBox + SearchOwner API remains available with its original Enter-invokes-menu semantics.

### Collapsed sidebar search

With `ShowFilter=True` and `IsCollapsed=True`, the top shows only a search icon. Clicking it, or pressing Enter / Space while focused, opens a search popup and focuses input. Expanded and collapsed layouts reuse one AutoCompleteBox and compiled bindings rather than duplicating search state.

Confirming closes input and suggestion popups, locates the original cascade path, and leaves the target menu popup visible. Esc or an outside click cancels, clears the query, and restores prior state. While closed, ViewModel SearchText changes do not open hidden suggestions. Expanding the sidebar closes the popup and restores inline input. Hiding ShowFilter, disabling, or unloading NavMenu also closes search and releases window-size listeners.

`OpenFilter()` supports ViewModel commands and shortcuts: it focuses inline input when expanded and opens the popup when collapsed. `CloseFilter()` cancels collapsed search. Read-only `IsFilterOpen` exposes popup state through compiled binding. Popups inherit theme and size, update while open, fit input width to available window space, and keep a bounded virtualized suggestion viewport.

PreviewHost's main sidebar and dedicated NavMenu page enable built-in search. The dedicated page can change ShowFilter, size, theme, and collapse state and search thousand-item deep data. Checks cover recycling for suggestion / menu item 1,000, keyboard / pointer confirmation, icon activation, dynamic updates, Esc / outside cancellation, closure, focus return, theme / size changes while open, and narrow windows:

See the [preview entry points](../../../design/PreviewHost/Program.cs) for commands and scope.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost labels example Classes, sizes and states directly on each control page. Description components explain public properties, APIs/events and model fields, including property types, defaults, binding modes, validation and API signatures. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

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

### NavMenuFilterBox

[API / model reference](../../api/en/TheOne.Avalonia.Controls.NavMenuFilterBox.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### NavMenuFilterResult

[API / model reference](../../api/en/TheOne.Avalonia.Controls.NavMenuFilterResult.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Item`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L78) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Label`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L79) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Display label, not a substitute for model identity, field paths or serialized identifiers. |
| [`Path`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L80) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Stable output-field path key matching FieldPath in data references. |
| [`SearchPath`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L81) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### NavMenuSearchText

[API / model reference](../../api/en/TheOne.Avalonia.Controls.NavMenuSearchText.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`HighlightQuery`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L12) | `string?` | writable; OneWay; attached; inherits | `null` | Query highlighted within source text; affects rendering without changing SourceText. |
| [`SourceText`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L13) | `string?` | writable; OneWay | `null` | Full unhighlighted source text, without rich-text markup. |

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
| NavMenu | `one-large` | automatic template class; do not bind manually | one&#124;NavMenu.one-large => CollapsedWidth=60; SubmenuWidth=320; SubmenuMaxHeight=400; FontSize=16; IconSize=20; IconSpacing=12; ItemHeight=44; Indent=20 |
| NavMenu | `one-normal` | automatic template class; do not bind manually | one&#124;NavMenu.one-normal => CollapsedWidth=52; SubmenuWidth=280; SubmenuMaxHeight=320; FontSize=14; IconSize=18; IconSpacing=10; ItemHeight=36; Indent=16 |
| NavMenu | `one-small` | automatic template class; do not bind manually | one&#124;NavMenu.one-small => CollapsedWidth=44; SubmenuWidth=240; SubmenuMaxHeight=240; FontSize=12; IconSize=16; IconSpacing=8; ItemHeight=30; Indent=12 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.NavMenu`](../../api/en/TheOne.Avalonia.Controls.NavMenu.md) | Class | 35 | 8 | 1 |
| [`TheOne.Avalonia.Controls.NavMenuFilterBox`](../../api/en/TheOne.Avalonia.Controls.NavMenuFilterBox.md) | Class | 0 | 1 | 1 |
| [`TheOne.Avalonia.Controls.NavMenuFilterResult`](../../api/en/TheOne.Avalonia.Controls.NavMenuFilterResult.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs`](../../api/en/TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.NavMenuSearchText`](../../api/en/TheOne.Avalonia.Controls.NavMenuSearchText.md) | Class | 2 | 3 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
