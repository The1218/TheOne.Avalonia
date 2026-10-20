# CalendarInput

[English](../en/TheOne.Avalonia.Controls.CalendarInput.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CalendarInput.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CalendarInput` · Class · `Avalonia.AvaloniaObject`

Click-to-open calendar with native date parsing and an Escape path back to the editor.

[Source / 声明](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L12)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`OpenOnClick`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L14) | `bool` | writable; OneWay; attached | `false` | Opens the native date popup when clicking the input, retaining text and keyboard behavior. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool CalendarInput.GetOpenOnClick(CalendarDatePicker picker)`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L43) | Reads the attached property. Opens the native date popup when clicking the input, retaining text and keyboard behavior. |
| [`static void CalendarInput.SetOpenOnClick(CalendarDatePicker picker, bool value)`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L48) | Sets the attached property. Opens the native date popup when clicking the input, retaining text and keyboard behavior. |
| [`CalendarInput.CalendarInput()`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L12) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> CalendarInput.OpenOnClickProperty`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
