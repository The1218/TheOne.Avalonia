# TreeSelect

[简体中文](../zh-cn/tree-select.md) · [Controls](README.md) · [Roadmap](../../plans/control-and-chart-roadmap.md)

Load OneTheme and use `TheOne.Avalonia.Controls.TreeSelect`. Single selection accepts any enabled node; multiple selection supports Independent or Cascade checks. Stable keys remap selection to new model instances after source replacement. Visible rows use a bounded native virtual ListBox, not a Control per tree node.

## Default model and binding

```csharp
public ObservableCollection<TreeSelectNode> Departments { get; } =
[
    new() { Key = "engineering", Text = "Engineering", Children =
    [ new() { Key = "frontend", Text = "Frontend" }, new() { Key = "backend", Text = "Backend" } ] }
];
```

```xml
<!-- Declare the actual ViewModel x:DataType on the view. -->
<one:TreeSelect ItemsSource="{CompiledBinding Departments}"
                SelectedKey="{CompiledBinding DepartmentKey, Mode=TwoWay}" />
<one:TreeSelect IsMultiSelect="True" CheckMode="Cascade" MaxVisibleTags="1"
                ItemsSource="{CompiledBinding Departments}"
                SelectedKeys="{CompiledBinding DepartmentKeys, Mode=TwoWay}" />
```

TreeSelectNode notifies property changes and exposes Key (object), Text (string), IsEnabled (bool, true) and replaceable Children (ObservableCollection&lt;TreeSelectNode&gt;?, initially empty). Null children mean a leaf. Keys must be non-null, unique by Equals and stable. Default generated keys last for the instance only; assign business keys for persistence/reload and do not mutate mounted keys.

Bind SelectedKey or SelectedItem for single selection, SelectedKeys for multiple. Avoid conflicting bindings to these entrances. SelectedItems is read-only and returns original models. In multiple mode SelectedKey/SelectedItem represent the first item; writing either replaces the selection. Switching to single keeps the first item. SelectedKeys accepts IList with TwoWay replacement and observable in-place edits. Writable lists are mutated where possible; fixed/read-only lists are replaced when needed. Duplicate/missing keys are normalized against a valid source. Pre-attachment configuration is supported; keys may wait while ItemsSource is null. The first non-null source prunes missing keys; clearing a loaded source clears selection.

## Custom models and compiled templates

```csharp
public TreeSelectItemAdapter DepartmentAdapter { get; } =
    TreeSelectItemAdapter.Create<Department>(
        item => item.Id, item => item.Name, item => item.Teams, item => item.IsEnabled);
```

```xml
<one:TreeSelect ItemsSource="{CompiledBinding Departments}"
                ItemAdapter="{CompiledBinding DepartmentAdapter}"
                SelectedKey="{CompiledBinding DepartmentKey, Mode=TwoWay}">
  <one:TreeSelect.ItemTemplate>
    <DataTemplate x:DataType="vm:Department">
      <TextBlock Text="{CompiledBinding Name}" />
    </DataTemplate>
  </one:TreeSelect.ItemTemplate>
</one:TreeSelect>
```

The adapter maps key, display/search text, IEnumerable? children and optional enabled state using typed delegates, without property-name reflection. ItemTemplate and SelectedItemTemplate receive original models and require their own x:DataType; both default to adapter text. Without an adapter, models must be TreeSelectNode.

## Added properties

| Property | Default / meaning |
| --- | --- |
| ItemsSource / ItemAdapter | IEnumerable? roots / optional typed mapping |
| IsMultiSelect / CheckMode | false / Independent; check mode is ignored for single selection |
| SelectedKey / SelectedItem | object?, TwoWay; key / original model |
| SelectedKeys | IList?, TwoWay; independent observable default list |
| ItemTemplate / SelectedItemTemplate | null; list / tag templates |
| SearchText | empty, TwoWay; case-insensitive search preserving ancestors |
| PlaceholderText / SearchPlaceholderText | “请选择” / “搜索节点”; consumer-localizable |
| EmptyText / ClearText / OpenText | “无匹配节点” / “清空可编辑选择” / “展开或收起树”; empty and accessible action labels |
| IsReadOnly / IsDropDownOpen | false / false; browse without editing / TwoWay popup state |
| MaxVisibleTags | 3, 0–100; single shows one, multiple overflow shows +N; 0 shows count only |
| MaxDropDownHeight | 320 DIP, finite and ≥80; bounded popup viewport |

