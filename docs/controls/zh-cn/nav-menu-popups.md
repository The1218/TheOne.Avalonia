# NavMenu 侧栏与级联弹层

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

**简体中文** · [English](../en/nav-menu-popups.md)

基础接入见[NavMenu](nav-menu.md)。

## 折叠侧栏与子菜单弹层

设置 `IsCollapsed="True"` 后，主导航只显示根菜单图标，隐藏文字、缩进、行内箭头及子级区域。没有 IconTemplate 时提供默认图形，避免把模型文字挤进图标栏；有子级的图标默认悬停打开菜单；没有子级的图标仍通过原内容模板显示提示。折叠不修改模型的 IsExpanded 或当前 SelectedItem。

```xml
<one:NavMenu IsCollapsed="{Binding NavigationCollapsed, Mode=TwoWay}"
             ItemsSource="{Binding MenuRoots}"
             ChildrenBinding="{Binding Entries}"
             ExpandedBinding="{Binding IsExpanded, Mode=TwoWay}"
             SelectedItem="{Binding SelectedMenu, Mode=TwoWay}"
             IconTemplate="{StaticResource MenuIconTemplate}"
             ContentTemplate="{StaticResource MenuContentTemplate}" />
```

悬停有子级的根图标默认等待 100ms 后打开右侧弹层；点击图标或在其获得焦点后按 Right / Enter / Space 立即打开。没有子级的普通项目直接导航；分组图标打开子级，不替换当前页面。每一级弹层使用有界 NavMenu，仅展示该级集合。悬停、点击右侧箭头或按 Right 继续打开下一层；不会展开行内子项，也不修改模型 IsExpanded。每级独立按视口创建与回收容器。

弹层中的选择同步到根导航，调用叶子项时只执行一次根 ItemInvoked / ItemInvokedCommand，然后关闭。选中子级时根图标保持强调状态。Left / Esc 关闭当前一级并将键盘焦点返回上一层锚点；点击弹层外、展开侧栏、禁用父级、根数据源替换、锚点容器回收或控件卸载都会关闭并释放弹层；显式打开的弹层关闭后焦点返回锚点；悬停打开不抢占当前焦点。关闭再打开时，双向模型绑定和同一模型身份的默认弱缓存保留展开状态。

弹层使用 Avalonia 原生 Flyout 的窗口内 overlay 宿主，位置受当前窗口约束。主题、密度、图标模板、内容模板、行高及 Motion.Enabled 在弹层打开期间也能实时更新。折叠侧栏本身立即隐藏行内子级，弹层继承 Motion.Enabled，悬停延迟独立于动效开关，关闭动效后仍可悬停导航。

| 属性 | Small | Normal | Large |
| --- | ---: | ---: | ---: |
| CollapsedWidth | 44 | 52 | 60 |
| SubmenuWidth | 240 | 280 | 320 |
| SubmenuMaxHeight | 240 | 320 | 400 |

以上属性支持 Style、绑定或局部覆盖。折叠宽度由主题设置 Width；若消费方显式设置了固定 Width，局部值会优先，应同时调整容器列宽和控件 Width。父级可用 Grid 的 Auto 列，让图标栏按实际密度占用空间。

```csharp
menu.OpenSubmenu(0);        // 打开第一个根项目的子级。
menu.SelectPath(0, 0, 999); // 折叠模式下自动打开弹层，并定位三级末项。
menu.CloseSubmenu();
```

IsSubmenuOpen 与 Submenu 是只读可观察属性；Submenu 在打开时指向当前一级的虚拟化子导航，关闭后为 null。继续读取 Submenu.Submenu 可观察已打开的级联层。不要保存其容器引用作为模型状态。

PreviewHost 的标题栏“☰”可切换真实主导航；扩展控件 → NavMenu 页面提供折叠开关、打开／关闭弹层、悬停开关、千条根／二级／三级末项定位及各级实时容器数。顶部尺寸和主题切换可在弹层打开时验证。定向导出覆盖 NavMenu 展开／级联、边缘翻转与窄窗口 × 三尺寸 × 明暗主题，共 48 张当前控件图片（含多级搜索场景和动效中间帧）；导出 PreviewHost 时另有主导航截图。

## 悬停与级联配置

