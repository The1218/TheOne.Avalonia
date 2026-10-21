# WorkflowNodeGroup

[English](../en/TheOne.Avalonia.Workflow.WorkflowNodeGroup.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeGroup.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowNodeGroup` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L265)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Items`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L265) | `TheOne.Avalonia.Workflow.WorkflowNodeDefinition[]` | 可写; CLR / init-only | `required constructor argument` | 当前项集合；具体元素与增删权限见类型签名和控件文档。 |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L265) | `string` | 可写; CLR / init-only | `required constructor argument` | 显示标题；不等于业务主键或本地化资源键。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowNodeGroup.WorkflowNodeGroup(string Title, WorkflowNodeDefinition[] Items)`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L265) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowNodeDefinition`](TheOne.Avalonia.Workflow.WorkflowNodeDefinition.md)
