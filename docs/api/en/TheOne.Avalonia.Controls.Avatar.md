# Avatar

[English](../en/TheOne.Avalonia.Controls.Avatar.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Avatar.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Avatar` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

An asynchronously loaded avatar with text or person-icon fallback.

[Source / 声明](../../../TheOne.Avalonia/Controls/Avatar.cs#L17)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CurrentImage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L34) | `Avalonia.Media.IImage?` | read-only; OneWay | `computed / state` | Read-only decoded image and successful-image state |
| [`DisplayText`](../../../TheOne.Avalonia/Controls/Avatar.cs#L38) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`EffectiveCornerRadius`](../../../TheOne.Avalonia/Controls/Avatar.cs#L39) | `Avalonia.CornerRadius` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L37) | `string?` | read-only; OneWay | `computed / state` | Read-only loading state and failure details; empty source clears errors |
| [`HasImage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L36) | `bool` | read-only; OneWay | `computed / state` | Read-only decoded image and successful-image state |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/Avatar.cs#L35) | `bool` | read-only; OneWay | `computed / state` | Read-only loading state and failure details; empty source clears errors |
| [`Shape`](../../../TheOne.Avalonia/Controls/Avatar.cs#L32) | `TheOne.Avalonia.Controls.AvatarShape` | writable; OneWay | `0 / default` | `Circle` (default), `Rounded`, or `Square` Validation: `Enum.IsDefined` Values: `Circle`, `Rounded`, `Square` |
| [`Source`](../../../TheOne.Avalonia/Controls/Avatar.cs#L30) | `object?` | writable; OneWay | `null` | Local absolute/relative path, `file://` URI, HTTP(S) URL, `avares://` URI, or caller-owned `IImage`; accepts strings and `Uri` objects |
| [`Stretch`](../../../TheOne.Avalonia/Controls/Avatar.cs#L33) | `Avalonia.Media.Stretch` | writable; OneWay | `Stretch.UniformToFill` | Native image stretch; default `UniformToFill` crops to fill the avatar Validation: `Enum.IsDefined` Values: `None`, `Fill`, `Uniform`, `UniformToFill` |
| [`Text`](../../../TheOne.Avalonia/Controls/Avatar.cs#L31) | `string?` | writable; OneWay | `null` | Fallback text; trims whitespace and displays at most two graphemes without splitting emoji; blank text uses a person icon |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void Avatar.Reload()`](../../../TheOne.Avalonia/Controls/Avatar.cs#L83) | Reloads the current source while attached, for example after a failed request. |
| [`Avatar.Avatar()`](../../../TheOne.Avalonia/Controls/Avatar.cs#L51) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<object?> Avatar.SourceProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> Avatar.TextProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<AvatarShape> Avatar.ShapeProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Stretch> Avatar.StretchProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Avatar, IImage?> Avatar.CurrentImageProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Avatar, bool> Avatar.IsLoadingProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Avatar, bool> Avatar.HasImageProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Avatar, string?> Avatar.ErrorMessageProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Avatar, string> Avatar.DisplayTextProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Avatar, CornerRadius> Avatar.EffectiveCornerRadiusProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.AvatarShape`](TheOne.Avalonia.Controls.AvatarShape.md)
