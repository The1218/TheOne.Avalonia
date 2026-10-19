# TreeSelectNode

[English](../en/TheOne.Avalonia.Controls.TreeSelectNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TreeSelectNode.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TreeSelectNode` · Class · `object`

Optional default model. Keys must be non-null, unique and stable across reloads.

[Source / 声明](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Children`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L17) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Controls.TreeSelectNode>?` | 可写; CLR / n/a | `[]` | 子级模型集合；保持父子与对象身份，避免循环和跨父级共享可变节点。 |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L16) | `bool` | 可写; CLR / n/a | `true` | 业务可用性；不可用项的选择、移除和命令行为遵循控件禁用规则。 |
| [`Key`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L14) | `object` | 可写; CLR / n/a | `Guid.NewGuid().ToString("N")` | 字段／参数键，用于关联业务值；与显示名称分开维护。 |
| [`Text`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L15) | `string` | 可写; CLR / n/a | `""` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`TreeSelectNode.TreeSelectNode()`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L8) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event PropertyChangedEventHandler? TreeSelectNode.PropertyChanged`](../../../TheOne.Avalonia/Controls/TreeSelectNode.cs#L18) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
