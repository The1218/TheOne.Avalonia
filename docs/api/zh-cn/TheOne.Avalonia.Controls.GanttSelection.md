# GanttSelection

[English](../en/TheOne.Avalonia.Controls.GanttSelection.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GanttSelection.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GanttSelection` · Class · `object`

The calculated display values for a selected leaf, milestone or summary task.

[Source / 声明](../../../TheOne.Avalonia/Controls/GanttTask.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`DisplayEnd`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L14) | `System.DateOnly` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`EndExclusive`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L10) | `System.DateOnly` | 可写; CLR / init-only | `required constructor argument` | 甘特任务结束日期，不包含当日；里程碑和汇总任务按甘特契约解释。 |
| [`IsSummary`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L12) | `bool` | 可写; CLR / init-only | `required constructor argument` | 当前项是否为汇总任务；由树形任务计算结果维护。 |
| [`Progress`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L11) | `double` | 可写; CLR / init-only | `required constructor argument` | 完成进度数值；范围／单位由所属上传或甘特模型契约定义，不用作成功标志。 |
| [`Start`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L9) | `System.DateOnly` | 可写; CLR / init-only | `required constructor argument` | 甘特任务起始日期，含当日；范围终点采用 EndExclusive。 |
| [`Task`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L8) | `TheOne.Avalonia.Controls.GanttTask` | 可写; CLR / init-only | `required constructor argument` | 命中的原始任务对象；供选择事件与业务详情使用。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`GanttSelection.GanttSelection(GanttTask Task, DateOnly Start, DateOnly EndExclusive, double Progress, bool IsSummary)`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L7) | The calculated display values for a selected leaf, milestone or summary task. |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.GanttTask`](TheOne.Avalonia.Controls.GanttTask.md)
