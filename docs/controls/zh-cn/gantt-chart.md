# GanttChart 甘特图

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

```bash
dotnet build design/PreviewHost/PreviewHost.csproj -c Release --artifacts-path .preview/gantt/artifacts
dotnet .preview/gantt/artifacts/bin/PreviewHost/release/PreviewHost.dll --verify-gantt .preview/gantt/managed
# macOS：仅在涉及 AOT／绑定／元数据时执行原生专项。
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 --artifacts-path .preview/gantt/native-artifacts -o .preview/gantt/native
.preview/gantt/native/NativeAotSmoke --require-aot --verify-gantt .preview/gantt/native-pixels
```

专项检查明暗主题、三档尺寸、窄窗口、实际任务与今天线像素、层级与选择、只读拖动、数据校验、重新挂载、编译绑定和远距离虚拟滚动。NativeAOT 命令必须执行原生产物，不能用托管运行替代。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 的“控件能力实验室”（各页右上入口）按样式、属性、API／事件和模型分类演示；复杂模型使用原业务示例。

### GanttChart

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.GanttChart.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`DayWidth`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L57) | `double` | 可写; OneWay | `48` | 甘特时间轴每一天的 DIP 宽度；改变横向缩放，不修改任务日期。 校验：`v => double.IsFinite(v) && v >= 4 && v <= 240` |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L72) | `string` | 可写; OneWay | `"No tasks"` | 没有可显示数据时的提示文本；由应用覆盖以进行本地化。 |
| [`GridBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L61) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 绘图区网格线画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`HeaderBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L62) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 甘特表头背景画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`HeaderHeight`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L60) | `double` | 可写; OneWay | `72` | 标题区高度；图表使用 DIP，报表布局使用其布局坐标，勿与毫米源模型混用。 校验：`Positive` |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L52) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Controls.GanttTask>?` | 可写; OneWay | `null` | 绑定原始模型集合，不预建大量 Control；动态增删使用带通知的集合，并提供有限视口。 |
| [`MutedBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L67) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 次要文字与辅助标记画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`OnTaskBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L68) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 任务条上方的对比文字画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`OwnerHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L70) | `string` | 可写; OneWay | `"Owner"` | 甘特左侧负责人列的标题文本。 |
| [`ProgressBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L65) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 甘特已完成进度区画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`ProgressHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L71) | `string` | 可写; OneWay | `"Progress"` | 甘特左侧完成进度列的标题文本。 |
| [`RowHeight`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L58) | `double` | 可写; OneWay | `44` | 每一行的 DIP 高度；与视口和回收／直接绘制布局一起决定可见行数。 校验：`Positive` |
| [`SelectedTask`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L53) | `TheOne.Avalonia.Controls.GanttTask?` | 可写; TwoWay | `null` | 当前选中的原始甘特任务；空值清除选择，保持模型对象身份。 |
| [`Selection`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L75) | `TheOne.Avalonia.Controls.GanttSelection?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SelectionBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L66) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 当前选中任务的强调背景画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`TaskBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L64) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 甘特任务条底色画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`TaskHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L69) | `string` | 可写; OneWay | `"Task"` | 甘特左侧任务名称列的标题文本。 |
| [`TaskListWidth`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L59) | `double` | 可写; OneWay | `420` | 甘特左侧任务信息区的 DIP 宽度；剩余区域分配给时间轴。 校验：`Positive` |
| [`Today`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L56) | `System.DateOnly` | 可写; OneWay | `DateOnly.FromDateTime(DateTime.Today)` | 时间轴上“今天”标记的日期；可显式设置以演示或固定业务日期。 |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L73) | `string?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ViewEnd`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L55) | `System.DateOnly` | 可写; OneWay | `DateOnly.FromDateTime(DateTime.Today).AddDays(31)` | Exclusive right boundary of the time axis. |
| [`ViewStart`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L54) | `System.DateOnly` | 可写; OneWay | `DateOnly.FromDateTime(DateTime.Today)` | 甘特可见日期范围起点；与终点共同定义时间轴，不修改任务模型。 |
| [`VisibleTaskCount`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L74) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`WeekendBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L63) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 时间轴周末区域画刷；动态主题使用 One.* 资源，显式局部值优先。 |

### GanttHeaderPresenter

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.GanttHeaderPresenter.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### 公共附加属性

[ControlSizing](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性; 向后代继承 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

[Motion](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| GanttChart | `one-large` | 模板自动类；不手动绑定 | ^.one-large => FontSize=16; RowHeight=52; HeaderHeight=80 |
| GanttChart | `one-small` | 模板自动类；不手动绑定 | ^.one-small => FontSize=12; RowHeight=36; HeaderHeight=64 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.GanttChart`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttChart.md) | Class | 24 | 5 | 1 |
| [`TheOne.Avalonia.Controls.GanttHeaderPresenter`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttHeaderPresenter.md) | Class | 0 | 2 | 0 |
| [`TheOne.Avalonia.Controls.GanttSelection`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttSelection.md) | Class | 6 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GanttTask`](../../api/zh-cn/TheOne.Avalonia.Controls.GanttTask.md) | Class | 9 | 1 | 1 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
