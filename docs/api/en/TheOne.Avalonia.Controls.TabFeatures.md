# TabFeatures

[English](../en/TheOne.Avalonia.Controls.TabFeatures.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TabFeatures.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TabFeatures` · Class · `Avalonia.AvaloniaObject`

Optional closing for native tabs and strips; reordering remains specific to TabControl.

[Source / 声明](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L13)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanClose`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | `bool` | writable; OneWay; attached | `false` | Enables closing; explicit False supported |
| [`CanReorder`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L22) | `bool` | writable; OneWay; attached | `false` | Enables pointer / keyboard ordering within a TabControl |
| [`CloseCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | `System.Windows.Input.ICommand?` | writable; OneWay; attached | `null` | Host command receives the data item and updates the collection when provided |
| [`CloseTabCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L35) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `new CloseTab()` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`IsClosable`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L23) | `bool` | writable; OneWay; attached | `true` | Per-TabItem closing; False pins the tab |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | `bool` | writable; OneWay; attached | `false` | Shows the close button; the host and associated commands retain control over closing. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool TabFeatures.GetCanClose(TabStrip tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L17) | Reads the attached property. Enables closing; explicit False supported |
| [`static void TabFeatures.SetCanClose(TabStrip tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L18) | Sets the attached property. Enables closing; explicit False supported |
| [`static bool TabFeatures.GetIsClosable(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L19) | Reads the attached property. Per-TabItem closing; False pins the tab |
| [`static void TabFeatures.SetIsClosable(TabStripItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L20) | Sets the attached property. Per-TabItem closing; False pins the tab |
| [`static bool TabFeatures.GetShowCloseButton(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L21) | Reads the attached property. Shows the close button; the host and associated commands retain control over closing. |
| [`static void TabFeatures.SetShowCloseButton(TabStripItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L22) | Sets the attached property. Shows the close button; the host and associated commands retain control over closing. |
| [`static ICommand? TabFeatures.GetCloseCommand(TabStrip tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L23) | Reads the attached property. Host command receives the data item and updates the collection when provided |
| [`static void TabFeatures.SetCloseCommand(TabStrip tabs, ICommand? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L24) | Sets the attached property. Host command receives the data item and updates the collection when provided |
| [`static bool TabFeatures.Close(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L55) | Attempts to close the container's data item |
| [`static bool TabFeatures.GetShowCloseButton(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L25) | Reads the attached property. Shows the close button; the host and associated commands retain control over closing. |
| [`static void TabFeatures.SetShowCloseButton(TabItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L26) | Sets the attached property. Shows the close button; the host and associated commands retain control over closing. |
| [`static bool TabFeatures.GetCanClose(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L50) | Reads the attached property. Enables closing; explicit False supported |
| [`static void TabFeatures.SetCanClose(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L51) | Sets the attached property. Enables closing; explicit False supported |
| [`static bool TabFeatures.GetCanReorder(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L52) | Reads the attached property. Enables pointer / keyboard ordering within a TabControl |
| [`static void TabFeatures.SetCanReorder(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L53) | Sets the attached property. Enables pointer / keyboard ordering within a TabControl |
| [`static bool TabFeatures.GetIsClosable(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L54) | Reads the attached property. Per-TabItem closing; False pins the tab |
| [`static void TabFeatures.SetIsClosable(TabItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L55) | Sets the attached property. Per-TabItem closing; False pins the tab |
| [`static ICommand? TabFeatures.GetCloseCommand(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L56) | Reads the attached property. Host command receives the data item and updates the collection when provided |
| [`static void TabFeatures.SetCloseCommand(TabControl tabs, ICommand? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L57) | Sets the attached property. Host command receives the data item and updates the collection when provided |
| [`static bool TabFeatures.Close(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L91) | Attempts to close the container's data item |
| [`static bool TabFeatures.Move(TabControl tabs, int from, int to)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L119) | Attempts reordering by zero-based indices |
| [`TabFeatures.TabFeatures()`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L13) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> TabFeatures.CanCloseProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> TabFeatures.CanReorderProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> TabFeatures.IsClosableProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> TabFeatures.ShowCloseButtonProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<ICommand?> TabFeatures.CloseCommandProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<TabClosingEventArgs> TabFeatures.TabClosingEvent`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
