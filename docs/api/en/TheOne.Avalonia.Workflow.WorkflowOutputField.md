# WorkflowOutputField

[English](../en/TheOne.Avalonia.Workflow.WorkflowOutputField.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowOutputField.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowOutputField` · Class · `object`

Stable, host-defined data type keys; no CLR type lookup or reflection is performed.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L14)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`DataType`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L14) | `string` | writable; CLR / init-only | `"text"` | Host-defined type key such as flow/text/number for port/reference compatibility, not a CLR type name. |
| [`Path`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L14) | `string` | writable; CLR / init-only | `required constructor argument` | Stable output-field path key matching FieldPath in data references. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L14) | `string` | writable; CLR / init-only | `required constructor argument` | Display title, distinct from business identity or a localization key. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowOutputField.WorkflowOutputField(string Path, string Title, string DataType = "text")`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L14) | Stable, host-defined data type keys; no CLR type lookup or reflection is performed. |
