# NavMenu multilevel navigation

[简体中文](../zh-cn/nav-menu.md) · **English**

`NavMenu` inherits native `TreeView`, using `NavMenuItem` containers and TheOne themes. It supports multilevel menus, separate icon and text templates, page selection, group headers, native keyboard navigation, virtualization, and recycling at every level. Loading `OneTheme` enables Light / Dark and all three sizes.

## Child properties and templates

`ItemsSource` provides the roots; `ChildrenBinding` provides each node's children. Node bindings and icon / content templates infer node types from strongly typed ItemsSource and use compiled bindings for arbitrary business paths such as `Entries`, `Children`, or `Navigation.Children`. They do not reflect over property names.

```xml
<one:NavMenu Height="480"
             ItemsSource="{Binding MenuRoots}"
             SelectedItem="{Binding SelectedMenu, Mode=TwoWay}"
             ChildrenBinding="{Binding Entries}"
             ExpandedBinding="{Binding IsExpanded, Mode=TwoWay}"
             IsGroupBinding="{Binding IsGroup}"
             IsItemEnabledBinding="{Binding IsEnabled}"
             ItemInvokedCommand="{Binding NavigateCommand}">
  <one:NavMenu.IconTemplate>
    <DataTemplate x:DataType="local:MenuNode">
      <PathIcon Data="{Binding Icon}" />
    </DataTemplate>
  </one:NavMenu.IconTemplate>
  <one:NavMenu.ContentTemplate>
    <DataTemplate x:DataType="local:MenuNode">
      <TextBlock Text="{Binding Title}" TextTrimming="CharacterEllipsis" />
    </DataTemplate>
  </one:NavMenu.ContentTemplate>
</one:NavMenu>
```

`one` is `using:TheOne.Avalonia.Controls`. Declare the window ViewModel's `x:DataType` on the view and enable `AvaloniaUseCompiledBindingsByDefault`; individual bindings need neither repeated DataType declarations nor CompiledBindingExtension. In the example, `local:MenuNode` is the node type. Templates receive the entire node, so icons and titles can come from different fields. No `IconTemplate` means no reserved icon spacing; no `ContentTemplate` uses native default content. The control manages `ItemTemplate` internally; customize row content through the other two templates.

Children can use `ObservableCollection<MenuNode>` for live additions / removals, and changing model properties should notify. Children binding defaults to null; without it, navigation has one level. `ExpandedBinding` is optional: expansion for the same model object survives recycling by default. To persist it across data reloads, use the example's TwoWay binding with stable model identities. All three state bindings use the node as source and infer types from strongly typed collections. If ItemsSource is declared only as non-generic IEnumerable, declare the node type in the node-binding or template scope.

## Selection, invocation, and groups

In collapsed mode, hovering a root opens its submenu without changing the selected page. Unrelated submenu rows remain unselected; active-ancestor styling requires a resolved, non-null item in the current menu level.

`SelectedItem` is the data object for the current page; use `SelectionChanged` to synchronize page content. `ItemInvoked` and `ItemInvokedCommand` receive that object on click, Enter, or Space, including repeated activation of the selected page. `SelectPath` performs programmatic navigation without firing invocation events or commands.

`IsGroupBinding` marks group headers. Clicking a header or pressing Enter / Space expands or collapses it without replacing page selection or running navigation commands. Ordinary nodes with children can still represent selectable pages; their arrow expands independently. Buttons, inputs, and selection controls in templates retain their own interactions.

Native arrow keys, Home / End, and scrolling navigation remain available. `SelectPath` locates unrealized multilevel targets, creating containers only along the ancestor path and in the current viewport:

```csharp
// 根节点 0 → 子节点 0 → 第 1,000 个页面。
bool selected = menu.SelectPath(0, 0, 999);
```

Invalid paths, group headers, and disabled items return false. Navigation uses single selection. Traverse models to read the full menu data rather than `GetRealizedTreeContainers()`.

## Density, theme, and motion

Set `styling:ControlSizing.Size="Small|Normal|Large"` on the menu or a parent, or use existing `small` / `normal` / `large` classes. `ItemHeight`, `IconSize`, `IconSpacing`, `Indent`, and `FontSize` can be overridden in Style.

