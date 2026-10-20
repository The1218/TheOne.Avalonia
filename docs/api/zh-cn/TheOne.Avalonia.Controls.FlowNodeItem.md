# FlowNodeItem

[English](../en/TheOne.Avalonia.Controls.FlowNodeItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowNodeItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowNodeItem` · Class · `object`

A bindable flow item. Content can be default fields, a control, or a custom model.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L72)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Content`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L88) | `object?` | 可写; CLR / n/a | `null` | 宿主内容或业务展示对象；具体模板作用域见内容模板契约，不把视图对象当作持久化数据。 |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L94) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; CLR / n/a | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`Status`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L78) | `TheOne.Avalonia.Controls.FlowNodeStatus` | 可写; CLR / n/a | `0 / default` | 节点业务状态枚举；样式由状态派生，不把样式类作为状态存储。 可选值：`Pending`, `InProgress`, `Completed` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`FlowNodeItem.FlowNodeItem()`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L72) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event PropertyChangedEventHandler? FlowNodeItem.PropertyChanged`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L100) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.FlowNodeStatus`](TheOne.Avalonia.Controls.FlowNodeStatus.md)
