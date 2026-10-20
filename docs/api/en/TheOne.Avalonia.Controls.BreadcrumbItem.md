# BreadcrumbItem

[English](../en/TheOne.Avalonia.Controls.BreadcrumbItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.BreadcrumbItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.BreadcrumbItem` · Class · `Avalonia.Controls.ContentControl`

A breadcrumb container with a separate, noninteractive separator and native navigation button.

[Source / 声明](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L16)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

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

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`BreadcrumbItem.BreadcrumbItem()`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L16) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<OneIconKind?> BreadcrumbItem.IconProperty`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> BreadcrumbItem.IconSizeProperty`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> BreadcrumbItem.SeparatorProperty`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> BreadcrumbItem.SeparatorTemplateProperty`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ICommand?> BreadcrumbItem.NavigationCommandProperty`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> BreadcrumbItem.NavigationParameterProperty`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<BreadcrumbItem, bool> BreadcrumbItem.IsCurrentProperty`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<BreadcrumbItem, bool> BreadcrumbItem.ShowSeparatorProperty`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<BreadcrumbItem, bool> BreadcrumbItem.CanNavigateProperty`](../../../TheOne.Avalonia/Controls/BreadcrumbItem.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.OneIconKind`](TheOne.Avalonia.Controls.OneIconKind.md)
