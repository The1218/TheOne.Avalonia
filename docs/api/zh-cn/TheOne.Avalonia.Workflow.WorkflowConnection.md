# WorkflowConnection

[English](../en/TheOne.Avalonia.Workflow.WorkflowConnection.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowConnection.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowConnection` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[继承的额外成员: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L84)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Id`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L87) | `string` | 可写; CLR / init-only | `Guid.NewGuid().ToString("N")` | 模型稳定标识；持久化／引用使用，避免在同一文档中重复。 |
| [`Label`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L92) | `string` | 可写; CLR / n/a | `""` | 显示标签文本；不作为模型身份、字段路径或序列化标识。 |
| [`SourceNodeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L88) | `string` | 可写; CLR / init-only | `""` | 连线起点节点的稳定 Id，须与文档节点集合对应。 |
| [`SourcePortId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L89) | `string` | 可写; CLR / init-only | `""` | 连线起点的输出端口 Id，须属于 SourceNodeId 对应节点。 |
| [`TargetNodeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L90) | `string` | 可写; CLR / init-only | `""` | 连线终点节点的稳定 Id，须与文档节点集合对应。 |
| [`TargetPortId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L91) | `string` | 可写; CLR / init-only | `""` | 连线终点的输入端口 Id，须属于 TargetNodeId 对应节点。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowConnection.WorkflowConnection()`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L84) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
