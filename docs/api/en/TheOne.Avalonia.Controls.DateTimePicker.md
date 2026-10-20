# DateTimePicker

[English](../en/TheOne.Avalonia.Controls.DateTimePicker.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DateTimePicker.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DateTimePicker` · Class · `Avalonia.Controls.CalendarDatePicker`

A native calendar input with an integrated 24-hour time editor.

[Source / 声明](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L11)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Format`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L71) | `string?` | writable; OneWay | `null` | `string?` / `null` ; .NET date/time format; null restores the default. Invalid formats retain native exception behavior. |
| [`Time`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L77) | `System.TimeSpan?` | writable; TwoWay | `inherited registration` | `TimeSpan?` / `null`, TwoWay by default ; Synchronized time of day, restricted to `[00:00,24:00)`. Invalid values are rejected. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`DateTimePicker.DateTimePicker()`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L25) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string?> DateTimePicker.FormatProperty`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L17) | The display format; null uses yyyy-MM-dd HH:mm. |
| [`static readonly StyledProperty<TimeSpan?> DateTimePicker.TimeProperty`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L21) | The time of day, synchronized with SelectedDate without replacing bindings. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `filled` | [`TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled`](../../../TheOne.Avalonia/Themes/Variants.axaml#L86) | TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled => Background={DynamicResource One.Subtle} |
