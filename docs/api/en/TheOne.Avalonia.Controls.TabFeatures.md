# TabFeatures

[English](../en/TheOne.Avalonia.Controls.TabFeatures.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TabFeatures.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TabFeatures` · Class · `Avalonia.AvaloniaObject`

Optional closing for native tabs and strips; reordering remains specific to TabControl.

[Source / 声明](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L13)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanClose`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | `bool` | writable; OneWay; attached | `false` | Enables closing; explicit False supported |
| [`CanReorder`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | `bool` | writable; OneWay; attached | `false` | Enables pointer / keyboard ordering within a TabControl |
| [`CloseCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | `System.Windows.Input.ICommand?` | writable; OneWay; attached | `null` | Host command receives the data item and updates the collection when provided |
| [`CloseTabCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L66) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `new CloseTab()` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`EnableContextMenu`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L12) | `bool` | writable; OneWay; attached | `false` | EnableContextMenu configuration on TabFeatures; type and initial value are listed here. EnableContextMenuProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Icon`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L24) | `object?` | writable; OneWay; attached | `null` | Icon content/vector geometry as defined by the signature; the host owns external bitmap lifetimes. |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L26) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay; attached | `DefaultIconTemplate` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`IsClosable`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L27) | `bool` | writable; OneWay; attached | `true` | Per-TabItem closing; False prevents closing but still allows reordering |
| [`IsIconVisible`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L30) | `bool` | read-only; OneWay; attached | `false` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsPinned`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L28) | `bool` | writable; OneWay; attached | `false` | IsPinned configuration on TabFeatures; type and initial value are listed here. IsPinnedProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L30) | `bool` | writable; OneWay; attached | `false` | Shows the close button; the host and associated commands retain control over closing. |
| [`ShowItemIcon`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L22) | `bool` | writable; OneWay; attached | `true` | ShowItemIcon configuration on TabFeatures; type and initial value are listed here. ShowItemIconProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool TabFeatures.GetCanClose(TabStrip tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L21) | Reads the attached property. Enables closing; explicit False supported |
| [`static void TabFeatures.SetCanClose(TabStrip tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L26) | Sets the attached property. Enables closing; explicit False supported |
| [`static bool TabFeatures.GetIsClosable(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L31) | Reads the attached property. Per-TabItem closing; False prevents closing but still allows reordering |
| [`static void TabFeatures.SetIsClosable(TabStripItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L36) | Sets the attached property. Per-TabItem closing; False prevents closing but still allows reordering |
| [`static bool TabFeatures.GetShowCloseButton(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L41) | Reads the attached property. Shows the close button; the host and associated commands retain control over closing. |
| [`static void TabFeatures.SetShowCloseButton(TabStripItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L46) | Sets the attached property. Shows the close button; the host and associated commands retain control over closing. |
| [`static ICommand? TabFeatures.GetCloseCommand(TabStrip tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L51) | Reads the attached property. Host command receives the data item and updates the collection when provided |
| [`static void TabFeatures.SetCloseCommand(TabStrip tabs, ICommand? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L56) | Sets the attached property. Host command receives the data item and updates the collection when provided |
| [`static bool TabFeatures.Close(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L109) | Attempts to close the container's data item |
| [`static bool TabFeatures.GetShowItemIcon(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L37) | Reads the attached property. ShowItemIcon configuration on TabFeatures; type and initial value are listed here. ShowItemIconProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`static bool TabFeatures.GetEnableContextMenu(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L38) | Reads the attached property. EnableContextMenu configuration on TabFeatures; type and initial value are listed here. EnableContextMenuProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`static void TabFeatures.SetEnableContextMenu(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L39) | Sets the attached property. EnableContextMenu configuration on TabFeatures; type and initial value are listed here. EnableContextMenuProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`static void TabFeatures.SetShowItemIcon(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L40) | Sets the attached property. ShowItemIcon configuration on TabFeatures; type and initial value are listed here. ShowItemIconProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`static object? TabFeatures.GetIcon(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L41) | Reads the attached property. Icon content/vector geometry as defined by the signature; the host owns external bitmap lifetimes. |
| [`static void TabFeatures.SetIcon(TabItem item, object? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L42) | Sets the attached property. Icon content/vector geometry as defined by the signature; the host owns external bitmap lifetimes. |
| [`static IDataTemplate? TabFeatures.GetIconTemplate(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L43) | Reads the attached property. Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`static void TabFeatures.SetIconTemplate(TabItem item, IDataTemplate? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L44) | Sets the attached property. Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`static bool TabFeatures.GetIsPinned(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L45) | Reads the attached property. IsPinned configuration on TabFeatures; type and initial value are listed here. IsPinnedProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`static void TabFeatures.SetIsPinned(TabItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L46) | Sets the attached property. IsPinned configuration on TabFeatures; type and initial value are listed here. IsPinnedProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`static bool TabFeatures.GetIsIconVisible(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L47) | Reads the attached property. Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`static bool TabFeatures.Pin(TabItem item, bool pinned = true)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L136) | Pins a page before all unpinned pages, preserving selection. Does not require drag reordering. |
| [`static int TabFeatures.CloseAll(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L155) | Closes a snapshot of closable pages, honoring cancellation, commands and pin state. |
| [`static bool TabFeatures.GetShowCloseButton(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L68) | Reads the attached property. Shows the close button; the host and associated commands retain control over closing. |
| [`static void TabFeatures.SetShowCloseButton(TabItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L73) | Sets the attached property. Shows the close button; the host and associated commands retain control over closing. |
| [`static bool TabFeatures.GetCanClose(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L85) | Reads the attached property. Enables closing; explicit False supported |
| [`static void TabFeatures.SetCanClose(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L90) | Sets the attached property. Enables closing; explicit False supported |
| [`static bool TabFeatures.GetCanReorder(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L95) | Reads the attached property. Enables pointer / keyboard ordering within a TabControl |
| [`static void TabFeatures.SetCanReorder(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L100) | Sets the attached property. Enables pointer / keyboard ordering within a TabControl |
| [`static bool TabFeatures.GetIsClosable(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L105) | Reads the attached property. Per-TabItem closing; False prevents closing but still allows reordering |
| [`static void TabFeatures.SetIsClosable(TabItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L110) | Sets the attached property. Per-TabItem closing; False prevents closing but still allows reordering |
| [`static ICommand? TabFeatures.GetCloseCommand(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L115) | Reads the attached property. Host command receives the data item and updates the collection when provided |
| [`static void TabFeatures.SetCloseCommand(TabControl tabs, ICommand? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L120) | Sets the attached property. Host command receives the data item and updates the collection when provided |
| [`static bool TabFeatures.Close(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L182) | Attempts to close the container's data item |
| [`static bool TabFeatures.Move(TabControl tabs, int from, int to)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L224) | Attempts reordering by zero-based indices |
| [`TabFeatures.TabFeatures()`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L13) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> TabFeatures.EnableContextMenuProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> TabFeatures.ShowItemIconProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<object?> TabFeatures.IconProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<IDataTemplate?> TabFeatures.IconTemplateProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> TabFeatures.IsPinnedProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> TabFeatures.IsIconVisibleProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> TabFeatures.CanCloseProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> TabFeatures.CanReorderProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> TabFeatures.IsClosableProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> TabFeatures.ShowCloseButtonProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<ICommand?> TabFeatures.CloseCommandProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<TabClosingEventArgs> TabFeatures.TabClosingEvent`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
