# ChartHierarchyNode

[English](../en/TheOne.Avalonia.Controls.ChartHierarchyNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartHierarchyNode.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartHierarchyNode` · Class · `TheOne.Avalonia.Controls.ChartPoint`

[继承的额外成员: ChartPoint](TheOne.Avalonia.Controls.ChartPoint.md)

A hierarchy item. Label names the item; Y supplies a positive leaf weight.

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Brush`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L31) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | Optional branch color; descendants inherit the nearest explicitly assigned brush. |
| [`Children`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L29) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartHierarchyNode>?` | 可写; OneWay | `null` | Child weights replace the parent's Y value when this collection is nonempty. |
| [`Key`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L27) | `string?` | 可写; OneWay | `null` | Optional application identity. Unique keys can retain selection across source replacement. |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ChartHierarchyNode.ChartHierarchyNode()`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L17) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
| [`ChartHierarchyNode.ChartHierarchyNode(string? label, double value, string? key = null)`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L19) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string?> ChartHierarchyNode.KeyProperty`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L10) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IList<ChartHierarchyNode>?> ChartHierarchyNode.ChildrenProperty`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> ChartHierarchyNode.BrushProperty`](../../../TheOne.Avalonia/Controls/ChartHierarchyData.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
