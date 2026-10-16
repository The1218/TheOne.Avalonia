# RelationGraphNodePresenter

[English](../en/TheOne.Avalonia.Controls.RelationGraphNodePresenter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RelationGraphNodePresenter.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RelationGraphNodePresenter` · Class · `Avalonia.Controls.Primitives.HeaderedContentControl`

A native node card. Header displays the model's Title.

[Source / 声明](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L15)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentForeground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L39) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 节点正文文字画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`HeaderFontSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L38) | `double` | 可写; OneWay | `16` | 节点标题字号，单位 DIP，按实际文本度量参与布局。 校验：`Positive` |
| [`Icon`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L32) | `object?` | 可写; OneWay | `null` | 图标内容或矢量几何，具体类型见签名；外部位图所有权仍由宿主负责。 |
| [`IconBackground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L40) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 图标底板画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`IconForeground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L41) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 图标前景画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`IconSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L34) | `double` | 可写; OneWay | `24` | 图标绘制尺寸，单位 DIP；独立于文本字号与按钮点击区域。 校验：`Positive` |
| [`IconSpacing`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L36) | `double` | 可写; OneWay | `14` | 图标与相邻文本／内容的 DIP 间距。 校验：`NonNegative` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L33) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`IconTileSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L35) | `double` | 可写; OneWay | `44` | 图标承载底板的 DIP 尺寸，独立于图标自身尺寸。 校验：`Positive` |
| [`IsRoot`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L42) | `bool` | 可写; OneWay | `false` | 此节点是否为根节点；由关系图生成器维护。 |
| [`IsSelected`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L44) | `bool` | 只读; OneWay | `computed / state` | Selection state maintained by the owning RelationGraph. |
| [`TextSpacing`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L37) | `double` | 可写; OneWay | `6` | 标题与正文之间的 DIP 间距。 校验：`NonNegative` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`RelationGraphNodePresenter.RelationGraphNodePresenter()`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L47) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<object?> RelationGraphNodePresenter.IconProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> RelationGraphNodePresenter.IconTemplateProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraphNodePresenter.IconSizeProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraphNodePresenter.IconTileSizeProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraphNodePresenter.IconSpacingProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraphNodePresenter.TextSpacingProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraphNodePresenter.HeaderFontSizeProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> RelationGraphNodePresenter.ContentForegroundProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> RelationGraphNodePresenter.IconBackgroundProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> RelationGraphNodePresenter.IconForegroundProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> RelationGraphNodePresenter.IsRootProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<RelationGraphNodePresenter, bool> RelationGraphNodePresenter.IsSelectedProperty`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
