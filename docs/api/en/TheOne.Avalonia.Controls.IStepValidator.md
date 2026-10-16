# IStepValidator

[English](../en/TheOne.Avalonia.Controls.IStepValidator.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IStepValidator.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IStepValidator` · Interface

Validates one step. Implement this for asynchronous or service-backed validation.

[Source / 声明](../../../TheOne.Avalonia/Controls/StepValidation.cs#L16)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`Task<IReadOnlyList<StepValidationError>> IStepValidator.ValidateAsync(CancellationToken cancellationToken)`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L18) | Validates model rules and native conversion errors; returns `false` on failure, cancellation, detachment, disabled state, or pending validation |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.StepValidationError`](TheOne.Avalonia.Controls.StepValidationError.md)
