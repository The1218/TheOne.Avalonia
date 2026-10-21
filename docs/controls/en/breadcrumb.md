# Breadcrumb

[简体中文](../zh-cn/breadcrumb.md) · [Control index](README.md)

`Breadcrumb` displays a caller-owned navigation path. Its last item represents the current page and cannot navigate or take keyboard focus. Ancestors retain native Button pointer, Enter / Space, command and CanExecute behavior. The control does not instantiate pages, update routes or truncate the source collection.

This guide describes current development source; use a subsequent package containing this control or reference source. Load `OneTheme` and declare `xmlns:one="using:TheOne.Avalonia.Controls"`.

<!-- visual-guide:start -->
## Appearance and example states

[Capture scope and visual index](visual-index.md). These are actual source-rendered examples; use this page’s usage notes for interaction and binding contracts.

### Breadcrumb

![Breadcrumb — Path, separators and current-page state; ancestors navigate while the current page does not navigate again.](../images/Breadcrumb-light.png)

Path, separators and current-page state; ancestors navigate while the current page does not navigate again.

[View Dark comparison](../images/Breadcrumb-dark.png)

<!-- visual-guide:end -->

## Basic usage

```xml
<one:Breadcrumb ItemInvoked="Navigate">
    <one:BreadcrumbItem Content="Home" Icon="Home" />
    <one:BreadcrumbItem Content="Projects" Icon="Folder" />
    <one:BreadcrumbItem Content="Details" />
</one:Breadcrumb>
```

```csharp
private void Navigate(object? sender, BreadcrumbItemInvokedEventArgs e)
{
    OpenPage(e.Item, e.Index); // Your application's navigation method
}
```

`ItemInvoked` bubbles and carries the original source item and its path index. With declarative items, Item is the BreadcrumbItem container. The first item has no separator. Current, disabled and CanExecute=false items cannot invoke navigation. An empty path has no placeholder. Read-only `CurrentItem` returns the last original item, or null for an empty path; it is not a writable selection property.

An activation captures its original item, index and command before raising ItemInvoked, then executes that captured command. Truncating/resetting the path, switching ItemsSource or replacing ItemInvokedCommand in the event cannot redirect this activation to a new item or command. Index is the activation-time index; Item is the original data reference rather than a copy of its fields. The captured command's CanExecute(Item) is checked again before execution.

When the event already performs navigation, set Handled to suppress the command and stop further bubbling:

```csharp
breadcrumb.ItemInvoked += (_, e) =>
{
    NavigateTo(e.Item); // Your navigation method may update the path here
    e.Handled = true;  // Suppresses ItemInvokedCommand for this activation
};
```

## Compiled binding and default model

Declare your view model through the consumer view's `x:DataType` and use explicit CompiledBinding:

```xml
<one:Breadcrumb ItemsSource="{CompiledBinding Path}"
                ItemInvokedCommand="{CompiledBinding NavigateCommand}" />
```

```csharp
public ObservableCollection<BreadcrumbItemModel> Path { get; } =
[
    new() { Header = "Home", Key = "home", Icon = OneIconKind.Home },
    new() { Header = "Projects", Key = "projects", Icon = OneIconKind.Folder },
    new() { Header = "Details", Key = "details" }
];
public ICommand NavigateCommand { get; } // Receives the original BreadcrumbItemModel
```

Read `((BreadcrumbItemModel)item).Key` in your command, perform application navigation and update Path. Key is caller-owned identity; the control does not reflect property names or match keys internally. The observable AvaloniaObject model supports binding and UI-thread updates:

| Model property | Default | Purpose |
| --- | --- | --- |
| `Header : object?` | null | Default content. |
| `Icon : OneIconKind?` | null | Optional built-in icon; adding, replacing and clearing update the view. Undefined values are rejected. |
| `Key : object?` | null | Application navigation identity. Events and commands receive the entire original item. |
| `IsEnabled : bool` | true | Access gate, combined with inherited enabled state and native CanExecute. |

Drive a navigation operation through either the command or event to avoid opening the same page twice. Clicking does not change CurrentItem; updating the final source item does.

## Additional properties and templates

| Breadcrumb property | Default | Usage |
| --- | --- | --- |
| `Separator : object?` | `"/"` | `Separator="›"` replaces it; `Separator="{x:Null}"` hides it. Use repeatable values; create visual instances through SeparatorTemplate. |
| `SeparatorTemplate : IDataTemplate?` | null | Separate noninteractive separator presentation. |
| `ItemInvokedCommand : ICommand?` | null | `ItemInvokedCommand="{CompiledBinding NavigateCommand}"`; receives the original item, not its Key or index. |
| `CurrentItem : object?` | null | Read-only final item: `breadcrumb.CurrentItem`. |

