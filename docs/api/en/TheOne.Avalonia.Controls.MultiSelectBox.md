# MultiSelectBox

[English](../en/TheOne.Avalonia.Controls.MultiSelectBox.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) · [API index](README.md)

`TheOne.Avalonia.Controls.MultiSelectBox` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A searchable selection over model items, with bounded tags and native virtualized results.

[Source / 声明](../../../TheOne.Avalonia/Controls/MultiSelectBox.Interaction.cs#L15)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanClear`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L223) | `bool` | read-only; OneWay | `computed / state` | Read-only empty-results state, editable-selection availability, and density minimum height |
| [`ClearText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L174) | `string` | writable; OneWay | `"清空可编辑选择"` | Accessible button names; removal format defaults to `移除 {0}`, with model text as its argument |
| [`DisplayTags`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L217) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.MultiSelectBoxItem>` | read-only; OneWay | `computed / state` | Read-only template data; `MultiSelectBoxItem.Item` is the original model, with text, enabled/selected state, and template content |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L168) | `string` | writable; OneWay | `"无匹配选项"` | Selection placeholder, search placeholder, and empty-results text; override for the host language |
| [`HasHiddenSelection`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L221) | `bool` | read-only; OneWay | `computed / state` | Read-only count/state for selections without visible tags |
| [`HasNoResults`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L222) | `bool` | read-only; OneWay | `computed / state` | Read-only empty-results state, editable-selection availability, and density minimum height |
| [`HasSelection`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L220) | `bool` | read-only; OneWay | `computed / state` | Read-only distinct selection count and nonempty state |
| [`HiddenSelectedCount`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L219) | `int` | read-only; OneWay | `computed / state` | Read-only count/state for selections without visible tags |
| [`IsDropDownOpen`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L198) | `bool` | writable; TwoWay | `false` | Selection readonly state and two-way popup state; disabling/detaching closes the popup |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L192) | `bool` | writable; OneWay | `false` | Selection readonly state and two-way popup state; disabling/detaching closes the popup |
| [`ItemAdapter`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L132) | `TheOne.Avalonia.Controls.MultiSelectItemAdapter?` | writable; OneWay | `null` | Explicit text and availability getters; text also drives search and default tags |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L224) | `double` | read-only; OneWay | `computed / state` | Read-only empty-results state, editable-selection availability, and density minimum height |
| [`ItemTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L138) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Templates for original result/tag models; declare their corresponding `x:DataType` |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L120) | `System.Collections.IEnumerable?` | writable; OneWay | `null` | Optional model `IEnumerable`; do not populate it with pre-created controls |
| [`MaxDropDownHeight`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L210) | `double` | writable; OneWay | `320` | Defaults to `320`, finite and at least 80; popup bounds also follow the host Validation: `value => double.IsFinite(value) && value >= 80` |
| [`MaxVisibleTags`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L204) | `int` | writable; OneWay | `3` | Defaults to `3`, range `0–100`; remaining selection is represented by its count Validation: `value => value is >= 0 and <= 100` |
| [`OpenText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L180) | `string` | writable; OneWay | `"展开或收起选项"` | Accessible button names; removal format defaults to `移除 {0}`, with model text as its argument |
| [`PlaceholderText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L156) | `string` | writable; OneWay | `"请选择"` | Selection placeholder, search placeholder, and empty-results text; override for the host language |
| [`RemoveTextFormat`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L186) | `string` | writable; OneWay | `"移除 {0}"` | Accessible button names; removal format defaults to `移除 {0}`, with model text as its argument |
| [`SearchPlaceholderText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L162) | `string` | writable; OneWay | `"搜索选项"` | Selection placeholder, search placeholder, and empty-results text; override for the host language |
| [`SearchText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L150) | `string` | writable; TwoWay | `""` | Defaults to empty, supports two-way binding; closing retains the query |
| [`SelectedCount`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L218) | `int` | read-only; OneWay | `computed / state` | Read-only distinct selection count and nonempty state |
| [`SelectedItemTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L144) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Templates for original result/tag models; declare their corresponding `x:DataType` |
| [`SelectedItems`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L126) | `System.Collections.IList?` | writable; TwoWay | `null` | Optional `IList`, with default two-way binding and synchronization in both directions |
| [`VisibleItems`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L216) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.MultiSelectBoxItem>` | read-only; OneWay | `computed / state` | Read-only template data; `MultiSelectBoxItem.Item` is the original model, with text, enabled/selected state, and template content |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`bool MultiSelectBox.OpenDropDown()`](../../../TheOne.Avalonia/Controls/MultiSelectBox.Interaction.cs#L137) | Opens and focuses search, or closes and returns focus to the field; opening requires attachment and enabled state |
| [`void MultiSelectBox.CloseDropDown()`](../../../TheOne.Avalonia/Controls/MultiSelectBox.Interaction.cs#L145) | Opens and focuses search, or closes and returns focus to the field; opening requires attachment and enabled state |
| [`void MultiSelectBox.RefreshItems()`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L298) | Refreshes plain collections and models that do not provide change notifications. |
| [`bool MultiSelectBox.ToggleItem(object item)`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L484) | Toggles an editable value; adding requires source membership, removal also supports selected external values; returns `false` if unchanged |
| [`bool MultiSelectBox.ClearSelection()`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L497) | Removes editable selections, retaining selected disabled items. |
| [`MultiSelectBox.MultiSelectBox()`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L115) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<SelectionChangedEventArgs>? MultiSelectBox.SelectionChanged`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L231) | Bubbling event with original models in AddedItems / RemovedItems; batch user edits produce one event |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IEnumerable?> MultiSelectBox.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IList?> MultiSelectBox.SelectedItemsProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<MultiSelectItemAdapter?> MultiSelectBox.ItemAdapterProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> MultiSelectBox.ItemTemplateProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> MultiSelectBox.SelectedItemTemplateProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> MultiSelectBox.SearchTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> MultiSelectBox.PlaceholderTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L39) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> MultiSelectBox.SearchPlaceholderTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L42) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> MultiSelectBox.EmptyTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L45) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> MultiSelectBox.ClearTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L48) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> MultiSelectBox.OpenTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L51) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> MultiSelectBox.RemoveTextFormatProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L54) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> MultiSelectBox.IsReadOnlyProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L57) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> MultiSelectBox.IsDropDownOpenProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L60) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<int> MultiSelectBox.MaxVisibleTagsProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L63) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> MultiSelectBox.MaxDropDownHeightProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L67) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<MultiSelectBox, IReadOnlyList<MultiSelectBoxItem>> MultiSelectBox.VisibleItemsProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L71) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<MultiSelectBox, IReadOnlyList<MultiSelectBoxItem>> MultiSelectBox.DisplayTagsProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L75) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<MultiSelectBox, int> MultiSelectBox.SelectedCountProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L79) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<MultiSelectBox, int> MultiSelectBox.HiddenSelectedCountProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L82) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<MultiSelectBox, bool> MultiSelectBox.HasSelectionProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L86) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<MultiSelectBox, bool> MultiSelectBox.HasHiddenSelectionProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L89) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<MultiSelectBox, bool> MultiSelectBox.HasNoResultsProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L93) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<MultiSelectBox, bool> MultiSelectBox.CanClearProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L96) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<MultiSelectBox, double> MultiSelectBox.ItemHeightProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L99) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<SelectionChangedEventArgs> MultiSelectBox.SelectionChangedEvent`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L102) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.MultiSelectBoxItem`](TheOne.Avalonia.Controls.MultiSelectBoxItem.md)
- [`TheOne.Avalonia.Controls.MultiSelectItemAdapter`](TheOne.Avalonia.Controls.MultiSelectItemAdapter.md)
