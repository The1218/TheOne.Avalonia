# NavMenu 搜索

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

**简体中文** · [English](../en/nav-menu-search.md)

基础接入见[NavMenu](nav-menu.md)。

## 多级搜索过滤

使用 SearchText 指定搜索词，SearchTextBinding 指定节点搜索字段；ChildrenBinding 继续指定子级来源。搜索输入可以放在页面或侧栏中，独立 NavMenu 预览页使用编译双向绑定：

```xml
<TextBox Text="{Binding MenuSearch, Mode=TwoWay}" PlaceholderText="搜索菜单" />
<one:NavMenu Height="480"
             ItemsSource="{Binding MenuRoots}"
             SelectedItem="{Binding SelectedMenu, Mode=TwoWay}"
             SearchText="{Binding MenuSearch, Mode=TwoWay}"
             SearchTextBinding="{Binding Title}"
             ChildrenBinding="{Binding Entries}"
             ExpandedBinding="{Binding IsExpanded, Mode=TwoWay}" />
```

大小写不敏感，空格分开的多个词必须同时命中字段。结果保留直接匹配项目及其完整父级路径，自动展开这些路径；SearchMatchCount 只计直接匹配项，IsSearching 表示过滤是否启用。空串、null 或仅空白会退出搜索；未设置 SearchTextBinding 时不启用过滤。属性支持样式与编译绑定。

过滤保持原始模型身份、图标／内容模板、禁用与分组行为。原始数据集合不被改写；ItemsSource 的有效值是当前过滤投影，外部 ViewModel 的根集合仍保留完整数据。搜索期间路径展开不写回 ExpandedBinding，清空恢复首次进入搜索时的展开、选择及根视口。连续修改搜索词不会重置该快照；仍匹配的当前选择会保留。原选择已从数据源移除时恢复为空。

未绑定 ExpandedBinding 时，展开快照使用现有模型身份缓存，回收后仍可恢复。INotifyPropertyChanged 的节点改名和 INotifyCollectionChanged 的根／子集合更新会刷新当前过滤；卸载取消订阅，重新挂载刷新结果。应在 UI 线程更新菜单模型，并使用有独立身份的节点对象。

普通树及折叠级联弹层均使用过滤后的集合与原生虚拟化面板，搜索不创建整棵树的 UI 容器。SelectPath 的索引对应当前过滤结果。PreviewHost → 扩展控件 → NavMenu 提供搜索三级末项、搜索千条子菜单与清空恢复命令，支持顶部主题／尺寸切换。

## 搜索高亮与输入框键盘导航

通过附加属性 `NavMenu.SearchOwner` 关联输入框与菜单；输入框 Text 和菜单 SearchText 仍双向绑定同一 ViewModel 字段，无需在 View 中处理 KeyDown：

```xml
<TextBox Name="MenuSearch" Text="{Binding MenuSearch, Mode=TwoWay}"
         one:NavMenu.SearchOwner="{Binding #Navigation}" />
<one:NavMenu Name="Navigation" Height="480"
             ItemsSource="{Binding MenuRoots}"
             SearchText="{Binding MenuSearch, Mode=TwoWay}"
             SearchTextBinding="{Binding Title}"
             ChildrenBinding="{Binding Entries}"
             SelectedItem="{Binding SelectedMenu, Mode=TwoWay}">
    <one:NavMenu.ContentTemplate>
        <DataTemplate x:DataType="local:MenuNode">
            <one:NavMenuSearchText SourceText="{Binding Title}" TextTrimming="CharacterEllipsis" />
        </DataTemplate>
    </one:NavMenu.ContentTemplate>
</one:NavMenu>
```

未指定 ContentTemplate 时，默认文字自动使用 SearchTextBinding 字段并高亮；自定义内容模板中使用 `NavMenuSearchText.SourceText` 绑定文字即可自动继承搜索词。任意复杂模板仍由消费方控制，库不会改写其中的普通 TextBlock、按钮或原有 Inlines。SourceText 独立于 TextBlock.Text，重建文字片段时不会破坏编译绑定。

匹配采用与过滤一致的 OrdinalIgnoreCase 和空白分词规则，所有命中片段加粗并添加下划线，重叠命中合并。文字继承菜单的前景色、字号及禁用状态，覆盖 Light / Dark 与 Small / Normal / Large；不会覆盖选中项的强调色。模型文字更新、查询变化和容器回收会重建片段，清空查询去除高亮。折叠弹层及标题同样支持高亮。

搜索框中的无修饰键行为：

| 按键 | 行为 |
| --- | --- |
| Down / Up | 按当前投影的深度优先顺序切换直接匹配且可调用的项目，跳过分组、禁用项及禁用祖先的后代；首次 Down 选择首项，首次 Up 选择末项，边界保持当前项。 |
| Enter | 调用当前结果；尚未选择结果时调用首项。保留查询，只触发一次 ItemInvoked / ItemInvokedCommand；折叠模式关闭整条弹层链。 |
| Esc | 清空 SearchText，经双向绑定同步输入框，恢复首次搜索前的展开、选择及根视口。 |