```xml
<one:TreeSelect ItemsSource="{CompiledBinding Departments}" IsMultiSelect="True"
                SearchText="{CompiledBinding Filter, Mode=TwoWay}" MaxVisibleTags="1"
                MaxDropDownHeight="280" IsReadOnly="{CompiledBinding IsLocked}" />
```

Independent stores parent/child keys independently. Cascade toggles all enabled descendant leaves and stores leaf keys only; supplied parent keys expand to enabled leaves. Parents derive checked/unchecked/indeterminate state from enabled leaves. Disabled nodes lock selection of their subtree but remain expandable. Preselected disabled leaves cannot be removed or cleared and do not affect parent full-check calculation. Search does not limit cascade scope; clearing search restores stored expansion.

No extra public appearance classes; themes, palettes and densities follow Home. Read-only state: VisibleItems, DisplayTags, SelectedItems, SelectedCount, HiddenSelectedCount, HasSelection, HasHiddenSelection, HasNoResults, CanClear, ValidationError and ItemHeight. TreeSelectItem exposes original Item, Key, Text, Depth/Indent, HasChildren, IsExpanded, IsEnabled, nullable IsChecked, ShowCheckBox, CanRemove, action labels and template presentation. Persist model keys, not presentation rows.

## Actions and keyboard

```csharp
tree.SetExpanded("engineering", true); // bool; stored expansion is unchanged during search
bool opened = tree.OpenDropDown();
tree.CloseDropDown();
bool changed = tree.ToggleItem(department); // Original model; respects read-only/disabled state
bool cleared = tree.ClearSelection(); // Retains disabled selections
tree.RefreshItems(); // Explicitly re-read non-notifying models
tree.SelectionChanged += (_, e) => SaveKeys(tree.SelectedKeys);
```

SelectionChanged bubbles with original models in AddedItems/RemovedItems. APIs and source/selection changes run on the UI thread. Enter/Space/Down, Alt+Down or F4 opens; Down from search enters results; Up/Down navigates, Right expands, Left collapses or moves to the parent, Enter/Space selects. Escape closes and restores focus; Tab/Shift+Tab continues focus traversal. Backspace on the focused field with empty search removes the last editable tag. Expanding never selects. Wheel, touch-scroll and inertia phases are contained at the popup boundary.

## Source lifecycle and scope

Observable Add/Remove/Move/Replace/Reset, root/children replacement and INotifyPropertyChanged rebuild the index, retain valid selected/expanded keys and release old subscriptions. Search filters the cache without re-reading adapters or rebuilding subscriptions. Detaching unsubscribes; attaching reads current data. Duplicate keys, null nodes, cycles and repeated model references report ValidationError without stale rows; observable repairs recover.

Model refresh scans the full tree. Row views are virtualized, not data fetching. There is no built-in async child loader, drag ordering or service client. Hosts can load observable Children and own request cancellation. Indentation is capped for very deep trees. The separate PreviewHost page shows single, independent/cascade multiple, typed models, bounded tags and an on-demand 10,000-node example.

