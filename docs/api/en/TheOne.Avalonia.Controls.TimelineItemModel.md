# TimelineItemModel

[English](../en/TheOne.Avalonia.Controls.TimelineItemModel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TimelineItemModel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TimelineItemModel` · Class · `Avalonia.AvaloniaObject`

Optional observable event data; Key remains caller-owned identity. Timestamp is displayed, never sorted or formatted implicitly.

[Source / 声明](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L6)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Content`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L16) | `object?` | writable; OneWay | `null` | Host content/presentation object; follow the content-template scope and do not persist visual objects as business data. |
| [`Dot`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L18) | `object?` | writable; OneWay | `null` | null ; Custom marker content/template; non-null replaces the dot, returning to null restores current Status |
| [`Header`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L15) | `object?` | writable; OneWay | `null` | Header content, as text or a model; HeaderTemplate controls custom presentation. |
| [`Key`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L14) | `object?` | writable; OneWay | `null` | Field/parameter key used to resolve business values, distinct from display names. |
| [`Status`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L19) | `TheOne.Avalonia.Controls.TimelineStatus` | writable; OneWay | `0 / default` | Default ; Default (accent), Success, Warning, Error, Pending; Pending is a static hollow marker without looping animation Validation: `Enum.IsDefined` Values: `Default`, `Success`, `Warning`, `Error`, `Pending` |
| [`Timestamp`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L17) | `object?` | writable; OneWay | `null` | null ; Timestamp content/template; null hides it. Above the body in Left/Right, opposite the rail in Alternate |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`TimelineItemModel.TimelineItemModel()`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L6) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<object?> TimelineItemModel.KeyProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L8) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> TimelineItemModel.HeaderProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> TimelineItemModel.ContentProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L10) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> TimelineItemModel.TimestampProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> TimelineItemModel.DotProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TimelineStatus> TimelineItemModel.StatusProperty`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.TimelineStatus`](TheOne.Avalonia.Controls.TimelineStatus.md)