| Size | Row height | Icon | Font size | Icon spacing | Level indent |
| --- | ---: | ---: | ---: | ---: | ---: |
| Small | 30 | 16 | 12 | 8 | 12 |
| Normal | 36 | 18 | 14 | 10 | 16 |
| Large | 44 | 20 | 16 | 12 | 20 |

Normal rows are transparent; hover uses `One.Subtle`, selection uses `One.AccentSoft` / `One.Accent`, and disabled text uses `One.DisabledText`. Theme and density can change live. Expansion / collapse uses TreeView's RevealHost and inherits `Motion.Enabled`.

## Virtualization and verification

Large root collections and every child level use native `VirtualizingStackPanel`, preserving `PART_*`, focus, selection, and expansion. With `ChildrenBinding` and at most 32 roots, the root layer uses `StackPanel` to retain group containers. This prevents entire branches from being recycled, remounted, and re-estimated when rapidly reversing the scroll direction. Children remain virtualized with one cached viewport on each side. Adding or removing roots across this limit switches the default panel automatically; an explicitly assigned `ItemsPanel` takes precedence.

Provide finite height or a bounded `*` row in `Grid`. Avoid unbounded navigation in a height-unconstrained `StackPanel` or outer infinite ScrollViewer. Virtualization reduces UI containers; it is not business-data pagination. The `--verify-navmenu-fling` check uses PreviewHost's actual sidebar to cover bottom-to-top wheel reversal, stable height across group boundaries, narrow sidebars, and large child collections.

PreviewHost's sidebar uses NavMenu with six groups: basic controls, data display, navigation / layout, charts, extensions, and interaction verification. Extensions → NavMenu demonstrates 1,000 items per level and 5,000 models in total, with navigation to the final root, second-level, and third-level pages and live realized-container counts.

See the [preview entry points](../../../design/PreviewHost/Program.cs) for commands and scope.

Checks cover light / dark themes and all sizes, realized-container counts, distant selection and keyboard navigation, expansion after recycling, compiled template updates, dynamic children, disabled states, non-navigating groups, custom template interactions, and live theme / size changes. Independent native consumption reuses the same compiled XAML and interactions without reflection bindings, dynamic XAML, or whole-assembly preservation.

## Feature guides

- [Collapsed sidebar, hover cascades, and edge adaptation](nav-menu-popups.md).
- [Multilevel search, highlighting, keyboard input, and built-in autocomplete](nav-menu-search.md).
- [Expansion and collapse motion](nav-menu-motion.md).

See the [preview entry points](../../../design/PreviewHost/Program.cs) for verification and native scenarios.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost's Control Laboratory (top-right action on each page) groups live examples by styles, properties, APIs/events and models; complex models use the complete business samples.

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

### NavMenuItem

[API / model reference](../../api/en/TheOne.Avalonia.Controls.NavMenuItem.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsActiveAncestor`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L22) | `bool` | writable; OneWay | `false` | Whether this item is an ancestor of the selected item; menu-owned and distinct from child selection. |
| [`IsCascadeItem`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L20) | `bool` | writable; OneWay | `false` | Whether this container belongs to a cascading popup, maintained by the menu/template. |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L21) | `bool` | writable; OneWay | `false` | Enables compact layout; the template determines visible fields without altering business data. |
| [`IsGroup`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L23) | `bool` | writable; OneWay | `false` | Whether this menu item is a group, distinct from selected state. |

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
| NavMenuItem | `one-no-motion` | automatic template class; do not bind manually | ToggleSwitch.one-no-motion /template/ Thumb, Expander.one-no-motion /template/ Path, TreeViewItem.one-no-motion /template/ Path, one&#124;NavMenuItem.one-no-motion /template/ Path => Transitions={x:Null} |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.NavMenu`](../../api/en/TheOne.Avalonia.Controls.NavMenu.md) | Class | 35 | 8 | 1 |
| [`TheOne.Avalonia.Controls.NavMenuFilterBox`](../../api/en/TheOne.Avalonia.Controls.NavMenuFilterBox.md) | Class | 0 | 1 | 1 |
| [`TheOne.Avalonia.Controls.NavMenuFilterResult`](../../api/en/TheOne.Avalonia.Controls.NavMenuFilterResult.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.NavMenuItem`](../../api/en/TheOne.Avalonia.Controls.NavMenuItem.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs`](../../api/en/TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.NavMenuSearchText`](../../api/en/TheOne.Avalonia.Controls.NavMenuSearchText.md) | Class | 2 | 3 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
