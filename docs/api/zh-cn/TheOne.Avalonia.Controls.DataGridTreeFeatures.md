# DataGridTreeFeatures

[English](../en/TheOne.Avalonia.Controls.DataGridTreeFeatures.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridTreeFeatures.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridTreeFeatures` · Class · `Avalonia.AvaloniaObject`

Coordinates the prototype tree projection with native editing and selection.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L12)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L14) | `bool` | 可写; OneWay; 附加属性 | `false` | 业务可用性；不可用项的选择、移除和命令行为遵循控件禁用规则。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool DataGridTreeFeatures.GetIsEnabled(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L20) | 读取附加属性。业务可用性；不可用项的选择、移除和命令行为遵循控件禁用规则。 |
| [`static void DataGridTreeFeatures.SetIsEnabled(DataGrid grid, bool value)`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L21) | 设置附加属性。业务可用性；不可用项的选择、移除和命令行为遵循控件禁用规则。 |
| [`DataGridTreeFeatures.DataGridTreeFeatures()`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L12) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<bool> DataGridTreeFeatures.IsEnabledProperty`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 相关 API 与内置模型

- [`Avalonia.Collections.DataGridTreeCollectionView<T>`](Avalonia.Collections.DataGridTreeCollectionView_T_.md)
- [`Avalonia.Collections.DataGridTreeNodeState`](Avalonia.Collections.DataGridTreeNodeState.md)
- [`Avalonia.Collections.IDataGridTreeQueryView`](Avalonia.Collections.IDataGridTreeQueryView.md)
