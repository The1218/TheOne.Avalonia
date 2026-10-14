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
dotnet build TheOne.Avalonia.slnx
dotnet design/PreviewHost/bin/Debug/net10.0/PreviewHost.dll --verify-gantt .preview/gantt/managed
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 -o .preview/gantt/native
.preview/gantt/native/NativeAotSmoke --require-aot --verify-gantt .preview/gantt/native-pixels
```

专项检查明暗主题、三档尺寸、窄窗口、实际任务与今天线像素、层级与选择、只读拖动、数据校验、重新挂载、编译绑定和远距离虚拟滚动。NativeAOT 命令必须执行原生产物，不能用托管运行替代。
