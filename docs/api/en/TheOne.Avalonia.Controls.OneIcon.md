# OneIcon

[English](../en/TheOne.Avalonia.Controls.OneIcon.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneIcon.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneIcon` · Class · `Avalonia.Controls.Control`

A themed vector glyph with a stable enum identity and no external font or icon dependency.

[Source / 声明](../../../TheOne.Avalonia/Controls/OneIcon.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Foreground`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L45) | `Avalonia.Media.IBrush?` | writable; OneWay | `inherited registration` | `IBrush?` / inherited text brush ; Inherits the host by default; supports bindings and dynamic resources. Null draws nothing. Override with `Foreground="{DynamicResource One.Accent}"`. |
| [`IconSize`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L41) | `double` | writable; OneWay | `20` | `double` / `20` ; Positive finite desired size in DIPs; theme density applies unless explicitly overridden. `IconSize="18"`. Validation: `value => double.IsFinite(value) && value > 0` |
| [`Kind`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L37) | `TheOne.Avalonia.Controls.OneIconKind` | writable; OneWay | `OneIconKind.Add` | `OneIconKind` / `Add` ; One of 320 defined values; `Kind="Save"`. Undefined enum values are rejected. Validation: `value => Enum.IsDefined(value)` Values: `Add`, `Remove`, `Edit`, `Delete`, `Search`, `Save`, `Refresh`, `More`, `Home`, `Menu`, `Back`, `Forward`, `Up`, `Down`, `Expand`, `Collapse`, `File`, `Folder`, `Document`, `Image`, `Upload`, `Download`, `Copy`, `Link`, `User`, `Users`, `Role`, `Lock`, `Unlock`, `Shield`, `Login`, `Logout`, `Info`, `Success`, `Warning`, `Error`, `Question`, `Bell`, `Message`, `Loading`, `Table`, `Database`, `LineChart`, `BarChart`, `PieChart`, `Dashboard`, `Filter`, `Sort`, `Calendar`, `Clock`, `DateRange`, `Timer`, `History`, `Schedule`, `PreviousDay`, `NextDay`, `Window`, `Tabs`, `Sidebar`, `Grid`, `List`, `Split`, `Maximize`, `Minimize`, `Settings`, `Monitor`, `Mobile`, `Printer`, `Keyboard`, `Mouse`, `Sun`, `Moon`, `Code`, `Terminal`, `Api`, `Server`, `Cloud`, `Network`, `Bug`, `Package`, `Start`, `End`, `Approval`, `Task`, `Branch`, `Parallel`, `Merge`, `Workflow`, `Pointer`, `Text`, `Rectangle`, `Line`, `AlignLeft`, `AlignCenter`, `Barcode`, `QrCode`, `Close`, `Undo`, `Redo`, `Share`, `ChevronLeft`, `ChevronRight`, `ChevronUp`, `ChevronDown`, `FileAdd`, `FolderOpen`, `Attachment`, `FileSearch`, `UserAdd`, `UserRemove`, `Key`, `Permission`, `BellOff`, `Mail`, `Send`, `Flag`, `SortAscending`, `SortDescending`, `Statistics`, `TrendUp`, `Alarm`, `CalendarCheck`, `CalendarAdd`, `CalendarRemove`, `Fullscreen`, `FullscreenExit`, `Pin`, `Unpin`, `Play`, `Pause`, `Stop`, `Volume`, `Wifi`, `WifiOff`, `Connection`, `Disconnect`, `Approve`, `Reject`, `Transfer`, `Pending`, `ZoomIn`, `ZoomOut`, `AlignRight`, `Crop`, `Cut`, `Paste`, `PrintPreview`, `Export`, `Compass`, `Location`, `Route`, `BreadcrumbPath`, `FileCheck`, `FileLock`, `Archive`, `TrashRestore`, `UserCheck`, `UserSettings`, `Organization`, `Fingerprint`, `NotificationRead`, `ChatReply`, `Heart`, `Star`, `ScatterPlot`, `AreaChart`, `Heatmap`, `DataImport`, `ClockCheck`, `ClockForward`, `CalendarWeek`, `TimeZone`, `DockBottom`, `PanelLeftClose`, `PanelLeftOpen`, `Layers`, `Camera`, `Microphone`, `Headphones`, `Battery`, `GitBranch`, `GitCommit`, `Braces`, `Container`, `Reassign`, `ApprovalChain`, `WorkflowValidate`, `WorkflowPublish`, `AlignTop`, `AlignBottom`, `DistributeHorizontal`, `Palette`, `Wallet`, `CreditCard`, `Bank`, `Cash`, `Coins`, `Receipt`, `Invoice`, `Calculator`, `Payment`, `Refund`, `Exchange`, `CurrencyYuan`, `CurrencyDollar`, `Safe`, `FinancialReport`, `StockTrend`, `Factory`, `Gear`, `Wrench`, `Hammer`, `RobotArm`, `Conveyor`, `Valve`, `Pump`, `Motor`, `Sensor`, `Gauge`, `Crane`, `Forklift`, `HardHat`, `Welding`, `Toolbox`, `MedicalCross`, `Hospital`, `Stethoscope`, `HeartPulse`, `Pill`, `Syringe`, `Thermometer`, `FirstAid`, `Ambulance`, `Wheelchair`, `Microscope`, `TestTube`, `Dna`, `Lungs`, `Tooth`, `Bandage`, `Building`, `Bridge`, `Road`, `Railway`, `Airport`, `PowerTower`, `PowerPlug`, `WaterDrop`, `WaterPipe`, `WindTurbine`, `SolarPanel`, `StreetLight`, `TrafficLight`, `FireHydrant`, `Warehouse`, `Recycle`, `Truck`, `DeliveryVan`, `Car`, `Bus`, `Bicycle`, `Train`, `Ship`, `FlightTakeoff`, `DeliveryBox`, `DeliveryTracking`, `Map`, `NavigationArrow`, `Parking`, `Fuel`, `Anchor`, `TrafficCone`, `ShoppingCart`, `ShoppingBag`, `Store`, `TagLabel`, `Discount`, `Gift`, `Product`, `Scan`, `ShoppingBasket`, `Order`, `OrderCheck`, `OrderCancel`, `Coupon`, `LoyaltyCard`, `CustomerService`, `ShoppingCompare`, `Book`, `BookOpen`, `GraduationCap`, `School`, `Pen`, `Notebook`, `Library`, `Certificate`, `Lightbulb`, `Flask`, `Ruler`, `Globe`, `Language`, `Presentation`, `ReadAloud`, `Backpack`, `CloudSun`, `CloudMoon`, `Rain`, `Snow`, `Thunderstorm`, `Wind`, `Fog`, `Tornado`, `Rainbow`, `Umbrella`, `TemperatureHigh`, `Leaf`, `Tree`, `Flower`, `Mountain`, `Earth` |
| [`StrokeThickness`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L43) | `double` | writable; OneWay | `1.75` | `double` / `1.75` ; Outline width in the 24-unit grid, range `(0, 3]`, scaled with the glyph; does not affect filled silhouettes. `StrokeThickness="1.5"`. Validation: `value => double.IsFinite(value) && value is > 0 and <= 3` |
| [`Variant`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L39) | `TheOne.Avalonia.Controls.OneIconVariant` | writable; OneWay | `OneIconVariant.Outline` | `OneIconVariant` / `Outline` ; `Outline` or `Filled`; unsupported filled variants use the same glyph's outline. `Variant="Filled"`. Validation: `value => Enum.IsDefined(value)` Values: `Outline`, `Filled` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void OneIcon.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L49) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`OneIcon.OneIcon()`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L10) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<OneIconKind> OneIcon.KindProperty`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<OneIconVariant> OneIcon.VariantProperty`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> OneIcon.IconSizeProperty`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> OneIcon.StrokeThicknessProperty`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> OneIcon.ForegroundProperty`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.OneIconKind`](TheOne.Avalonia.Controls.OneIconKind.md)
- [`TheOne.Avalonia.Controls.OneIconVariant`](TheOne.Avalonia.Controls.OneIconVariant.md)
