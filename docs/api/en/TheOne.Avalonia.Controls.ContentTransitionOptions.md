# ContentTransitionOptions

[English](../en/TheOne.Avalonia.Controls.ContentTransitionOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ContentTransitionOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ContentTransitionOptions` · Class · `Avalonia.AvaloniaObject`

Connects native content transitions, including consumer-provided transitions, to Motion.Enabled.

[Source / 声明](../../../TheOne.Avalonia/Controls/ContentTransitionOptions.cs#L12)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`UseMotionSettings`](../../../TheOne.Avalonia/Controls/ContentTransitionOptions.cs#L14) | `bool` | writable; OneWay; attached | `false` | Uses shared motion configuration without forcing animation when globally/locally disabled. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool ContentTransitionOptions.GetUseMotionSettings(TransitioningContentControl control)`](../../../TheOne.Avalonia/Controls/ContentTransitionOptions.cs#L26) | Reads the attached property. Uses shared motion configuration without forcing animation when globally/locally disabled. |
| [`static void ContentTransitionOptions.SetUseMotionSettings(TransitioningContentControl control, bool value)`](../../../TheOne.Avalonia/Controls/ContentTransitionOptions.cs#L31) | Sets the attached property. Uses shared motion configuration without forcing animation when globally/locally disabled. |
| [`static IPageTransition? ContentTransitionOptions.GetConfiguredTransition(TransitioningContentControl control)`](../../../TheOne.Avalonia/Controls/ContentTransitionOptions.cs#L36) | Reads the configured content transition; motion settings still determine playback. |
| [`ContentTransitionOptions.ContentTransitionOptions()`](../../../TheOne.Avalonia/Controls/ContentTransitionOptions.cs#L12) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> ContentTransitionOptions.UseMotionSettingsProperty`](../../../TheOne.Avalonia/Controls/ContentTransitionOptions.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
