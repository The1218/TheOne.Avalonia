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
dotnet build TheOne.Avalonia.slnx
dotnet design/PreviewHost/bin/Debug/net10.0/PreviewHost.dll --verify-gantt .preview/gantt/managed
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 -o .preview/gantt/native
.preview/gantt/native/NativeAotSmoke --require-aot --verify-gantt .preview/gantt/native-pixels
```

The suite covers light / dark, three densities, narrow windows, rendered task and today-line pixels, hierarchy and selection, read-only dragging, validation, reattachment, compiled bindings and distant virtualization. Run the native executable for NativeAOT validation; a managed run does not substitute for it.
