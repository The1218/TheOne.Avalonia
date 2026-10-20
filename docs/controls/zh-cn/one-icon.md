# OneIcon 自有图标

[English](../en/one-icon.md) · [控件目录](README.md)

`OneIcon` 是原生直接绘制的矢量图标。内置 20 类、320 个基础图标，不依赖图标字体、网络、Material 或其他外部图标包。几何源码在 `OneIconCatalog.cs` 中维护，轮廓按首次使用解析并共享缓存，专用实心几何仅在首次使用 Filled 时解析，不按实例或每帧重复解析。

本文对应当前开发源码；OneIcon 不包含在此前发布的 `1.0.0-preview.7` 包中。可先引用源码，或使用后续明确包含此功能的版本。

本轮新增交通物流、商业购物、教育学习、天气环境四类，各 16 个，共增加 64 个。新增编号 256–319，原有 0–255 的编号、图形和分类保持不变，分类枚举也只在尾部追加。共有 32 个专用实心变体，其余 Filled 按原规则回退到线性。

```xml
<one:OneIcon Kind="Wallet" />
<one:OneIcon Kind="Factory" />
<one:OneIcon Kind="Stethoscope" />
<one:OneIcon Kind="PowerTower" />
```

以上示例沿用下方的 `one` 命名空间；新增项与原有图标使用相同的枚举绑定、前景色继承和尺寸配置。

## 使用与绑定

加载 `OneTheme` 后直接使用：

```xml
<!-- xmlns:one="using:TheOne.Avalonia.Controls" -->
<one:OneIcon Kind="Search" />

<!-- 放在按钮中时继承按钮内容前景色，不必单独指定颜色。 -->
<Button Classes="primary" AutomationProperties.Name="搜索">
    <one:OneIcon Kind="Search" />
</Button>
```

普通绑定统一使用 `CompiledBinding`。在消费端视图设置正确的 `x:DataType`，数据上下文属性直接返回枚举，不传图标名称字符串：

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:one="using:TheOne.Avalonia.Controls"
             xmlns:vm="using:MyApp.ViewModels"
             x:Class="MyApp.Views.IconView" x:DataType="vm:IconViewModel">
    <one:OneIcon Kind="{CompiledBinding CurrentIcon}"
                 Variant="{CompiledBinding IconVariant}" />
</UserControl>
```

```csharp
using System.ComponentModel;
using TheOne.Avalonia.Controls;

namespace MyApp.ViewModels;

public sealed class IconViewModel : INotifyPropertyChanged
{
    private OneIconKind _currentIcon = OneIconKind.Search;
    public OneIconKind CurrentIcon
    {
        get => _currentIcon;
        set
        {
            if (_currentIcon == value) return;
            _currentIcon = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentIcon)));
        }
    }
    public OneIconVariant IconVariant => OneIconVariant.Outline;
    public event PropertyChangedEventHandler? PropertyChanged;
}
```

消费端仍负责设置视图 DataContext。Kind/Variant 变化不会创建新的控件，不使用反射属性名解析或程序集保留规则。

## 专属属性

| 属性 | 类型 / 默认值 | 说明与短例 |
| --- | --- | --- |
| `Kind` | `OneIconKind` / `Add` | 320 个有效枚举；`Kind="Save"`。未定义枚举值被拒绝。 |
| `Variant` | `OneIconVariant` / `Outline` | `Outline` 或 `Filled`；无实心资源时回退同一图标的 Outline。`Variant="Filled"`。 |
| `IconSize` | `double` / `20` | 正有限 DIP；默认跟随三档密度；显式值优先。`IconSize="18"`。 |
| `StrokeThickness` | `double` / `1.75` | 24 单位设计网格的轮廓线宽，范围 `(0, 3]`，随图标等比缩放；不影响实心轮廓。`StrokeThickness="1.5"`。 |
| `Foreground` | `IBrush?` / 继承宿主文字色 | 支持主题资源与绑定；`null` 不绘制。通常继承，显式覆盖可用 `Foreground="{DynamicResource One.Accent}"`。 |

没有额外外观 Classes。公共 `small / normal / large` 和可继承的 `ControlSizing.Size` 对应 **16 / 20 / 24 DIP**，正常沿用 [公共尺寸约定](../../theme/zh-cn/README.md)。改变 Classes、祖先尺寸或主题后自动更新。默认水平/垂直居中，避免在星号 Grid 中被撑成大图标。显式 `IconSize` 优先于主题尺寸；显式 Width/Height 约束布局后，图形在实际边界内等比居中，不拉伸变形。

## 主题、状态与宿主

- Foreground 复用原生可继承文字前景色：Window、Button 内容、菜单等可正常传递颜色；不会以默认图标色遮蔽宿主的选中/按下文字色。
- `OneTheme` 的禁用状态使用 `One.DisabledText`；显式局部 Foreground 仍按 Avalonia 优先级生效。宿主本身的透明度也可能作用于子图标。
- 不把背景或强调色固化进图标，Light/Dark 与颜色资源切换由宿主及动态资源驱动。
- 默认不参与命中、不获取焦点，不截断宿主的指针和键盘命令；可通过原生属性显式改变，但图标不是按钮。
- 图标默认是装饰。纯图标按钮的可访问名称设在按钮上；内置中文 DisplayName 不自动充当屏幕阅读名称。
- `Loading` 是静态辐射线图标，不启动计时器；需要旋转时由宿主动画控制并尊重 Motion 设置。
- 方向图标表达固定方向，不自动镜像 RTL；由宿主选择相应枚举。

与 IconButton 组合：

```xml
<one:IconButton Content="保存" IconSize="20">
    <one:IconButton.Icon><one:OneIcon Kind="Save" /></one:IconButton.Icon>
