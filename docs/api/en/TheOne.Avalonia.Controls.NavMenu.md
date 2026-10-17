# NavMenu

[English](../en/TheOne.Avalonia.Controls.NavMenu.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavMenu.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavMenu` · Class · `Avalonia.Controls.TreeView`

A native virtualized navigation tree with separately templated icons and labels.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L19)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

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

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`bool NavMenu.OpenSubmenu(int rootIndex)`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L63) | Open a collapsed root's children without selecting or expanding its model. |
| [`void NavMenu.CloseSubmenu()`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L69) | Closes the current cascading submenu and clears popup state while retaining selection. |
| [`bool NavMenu.OpenFilter()`](../../../TheOne.Avalonia/Controls/NavMenu.FilterPopup.cs#L64) | Focuses the inline filter, or opens its compact search popup. |
| [`void NavMenu.CloseFilter()`](../../../TheOne.Avalonia/Controls/NavMenu.FilterPopup.cs#L87) | Dismisses compact search and restores the pre-search navigation state. |
| [`static NavMenu? NavMenu.GetSearchOwner(TextBox input)`](../../../TheOne.Avalonia/Controls/NavMenu.SearchInput.cs#L12) | Reads the attached property. Menu instance associated with search results, used to locate and activate original items. |
| [`static void NavMenu.SetSearchOwner(TextBox input, NavMenu? menu)`](../../../TheOne.Avalonia/Controls/NavMenu.SearchInput.cs#L13) | Sets the attached property. Menu instance associated with search results, used to locate and activate original items. |
| [`bool NavMenu.SelectPath(params int[] path)`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L81) | Realize only an ancestor path, then select its target data item. |
| [`NavMenu.NavMenu()`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L45) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<NavMenuItemInvokedEventArgs>? NavMenu.ItemInvoked`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L77) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<bool> NavMenu.IsCollapsedProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> NavMenu.CollapsedWidthProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> NavMenu.SubmenuWidthProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> NavMenu.SubmenuMaxHeightProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<NavMenu, bool> NavMenu.IsSubmenuOpenProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<NavMenu, NavMenu?> NavMenu.SubmenuProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<NavMenu, IDataTemplate?> NavMenu.DisplayIconTemplateProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<NavMenu, bool> NavMenu.IsCompactSubmenuProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L45) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<NavMenu, bool> NavMenu.SubmenuOpensLeftProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L46) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> NavMenu.ShowFilterProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> NavMenu.FilterPlaceholderProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<NavMenu, IReadOnlyList<NavMenuFilterResult>> NavMenu.FilterResultsProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<NavMenu, bool> NavMenu.IsFilterOpenProperty`](../../../TheOne.Avalonia/Controls/NavMenu.FilterPopup.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> NavMenu.OpenOnHoverProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TimeSpan> NavMenu.HoverOpenDelayProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TimeSpan> NavMenu.HoverCloseDelayProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TimeSpan> NavMenu.TransitionDurationProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<NavMenu, double> NavMenu.TransitionExtentProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> NavMenu.SearchTextProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> NavMenu.SearchTextBindingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<NavMenu, bool> NavMenu.IsSearchingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<NavMenu, int> NavMenu.SearchMatchCountProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<NavMenu?> NavMenu.SearchOwnerProperty`](../../../TheOne.Avalonia/Controls/NavMenu.SearchInput.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> NavMenu.IconTemplateProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> NavMenu.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> NavMenu.ChildrenBindingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> NavMenu.ExpandedBindingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> NavMenu.IsGroupBindingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> NavMenu.IsItemEnabledBindingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> NavMenu.ItemHeightProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> NavMenu.IconSizeProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> NavMenu.IconSpacingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> NavMenu.IndentProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ICommand?> NavMenu.ItemInvokedCommandProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<NavMenuItemInvokedEventArgs> NavMenu.ItemInvokedEvent`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L37) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.NavMenuFilterBox`](TheOne.Avalonia.Controls.NavMenuFilterBox.md)
- [`TheOne.Avalonia.Controls.NavMenuFilterResult`](TheOne.Avalonia.Controls.NavMenuFilterResult.md)
- [`TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs`](TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs.md)

## Navigation additions

[Usage and examples](../../controls/en/nav-menu.md)

| Name | Type | Default / binding |
| --- | --- | --- |
| [DisplayWidth](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L31) | `double` | `computed / state`; OneWay |
| [ExpandedWidth](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L17) | `double` | `280`; OneWay |
