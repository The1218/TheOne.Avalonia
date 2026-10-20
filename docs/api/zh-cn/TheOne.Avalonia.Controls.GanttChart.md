# GanttChart

[English](../en/TheOne.Avalonia.Controls.GanttChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GanttChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GanttChart` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A read-only all-day Gantt with native virtualized selection and one shared row viewport.

[Source / 声明](../../../TheOne.Avalonia/Controls/GanttChart.Rendering.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

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

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void GanttChart.RefreshData()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L352) | Refresh a non-notifying source. Call on the UI thread. |
| [`void GanttChart.ScrollToToday()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L630) | 将 Today 所在日期尽量居中 |
| [`void GanttChart.FitToProject()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L639) | Fit the complete project, including collapsed descendants, to the current width. |
| [`bool GanttChart.BringTaskIntoView(string id)`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L657) | Expand ancestors and reveal the row and its date, without changing task dates. |
| [`GanttChart.GanttChart()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L113) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler? GanttChart.SelectionChanged`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L248) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IEnumerable<GanttTask>?> GanttChart.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<GanttTask?> GanttChart.SelectedTaskProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<DateOnly> GanttChart.ViewStartProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<DateOnly> GanttChart.ViewEndProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L30) | Exclusive right boundary of the time axis. |
| [`static readonly StyledProperty<DateOnly> GanttChart.TodayProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GanttChart.DayWidthProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GanttChart.RowHeightProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L41) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GanttChart.TaskListWidthProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L44) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> GanttChart.HeaderHeightProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L47) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GanttChart.GridBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L50) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GanttChart.HeaderBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L53) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GanttChart.WeekendBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L56) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GanttChart.TaskBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L59) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GanttChart.ProgressBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L62) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GanttChart.SelectionBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L65) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GanttChart.MutedBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L68) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> GanttChart.OnTaskBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L71) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> GanttChart.TaskHeaderProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L74) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> GanttChart.OwnerHeaderProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L77) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> GanttChart.ProgressHeaderProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L80) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> GanttChart.EmptyTextProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L83) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<GanttChart, string?> GanttChart.ValidationErrorProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L86) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<GanttChart, int> GanttChart.VisibleTaskCountProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L89) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<GanttChart, GanttSelection?> GanttChart.SelectionProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L92) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.GanttSelection`](TheOne.Avalonia.Controls.GanttSelection.md)
- [`TheOne.Avalonia.Controls.GanttTask`](TheOne.Avalonia.Controls.GanttTask.md)
