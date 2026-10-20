# TimelineItem

[English](../en/TheOne.Avalonia.Controls.TimelineItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TimelineItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TimelineItem` · Class · `Avalonia.Controls.Primitives.HeaderedContentControl`

An event with a title, body, timestamp and semantic or custom marker.

[Source / 声明](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L13)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Dot`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L30) | `object?` | writable; OneWay | `null` | null ; Custom marker content/template; non-null replaces the dot, returning to null restores current Status |
| [`DotTemplate`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L31) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | null ; Custom marker content/template; non-null replaces the dot, returning to null restores current Status |
| [`ItemSpacing`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L33) | `double` | writable; OneWay; inherits | `16` | 16 DIP ; Finite nonnegative spacing inherited by TimelineItem; individually overridable; themed Small/Normal/Large values are 12/16/20 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`Status`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L32) | `TheOne.Avalonia.Controls.TimelineStatus` | writable; OneWay | `0 / default` | Default ; Default (accent), Success, Warning, Error, Pending; Pending is a static hollow marker without looping animation Validation: `Enum.IsDefined` Values: `Default`, `Success`, `Warning`, `Error`, `Pending` |
| [`Timestamp`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L28) | `object?` | writable; OneWay | `null` | null ; Timestamp content/template; null hides it. Above the body in Left/Right, opposite the rail in Alternate |
| [`TimestampTemplate`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L29) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | null ; Timestamp content/template; null hides it. Above the body in Left/Right, opposite the rail in Alternate |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`TimelineItem.TimelineItem()`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L27) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<object?> TimelineItem.TimestampProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> TimelineItem.TimestampTemplateProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> TimelineItem.DotProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> TimelineItem.DotTemplateProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TimelineStatus> TimelineItem.StatusProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> TimelineItem.ItemSpacingProperty`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.TimelineItemModel`](TheOne.Avalonia.Controls.TimelineItemModel.md)
- [`TheOne.Avalonia.Controls.TimelineStatus`](TheOne.Avalonia.Controls.TimelineStatus.md)
