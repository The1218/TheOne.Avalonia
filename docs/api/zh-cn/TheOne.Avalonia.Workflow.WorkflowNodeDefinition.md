# WorkflowNodeDefinition

[English](../en/TheOne.Avalonia.Workflow.WorkflowNodeDefinition.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeDefinition.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowNodeDefinition` · Class · `object`

The host supplies node kinds and factories. Returned instances must have fresh IDs.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L96)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Category`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L101) | `string` | 可写; CLR / init-only | `"基础节点"` | 节点库的分类名称；用于组织定义列表。 |
| [`Content`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L102) | `object?` | 可写; CLR / init-only | `null` | 宿主内容或业务展示对象；具体模板作用域见内容模板契约，不把视图对象当作持久化数据。 |
| [`CreateNode`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L103) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode>` | 可写; CLR / init-only | `null` | 创建新节点模型的宿主工厂委托；每次返回独立实例并保留类型键。 |
| [`Description`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L100) | `string` | 可写; CLR / init-only | `""` | 描述文本；由宿主提供业务说明，未提供时沿用初值。 |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L99) | `string` | 可写; CLR / init-only | `null` | 显示标题；不等于业务主键或本地化资源键。 |
| [`TypeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L98) | `string` | 可写; CLR / init-only | `null` | 宿主节点定义的稳定类型键；加载后用它恢复定义和非序列化内容，不进行 CLR 类型查找。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowNodeDefinition.WorkflowNodeDefinition()`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L96) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowNode`](TheOne.Avalonia.Workflow.WorkflowNode.md)
