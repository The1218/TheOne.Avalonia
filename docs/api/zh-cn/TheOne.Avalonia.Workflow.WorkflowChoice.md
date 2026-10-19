# WorkflowChoice

[English](../en/TheOne.Avalonia.Workflow.WorkflowChoice.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowChoice.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowChoice` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[继承的额外成员: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

Presentation for a stable key. Unresolved saved keys remain editable instead of being discarded.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L25)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsAvailable`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L29) | `bool` | 只读; CLR / n/a | `true` | 该 Key 当前是否存在于宿主候选数据源；不代表业务审批权限。 |
| [`Key`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L27) | `string` | 只读; CLR / n/a | `key` | 字段／参数键，用于关联业务值；与显示名称分开维护。 |
| [`Text`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L28) | `string` | 只读; CLR / n/a | `text` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowChoice.WorkflowChoice(string key, string text)`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L25) | Presentation for a stable key. Unresolved saved keys remain editable instead of being discarded. |
