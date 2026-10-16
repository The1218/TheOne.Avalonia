# RefreshMotion

[English](../en/TheOne.Avalonia.Controls.RefreshMotion.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RefreshMotion.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RefreshMotion` · Class · `Avalonia.AvaloniaObject`

Applies Motion.Enabled to the native refresh compositor without changing refresh state or input.

[Source / 声明](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L20)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`UseMotionSettings`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L22) | `bool` | writable; OneWay; attached | `false` | Uses shared motion configuration without forcing animation when globally/locally disabled. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool RefreshMotion.GetUseMotionSettings(RefreshVisualizer control)`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L30) | Reads the attached property. Uses shared motion configuration without forcing animation when globally/locally disabled. |
| [`static void RefreshMotion.SetUseMotionSettings(RefreshVisualizer control, bool value)`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L31) | Sets the attached property. Uses shared motion configuration without forcing animation when globally/locally disabled. |
| [`RefreshMotion.RefreshMotion()`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L20) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> RefreshMotion.UseMotionSettingsProperty`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
