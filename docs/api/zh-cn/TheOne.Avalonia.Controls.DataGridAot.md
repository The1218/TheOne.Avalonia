# DataGridAot

[English](../en/TheOne.Avalonia.Controls.DataGridAot.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridAot.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridAot` · Class · `object`

Registers public row properties and the parameterless constructor used by the native DataGrid. Compiled column bindings alone do not retain metadata needed for editing, sorting and grouping.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridAot.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static Type DataGridAot.PreserveRowType<T>()`](../../../TheOne.Avalonia/Controls/DataGridAot.cs#L13) | Call before creating views or grids for each row or nested property type. Only that type's public properties and public parameterless constructor are retained. |
