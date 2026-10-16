# NavMenu 动效

**简体中文** · [English](../en/nav-menu-motion.md)

基础接入见[NavMenu](nav-menu.md)。

## 展开、收起与侧栏切换动效

`TransitionDuration` 是可样式化／编译绑定的 TimeSpan，主题默认使用 `One.Motion.Expand`（200ms），收起用 70% 时长；设为零立即切换。`Motion.Enabled=False` 在动画中也会直接到达当前目标，重新开启不重放旧动作。

千条子项的行内展开采用有界视口行程，动画期间复用完整子布局，避免按数万像素的列表高度推进而让首屏突现、收尾拖延。完成后恢复完整原生滚动范围；关闭期间立即禁止子级命中测试。快速反转从当前进度继续，显式 SelectPath／键盘远距离导航会完成祖先过渡后再定位。只读 TransitionExtent 反映当前菜单的有限视口高度，不改变最终列表范围。

侧栏的尺寸和导航模式一次更新，已在可见视口中的图标从当前渲染位置平移到新位置，展开时文字渐显。未对页面 Width／Height 做逐帧过渡，不缩放图标或文字；只驱动一个进度动画，回收后的项目不会重放入场。切换尺寸、关闭动效、控件卸载及容器回收会清理临时变换与透明度，选择和业务展开状态保持不变。

PreviewHost 独立 NavMenu 页提供“展开 / 收起千条子项”“展开 / 收起三级”“快速反转侧栏”，配合顶部动效开关可验证中途关闭动效。专项使用真实 Dispatcher 帧循环与 Skia 帧推进，覆盖 Light / Dark × 三尺寸、千条实际容器回收、连续中间帧、快速反转、完整范围恢复、焦点／选择状态及卸载清理：

执行命令与测试范围见[预览专项入口](../../../design/PreviewHost/Program.cs)。

定向导出包含收起、展开与侧栏切换的中间帧，以及内置搜索建议和折叠搜索图标／弹层，当前 NavMenu 合计 66 张图片，未导出其他控件。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节只列出本项目增加的 Classes、属性、扩展 API／事件及关联模型，不重复 Avalonia 原生继承 API。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用可见示例对照本项目附加样式，用 Description 组件说明专属属性、API 签名与模型字段；ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

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

### RevealHost

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.RevealHost.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ConstrainContent`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L31) | `bool` | 可写; OneWay | `false` | Preserve a finite parent viewport for content such as a virtualized list. |
| [`Direction`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L28) | `Avalonia.Controls.ExpandDirection` | 可写; OneWay | `ExpandDirection.Down` | 展开或端口方向枚举；具体枚举类型列明可选值，不用布尔值代替方向。 可选值：`Down`, `Up`, `Left`, `Right` |
| [`Duration`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L29) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(200)` | 持续时间；动效仍遵守继承的 Motion.Enabled，不能仅用时长替代关闭开关。 |
| [`IsOpen`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L27) | `bool` | 可写; OneWay | `false` | 展开或显示内容；收起保留内容对象，过渡遵循 Motion.Enabled。 |
| [`TransitionExtent`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L22) | `double` | 可写; OneWay | `double.PositiveInfinity` | Optional visible-distance cap during animation; the settled layout retains its full extent. 校验：`value => value > 0 && !double.IsNaN(value)` |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| NavMenu | `one-large` | 模板自动类；不手动绑定 | one&#124;NavMenu.one-large => CollapsedWidth=60; SubmenuWidth=320; SubmenuMaxHeight=400; FontSize=16; IconSize=20; IconSpacing=12; ItemHeight=44; Indent=20 |
| NavMenu | `one-normal` | 模板自动类；不手动绑定 | one&#124;NavMenu.one-normal => CollapsedWidth=52; SubmenuWidth=280; SubmenuMaxHeight=320; FontSize=14; IconSize=18; IconSpacing=10; ItemHeight=36; Indent=16 |
| NavMenu | `one-small` | 模板自动类；不手动绑定 | one&#124;NavMenu.one-small => CollapsedWidth=44; SubmenuWidth=240; SubmenuMaxHeight=240; FontSize=12; IconSize=16; IconSpacing=8; ItemHeight=30; Indent=12 |
| NavMenuItem | `one-no-motion` | 模板自动类；不手动绑定 | ToggleSwitch.one-no-motion /template/ Thumb, Expander.one-no-motion /template/ Path, TreeViewItem.one-no-motion /template/ Path, one&#124;NavMenuItem.one-no-motion /template/ Path => Transitions={x:Null} |

`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出附加外观类的控件使用默认主题及其专属属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | Class | 35 | 8 | 1 |
| [`TheOne.Avalonia.Controls.NavMenuFilterBox`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterBox.md) | Class | 0 | 1 | 1 |
| [`TheOne.Avalonia.Controls.NavMenuFilterResult`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterResult.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.NavMenuItem`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuItem.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.RevealHost`](../../api/zh-cn/TheOne.Avalonia.Controls.RevealHost.md) | Class | 5 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
