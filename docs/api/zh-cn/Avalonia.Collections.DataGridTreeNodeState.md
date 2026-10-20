# DataGridTreeNodeState

[English](../en/Avalonia.Collections.DataGridTreeNodeState.md) · [简体中文](../zh-cn/Avalonia.Collections.DataGridTreeNodeState.md) · [API index](README.md)

`Avalonia.Collections.DataGridTreeNodeState` · Struct · `System.ValueType`

Presentation state for an original model in a local tree.

[Source / 声明](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L16)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Depth`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L16) | `int` | 可写; CLR / init-only | `required constructor argument` | 节点深度，根级从 0 开始；用于计算缩进。 |
| [`HasChildren`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L16) | `bool` | 可写; CLR / init-only | `required constructor argument` | 节点是否存在子项；用于显示展开入口，不代表当前已展开。 |
| [`IsExpanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L16) | `bool` | 可写; CLR / init-only | `required constructor argument` | 当前节点展开状态；需要在回收或数据重新加载后保留时，将其绑定到稳定模型。 |
| [`IsFilterMatch`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L19) | `bool` | 可写; CLR / init-only | `false` | 当前节点是否直接匹配树筛选条件；祖先保留与直接命中分开。 |
| [`IsForcedExpanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L18) | `bool` | 可写; CLR / init-only | `false` | 筛选是否强制展开当前节点以呈现匹配后代；与用户保存的展开状态分开。 |
| [`Parent`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L16) | `object?` | 可写; CLR / init-only | `required constructor argument` | 树节点的直接父对象；根节点为空，保持原始模型身份。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`DataGridTreeNodeState.DataGridTreeNodeState(int Depth, object? Parent, bool HasChildren, bool IsExpanded)`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L16) | Presentation state for an original model in a local tree. |
| [`DataGridTreeNodeState.DataGridTreeNodeState()`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L16) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
