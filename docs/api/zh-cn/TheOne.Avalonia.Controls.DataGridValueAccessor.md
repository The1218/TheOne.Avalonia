# DataGridValueAccessor

[English](../en/TheOne.Avalonia.Controls.DataGridValueAccessor.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridValueAccessor.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridValueAccessor` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L26)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static IDataGridValueAccessor DataGridValueAccessor.Create<TItem, TValue>(Func<TItem, TValue> getter)`](../../../TheOne.Avalonia/Controls/DataGridFilterCondition.cs#L28) | 通过显式类型参数、模型及委托创建适配器／数据源；签名中的 getter 保留编译类型信息。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.IDataGridValueAccessor`](TheOne.Avalonia.Controls.IDataGridValueAccessor.md)
