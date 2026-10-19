# WorkflowPort

[English](../en/TheOne.Avalonia.Workflow.WorkflowPort.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowPort.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowPort` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L40)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`DataType`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L44) | `string` | 可写; CLR / init-only | `"flow"` | 宿主定义的类型键，例如 flow、text、number；用于端口／引用兼容判断，不是 CLR 类型名。 |
| [`Direction`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L43) | `TheOne.Avalonia.Workflow.WorkflowPortDirection` | 可写; CLR / init-only | `required constructor argument` | 展开或端口方向枚举；具体枚举类型列明可选值，不用布尔值代替方向。 可选值：`Input`, `Output` |
| [`Fields`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L46) | `TheOne.Avalonia.Workflow.WorkflowOutputField[]?` | 可写; CLR / init-only | `null` | 端口公开的输出字段元数据；允许宿主描述可引用的数据结构。 |
| [`Id`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L41) | `string` | 可写; CLR / init-only | `required constructor argument` | 模型稳定标识；持久化／引用使用，避免在同一文档中重复。 |
| [`MaxConnections`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L45) | `int` | 可写; CLR / init-only | `1` | 端口最大连线数；具体允许值及连接约束由流程文档校验处理。 |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L42) | `string` | 可写; CLR / init-only | `required constructor argument` | 显示标题；不等于业务主键或本地化资源键。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowPort.WorkflowPort(string Id, string Title, WorkflowPortDirection Direction, string DataType = "flow", int MaxConnections = 1, WorkflowOutputField[]? Fields = null)`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L40) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowOutputField`](TheOne.Avalonia.Workflow.WorkflowOutputField.md)
- [`TheOne.Avalonia.Workflow.WorkflowPortDirection`](TheOne.Avalonia.Workflow.WorkflowPortDirection.md)
