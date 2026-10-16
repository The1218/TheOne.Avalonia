# NumericFilterCondition

[English](../en/TheOne.Avalonia.Controls.NumericFilterCondition_TNumber_.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NumericFilterCondition_TNumber_.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NumericFilterCondition<TNumber>` · Class · `TheOne.Avalonia.Controls.DataGridFilterCondition`

[继承的额外成员: DataGridFilterCondition](TheOne.Avalonia.Controls.DataGridFilterCondition.md)

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L51)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsEmpty`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L66) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Maximum`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L65) | `TNumber?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Minimum`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L64) | `TNumber?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Mode`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L62) | `TheOne.Avalonia.Controls.NumericFilterMode` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 可选值：`Exact`, `Range` |
| [`Value`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L63) | `TNumber?` | 只读; CLR / n/a | `accessor-defined / state` | 当前值；单位、范围及空值含义以所属模型和校验条件为准。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override bool NumericFilterCondition<TNumber>.Matches(object? value)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L67) | 判断输入值是否符合此筛选条件；具体空值和比较规则见条件类型。 |
| [`NumericFilterCondition<TNumber>.NumericFilterCondition(NumericFilterMode mode, TNumber? value = null, TNumber? minimum = null, TNumber? maximum = null)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L53) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.NumericFilterMode`](TheOne.Avalonia.Controls.NumericFilterMode.md)
