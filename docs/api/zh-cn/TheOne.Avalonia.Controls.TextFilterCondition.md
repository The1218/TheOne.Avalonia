# TextFilterCondition

[English](../en/TheOne.Avalonia.Controls.TextFilterCondition.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TextFilterCondition.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TextFilterCondition` · Class · `TheOne.Avalonia.Controls.DataGridFilterCondition`

[继承的额外成员: DataGridFilterCondition](TheOne.Avalonia.Controls.DataGridFilterCondition.md)

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L37)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Comparison`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L45) | `System.StringComparison` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 可选值：`CurrentCulture`, `CurrentCultureIgnoreCase`, `InvariantCulture`, `InvariantCultureIgnoreCase`, `Ordinal`, `OrdinalIgnoreCase` |
| [`IsEmpty`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L46) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Text`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L44) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override bool TextFilterCondition.Matches(object? value)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L47) | 判断输入值是否符合此筛选条件；具体空值和比较规则见条件类型。 |
| [`TextFilterCondition.TextFilterCondition(string? text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L39) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
