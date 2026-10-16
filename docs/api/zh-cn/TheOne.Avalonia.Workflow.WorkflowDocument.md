# WorkflowDocument

[English](../en/TheOne.Avalonia.Workflow.WorkflowDocument.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowDocument` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[继承的额外成员: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

Observable UI-thread document. Editor operations are reversible; direct host changes reset history.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanRedo`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L19) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanUndo`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L18) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Connections`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L17) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowConnection>` | 只读; CLR / n/a | `[]` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`Nodes`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L16) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowNode>` | 只读; CLR / n/a | `[]` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`Revision`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L20) | `long` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L15) | `string` | 可写; CLR / n/a | `"新建流程"` | 显示标题；不等于业务主键或本地化资源键。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void WorkflowDocument.Edit(string title, Action redo, Action undo)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L52) | Apply a host edit to the same history as graph edits. Actions must be deterministic and not throw. |
| [`void WorkflowDocument.Undo()`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L59) | 撤销最近一次可撤销的文档编辑。 |
| [`void WorkflowDocument.Redo()`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L60) | 重做最近撤销的文档编辑。 |
| [`void WorkflowDocument.ClearHistory()`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L61) | 清空撤销与重做记录，保留当前文档数据。 |
| [`void WorkflowDocument.AddNode(WorkflowNode node)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L62) | 向当前工作流文档加入节点；通过文档历史入口保存可撤销操作。 |
| [`void WorkflowDocument.Remove(IEnumerable<WorkflowNode> nodes, WorkflowConnection? connection = null)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L68) | 删除指定工作流对象及关联关系，通过文档编辑历史执行。 |
| [`void WorkflowDocument.MoveNodes(IEnumerable<(WorkflowNode Node, double X, double Y)> positions)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L77) | 批量更新节点坐标并记录一次可撤销移动。 |
| [`void WorkflowDocument.SetProperty(WorkflowProperty property, string value, WorkflowDataReference? reference)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L84) | 通过文档修改流程属性的固定值／引用，以保持撤销与通知。 |
| [`string? WorkflowDocument.CheckConnection(string sourceNode, string sourcePort, string targetNode, string targetPort, bool allowCycles = false)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L90) | 校验候选端口连接，返回失败原因而不创建连线。 |
| [`bool WorkflowDocument.TryConnect(string sourceNode, string sourcePort, string targetNode, string targetPort, out string? error, bool allowCycles = false)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L104) | 校验并尝试创建连接；返回是否成功及相关结果。 |
| [`bool WorkflowDocument.IsReachable(string from, string to)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L111) | 判断流程图中目标节点是否可从起点到达，用于连线与循环判断。 |
| [`IReadOnlyList<WorkflowReferenceOption> WorkflowDocument.GetReferenceOptions(WorkflowNode target, WorkflowProperty property)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L117) | 为当前节点属性列出兼容的上游数据引用选项。 |
| [`IReadOnlyList<WorkflowValidationIssue> WorkflowDocument.Validate(bool allowCycles = false)`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L121) | 校验工作流结构、端口与属性引用，返回可定位的问题集合。 |
| [`WorkflowDocument.WorkflowDocument()`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L22) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler? WorkflowDocument.Changed`](../../../TheOne.Avalonia/Workflow/WorkflowDocument.cs#L21) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowConnection`](TheOne.Avalonia.Workflow.WorkflowConnection.md)
- [`TheOne.Avalonia.Workflow.WorkflowDataReference`](TheOne.Avalonia.Workflow.WorkflowDataReference.md)
- [`TheOne.Avalonia.Workflow.WorkflowNode`](TheOne.Avalonia.Workflow.WorkflowNode.md)
- [`TheOne.Avalonia.Workflow.WorkflowProperty`](TheOne.Avalonia.Workflow.WorkflowProperty.md)
- [`TheOne.Avalonia.Workflow.WorkflowReferenceOption`](TheOne.Avalonia.Workflow.WorkflowReferenceOption.md)
- [`TheOne.Avalonia.Workflow.WorkflowValidationIssue`](TheOne.Avalonia.Workflow.WorkflowValidationIssue.md)
