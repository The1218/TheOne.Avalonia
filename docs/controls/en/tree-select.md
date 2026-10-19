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
