# DataGridTreeCollectionView

[English](../en/Avalonia.Collections.DataGridTreeCollectionView_T_.md) · [简体中文](../zh-cn/Avalonia.Collections.DataGridTreeCollectionView_T_.md) · [API index](README.md)

`Avalonia.Collections.DataGridTreeCollectionView<T>` · Class · `Avalonia.Collections.DataGridCollectionView`

Local tree projection with sibling sorting, ancestor-preserving filtering, native editing and currency. Grouping, paging and view-based structural edits are unsupported.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanAddNew`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L80) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanChangePage`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L86) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanFilter`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L83) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanGroup`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L85) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanRemove`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L81) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanSort`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L82) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Filter`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L55) | `System.Func<object, bool>` | 可写; CLR / n/a | `accessor-defined / state` | Direct match predicate. Ancestors of matches remain visible without matching this predicate. |
| [`LoadedItems`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L96) | `System.Collections.IEnumerable` | 只读; CLR / n/a | `accessor-defined / state` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`PageSize`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L87) | `int` | 可写; CLR / n/a | `accessor-defined / state` | 每页行数；树形视图不支持分页，此成员遵循该视图的限制。 |
| [`QueryError`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L25) | `string?` | 只读; CLR / n/a | `accessor-defined / state` | Failure of a live query or sort change. The last valid projection is retained. |
| [`SortDescriptions`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L27) | `Avalonia.Collections.DataGridSortDescriptionCollection` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override IDisposable DataGridTreeCollectionView<T>.DeferRefresh()`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L73) | 延迟集合刷新并合并批量变更；在 using 范围结束时释放返回的刷新令牌。 |
| [`bool DataGridTreeCollectionView<T>.IsFilterMatch(object item)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L97) | 查询原始节点是否直接命中当前筛选条件。 |
| [`bool DataGridTreeCollectionView<T>.IsLeaf(object item)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L98) | 查询原始节点是否为没有子项的叶节点。 |
| [`bool DataGridTreeCollectionView<T>.TryGetNodeState(object item, out DataGridTreeNodeState state)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L100) | 尝试读取原始节点的层级、父级及展开状态；返回 false 表示未找到。 |
| [`bool DataGridTreeCollectionView<T>.TrySetExpanded(object item, bool expanded)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L112) | 尝试修改原始节点展开状态；返回值表示是否成功，仍遵守树状态限制。 |
| [`bool DataGridTreeCollectionView<T>.RefreshTree()`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L135) | Refreshes plain collections or reconnects replaced child collections. Returns false when an edit or a cancellable presentation change prevents refresh. |
| [`void DataGridTreeCollectionView<T>.Dispose()`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L252) | 解除订阅并释放实例持有的资源；释放后不再将其作为活动视图使用。 |
| [`DataGridTreeCollectionView<T>.DataGridTreeCollectionView(IEnumerable<T> roots, Func<T, IEnumerable<T>?> childrenSelector)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L61) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<DataGridTreeChangingEventArgs>? DataGridTreeCollectionView<T>.TreeChanging`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L93) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`event EventHandler? DataGridTreeCollectionView<T>.TreeChanged`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L94) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`event EventHandler? DataGridTreeCollectionView<T>.LoadedItemsChanged`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L95) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 相关 API 与内置模型

- [`Avalonia.Collections.DataGridTreeChangingEventArgs`](Avalonia.Collections.DataGridTreeChangingEventArgs.md)
- [`Avalonia.Collections.DataGridTreeNodeState`](Avalonia.Collections.DataGridTreeNodeState.md)
