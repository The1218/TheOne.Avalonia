# GanttTask

[English](../en/TheOne.Avalonia.Controls.GanttTask.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GanttTask.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GanttTask` · Class · `object`

An all-day task. Non-milestone leaf tasks use [Start, EndExclusive).

[Source / 声明](../../../TheOne.Avalonia/Controls/GanttTask.cs#L18)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`EndExclusive`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L50) | `System.DateOnly` | writable; CLR / n/a | `DateOnly.FromDateTime(DateTime.Today).AddDays(1)` | Exclusive Gantt task end date; milestone/summary behavior follows the Gantt contract. |
| [`Id`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L20) | `string` | writable; CLR / n/a | `Guid.NewGuid().ToString("N")` | Stable model identifier for persistence/references; avoid duplicates in one document. |
| [`IsExpanded`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L69) | `bool` | writable; CLR / n/a | `true` | Current node expansion; bind to a stable model when it must survive recycling or reload. |
| [`IsMilestone`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L63) | `bool` | writable; CLR / n/a | `false` | Displays the task as a milestone rather than an ordinary duration bar. |
| [`Owner`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L38) | `string?` | writable; CLR / n/a | `null` | Task-owner text, distinct from popup/window ownership. |
| [`ParentId`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L26) | `string?` | writable; CLR / n/a | `null` | Stable parent-task identifier; null denotes a root, and cycles are invalid. |
| [`Progress`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L57) | `double` | writable; CLR / n/a | `0 / default` | Completion fraction in [0, 1]. Summary values are computed from descendants. |
| [`Start`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L44) | `System.DateOnly` | writable; CLR / n/a | `DateOnly.FromDateTime(DateTime.Today)` | Inclusive Gantt task start date, paired with EndExclusive. |
| [`Title`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L32) | `string` | writable; CLR / n/a | `""` | Display title, distinct from business identity or a localization key. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`GanttTask.GanttTask()`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L18) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event PropertyChangedEventHandler? GanttTask.PropertyChanged`](../../../TheOne.Avalonia/Controls/GanttTask.cs#L75) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
