# WorkflowReferenceOption

[English](../en/TheOne.Avalonia.Workflow.WorkflowReferenceOption.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowReferenceOption.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowReferenceOption` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L52)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Reference`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L52) | `TheOne.Avalonia.Workflow.WorkflowDataReference` | writable; CLR / init-only | `required constructor argument` | Structured upstream-output reference; null uses the literal value, without persisting resolved runtime data. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L52) | `string` | writable; CLR / init-only | `required constructor argument` | Display title, distinct from business identity or a localization key. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowReferenceOption.WorkflowReferenceOption(string Title, WorkflowDataReference Reference)`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L52) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowDataReference`](TheOne.Avalonia.Workflow.WorkflowDataReference.md)
