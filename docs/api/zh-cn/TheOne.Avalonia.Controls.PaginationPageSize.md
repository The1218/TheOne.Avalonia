# PaginationPageSize

[English](../en/TheOne.Avalonia.Controls.PaginationPageSize.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.PaginationPageSize.md) · [API index](README.md)

`TheOne.Avalonia.Controls.PaginationPageSize` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L21)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Text`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L21) | `string` | 可写; CLR / init-only | `required constructor argument` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |
| [`Value`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L21) | `int` | 可写; CLR / init-only | `required constructor argument` | 当前值；单位、范围及空值含义以所属模型和校验条件为准。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`PaginationPageSize.PaginationPageSize(int Value, string Text)`](../../../TheOne.Avalonia/Controls/PaginationModels.cs#L21) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
