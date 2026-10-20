# IDataGridTreeCollectionView

[English](../en/Avalonia.Collections.IDataGridTreeCollectionView.md) · [简体中文](../zh-cn/Avalonia.Collections.IDataGridTreeCollectionView.md) · [API index](README.md)

`Avalonia.Collections.IDataGridTreeCollectionView` · Interface

Non-generic tree presentation contract; row objects remain the original models.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L30)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`bool IDataGridTreeCollectionView.TryGetNodeState(object item, out DataGridTreeNodeState state)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L34) | 尝试读取原始节点的层级、父级及展开状态；返回 false 表示未找到。 |
| [`bool IDataGridTreeCollectionView.TrySetExpanded(object item, bool expanded)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L35) | 尝试修改原始节点展开状态；返回值表示是否成功，仍遵守树状态限制。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<DataGridTreeChangingEventArgs>? IDataGridTreeCollectionView.TreeChanging`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L32) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`event EventHandler? IDataGridTreeCollectionView.TreeChanged`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L33) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 相关 API 与内置模型

- [`Avalonia.Collections.DataGridTreeChangingEventArgs`](Avalonia.Collections.DataGridTreeChangingEventArgs.md)
- [`Avalonia.Collections.DataGridTreeNodeState`](Avalonia.Collections.DataGridTreeNodeState.md)
