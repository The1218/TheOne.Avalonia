# GanttChart 甘特图

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

[English](../en/gantt-chart.md) · **简体中文**

`GanttChart` 是全天任务的只读甘特图。任务列表使用原生 ListBox 的选择、键盘导航和行虚拟化；每个可见行同时绘制任务信息与时间条，共用垂直滚动。水平 ScrollBar 只移动时间轴，表头保持固定。

## 编译绑定示例

视图加载 OneTheme，并声明实际 ViewModel 的 `x:DataType`：

```xml
<one:GanttChart Height="520"
                ItemsSource="{Binding Tasks}"
                SelectedTask="{Binding SelectedTask, Mode=TwoWay}"
                ViewStart="{Binding ViewStart, Mode=TwoWay}"
                ViewEnd="{Binding ViewEnd, Mode=TwoWay}"
                DayWidth="{Binding DayWidth, Mode=TwoWay}"
                TaskHeader="任务名称" OwnerHeader="负责人" ProgressHeader="进度" />
```

`Tasks` 可使用 `ObservableCollection<GanttTask>`；模型实现 INotifyPropertyChanged，集合增删和模型变化自动刷新。普通 List 修改后调用 `RefreshData()`。集合和模型变更、方法调用应在 UI 线程执行。控件需要有限高度；避免无限高度的外层 ScrollViewer。

```csharp
public ObservableCollection<GanttTask> Tasks { get; } =
[
    new() { Id = "project", Title = "控件开发", Owner = "研发组" },
    new() { Id = "drawing", ParentId = "project", Title = "时间轴绘制", Owner = "周宁",
            Start = new(2026, 10, 12), EndExclusive = new(2026, 10, 17), Progress = .6 },
    new() { Id = "acceptance", ParentId = "project", Title = "版本验收",
            Start = new(2026, 10, 23), IsMilestone = true }
];
```

## 单任务增量刷新

公共属性与 API 保持不变，不需要调用额外方法：

```csharp
task.Title = "交互设计完成";
task.Owner = "周宁";
task.Progress = .8;
task.EndExclusive = new DateOnly(2026, 10, 20);
// 同一轮 UI 通知合并处理；保留行对象、原生容器、焦点与滚动位置。
```

- `Title`／`Owner` 只刷新该任务的已实现行、ToolTip 和自动化名称，不枚举数据源、不重建任务树或订阅。
- `Progress`／`Start`／`EndExclusive`／`IsMilestone` 更新叶任务缓存，再按受影响祖先更新日期与日历天数加权进度。普通进度更新沿父链传播；原有最早／最晚日期向内收缩时，重新检查对应父节点的直接子任务，不扫描整棵树。汇总任务自己的日期与进度仍不参与计算。
- `IsExpanded` 只在可见行集合中插入／移除对应后代，保留其他行的身份及订阅；折叠后选择移到最近的可见祖先。调整行号的成本随后续可见行数量增长。
- 集合变化、`Id`／`ParentId`、空属性名或未知属性通知，以及显式 `RefreshData()` 仍完整校验并重建任务树。订阅按实际源与模型成员变化增减，不重复订阅未改变的模型。
- 无效数据仍使整个图表隐藏，修正通知后完整恢复；卸载取消订阅，切换源不会应用旧任务的待处理更新。普通集合的未通知修改仍需 `RefreshData()`。

增量路径增加任务引用索引与加权完成量缓存，均随任务数增长。本次优化针对已加载后的任务更新，不承诺降低首次加载或结构重建成本。所有通知仍须在 UI 线程发出。

### 性能记录与复测

2026-10-18 当前源码专项：macOS 27.0.1 ARM64、.NET 10.0.0、Release、Skia Headless；固定 1100 × 520 视口、关闭动效，每场景预热 8 次、采样 32 次。基线控件源码为 `e083204`。下表是 10,000 任务、每组 100 个子任务的 UI 线程更新／队列处理／布局耗时，不代表 GPU 或屏幕呈现延迟；分配量另包含该次 Headless 帧处理，仅统计当前线程。

| 修改 | 中位耗时：优化前 → 后 | P95：优化前 → 后 | 每次分配中位数：优化前 → 后 |
| --- | --- | --- | --- |
| 标题 | 7.593 → 0.383 ms | 13.413 → 0.443 ms | 6,431,560 → 54,704 B |
| 负责人 | 6.374 → 0.422 ms | 12.683 → 0.917 ms | 6,467,632 → 58,296 B |
| 进度 | 6.426 → 0.741 ms | 14.763 → 1.319 ms | 6,406,280 → 118,248 B |
| 日期 | 6.077 → 0.710 ms | 12.914 → 1.317 ms | 6,404,752 → 118,648 B |

