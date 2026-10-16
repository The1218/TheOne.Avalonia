# DataGridFilterApplyResult

[English](../en/TheOne.Avalonia.Controls.DataGridFilterApplyResult.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridFilterApplyResult.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridFilterApplyResult` · Struct · `System.ValueType`

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Error`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L8) | `string?` | writable; CLR / init-only | `null` | Structured/text failure information; inspect it together with the success flag. |
| [`Succeeded`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L8) | `bool` | writable; CLR / init-only | `required constructor argument` | Whether the operation succeeded; read Error on failure rather than treating empty output as success. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`DataGridFilterApplyResult.DataGridFilterApplyResult(bool Succeeded, string? Error = null)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L8) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
| [`DataGridFilterApplyResult.DataGridFilterApplyResult()`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L8) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
