# DataGridFilterCondition

[English](../en/TheOne.Avalonia.Controls.DataGridFilterCondition.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridFilterCondition.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridFilterCondition` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L45)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsEmpty`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L47) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`abstract bool DataGridFilterCondition.Matches(object? value)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L48) | 判断输入值是否符合此筛选条件；具体空值和比较规则见条件类型。 |
