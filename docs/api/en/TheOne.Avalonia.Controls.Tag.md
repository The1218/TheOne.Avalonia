# Tag

[English](../en/TheOne.Avalonia.Controls.Tag.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Tag.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Tag` · Class · `Avalonia.Controls.ContentControl`

A compact status label with optional caller-owned icon and content.

[Source / 声明](../../../TheOne.Avalonia/Controls/Tag.cs#L27)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Icon`](../../../TheOne.Avalonia/Controls/Tag.cs#L59) | `object?` | writable; OneWay | `null` | Optional PathIcon, Image, or model template; retains logical-tree / DataContext behavior |
| [`IconSize`](../../../TheOne.Avalonia/Controls/Tag.cs#L53) | `double` | writable; OneWay | `13` | Scaled side length, default 13 px, one step below the former 14 px baseline; shared across densities and overridable without changing caller icon dimensions Validation: `value => double.IsFinite(value) && value > 0` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/Tag.cs#L65) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Optional PathIcon, Image, or model template; retains logical-tree / DataContext behavior |
| [`Spacing`](../../../TheOne.Avalonia/Controls/Tag.cs#L83) | `double` | writable; OneWay | `6` | Icon / content gap, default 6; absent icon / text leaves no gap Validation: `value => double.IsFinite(value) && value >= 0` |
| [`Status`](../../../TheOne.Avalonia/Controls/Tag.cs#L71) | `TheOne.Avalonia.Controls.TagStatus` | writable; OneWay | `0 / default` | `Default`, `Information`, `Success`, `Warning`, `Error`, `Processing` Validation: `Enum.IsDefined` Values: `Default`, `Information`, `Success`, `Warning`, `Error`, `Processing` |
| [`Variant`](../../../TheOne.Avalonia/Controls/Tag.cs#L77) | `TheOne.Avalonia.Controls.TagVariant` | writable; OneWay | `0 / default` | `Soft` background, `Outline` border, `Solid` fill Validation: `Enum.IsDefined` Values: `Soft`, `Outline`, `Solid` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`Tag.Tag()`](../../../TheOne.Avalonia/Controls/Tag.cs#L48) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<object?> Tag.IconProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> Tag.IconTemplateProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TagStatus> Tag.StatusProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TagVariant> Tag.VariantProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L37) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> Tag.IconSizeProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L40) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> Tag.SpacingProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L44) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `dashed` | [`^.dashed`](../../../TheOne.Avalonia/Themes/Controls/Tag.axaml#L191) | ^.dashed => BorderBrush=Transparent; ^.dashed /template/ Rectangle#PART_DashedOutline => IsVisible=True |
| `dot` | [`^.dot /template/ Ellipse#PART_StatusDot`](../../../TheOne.Avalonia/Themes/Controls/Tag.axaml#L197) | ^.dot /template/ Ellipse#PART_StatusDot => IsVisible=True |
| `gradient` | [`^.gradient`](../../../TheOne.Avalonia/Themes/Controls/Tag.axaml#L200) | ^.gradient => Background={DynamicResource One.Gradient.Brand}; Foreground={DynamicResource One.OnAccent}; BorderBrush=Transparent |
| `pill` | [`^.pill`](../../../TheOne.Avalonia/Themes/Controls/Tag.axaml#L185) | ^.pill => CornerRadius=999 |
| `square` | [`^.square`](../../../TheOne.Avalonia/Themes/Controls/Tag.axaml#L188) | ^.square => CornerRadius=0 |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.TagStatus`](TheOne.Avalonia.Controls.TagStatus.md)
- [`TheOne.Avalonia.Controls.TagVariant`](TheOne.Avalonia.Controls.TagVariant.md)