导航只按需实现目标路径的原生容器，焦点与输入位置保留在搜索框；折叠模式通过不抢焦点的 Transient 弹层定位。没有结果时方向键与 Enter 不调用菜单。未搜索时，以及 Ctrl / Alt / Shift / Meta 修饰的组合键，保持 TextBox 原生行为。

## 顶部自动完成搜索

设置 `ShowFilter=True` 在 NavMenu 顶部显示原生 AutoCompleteBox 搜索框；默认 False，保留现有布局。`FilterPlaceholder` 可设置提示文字，搜索框继承菜单的三种尺寸与明暗主题。

```xml
<one:NavMenu Height="480" ShowFilter="True"
             FilterPlaceholder="搜索菜单"
             ItemsSource="{Binding MenuRoots}"
             SearchText="{Binding MenuSearch, Mode=TwoWay}"
             SearchTextBinding="{Binding Title}"
             ChildrenBinding="{Binding Entries}"
             SelectedItem="{Binding SelectedMenu, Mode=TwoWay}" />
```

建议数据自动来自原始 ItemsSource 的各级模型，通过 ChildrenBinding 递归读取子集合，通过 SearchTextBinding 获取文字，无需另外维护建议列表。建议展示菜单名称与父级路径，按完整路径进行忽略大小写的空白分词匹配；分组、禁用节点及禁用祖先下的后代不作为可选结果。模型与集合通知会更新索引，隐藏搜索框或卸载控件时释放订阅。

Up / Down 预选建议并保留输入关键词，Enter 或鼠标点击确认。确认后先清空搜索投影，再展开原始菜单的父级路径、选择并滚动定位目标；折叠模式通过级联弹层定位。SelectedItem 的双向绑定同步原始模型，确认定位不额外触发 ItemInvoked。Esc 清空搜索并恢复搜索前的展开与选择状态。展开布局保留输入焦点，折叠布局确认或取消后将焦点返回搜索图标。

搜索建议使用有界视口、VirtualizingStackPanel 与原生容器回收；只创建可见建议和定位所需的菜单容器。索引是轻量模型数据，不预创建控件。原有外置 TextBox + SearchOwner API 仍可使用，其 Enter 调用菜单语义不变。

### 折叠侧栏搜索

`ShowFilter=True` 且 `IsCollapsed=True` 时，顶部只显示搜索图标。鼠标点击或键盘聚焦后按 Enter／空格打开搜索弹层，自动聚焦输入框。展开与折叠布局复用同一个 AutoCompleteBox 和编译绑定，不维护第二份搜索状态。

确认建议后关闭搜索输入及建议弹层，定位原始菜单的级联路径；目标菜单弹层保持可见。Esc 或点击外部取消搜索、清空查询并恢复搜索前的状态。关闭状态下，ViewModel 更新 SearchText 不会打开隐藏的建议弹层。切换为展开侧栏会关闭搜索弹层，并恢复顶部内联输入框；隐藏 ShowFilter、禁用或卸载 NavMenu 同样关闭搜索弹层并释放窗口尺寸监听。

`OpenFilter()` 可用于 ViewModel 命令或快捷键：展开布局聚焦内联输入框，折叠布局打开搜索弹层。`CloseFilter()` 取消折叠搜索；只读 `IsFilterOpen` 可通过编译绑定观察折叠搜索弹层状态。弹层继承当前明暗主题和三种尺寸，打开期间切换主题／尺寸即时更新；输入宽度随窗口可用空间调整，建议保持有界虚拟化视口。

PreviewHost 主导航与独立 NavMenu 页已启用内置搜索。独立页可切换 ShowFilter、尺寸、主题和折叠模式，搜索千条深层数据；专项检查覆盖第 1,000 项建议与菜单的实际容器回收、键盘／鼠标确认、图标激活、动态数据更新、Esc／外部取消、弹层关闭、焦点返回、打开时切换主题／尺寸及窄窗口适配：

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

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

### NavMenuFilterBox

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterBox.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### NavMenuFilterResult

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterResult.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Item`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L78) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Label`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L79) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 显示标签文本；不作为模型身份、字段路径或序列化标识。 |
| [`Path`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L80) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 输出字段的稳定路径键；与数据引用中的 FieldPath 一致。 |
| [`SearchPath`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L81) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### NavMenuSearchText

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuSearchText.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`HighlightQuery`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L12) | `string?` | 可写; OneWay; 附加属性; 向后代继承 | `null` | 需在源文本中高亮的查询；只改变绘制，不修改 SourceText。 |
| [`SourceText`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L13) | `string?` | 可写; OneWay | `null` | 高亮前的完整源文本；不要传入格式化的富文本标记。 |

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
