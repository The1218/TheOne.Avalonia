# GanttTask

[English](../en/TheOne.Avalonia.Controls.GanttTask.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GanttTask.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GanttTask` · Class · `object`

An all-day task. Non-milestone leaf tasks use [Start, EndExclusive).

[Source / 声明](../../../TheOne.Avalonia/Controls/GanttTask.cs#L13)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`EndExclusive`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L20) | `System.DateOnly` | 可写; CLR / n/a | `DateOnly.FromDateTime(DateTime.Today).AddDays(1)` | 甘特任务结束日期，不包含当日；里程碑和汇总任务按甘特契约解释。 |
| [`Id`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L15) | `string` | 可写; CLR / n/a | `Guid.NewGuid().ToString("N")` | 模型稳定标识；持久化／引用使用，避免在同一文档中重复。 |
| [`IsExpanded`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L24) | `bool` | 可写; CLR / n/a | `true` | 当前节点展开状态；需要在回收或数据重新加载后保留时，将其绑定到稳定模型。 |
| [`IsMilestone`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L23) | `bool` | 可写; CLR / n/a | `false` | 将任务显示为里程碑；不将其当作普通持续区间条。 |
| [`Owner`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L18) | `string?` | 可写; CLR / n/a | `null` | 任务负责人文本，与控件弹窗的窗口 owner 是不同概念。 |
| [`ParentId`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L16) | `string?` | 可写; CLR / n/a | `null` | 父任务的稳定标识；空值表示根级，不能形成循环。 |
| [`Progress`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L22) | `double` | 可写; CLR / n/a | `0 / default` | Completion fraction in [0, 1]. Summary values are computed from descendants. |
| [`Start`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L19) | `System.DateOnly` | 可写; CLR / n/a | `DateOnly.FromDateTime(DateTime.Today)` | 甘特任务起始日期，含当日；范围终点采用 EndExclusive。 |
| [`Title`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L17) | `string` | 可写; CLR / n/a | `""` | 显示标题；不等于业务主键或本地化资源键。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`GanttTask.GanttTask()`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L13) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event PropertyChangedEventHandler? GanttTask.PropertyChanged`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L25) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
