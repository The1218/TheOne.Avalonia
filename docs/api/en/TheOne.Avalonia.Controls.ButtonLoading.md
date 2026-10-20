# ButtonLoading

[English](../en/TheOne.Avalonia.Controls.ButtonLoading.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ButtonLoading.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ButtonLoading` · Class · `Avalonia.AvaloniaObject`

Loading state for official Button controls; the temporary override preserves IsEnabled bindings.

[Source / 声明](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L12) | `bool` | writable; OneWay; attached | `false` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool ButtonLoading.GetIsLoading(Button button)`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L23) | Reads the attached property. Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |
| [`static void ButtonLoading.SetIsLoading(Button button, bool value)`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L28) | Sets the attached property. Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |
| [`ButtonLoading.ButtonLoading()`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L10) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> ButtonLoading.IsLoadingProperty`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
