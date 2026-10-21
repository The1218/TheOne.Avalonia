# WorkflowSerializer

[English](../en/TheOne.Avalonia.Workflow.WorkflowSerializer.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowSerializer.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowSerializer` · Class · `object`

Versioned, source-generated JSON. Templates, Content, delegates and history are not persisted.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static string WorkflowSerializer.Save(WorkflowDocument document)`](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L13) | 以当前格式保存完整设计，不保存视觉对象、运行状态或撤销历史。 |
| [`static WorkflowDocument WorkflowSerializer.Load(string json)`](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L38) | 读取版本 1 或 2；非法身份和字段抛出 JsonException，失效图引用保留给 Validate 报告。 |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`const int WorkflowSerializer.CurrentVersion`](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L10) | 当前快照格式；版本 2 增加连线字段，仍可读取版本 1。 |

## Related APIs and built-in models

- [`TheOne.Avalonia.Workflow.WorkflowDocument`](TheOne.Avalonia.Workflow.WorkflowDocument.md)
