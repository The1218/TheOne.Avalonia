# Dates and times

[简体中文](../zh-cn/date-time.md) · **English**

CalendarItem, CalendarButton, and CalendarDayButton belong to calendar templates; DatePickerPresenter and TimePickerPresenter belong to picker popups. They receive themes from their parent and are not created independently. Replacement templates must preserve native PART_* and binding contracts.

[Back to control documentation](README.md). Extension properties use `xmlns:one="using:TheOne.Avalonia.Controls"`.

## CalendarDatePicker

```xml
<CalendarDatePicker SelectedDate="{Binding Date, Mode=TwoWay}"
                    SelectedDateFormat="Custom"
                    CustomDateFormatString="yyyy-MM-dd"
                    PlaceholderText="yyyy-MM-dd"
                    one:CalendarInput.OpenOnClick="False" />
```

`SelectedDate` is `DateTime?`. Keyboard entry and Enter commit are supported; invalid input uses the native `DateValidationError` event. Clicking the editor opens the calendar by default. `OpenOnClick="False"` disables automatic opening while keeping the calendar button available. Use `DisplayDateStart`, `DisplayDateEnd`, and `BlackoutDates` to constrain selection.

## Date/time, month, and year input

```xml
<CalendarDatePicker one:CalendarOptions.DisplayStyle="DateTime"
                    one:CalendarOptions.Format="yyyy/MM/dd HH:mm"
                    SelectedDate="{Binding Appointment, Mode=TwoWay}" />
<CalendarDatePicker one:CalendarOptions.DisplayStyle="Month"
                    SelectedDate="{Binding ReportingMonth, Mode=TwoWay}" />
<CalendarDatePicker one:CalendarOptions.DisplayStyle="Year"
                    SelectedDate="{Binding ReportingYear, Mode=TwoWay}" />
```

| Attached property | Usage |
| --- | --- |
| `CalendarOptions.DisplayStyle` | `Date` is the default date mode; `DateTime` adds time; `Month` selects months; `Year` selects years |
| `CalendarOptions.Format` | .NET date/time format string, overriding DisplayStyle's automatic format |
| `CalendarOptions.InitialView` | `Month` date grid; `Year` month grid; `Decade` year grid; explicit settings take precedence |
| `CalendarOptions.Time` | `TimeSpan?`, synchronized with SelectedDate's time in DateTime mode; valid range `[00:00, 24:00)` |
| `CalendarOptions.WheelNavigation` | Set on Calendar to control wheel navigation through months / years / decades; enabled by the theme |

DateTime, Month, and Year default to `yyyy-MM-dd HH:mm`, `yyyy-MM`, and `yyyy`. Month / Year change display format and initial view; `SelectedDate` remains a full date, from which business code extracts year / month. DateTime adds a time editor and preserves time when changing dates. Date mode uses native formatting properties.

## Standalone Calendar

```xml
<Calendar SelectedDate="{Binding Date, Mode=TwoWay}"
          SelectionMode="SingleDate"
          one:CalendarOptions.WheelNavigation="False" />
```

Native month paging, year / decade views, ranges, blackout dates, and keyboard selection remain available. Today has an outline; selection uses the accent color. Use native `SelectionMode` and `SelectedDates` for multiple dates. The date grid has a fixed layout bound.

Date, month, and year grids reserve the same body height within each density: Small 258px, Normal 294px, Large 342px. Switching modes or opening directly in month/year selection keeps the outer height stable. Date cells stay centered at their natural size. Month/year buttons are centered at their natural label size and native minimum height, so selection no longer fills the entire grid slot. The native hit target, focus, and selection behavior remain on the button. CalendarDatePicker popups share this layout. Theme resources `One.CalendarBodyHeight.Small`, `.Normal`, and `.Large` can be overridden when customizing calendar fonts or cell dimensions. PreviewHost's `--verify-calendar-layout [output]` checks only Calendar and its CalendarDatePicker popup caller.

## DatePicker and TimePicker

```xml
<DatePicker SelectedDate="{Binding OffsetDate, Mode=TwoWay}" />
<TimePicker ClockIdentifier="24HourClock" MinuteIncrement="5"
            SelectedTime="{Binding Time, Mode=TwoWay}" />
```

DatePicker `SelectedDate` is `DateTimeOffset?`; TimePicker `SelectedTime` is `TimeSpan?`, distinct from CalendarDatePicker's `DateTime?`. Keep the native viewport-recycling DateTimePickerPanel rather than replacing its scrolling panel. Examples: [EnhancementsPreview.cs](../../../design/PreviewHost/Verification/Basic/EnhancementsPreview.cs) and [ControlGallery.axaml](../../../design/PreviewHost/Verification/Fixtures/LegacyControlGallery.axaml).

Date, month, time, and period fields are centered horizontally and vertically in every size, with the same layout for null and disabled states.

Item / selection-strip heights are 32 / 40 / 48 px, popup fonts 12 / 14 / 16 px, and maximum heights 288 / 340 / 400 px. Confirm commits; cancel preserves the old value. Arrows support hold-to-repeat.

PickerScroll.Enabled is enabled internally: wheel scrolling snaps to whole rows after 140 ms of inactivity; gesture completion aligns immediately; confirm / Enter aligns before committing. Non-looping lists remain within range. Separate clipping inside and outside the selection strip prevents fractional scrolling from showing state text on the wrong background. Native selection is unchanged; unloading stops timers and subscriptions. See [preview entry points](../../../design/PreviewHost/Program.cs) for checks and rendering.
