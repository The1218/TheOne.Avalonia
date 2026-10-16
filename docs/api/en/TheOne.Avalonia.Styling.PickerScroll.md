# PickerScroll

[English](../en/TheOne.Avalonia.Styling.PickerScroll.md) · [简体中文](../zh-cn/TheOne.Avalonia.Styling.PickerScroll.md) · [API index](README.md)

`TheOne.Avalonia.Styling.PickerScroll` · Class · `Avalonia.AvaloniaObject`

Finishes fractional wheel offsets without replacing native picker selection or recycling.

[Source / 声明](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L15)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L17) | `bool` | writable; OneWay; attached | `false` | Interaction switch for picker wheels, retaining native commit/cancel/focus behavior. |
| [`HighlightClip`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L18) | `Avalonia.Media.Geometry` | writable; OneWay; attached | `new RectangleGeometry(new Rect())` | Clipping configuration for the wheel highlight, normally maintained by picker templates. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool PickerScroll.GetEnabled(DateTimePickerPanel panel)`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L21) | Reads the attached property. Interaction switch for picker wheels, retaining native commit/cancel/focus behavior. |
| [`static void PickerScroll.SetEnabled(DateTimePickerPanel panel, bool value)`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L22) | Sets the attached property. Interaction switch for picker wheels, retaining native commit/cancel/focus behavior. |
| [`static Geometry PickerScroll.GetHighlightClip(ListBoxItem item)`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L23) | Reads the attached property. Clipping configuration for the wheel highlight, normally maintained by picker templates. |
| [`static void PickerScroll.SetHighlightClip(ListBoxItem item, Geometry value)`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L24) | Sets the attached property. Clipping configuration for the wheel highlight, normally maintained by picker templates. |
| [`PickerScroll.PickerScroll()`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L15) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> PickerScroll.EnabledProperty`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<Geometry> PickerScroll.HighlightClipProperty`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
