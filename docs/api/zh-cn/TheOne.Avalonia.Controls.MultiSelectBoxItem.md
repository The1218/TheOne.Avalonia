# MultiSelectBoxItem

[English](../en/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.MultiSelectBoxItem` · Class · `object`

Read-only presentation state used by the default result and tag templates.

[Source / 声明](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L25)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanRemove`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L33) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L36) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; CLR / n/a | `accessor-defined / state` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`DisplayContent`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L35) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L31) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 业务可用性；不可用项的选择、移除和命令行为遵循控件禁用规则。 |
| [`IsSelected`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L32) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 当前项的选择状态；与控件选中模型保持一致，不与无关项共享状态。 |
| [`Item`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L29) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`RemoveLabel`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L34) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`TagContent`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L37) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`TagTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L38) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; CLR / n/a | `accessor-defined / state` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`Text`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L30) | `string` | 只读; CLR / n/a | `""` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event PropertyChangedEventHandler? MultiSelectBoxItem.PropertyChanged`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L39) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
