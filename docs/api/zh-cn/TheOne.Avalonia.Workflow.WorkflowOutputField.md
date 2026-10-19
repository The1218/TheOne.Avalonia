# WorkflowOutputField

[English](../en/TheOne.Avalonia.Workflow.WorkflowOutputField.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowOutputField.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowOutputField` · Class · `object`

Stable, host-defined data type keys; no CLR type lookup or reflection is performed.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L38)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`DataType`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L38) | `string` | 可写; CLR / init-only | `"text"` | 宿主定义的类型键，例如 flow、text、number；用于端口／引用兼容判断，不是 CLR 类型名。 |
| [`Path`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L38) | `string` | 可写; CLR / init-only | `required constructor argument` | 输出字段的稳定路径键；与数据引用中的 FieldPath 一致。 |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L38) | `string` | 可写; CLR / init-only | `required constructor argument` | 显示标题；不等于业务主键或本地化资源键。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowOutputField.WorkflowOutputField(string Path, string Title, string DataType = "text")`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L38) | Stable, host-defined data type keys; no CLR type lookup or reflection is performed. |
