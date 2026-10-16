# NavMenu motion

[简体中文](../zh-cn/nav-menu-motion.md) · **English**

See [NavMenu](nav-menu.md) for basic setup.

## Expanding, collapsing, and switching the sidebar

`TransitionDuration` is a styleable TimeSpan that supports compiled binding. The theme defaults to `One.Motion.Expand` (200 ms); collapse takes 70% of that duration. Zero switches immediately. `Motion.Enabled=False` immediately reaches the current target even during animation; re-enabling it does not replay prior actions.

Inline expansion of a thousand children travels across a bounded viewport while reusing the full child layout. It does not animate tens of thousands of list pixels, avoiding abrupt first-screen appearance or delayed completion. The full native scrolling range returns when complete; child hit testing is disabled immediately during collapse. Rapid reversals continue from current progress. Explicit SelectPath / distant keyboard navigation completes ancestor transitions before locating the item. Read-only TransitionExtent reports the menu's bounded viewport height without changing the final list extent.

Sidebar sizing and navigation mode update together. Icons already in view translate from their current rendered position to the new one; labels fade in on expansion. Page Width / Height do not animate per frame, and icons / text do not scale. A single progress animation drives the change; recycled items do not replay entry animations. Size changes, disabling motion, unloading, and recycling clear temporary transforms and opacity while preserving selection and business expansion state.

PreviewHost's dedicated NavMenu page provides actions to expand / collapse a thousand children, expand / collapse three levels, and rapidly reverse the sidebar. The top motion switch tests disabling motion midway. Verification uses real Dispatcher frames and Skia advancement, covering Light / Dark × three sizes, actual thousand-item recycling, intermediate frames, reversal, full extent restoration, focus / selection, and unload cleanup:

See the [preview entry points](../../../design/PreviewHost/Program.cs) for commands and scope.

Targeted rendering includes intermediate collapse, expansion, and sidebar frames, plus built-in search suggestions and the collapsed search icon / popup. The current NavMenu set contains 66 images and does not render unrelated controls.

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

### NavMenuItem

[API / model reference](../../api/en/TheOne.Avalonia.Controls.NavMenuItem.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsActiveAncestor`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L22) | `bool` | writable; OneWay | `false` | Whether this item is an ancestor of the selected item; menu-owned and distinct from child selection. |
| [`IsCascadeItem`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L20) | `bool` | writable; OneWay | `false` | Whether this container belongs to a cascading popup, maintained by the menu/template. |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L21) | `bool` | writable; OneWay | `false` | Enables compact layout; the template determines visible fields without altering business data. |
| [`IsGroup`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L23) | `bool` | writable; OneWay | `false` | Whether this menu item is a group, distinct from selected state. |

### RevealHost

[API / model reference](../../api/en/TheOne.Avalonia.Controls.RevealHost.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ConstrainContent`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L31) | `bool` | writable; OneWay | `false` | Preserve a finite parent viewport for content such as a virtualized list. |
| [`Direction`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L28) | `Avalonia.Controls.ExpandDirection` | writable; OneWay | `ExpandDirection.Down` | Reveal/port direction enum; choices are defined by its concrete enum type. Values: `Down`, `Up`, `Left`, `Right` |
| [`Duration`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L29) | `System.TimeSpan` | writable; OneWay | `TimeSpan.FromMilliseconds(200)` | Duration; motion still follows inherited Motion.Enabled rather than using duration as its only switch. |
| [`IsOpen`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L27) | `bool` | writable; OneWay | `false` | Expands/shows content; collapsing retains the content object and transitions follow Motion.Enabled. |
| [`TransitionExtent`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L22) | `double` | writable; OneWay | `double.PositiveInfinity` | Optional visible-distance cap during animation; the settled layout retains its full extent. Validation: `value => value > 0 && !double.IsNaN(value)` |

### Motion

[API / model reference](../../api/en/TheOne.Avalonia.Styling.Motion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | writable; OneWay; attached; inherits | `true` | Inherited motion switch; disabling goes to final layout/state while preserving native input. |

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
| [`TheOne.Avalonia.Controls.RevealHost`](../../api/en/TheOne.Avalonia.Controls.RevealHost.md) | Class | 5 | 1 | 0 |
| [`TheOne.Avalonia.Styling.Motion`](../../api/en/TheOne.Avalonia.Styling.Motion.md) | Class | 1 | 3 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
