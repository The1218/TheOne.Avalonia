# NavPage

[English](../en/TheOne.Avalonia.Controls.NavPage.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavPage.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavPage` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

Navigation shell with retained, closable card pages and a collapsible sidebar.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavPage.cs#L16)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ChildrenBinding`](../../../TheOne.Avalonia/Controls/NavPage.cs#L57) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取菜单子集合的原生绑定；须使用具体模型的编译绑定，不用反射路径。 |
| [`CollapsedWidth`](../../../TheOne.Avalonia/Controls/NavPage.cs#L68) | `double` | 可写; OneWay | `64` | NavPage 的 CollapsedWidth 配置字段；值类型和初值如表，通过 CollapsedWidthProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 校验：`v => v > 0 && double.IsFinite(v)` |
| [`DefaultHome`](../../../TheOne.Avalonia/Controls/NavPage.cs#L49) | `TheOne.Avalonia.Controls.NavPageModel?` | 可写; OneWay | `null` | NavPage 的 DefaultHome 配置字段；值类型和初值如表，通过 DefaultHomeProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`ExpandedWidth`](../../../TheOne.Avalonia/Controls/NavPage.cs#L67) | `double` | 可写; OneWay | `260` | NavPage 的 ExpandedWidth 配置字段；值类型和初值如表，通过 ExpandedWidthProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 校验：`v => v > 0 && double.IsFinite(v)` |
| [`FooterTemplate`](../../../TheOne.Avalonia/Controls/NavPage.cs#L60) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`HeaderIcon`](../../../TheOne.Avalonia/Controls/NavPage.cs#L64) | `object?` | 可写; OneWay | `null` | 标题区图标内容，与标题文本分开定制。 |
| [`HeaderTemplate`](../../../TheOne.Avalonia/Controls/NavPage.cs#L59) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`HeaderTitle`](../../../TheOne.Avalonia/Controls/NavPage.cs#L63) | `string` | 可写; OneWay | `"THE ONE"` | NavPage 的 HeaderTitle 配置字段；值类型和初值如表，通过 HeaderTitleProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`IconBinding`](../../../TheOne.Avalonia/Controls/NavPage.cs#L55) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | NavPage 的 IconBinding 配置字段；值类型和初值如表，通过 IconBindingProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`IsCollapsed`](../../../TheOne.Avalonia/Controls/NavPage.cs#L65) | `bool` | 可写; TwoWay | `false` | 切换菜单为紧凑图标栏；子菜单通过悬浮弹层展示，保留当前选择。 |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/NavPage.cs#L48) | `System.Collections.IEnumerable?` | 可写; OneWay | `null` | 绑定原始模型集合，不预建大量 Control；动态增删使用带通知的集合，并提供有限视口。 |
| [`PageTemplate`](../../../TheOne.Avalonia/Controls/NavPage.cs#L58) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`RightHeader`](../../../TheOne.Avalonia/Controls/NavPage.cs#L61) | `object?` | 可写; OneWay | `null` | NavPage 的 RightHeader 配置字段；值类型和初值如表，通过 RightHeaderProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`RightHeaderTemplate`](../../../TheOne.Avalonia/Controls/NavPage.cs#L62) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`SelectedPage`](../../../TheOne.Avalonia/Controls/NavPage.cs#L70) | `object?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ShowItemIcon`](../../../TheOne.Avalonia/Controls/NavPage.cs#L66) | `bool` | 可写; OneWay | `true` | NavPage 的 ShowItemIcon 配置字段；值类型和初值如表，通过 ShowItemIconProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`TitleBinding`](../../../TheOne.Avalonia/Controls/NavPage.cs#L53) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | NavPage 的 TitleBinding 配置字段；值类型和初值如表，通过 TitleBindingProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`TransitionDuration`](../../../TheOne.Avalonia/Controls/NavPage.cs#L69) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(200)` | 展开／切换过渡时长；Motion.Enabled=False 时不执行动画。 校验：`v => v >= TimeSpan.Zero` |
| [`ViewModelBinding`](../../../TheOne.Avalonia/Controls/NavPage.cs#L51) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | NavPage 的 ViewModelBinding 配置字段；值类型和初值如表，通过 ViewModelBindingProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`bool NavPage.OpenPage(object source)`](../../../TheOne.Avalonia/Controls/NavPage.cs#L233) | Opens once per model identity and activates the retained page. Null view models are ignored. |
| [`bool NavPage.ClosePage(object source)`](../../../TheOne.Avalonia/Controls/NavPage.cs#L279) | 签名列明参数类型、可选初值及返回类型；具体操作与异常边界见所属控件用法和链接处实现。 |
| [`int NavPage.CloseAllPages()`](../../../TheOne.Avalonia/Controls/NavPage.cs#L280) | 签名列明参数类型、可选初值及返回类型；具体操作与异常边界见所属控件用法和链接处实现。 |
| [`NavPage.NavPage()`](../../../TheOne.Avalonia/Controls/NavPage.cs#L16) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IEnumerable?> NavPage.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<NavPageModel?> NavPage.DefaultHomeProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> NavPage.ViewModelBindingProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> NavPage.TitleBindingProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> NavPage.IconBindingProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> NavPage.ChildrenBindingProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> NavPage.PageTemplateProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> NavPage.HeaderTemplateProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> NavPage.FooterTemplateProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> NavPage.RightHeaderProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> NavPage.RightHeaderTemplateProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> NavPage.HeaderTitleProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> NavPage.HeaderIconProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> NavPage.IsCollapsedProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> NavPage.ShowItemIconProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> NavPage.ExpandedWidthProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> NavPage.CollapsedWidthProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TimeSpan> NavPage.TransitionDurationProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<NavPage, object?> NavPage.SelectedPageProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L36) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.NavPageModel`](TheOne.Avalonia.Controls.NavPageModel.md)
