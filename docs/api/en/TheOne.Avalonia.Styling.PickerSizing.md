# PickerSizing

[English](../en/TheOne.Avalonia.Styling.PickerSizing.md) · [简体中文](../zh-cn/TheOne.Avalonia.Styling.PickerSizing.md) · [API index](README.md)

`TheOne.Avalonia.Styling.PickerSizing` · Class · `Avalonia.AvaloniaObject`

Shared row metrics keep the native picker viewport and selection band aligned.

[Source / 声明](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ItemHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L9) | `double` | writable; OneWay; attached; inherits | `40` | Item/wheel-cell height in DIPs; theme density or explicit values may override registration defaults. |
| [`PopupHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L10) | `double` | writable; OneWay; attached; inherits | `340` | Picker-popup height in DIPs, coordinated with ItemHeight and ViewportHeight. |
| [`ViewportHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L11) | `double` | writable; OneWay; attached; inherits | `200` | Visible picker-wheel height in DIPs, distinct from item count. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static double PickerSizing.GetViewportHeight(Control control)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L12) | Reads the attached property. Visible picker-wheel height in DIPs, distinct from item count. |
| [`static void PickerSizing.SetViewportHeight(Control control, double value)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L13) | Sets the attached property. Visible picker-wheel height in DIPs, distinct from item count. |
| [`static double PickerSizing.GetItemHeight(Control control)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L14) | Reads the attached property. Item/wheel-cell height in DIPs; theme density or explicit values may override registration defaults. |
| [`static void PickerSizing.SetItemHeight(Control control, double value)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L15) | Sets the attached property. Item/wheel-cell height in DIPs; theme density or explicit values may override registration defaults. |
| [`static double PickerSizing.GetPopupHeight(Control control)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L16) | Reads the attached property. Picker-popup height in DIPs, coordinated with ItemHeight and ViewportHeight. |
| [`static void PickerSizing.SetPopupHeight(Control control, double value)`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L17) | Sets the attached property. Picker-popup height in DIPs, coordinated with ItemHeight and ViewportHeight. |
| [`PickerSizing.PickerSizing()`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<double> PickerSizing.ItemHeightProperty`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<double> PickerSizing.PopupHeightProperty`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L10) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<double> PickerSizing.ViewportHeightProperty`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