另测 1,000／10,000 任务的宽树。全部 16 个场景中，优化前每组 32 次更新均枚举源、重新订阅源并 Reset 行集合 32 次；优化后这些计数均为 0，选中行对象保持不变。日期场景交替扩大／收缩边界，包含直接子任务重查；宽树的此类重查最坏仍随该父节点的子任务数增长。测试不设置依赖机器速度的固定耗时通过门槛。

## 数据契约

- `Id` 非空且唯一，`ParentId = null` 表示根任务；父任务必须在同一源中，禁止循环。兄弟任务按源顺序排列。
- 普通叶任务采用 `DateOnly` 的 `[Start, EndExclusive)`：上例显示 10 月 12—16 日。`Progress` 是 0—1 的有限数值。
- 里程碑只使用 Start，不使用 EndExclusive；不能包含子任务。
- 包含子任务的节点自动成为汇总任务：日期包含所有后代，进度按叶任务的日历天数加权；里程碑不参与进度权重。汇总节点自身的日期和进度不参与计算。
- `IsExpanded` 默认 true，存储在模型中；折叠不会修改任务日期。选中的子任务被折叠后，选择移到最近的可见祖先。
- 不合法的任务源整体不显示，`ValidationError` 返回原因；修正可通知的数据后自动恢复。`VisibleTaskCount` 是展开后可见的任务数量。

## 视口、选择与输入

`ViewStart` / `ViewEnd` 同样采用左闭右开区间；ViewEnd 必须大于 ViewStart。`DayWidth` 是每个日历日的像素宽度，范围 4—240；缩小后日期标签按周／月减少，日期坐标仍按等宽日历日映射。非工作日只是底色，不压缩时间。

`SelectedTask` 支持双向绑定。只读 `Selection` 返回实际显示的 Start、EndExclusive、DisplayEnd、Progress、IsSummary 与 Task，适合详情面板；汇总任务应读取 Selection，而非模型自己的日期。`SelectionChanged` 在选择或其计算值变化时触发。

| 输入 / 方法 | 行为 |
| --- | --- |
| 行点击、上／下键 | 原生 ListBox 选择与定位 |
| 展开按钮、左／右键 | 折叠／展开，或移动到父任务／首个子任务 |
| 滚轮 | 原生垂直滚动，左右内容共用一行 |
| Shift + 滚轮、水平滚轮 | 只移动时间轴 |
| Ctrl / ⌘ + 滚轮 | 围绕指针缩放时间轴 |
| `ScrollToToday()` | 将 Today 所在日期尽量居中 |
| `FitToProject()` | 按包含折叠后代的项目范围适应当前宽度 |
| `BringTaskIntoView(id)` | 展开祖先，选择并定位任务；未找到返回 false |

首版始终只读，没有 IsReadOnly 切换、拖动编辑、依赖线、工作日历、自动排期或导出 API。搜索、负责人筛选和工具栏由消费端组合；过滤源必须保留匹配任务的祖先。

## 主题与尺寸

`RowHeight` 默认 Small / Normal / Large 为 36 / 44 / 52，`HeaderHeight` 为 64 / 72 / 80；TaskListWidth 默认 420。窄窗口缩小任务区并隐藏负责人、进度列，保留完整任务的 ToolTip 与横向时间轴滚动。

默认画刷由 `One.Gantt.Background`、`Text`、`Grid`、`Header`、`Weekend`、`Task`、`Progress`、`Selection`、`Muted`、`OnTask` 动态资源提供，继承 One 的明暗语义色；对应画刷属性可单独覆盖。

模板保留 `PART_Header`（GanttHeaderPresenter）、`PART_Rows`（ListBox）、`PART_HorizontalScrollBar`（ScrollBar）和 `PART_Empty`（TextBlock）。行容器必须零 Padding，拉伸内容并保留 VirtualizingStackPanel。任务条、网格、今天线与文本直接绘制，展开按钮是原生 Button；行和按钮提供自动化名称。

## 验证

PreviewHost 的“数据展示 / GanttChart”页面包含设计稿任务、筛选、缩放、全部展开／折叠，以及 1,500 条任务和末行定位。

