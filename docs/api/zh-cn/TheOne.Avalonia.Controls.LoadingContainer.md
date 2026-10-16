# LoadingContainer

[English](../en/TheOne.Avalonia.Controls.LoadingContainer.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.LoadingContainer.md) · [API index](README.md)

`TheOne.Avalonia.Controls.LoadingContainer` · Class · `Avalonia.Controls.ContentControl`

Hosts content with a modal, theme-aware loading overlay without replacing its DataContext or layout.

[Source / 声明](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L12)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`HasCustomLoadingIcon`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L31) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasLoadingText`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L33) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L24) | `bool` | 可写; OneWay | `false` | 显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。 |
| [`LoadingIcon`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L28) | `object?` | 可写; OneWay | `null` | 加载状态图标；为空时沿用默认指示器。 |
| [`LoadingIconSize`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L30) | `double` | 可写; OneWay | `28` | 加载图标的 DIP 尺寸；不控制覆盖层的面积。 校验：`value => double.IsFinite(value) && value > 0` |
| [`LoadingIconTemplate`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L29) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`LoadingText`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L25) | `string?` | 可写; OneWay | `"正在加载…"` | 加载中的说明文本，可本地化；不代表任务完成结果。 |
| [`OverlayBrush`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L27) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 加载覆盖层画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`OverlayOpacity`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L26) | `double` | 可写; OneWay | `.65` | 覆盖层不透明度；范围及有限数约束见校验列。 校验：`value => double.IsFinite(value) && value is >= 0 and <= 1` |
| [`ShowDefaultLoadingIndicator`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L32) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`LoadingContainer.LoadingContainer()`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L34) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<bool> LoadingContainer.IsLoadingProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> LoadingContainer.LoadingTextProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> LoadingContainer.OverlayOpacityProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> LoadingContainer.OverlayBrushProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> LoadingContainer.LoadingIconProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> LoadingContainer.LoadingIconTemplateProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> LoadingContainer.LoadingIconSizeProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<LoadingContainer, bool> LoadingContainer.HasCustomLoadingIconProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<LoadingContainer, bool> LoadingContainer.ShowDefaultLoadingIndicatorProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<LoadingContainer, bool> LoadingContainer.HasLoadingTextProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
