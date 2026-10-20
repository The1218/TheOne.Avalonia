# GanttChart

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

**English** · [简体中文](../zh-cn/gantt-chart.md)

`GanttChart` displays read-only all-day schedules. A native ListBox provides selection, keyboard navigation and row virtualization. Each visible row draws both task information and its timeline, so vertical scrolling cannot separate them. The horizontal ScrollBar moves only the timeline; the header stays fixed.

## Compiled binding example

Load OneTheme and declare the actual ViewModel as the view's `x:DataType`:

```xml
<one:GanttChart Height="520"
                ItemsSource="{Binding Tasks}"
                SelectedTask="{Binding SelectedTask, Mode=TwoWay}"
                ViewStart="{Binding ViewStart, Mode=TwoWay}"
                ViewEnd="{Binding ViewEnd, Mode=TwoWay}"
                DayWidth="{Binding DayWidth, Mode=TwoWay}"
                TaskHeader="Task" OwnerHeader="Owner" ProgressHeader="Progress" />
```

Use `ObservableCollection<GanttTask>` for Tasks. The model implements INotifyPropertyChanged; collection and model notifications refresh the chart. Call `RefreshData()` after editing a plain List. Change models and collections, and call control methods, on the UI thread. Give the chart a bounded height; avoid an outer ScrollViewer with infinite height.

```csharp
public ObservableCollection<GanttTask> Tasks { get; } =
[
    new() { Id = "project", Title = "Control development", Owner = "Engineering" },
    new() { Id = "drawing", ParentId = "project", Title = "Timeline rendering", Owner = "Zhou",
            Start = new(2026, 10, 12), EndExclusive = new(2026, 10, 17), Progress = .6 },
    new() { Id = "acceptance", ParentId = "project", Title = "Acceptance",
            Start = new(2026, 10, 23), IsMilestone = true }
];
```

## Incremental task updates

Public properties and APIs are unchanged; no new method calls are needed:

```csharp
task.Title = "Interaction design complete";
task.Owner = "Zhou";
task.Progress = .8;
task.EndExclusive = new DateOnly(2026, 10, 20);
// Notifications in one UI turn are coalesced; row/container identity, focus and scroll position remain.
```

- `Title` / `Owner` refresh only the task's realized row, tooltip and automation name. They do not enumerate the source or rebuild the task tree or subscriptions.
- `Progress` / `Start` / `EndExclusive` / `IsMilestone` update cached leaf values and affected ancestors' dates and duration-weighted progress. Ordinary progress updates follow the parent chain. When an existing date extremum moves inward, the corresponding parent's direct children are checked again, rather than scanning the whole tree. Summary model dates and progress remain ignored.
- `IsExpanded` inserts or removes the affected descendants in the visible-row projection, retaining other row identities and subscriptions. A hidden selected descendant moves to its closest visible ancestor. Renumbering costs grow with the following visible rows.
- Collection changes, `Id` / `ParentId`, empty or unknown property notifications, and explicit `RefreshData()` still validate and rebuild the complete task tree. Subscriptions are reconciled only for actual source/model membership changes; unchanged models are not resubscribed.
- Invalid data still hides the complete chart; notifying corrections recover through full validation. Detaching releases subscriptions, and replacing the source supersedes pending old-task edits. Unnotified changes to plain collections still require `RefreshData()`.

The incremental path adds a task-reference index and completed-duration caches, both proportional to task count. This optimizes updates after loading, not initial load or structural rebuilds. Notifications must still be raised on the UI thread.

### Performance record and reproduction

Current-source measurement on 2026-10-18: macOS 27.0.1 ARM64, .NET 10.0.0, Release, Skia Headless, fixed 1100 × 520 viewport, motion disabled, eight warm-up updates and 32 measured updates per scenario. Baseline control source: `e083204`. This table uses 10,000 tasks in groups of 100. Timings cover the UI-thread update, dispatcher processing and layout, not GPU/display latency. Allocation additionally includes the subsequent headless frame processing, on the current thread only.

