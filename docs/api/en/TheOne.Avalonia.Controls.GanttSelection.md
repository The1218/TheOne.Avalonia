# GanttSelection

[English](../en/TheOne.Avalonia.Controls.GanttSelection.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GanttSelection.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GanttSelection` · Class · `object`

The calculated display values for a selected leaf, milestone or summary task.

[Source / 声明](../../../TheOne.Avalonia/Controls/GanttTask.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`DisplayEnd`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L14) | `System.DateOnly` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`EndExclusive`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L10) | `System.DateOnly` | writable; CLR / init-only | `required constructor argument` | Exclusive Gantt task end date; milestone/summary behavior follows the Gantt contract. |
| [`IsSummary`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L12) | `bool` | writable; CLR / init-only | `required constructor argument` | Whether the item is a summary task, maintained by hierarchical task calculation. |
| [`Progress`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L11) | `double` | writable; CLR / init-only | `required constructor argument` | Progress value with range/unit defined by its upload/Gantt model, distinct from a success flag. |
| [`Start`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L9) | `System.DateOnly` | writable; CLR / init-only | `required constructor argument` | Inclusive Gantt task start date, paired with EndExclusive. |
| [`Task`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L8) | `TheOne.Avalonia.Controls.GanttTask` | writable; CLR / init-only | `required constructor argument` | Original hit-tested task object for selection events/business details. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`GanttSelection.GanttSelection(GanttTask Task, DateOnly Start, DateOnly EndExclusive, double Progress, bool IsSummary)`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L7) | The calculated display values for a selected leaf, milestone or summary task. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.GanttTask`](TheOne.Avalonia.Controls.GanttTask.md)
