# StepField

[English](../en/TheOne.Avalonia.Controls.StepField.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.StepField.md) · [API index](README.md)

`TheOne.Avalonia.Controls.StepField` · Class · `Avalonia.Controls.ContentControl`

Places a step member's validation messages directly below its caller-owned input.

[Source / 声明](../../../TheOne.Avalonia/Controls/StepField.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Errors`](../../../TheOne.Avalonia/Controls/StepField.cs#L16) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasErrors`](../../../TheOne.Avalonia/Controls/StepField.cs#L17) | `bool` | read-only; OneWay | `computed / state` | All validation errors |
| [`MemberName`](../../../TheOne.Avalonia/Controls/StepField.cs#L15) | `string` | writable; OneWay | `""` | Exact model member name used to map validation errors to a field, separate from its display label. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`StepField.StepField()`](../../../TheOne.Avalonia/Controls/StepField.cs#L8) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string> StepField.MemberNameProperty`](../../../TheOne.Avalonia/Controls/StepField.cs#L10) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<StepField, IReadOnlyList<StepValidationError>> StepField.ErrorsProperty`](../../../TheOne.Avalonia/Controls/StepField.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<StepField, bool> StepField.HasErrorsProperty`](../../../TheOne.Avalonia/Controls/StepField.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.StepValidationError`](TheOne.Avalonia.Controls.StepValidationError.md)
