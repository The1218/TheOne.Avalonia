# WorkflowPort

[English](../en/TheOne.Avalonia.Workflow.WorkflowPort.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowPort.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowPort` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L15)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`DataType`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L16) | `string` | writable; CLR / init-only | `"flow"` | Host-defined type key such as flow/text/number for port/reference compatibility, not a CLR type name. |
| [`Direction`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L15) | `TheOne.Avalonia.Workflow.WorkflowPortDirection` | writable; CLR / init-only | `required constructor argument` | Reveal/port direction enum; choices are defined by its concrete enum type. Values: `Input`, `Output` |
| [`Fields`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L16) | `TheOne.Avalonia.Workflow.WorkflowOutputField[]?` | writable; CLR / init-only | `null` | Output-field metadata exposed by a port to describe referenceable data. |
| [`Id`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L15) | `string` | writable; CLR / init-only | `required constructor argument` | Stable model identifier for persistence/references; avoid duplicates in one document. |
| [`MaxConnections`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L16) | `int` | writable; CLR / init-only | `1` | Maximum connections for a port; workflow document validation enforces supported values/connection rules. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L15) | `string` | writable; CLR / init-only | `required constructor argument` | Display title, distinct from business identity or a localization key. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowPort.WorkflowPort(string Id, string Title, WorkflowPortDirection Direction, string DataType = "flow", int MaxConnections = 1, WorkflowOutputField[]? Fields = null)`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L15) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowOutputField`](TheOne.Avalonia.Workflow.WorkflowOutputField.md)
- [`TheOne.Avalonia.Workflow.WorkflowPortDirection`](TheOne.Avalonia.Workflow.WorkflowPortDirection.md)
