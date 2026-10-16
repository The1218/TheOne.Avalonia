# StepCompareAttribute

[English](../en/TheOne.Avalonia.Controls.StepCompareAttribute.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.StepCompareAttribute.md) · [API index](README.md)

`TheOne.Avalonia.Controls.StepCompareAttribute` · Class · `System.ComponentModel.DataAnnotations.ValidationAttribute`

Cross-field comparison using StepValidation's preserved property accessors, without runtime type discovery.

[Source / 声明](../../../TheOne.Avalonia/Controls/StepCompareAttribute.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`OtherProperty`](../../../TheOne.Avalonia/Controls/StepCompareAttribute.cs#L10) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`StepCompareAttribute.StepCompareAttribute(string otherProperty)`](../../../TheOne.Avalonia/Controls/StepCompareAttribute.cs#L11) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