Items, ItemsSource and ItemTemplate are inherited native ItemsControl capabilities. Custom data does not need to inherit the built-in model or expose reflection-based property paths:

```xml
<one:Breadcrumb ItemsSource="{CompiledBinding Routes}">
    <one:Breadcrumb.ItemTemplate>
        <DataTemplate x:DataType="vm:RouteItem">
            <TextBlock Text="{CompiledBinding Title}" MaxWidth="200"
                       TextTrimming="CharacterEllipsis" />
        </DataTemplate>
    </one:Breadcrumb.ItemTemplate>
    <one:Breadcrumb.SeparatorTemplate>
        <DataTemplate>
            <one:OneIcon Kind="ChevronRight" IconSize="12" />
        </DataTemplate>
    </one:Breadcrumb.SeparatorTemplate>
</one:Breadcrumb>
```

RouteItem.Title belongs to your application. Templates receive original data, including the entire BreadcrumbItemModel when using that model with a custom template. The model's icon is still rendered by its container; leave Model.Icon null if the custom template supplies its own icon.

Declarative BreadcrumbItem supports Icon and an explicitly overridable IconSize, default 16 DIP. IsCurrent, ShowSeparator, CanNavigate and the container's Separator/SeparatorTemplate/NavigationCommand/NavigationParameter are owner-managed template state, exposed as read-only CLR properties rather than application navigation APIs. Replacement container templates retain the native Button `PART_Navigate`; the default also includes `PART_Separator` and `PART_Label`.

## Sizing, themes and keyboard

No dedicated Classes are added. See [theme documentation](../../theme/en/README.md) for shared sizing and color switching. Small / Normal / Large use minimum item heights 26 / 30 / 34 DIP and icons 14 / 16 / 18 DIP. Foreground, hover, press, focus border and disabled states reuse One.* resources. The current page uses text color and semibold weight instead of disabled gray.

The default WrapPanel wraps narrow paths and grows vertically without a fixed clipping height, popup or wheel interception. Constrain a single long label in ItemTemplate when needed. ItemsPanel remains replaceable through native API. Breadcrumb targets short paths; it does not virtualize or automatically collapse middle segments.

Tab follows native focus order. Left / Right and Home / End move focus between available ancestors, skipping current, disabled and CanExecute=false items; Enter / Space activates. Directional keys follow RTL layout and do not navigate until activated. Icons and separators do not receive pointer input or focus. Supply accessible names for icon-only content and avoid nesting interactive controls inside breadcrumb buttons.

## Collection changes and preview

Containers use native ItemsControl preparation and clearing; only built-in models receive additional property subscriptions. Remove, Replace, Move, Reset and ItemsSource replacement recalculate current/separator state and clear old model subscriptions, command parameters and state. A Control cannot be reused for multiple Headers or separators; use data templates to create separate visuals.

PreviewHost → 导航与布局 → **Breadcrumb** demonstrates ancestor navigation, path restoration, icons, text/icon/hidden separators, disabled items and compiled custom templates. Description components directly explain the added properties, event and model with short examples.

Focused validation covers Breadcrumb, its containers/model and direct preview bindings only. Runtime and platform coverage are reported with delivery.

2026-10-20: Release build passed. Managed and native AOT macOS ARM64 Skia headless runs each passed **205 focused checks**, including real pointer input, Enter / Space, directional keys, RTL, CanExecute, accessible names, source lifecycle and actual AXAML compiled bindings. Twelve previews covering Light/Dark × three densities × 360/900 DIP were byte-identical between managed/AOT runs. Native compilation used clang and the SDK from the same Xcode without new trim/AOT warnings. No full-library regression, Windows/Linux runtime or physical-display checks were performed.

When truncating a path turns its focused item into the current page, focus moves to an available ancestor, or clears if none is available; keyboard focus is not left on the non-navigable current item.

## Dynamic paths and lifecycle

Add, Remove, Move, Replace, Reset and ItemsSource switching validate containers against their original item and position rather than assigning new parameters from stale indices. While native container indices are being updated, mismatched old visuals temporarily cannot navigate; preparation/index updates restore the appropriate state. Repeated references to one model still mark only the final position as current. Value-type items match by value instead of boxed identity.