| 属性 | 默认值 | 行为 |
| --- | --- | --- |
| OpenOnHover | true | 折叠根图标和各级弹层中的分支支持悬停打开；false 时仍可点击、键盘或调用 API。 |
| HoverOpenDelay | 00:00:00.100 | 快速经过图标不打开，停留后才打开；可设为零。 |
| HoverCloseDelay | 00:00:00.300 | 离开锚点与弹层链后延迟关闭，进入任一级弹层或返回锚点取消待关闭操作。 |

这些 StyledProperty 支持 Style、编译绑定和局部值；延迟非负，自动传递到所有级联层。移到兄弟叶子项后只关闭旧的下一级；移到兄弟分支后替换对应子弹层。移动经过锚点与弹层的间隙时有关闭缓冲。触摸点击沿用显式操作，不触发鼠标悬停计时。

调用最深层叶子只向根导航发送一次 ItemInvoked / ItemInvokedCommand，并关闭整条链。锚点回收、禁用、移除最后一个子项、数据源替换和控件卸载会清理对应弹层及计时器，防止旧模型在延迟结束后重新弹出。

专项鼠标验证：

## 窗口边缘与窄窗口

弹层优先沿上一层方向打开。空间不足且另一侧不会覆盖可见祖先菜单时自动翻转；根图标旁的弹层也会按剩余空间缩窄。宽高与位置限制在窗口客户区内，实时缩放时重新计算。

无法并排容纳时采用单级展示：暂时隐藏父层并关闭其命中测试，下一层显示返回按钮。返回恢复父层的选择和滚动状态；鼠标停在返回后露出的原分支时不会立即重开，离开再进入恢复悬停。Left / Esc 同样逐级返回。加宽窗口后恢复级联。

