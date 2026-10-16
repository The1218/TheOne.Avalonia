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

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost keeps complete demos on their own pages, with styles, additional properties, APIs/events and models on each page. Only Button, Input and Select share comparison pages.

### CalendarInput

[API / model reference](../../api/en/TheOne.Avalonia.Controls.CalendarInput.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`OpenOnClick`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L14) | `bool` | writable; OneWay; attached | `false` | Opens the native date popup when clicking the input, retaining text and keyboard behavior. |

### CalendarOptions

[API / model reference](../../api/en/TheOne.Avalonia.Controls.CalendarOptions.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`DisplayStyle`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L17) | `TheOne.Avalonia.Controls.DateInputDisplayStyle` | writable; OneWay; attached | `0 / default` | `Date` is the default date mode; `DateTime` adds time; `Month` selects months; `Year` selects years Values: `Date`, `DateTime`, `Month`, `Year` |
| [`Format`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L16) | `string?` | writable; OneWay; attached | `null` | .NET date/time format string, overriding DisplayStyle's automatic format |
| [`InitialView`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L18) | `Avalonia.Controls.CalendarMode` | writable; OneWay; attached | `CalendarMode.Month` | `Month` date grid; `Year` month grid; `Decade` year grid; explicit settings take precedence Values: `Month`, `Year`, `Decade` |
| [`ShowTime`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L21) | `bool` | read-only; OneWay; attached | `false` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Time`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L20) | `System.TimeSpan?` | writable; OneWay; attached | `null` | `TimeSpan?`, synchronized with SelectedDate's time in DateTime mode; valid range `[00:00, 24:00)` Validation: `value => value is null &#124;&#124; value >= TimeSpan.Zero && value < TimeSpan.FromDays(1)` |
| [`WheelNavigation`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L19) | `bool` | writable; OneWay; attached | `false` | Set on Calendar to control wheel navigation through months / years / decades; enabled by the theme |

### PickerScroll

[API / model reference](../../api/en/TheOne.Avalonia.Styling.PickerScroll.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L17) | `bool` | writable; OneWay; attached | `false` | Interaction switch for picker wheels, retaining native commit/cancel/focus behavior. |
| [`HighlightClip`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L18) | `Avalonia.Media.Geometry` | writable; OneWay; attached | `new RectangleGeometry(new Rect())` | Clipping configuration for the wheel highlight, normally maintained by picker templates. |

### PickerSizing

[API / model reference](../../api/en/TheOne.Avalonia.Styling.PickerSizing.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ItemHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L9) | `double` | writable; OneWay; attached; inherits | `40` | Item/wheel-cell height in DIPs; theme density or explicit values may override registration defaults. |
| [`PopupHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L10) | `double` | writable; OneWay; attached; inherits | `340` | Picker-popup height in DIPs, coordinated with ItemHeight and ViewportHeight. |
| [`ViewportHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L11) | `double` | writable; OneWay; attached; inherits | `200` | Visible picker-wheel height in DIPs, distinct from item count. |

### Shared attached properties

