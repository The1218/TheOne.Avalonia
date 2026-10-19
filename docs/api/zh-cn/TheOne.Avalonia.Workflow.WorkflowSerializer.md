# WorkflowSerializer

[English](../en/TheOne.Avalonia.Workflow.WorkflowSerializer.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowSerializer.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowSerializer` · Class · `object`

Versioned, source-generated JSON. Templates, Content, delegates and history are not persisted.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static string WorkflowSerializer.Save(WorkflowDocument document)`](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L9) | 将业务文档保存为可移植 JSON；不包含运行期凭据、视觉对象或非持久化状态。 |
| [`static WorkflowDocument WorkflowSerializer.Load(string json)`](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L30) | 从 JSON 载入类型化业务文档；版本、字段及结构校验见对应序列化契约。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowDocument`](TheOne.Avalonia.Workflow.WorkflowDocument.md)
