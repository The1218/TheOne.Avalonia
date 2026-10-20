# Timeline

[English](../en/TheOne.Avalonia.Controls.Timeline.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Timeline.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Timeline` · Class · `Avalonia.Controls.ItemsControl`

A naturally sized vertical event history. Collection order is caller-owned.

[Source / 声明](../../../TheOne.Avalonia/Controls/Timeline.cs#L15)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ItemSpacing`](../../../TheOne.Avalonia/Controls/Timeline.cs#L34) | `double` | writable; OneWay; inherits | `16` | 16 DIP ; Finite nonnegative spacing inherited by TimelineItem; individually overridable; themed Small/Normal/Large values are 12/16/20 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`Mode`](../../../TheOne.Avalonia/Controls/Timeline.cs#L30) | `TheOne.Avalonia.Controls.TimelineMode` | writable; OneWay | `0 / default` | Left ; Left: left rail/right content; Right: right rail/left content; Alternate: centered rail, alternating by visual order, starting on the right Validation: `Enum.IsDefined` Values: `Left`, `Right`, `Alternate` |
| [`Reverse`](../../../TheOne.Avalonia/Controls/Timeline.cs#L32) | `bool` | writable; OneWay; inherits | `false` | false ; Reverses visual arrangement without mutating data; alternation and the last connector follow visual order |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`Timeline.Timeline()`](../../../TheOne.Avalonia/Controls/Timeline.cs#L27) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<TimelineMode> Timeline.ModeProperty`](../../../TheOne.Avalonia/Controls/Timeline.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> Timeline.ReverseProperty`](../../../TheOne.Avalonia/Controls/Timeline.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> Timeline.ItemSpacingProperty`](../../../TheOne.Avalonia/Controls/Timeline.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.TimelineItem`](TheOne.Avalonia.Controls.TimelineItem.md)
- [`TheOne.Avalonia.Controls.TimelineItemModel`](TheOne.Avalonia.Controls.TimelineItemModel.md)
- [`TheOne.Avalonia.Controls.TimelineMode`](TheOne.Avalonia.Controls.TimelineMode.md)
- [`TheOne.Avalonia.Controls.TimelineStatus`](TheOne.Avalonia.Controls.TimelineStatus.md)
