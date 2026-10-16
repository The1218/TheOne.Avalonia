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
