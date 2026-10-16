# NavMenu 侧栏与级联弹层

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

执行命令与测试范围见[预览专项入口](../../../design/PreviewHost/Program.cs)。

`--verify-navmenu`、`--verify-controls` 和 NativeAOT 消费验证同样包含悬停与级联场景，覆盖三尺寸 × 明暗主题、跨层移动、延迟关闭取消、快速切换、逐级键盘返回、千项远距离选择、回收与卸载后的计时器清理。

## 窗口边缘与窄窗口

弹层优先沿上一层方向打开。空间不足且另一侧不会覆盖可见祖先菜单时自动翻转；根图标旁的弹层也会按剩余空间缩窄。宽高与位置限制在窗口客户区内，实时缩放时重新计算。

无法并排容纳时采用单级展示：暂时隐藏父层并关闭其命中测试，下一层显示返回按钮。返回恢复父层的选择和滚动状态；鼠标停在返回后露出的原分支时不会立即重开，离开再进入恢复悬停。Left / Esc 同样逐级返回。加宽窗口后恢复级联。

只读 Avalonia 属性 IsCompactSubmenu 和 SubmenuOpensLeft 可用于观察布局。鼠标移动按可见弹层链判断，布局改变时保留关闭缓冲，避免翻转或缩放后误关闭。每一级仍保留有限视口与容器回收。

PreviewHost → 扩展控件 → NavMenu 提供窗口宽度、靠右选项和边缘预览窗口，每级 1,000 项。`--verify-navmenu-edge` 覆盖三尺寸、明暗主题、左右边缘、260／420／900 宽度、实时宽高变化、返回及远距离选择；完整 NavMenu 验证包含相同检查。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 在各控件页直接标明示例的 Classes、尺寸及状态；公共属性、API／事件和模型字段使用 Description 组件说明，包含属性类型、默认值、绑定模式、校验条件及 API 签名。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### NavMenu

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md)

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

### NavMenuItem

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuItem.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsActiveAncestor`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L22) | `bool` | 可写; OneWay | `false` | 当前项是否为选中项的祖先；由菜单维护，不能用作普通子项的选中状态。 |
| [`IsCascadeItem`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L20) | `bool` | 可写; OneWay | `false` | 当前容器是否属于级联弹出菜单；由模板／菜单维护。 |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L21) | `bool` | 可写; OneWay | `false` | 启用紧凑布局；具体显示字段由所属模板决定，不改变业务数据。 |
| [`IsGroup`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L23) | `bool` | 可写; OneWay | `false` | 此菜单项是否为分组；不等同于当前项是否选中。 |

### 公共附加属性

[ControlSizing](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性; 向后代继承 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

[Motion](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| NavMenu | `one-large` | 模板自动类；不手动绑定 | one&#124;NavMenu.one-large => CollapsedWidth=60; SubmenuWidth=320; SubmenuMaxHeight=400; FontSize=16; IconSize=20; IconSpacing=12; ItemHeight=44; Indent=20 |
| NavMenu | `one-normal` | 模板自动类；不手动绑定 | one&#124;NavMenu.one-normal => CollapsedWidth=52; SubmenuWidth=280; SubmenuMaxHeight=320; FontSize=14; IconSize=18; IconSpacing=10; ItemHeight=36; Indent=16 |
| NavMenu | `one-small` | 模板自动类；不手动绑定 | one&#124;NavMenu.one-small => CollapsedWidth=44; SubmenuWidth=240; SubmenuMaxHeight=240; FontSize=12; IconSize=16; IconSpacing=8; ItemHeight=30; Indent=12 |
| NavMenuItem | `one-no-motion` | 模板自动类；不手动绑定 | ToggleSwitch.one-no-motion /template/ Thumb, Expander.one-no-motion /template/ Path, TreeViewItem.one-no-motion /template/ Path, one&#124;NavMenuItem.one-no-motion /template/ Path => Transitions={x:Null} |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | Class | 35 | 8 | 1 |
| [`TheOne.Avalonia.Controls.NavMenuFilterBox`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterBox.md) | Class | 0 | 1 | 1 |
| [`TheOne.Avalonia.Controls.NavMenuFilterResult`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterResult.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.NavMenuItem`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuItem.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs.md) | Class | 1 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
