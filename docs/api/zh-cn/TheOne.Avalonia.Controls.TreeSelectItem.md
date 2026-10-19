# TreeSelectItem

[English](../en/TheOne.Avalonia.Controls.TreeSelectItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TreeSelectItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TreeSelectItem` · Class · `object`

Read-only row state. Item is the original application model, never a copied model.

[Source / 声明](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanRemove`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L28) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L33) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; CLR / n/a | `accessor-defined / state` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`Depth`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L21) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 节点深度，根级从 0 开始；用于计算缩进。 |
| [`DisplayContent`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L32) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ExpandText`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L30) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ExpansionGlyph`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L29) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasChildren`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L23) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 节点是否存在子项；用于显示展开入口，不代表当前已展开。 |
| [`Indent`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L22) | `Avalonia.Thickness` | 只读; CLR / n/a | `accessor-defined / state` | 每一级层次增加的 DIP 缩进；不改变父子关系。 |
| [`IsChecked`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L26) | `bool?` | 只读; CLR / n/a | `accessor-defined / state` | 报表复选框元素的勾选状态。 |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L24) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 业务可用性；不可用项的选择、移除和命令行为遵循控件禁用规则。 |
| [`IsExpanded`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L25) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 当前节点展开状态；需要在回收或数据重新加载后保留时，将其绑定到稳定模型。 |
| [`Item`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L18) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Key`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L19) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 字段／参数键，用于关联业务值；与显示名称分开维护。 |
| [`RemoveText`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L31) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ShowCheckBox`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L27) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`TagContent`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L34) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`TagTemplate`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L35) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; CLR / n/a | `accessor-defined / state` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`Text`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L20) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event PropertyChangedEventHandler? TreeSelectItem.PropertyChanged`](../../../TheOne.Avalonia/Controls/TreeSelectItem.cs#L36) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
