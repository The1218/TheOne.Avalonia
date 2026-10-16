# MultiSelectBox

`MultiSelectBox` combines local search, selected-item tags, clearing, and a native virtualized results list. Selection contains model objects and survives filtering, including queries that hide every selected item. Load `OneTheme` to use it.

## Compiled bindings

```csharp
public ObservableCollection<Choice> Choices { get; } = [];
public ObservableCollection<Choice> Selected { get; } = [];
public MultiSelectItemAdapter Adapter { get; } =
    MultiSelectItemAdapter.Create<Choice>(item => item.Name, item => item.Available);
```

```xml
<one:MultiSelectBox ItemsSource="{Binding Choices}"
                    SelectedItems="{Binding Selected, Mode=TwoWay}"
                    ItemAdapter="{Binding Adapter}"
                    PlaceholderText="Choose applicable items"
                    AutomationProperties.Name="Applicable items" />
```

Declare `xmlns:one="using:TheOne.Avalonia.Controls"` and the view's concrete `x:DataType`, keeping compiled bindings enabled. The adapter supplies explicit typed getters for search, tag text, and availability, without reflection paths or additional metadata registration. Without an adapter, items use `ToString()` and are all enabled.

## Selection and disabled items

- `SelectedItems` accepts `IList`; `ObservableCollection<T>` works directly. The control edits the host collection instead of replacing its binding. Collection replacement and notifications also synchronize.
- Each instance creates its own mutable selection by default. An explicitly bound null, readonly, or fixed-size list is display-only. `IsReadOnly=True` also blocks selection, removal, and clearing while allowing search and browsing.
- Disabled options cannot be selected or removed by the user. Externally selected disabled values appear as locked tags. Clearing removes only editable values and retains locked selection.
- Filtering and empty results preserve selection. Ctrl / Cmd + A in results adds only enabled, loaded matches from the current query while retaining hidden selection. In search, the shortcut retains native text selection.
- Items use `Equals` / `GetHashCode`. Equal duplicates represent one option/selection, and null entries are ignored. Equality and hash semantics must remain stable while the items are in use.
- Removing an option from the source does not silently rewrite the external selection. Its tag remains removable when enabled. Removing a selected value removes all equal occurrences from the host list.
- This version filters loaded models synchronously using current-culture case-insensitive text matching and a trimmed query. It does not provide remote queries, server paging, or data virtualization.

## Public contract

| API | Behavior |
| --- | --- |
| `ItemsSource` | Optional model `IEnumerable`; do not populate it with pre-created controls |
| `SelectedItems` | Optional `IList`, with default two-way binding and synchronization in both directions |
| `ItemAdapter` | Explicit text and availability getters; text also drives search and default tags |
| `ItemTemplate` / `SelectedItemTemplate` | Templates for original result/tag models; declare their corresponding `x:DataType` |
| `SearchText` | Defaults to empty, supports two-way binding; closing retains the query |
| `PlaceholderText` / `SearchPlaceholderText` / `EmptyText` | Selection placeholder, search placeholder, and empty-results text; override for the host language |
| `ClearText` / `OpenText` / `RemoveTextFormat` | Accessible button names; removal format defaults to `移除 {0}`, with model text as its argument |
| `MaxVisibleTags` | Defaults to `3`, range `0–100`; remaining selection is represented by its count |
| `MaxDropDownHeight` | Defaults to `320`, finite and at least 80; popup bounds also follow the host |
| `IsReadOnly` / `IsDropDownOpen` | Selection readonly state and two-way popup state; disabling/detaching closes the popup |
| `SelectedCount` / `HasSelection` | Read-only distinct selection count and nonempty state |
| `HiddenSelectedCount` / `HasHiddenSelection` | Read-only count/state for selections without visible tags |
| `VisibleItems` / `DisplayTags` | Read-only template data; `MultiSelectBoxItem.Item` is the original model, with text, enabled/selected state, and template content |
| `HasNoResults` / `CanClear` / `ItemHeight` | Read-only empty-results state, editable-selection availability, and density minimum height |
| `ToggleItem(object)` | Toggles an editable value; adding requires source membership, removal also supports selected external values; returns `false` if unchanged |
| `ClearSelection()` | Batch-removes editable values, retaining disabled selections; returns `false` if unchanged |
| `RefreshItems()` | Refreshes plain collections, models without notifications, or getters dependent on other state |
| `OpenDropDown()` / `CloseDropDown()` | Opens and focuses search, or closes and returns focus to the field; opening requires attachment and enabled state |
| `SelectionChanged` | Bubbling event with original models in AddedItems / RemovedItems; batch user edits produce one event |

Use `INotifyCollectionChanged` and model `INotifyPropertyChanged` for automatic updates, raised on the UI thread. Detaching releases subscriptions; reattaching reads current data. Call `RefreshItems()` for changes without notifications. Custom templates change presentation only; search and availability still use the adapter.

## Keyboard and focus

| Location and key | Behavior |
| --- | --- |
| Focused field, Enter / Space / Down | Opens and focuses search |
| Focused field, F4 / Alt + Down | Toggles the popup |
| Search, Down / Up | Moves to an enabled result, skipping disabled options |
| Results, Up / Down / Home / End | Moves focus without changing selection |
| Search, Enter | Toggles the active result, or the first enabled result if none is active |
| Results, Enter / Space | Toggles the focused result while retaining other selections |
| Results, Ctrl / Cmd + A | Adds enabled results in the current query |
| Popup, Esc | Closes and restores field focus |
| Popup, Tab / Shift + Tab | Closes and follows native Tab navigation |
| Field or empty search, Backspace | Removes the last editable selection, skipping locked values |

Remove and clear buttons retain native keyboard/focus support. During composition, the native text input owns key handling. Light-dismiss does not steal outside focus. The field exposes the ComboBox automation role and expand/collapse provider; the native list retains row selection automation. Validate screen-reader behavior on target platforms separately.

## Themes and scale

Colors use `One.*` resources: a visible `One.InputLine` field border and `One.AccentSoft` selected tags with `One.Text` foreground. Small / Normal / Large single-line field and default result-row heights are 30 / 36 / 44; wrapped tags or custom content can increase actual height. Results have a 4px gap between items. Open popups synchronize theme and density, with width constrained to the host client area.

The bounded native `ListBox` and `VirtualizingStackPanel` create UI only for visible rows. Tags create at most `MaxVisibleTags` controls. Model storage, filtering, and subscriptions still cover loaded data; UI virtualization does not reduce business data size. Preserve the finite results viewport when customizing the template.

## Preview and focused checks

PreviewHost: **Extensions → MultiSelectBox**, with ten thousand models, preselected disabled values, and compiled bindings.

```sh
dotnet build design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/multiselect/artifacts
dotnet .preview/multiselect/artifacts/bin/PreviewHost/debug/PreviewHost.dll --verify-multiselect .preview/multiselect/screenshots
```

The check runs only MultiSelectBox: real key/pointer input, search, locked tags, clearing, collection replacement, readonly/disabled state, template and attachment lifetime, empty results, distant recycling of ten thousand options, bounded tags for bulk selection, and layout/pixels across light/dark × three densities × wide/narrow viewports.

`NativeAotSmoke` reuses the same check. Publish and execute the native program with `--require-aot --verify-multiselect` to verify getters, compiled templates, and bindings. Managed checks or cross-RID builds do not establish native execution on other platforms.
