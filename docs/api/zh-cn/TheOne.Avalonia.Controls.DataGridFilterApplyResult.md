# DataGridFilterApplyResult

[English](../en/TheOne.Avalonia.Controls.DataGridFilterApplyResult.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridFilterApplyResult.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridFilterApplyResult` · Struct · `System.ValueType`

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Error`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L8) | `string?` | 可写; CLR / init-only | `null` | 操作失败的结构化／文本信息；与成功标志一起检查，不忽略失败。 |
| [`Succeeded`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L8) | `bool` | 可写; CLR / init-only | `required constructor argument` | 本次操作是否成功；失败时读取 Error，不把空结果等同于成功。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`DataGridFilterApplyResult.DataGridFilterApplyResult(bool Succeeded, string? Error = null)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L8) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
| [`DataGridFilterApplyResult.DataGridFilterApplyResult()`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L8) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
