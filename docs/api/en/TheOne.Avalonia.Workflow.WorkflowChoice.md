# WorkflowChoice

[English](../en/TheOne.Avalonia.Workflow.WorkflowChoice.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowChoice.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowChoice` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[Inherited additional members: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

Presentation for a stable key. Unresolved saved keys remain editable instead of being discarded.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L25)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IsAvailable`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L29) | `bool` | read-only; CLR / n/a | `true` | Whether this key currently exists in the host catalog; it does not indicate approval authorization. |
| [`Key`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L27) | `string` | read-only; CLR / n/a | `key` | Field/parameter key used to resolve business values, distinct from display names. |
| [`Text`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L28) | `string` | read-only; CLR / n/a | `text` | Input/display text; notifications and validation follow the model/native editor contract. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`WorkflowChoice.WorkflowChoice(string key, string text)`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L25) | Presentation for a stable key. Unresolved saved keys remain editable instead of being discarded. |
