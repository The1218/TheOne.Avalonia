# 项表面间距

**简体中文** · [English](../en/item-spacing.md)

列表和导航中的可选择项表面与外框至少相隔 4px，相邻表面间隔 4px；弹层原有的更大留白继续保留。横向排列时，每项左右各 2px 形成 4px 项间距，由父容器补齐边缘留白。嵌套结构在可见标题表面设置间距，避免整个分支的边距随深度累加。

| 控件族 | 实现 |
| --- | --- |
| ListBoxItem / ComboBoxItem | `Margin="4,2"` |
| TreeViewItem / NavMenuItem | 标题表面 `PART_LayoutRoot` 使用 `Margin="4,2"`，保留原生子项缩进 |
| AutoCompleteBox / MultiSelectBox 结果列表 | 继承 ListBoxItem 间距 |
| 纵向 MenuItem | `Margin="0,2"`；ContextMenu 和子菜单保留 6px 外框内边距 |
| Menu / 横向菜单浮层 | 项使用 `Margin="2"`；菜单栏父容器补 2px 内边距，浮层保留 6px 内边距 |
| MenuFlyoutPresenter | 默认 6px 内边距传递给实际边框，局部 Padding 设置仍然有效 |
| 普通 TabControl / TabItem | 标题滚动区留 2px 内边距、项留 2px 外边距，支持上／下／左／右四种位置 |
| TabStripItem | `Margin="2"`，兼顾横向和竖向面板；TabStrip 保留原有 4px 内边距 |

菜单栏使用最小高度并随菜单项自然增长，使三档尺寸都有完整留白。可以局部设置项的 Margin 或 Presenter 的 Padding 来覆盖默认值。

检查中也区分了下列结构：

- 日历格已有 2px 外边距，二维网格中相邻表面间隔 4px。
- Pagination 页码按钮已有 6px 间距，FlowStep 有显式节点间距；VerificationStep 使用指示器连线，不存在相邻填充选中表面。
- 卡片式标签页保留分隔线和选中页接缝；DataGrid 单元格保留连续网格和数据对齐；Gantt 的派生行主题显式设置零 Margin，避免继承列表间隔后产生时间轴网格缝隙；Carousel 页面保留整视口翻页，不追加列表边距。
- 通用 ItemsControl 的内容由调用方定义，没有默认选中表面，不在模板中强加项边距。WorkflowEditor 使用共享 TabStrip 主题，节点库已有显式间隔。

PreviewHost 提供独立入口，只运行受影响控件族：

```text
--verify-menu-item-spacing [output]
--verify-tab-item-spacing [output]
--verify-tabstrip-item-spacing [output]
--verify-gantt [output]
```

