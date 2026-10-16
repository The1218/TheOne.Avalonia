# RelationGraphNode

[English](../en/TheOne.Avalonia.Controls.RelationGraphNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RelationGraphNode.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RelationGraphNode` · Class · `object`

The default model for a single-root relation tree.

[Source / 声明](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Children`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L18) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.RelationGraphNode>?` | 可写; CLR / n/a | `new ObservableCollection<RelationGraphNode>()` | 子级模型集合；保持父子与对象身份，避免循环和跨父级共享可变节点。 |
| [`Content`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L17) | `object?` | 可写; CLR / n/a | `null` | 宿主内容或业务展示对象；具体模板作用域见内容模板契约，不把视图对象当作持久化数据。 |
| [`Icon`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L15) | `object?` | 可写; CLR / n/a | `null` | 图标内容或矢量几何，具体类型见签名；外部位图所有权仍由宿主负责。 |
| [`Title`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L16) | `object?` | 可写; CLR / n/a | `null` | 显示标题；不等于业务主键或本地化资源键。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`RelationGraphNode.RelationGraphNode()`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L8) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event PropertyChangedEventHandler? RelationGraphNode.PropertyChanged`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L19) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