</one:IconButton>
```

`OneIcon` 是 Control，不能作为一个实例被多个宿主共享。可共享枚举和元数据，通过模板创建各自图标实例。与 [IconButton](icon-button.md) 已有 Geometry、自定义模板和外部图标接入兼容。

## 变体与完整目录

32 个图标提供专门的实心几何：`Search`、`Home`、`File`、`Folder`、`User`、`Lock`、`Shield`、`Info`、`Success`、`Warning`、`Error`、`Question`、`Bell`、`Message`、`Start`、`End`、`Heart`、`Star`、`Camera`、`Battery`、`Wallet`、`Bank`、`Factory`、`HardHat`、`MedicalCross`、`FirstAid`、`Building`、`WaterDrop`、`Car`、`ShoppingBag`、`Book`、`Leaf`。其余 `Filled` 明确回退为 Outline，不把开放路径简单填黑。动态切换 Kind/Variant 不保留旧填充。

| `OneIconCategory` / 分类 | `OneIconKind`（每类 16 个） |
| --- | --- |
| Actions / 常用操作 | Add, Remove, Edit, Delete, Search, Save, Refresh, More, Close, Undo, Redo, Share, Cut, Paste, PrintPreview, Export |
| Navigation / 方向导航 | Home, Menu, Back, Forward, Up, Down, Expand, Collapse, ChevronLeft, ChevronRight, ChevronUp, ChevronDown, Compass, Location, Route, BreadcrumbPath |
| Files / 文件内容 | File, Folder, Document, Image, Upload, Download, Copy, Link, FileAdd, FolderOpen, Attachment, FileSearch, FileCheck, FileLock, Archive, TrashRestore |
| People / 用户权限 | User, Users, Role, Lock, Unlock, Shield, Login, Logout, UserAdd, UserRemove, Key, Permission, UserCheck, UserSettings, Organization, Fingerprint |
| Feedback / 消息反馈 | Info, Success, Warning, Error, Question, Bell, Message, Loading, BellOff, Mail, Send, Flag, NotificationRead, ChatReply, Heart, Star |
| Data / 数据图表 | Table, Database, LineChart, BarChart, PieChart, Dashboard, Filter, Sort, SortAscending, SortDescending, Statistics, TrendUp, ScatterPlot, AreaChart, Heatmap, DataImport |
| DateTime / 日期时间 | Calendar, Clock, DateRange, Timer, History, Schedule, PreviousDay, NextDay, Alarm, CalendarCheck, CalendarAdd, CalendarRemove, ClockCheck, ClockForward, CalendarWeek, TimeZone |
| Layout / 界面布局 | Window, Tabs, Sidebar, Grid, List, Split, Maximize, Minimize, Fullscreen, FullscreenExit, Pin, Unpin, DockBottom, PanelLeftClose, PanelLeftOpen, Layers |
| Devices / 系统设备 | Settings, Monitor, Mobile, Printer, Keyboard, Mouse, Sun, Moon, Play, Pause, Stop, Volume, Camera, Microphone, Headphones, Battery |
| Development / 开发网络 | Code, Terminal, Api, Server, Cloud, Network, Bug, Package, Wifi, WifiOff, Connection, Disconnect, GitBranch, GitCommit, Braces, Container |
| Workflow / 流程审批 | Start, End, Approval, Task, Branch, Parallel, Merge, Workflow, Approve, Reject, Transfer, Pending, Reassign, ApprovalChain, WorkflowValidate, WorkflowPublish |
| Design / 编辑设计 | Pointer, Text, Rectangle, Line, AlignLeft, AlignCenter, Barcode, QrCode, ZoomIn, ZoomOut, AlignRight, Crop, AlignTop, AlignBottom, DistributeHorizontal, Palette |
| Finance / 金融财务 | Wallet, CreditCard, Bank, Cash, Coins, Receipt, Invoice, Calculator, Payment, Refund, Exchange, CurrencyYuan, CurrencyDollar, Safe, FinancialReport, StockTrend |
| Industry / 工业制造 | Factory, Gear, Wrench, Hammer, RobotArm, Conveyor, Valve, Pump, Motor, Sensor, Gauge, Crane, Forklift, HardHat, Welding, Toolbox |
| Medical / 医疗健康 | MedicalCross, Hospital, Stethoscope, HeartPulse, Pill, Syringe, Thermometer, FirstAid, Ambulance, Wheelchair, Microscope, TestTube, Dna, Lungs, Tooth, Bandage |
| Infrastructure / 基础设施 | Building, Bridge, Road, Railway, Airport, PowerTower, PowerPlug, WaterDrop, WaterPipe, WindTurbine, SolarPanel, StreetLight, TrafficLight, FireHydrant, Warehouse, Recycle |
| Transport / 交通物流 | Truck, DeliveryVan, Car, Bus, Bicycle, Train, Ship, FlightTakeoff, DeliveryBox, DeliveryTracking, Map, NavigationArrow, Parking, Fuel, Anchor, TrafficCone |
| Commerce / 商业购物 | ShoppingCart, ShoppingBag, Store, TagLabel, Discount, Gift, Product, Scan, ShoppingBasket, Order, OrderCheck, OrderCancel, Coupon, LoyaltyCard, CustomerService, ShoppingCompare |
| Education / 教育学习 | Book, BookOpen, GraduationCap, School, Pen, Notebook, Library, Certificate, Lightbulb, Flask, Ruler, Globe, Language, Presentation, ReadAloud, Backpack |
| Weather / 天气环境 | CloudSun, CloudMoon, Rain, Snow, Thunderstorm, Wind, Fog, Tornado, Rainbow, Umbrella, TemperatureHigh, Leaf, Tree, Flower, Mountain, Earth |

Role 使用身份卡而非学位帽；Tabs 使用三段标签条而非文件夹；PreviousDay/NextDay 带日历边框；Parallel 是分叉/汇合栅栏，Merge 是多输入汇聚箭头。Warning/Success/Error 用形状区分，不只依赖颜色。新增的 Bank 使用柱廊，Factory 使用锯齿厂房，Hospital 使用医疗十字与裙楼，Building 使用窗格；Gear 与 Settings、HeartPulse 与 Heart、ClockForward 与 History 均使用不同图形。MedicalCross 是通用医疗符号，不是认证标识。QrCode、Barcode 仅为图标，不编码业务数据；生成真实二维码使用 [QRCode](qr-code.md)。

## 目录 API 与内置 Model

```csharp
IReadOnlyList<OneIconInfo> icons = OneIconCatalog.All;
OneIconInfo search = OneIconCatalog.Get(OneIconKind.Search);
var workflowIcons = icons.Where(icon => icon.Category == OneIconCategory.Workflow);
var financeIcons = icons.Where(icon => icon.Category == OneIconCategory.Finance);
bool supportsFilled = search.HasFilledVariant;
```

`OneIconCatalog.All` 是只读列表，保持枚举顺序；`Get` 对非法枚举抛出 `ArgumentOutOfRangeException`，不悄悄返回其他图形。没有全局注册或替换共享几何的 API。

`OneIconInfo` 是不可变 record：`Kind`（枚举标识）、`Category`（分类）、`Name`（稳定英文枚举名）、`DisplayName`（内置中文标签）、`HasFilledVariant`（是否提供独立实心轮廓）。Name 适合检索/日志；业务本地化可按 Kind 映射，不修改共享目录。枚举数值稳定，后续图标追加，既有编号不复用。

## 独立预览与验证

### 绘制与页面打开性能

图标保持一次 `DrawGeometry`，不按子路径创建控件。轮廓共享缓存，实心资源独立按需解析；每个实例保留自己的 Pen，继续支持可变画刷与主题更新，不引入全局画刷缓存。原有外观和公共 API 不变。

原预览页使用两层非虚拟化 ItemsControl，一次性构建全部 Button、文字和模板；包装页还构建未显示的 API/枚举表。现在按原生 VirtualizingStackPanel 可见行实现与回收卡片，只有列数变化时才重组行。不固定画廊高度、不增加内层滚动条；手写说明页的隐藏契约表仅在首次访问 Details 时创建。

2026-10-20，macOS ARM64 Release、Avalonia Headless + Skia、1100×850 窗口。每组独立进程，记录应用初始化后的首次打开，再记录 15 次完整打开。计时包含页面构造、布局和 RenderTargetBitmap 绘制。分配为 UI 线程累计托管分配，非峰值内存或显存；nearest-rank p95 在 15 个样本时等于最大值。

| 指标 | 优化前 256 个 | 优化后 256 个 | 扩充后 320 个 |
| --- | ---: | ---: | ---: |
| 打开 p50（ms） | 264.74 | 71.72 | 74.79 |
| 打开 p95（ms） | 768.96 | 321.63 | 304.45 |
| 单次 UI 线程分配中位数（MB） | 137.08 | 29.00 | 29.02 |
| 已实现可视控件 | 3,082 | 779 | 779 |
| 已实现图标（含右侧示例） | 267 | 50 | 50 |
| 首次打开，单次观测（ms） | 1,384.78 | 695.11 | 512.75 |

同规模下打开中位数下降约 73%，分配下降约 79%；扩充至 320 个后可视实例不增加。首次打开受 JIT、字体和主题冷启动影响，单次观测不代表稳定尾延迟。以上是页面 CPU、布局与离屏绘制数据，不是物理窗口帧率或单图标 GPU 光栅提速；这一轮尚未采集桌面合成器原生调用栈，后续桌面测量见下节。

PreviewHost → 基础控件 → **OneIcon**：320 个图标分类总览、中英文搜索、点击预览、变体与尺寸选择、三档密度、明暗对照、按钮搭配、额外属性/Model 的 Description 及可查看的编译绑定源码。画廊按可见行虚拟化，随宽度重组列数并回收离屏视图，全部元数据与选中项始终保留。窄窗口变为单列，内容自然增长，不固定画廊高度，也不增加内层滚动条。

专项只覆盖图标、实际预览绑定与图标宿主集成；包含 320 枚举与分类完整性、所有图形非空/边界检查、变体回退与恢复、主题/配色、密度/覆盖优先级、指针透传和焦点、PNG 图集及原生 AOT 消费。实际执行结果见本次交付，不将静态概念图视为运行验证。

2026-10-19：Release 构建通过；macOS ARM64 Skia headless 托管及原生 AOT 各 **4,847** 项通过，6 张明暗 × 三档密度图集在托管/AOT 间逐字节一致。原有 96 个图标在 6 张图集中的像素与扩充前完全一致。所有 144 个图标检查 1×/2× DPI、16/20/24 DIP，以及最大线宽边界；实际预览的枚举绑定、分类搜索和图标内指针点击通过。原生构建使用同一 Xcode 的 clang 与 SDK，没有新增裁剪/AOT 警告。未执行全库回归，未验证 Windows/Linux 或物理显示器呈现。

2026-10-20：扩充至 **16 类 256 个**，托管及 macOS ARM64 NativeAOT 图标专项各 **8,547 项通过**，Release 构建通过。覆盖全部图形在三档尺寸、1×/2× DPI 下的非空、边界、最大线宽、轮廓唯一性和变体切换，以及主题/配色、绑定、四类新筛选、窄窗口与按钮指针透传。修正传送带、传感器、叉车的大线宽越界，并区分时间顺延和历史图形。原有 144 个条目的定义和编号不变；六张图集的原有区域逐像素一致，托管与 NativeAOT 六张完整图集逐字节一致。只验证 OneIcon 及其预览/宿主集成，没有新增依赖或裁剪/AOT 警告；未执行全库回归、Windows/Linux 实跑或物理显示器检查。

2026-10-20 后续扩充与性能专项：新增交通物流、商业购物、教育学习、天气环境四类，合计 20 类 320 个；托管与 macOS ARM64 NativeAOT 各 **10,705 项通过**，Release 构建通过。补测可见行数量上限、滚动到底与返回、回收后的标签/枚举一致性、筛选清空与恢复、选择保留、搜索焦点、键盘激活和窄宽重排；对受延迟创建影响的五个手写说明页验证其契约入口仍可访问。修复新 CloudMoon 的轮廓越界。原有 256 个条目定义和编号不变，六组旧区域像素一致，托管与 NativeAOT 六张完整图集逐字节一致。没有新增依赖或裁剪/AOT 警告；未做全库回归、Windows/Linux 或物理显示器实跑。

### 真实桌面后续优化：打开与滚动尾延迟

剩余开销主要来自画廊控件创建、样式、绑定和布局。现在保留原生虚拟化行容器与 Button 槽位，移除每行通用 ItemsControl 包装。画廊 Button 直接调用既有 OneIcon 绘制路径，并缓存两行文字布局，减少 Presenter、文字等可视子树；仍使用原生 Button 的指针、键盘、焦点及自动化行为。首次 Measure 前确定列数；搜索词和分类未变化时不重建行。池中槽位只持有不可变目录数据，复用前完整替换。控件库公共 API、图形定义、主题资源和依赖不变。

2026-10-20，macOS ARM64 Release，真实 `Avalonia.Native.WindowImpl`，1100×850 DIP、2× 缩放，前后均为 320 个图标。各独立进程记录一次冷打开、随后 30 次打开，再执行 420 次确定性滚动偏移更新，覆盖三轮到底/返回。计时终点为原生渲染线程合成批次的 `Rendered` 完成，包含 UI 工作和调度等待；**不是物理屏幕呈现时间，也不是硬件滚轮/触摸板输入延迟**。分配为 UI 线程累计托管分配，不是进程总内存或显存；p95 使用 nearest-rank。调用栈采集与计时分开运行。

| 指标 | 优化前 | 优化后 |
| --- | ---: | ---: |
| 首次打开，单次观测（ms） | 713.07 | 356.62 |
| 后续打开 p50 / p95（ms），30 次 | 76.55 / 310.50 | 43.22 / 143.56 |
| 单次打开 UI 分配中位数（MB） | 28.80 | 8.16 |
| 打开时可视控件数 | 779 | 359 |
| 滚动更新 p50 / p95（ms），420 次 | 16.77 / 33.48 | 16.18 / 17.46 |
| 单次滚动 UI 分配中位数（bytes） | 3,552,744 | 114,896 |
| 整轮滚动新建图标实例 | 1,693 | 26 |
| 整轮滚动图标 Measure / Arrange 次数 | 1,693 / 1,693 | 26 / 26 |
| 整轮滚动图标绘制记录次数 | 1,693 | 1,693 |

Measure/Arrange 计数仅指 OneIcon 实例，不是全部控件布局次数。画廊现在使用私有图标绘制实例，不再作为可视子控件，因此不能仅数可视树中的 OneIcon 推算实例总数。必要的图标绘制记录次数没有减少，收益来自消除创建、布局和绑定抖动。前后各 500ms 静置检查均未发生图标创建、布局或绘制。

托管 CPU 采样定位到原有样式、模板、绑定和虚拟化布局开销；macOS 原生采样包含 AGX Metal 命令缓冲及渲染编码器栈，确认采集了真实桌面渲染后端。优化前轨迹包含打开和滚动，优化后轨迹覆盖滚动，其包含式采样耗时不能直接作为前后 CPU 百分比比较。临时采集和探针隔离在 `.preview/one-icons-desktop/`，未向 PreviewHost 正常入口增加基准参数。

托管及 macOS ARM64 NativeAOT 图标专项各 **10,709 项通过**，正常 Release 构建通过，保留一条既有的无关 LoginDemoViewModel 可空警告。新增回收 Button 元数据、相同筛选不重建、复用后绘制与可变画刷恢复检查，以及原生 Button 指针/空格激活和焦点检查；既有覆盖保留 320 图标、三档尺寸、1×/2× DPI、明暗主题、响应布局、搜索和选择。六张图标图集与上一版逐像素一致。另在真实桌面核验 Factory 搜索、点击/空格选择、清空筛选及滚轮向下/返回，搜索焦点保持。打开 p95 仍有 143.56ms 尾延迟；这些结果不代表 Windows/Linux 性能，也未消除冷启动成本。未执行全库回归。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### OneIcon

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.OneIcon.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Foreground`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L45) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | `IBrush?` / 继承宿主文字色 ; 支持主题资源与绑定；`null` 不绘制。通常继承，显式覆盖可用 `Foreground="{DynamicResource One.Accent}"`。 |
| [`IconSize`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L41) | `double` | 可写; OneWay | `20` | `double` / `20` ; 正有限 DIP；默认跟随三档密度；显式值优先。`IconSize="18"`。 校验：`value => double.IsFinite(value) && value > 0` |
| [`Kind`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L37) | `TheOne.Avalonia.Controls.OneIconKind` | 可写; OneWay | `OneIconKind.Add` | `OneIconKind` / `Add` ; 320 个有效枚举；`Kind="Save"`。未定义枚举值被拒绝。 校验：`value => Enum.IsDefined(value)` 可选值：`Add`, `Remove`, `Edit`, `Delete`, `Search`, `Save`, `Refresh`, `More`, `Home`, `Menu`, `Back`, `Forward`, `Up`, `Down`, `Expand`, `Collapse`, `File`, `Folder`, `Document`, `Image`, `Upload`, `Download`, `Copy`, `Link`, `User`, `Users`, `Role`, `Lock`, `Unlock`, `Shield`, `Login`, `Logout`, `Info`, `Success`, `Warning`, `Error`, `Question`, `Bell`, `Message`, `Loading`, `Table`, `Database`, `LineChart`, `BarChart`, `PieChart`, `Dashboard`, `Filter`, `Sort`, `Calendar`, `Clock`, `DateRange`, `Timer`, `History`, `Schedule`, `PreviousDay`, `NextDay`, `Window`, `Tabs`, `Sidebar`, `Grid`, `List`, `Split`, `Maximize`, `Minimize`, `Settings`, `Monitor`, `Mobile`, `Printer`, `Keyboard`, `Mouse`, `Sun`, `Moon`, `Code`, `Terminal`, `Api`, `Server`, `Cloud`, `Network`, `Bug`, `Package`, `Start`, `End`, `Approval`, `Task`, `Branch`, `Parallel`, `Merge`, `Workflow`, `Pointer`, `Text`, `Rectangle`, `Line`, `AlignLeft`, `AlignCenter`, `Barcode`, `QrCode`, `Close`, `Undo`, `Redo`, `Share`, `ChevronLeft`, `ChevronRight`, `ChevronUp`, `ChevronDown`, `FileAdd`, `FolderOpen`, `Attachment`, `FileSearch`, `UserAdd`, `UserRemove`, `Key`, `Permission`, `BellOff`, `Mail`, `Send`, `Flag`, `SortAscending`, `SortDescending`, `Statistics`, `TrendUp`, `Alarm`, `CalendarCheck`, `CalendarAdd`, `CalendarRemove`, `Fullscreen`, `FullscreenExit`, `Pin`, `Unpin`, `Play`, `Pause`, `Stop`, `Volume`, `Wifi`, `WifiOff`, `Connection`, `Disconnect`, `Approve`, `Reject`, `Transfer`, `Pending`, `ZoomIn`, `ZoomOut`, `AlignRight`, `Crop`, `Cut`, `Paste`, `PrintPreview`, `Export`, `Compass`, `Location`, `Route`, `BreadcrumbPath`, `FileCheck`, `FileLock`, `Archive`, `TrashRestore`, `UserCheck`, `UserSettings`, `Organization`, `Fingerprint`, `NotificationRead`, `ChatReply`, `Heart`, `Star`, `ScatterPlot`, `AreaChart`, `Heatmap`, `DataImport`, `ClockCheck`, `ClockForward`, `CalendarWeek`, `TimeZone`, `DockBottom`, `PanelLeftClose`, `PanelLeftOpen`, `Layers`, `Camera`, `Microphone`, `Headphones`, `Battery`, `GitBranch`, `GitCommit`, `Braces`, `Container`, `Reassign`, `ApprovalChain`, `WorkflowValidate`, `WorkflowPublish`, `AlignTop`, `AlignBottom`, `DistributeHorizontal`, `Palette`, `Wallet`, `CreditCard`, `Bank`, `Cash`, `Coins`, `Receipt`, `Invoice`, `Calculator`, `Payment`, `Refund`, `Exchange`, `CurrencyYuan`, `CurrencyDollar`, `Safe`, `FinancialReport`, `StockTrend`, `Factory`, `Gear`, `Wrench`, `Hammer`, `RobotArm`, `Conveyor`, `Valve`, `Pump`, `Motor`, `Sensor`, `Gauge`, `Crane`, `Forklift`, `HardHat`, `Welding`, `Toolbox`, `MedicalCross`, `Hospital`, `Stethoscope`, `HeartPulse`, `Pill`, `Syringe`, `Thermometer`, `FirstAid`, `Ambulance`, `Wheelchair`, `Microscope`, `TestTube`, `Dna`, `Lungs`, `Tooth`, `Bandage`, `Building`, `Bridge`, `Road`, `Railway`, `Airport`, `PowerTower`, `PowerPlug`, `WaterDrop`, `WaterPipe`, `WindTurbine`, `SolarPanel`, `StreetLight`, `TrafficLight`, `FireHydrant`, `Warehouse`, `Recycle`, `Truck`, `DeliveryVan`, `Car`, `Bus`, `Bicycle`, `Train`, `Ship`, `FlightTakeoff`, `DeliveryBox`, `DeliveryTracking`, `Map`, `NavigationArrow`, `Parking`, `Fuel`, `Anchor`, `TrafficCone`, `ShoppingCart`, `ShoppingBag`, `Store`, `TagLabel`, `Discount`, `Gift`, `Product`, `Scan`, `ShoppingBasket`, `Order`, `OrderCheck`, `OrderCancel`, `Coupon`, `LoyaltyCard`, `CustomerService`, `ShoppingCompare`, `Book`, `BookOpen`, `GraduationCap`, `School`, `Pen`, `Notebook`, `Library`, `Certificate`, `Lightbulb`, `Flask`, `Ruler`, `Globe`, `Language`, `Presentation`, `ReadAloud`, `Backpack`, `CloudSun`, `CloudMoon`, `Rain`, `Snow`, `Thunderstorm`, `Wind`, `Fog`, `Tornado`, `Rainbow`, `Umbrella`, `TemperatureHigh`, `Leaf`, `Tree`, `Flower`, `Mountain`, `Earth` |
| [`StrokeThickness`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L43) | `double` | 可写; OneWay | `1.75` | `double` / `1.75` ; 24 单位设计网格的轮廓线宽，范围 `(0, 3]`，随图标等比缩放；不影响实心轮廓。`StrokeThickness="1.5"`。 校验：`value => double.IsFinite(value) && value is > 0 and <= 3` |
| [`Variant`](../../../TheOne.Avalonia/Controls/OneIcon.cs#L39) | `TheOne.Avalonia.Controls.OneIconVariant` | 可写; OneWay | `OneIconVariant.Outline` | `OneIconVariant` / `Outline` ; `Outline` 或 `Filled`；无实心资源时回退同一图标的 Outline。`Variant="Filled"`。 校验：`value => Enum.IsDefined(value)` 可选值：`Outline`, `Filled` |

### OneIconCatalog

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.OneIconCatalog.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`All`](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L402) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.OneIconInfo>` | 只读; CLR / n/a | `Array.AsReadOnly(Entries.Select(entry => entry.Info).ToArray())` | Read-only metadata for all glyphs, in stable enum order. |

### OneIconCategory

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.OneIconCategory.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### OneIconInfo

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.OneIconInfo.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Category`](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L6) | `TheOne.Avalonia.Controls.OneIconCategory` | 可写; CLR / init-only | `required constructor argument` | 节点库的分类名称；用于组织定义列表。 可选值：`Actions`, `Navigation`, `Files`, `People`, `Feedback`, `Data`, `DateTime`, `Layout`, `Devices`, `Development`, `Workflow`, `Design`, `Finance`, `Industry`, `Medical`, `Infrastructure`, `Transport`, `Commerce`, `Education`, `Weather` |
| [`DisplayName`](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L6) | `string` | 可写; CLR / init-only | `required constructor argument` | OneIconInfo 的 DisplayName 配置字段；值类型和初值如表，通过此模型的公开属性访问器读写。具体上下文见控件用法及链接处的声明／访问器。 |
| [`HasFilledVariant`](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L6) | `bool` | 可写; CLR / init-only | `required constructor argument` | OneIconInfo 的 HasFilledVariant 配置字段；值类型和初值如表，通过此模型的公开属性访问器读写。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Kind`](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L6) | `TheOne.Avalonia.Controls.OneIconKind` | 可写; CLR / init-only | `required constructor argument` | `OneIconKind` / `Add` ; 320 个有效枚举；`Kind="Save"`。未定义枚举值被拒绝。 可选值：`Add`, `Remove`, `Edit`, `Delete`, `Search`, `Save`, `Refresh`, `More`, `Home`, `Menu`, `Back`, `Forward`, `Up`, `Down`, `Expand`, `Collapse`, `File`, `Folder`, `Document`, `Image`, `Upload`, `Download`, `Copy`, `Link`, `User`, `Users`, `Role`, `Lock`, `Unlock`, `Shield`, `Login`, `Logout`, `Info`, `Success`, `Warning`, `Error`, `Question`, `Bell`, `Message`, `Loading`, `Table`, `Database`, `LineChart`, `BarChart`, `PieChart`, `Dashboard`, `Filter`, `Sort`, `Calendar`, `Clock`, `DateRange`, `Timer`, `History`, `Schedule`, `PreviousDay`, `NextDay`, `Window`, `Tabs`, `Sidebar`, `Grid`, `List`, `Split`, `Maximize`, `Minimize`, `Settings`, `Monitor`, `Mobile`, `Printer`, `Keyboard`, `Mouse`, `Sun`, `Moon`, `Code`, `Terminal`, `Api`, `Server`, `Cloud`, `Network`, `Bug`, `Package`, `Start`, `End`, `Approval`, `Task`, `Branch`, `Parallel`, `Merge`, `Workflow`, `Pointer`, `Text`, `Rectangle`, `Line`, `AlignLeft`, `AlignCenter`, `Barcode`, `QrCode`, `Close`, `Undo`, `Redo`, `Share`, `ChevronLeft`, `ChevronRight`, `ChevronUp`, `ChevronDown`, `FileAdd`, `FolderOpen`, `Attachment`, `FileSearch`, `UserAdd`, `UserRemove`, `Key`, `Permission`, `BellOff`, `Mail`, `Send`, `Flag`, `SortAscending`, `SortDescending`, `Statistics`, `TrendUp`, `Alarm`, `CalendarCheck`, `CalendarAdd`, `CalendarRemove`, `Fullscreen`, `FullscreenExit`, `Pin`, `Unpin`, `Play`, `Pause`, `Stop`, `Volume`, `Wifi`, `WifiOff`, `Connection`, `Disconnect`, `Approve`, `Reject`, `Transfer`, `Pending`, `ZoomIn`, `ZoomOut`, `AlignRight`, `Crop`, `Cut`, `Paste`, `PrintPreview`, `Export`, `Compass`, `Location`, `Route`, `BreadcrumbPath`, `FileCheck`, `FileLock`, `Archive`, `TrashRestore`, `UserCheck`, `UserSettings`, `Organization`, `Fingerprint`, `NotificationRead`, `ChatReply`, `Heart`, `Star`, `ScatterPlot`, `AreaChart`, `Heatmap`, `DataImport`, `ClockCheck`, `ClockForward`, `CalendarWeek`, `TimeZone`, `DockBottom`, `PanelLeftClose`, `PanelLeftOpen`, `Layers`, `Camera`, `Microphone`, `Headphones`, `Battery`, `GitBranch`, `GitCommit`, `Braces`, `Container`, `Reassign`, `ApprovalChain`, `WorkflowValidate`, `WorkflowPublish`, `AlignTop`, `AlignBottom`, `DistributeHorizontal`, `Palette`, `Wallet`, `CreditCard`, `Bank`, `Cash`, `Coins`, `Receipt`, `Invoice`, `Calculator`, `Payment`, `Refund`, `Exchange`, `CurrencyYuan`, `CurrencyDollar`, `Safe`, `FinancialReport`, `StockTrend`, `Factory`, `Gear`, `Wrench`, `Hammer`, `RobotArm`, `Conveyor`, `Valve`, `Pump`, `Motor`, `Sensor`, `Gauge`, `Crane`, `Forklift`, `HardHat`, `Welding`, `Toolbox`, `MedicalCross`, `Hospital`, `Stethoscope`, `HeartPulse`, `Pill`, `Syringe`, `Thermometer`, `FirstAid`, `Ambulance`, `Wheelchair`, `Microscope`, `TestTube`, `Dna`, `Lungs`, `Tooth`, `Bandage`, `Building`, `Bridge`, `Road`, `Railway`, `Airport`, `PowerTower`, `PowerPlug`, `WaterDrop`, `WaterPipe`, `WindTurbine`, `SolarPanel`, `StreetLight`, `TrafficLight`, `FireHydrant`, `Warehouse`, `Recycle`, `Truck`, `DeliveryVan`, `Car`, `Bus`, `Bicycle`, `Train`, `Ship`, `FlightTakeoff`, `DeliveryBox`, `DeliveryTracking`, `Map`, `NavigationArrow`, `Parking`, `Fuel`, `Anchor`, `TrafficCone`, `ShoppingCart`, `ShoppingBag`, `Store`, `TagLabel`, `Discount`, `Gift`, `Product`, `Scan`, `ShoppingBasket`, `Order`, `OrderCheck`, `OrderCancel`, `Coupon`, `LoyaltyCard`, `CustomerService`, `ShoppingCompare`, `Book`, `BookOpen`, `GraduationCap`, `School`, `Pen`, `Notebook`, `Library`, `Certificate`, `Lightbulb`, `Flask`, `Ruler`, `Globe`, `Language`, `Presentation`, `ReadAloud`, `Backpack`, `CloudSun`, `CloudMoon`, `Rain`, `Snow`, `Thunderstorm`, `Wind`, `Fog`, `Tornado`, `Rainbow`, `Umbrella`, `TemperatureHigh`, `Leaf`, `Tree`, `Flower`, `Mountain`, `Earth` |
| [`Name`](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L6) | `string` | 可写; CLR / init-only | `required constructor argument` | 名称字段，用于业务展示或标识；与显示模板、键值字段保持一致。 |

### OneIconKind

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.OneIconKind.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### OneIconVariant

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.OneIconVariant.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`OneIconCatalog`](../../api/zh-cn/TheOne.Avalonia.Controls.OneIconCatalog.md) | `static OneIconInfo OneIconCatalog.Get(OneIconKind kind)` | Gets metadata for a valid enum value. Throws for undefined values. |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.OneIconCategory`](../../api/zh-cn/TheOne.Avalonia.Controls.OneIconCategory.md) | 20 |
| [`TheOne.Avalonia.Controls.OneIconInfo`](../../api/zh-cn/TheOne.Avalonia.Controls.OneIconInfo.md) | 5 |
| [`TheOne.Avalonia.Controls.OneIconKind`](../../api/zh-cn/TheOne.Avalonia.Controls.OneIconKind.md) | 320 |
| [`TheOne.Avalonia.Controls.OneIconVariant`](../../api/zh-cn/TheOne.Avalonia.Controls.OneIconVariant.md) | 2 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
