# DataGridRowMetadata

[English](../en/Avalonia.Controls.DataGridRowMetadata.md) · [简体中文](../zh-cn/Avalonia.Controls.DataGridRowMetadata.md) · [API index](README.md)

`Avalonia.Controls.DataGridRowMetadata` · Class · `object`

Explicit metadata for native DataGrid editing, path sorting and automatic columns.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/DataGridRowMetadata.cs#L14)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static Type DataGridRowMetadata.Register<T>()`](../../../vendor/Avalonia.Controls.DataGrid/DataGridRowMetadata.cs#L23) | Registers only public properties and the parameterless constructor of a known row type. Register nested property and custom comparable types separately. |
| [`static void DataGridRowMetadata.RegisterCollection<TCollection, TRow>() where TCollection : IEnumerable<TRow>`](../../../vendor/Avalonia.Controls.DataGrid/DataGridRowMetadata.cs#L37) | Registers an additional collection shape, including empty custom collections. |
