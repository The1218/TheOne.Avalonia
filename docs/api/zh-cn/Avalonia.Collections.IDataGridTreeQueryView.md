# IDataGridTreeQueryView

[English](../en/Avalonia.Collections.IDataGridTreeQueryView.md) · [简体中文](../zh-cn/Avalonia.Collections.IDataGridTreeQueryView.md) · [API index](README.md)

`Avalonia.Collections.IDataGridTreeQueryView` · Interface

Optional query/summary contract, separate from the existing tree presentation API.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L39)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`LoadedItems`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L41) | `System.Collections.IEnumerable` | 只读; CLR / n/a | `accessor-defined / state` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`bool IDataGridTreeQueryView.IsFilterMatch(object item)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L43) | 查询原始节点是否直接命中当前筛选条件。 |
| [`bool IDataGridTreeQueryView.IsLeaf(object item)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L44) | 查询原始节点是否为没有子项的叶节点。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler? IDataGridTreeQueryView.LoadedItemsChanged`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L42) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