| Edit | Median before → after | P95 before → after | Median allocation per update before → after |
| --- | --- | --- | --- |
| Title | 7.593 → 0.383 ms | 13.413 → 0.443 ms | 6,431,560 → 54,704 B |
| Owner | 6.374 → 0.422 ms | 12.683 → 0.917 ms | 6,467,632 → 58,296 B |
| Progress | 6.426 → 0.741 ms | 14.763 → 1.319 ms | 6,406,280 → 118,248 B |
| Date | 6.077 → 0.710 ms | 12.914 → 1.317 ms | 6,404,752 → 118,648 B |

Wide trees and 1,000-task sources are also measured. Across all 16 scenarios, each 32-update baseline run enumerates/resubscribes the source and resets rows 32 times; all corresponding counters are zero after optimization and the selected row retains its identity. The date scenario alternates outward and inward boundaries, including sibling rechecks. Those rechecks can still grow with the parent's direct-child count in wide trees. Tests do not impose hardware-dependent timing pass thresholds.

## Data contract

- Id must be non-empty and unique. ParentId = null denotes a root. Parents must exist in the same source; cycles are invalid. Siblings follow source order.
- Leaf tasks use DateOnly intervals `[Start, EndExclusive)`: the example displays October 12–16. Progress must be a finite fraction between 0 and 1.
- Milestones use Start and ignore EndExclusive. They cannot have children.
- Nodes with children are summaries. Their dates cover all descendants; progress is weighted by leaf duration in calendar days. Milestones contribute no progress weight. A summary's own dates and progress are ignored.
- IsExpanded defaults to true and lives in the model. Collapsing does not change dates. A hidden selected descendant moves selection to its nearest visible ancestor.
- Invalid sources are not displayed. ValidationError explains the problem; notifying corrections recover automatically. VisibleTaskCount counts rows after expansion.

## Viewport, selection and input

ViewStart / ViewEnd use the same exclusive end convention; ViewEnd must follow ViewStart. DayWidth is pixels per civil calendar day, between 4 and 240. Zooming out reduces labels to weeks or months without changing the equal-day date mapping. Weekend shading does not compress time.

SelectedTask supports two-way binding. Read-only Selection exposes the calculated Start, EndExclusive, DisplayEnd, Progress, IsSummary and Task for detail panels. Read Selection for summary dates rather than the model's own fields. SelectionChanged fires when selection or its calculated values change.

| Input / method | Behavior |
| --- | --- |
| Row click, up / down | Native ListBox selection and reveal |
| Expansion button, left / right | Collapse / expand, or move to parent / first child |
| Wheel | Native vertical scrolling of the shared rows |
| Shift + wheel, horizontal wheel | Move only the timeline |
| Ctrl / Cmd + wheel | Zoom around the pointer |
| `ScrollToToday()` | Center Today where the range allows |
| `FitToProject()` | Fit the complete project, including collapsed descendants |
| `BringTaskIntoView(id)` | Expand ancestors, select and reveal a task; false if absent |

This version is always read-only. It has no IsReadOnly switch, editing, dependency lines, working calendar, automatic scheduling or export API. Consumers compose search, owner filters and toolbars; filtered sources must retain ancestors of matching tasks.

## Theme and density

Small / Normal / Large RowHeight defaults are 36 / 44 / 52; HeaderHeight defaults are 64 / 72 / 80. TaskListWidth defaults to 420. Narrow windows reduce the task area and hide owner and progress columns, while preserving full-task tooltips and horizontal timeline scrolling.

Dynamic `One.Gantt.Background`, `Text`, `Grid`, `Header`, `Weekend`, `Task`, `Progress`, `Selection`, `Muted` and `OnTask` brushes inherit One's light / dark semantic colors. Corresponding brush properties can be overridden individually.

The template retains PART_Header (GanttHeaderPresenter), PART_Rows (ListBox), PART_HorizontalScrollBar (ScrollBar) and PART_Empty (TextBlock). Row containers require zero Padding, stretched content and VirtualizingStackPanel. Bars, grid, today line and text are drawn directly; expansion uses native Buttons. Rows and buttons supply automation names.

## Verification

PreviewHost's Data display / GanttChart page includes the design sample, filtering, zoom, expand / collapse all, 1,500 tasks and last-row reveal.

