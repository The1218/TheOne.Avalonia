# GanttChart

[English](../en/TheOne.Avalonia.Controls.GanttChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GanttChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GanttChart` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A read-only all-day Gantt with native virtualized selection and one shared row viewport.

[Source / 声明](../../../TheOne.Avalonia/Controls/GanttChart.Rendering.cs#L11)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

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

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void GanttChart.RefreshData()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L151) | Refresh a non-notifying source. Call on the UI thread. |
| [`void GanttChart.ScrollToToday()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L354) | Center Today where the range allows |
| [`void GanttChart.FitToProject()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L359) | Fit the complete project, including collapsed descendants, to the current width. |
| [`bool GanttChart.BringTaskIntoView(string id)`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L373) | Expand ancestors and reveal the row and its date, without changing task dates. |
| [`GanttChart.GanttChart()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L21) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler? GanttChart.SelectionChanged`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L76) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IEnumerable<GanttTask>?> GanttChart.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<GanttTask?> GanttChart.SelectedTaskProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<DateOnly> GanttChart.ViewStartProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<DateOnly> GanttChart.ViewEndProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L30) | Exclusive right boundary of the time axis. |
| [`static readonly StyledProperty<DateOnly> GanttChart.TodayProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GanttChart.DayWidthProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GanttChart.RowHeightProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GanttChart.TaskListWidthProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GanttChart.HeaderHeightProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.GridBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.HeaderBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L37) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.WeekendBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.TaskBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L39) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.ProgressBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L40) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.SelectionBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.MutedBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L42) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.OnTaskBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L43) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> GanttChart.TaskHeaderProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L44) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> GanttChart.OwnerHeaderProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L45) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> GanttChart.ProgressHeaderProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L46) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> GanttChart.EmptyTextProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L47) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<GanttChart, string?> GanttChart.ValidationErrorProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L48) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<GanttChart, int> GanttChart.VisibleTaskCountProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L49) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<GanttChart, GanttSelection?> GanttChart.SelectionProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L50) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.GanttSelection`](TheOne.Avalonia.Controls.GanttSelection.md)
- [`TheOne.Avalonia.Controls.GanttTask`](TheOne.Avalonia.Controls.GanttTask.md)
