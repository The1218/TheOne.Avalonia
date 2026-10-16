# FlowNodeContent

[English](../en/TheOne.Avalonia.Controls.FlowNodeContent.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FlowNodeContent.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FlowNodeContent` · Class · `object`

The four optional fields displayed by the default FlowNode template.

[Source / 声明](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`HasText`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L19) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasTime`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L17) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasTitle`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L18) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`State`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L15) | `string?` | 可写; CLR / n/a | `null` | 模型的状态枚举；可选值及持久化含义见对应枚举和所属控件契约。 |
| [`Text`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L16) | `string?` | 可写; CLR / n/a | `null` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |
| [`Time`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L14) | `string?` | 可写; CLR / n/a | `null` | 节点内容的时间显示文本；不参与节点执行或步骤排序。 |
| [`Title`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L13) | `string?` | 可写; CLR / n/a | `null` | 显示标题；不等于业务主键或本地化资源键。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`FlowNodeContent.FlowNodeContent()`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L10) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event PropertyChangedEventHandler? FlowNodeContent.PropertyChanged`](../../../TheOne.Avalonia/Controls/FlowNodeContent.cs#L20) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
