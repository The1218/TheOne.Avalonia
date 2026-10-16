# StepValidationError

[English](../en/TheOne.Avalonia.Controls.StepValidationError.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.StepValidationError.md) · [API index](README.md)

`TheOne.Avalonia.Controls.StepValidationError` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`MemberName`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | writable; CLR / init-only | `required constructor argument` | Inherited from `StepField`; matches error member names such as `Name`, never executed as a reflection binding path |
| [`Message`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | writable; CLR / init-only | `required constructor argument` | Result message for users/diagnostics; status fields still determine the operation outcome. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`StepValidationError.StepValidationError(string MemberName, string Message)`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
