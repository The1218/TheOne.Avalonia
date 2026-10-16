# MultiSelectBox

The popup owns a bounded viewport: overflow displays a native vertical scrollbar, while a short list correctly has none. Wheel input over results, search, empty results and either scroll boundary stays inside the popup without moving an outer page such as Home. Closing restores ordinary page scrolling.

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

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost's Control Laboratory (top-right action on each page) groups live examples by styles, properties, APIs/events and models; complex models use the complete business samples.

### MultiSelectBox

[API / model reference](../../api/en/TheOne.Avalonia.Controls.MultiSelectBox.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanClear`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L76) | `bool` | read-only; OneWay | `computed / state` | Read-only empty-results state, editable-selection availability, and density minimum height |
| [`ClearText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L62) | `string` | writable; OneWay | `"清空可编辑选择"` | Accessible button names; removal format defaults to `移除 {0}`, with model text as its argument |
| [`DisplayTags`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L70) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.MultiSelectBoxItem>` | read-only; OneWay | `computed / state` | Read-only template data; `MultiSelectBoxItem.Item` is the original model, with text, enabled/selected state, and template content |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L61) | `string` | writable; OneWay | `"无匹配选项"` | Selection placeholder, search placeholder, and empty-results text; override for the host language |
| [`HasHiddenSelection`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L74) | `bool` | read-only; OneWay | `computed / state` | Read-only count/state for selections without visible tags |
| [`HasNoResults`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L75) | `bool` | read-only; OneWay | `computed / state` | Read-only empty-results state, editable-selection availability, and density minimum height |
| [`HasSelection`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L73) | `bool` | read-only; OneWay | `computed / state` | Read-only distinct selection count and nonempty state |
| [`HiddenSelectedCount`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L72) | `int` | read-only; OneWay | `computed / state` | Read-only count/state for selections without visible tags |
| [`IsDropDownOpen`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L66) | `bool` | writable; TwoWay | `false` | Selection readonly state and two-way popup state; disabling/detaching closes the popup |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L65) | `bool` | writable; OneWay | `false` | Selection readonly state and two-way popup state; disabling/detaching closes the popup |
| [`ItemAdapter`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L55) | `TheOne.Avalonia.Controls.MultiSelectItemAdapter?` | writable; OneWay | `null` | Explicit text and availability getters; text also drives search and default tags |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L77) | `double` | read-only; OneWay | `computed / state` | Read-only empty-results state, editable-selection availability, and density minimum height |
| [`ItemTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L56) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Templates for original result/tag models; declare their corresponding `x:DataType` |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L53) | `System.Collections.IEnumerable?` | writable; OneWay | `null` | Optional model `IEnumerable`; do not populate it with pre-created controls |
| [`MaxDropDownHeight`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L68) | `double` | writable; OneWay | `320` | Defaults to `320`, finite and at least 80; popup bounds also follow the host Validation: `value => double.IsFinite(value) && value >= 80` |
| [`MaxVisibleTags`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L67) | `int` | writable; OneWay | `3` | Defaults to `3`, range `0–100`; remaining selection is represented by its count Validation: `value => value is >= 0 and <= 100` |
| [`OpenText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L63) | `string` | writable; OneWay | `"展开或收起选项"` | Accessible button names; removal format defaults to `移除 {0}`, with model text as its argument |
| [`PlaceholderText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L59) | `string` | writable; OneWay | `"请选择"` | Selection placeholder, search placeholder, and empty-results text; override for the host language |
| [`RemoveTextFormat`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L64) | `string` | writable; OneWay | `"移除 {0}"` | Accessible button names; removal format defaults to `移除 {0}`, with model text as its argument |
| [`SearchPlaceholderText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L60) | `string` | writable; OneWay | `"搜索选项"` | Selection placeholder, search placeholder, and empty-results text; override for the host language |
| [`SearchText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L58) | `string` | writable; TwoWay | `""` | Defaults to empty, supports two-way binding; closing retains the query |
| [`SelectedCount`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L71) | `int` | read-only; OneWay | `computed / state` | Read-only distinct selection count and nonempty state |
| [`SelectedItemTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L57) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Templates for original result/tag models; declare their corresponding `x:DataType` |
| [`SelectedItems`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L54) | `System.Collections.IList?` | writable; TwoWay | `null` | Optional `IList`, with default two-way binding and synchronization in both directions |
| [`VisibleItems`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L69) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.MultiSelectBoxItem>` | read-only; OneWay | `computed / state` | Read-only template data; `MultiSelectBoxItem.Item` is the original model, with text, enabled/selected state, and template content |

### MultiSelectBoxItem

[API / model reference](../../api/en/TheOne.Avalonia.Controls.MultiSelectBoxItem.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanRemove`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L33) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L36) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; CLR / n/a | `accessor-defined / state` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`DisplayContent`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L35) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L31) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Business availability; selection, removal and commands follow the control's disabled-item rules. |
| [`IsSelected`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L32) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Current-item selection state, synchronized with the selected model and not shared with unrelated items. |
| [`Item`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L29) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`RemoveLabel`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L34) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`TagContent`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L37) | `object` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`TagTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L38) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; CLR / n/a | `accessor-defined / state` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`Text`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L30) | `string` | read-only; CLR / n/a | `""` | Input/display text; notifications and validation follow the model/native editor contract. |

### MultiSelectItemAdapter

[API / model reference](../../api/en/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

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
| MultiSelectBox | `one-large` | automatic template class; do not bind manually | ^.one-large => FontSize=16 |
| MultiSelectBox | `one-normal` | automatic template class; do not bind manually | ^.one-normal => FontSize=14 |
| MultiSelectBox | `one-small` | automatic template class; do not bind manually | ^.one-small => FontSize=12 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.MultiSelectBox`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBox.md) | Class | 25 | 6 | 1 |
| [`TheOne.Avalonia.Controls.MultiSelectBoxItem`](../../api/en/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) | Class | 10 | 0 | 1 |
| [`TheOne.Avalonia.Controls.MultiSelectItemAdapter`](../../api/en/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) | Class | 0 | 3 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
