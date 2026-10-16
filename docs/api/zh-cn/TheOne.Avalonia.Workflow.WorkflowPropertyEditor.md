# WorkflowPropertyEditor

[English](../en/TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowPropertyEditor` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[继承的额外成员: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

A literal/reference editor backed by the same document undo history.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L33)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsLiteral`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L41) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Literal`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L40) | `string` | 可写; CLR / n/a | `accessor-defined / state` | 当前流程属性的固定值文本；通过文档编辑入口修改，以参与撤销历史。 |
| [`Options`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L42) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowReferenceOption>` | 只读; CLR / n/a | `[]` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`Property`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L37) | `TheOne.Avalonia.Workflow.WorkflowProperty` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SelectedOption`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L43) | `TheOne.Avalonia.Workflow.WorkflowReferenceOption?` | 可写; CLR / n/a | `accessor-defined / state` | 属性检查器选中的固定值／上游引用选项；切换通过文档历史更新 Reference。 |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L38) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 显示标题；不等于业务主键或本地化资源键。 |
| [`TypeHint`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L39) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void WorkflowPropertyEditor.Refresh()`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L57) | 从底层模型与文档重新同步属性检查器，包含撤销后及上游变化。 |
| [`WorkflowPropertyEditor.WorkflowPropertyEditor(WorkflowDocument document, WorkflowNode node, WorkflowProperty property)`](../../../TheOne.Avalonia/Workflow/WorkflowNodeEditor.cs#L53) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowDocument`](TheOne.Avalonia.Workflow.WorkflowDocument.md)
- [`TheOne.Avalonia.Workflow.WorkflowNode`](TheOne.Avalonia.Workflow.WorkflowNode.md)
- [`TheOne.Avalonia.Workflow.WorkflowProperty`](TheOne.Avalonia.Workflow.WorkflowProperty.md)
- [`TheOne.Avalonia.Workflow.WorkflowReferenceOption`](TheOne.Avalonia.Workflow.WorkflowReferenceOption.md)
