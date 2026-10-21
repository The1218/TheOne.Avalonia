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
| [`static string WorkflowSerializer.Save(WorkflowDocument document)`](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L13) | 以当前格式保存完整设计，不保存视觉对象、运行状态或撤销历史。 |
| [`static WorkflowDocument WorkflowSerializer.Load(string json)`](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L38) | 读取版本 1 或 2；非法身份和字段抛出 JsonException，失效图引用保留给 Validate 报告。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`const int WorkflowSerializer.CurrentVersion`](../../../TheOne.Avalonia/Workflow/WorkflowSerializer.cs#L10) | 当前快照格式；版本 2 增加连线字段，仍可读取版本 1。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowDocument`](TheOne.Avalonia.Workflow.WorkflowDocument.md)
