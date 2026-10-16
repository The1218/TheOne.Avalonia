# NavMenu

[English](../en/TheOne.Avalonia.Controls.NavMenu.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavMenu.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavMenu` · Class · `Avalonia.Controls.TreeView`

A native virtualized navigation tree with separately templated icons and labels.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L19)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ChildrenBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L68) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取菜单子集合的原生绑定；须使用具体模型的编译绑定，不用反射路径。 |
| [`CollapsedWidth`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L53) | `double` | 可写; OneWay | `52` | 44 ; 52 ; 60 校验：`Positive` |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L67) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`DisplayIconTemplate`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L60) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; OneWay | `computed / state` | The icon template including the compact-mode fallback glyph. |
| [`ExpandedBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L69) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 菜单项展开状态的绑定；绑定到稳定模型可跨回收保留展开。 |
| [`FilterBox`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L21) | `TheOne.Avalonia.Controls.NavMenuFilterBox?` | 只读; CLR / n/a | `null` | The template's native auto-complete input, available after template application. |
| [`FilterPlaceholder`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L18) | `string` | 可写; OneWay | `"搜索菜单"` | 菜单内置筛选输入的占位提示文本。 |
| [`FilterResults`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L19) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.NavMenuFilterResult>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HoverCloseDelay`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L23) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(300)` | 00:00:00.300 ; 离开锚点与弹层链后延迟关闭，进入任一级弹层或返回锚点取消待关闭操作。 校验：`ValidDelay` |
| [`HoverOpenDelay`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L22) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(100)` | 00:00:00.100 ; 快速经过图标不打开，停留后才打开；可设为零。 校验：`ValidDelay` |
| [`IconSize`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L73) | `double` | 可写; OneWay | `18` | 图标绘制尺寸，单位 DIP；独立于文本字号与按钮点击区域。 校验：`Positive` |
| [`IconSpacing`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L74) | `double` | 可写; OneWay | `10` | 图标与相邻文本／内容的 DIP 间距。 校验：`value => double.IsFinite(value) && value >= 0` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L65) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`Indent`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L75) | `double` | 可写; OneWay | `16` | 每一级层次增加的 DIP 缩进；不改变父子关系。 校验：`value => double.IsFinite(value) && value >= 0` |
| [`IsCollapsed`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L52) | `bool` | 可写; TwoWay | `false` | 切换菜单为紧凑图标栏；子菜单通过悬浮弹层展示，保留当前选择。 |
| [`IsCompactSubmenu`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L48) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsFilterOpen`](../../../TheOne.Avalonia/Controls/NavMenu.FilterPopup.cs#L15) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsGroupBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L70) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取菜单项是否为分组的绑定；分组与普通可导航项行为不同。 |
| [`IsItemEnabledBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L71) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取单项可用性的绑定；禁用项不响应用户导航选择。 |
| [`IsSearching`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L26) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsSubmenuOpen`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L56) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L72) | `double` | 可写; OneWay | `36` | 单项／滚轮单元的 DIP 高度；主题密度和显式局部值可能覆盖注册初值。 校验：`Positive` |
| [`ItemInvokedCommand`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L76) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`OpenOnHover`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L21) | `bool` | 可写; OneWay | `true` | true ; 折叠根图标和各级弹层中的分支支持悬停打开；false 时仍可点击、键盘或调用 API。 |
| [`SearchMatchCount`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L27) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SearchOwner`](../../../TheOne.Avalonia/Controls/NavMenu.SearchInput.cs#L11) | `TheOne.Avalonia.Controls.NavMenu?` | 可写; OneWay; 附加属性 | `null` | 搜索结果关联的菜单实例；供筛选弹层定位并激活原始菜单项。 |
| [`SearchText`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L24) | `string?` | 可写; TwoWay | `null` | 当前搜索文本；搜索与选中状态独立，不用查询字符串充当模型主键。 |
| [`SearchTextBinding`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L25) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取单项搜索文本的编译绑定；展示模板仍可独立定制。 |
| [`ShowFilter`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L17) | `bool` | 可写; OneWay | `false` | 显示菜单内置筛选框；关闭后仍可由宿主提供独立搜索入口。 |
| [`Submenu`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L58) | `TheOne.Avalonia.Controls.NavMenu?` | 只读; OneWay | `computed / state` | The currently open, virtualized submenu. Null when closed. |
| [`SubmenuMaxHeight`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L55) | `double` | 可写; OneWay | `320` | 240 ; 320 ; 400 校验：`Positive` |
| [`SubmenuOpensLeft`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L49) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SubmenuWidth`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L54) | `double` | 可写; OneWay | `280` | 240 ; 280 ; 320 校验：`Positive` |
| [`TransitionDuration`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L19) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(200)` | 展开／切换过渡时长；Motion.Enabled=False 时不执行动画。 校验：`value => value >= TimeSpan.Zero` |
| [`TransitionExtent`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L20) | `double` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`bool NavMenu.OpenSubmenu(int rootIndex)`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L63) | Open a collapsed root's children without selecting or expanding its model. |
| [`void NavMenu.CloseSubmenu()`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L69) | 关闭当前级联子菜单并清理其弹出状态，保留业务选择。 |
| [`bool NavMenu.OpenFilter()`](../../../TheOne.Avalonia/Controls/NavMenu.FilterPopup.cs#L64) | Focuses the inline filter, or opens its compact search popup. |
| [`void NavMenu.CloseFilter()`](../../../TheOne.Avalonia/Controls/NavMenu.FilterPopup.cs#L87) | Dismisses compact search and restores the pre-search navigation state. |
| [`static NavMenu? NavMenu.GetSearchOwner(TextBox input)`](../../../TheOne.Avalonia/Controls/NavMenu.SearchInput.cs#L12) | 读取附加属性。搜索结果关联的菜单实例；供筛选弹层定位并激活原始菜单项。 |
| [`static void NavMenu.SetSearchOwner(TextBox input, NavMenu? menu)`](../../../TheOne.Avalonia/Controls/NavMenu.SearchInput.cs#L13) | 设置附加属性。搜索结果关联的菜单实例；供筛选弹层定位并激活原始菜单项。 |
| [`bool NavMenu.SelectPath(params int[] path)`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L81) | Realize only an ancestor path, then select its target data item. |
| [`NavMenu.NavMenu()`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L45) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<NavMenuItemInvokedEventArgs>? NavMenu.ItemInvoked`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L77) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<bool> NavMenu.IsCollapsedProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> NavMenu.CollapsedWidthProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> NavMenu.SubmenuWidthProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> NavMenu.SubmenuMaxHeightProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<NavMenu, bool> NavMenu.IsSubmenuOpenProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<NavMenu, NavMenu?> NavMenu.SubmenuProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<NavMenu, IDataTemplate?> NavMenu.DisplayIconTemplateProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<NavMenu, bool> NavMenu.IsCompactSubmenuProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L45) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<NavMenu, bool> NavMenu.SubmenuOpensLeftProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L46) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> NavMenu.ShowFilterProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> NavMenu.FilterPlaceholderProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<NavMenu, IReadOnlyList<NavMenuFilterResult>> NavMenu.FilterResultsProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<NavMenu, bool> NavMenu.IsFilterOpenProperty`](../../../TheOne.Avalonia/Controls/NavMenu.FilterPopup.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> NavMenu.OpenOnHoverProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TimeSpan> NavMenu.HoverOpenDelayProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TimeSpan> NavMenu.HoverCloseDelayProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TimeSpan> NavMenu.TransitionDurationProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<NavMenu, double> NavMenu.TransitionExtentProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> NavMenu.SearchTextProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> NavMenu.SearchTextBindingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<NavMenu, bool> NavMenu.IsSearchingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<NavMenu, int> NavMenu.SearchMatchCountProperty`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<NavMenu?> NavMenu.SearchOwnerProperty`](../../../TheOne.Avalonia/Controls/NavMenu.SearchInput.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> NavMenu.IconTemplateProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> NavMenu.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> NavMenu.ChildrenBindingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> NavMenu.ExpandedBindingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> NavMenu.IsGroupBindingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<BindingBase?> NavMenu.IsItemEnabledBindingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> NavMenu.ItemHeightProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> NavMenu.IconSizeProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> NavMenu.IconSpacingProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> NavMenu.IndentProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ICommand?> NavMenu.ItemInvokedCommandProperty`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L36) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<NavMenuItemInvokedEventArgs> NavMenu.ItemInvokedEvent`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.NavMenuFilterBox`](TheOne.Avalonia.Controls.NavMenuFilterBox.md)
- [`TheOne.Avalonia.Controls.NavMenuFilterResult`](TheOne.Avalonia.Controls.NavMenuFilterResult.md)
- [`TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs`](TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs.md)
