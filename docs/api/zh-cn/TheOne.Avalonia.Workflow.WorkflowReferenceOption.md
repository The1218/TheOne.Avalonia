# WorkflowReferenceOption

[English](../en/TheOne.Avalonia.Workflow.WorkflowReferenceOption.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowReferenceOption.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowReferenceOption` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L19)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Reference`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L19) | `TheOne.Avalonia.Workflow.WorkflowDataReference` | 可写; CLR / init-only | `required constructor argument` | 指向上游输出的结构化引用；空值表示使用固定值，不保存解析后的运行数据。 |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L19) | `string` | 可写; CLR / init-only | `required constructor argument` | 显示标题；不等于业务主键或本地化资源键。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowReferenceOption.WorkflowReferenceOption(string Title, WorkflowDataReference Reference)`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L19) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowDataReference`](TheOne.Avalonia.Workflow.WorkflowDataReference.md)
