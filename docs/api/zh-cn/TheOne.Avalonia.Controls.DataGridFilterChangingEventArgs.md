# DataGridFilterChangingEventArgs

[English](../en/TheOne.Avalonia.Controls.DataGridFilterChangingEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridFilterChangingEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridFilterChangingEventArgs` · Class · `Avalonia.Interactivity.CancelRoutedEventArgs`

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Column`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L13) | `Avalonia.Controls.DataGridColumn` | 只读; CLR / n/a | `column` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`NewCondition`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L15) | `TheOne.Avalonia.Controls.DataGridFilterCondition?` | 只读; CLR / n/a | `newCondition` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`OldCondition`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L14) | `TheOne.Avalonia.Controls.DataGridFilterCondition?` | 只读; CLR / n/a | `oldCondition` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`DataGridFilterChangingEventArgs.DataGridFilterChangingEventArgs(DataGridColumn column, DataGridFilterCondition? oldCondition, DataGridFilterCondition? newCondition)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L10) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.DataGridFilterCondition`](TheOne.Avalonia.Controls.DataGridFilterCondition.md)