Removing/replacing a focused item or switching its source recovers focus after native layout. Recovery first searches available ancestors at/before the old position, then later ancestors; it clears focus if none is available. Valid focus remains unchanged. A subsequent user focus choice in a text box, another path item or another breadcrumb is never overwritten by queued recovery. Empty/single-item paths and paths with all ancestors disabled cannot retain focus on a non-navigable item. Old sources, models and commands no longer drive replacement containers.

Replacing the source during a pointer press cancels the old activation. Inserting or reordering items cannot redirect the originally pressed item to another item's command parameter.

2026-10-20 lifecycle review: six command/focus issues were reproduced before the fix. Afterwards all **534 focused Breadcrumb checks** passed on macOS ARM64 Skia headless, covering source/command replacement in the event, Handled cancellation, ancestor return, focused item removal/replacement, data changes during pointer press, repeated references, value types and 20 batches of mutations before layout. The normal PreviewHost Release build passed. This review changed no templates, themes or compiled bindings, so drawing matrices, native AOT and full-library regression were not repeated; the earlier 205-check AOT record belongs to initial implementation.

## Consecutive input and unload/reload

- Each distinct pointer click or complete Enter / Space press/release can execute once; no timed debounce is applied. Holding a navigation key does not repeatedly navigate. Directional keys and Home / End only move focus.
- Recursive activation of another item in this breadcrumb during the same synchronous event/command is ignored. The guard releases even after an exception. Express asynchronous business busy state through CanExecute; the control cannot infer completion of work after ICommand.Execute returns.
- Space release belongs to the original button and path version. Focus loss, path replacement/reordering, template replacement or unloading cancels the gesture. Native button pressed-state cleanup still runs without navigating to replacement data. Releasing the key while unloaded does not block a fresh gesture after reload.
- Unloading from the navigation event cancels its not-yet-executed command, including immediate reload within that event. Source-only replacement retains the snapshot behavior described above. Business work that already began is not rolled back.
- Unloading aborts queued focus recovery, removes its temporary focus observer and invalidates its version. Moving focus outside and subsequently clearing it cannot revive an old recovery request. New-lifetime recovery remains independent. Retired template buttons cannot invoke old commands, and repeated loading does not accumulate click handlers.

Keyboard observation follows attachment lifetime. Only an Enter / Space still held at unload keeps temporary weak-owner release observation; release or window deactivation clears it without strongly retaining the unloaded control.

