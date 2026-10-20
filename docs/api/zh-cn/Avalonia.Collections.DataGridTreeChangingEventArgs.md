# DataGridTreeChangingEventArgs

[English](../en/Avalonia.Collections.DataGridTreeChangingEventArgs.md) · [简体中文](../zh-cn/Avalonia.Collections.DataGridTreeChangingEventArgs.md) · [API index](README.md)

`Avalonia.Collections.DataGridTreeChangingEventArgs` · Class · `System.ComponentModel.CancelEventArgs`

A cancellable change to the visible tree. Item is null for a source refresh.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L23)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Expanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L26) | `bool?` | 只读; CLR / n/a | `expanded` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Item`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L25) | `object?` | 只读; CLR / n/a | `item` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`DataGridTreeChangingEventArgs.DataGridTreeChangingEventArgs(object? item, bool? expanded)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L23) | A cancellable change to the visible tree. Item is null for a source refresh. |