[ControlSizing](../../api/en/TheOne.Avalonia.Styling.ControlSizing.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | writable; OneWay; attached; inherits | `ControlSize.Normal` | Shared Small/Normal/Large density inherited by descendants; explicit local values/classes can override it. Values: `Small`, `Normal`, `Large` |

[Motion](../../api/en/TheOne.Avalonia.Styling.Motion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | writable; OneWay; attached; inherits | `true` | Inherited motion switch; disabling goes to final layout/state while preserving native input. |

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| Calendar | `one-large` | automatic template class; do not bind manually | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| Calendar | `one-normal` | automatic template class; do not bind manually | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| Calendar | `one-small` | automatic template class; do not bind manually | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |
| CalendarDatePicker | `filled` | explicit Classes; combine with density | TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled => Background={DynamicResource One.Subtle} |
| CalendarDatePicker | `one-large` | automatic template class; do not bind manually | :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, ButtonSpinner.one-large, CheckBox.one-large, RadioButton.one-large, ToggleSwitch.one-large, ListBoxItem.one-large, TreeViewItem.one-large, MenuItem.one-large, TabItem.one-large, CalendarDatePicker.one-large, DatePicker.one-large, TimePicker.one-large, Expander.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16; :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, CalendarDatePicker.one-large, ListBoxItem.one-large, MenuItem.one-large => Padding=16,10; CalendarDatePicker.one-large /template/ Button#PART_Button => Width=44 |
| CalendarDatePicker | `one-normal` | automatic template class; do not bind manually | :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, ButtonSpinner.one-normal, CheckBox.one-normal, RadioButton.one-normal, ToggleSwitch.one-normal, ListBoxItem.one-normal, TreeViewItem.one-normal, MenuItem.one-normal, TabItem.one-normal, CalendarDatePicker.one-normal, DatePicker.one-normal, TimePicker.one-normal, Expander.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14; :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, CalendarDatePicker.one-normal, ListBoxItem.one-normal, MenuItem.one-normal => Padding=12,7; CalendarDatePicker.one-normal /template/ Button#PART_Button => Width=36 |
| CalendarDatePicker | `one-small` | automatic template class; do not bind manually | :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, ButtonSpinner.one-small, CheckBox.one-small, RadioButton.one-small, ToggleSwitch.one-small, ListBoxItem.one-small, TreeViewItem.one-small, MenuItem.one-small, TabItem.one-small, CalendarDatePicker.one-small, DatePicker.one-small, TimePicker.one-small, Expander.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12; :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, CalendarDatePicker.one-small, ListBoxItem.one-small, MenuItem.one-small => Padding=10,4; CalendarDatePicker.one-small /template/ Button#PART_Button => Width=30 |
| DatePicker | `filled` | explicit Classes; combine with density | TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled => Background={DynamicResource One.Subtle} |
| DatePicker | `one-large` | automatic template class; do not bind manually | :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, ButtonSpinner.one-large, CheckBox.one-large, RadioButton.one-large, ToggleSwitch.one-large, ListBoxItem.one-large, TreeViewItem.one-large, MenuItem.one-large, TabItem.one-large, CalendarDatePicker.one-large, DatePicker.one-large, TimePicker.one-large, Expander.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16 |
| DatePicker | `one-normal` | automatic template class; do not bind manually | :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, ButtonSpinner.one-normal, CheckBox.one-normal, RadioButton.one-normal, ToggleSwitch.one-normal, ListBoxItem.one-normal, TreeViewItem.one-normal, MenuItem.one-normal, TabItem.one-normal, CalendarDatePicker.one-normal, DatePicker.one-normal, TimePicker.one-normal, Expander.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14 |
| DatePicker | `one-small` | automatic template class; do not bind manually | :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, ButtonSpinner.one-small, CheckBox.one-small, RadioButton.one-small, ToggleSwitch.one-small, ListBoxItem.one-small, TreeViewItem.one-small, MenuItem.one-small, TabItem.one-small, CalendarDatePicker.one-small, DatePicker.one-small, TimePicker.one-small, Expander.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12 |
| TimePicker | `filled` | explicit Classes; combine with density | TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled => Background={DynamicResource One.Subtle} |
| TimePicker | `one-large` | automatic template class; do not bind manually | :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, ButtonSpinner.one-large, CheckBox.one-large, RadioButton.one-large, ToggleSwitch.one-large, ListBoxItem.one-large, TreeViewItem.one-large, MenuItem.one-large, TabItem.one-large, CalendarDatePicker.one-large, DatePicker.one-large, TimePicker.one-large, Expander.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16 |
| TimePicker | `one-normal` | automatic template class; do not bind manually | :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, ButtonSpinner.one-normal, CheckBox.one-normal, RadioButton.one-normal, ToggleSwitch.one-normal, ListBoxItem.one-normal, TreeViewItem.one-normal, MenuItem.one-normal, TabItem.one-normal, CalendarDatePicker.one-normal, DatePicker.one-normal, TimePicker.one-normal, Expander.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14 |
| TimePicker | `one-small` | automatic template class; do not bind manually | :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, ButtonSpinner.one-small, CheckBox.one-small, RadioButton.one-small, ToggleSwitch.one-small, ListBoxItem.one-small, TreeViewItem.one-small, MenuItem.one-small, TabItem.one-small, CalendarDatePicker.one-small, DatePicker.one-small, TimePicker.one-small, Expander.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.CalendarInput`](../../api/en/TheOne.Avalonia.Controls.CalendarInput.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.CalendarOptions`](../../api/en/TheOne.Avalonia.Controls.CalendarOptions.md) | Class | 6 | 12 | 0 |
| [`TheOne.Avalonia.Controls.DateInputDisplayStyle`](../../api/en/TheOne.Avalonia.Controls.DateInputDisplayStyle.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Styling.PickerScroll`](../../api/en/TheOne.Avalonia.Styling.PickerScroll.md) | Class | 2 | 5 | 0 |
| [`TheOne.Avalonia.Styling.PickerSizing`](../../api/en/TheOne.Avalonia.Styling.PickerSizing.md) | Class | 3 | 7 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
