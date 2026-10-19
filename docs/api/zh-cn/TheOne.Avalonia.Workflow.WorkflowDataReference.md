# WorkflowDataReference

[English](../en/TheOne.Avalonia.Workflow.WorkflowDataReference.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowDataReference.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowDataReference` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L48)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`FieldPath`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L48) | `string` | 可写; CLR / init-only | `""` | 上游输出字段的稳定路径键；由宿主定义，不解析 CLR 反射路径。 |
| [`NodeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L48) | `string` | 可写; CLR / init-only | `required constructor argument` | 引用或校验问题关联的节点 Id；可空类型表示没有定位到特定节点。 |
| [`OutputPortId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L48) | `string` | 可写; CLR / init-only | `required constructor argument` | 数据引用的上游输出端口 Id。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowDataReference.WorkflowDataReference(string NodeId, string OutputPortId, string FieldPath = "")`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L48) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
