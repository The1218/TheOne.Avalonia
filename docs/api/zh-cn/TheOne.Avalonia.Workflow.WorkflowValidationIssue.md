# WorkflowValidationIssue

[English](../en/TheOne.Avalonia.Workflow.WorkflowValidationIssue.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowValidationIssue.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowValidationIssue` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L50)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ConnectionId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L50) | `string?` | 可写; CLR / init-only | `null` | 校验问题关联的连线 Id；空值表示问题不定位到具体连线。 |
| [`Message`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L50) | `string` | 可写; CLR / init-only | `required constructor argument` | 可供用户或诊断展示的结果说明；操作结果仍以状态字段为准。 |
| [`NodeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L50) | `string?` | 可写; CLR / init-only | `null` | 引用或校验问题关联的节点 Id；可空类型表示没有定位到特定节点。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowValidationIssue.WorkflowValidationIssue(string Message, string? NodeId = null, string? ConnectionId = null)`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L50) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
