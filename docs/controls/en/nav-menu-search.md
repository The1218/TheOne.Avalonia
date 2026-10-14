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
