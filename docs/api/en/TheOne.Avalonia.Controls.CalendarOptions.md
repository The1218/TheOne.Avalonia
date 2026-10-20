# CalendarOptions

[English](../en/TheOne.Avalonia.Controls.CalendarOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CalendarOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CalendarOptions` · Class · `Avalonia.AvaloniaObject`

Date formatting, optional time editing and wheel navigation for native calendars.

[Source / 声明](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L20)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`DisplayStyle`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L25) | `TheOne.Avalonia.Controls.DateInputDisplayStyle` | writable; OneWay; attached | `0 / default` | `Date` is the default date mode; `DateTime` adds time; `Month` selects months; `Year` selects years Values: `Date`, `DateTime`, `Month`, `Year` |
| [`Format`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L22) | `string?` | writable; OneWay; attached | `null` | .NET date/time format string, overriding DisplayStyle's automatic format |
| [`InitialView`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L28) | `Avalonia.Controls.CalendarMode` | writable; OneWay; attached | `0 / default` | `Month` date grid; `Year` month grid; `Decade` year grid; explicit settings take precedence Values: `Month`, `Year`, `Decade` |
| [`ShowTime`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L38) | `bool` | read-only; OneWay; attached | `false` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Time`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L34) | `System.TimeSpan?` | writable; OneWay; attached | `null` | `TimeSpan?`, synchronized with SelectedDate's time in DateTime mode; valid range `[00:00, 24:00)` Validation: `value => value is null &#124;&#124; (value >= TimeSpan.Zero && value < TimeSpan.FromDays(1))` |
| [`WheelNavigation`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L31) | `bool` | writable; OneWay; attached | `false` | Set on Calendar to control wheel navigation through months / years / decades; enabled by the theme |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static string? CalendarOptions.GetFormat(CalendarDatePicker picker)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L103) | Reads the attached property. .NET date/time format string, overriding DisplayStyle's automatic format |
| [`static void CalendarOptions.SetFormat(CalendarDatePicker picker, string? value)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L108) | Sets the attached property. .NET date/time format string, overriding DisplayStyle's automatic format |
| [`static DateInputDisplayStyle CalendarOptions.GetDisplayStyle(Control control)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L113) | Reads the attached property. `Date` is the default date mode; `DateTime` adds time; `Month` selects months; `Year` selects years |
| [`static void CalendarOptions.SetDisplayStyle(Control control, DateInputDisplayStyle value)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L118) | Sets the attached property. `Date` is the default date mode; `DateTime` adds time; `Month` selects months; `Year` selects years |
| [`static CalendarMode CalendarOptions.GetInitialView(CalendarDatePicker picker)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L123) | Reads the attached property. `Month` date grid; `Year` month grid; `Decade` year grid; explicit settings take precedence |
| [`static void CalendarOptions.SetInitialView(CalendarDatePicker picker, CalendarMode value)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L128) | Sets the attached property. `Month` date grid; `Year` month grid; `Decade` year grid; explicit settings take precedence |
| [`static bool CalendarOptions.GetWheelNavigation(Calendar calendar)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L133) | Reads the attached property. Set on Calendar to control wheel navigation through months / years / decades; enabled by the theme |
| [`static void CalendarOptions.SetWheelNavigation(Calendar calendar, bool value)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L138) | Sets the attached property. Set on Calendar to control wheel navigation through months / years / decades; enabled by the theme |
| [`static TimeSpan? CalendarOptions.GetTime(Control control)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L143) | Reads the attached property. `TimeSpan?`, synchronized with SelectedDate's time in DateTime mode; valid range `[00:00, 24:00)` |
| [`static void CalendarOptions.SetTime(Control control, TimeSpan? value)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L148) | Sets the attached property. `TimeSpan?`, synchronized with SelectedDate's time in DateTime mode; valid range `[00:00, 24:00)` |
| [`static bool CalendarOptions.GetShowTime(Control control)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L153) | Reads the attached property. Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CalendarOptions.CalendarOptions()`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L20) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<string?> CalendarOptions.FormatProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<DateInputDisplayStyle> CalendarOptions.DisplayStyleProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<CalendarMode> CalendarOptions.InitialViewProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> CalendarOptions.WheelNavigationProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<TimeSpan?> CalendarOptions.TimeProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> CalendarOptions.ShowTimeProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.DateInputDisplayStyle`](TheOne.Avalonia.Controls.DateInputDisplayStyle.md)
