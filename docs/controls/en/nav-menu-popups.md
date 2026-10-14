# NavMenu sidebar and cascading popups

[简体中文](../zh-cn/nav-menu-popups.md) · **English**

See [NavMenu](nav-menu.md) for basic setup.

## Collapsed sidebar and submenus

With `IsCollapsed="True"`, navigation shows only root icons, hiding labels, indentation, inline arrows, and children. Without IconTemplate, a default graphic prevents model text from being squeezed into the icon rail. Icons with children open submenus on hover by default; leaf icons retain tooltips from the original content template. Collapsing does not change model IsExpanded or current SelectedItem.

```xml
<one:NavMenu IsCollapsed="{Binding NavigationCollapsed, Mode=TwoWay}"
             ItemsSource="{Binding MenuRoots}"
             ChildrenBinding="{Binding Entries}"
             ExpandedBinding="{Binding IsExpanded, Mode=TwoWay}"
             SelectedItem="{Binding SelectedMenu, Mode=TwoWay}"
             IconTemplate="{StaticResource MenuIconTemplate}"
             ContentTemplate="{StaticResource MenuContentTemplate}" />
```

Hovering a root icon with children opens a right-side popup after 100 ms by default. Clicking it, or pressing Right / Enter / Space while focused, opens immediately. Ordinary leaves navigate directly; group icons open children without replacing the page. Each popup is a bounded NavMenu containing only that level. Hover, the right arrow, or Right opens the next level without expanding inline children or changing model IsExpanded. Each level creates and recycles its own viewport containers.

Popup selection synchronizes with root navigation. Invoking a leaf runs root ItemInvoked / ItemInvokedCommand once and closes the chain. Selecting a descendant emphasizes the root icon. Left / Esc closes one level and returns keyboard focus to its parent anchor. Outside clicks, sidebar expansion, parent disabling, root source replacement, anchor recycling, and unloading close and release popups. Explicitly opened popups return focus to their anchor on close; hover opening does not steal focus. TwoWay model bindings and the default weak cache for stable model identities preserve expansion across reopening.

Popups use native Avalonia Flyout's in-window overlay host and stay within the current window. Theme, density, icon / content templates, row height, and Motion.Enabled update even while open. Collapsing hides inline children immediately. Popups inherit Motion.Enabled, but hover delays are independent of motion, so hover navigation remains available when animations are disabled.

| Property | Small | Normal | Large |
| --- | ---: | ---: | ---: |
| CollapsedWidth | 44 | 52 | 60 |
| SubmenuWidth | 240 | 280 | 320 |
| SubmenuMaxHeight | 240 | 320 | 400 |

These properties support styles, bindings, and local overrides. The theme sets Width for collapsed mode. A consumer's fixed local Width takes precedence, so adjust both the container column and control Width. An Auto Grid column lets the icon rail follow its density.

```csharp
menu.OpenSubmenu(0);        // 打开第一个根项目的子级。
menu.SelectPath(0, 0, 999); // 折叠模式下自动打开弹层，并定位三级末项。
menu.CloseSubmenu();
```

IsSubmenuOpen and Submenu are read-only observable properties. While open, Submenu points to the current virtualized child menu; it is null after close. Follow Submenu.Submenu to observe deeper open levels. Do not retain container references as model state.

PreviewHost's title-bar “☰” toggles the real sidebar. Extensions → NavMenu provides collapse, open / close popup, hover enablement, navigation to final root / second / third-level items, and per-level container counts. Top theme / size controls work with open popups. Targeted rendering covers expansion / cascades, edge flipping, and narrow windows × three sizes × light / dark, with 48 images for this control, including multilevel search and motion frames. PreviewHost rendering also includes the main sidebar.

## Hover and cascade settings

| Property | Default | Behavior |
| --- | --- | --- |
| OpenOnHover | true | Enables hover opening for collapsed root icons and popup branches. false still permits clicks, keyboard input, and APIs. |
| HoverOpenDelay | 00:00:00.100 | Passing an icon briefly does not open it; remaining opens it after the delay. Zero is allowed. |
| HoverCloseDelay | 00:00:00.300 | Closes after leaving the anchor and popup chain. Entering any popup or returning to the anchor cancels pending closure. |

These StyledProperty values support styles, compiled bindings, and local settings. Delays are nonnegative and propagate to all cascade levels. Moving to a sibling leaf closes only the previous next level; moving to a sibling branch replaces its child popup. The close delay buffers pointer travel across gaps. Touch clicks use explicit actions rather than mouse-hover timers.

Invoking the deepest leaf sends ItemInvoked / ItemInvokedCommand to the root exactly once and closes the entire chain. Recycling or disabling anchors, removing the last child, replacing data sources, and unloading clean up the relevant popups and timers so stale models cannot reopen them after a delay.

Pointer verification:

See the [preview entry points](../../../design/PreviewHost/Program.cs) for commands and scope.

`--verify-navmenu`, `--verify-controls`, and NativeAOT consumption also include hover and cascade scenarios: three sizes × light / dark, cross-level movement, cancelled delayed closure, rapid switching, keyboard return, thousand-item distant selection, recycling, and timer cleanup on unload.

## Window edges and narrow windows

Popups prefer the previous level's opening direction. When space is insufficient, they flip if the other side will not cover visible ancestors. Root-adjacent popups also narrow to available space. Size and position stay within the client area and recalculate on resizing.

When levels cannot fit side by side, navigation shows one level at a time: the parent is temporarily hidden and hit testing disabled, and the child shows a back button. Returning restores parent selection and scrolling. A pointer resting on the newly exposed old branch does not reopen it until it leaves and re-enters. Left / Esc also returns level by level. Widening the window restores cascades.

Read-only Avalonia properties IsCompactSubmenu and SubmenuOpensLeft expose layout state. Pointer movement considers the visible popup chain and preserves close buffering through layout changes, preventing accidental closure after flipping or resizing. Each level retains a bounded viewport and recycling.

PreviewHost → Extensions → NavMenu provides window-width, right-edge placement, and edge-preview controls with 1,000 items per level. `--verify-navmenu-edge` covers all sizes, light / dark themes, both edges, widths 260 / 420 / 900, live width / height changes, back navigation, and distant selection. Full NavMenu verification includes these checks.
