# GanttChart

[English](../en/TheOne.Avalonia.Controls.GanttChart.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.GanttChart.md) · [API index](README.md)

`TheOne.Avalonia.Controls.GanttChart` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A read-only all-day Gantt with native virtualized selection and one shared row viewport.

[Source / 声明](../../../TheOne.Avalonia/Controls/GanttChart.Rendering.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

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

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void GanttChart.RefreshData()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L352) | Refresh a non-notifying source. Call on the UI thread. |
| [`void GanttChart.ScrollToToday()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L630) | Center Today where the range allows |
| [`void GanttChart.FitToProject()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L639) | Fit the complete project, including collapsed descendants, to the current width. |
| [`bool GanttChart.BringTaskIntoView(string id)`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L657) | Expand ancestors and reveal the row and its date, without changing task dates. |
| [`GanttChart.GanttChart()`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L113) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler? GanttChart.SelectionChanged`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L248) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IEnumerable<GanttTask>?> GanttChart.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<GanttTask?> GanttChart.SelectedTaskProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<DateOnly> GanttChart.ViewStartProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<DateOnly> GanttChart.ViewEndProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L30) | Exclusive right boundary of the time axis. |
| [`static readonly StyledProperty<DateOnly> GanttChart.TodayProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GanttChart.DayWidthProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L37) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GanttChart.RowHeightProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GanttChart.TaskListWidthProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L44) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> GanttChart.HeaderHeightProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L47) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.GridBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L50) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.HeaderBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L53) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.WeekendBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L56) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.TaskBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L59) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.ProgressBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L62) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.SelectionBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L65) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.MutedBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L68) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> GanttChart.OnTaskBrushProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L71) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> GanttChart.TaskHeaderProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L74) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> GanttChart.OwnerHeaderProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L77) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> GanttChart.ProgressHeaderProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L80) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> GanttChart.EmptyTextProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L83) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<GanttChart, string?> GanttChart.ValidationErrorProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L86) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<GanttChart, int> GanttChart.VisibleTaskCountProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L89) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<GanttChart, GanttSelection?> GanttChart.SelectionProperty`](../../../TheOne.Avalonia/Controls/GanttChart.cs#L92) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.GanttSelection`](TheOne.Avalonia.Controls.GanttSelection.md)
- [`TheOne.Avalonia.Controls.GanttTask`](TheOne.Avalonia.Controls.GanttTask.md)