Exact declarations: [TreeSelect](../../api/en/TheOne.Avalonia.Controls.TreeSelect.md), [default model](../../api/en/TheOne.Avalonia.Controls.TreeSelectNode.md), [adapter](../../api/en/TheOne.Avalonia.Controls.TreeSelectItemAdapter.md), [row state](../../api/en/TheOne.Avalonia.Controls.TreeSelectItem.md), [check modes](../../api/en/TheOne.Avalonia.Controls.TreeSelectCheckMode.md).

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### TreeSelect

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.TreeSelect.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanClear`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L132) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CheckMode`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L35) | `TheOne.Avalonia.Controls.TreeSelectCheckMode` | writable; OneWay | `TreeSelectCheckMode.Independent` | false / Independent; check mode is ignored for single selection Validation: `value => Enum.IsDefined(value)` Values: `Independent`, `Cascade` |
| [`ClearText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L67) | `string` | writable; OneWay | `"清空可编辑选择"` | “无匹配节点” / “清空可编辑选择” / “展开或收起树”; empty and accessible action labels |
| [`DisplayTags`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L97) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.TreeSelectItem>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L63) | `string` | writable; OneWay | `"无匹配节点"` | “无匹配节点” / “清空可编辑选择” / “展开或收起树”; empty and accessible action labels |
| [`HasHiddenSelection`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L122) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasNoResults`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L127) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasSelection`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L117) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HiddenSelectedCount`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L112) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsDropDownOpen`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L79) | `bool` | writable; TwoWay | `false` | false / false; browse without editing / TwoWay popup state |
| [`IsMultiSelect`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L31) | `bool` | writable; OneWay | `false` | false / Independent; check mode is ignored for single selection |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L75) | `bool` | writable; OneWay | `false` | false / false; browse without editing / TwoWay popup state |
| [`ItemAdapter`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L39) | `TheOne.Avalonia.Controls.TreeSelectItemAdapter?` | writable; OneWay | `null` | IEnumerable? roots / optional typed mapping |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L142) | `double` | read-only; OneWay | `computed / state` | Item/wheel-cell height in DIPs; theme density or explicit values may override registration defaults. |
| [`ItemTemplate`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L43) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | null; list / tag templates |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L15) | `System.Collections.IEnumerable?` | writable; OneWay | `null` | IEnumerable? roots / optional typed mapping |
| [`MaxDropDownHeight`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L87) | `double` | writable; OneWay | `320` | 320 DIP, finite and ≥80; bounded popup viewport Validation: `value => double.IsFinite(value) && value >= 80` |
| [`MaxVisibleTags`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L83) | `int` | writable; OneWay | `3` | 3, 0–100; single shows one, multiple overflow shows +N; 0 shows count only Validation: `value => value is >= 0 and <= 100` |
| [`OpenText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L71) | `string` | writable; OneWay | `"展开或收起树"` | “无匹配节点” / “清空可编辑选择” / “展开或收起树”; empty and accessible action labels |
| [`PlaceholderText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L55) | `string` | writable; OneWay | `"请选择"` | “请选择” / “搜索节点”; consumer-localizable |
| [`SearchPlaceholderText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L59) | `string` | writable; OneWay | `"搜索节点"` | “请选择” / “搜索节点”; consumer-localizable |
| [`SearchText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L51) | `string` | writable; TwoWay | `""` | empty, TwoWay; case-insensitive search preserving ancestors |
| [`SelectedCount`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L107) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectedItem`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L27) | `object?` | writable; TwoWay | `null` | object?, TwoWay; key / original model |
| [`SelectedItemTemplate`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L47) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | null; list / tag templates |
| [`SelectedItems`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L102) | `System.Collections.Generic.IReadOnlyList<object>` | read-only; OneWay | `computed / state` | Selected model collection; elements belong to the source and mutations follow disabled/read-only selection semantics. |
| [`SelectedKey`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L23) | `object?` | writable; TwoWay | `null` | object?, TwoWay; key / original model |
| [`SelectedKeys`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L19) | `System.Collections.IList?` | writable; TwoWay | `null` | IList?, TwoWay; independent observable default list |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L137) | `string?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`VisibleItems`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L92) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.TreeSelectItem>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### TreeSelectCheckMode

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.TreeSelectCheckMode.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### TreeSelectItem

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.TreeSelectItem.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanRemove`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L28) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L33) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; CLR / n/a | `accessor-defined / state` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`Depth`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L21) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Node depth starting at zero for roots, used to calculate indentation. |
| [`DisplayContent`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L32) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ExpandText`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L30) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ExpansionGlyph`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L29) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasChildren`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L23) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Whether a node has children, controlling expansion affordances independently of current expansion. |
| [`Indent`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L22) | `Avalonia.Thickness` | read-only; CLR / n/a | `accessor-defined / state` | Additional DIP indentation per hierarchy level, without changing parent/child relationships. |
| [`IsChecked`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L26) | `bool?` | read-only; CLR / n/a | `accessor-defined / state` | Checked state of a report checkbox element. |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L24) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Business availability; selection, removal and commands follow the control's disabled-item rules. |
| [`IsExpanded`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L25) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Current node expansion; bind to a stable model when it must survive recycling or reload. |
| [`Item`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L18) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Key`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L19) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Field/parameter key used to resolve business values, distinct from display names. |
| [`RemoveText`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L31) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ShowCheckBox`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L27) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`TagContent`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L34) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`TagTemplate`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L35) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; CLR / n/a | `accessor-defined / state` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`Text`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L20) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Input/display text; notifications and validation follow the model/native editor contract. |

### TreeSelectItemAdapter

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.TreeSelectItemAdapter.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### TreeSelectNode

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.TreeSelectNode.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Children`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L17) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Controls.TreeSelectNode>?` | writable; CLR / n/a | `[]` | Child model collection; retain identity/parent relationships and avoid cycles/shared mutable nodes. |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L16) | `bool` | writable; CLR / n/a | `true` | Business availability; selection, removal and commands follow the control's disabled-item rules. |
| [`Key`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L14) | `object` | writable; CLR / n/a | `Guid.NewGuid().ToString("N")` | Field/parameter key used to resolve business values, distinct from display names. |
| [`Text`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L15) | `string` | writable; CLR / n/a | `""` | Input/display text; notifications and validation follow the model/native editor contract. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`TreeSelect`](../../api/en/TheOne.Avalonia.Controls.TreeSelect.md) | `bool TreeSelect.OpenDropDown()` | Opens the multiselect popup and focuses search; requires an attached/enabled control and reports whether it opened. |
| [`TreeSelect`](../../api/en/TheOne.Avalonia.Controls.TreeSelect.md) | `void TreeSelect.CloseDropDown()` | Closes the multiselect popup and restores field focus according to interaction context. |
| [`TreeSelect`](../../api/en/TheOne.Avalonia.Controls.TreeSelect.md) | `void TreeSelect.RefreshItems()` | Re-read model structure and mapping; search alone never rebuilds subscriptions. |
| [`TreeSelect`](../../api/en/TheOne.Avalonia.Controls.TreeSelect.md) | `bool TreeSelect.ToggleItem(object item)` | Toggle a model's selection. Cascade affects enabled leaves only; disabled selections remain locked. |
| [`TreeSelect`](../../api/en/TheOne.Avalonia.Controls.TreeSelect.md) | `bool TreeSelect.ClearSelection()` | Clear editable selections; selected disabled nodes remain. |
| [`TreeSelect`](../../api/en/TheOne.Avalonia.Controls.TreeSelect.md) | `bool TreeSelect.SetExpanded(object key, bool expanded)` |  |
| [`TreeSelectItemAdapter`](../../api/en/TheOne.Avalonia.Controls.TreeSelectItemAdapter.md) | `abstract object TreeSelectItemAdapter.GetKey(object item)` |  |
| [`TreeSelectItemAdapter`](../../api/en/TheOne.Avalonia.Controls.TreeSelectItemAdapter.md) | `abstract string TreeSelectItemAdapter.GetText(object item)` | Reads original-model display/search text through the configured typed adapter. |
| [`TreeSelectItemAdapter`](../../api/en/TheOne.Avalonia.Controls.TreeSelectItemAdapter.md) | `abstract IEnumerable? TreeSelectItemAdapter.GetChildren(object item)` |  |
| [`TreeSelectItemAdapter`](../../api/en/TheOne.Avalonia.Controls.TreeSelectItemAdapter.md) | `virtual bool TreeSelectItemAdapter.IsEnabled(object item)` | Uses the adapter to determine whether an original model permits user selection. |

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`TreeSelect`](../../api/en/TheOne.Avalonia.Controls.TreeSelect.md) | `SelectionChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`TreeSelectItem`](../../api/en/TheOne.Avalonia.Controls.TreeSelectItem.md) | `PropertyChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`TreeSelectNode`](../../api/en/TheOne.Avalonia.Controls.TreeSelectNode.md) | `PropertyChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.TreeSelectCheckMode`](../../api/en/TheOne.Avalonia.Controls.TreeSelectCheckMode.md) | 2 |
| [`TheOne.Avalonia.Controls.TreeSelectItem`](../../api/en/TheOne.Avalonia.Controls.TreeSelectItem.md) | 18 |
| [`TheOne.Avalonia.Controls.TreeSelectNode`](../../api/en/TheOne.Avalonia.Controls.TreeSelectNode.md) | 4 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
