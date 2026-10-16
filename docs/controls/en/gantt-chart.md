# GanttChart

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

```bash
dotnet build design/PreviewHost/PreviewHost.csproj -c Release --artifacts-path .preview/gantt/artifacts
dotnet .preview/gantt/artifacts/bin/PreviewHost/release/PreviewHost.dll --verify-gantt .preview/gantt/managed
# macOS: native checks only for affected AOT/binding/metadata changes.
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 --artifacts-path .preview/gantt/native-artifacts -o .preview/gantt/native
.preview/gantt/native/NativeAotSmoke --require-aot --verify-gantt .preview/gantt/native-pixels
```

The suite covers light / dark, three densities, narrow windows, rendered task and today-line pixels, hierarchy and selection, read-only dragging, validation, reattachment, compiled bindings and distant virtualization. Run the native executable for NativeAOT validation; a managed run does not substitute for it.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section lists only Classes, properties, extension APIs/events and related models added by this project, without repeating inherited Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost visibly compares project-added styles and uses Description components for control-specific properties, API signatures and model fields. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### GanttChart

[API / model reference](../../api/en/TheOne.Avalonia.Controls.GanttChart.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`DayWidth`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L57) | `double` | writable; OneWay | `48` | DIP width of one day on the Gantt timeline; changes horizontal scale without modifying task dates. Validation: `v => double.IsFinite(v) && v >= 4 && v <= 240` |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L72) | `string` | writable; OneWay | `"No tasks"` | Message when no data can be displayed; applications override it for localization. |
| [`GridBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L61) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Plot grid-line brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`HeaderBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L62) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Gantt header background brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`HeaderHeight`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L60) | `double` | writable; OneWay | `72` | Header height in chart DIPs or report layout coordinates; do not confuse it with millimetre source fields. Validation: `Positive` |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L52) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Controls.GanttTask>?` | writable; OneWay | `null` | Bind source models rather than precreated controls; use notified collections for changes and a bounded viewport. |
| [`MutedBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L67) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Muted text/helper-mark brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`OnTaskBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L68) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Contrasting text on task bars brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`OwnerHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L70) | `string` | writable; OneWay | `"Owner"` | Header text for the Gantt owner column. |
| [`ProgressBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L65) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Gantt completed-progress area brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`ProgressHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L71) | `string` | writable; OneWay | `"Progress"` | Header text for the Gantt progress column. |
| [`RowHeight`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L58) | `double` | writable; OneWay | `44` | Row height in DIPs; combines with the viewport and rendering/recycling layout to determine visible rows. Validation: `Positive` |
| [`SelectedTask`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L53) | `TheOne.Avalonia.Controls.GanttTask?` | writable; TwoWay | `null` | Selected original Gantt task; null clears selection while preserving model identity. |
| [`Selection`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L75) | `TheOne.Avalonia.Controls.GanttSelection?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectionBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L66) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Selected-task highlight brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`TaskBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L64) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Gantt task-bar background brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`TaskHeader`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L69) | `string` | writable; OneWay | `"Task"` | Header text for the Gantt task-name column. |
| [`TaskListWidth`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L59) | `double` | writable; OneWay | `420` | DIP width of the Gantt task-information pane; the timeline uses the remaining space. Validation: `Positive` |
| [`Today`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L56) | `System.DateOnly` | writable; OneWay | `DateOnly.FromDateTime(DateTime.Today)` | Date used for the timeline's today marker; set explicitly for demos or a fixed business date. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L73) | `string?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ViewEnd`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L55) | `System.DateOnly` | writable; OneWay | `DateOnly.FromDateTime(DateTime.Today).AddDays(31)` | Exclusive right boundary of the time axis. |
| [`ViewStart`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L54) | `System.DateOnly` | writable; OneWay | `DateOnly.FromDateTime(DateTime.Today)` | Start of the Gantt date range; defines the timeline with its end without modifying tasks. |
| [`VisibleTaskCount`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L74) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`WeekendBrush`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L63) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Timeline weekend region brush; use One.* dynamic resources for themes. Explicit local values take precedence. |

### GanttHeaderPresenter

[API / model reference](../../api/en/TheOne.Avalonia.Controls.GanttHeaderPresenter.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| GanttChart | `one-large` | automatic template class; do not bind manually | ^.one-large => FontSize=16; RowHeight=52; HeaderHeight=80 |
| GanttChart | `one-small` | automatic template class; do not bind manually | ^.one-small => FontSize=12; RowHeight=36; HeaderHeight=64 |

`:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without added appearance classes use their default theme and control-specific properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.GanttChart`](../../api/en/TheOne.Avalonia.Controls.GanttChart.md) | Class | 24 | 5 | 1 |
| [`TheOne.Avalonia.Controls.GanttHeaderPresenter`](../../api/en/TheOne.Avalonia.Controls.GanttHeaderPresenter.md) | Class | 0 | 2 | 0 |
| [`TheOne.Avalonia.Controls.GanttSelection`](../../api/en/TheOne.Avalonia.Controls.GanttSelection.md) | Class | 6 | 1 | 0 |
| [`TheOne.Avalonia.Controls.GanttTask`](../../api/en/TheOne.Avalonia.Controls.GanttTask.md) | Class | 9 | 1 | 1 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
