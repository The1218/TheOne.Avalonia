# DataGridSummaryResult

[English](../en/TheOne.Avalonia.Controls.DataGridSummaryResult.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridSummaryResult.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridSummaryResult` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridSummary.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Error`](../../../TheOne.Avalonia/Controls/DataGridSummary.cs#L8) | `string?` | writable; CLR / init-only | `null` | Structured/text failure information; inspect it together with the success flag. |
| [`ValidCount`](../../../TheOne.Avalonia/Controls/DataGridSummary.cs#L8) | `long` | writable; CLR / init-only | `required constructor argument` | Count of valid values included in the aggregate, distinct from total source rows. |
| [`Value`](../../../TheOne.Avalonia/Controls/DataGridSummary.cs#L8) | `object?` | writable; CLR / init-only | `required constructor argument` | Current value; units, range and null semantics are defined by its model and validation constraints. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`DataGridSummaryResult.DataGridSummaryResult(object? Value, long ValidCount, string? Error = null)`](../../../TheOne.Avalonia/Controls/DataGridSummary.cs#L8) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
