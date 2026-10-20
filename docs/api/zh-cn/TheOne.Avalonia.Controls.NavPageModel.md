# NavPageModel

[English](../en/TheOne.Avalonia.Controls.NavPageModel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavPageModel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavPageModel` · Class · `Avalonia.AvaloniaObject`

Default navigation data. ViewModel is resolved by the host's data templates.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Children`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L19) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Controls.NavPageModel>` | 只读; CLR / n/a | `[]` | 子级模型集合；保持父子与对象身份，避免循环和跨父级共享可变节点。 |
| [`Icon`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L15) | `object?` | 可写; OneWay | `null` | 图标内容或矢量几何，具体类型见签名；外部位图所有权仍由宿主负责。 |
| [`IsClosable`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L17) | `bool` | 可写; OneWay | `true` | NavPageModel 的 IsClosable 配置字段；值类型和初值如表，通过 IsClosableProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`IsPinned`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L18) | `bool` | 可写; OneWay | `false` | NavPageModel 的 IsPinned 配置字段；值类型和初值如表，通过 IsPinnedProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Title`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L14) | `string` | 可写; OneWay | `""` | 显示标题；不等于业务主键或本地化资源键。 |
| [`ViewModel`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L16) | `object?` | 可写; OneWay | `null` | NavPageModel 的 ViewModel 配置字段；值类型和初值如表，通过 ViewModelProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`NavPageModel.NavPageModel()`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L7) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string> NavPageModel.TitleProperty`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L9) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> NavPageModel.IconProperty`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L10) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> NavPageModel.ViewModelProperty`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> NavPageModel.IsClosableProperty`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> NavPageModel.IsPinnedProperty`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