The suite covers light / dark, three densities, narrow windows, rendered task and today-line pixels, hierarchy and selection, read-only dragging, validation, reattachment, compiled bindings and distant virtualization. Run the native executable for NativeAOT validation; a managed run does not substitute for it.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### GanttChart

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.GanttChart.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`DayWidth`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L149) | `double` | writable; OneWay | `48` | DIP width of one day on the Gantt timeline; changes horizontal scale without modifying task dates. Validation: `v => double.IsFinite(v) && v >= 4 && v <= 240` |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L239) | `string` | writable; OneWay | `"No tasks"` | Message when no data can be displayed; applications override it for localization. |
| [`GridBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L173) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Plot grid-line brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`HeaderBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L179) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Gantt header background brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`HeaderHeight`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L167) | `double` | writable; OneWay | `72` | Header height in chart DIPs or report layout coordinates; do not confuse it with millimetre source fields. Validation: `Positive` |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L119) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Controls.GanttTask>?` | writable; OneWay | `null` | Bind source models rather than precreated controls; use notified collections for changes and a bounded viewport. |
| [`MutedBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L209) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Muted text/helper-mark brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`OnTaskBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L215) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Contrasting text on task bars brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`OwnerHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L227) | `string` | writable; OneWay | `"Owner"` | Header text for the Gantt owner column. |
| [`ProgressBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L197) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Gantt completed-progress area brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`ProgressHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L233) | `string` | writable; OneWay | `"Progress"` | Header text for the Gantt progress column. |
| [`RowHeight`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L155) | `double` | writable; OneWay | `44` | Row height in DIPs; combines with the viewport and rendering/recycling layout to determine visible rows. Validation: `Positive` |
| [`SelectedTask`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L125) | `TheOne.Avalonia.Controls.GanttTask?` | writable; TwoWay | `null` | Selected original Gantt task; null clears selection while preserving model identity. |
| [`Selection`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L247) | `TheOne.Avalonia.Controls.GanttSelection?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectionBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L203) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Selected-task highlight brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`TaskBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L191) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Gantt task-bar background brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`TaskHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L221) | `string` | writable; OneWay | `"Task"` | Header text for the Gantt task-name column. |
| [`TaskListWidth`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L161) | `double` | writable; OneWay | `420` | DIP width of the Gantt task-information pane; the timeline uses the remaining space. Validation: `Positive` |
| [`Today`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L143) | `System.DateOnly` | writable; OneWay | `DateOnly.FromDateTime(DateTime.Today)` | Date used for the timeline's today marker; set explicitly for demos or a fixed business date. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L245) | `string?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ViewEnd`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L137) | `System.DateOnly` | writable; OneWay | `DateOnly.FromDateTime(DateTime.Today).AddDays(31)` | Exclusive right boundary of the time axis. |
| [`ViewStart`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L131) | `System.DateOnly` | writable; OneWay | `DateOnly.FromDateTime(DateTime.Today)` | Start of the Gantt date range; defines the timeline with its end without modifying tasks. |
| [`VisibleTaskCount`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L246) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`WeekendBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L185) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Timeline weekend region brush; use One.* dynamic resources for themes. Explicit local values take precedence. |

### GanttHeaderPresenter

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.GanttHeaderPresenter.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`GanttChart`](../../api/en/TheOne.Avalonia.Controls.GanttChart.md) | `void GanttChart.RefreshData()` | Refresh a non-notifying source. Call on the UI thread. |
| [`GanttChart`](../../api/en/TheOne.Avalonia.Controls.GanttChart.md) | `void GanttChart.ScrollToToday()` | Center Today where the range allows |
| [`GanttChart`](../../api/en/TheOne.Avalonia.Controls.GanttChart.md) | `void GanttChart.FitToProject()` | Fit the complete project, including collapsed descendants, to the current width. |
| [`GanttChart`](../../api/en/TheOne.Avalonia.Controls.GanttChart.md) | `bool GanttChart.BringTaskIntoView(string id)` | Expand ancestors and reveal the row and its date, without changing task dates. |

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`GanttChart`](../../api/en/TheOne.Avalonia.Controls.GanttChart.md) | `SelectionChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |
| [`GanttTask`](../../api/en/TheOne.Avalonia.Controls.GanttTask.md) | `PropertyChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.GanttSelection`](../../api/en/TheOne.Avalonia.Controls.GanttSelection.md) | 6 |
| [`TheOne.Avalonia.Controls.GanttTask`](../../api/en/TheOne.Avalonia.Controls.GanttTask.md) | 9 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
