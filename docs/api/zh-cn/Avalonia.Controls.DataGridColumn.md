# DataGridColumn

[English](../en/Avalonia.Controls.DataGridColumn.md) · [简体中文](../zh-cn/Avalonia.Controls.DataGridColumn.md) · [API index](README.md)

`Avalonia.Controls.DataGridColumn` · Class · `Avalonia.AvaloniaObject`

TheOne additions to the native column; inherited/upstream APIs are omitted.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/DataGridColumn.cs#L28)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`SortPriority`](../../../vendor/Avalonia.Controls.DataGrid/DataGridColumn.cs#L743) | `int` | 只读; CLR / n/a | `accessor-defined / state` | Gets the one-based priority, or zero if this column is not sorted. |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void DataGridColumn.CycleSort(KeyModifiers modifiers = KeyModifiers.None)`](../../../vendor/Avalonia.Controls.DataGrid/DataGridColumn.cs#L734) | Cycles unsorted, ascending, descending, then unsorted through the native edit pipeline. |