2026-10-20 consecutive-input review: nine cases failed before the fix; afterwards **570 focused Breadcrumb checks** passed on macOS ARM64 Skia headless, including rapid alternating clicks, recursive activation, keyboard repeats, Space across data/focus/attachment changes, 12 reload cycles, retired template buttons and retry after command failure. The normal PreviewHost Release build passed. Only interaction/lifecycle code changed; templates, themes and compiled bindings were unchanged. No full-library regression, AOT rerun or Windows/Linux runtime checks were performed.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### Breadcrumb

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.Breadcrumb.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CurrentItem`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L43) | `object?` | read-only; OneWay | `computed / state` | null ; Read-only final item: `breadcrumb.CurrentItem`. |
| [`ItemInvokedCommand`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L42) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | null ; `ItemInvokedCommand="{CompiledBinding NavigateCommand}"`; receives the original item, not its Key or index. |
| [`Separator`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L39) | `object?` | writable; OneWay | `"/"` | `"/"` ; `Separator="›"` replaces it; `Separator="{x:Null}"` hides it. Use repeatable values; create visual instances through SeparatorTemplate. |
| [`SeparatorTemplate`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L40) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | null ; Separate noninteractive separator presentation. |

### BreadcrumbItem

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.BreadcrumbItem.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanNavigate`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L47) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Icon`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L39) | `TheOne.Avalonia.Controls.OneIconKind?` | writable; OneWay | `null` | null ; Optional built-in icon; adding, replacing and clearing update the view. Undefined values are rejected. Validation: `value => value is null &#124;&#124; Enum.IsDefined(value.Value)` Values: `Add`, `Remove`, `Edit`, `Delete`, `Search`, `Save`, `Refresh`, `More`, `Home`, `Menu`, `Back`, `Forward`, `Up`, `Down`, `Expand`, `Collapse`, `File`, `Folder`, `Document`, `Image`, `Upload`, `Download`, `Copy`, `Link`, `User`, `Users`, `Role`, `Lock`, `Unlock`, `Shield`, `Login`, `Logout`, `Info`, `Success`, `Warning`, `Error`, `Question`, `Bell`, `Message`, `Loading`, `Table`, `Database`, `LineChart`, `BarChart`, `PieChart`, `Dashboard`, `Filter`, `Sort`, `Calendar`, `Clock`, `DateRange`, `Timer`, `History`, `Schedule`, `PreviousDay`, `NextDay`, `Window`, `Tabs`, `Sidebar`, `Grid`, `List`, `Split`, `Maximize`, `Minimize`, `Settings`, `Monitor`, `Mobile`, `Printer`, `Keyboard`, `Mouse`, `Sun`, `Moon`, `Code`, `Terminal`, `Api`, `Server`, `Cloud`, `Network`, `Bug`, `Package`, `Start`, `End`, `Approval`, `Task`, `Branch`, `Parallel`, `Merge`, `Workflow`, `Pointer`, `Text`, `Rectangle`, `Line`, `AlignLeft`, `AlignCenter`, `Barcode`, `QrCode`, `Close`, `Undo`, `Redo`, `Share`, `ChevronLeft`, `ChevronRight`, `ChevronUp`, `ChevronDown`, `FileAdd`, `FolderOpen`, `Attachment`, `FileSearch`, `UserAdd`, `UserRemove`, `Key`, `Permission`, `BellOff`, `Mail`, `Send`, `Flag`, `SortAscending`, `SortDescending`, `Statistics`, `TrendUp`, `Alarm`, `CalendarCheck`, `CalendarAdd`, `CalendarRemove`, `Fullscreen`, `FullscreenExit`, `Pin`, `Unpin`, `Play`, `Pause`, `Stop`, `Volume`, `Wifi`, `WifiOff`, `Connection`, `Disconnect`, `Approve`, `Reject`, `Transfer`, `Pending`, `ZoomIn`, `ZoomOut`, `AlignRight`, `Crop`, `Cut`, `Paste`, `PrintPreview`, `Export`, `Compass`, `Location`, `Route`, `BreadcrumbPath`, `FileCheck`, `FileLock`, `Archive`, `TrashRestore`, `UserCheck`, `UserSettings`, `Organization`, `Fingerprint`, `NotificationRead`, `ChatReply`, `Heart`, `Star`, `ScatterPlot`, `AreaChart`, `Heatmap`, `DataImport`, `ClockCheck`, `ClockForward`, `CalendarWeek`, `TimeZone`, `DockBottom`, `PanelLeftClose`, `PanelLeftOpen`, `Layers`, `Camera`, `Microphone`, `Headphones`, `Battery`, `GitBranch`, `GitCommit`, `Braces`, `Container`, `Reassign`, `ApprovalChain`, `WorkflowValidate`, `WorkflowPublish`, `AlignTop`, `AlignBottom`, `DistributeHorizontal`, `Palette`, `Wallet`, `CreditCard`, `Bank`, `Cash`, `Coins`, `Receipt`, `Invoice`, `Calculator`, `Payment`, `Refund`, `Exchange`, `CurrencyYuan`, `CurrencyDollar`, `Safe`, `FinancialReport`, `StockTrend`, `Factory`, `Gear`, `Wrench`, `Hammer`, `RobotArm`, `Conveyor`, `Valve`, `Pump`, `Motor`, `Sensor`, `Gauge`, `Crane`, `Forklift`, `HardHat`, `Welding`, `Toolbox`, `MedicalCross`, `Hospital`, `Stethoscope`, `HeartPulse`, `Pill`, `Syringe`, `Thermometer`, `FirstAid`, `Ambulance`, `Wheelchair`, `Microscope`, `TestTube`, `Dna`, `Lungs`, `Tooth`, `Bandage`, `Building`, `Bridge`, `Road`, `Railway`, `Airport`, `PowerTower`, `PowerPlug`, `WaterDrop`, `WaterPipe`, `WindTurbine`, `SolarPanel`, `StreetLight`, `TrafficLight`, `FireHydrant`, `Warehouse`, `Recycle`, `Truck`, `DeliveryVan`, `Car`, `Bus`, `Bicycle`, `Train`, `Ship`, `FlightTakeoff`, `DeliveryBox`, `DeliveryTracking`, `Map`, `NavigationArrow`, `Parking`, `Fuel`, `Anchor`, `TrafficCone`, `ShoppingCart`, `ShoppingBag`, `Store`, `TagLabel`, `Discount`, `Gift`, `Product`, `Scan`, `ShoppingBasket`, `Order`, `OrderCheck`, `OrderCancel`, `Coupon`, `LoyaltyCard`, `CustomerService`, `ShoppingCompare`, `Book`, `BookOpen`, `GraduationCap`, `School`, `Pen`, `Notebook`, `Library`, `Certificate`, `Lightbulb`, `Flask`, `Ruler`, `Globe`, `Language`, `Presentation`, `ReadAloud`, `Backpack`, `CloudSun`, `CloudMoon`, `Rain`, `Snow`, `Thunderstorm`, `Wind`, `Fog`, `Tornado`, `Rainbow`, `Umbrella`, `TemperatureHigh`, `Leaf`, `Tree`, `Flower`, `Mountain`, `Earth` |
| [`IconSize`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L40) | `double` | writable; OneWay | `16` | Icon drawing size in DIPs, independent of font size and button hit area. Validation: `value => double.IsFinite(value) && value > 0` |
| [`IsCurrent`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L45) | `bool` | read-only; OneWay | `computed / state` | Whether this is the current pagination page, derived from paging state. |
| [`NavigationCommand`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L43) | `System.Windows.Input.ICommand?` | read-only; OneWay | `null` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`NavigationParameter`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L44) | `object?` | read-only; OneWay | `null` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Separator`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L41) | `object?` | read-only; OneWay | `"/"` | `"/"` ; `Separator="›"` replaces it; `Separator="{x:Null}"` hides it. Use repeatable values; create visual instances through SeparatorTemplate. |
| [`SeparatorTemplate`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L42) | `Avalonia.Controls.Templates.IDataTemplate?` | read-only; OneWay | `null` | null ; Separate noninteractive separator presentation. |
| [`ShowSeparator`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L46) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### BreadcrumbItemInvokedEventArgs

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Index`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L252) | `int` | read-only; CLR / n/a | `index` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Item`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L251) | `object?` | read-only; CLR / n/a | `item` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### BreadcrumbItemModel

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.BreadcrumbItemModel.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Header`](../../../TheOne.Avalonia/Controls/BreadcrumbItemModel.cs#L14) | `object?` | writable; OneWay | `null` | null ; Default content. |
| [`Icon`](../../../TheOne.Avalonia/Controls/BreadcrumbItemModel.cs#L15) | `TheOne.Avalonia.Controls.OneIconKind?` | writable; OneWay | `null` | null ; Optional built-in icon; adding, replacing and clearing update the view. Undefined values are rejected. Validation: `value => value is null &#124;&#124; Enum.IsDefined(value.Value)` Values: `Add`, `Remove`, `Edit`, `Delete`, `Search`, `Save`, `Refresh`, `More`, `Home`, `Menu`, `Back`, `Forward`, `Up`, `Down`, `Expand`, `Collapse`, `File`, `Folder`, `Document`, `Image`, `Upload`, `Download`, `Copy`, `Link`, `User`, `Users`, `Role`, `Lock`, `Unlock`, `Shield`, `Login`, `Logout`, `Info`, `Success`, `Warning`, `Error`, `Question`, `Bell`, `Message`, `Loading`, `Table`, `Database`, `LineChart`, `BarChart`, `PieChart`, `Dashboard`, `Filter`, `Sort`, `Calendar`, `Clock`, `DateRange`, `Timer`, `History`, `Schedule`, `PreviousDay`, `NextDay`, `Window`, `Tabs`, `Sidebar`, `Grid`, `List`, `Split`, `Maximize`, `Minimize`, `Settings`, `Monitor`, `Mobile`, `Printer`, `Keyboard`, `Mouse`, `Sun`, `Moon`, `Code`, `Terminal`, `Api`, `Server`, `Cloud`, `Network`, `Bug`, `Package`, `Start`, `End`, `Approval`, `Task`, `Branch`, `Parallel`, `Merge`, `Workflow`, `Pointer`, `Text`, `Rectangle`, `Line`, `AlignLeft`, `AlignCenter`, `Barcode`, `QrCode`, `Close`, `Undo`, `Redo`, `Share`, `ChevronLeft`, `ChevronRight`, `ChevronUp`, `ChevronDown`, `FileAdd`, `FolderOpen`, `Attachment`, `FileSearch`, `UserAdd`, `UserRemove`, `Key`, `Permission`, `BellOff`, `Mail`, `Send`, `Flag`, `SortAscending`, `SortDescending`, `Statistics`, `TrendUp`, `Alarm`, `CalendarCheck`, `CalendarAdd`, `CalendarRemove`, `Fullscreen`, `FullscreenExit`, `Pin`, `Unpin`, `Play`, `Pause`, `Stop`, `Volume`, `Wifi`, `WifiOff`, `Connection`, `Disconnect`, `Approve`, `Reject`, `Transfer`, `Pending`, `ZoomIn`, `ZoomOut`, `AlignRight`, `Crop`, `Cut`, `Paste`, `PrintPreview`, `Export`, `Compass`, `Location`, `Route`, `BreadcrumbPath`, `FileCheck`, `FileLock`, `Archive`, `TrashRestore`, `UserCheck`, `UserSettings`, `Organization`, `Fingerprint`, `NotificationRead`, `ChatReply`, `Heart`, `Star`, `ScatterPlot`, `AreaChart`, `Heatmap`, `DataImport`, `ClockCheck`, `ClockForward`, `CalendarWeek`, `TimeZone`, `DockBottom`, `PanelLeftClose`, `PanelLeftOpen`, `Layers`, `Camera`, `Microphone`, `Headphones`, `Battery`, `GitBranch`, `GitCommit`, `Braces`, `Container`, `Reassign`, `ApprovalChain`, `WorkflowValidate`, `WorkflowPublish`, `AlignTop`, `AlignBottom`, `DistributeHorizontal`, `Palette`, `Wallet`, `CreditCard`, `Bank`, `Cash`, `Coins`, `Receipt`, `Invoice`, `Calculator`, `Payment`, `Refund`, `Exchange`, `CurrencyYuan`, `CurrencyDollar`, `Safe`, `FinancialReport`, `StockTrend`, `Factory`, `Gear`, `Wrench`, `Hammer`, `RobotArm`, `Conveyor`, `Valve`, `Pump`, `Motor`, `Sensor`, `Gauge`, `Crane`, `Forklift`, `HardHat`, `Welding`, `Toolbox`, `MedicalCross`, `Hospital`, `Stethoscope`, `HeartPulse`, `Pill`, `Syringe`, `Thermometer`, `FirstAid`, `Ambulance`, `Wheelchair`, `Microscope`, `TestTube`, `Dna`, `Lungs`, `Tooth`, `Bandage`, `Building`, `Bridge`, `Road`, `Railway`, `Airport`, `PowerTower`, `PowerPlug`, `WaterDrop`, `WaterPipe`, `WindTurbine`, `SolarPanel`, `StreetLight`, `TrafficLight`, `FireHydrant`, `Warehouse`, `Recycle`, `Truck`, `DeliveryVan`, `Car`, `Bus`, `Bicycle`, `Train`, `Ship`, `FlightTakeoff`, `DeliveryBox`, `DeliveryTracking`, `Map`, `NavigationArrow`, `Parking`, `Fuel`, `Anchor`, `TrafficCone`, `ShoppingCart`, `ShoppingBag`, `Store`, `TagLabel`, `Discount`, `Gift`, `Product`, `Scan`, `ShoppingBasket`, `Order`, `OrderCheck`, `OrderCancel`, `Coupon`, `LoyaltyCard`, `CustomerService`, `ShoppingCompare`, `Book`, `BookOpen`, `GraduationCap`, `School`, `Pen`, `Notebook`, `Library`, `Certificate`, `Lightbulb`, `Flask`, `Ruler`, `Globe`, `Language`, `Presentation`, `ReadAloud`, `Backpack`, `CloudSun`, `CloudMoon`, `Rain`, `Snow`, `Thunderstorm`, `Wind`, `Fog`, `Tornado`, `Rainbow`, `Umbrella`, `TemperatureHigh`, `Leaf`, `Tree`, `Flower`, `Mountain`, `Earth` |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/BreadcrumbItemModel.cs#L17) | `bool` | writable; OneWay | `true` | true ; Access gate, combined with inherited enabled state and native CanExecute. |
| [`Key`](../../../TheOne.Avalonia/Controls/BreadcrumbItemModel.cs#L16) | `object?` | writable; OneWay | `null` | null ; Application navigation identity. Events and commands receive the entire original item. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

No additional business methods; configure the control-specific properties above.

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`Breadcrumb`](../../api/en/TheOne.Avalonia.Controls.Breadcrumb.md) | `ItemInvoked` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs`](../../api/en/TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs.md) | 2 |
| [`TheOne.Avalonia.Controls.BreadcrumbItemModel`](../../api/en/TheOne.Avalonia.Controls.BreadcrumbItemModel.md) | 4 |
| [`TheOne.Avalonia.Controls.OneIconKind`](../../api/en/TheOne.Avalonia.Controls.OneIconKind.md) | 320 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
