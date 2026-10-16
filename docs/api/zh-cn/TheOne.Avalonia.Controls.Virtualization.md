# Virtualization

[English](../en/TheOne.Avalonia.Controls.Virtualization.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Virtualization.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Virtualization` · Class · `Avalonia.AvaloniaObject`

Viewport-aware selection and navigation for native tab and hierarchical controls.

[Source / 声明](../../../TheOne.Avalonia/Controls/Virtualization.cs#L13)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L15) | `bool` | 可写; OneWay; 附加属性 | `false` | 启用库支持的虚拟化方案；仍需有限视口和模型数据源，不预建全部控件。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool Virtualization.GetEnabled(ItemsControl control)`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L33) | 读取附加属性。启用库支持的虚拟化方案；仍需有限视口和模型数据源，不预建全部控件。 |
| [`static void Virtualization.SetEnabled(ItemsControl control, bool value)`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L34) | 设置附加属性。启用库支持的虚拟化方案；仍需有限视口和模型数据源，不预建全部控件。 |
| [`static TreeViewItem? Virtualization.BringIntoView(TreeView tree, params int[] path)`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L55) | Materializes only the ancestor path and target of a large tree. |
| [`static void Virtualization.SelectAll(TreeView tree)`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L75) | Selects all nodes exposed by expanded branches without creating their containers. |
| [`Virtualization.Virtualization()`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L13) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<bool> Virtualization.EnabledProperty`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
