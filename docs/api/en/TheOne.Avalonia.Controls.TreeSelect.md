# TreeSelect

[English](../en/TheOne.Avalonia.Controls.TreeSelect.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TreeSelect.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TreeSelect` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A searchable tree selector with stable keys and virtualized, flattened visible rows.

[Source / 声明](../../../TheOne.Avalonia/Controls/TreeSelect.Interaction.cs#L16)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanClear`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L132) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CheckMode`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L35) | `TheOne.Avalonia.Controls.TreeSelectCheckMode` | writable; OneWay | `TreeSelectCheckMode.Independent` | CheckMode configuration on TreeSelect; type and initial value are listed here. CheckModeProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => Enum.IsDefined(value)` Values: `Independent`, `Cascade` |
| [`ClearText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L67) | `string` | writable; OneWay | `"清空可编辑选择"` | ClearText configuration on TreeSelect; type and initial value are listed here. ClearTextProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`DisplayTags`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L97) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.TreeSelectItem>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L63) | `string` | writable; OneWay | `"无匹配节点"` | Message when no data can be displayed; applications override it for localization. |
| [`HasHiddenSelection`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L122) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasNoResults`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L127) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasSelection`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L117) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HiddenSelectedCount`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L112) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsDropDownOpen`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L79) | `bool` | writable; TwoWay | `false` | IsDropDownOpen configuration on TreeSelect; type and initial value are listed here. IsDropDownOpenProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`IsMultiSelect`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L31) | `bool` | writable; OneWay | `false` | IsMultiSelect configuration on TreeSelect; type and initial value are listed here. IsMultiSelectProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L75) | `bool` | writable; OneWay | `false` | Whether editing is blocked; the control's read-only contract determines remaining display, selection and navigation behavior. |
| [`ItemAdapter`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L39) | `TheOne.Avalonia.Controls.TreeSelectItemAdapter?` | writable; OneWay | `null` | ItemAdapter configuration on TreeSelect; type and initial value are listed here. ItemAdapterProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L142) | `double` | read-only; OneWay | `computed / state` | Item/wheel-cell height in DIPs; theme density or explicit values may override registration defaults. |
| [`ItemTemplate`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L43) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L15) | `System.Collections.IEnumerable?` | writable; OneWay | `null` | Bind source models rather than precreated controls; use notified collections for changes and a bounded viewport. |
| [`MaxDropDownHeight`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L87) | `double` | writable; OneWay | `320` | MaxDropDownHeight configuration on TreeSelect; type and initial value are listed here. MaxDropDownHeightProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => double.IsFinite(value) && value >= 80` |
| [`MaxVisibleTags`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L83) | `int` | writable; OneWay | `3` | MaxVisibleTags configuration on TreeSelect; type and initial value are listed here. MaxVisibleTagsProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => value is >= 0 and <= 100` |
| [`OpenText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L71) | `string` | writable; OneWay | `"展开或收起树"` | OpenText configuration on TreeSelect; type and initial value are listed here. OpenTextProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`PlaceholderText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L55) | `string` | writable; OneWay | `"请选择"` | PlaceholderText configuration on TreeSelect; type and initial value are listed here. PlaceholderTextProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`SearchPlaceholderText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L59) | `string` | writable; OneWay | `"搜索节点"` | SearchPlaceholderText configuration on TreeSelect; type and initial value are listed here. SearchPlaceholderTextProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`SearchText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L51) | `string` | writable; TwoWay | `""` | Current search text, independent of selection and model identity. |
| [`SelectedCount`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L107) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectedItem`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L27) | `object?` | writable; TwoWay | `null` | Currently selected model object; retain identity rather than substituting display text. Null represents no selection. |
| [`SelectedItemTemplate`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L47) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`SelectedItems`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L102) | `System.Collections.Generic.IReadOnlyList<object>` | read-only; OneWay | `computed / state` | Selected model collection; elements belong to the source and mutations follow disabled/read-only selection semantics. |
| [`SelectedKey`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L23) | `object?` | writable; TwoWay | `null` | SelectedKey configuration on TreeSelect; type and initial value are listed here. SelectedKeyProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`SelectedKeys`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L19) | `System.Collections.IList?` | writable; TwoWay | `null` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L137) | `string?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`VisibleItems`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L92) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.TreeSelectItem>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`bool TreeSelect.OpenDropDown()`](../../../TheOne.Avalonia/Controls/TreeSelect.Interaction.cs#L105) | Opens the multiselect popup and focuses search; requires an attached/enabled control and reports whether it opened. |
| [`void TreeSelect.CloseDropDown()`](../../../TheOne.Avalonia/Controls/TreeSelect.Interaction.cs#L111) | Closes the multiselect popup and restores field focus according to interaction context. |
| [`void TreeSelect.RefreshItems()`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L104) | Re-read model structure and mapping; search alone never rebuilds subscriptions. |
| [`bool TreeSelect.ToggleItem(object item)`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L281) | Toggle a model's selection. Cascade affects enabled leaves only; disabled selections remain locked. |
| [`bool TreeSelect.ClearSelection()`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L295) | Clear editable selections; selected disabled nodes remain. |
| [`bool TreeSelect.SetExpanded(object key, bool expanded)`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L310) | The signature specifies parameter types, optional defaults and return types; see the owning usage contract and linked implementation for operation/error boundaries. |
| [`TreeSelect.TreeSelect()`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L34) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<SelectionChangedEventArgs>? TreeSelect.SelectionChanged`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L19) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IEnumerable?> TreeSelect.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IList?> TreeSelect.SelectedKeysProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> TreeSelect.SelectedKeyProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> TreeSelect.SelectedItemProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> TreeSelect.IsMultiSelectProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TreeSelectCheckMode> TreeSelect.CheckModeProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TreeSelectItemAdapter?> TreeSelect.ItemAdapterProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L37) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> TreeSelect.ItemTemplateProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> TreeSelect.SelectedItemTemplateProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L45) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> TreeSelect.SearchTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L49) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> TreeSelect.PlaceholderTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L53) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> TreeSelect.SearchPlaceholderTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L57) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> TreeSelect.EmptyTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L61) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> TreeSelect.ClearTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L65) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> TreeSelect.OpenTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L69) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> TreeSelect.IsReadOnlyProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L73) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> TreeSelect.IsDropDownOpenProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L77) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<int> TreeSelect.MaxVisibleTagsProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L81) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> TreeSelect.MaxDropDownHeightProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L85) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<TreeSelect, IReadOnlyList<TreeSelectItem>> TreeSelect.VisibleItemsProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L89) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<TreeSelect, IReadOnlyList<TreeSelectItem>> TreeSelect.DisplayTagsProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L94) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<TreeSelect, IReadOnlyList<object>> TreeSelect.SelectedItemsProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L99) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<TreeSelect, int> TreeSelect.SelectedCountProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L104) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<TreeSelect, int> TreeSelect.HiddenSelectedCountProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L109) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<TreeSelect, bool> TreeSelect.HasSelectionProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L114) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<TreeSelect, bool> TreeSelect.HasHiddenSelectionProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L119) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<TreeSelect, bool> TreeSelect.HasNoResultsProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L124) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<TreeSelect, bool> TreeSelect.CanClearProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L129) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<TreeSelect, string?> TreeSelect.ValidationErrorProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L134) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<TreeSelect, double> TreeSelect.ItemHeightProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L139) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<SelectionChangedEventArgs> TreeSelect.SelectionChangedEvent`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.TreeSelectCheckMode`](TheOne.Avalonia.Controls.TreeSelectCheckMode.md)
- [`TheOne.Avalonia.Controls.TreeSelectItem`](TheOne.Avalonia.Controls.TreeSelectItem.md)
- [`TheOne.Avalonia.Controls.TreeSelectItemAdapter`](TheOne.Avalonia.Controls.TreeSelectItemAdapter.md)
