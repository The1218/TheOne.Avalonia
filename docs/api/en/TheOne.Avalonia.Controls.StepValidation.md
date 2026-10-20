# StepValidation

[English](../en/TheOne.Avalonia.Controls.StepValidation.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.StepValidation.md) · [API index](README.md)

`TheOne.Avalonia.Controls.StepValidation` · Class · `object`

Creates attribute validators for concrete bound model types without dynamic binding or assembly-wide preservation.

[Source / 声明](../../../TheOne.Avalonia/Controls/StepValidation.cs#L25)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static IStepValidator StepValidation.For<T>(T model, params string[] memberNames) where T : class`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L27) | Creates a typed step validator for the supplied model/member set. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.IStepValidator`](TheOne.Avalonia.Controls.IStepValidator.md)
