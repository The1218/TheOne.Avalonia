# RelationGraph

[English](../en/TheOne.Avalonia.Controls.RelationGraph.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RelationGraph.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RelationGraph` · Class · `Avalonia.Controls.Control`

A single-root tree with native node cards and directly drawn orthogonal connectors.

[Source / 声明](../../../TheOne.Avalonia/Controls/RelationGraph.Layout.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L107) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 背景画刷；主题相关背景使用 DynamicResource，避免固定颜色破坏明暗切换。 |
| [`ChildrenSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L134) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | A binding evaluated against each node, not against the graph's data context. |
| [`ConnectorBrush`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L215) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | One.Accent / 1.5 ; 连接线颜色与宽度，宽度为 0 时隐藏 |
| [`ConnectorCornerRadius`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L227) | `double` | 可写; OneWay | `8` | 8 / 3 ; 正交拐角与连接端点半径 校验：`NonNegative` |
| [`ConnectorThickness`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L221) | `double` | 可写; OneWay | `1.5` | One.Accent / 1.5 ; 连接线颜色与宽度，宽度为 0 时隐藏 校验：`NonNegative` |
| [`ContentSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L155) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取关系节点正文的类型化字段访问器；不使用字符串反射路径。 |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L173) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`IconSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L141) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取关系节点图标的类型化字段访问器。 |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L161) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`LevelSpacing`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L203) | `double` | 可写; OneWay | `80` | 80 ; 层级间距；每层按最大节点高度或宽度计算 校验：`NonNegative` |
| [`NodeCount`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L242) | `int` | 只读; OneWay | `computed / state` | 只读 ; 当前有效树的节点数量 / 数据校验错误 |
| [`NodeMinHeight`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L191) | `double` | 可写; OneWay | `88` | 260 / 88 ; 固定卡片宽度 / 自然内容高度下限 校验：`Positive` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L197) | `double` | 可写; OneWay | `36` | 36 ; 相邻子树在横截方向的间距 校验：`NonNegative` |
| [`NodeTheme`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L179) | `Avalonia.Styling.ControlTheme?` | 可写; OneWay | `null` | 默认主题 ; 替换 `RelationGraphNodePresenter` 的 ControlTheme |
| [`NodeWidth`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L185) | `double` | 可写; OneWay | `260` | 260 / 88 ; 固定卡片宽度 / 自然内容高度下限 校验：`Positive` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L126) | `Avalonia.Layout.Orientation` | 可写; OneWay | `0 / default` | Horizontal ; 树的展开方向 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |
| [`Padding`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L209) | `Avalonia.Thickness` | 可写; OneWay | `inherited registration` | 16 ; 整张图边缘留白 |
| [`PortRadius`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L233) | `double` | 可写; OneWay | `3` | 8 / 3 ; 正交拐角与连接端点半径 校验：`NonNegative` |
| [`RootNode`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L113) | `object?` | 可写; OneWay | `null` | 关系图的根模型；子节点通过模型树或访问器获取。 |
| [`SelectItem`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L120) | `object?` | 可写; TwoWay | `null` | The selected node model, matched by reference. Null clears selection. |
| [`TitleSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L148) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取关系节点标题的类型化字段访问器。 |
| [`TitleTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L167) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L240) | `string?` | 只读; OneWay | `computed / state` | 只读 ; 当前有效树的节点数量 / 数据校验错误 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override void RelationGraph.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/RelationGraph.Layout.cs#L78) | Avalonia 绘制回调；使用提供的 DrawingContext 绘制当前状态，业务代码通常不直接调用。 |
| [`RelationGraphNodePresenter? RelationGraph.GetNodePresenter(object node)`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L246) | Finds a realized native card; returns null before layout or for invalid trees. |
| [`void RelationGraph.RefreshData()`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L252) | Refresh after changing a collection that does not notify. |
| [`RelationGraph.RelationGraph()`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L98) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> RelationGraph.BackgroundProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> RelationGraph.RootNodeProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> RelationGraph.SelectItemProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Orientation> RelationGraph.OrientationProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.ChildrenSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.IconSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.TitleSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L38) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> RelationGraph.ContentSourceProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L41) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> RelationGraph.IconTemplateProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L44) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> RelationGraph.TitleTemplateProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L47) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> RelationGraph.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L50) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ControlTheme?> RelationGraph.NodeThemeProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L53) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraph.NodeWidthProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L56) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraph.NodeMinHeightProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L59) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraph.NodeSpacingProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L62) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraph.LevelSpacingProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L65) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Thickness> RelationGraph.PaddingProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L68) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> RelationGraph.ConnectorBrushProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L71) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraph.ConnectorThicknessProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L74) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraph.ConnectorCornerRadiusProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L77) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RelationGraph.PortRadiusProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L80) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<RelationGraph, string?> RelationGraph.ValidationErrorProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L83) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<RelationGraph, int> RelationGraph.NodeCountProperty`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L87) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.RelationGraphNode`](TheOne.Avalonia.Controls.RelationGraphNode.md)
- [`TheOne.Avalonia.Controls.RelationGraphNodePresenter`](TheOne.Avalonia.Controls.RelationGraphNodePresenter.md)
