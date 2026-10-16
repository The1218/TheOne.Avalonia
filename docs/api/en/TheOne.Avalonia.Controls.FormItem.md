# FormItem

[English](../en/TheOne.Avalonia.Controls.FormItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FormItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FormItem` · Class · `TheOne.Avalonia.Controls.StepField`

[Inherited additional members: StepField](TheOne.Avalonia.Controls.StepField.md)

A labelled field with caller-owned native input and shared step validation messages.

[Source / 声明](../../../TheOne.Avalonia/Controls/FormItem.cs#L13)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`EffectiveLabelWidth`](../../../TheOne.Avalonia/Controls/FormItem.cs#L37) | `double` | read-only; OneWay | `computed / state` | Read-only template state for region visibility, label docking, and effective label width |
| [`HasHelpText`](../../../TheOne.Avalonia/Controls/FormItem.cs#L39) | `bool` | read-only; OneWay | `computed / state` | Read-only template state for region visibility, label docking, and effective label width |
| [`HasLabel`](../../../TheOne.Avalonia/Controls/FormItem.cs#L38) | `bool` | read-only; OneWay | `computed / state` | Read-only template state for region visibility, label docking, and effective label width |
| [`HelpText`](../../../TheOne.Avalonia/Controls/FormItem.cs#L31) | `string?` | writable; OneWay | `null` | Label and help below input; empty values hide their regions |
| [`IsRequired`](../../../TheOne.Avalonia/Controls/FormItem.cs#L33) | `bool` | writable; OneWay | `false` | Defaults to `false`; visual marker only, with actual rules owned by the validator |
| [`Label`](../../../TheOne.Avalonia/Controls/FormItem.cs#L30) | `string?` | writable; OneWay | `null` | Label and help below input; empty values hide their regions |
| [`LabelDock`](../../../TheOne.Avalonia/Controls/FormItem.cs#L36) | `Avalonia.Controls.Dock` | read-only; OneWay | `computed / state` | Read-only template state for region visibility, label docking, and effective label width Values: `Left`, `Bottom`, `Right`, `Top` |
| [`LabelOrientation`](../../../TheOne.Avalonia/Controls/FormItem.cs#L34) | `Avalonia.Layout.Orientation` | writable; OneWay; inherits | `Orientation.Vertical` | Defaults to `Vertical` / `120`, inherited by items; width must be positive and finite Validation: `Enum.IsDefined` Values: `Horizontal`, `Vertical` |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/FormItem.cs#L35) | `double` | writable; OneWay; inherits | `120` | Defaults to `Vertical` / `120`, inherited by items; width must be positive and finite Validation: `value => double.IsFinite(value) && value > 0` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`FormItem.FormItem()`](../../../TheOne.Avalonia/Controls/FormItem.cs#L13) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string?> FormItem.LabelProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> FormItem.HelpTextProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> FormItem.IsRequiredProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Orientation> FormItem.LabelOrientationProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> FormItem.LabelWidthProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<FormItem, Dock> FormItem.LabelDockProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<FormItem, double> FormItem.EffectiveLabelWidthProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<FormItem, bool> FormItem.HasLabelProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<FormItem, bool> FormItem.HasHelpTextProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
