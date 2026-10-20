# IDataGridValueAccessor

[English](../en/TheOne.Avalonia.Controls.IDataGridValueAccessor.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IDataGridValueAccessor.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IDataGridValueAccessor` · Interface

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L20)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ValueType`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L22) | `System.Type` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`object? IDataGridValueAccessor.GetValue(object item)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L23) | 通过显式字段键／类型化 getter 从原始行读取值；不使用反射路径。 |