专项检查明暗主题、三档尺寸、窄窗口、实际任务与今天线像素、层级与选择、只读拖动、数据校验、重新挂载、编译绑定和远距离虚拟滚动。NativeAOT 命令必须执行原生产物，不能用托管运行替代。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### GanttChart

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.GanttChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`DayWidth`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L149) | `double` | 可写; OneWay | `48` | 甘特时间轴每一天的 DIP 宽度；改变横向缩放，不修改任务日期。 校验：`v => double.IsFinite(v) && v >= 4 && v <= 240` |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L239) | `string` | 可写; OneWay | `"No tasks"` | 没有可显示数据时的提示文本；由应用覆盖以进行本地化。 |
| [`GridBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L173) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 绘图区网格线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`HeaderBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L179) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 甘特表头背景画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`HeaderHeight`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L167) | `double` | 可写; OneWay | `72` | 标题区高度；图表使用 DIP，报表布局使用其布局坐标，勿与毫米源模型混用。 校验：`Positive` |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L119) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Controls.GanttTask>?` | 可写; OneWay | `null` | 绑定原始模型集合，不预建大量 Control；动态增删使用带通知的集合，并提供有限视口。 |
| [`MutedBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L209) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 次要文字与辅助标记画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`OnTaskBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L215) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 任务条上方的对比文字画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`OwnerHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L227) | `string` | 可写; OneWay | `"Owner"` | 甘特左侧负责人列的标题文本。 |
| [`ProgressBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L197) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 甘特已完成进度区画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`ProgressHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L233) | `string` | 可写; OneWay | `"Progress"` | 甘特左侧完成进度列的标题文本。 |
| [`RowHeight`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L155) | `double` | 可写; OneWay | `44` | 每一行的 DIP 高度；与视口和回收／直接绘制布局一起决定可见行数。 校验：`Positive` |
| [`SelectedTask`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L125) | `TheOne.Avalonia.Controls.GanttTask?` | 可写; TwoWay | `null` | 当前选中的原始甘特任务；空值清除选择，保持模型对象身份。 |
| [`Selection`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L247) | `TheOne.Avalonia.Controls.GanttSelection?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SelectionBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L203) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 当前选中任务的强调背景画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`TaskBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L191) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 甘特任务条底色画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`TaskHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L221) | `string` | 可写; OneWay | `"Task"` | 甘特左侧任务名称列的标题文本。 |
| [`TaskListWidth`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L161) | `double` | 可写; OneWay | `420` | 甘特左侧任务信息区的 DIP 宽度；剩余区域分配给时间轴。 校验：`Positive` |
| [`Today`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L143) | `System.DateOnly` | 可写; OneWay | `DateOnly.FromDateTime(DateTime.Today)` | 时间轴上“今天”标记的日期；可显式设置以演示或固定业务日期。 |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L245) | `string?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ViewEnd`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L137) | `System.DateOnly` | 可写; OneWay | `DateOnly.FromDateTime(DateTime.Today).AddDays(31)` | Exclusive right boundary of the time axis. |
| [`ViewStart`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L131) | `System.DateOnly` | 可写; OneWay | `DateOnly.FromDateTime(DateTime.Today)` | 甘特可见日期范围起点；与终点共同定义时间轴，不修改任务模型。 |
| [`VisibleTaskCount`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L246) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`WeekendBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L185) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 时间轴周末区域画刷；动态主题使用 One.* 资源，显式局部值优先。 |

### GanttHeaderPresenter

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.GanttHeaderPresenter.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`GanttChart`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttChart.md) | `void GanttChart.RefreshData()` | Refresh a non-notifying source. Call on the UI thread. |
| [`GanttChart`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttChart.md) | `void GanttChart.ScrollToToday()` | 将 Today 所在日期尽量居中 |
| [`GanttChart`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttChart.md) | `void GanttChart.FitToProject()` | Fit the complete project, including collapsed descendants, to the current width. |
| [`GanttChart`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttChart.md) | `bool GanttChart.BringTaskIntoView(string id)` | Expand ancestors and reveal the row and its date, without changing task dates. |

### 事件订阅

按需订阅下列通知，并在宿主释放时解除订阅；这里只列事件入口，不列路由事件标识字段。

| 类型 | 事件 | 用途 |
| --- | --- | --- |
| [`GanttChart`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttChart.md) | `SelectionChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`GanttTask`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttTask.md) | `PropertyChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.GanttSelection`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttSelection.md) | 6 |
| [`TheOne.Avalonia.Controls.GanttTask`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttTask.md) | 9 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
