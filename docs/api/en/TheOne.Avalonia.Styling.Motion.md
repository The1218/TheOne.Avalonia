# Motion

[English](../en/TheOne.Avalonia.Styling.Motion.md) · [简体中文](../zh-cn/TheOne.Avalonia.Styling.Motion.md) · [API index](README.md)

`TheOne.Avalonia.Styling.Motion` · Class · `Avalonia.AvaloniaObject`

Set Enabled=False on a subtree to suppress theme motion.

[Source / 声明](../../../TheOne.Avalonia/Styling/Motion.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | writable; OneWay; attached; inherits | `true` | Inherited motion switch; disabling goes to final layout/state while preserving native input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool Motion.GetEnabled(Control control)`](../../../TheOne.Avalonia/Styling/Motion.cs#L15) | Reads the attached property. Inherited motion switch; disabling goes to final layout/state while preserving native input. |
| [`static void Motion.SetEnabled(Control control, bool value)`](../../../TheOne.Avalonia/Styling/Motion.cs#L16) | Sets the attached property. Inherited motion switch; disabling goes to final layout/state while preserving native input. |
| [`Motion.Motion()`](../../../TheOne.Avalonia/Styling/Motion.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> Motion.EnabledProperty`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
