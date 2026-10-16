# DescriptionItem

[English](../en/TheOne.Avalonia.Controls.DescriptionItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DescriptionItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DescriptionItem` · Class · `Avalonia.Controls.Primitives.HeaderedContentControl`

A label and value row. Header and Content accept text, controls or typed data templates.

[Source / 声明](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L18) | `double` | writable; OneWay; inherits | `180` | `double`, `180` ; Label column width in DIPs in two-column layouts. Bindable, styleable and inherited by rows; must be finite and nonnegative. Validation: `value => double.IsFinite(value) && value >= 0` |
| [`StackWidth`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L21) | `double` | writable; OneWay; inherits | `520` | `double`, `520` ; Rows narrower than this value place the label above the value. `0` retains two columns; must be finite and nonnegative. Validation: `value => double.IsFinite(value) && value >= 0` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`DescriptionItem.DescriptionItem()`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L10) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<double> DescriptionItem.LabelWidthProperty`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> DescriptionItem.StackWidthProperty`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.Description`](TheOne.Avalonia.Controls.Description.md)