只读 Avalonia 属性 IsCompactSubmenu 和 SubmenuOpensLeft 可用于观察布局。鼠标移动按可见弹层链判断，布局改变时保留关闭缓冲，避免翻转或缩放后误关闭。每一级仍保留有限视口与容器回收。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### NavMenu

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ChildrenBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L119) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取菜单子集合的原生绑定；须使用具体模型的编译绑定，不用反射路径。 |
| [`CollapsedWidth`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L89) | `double` | 可写; OneWay | `52` | 44 ; 52 ; 60 校验：`Positive` |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L111) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`DisplayIconTemplate`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L113) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; OneWay | `computed / state` | The icon template including the compact-mode fallback glyph. |
| [`DisplayWidth`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L31) | `double` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ExpandedBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L127) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 菜单项展开状态的绑定；绑定到稳定模型可跨回收保留展开。 |
| [`ExpandedWidth`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L17) | `double` | 可写; OneWay | `280` | NavMenu 的 ExpandedWidth 配置字段；值类型和初值如表，通过 ExpandedWidthProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 校验：`Positive` |
| [`FilterBox`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L46) | `TheOne.Avalonia.Controls.NavMenuFilterBox?` | 只读; CLR / n/a | `accessor-defined / state` | The template's native auto-complete input, available after template application. |
| [`FilterPlaceholder`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L37) | `string` | 可写; OneWay | `"搜索菜单"` | 菜单内置筛选输入的占位提示文本。 |
| [`FilterResults`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L43) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.NavMenuFilterResult>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HoverCloseDelay`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L40) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(300)` | 00:00:00.300 ; 离开锚点与弹层链后延迟关闭，进入任一级弹层或返回锚点取消待关闭操作。 校验：`ValidDelay` |
| [`HoverOpenDelay`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L34) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(100)` | 00:00:00.100 ; 快速经过图标不打开，停留后才打开；可设为零。 校验：`ValidDelay` |
| [`IconSize`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L155) | `double` | 可写; OneWay | `18` | 图标绘制尺寸，单位 DIP；独立于文本字号与按钮点击区域。 校验：`Positive` |
| [`IconSpacing`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L161) | `double` | 可写; OneWay | `10` | 图标与相邻文本／内容的 DIP 间距。 校验：`value => double.IsFinite(value) && value >= 0` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L104) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`Indent`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L167) | `double` | 可写; OneWay | `16` | 每一级层次增加的 DIP 缩进；不改变父子关系。 校验：`value => double.IsFinite(value) && value >= 0` |
| [`IsCollapsed`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L83) | `bool` | 可写; TwoWay | `false` | 切换菜单为紧凑图标栏；子菜单通过悬浮弹层展示，保留当前选择。 |
| [`IsCompactSubmenu`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L71) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsFilterOpen`](../../../TheOne.Avalonia/Controls/NavMenu.FilterPopup.cs#L22) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsGroupBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L135) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取菜单项是否为分组的绑定；分组与普通可导航项行为不同。 |
| [`IsItemEnabledBinding`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L143) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取单项可用性的绑定；禁用项不响应用户导航选择。 |
| [`IsSearching`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L53) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsSubmenuOpen`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L107) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L149) | `double` | 可写; OneWay | `36` | 单项／滚轮单元的 DIP 高度；主题密度和显式局部值可能覆盖注册初值。 校验：`Positive` |
| [`ItemInvokedCommand`](../../../TheOne.Avalonia/Controls/NavMenu.cs#L173) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`OpenOnHover`](../../../TheOne.Avalonia/Controls/NavMenu.Hover.cs#L28) | `bool` | 可写; OneWay | `true` | true ; 折叠根图标和各级弹层中的分支支持悬停打开；false 时仍可点击、键盘或调用 API。 |
| [`SearchMatchCount`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L54) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SearchOwner`](../../../TheOne.Avalonia/Controls/NavMenu.SearchInput.cs#L11) | `TheOne.Avalonia.Controls.NavMenu?` | 可写; OneWay; 附加属性 | `null` | 搜索结果关联的菜单实例；供筛选弹层定位并激活原始菜单项。 |
| [`SearchText`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L39) | `string?` | 可写; TwoWay | `null` | 当前搜索文本；搜索与选中状态独立，不用查询字符串充当模型主键。 |
| [`SearchTextBinding`](../../../TheOne.Avalonia/Controls/NavMenu.Search.cs#L47) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取单项搜索文本的编译绑定；展示模板仍可独立定制。 |
| [`ShowFilter`](../../../TheOne.Avalonia/Controls/NavMenu.Filter.cs#L31) | `bool` | 可写; OneWay | `false` | 显示菜单内置筛选框；关闭后仍可由宿主提供独立搜索入口。 |
| [`Submenu`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L110) | `TheOne.Avalonia.Controls.NavMenu?` | 只读; OneWay | `computed / state` | The currently open, virtualized submenu. Null when closed. |
| [`SubmenuMaxHeight`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L101) | `double` | 可写; OneWay | `320` | 240 ; 320 ; 400 校验：`Positive` |
| [`SubmenuOpensLeft`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L77) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SubmenuWidth`](../../../TheOne.Avalonia/Controls/NavMenu.Collapsed.cs#L95) | `double` | 可写; OneWay | `280` | 240 ; 280 ; 320 校验：`Positive` |
| [`TransitionDuration`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L88) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(200)` | 展开／切换过渡时长；Motion.Enabled=False 时不执行动画。 校验：`value => value >= TimeSpan.Zero` |
| [`TransitionExtent`](../../../TheOne.Avalonia/Controls/NavMenu.Motion.cs#L94) | `double` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### NavMenuItem

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuItem.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsActiveAncestor`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L50) | `bool` | 可写; OneWay | `false` | 当前项是否为选中项的祖先；由菜单维护，不能用作普通子项的选中状态。 |
| [`IsCascadeItem`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L38) | `bool` | 可写; OneWay | `false` | 当前容器是否属于级联弹出菜单；由模板／菜单维护。 |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L44) | `bool` | 可写; OneWay | `false` | 启用紧凑布局；具体显示字段由所属模板决定，不改变业务数据。 |
| [`IsGroup`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L56) | `bool` | 可写; OneWay | `false` | 此菜单项是否为分组；不等同于当前项是否选中。 |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `bool NavMenu.OpenSubmenu(int rootIndex)` | Open a collapsed root's children without selecting or expanding its model. |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `void NavMenu.CloseSubmenu()` | 关闭当前级联子菜单并清理其弹出状态，保留业务选择。 |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `bool NavMenu.OpenFilter()` | Focuses the inline filter, or opens its compact search popup. |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `void NavMenu.CloseFilter()` | Dismisses compact search and restores the pre-search navigation state. |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `bool NavMenu.SelectPath(params int[] path)` | Realize only an ancestor path, then select its target data item. |

### 事件订阅

按需订阅下列通知，并在宿主释放时解除订阅；这里只列事件入口，不列路由事件标识字段。

| 类型 | 事件 | 用途 |
| --- | --- | --- |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `ItemInvoked` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`NavMenuFilterBox`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterBox.md) | `ResultCommitted` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.NavMenuFilterResult`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterResult.md) | 4 |
| [`TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs.md) | 1 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
