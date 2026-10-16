# StepValidationError

[English](../en/TheOne.Avalonia.Controls.StepValidationError.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.StepValidationError.md) · [API index](README.md)

`TheOne.Avalonia.Controls.StepValidationError` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`MemberName`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | 继承自 `StepField`，匹配错误中的属性名，如 `Name`；不会作为反射路径执行 |
| [`Message`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | 可供用户或诊断展示的结果说明；操作结果仍以状态字段为准。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`StepValidationError.StepValidationError(string MemberName, string Message)`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