覆盖明暗主题、Small / Normal / Large、宽窄视口、实际表面与间距像素、禁用项以及原生按键／指针交互。菜单专项包含 ContextMenu、纵向／横向浮层和嵌套子菜单；TabControl 覆盖四种位置，TabStrip 覆盖两种面板方向。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### MultiSelectBox

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanClear`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L76) | `bool` | 只读; OneWay | `computed / state` | 只读，无结果、存在可清空项和当前密度的最小输入高度 |
| [`ClearText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L62) | `string` | 可写; OneWay | `"清空可编辑选择"` | 原生操作按钮的可访问名称；移除格式默认 `移除 {0}`，参数为模型文本 |
| [`DisplayTags`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L70) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.MultiSelectBoxItem>` | 只读; OneWay | `computed / state` | 只读模板数据；`MultiSelectBoxItem.Item` 是原始模型，其他属性是文字、可选／选中状态和模板内容 |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L61) | `string` | 可写; OneWay | `"无匹配选项"` | 未选择、搜索输入与无匹配提示，可覆盖为宿主语言 |
| [`HasHiddenSelection`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L74) | `bool` | 只读; OneWay | `computed / state` | 只读，未建立标签的选择数量与是否有溢出 |
| [`HasNoResults`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L75) | `bool` | 只读; OneWay | `computed / state` | 只读，无结果、存在可清空项和当前密度的最小输入高度 |
| [`HasSelection`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L73) | `bool` | 只读; OneWay | `computed / state` | 只读，去重后选择数量与是否非空 |
| [`HiddenSelectedCount`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L72) | `int` | 只读; OneWay | `computed / state` | 只读，未建立标签的选择数量与是否有溢出 |
| [`IsDropDownOpen`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L66) | `bool` | 可写; TwoWay | `false` | 选择只读及可双向绑定的弹层状态；禁用或卸载会关闭弹层 |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L65) | `bool` | 可写; OneWay | `false` | 选择只读及可双向绑定的弹层状态；禁用或卸载会关闭弹层 |
| [`ItemAdapter`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L55) | `TheOne.Avalonia.Controls.MultiSelectItemAdapter?` | 可写; OneWay | `null` | 显式文本与可选状态 getter；文本同时用于搜索和默认标签 |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L77) | `double` | 只读; OneWay | `computed / state` | 只读，无结果、存在可清空项和当前密度的最小输入高度 |
| [`ItemTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L56) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 分别呈现结果和标签中的原始模型；模板要声明对应 `x:DataType` |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L53) | `System.Collections.IEnumerable?` | 可写; OneWay | `null` | `IEnumerable?`，选项模型；不传入预创建控件 |
| [`MaxDropDownHeight`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L68) | `double` | 可写; OneWay | `320` | 默认 `320`，必须为至少 80 的有限数；弹层同时约束到宿主可用范围 校验：`value => double.IsFinite(value) && value >= 80` |
| [`MaxVisibleTags`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L67) | `int` | 可写; OneWay | `3` | 默认 `3`，范围 `0–100`；只为前 N 个选择建立标签，其余显示数量 校验：`value => value is >= 0 and <= 100` |
| [`OpenText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L63) | `string` | 可写; OneWay | `"展开或收起选项"` | 原生操作按钮的可访问名称；移除格式默认 `移除 {0}`，参数为模型文本 |
| [`PlaceholderText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L59) | `string` | 可写; OneWay | `"请选择"` | 未选择、搜索输入与无匹配提示，可覆盖为宿主语言 |
| [`RemoveTextFormat`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L64) | `string` | 可写; OneWay | `"移除 {0}"` | 原生操作按钮的可访问名称；移除格式默认 `移除 {0}`，参数为模型文本 |
| [`SearchPlaceholderText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L60) | `string` | 可写; OneWay | `"搜索选项"` | 未选择、搜索输入与无匹配提示，可覆盖为宿主语言 |
| [`SearchText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L58) | `string` | 可写; TwoWay | `""` | 默认空字符串，支持双向绑定；关闭弹层保留查询 |
| [`SelectedCount`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L71) | `int` | 只读; OneWay | `computed / state` | 只读，去重后选择数量与是否非空 |
| [`SelectedItemTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L57) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 分别呈现结果和标签中的原始模型；模板要声明对应 `x:DataType` |
| [`SelectedItems`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L54) | `System.Collections.IList?` | 可写; TwoWay | `null` | `IList?`，默认双向绑定；用户操作与外部集合变更同步 |
| [`VisibleItems`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L69) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.MultiSelectBoxItem>` | 只读; OneWay | `computed / state` | 只读模板数据；`MultiSelectBoxItem.Item` 是原始模型，其他属性是文字、可选／选中状态和模板内容 |

### NavMenu

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md)

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

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

- `ComboBox`: `Classes="filled"` / `Classes="underlined"`
- `ListBox`: `Classes="borderless"` / `Classes="cards"`
- `TabControl`: `Classes="card"` / `Classes="pills"`

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `bool MultiSelectBox.OpenDropDown()` | 打开并聚焦搜索，或关闭并回焦字段；打开要求已挂载且可用 |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `void MultiSelectBox.CloseDropDown()` | 打开并聚焦搜索，或关闭并回焦字段；打开要求已挂载且可用 |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `void MultiSelectBox.RefreshItems()` | Refreshes plain collections and models that do not provide change notifications. |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `bool MultiSelectBox.ToggleItem(object item)` | 切换可编辑项；新增选择要求项在数据源中，移除允许已选的外部值；未改变返回 `false` |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `bool MultiSelectBox.ClearSelection()` | Removes editable selections, retaining selected disabled items. |
| [`MultiSelectItemAdapter`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) | `abstract string MultiSelectItemAdapter.GetText(object item)` | 使用已配置的类型化适配器读取原始模型显示／搜索文本。 |
| [`MultiSelectItemAdapter`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) | `virtual bool MultiSelectItemAdapter.IsEnabled(object item)` | 使用适配器判断原始模型能否参与用户选择。 |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `bool NavMenu.OpenSubmenu(int rootIndex)` | Open a collapsed root's children without selecting or expanding its model. |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `void NavMenu.CloseSubmenu()` | 关闭当前级联子菜单并清理其弹出状态，保留业务选择。 |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `bool NavMenu.OpenFilter()` | Focuses the inline filter, or opens its compact search popup. |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `void NavMenu.CloseFilter()` | Dismisses compact search and restores the pre-search navigation state. |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `bool NavMenu.SelectPath(params int[] path)` | Realize only an ancestor path, then select its target data item. |

### 事件订阅

按需订阅下列通知，并在宿主释放时解除订阅；这里只列事件入口，不列路由事件标识字段。

| 类型 | 事件 | 用途 |
| --- | --- | --- |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `SelectionChanged` | 冒泡事件，AddedItems / RemovedItems 提供原始模型，批量用户操作合并为一次通知 |
| [`MultiSelectBoxItem`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) | `PropertyChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`NavMenu`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenu.md) | `ItemInvoked` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`NavMenuFilterBox`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterBox.md) | `ResultCommitted` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.MultiSelectBoxItem`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) | 10 |
| [`TheOne.Avalonia.Controls.NavMenuFilterResult`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterResult.md) | 4 |
| [`TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuItemInvokedEventArgs.md) | 1 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
