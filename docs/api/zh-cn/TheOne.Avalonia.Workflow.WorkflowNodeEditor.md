# WorkflowNodeEditor

[English](../en/TheOne.Avalonia.Workflow.WorkflowNodeEditor.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeEditor.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowNodeEditor` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[继承的额外成员: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

Undo-aware presentation for the default inspector; usable in custom templates.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L6)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Description`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L12) | `string` | 可写; CLR / n/a | `accessor-defined / state` | 描述文本；由宿主提供业务说明，未提供时沿用初值。 |
| [`Document`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L8) | `TheOne.Avalonia.Workflow.WorkflowDocument` | 只读; CLR / n/a | `accessor-defined / state` | 当前业务文档模型；替换时重接订阅、选择和历史的具体行为见编辑器文档。 |
| [`Fields`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L10) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowPropertyEditor>` | 只读; CLR / n/a | `[]` | 端口公开的输出字段元数据；允许宿主描述可引用的数据结构。 |
| [`Node`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L9) | `TheOne.Avalonia.Workflow.WorkflowNode` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L11) | `string` | 可写; CLR / n/a | `accessor-defined / state` | 显示标题；不等于业务主键或本地化资源键。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void WorkflowNodeEditor.Refresh()`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L20) | Refresh after document edits, including undo and upstream metadata changes. |
| [`WorkflowNodeEditor.WorkflowNodeEditor(WorkflowDocument document, WorkflowNode node)`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L14) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowDocument`](TheOne.Avalonia.Workflow.WorkflowDocument.md)
- [`TheOne.Avalonia.Workflow.WorkflowNode`](TheOne.Avalonia.Workflow.WorkflowNode.md)
- [`TheOne.Avalonia.Workflow.WorkflowPropertyEditor`](TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md)
