# NavMenu 多级导航

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

**简体中文** · [English](../en/nav-menu.md)

`NavMenu` 继承官方 `TreeView`，使用 `NavMenuItem` 容器和 TheOne 主题。支持多级菜单、独立图标与文字模板、页面选择、分组标题、原生键盘导航，以及所有层级的虚拟化和容器回收。加载 `OneTheme` 后自动支持 Light / Dark 和三种尺寸。

## 指定子级属性与模板

`ItemsSource` 提供根菜单，`ChildrenBinding` 指定每个节点的子级来源。节点绑定及图标／内容模板从强类型 ItemsSource 自动推断节点类型，通过编译绑定访问任意业务属性路径，例如 `Entries`、`Children` 或 `Navigation.Children`；不通过属性名称反射读取对象。

```xml
<one:NavMenu Height="480"
             ItemsSource="{Binding MenuRoots}"
             SelectedItem="{Binding SelectedMenu, Mode=TwoWay}"
             ChildrenBinding="{Binding Entries}"
             ExpandedBinding="{Binding IsExpanded, Mode=TwoWay}"
             IsGroupBinding="{Binding IsGroup}"
             IsItemEnabledBinding="{Binding IsEnabled}"
             ItemInvokedCommand="{Binding NavigateCommand}">
  <one:NavMenu.IconTemplate>
    <DataTemplate x:DataType="local:MenuNode">
      <PathIcon Data="{Binding Icon}" />
    </DataTemplate>
  </one:NavMenu.IconTemplate>
  <one:NavMenu.ContentTemplate>
    <DataTemplate x:DataType="local:MenuNode">
      <TextBlock Text="{Binding Title}" TextTrimming="CharacterEllipsis" />
    </DataTemplate>
  </one:NavMenu.ContentTemplate>
</one:NavMenu>
```

`one` 为 `using:TheOne.Avalonia.Controls`。视图本身需声明窗口 ViewModel 的 `x:DataType`，并启用 `AvaloniaUseCompiledBindingsByDefault`；无需逐个绑定填写 DataType 或使用 CompiledBindingExtension。上例 `local:MenuNode` 是菜单节点类型。各模板接收完整节点，图标与标题可以来自不同业务字段。未指定 `IconTemplate` 时不保留图标间距；未指定 `ContentTemplate` 时使用原生默认内容显示。`ItemTemplate` 由控件内部管理，消费方通过这两个模板定制行内容。

子级集合可以是 `ObservableCollection<MenuNode>`，增删实时更新；模型的可变属性应提供通知。子级绑定默认空，未指定时展示单级导航。`ExpandedBinding` 可选：同一模型对象的展开状态默认在容器回收后保留；跨数据重载持久化展开状态时，使用上例的双向绑定和稳定模型身份。三种状态绑定均以节点为源，强类型集合自动推断；若 ItemsSource 只声明为非泛型 IEnumerable，需在节点绑定或模板的作用域声明节点类型。

## 选择、调用与分组

折叠后悬停一级菜单只打开子菜单，不改变当前页面。无关子菜单项保持未选中；当前页面祖先的高亮只匹配当前菜单层级中已解析的非空数据项。

`SelectedItem` 是当前页面对应的数据对象；`SelectionChanged` 可用于同步页面内容。`ItemInvoked` 和 `ItemInvokedCommand` 在点击、Enter 或 Space 调用菜单时传入该对象，重复点击已选中页面也会调用。`SelectPath` 是程序导航，不执行调用事件或命令。

通过 `IsGroupBinding` 标记分组标题。点击标题或按 Enter / Space 展开、收起分组，不替换当前页面选择，也不执行导航命令。普通节点即使包含子菜单，也可选择为页面，箭头按钮独立展开。模板内的按钮、输入框和选择控件保留自身交互。

原生方向键、Home / End 及滚动导航保留；`SelectPath` 用于定位尚未创建的多级目标，只创建祖先路径与当前视口的容器：

```csharp
// 根节点 0 → 子节点 0 → 第 1,000 个页面。
bool selected = menu.SelectPath(0, 0, 999);
```

无效路径、分组标题、禁用项目返回 false。导航选择使用单选模式；读取完整菜单数据应遍历模型，不应遍历 `GetRealizedTreeContainers()`。

## 密度、主题与动效

可在控件或父级设置 `styling:ControlSizing.Size="Small|Normal|Large"`，也可通过现有 `small` / `normal` / `large` 类切换。`ItemHeight`、`IconSize`、`IconSpacing`、`Indent` 和 `FontSize` 均为可用 Style 覆盖的属性。

| 尺寸 | 行高度 | 图标 | 字号 | 图标间距 | 层级缩进 |
| --- | ---: | ---: | ---: | ---: | ---: |
| Small | 30 | 16 | 12 | 8 | 12 |
| Normal | 36 | 18 | 14 | 10 | 16 |
| Large | 44 | 20 | 16 | 12 | 20 |

普通行透明，悬停使用 `One.Subtle`，选中使用 `One.AccentSoft` / `One.Accent`，禁用使用 `One.DisabledText`；主题与密度可实时切换。展开收起沿用 TreeView 的 RevealHost，遵循继承的 `Motion.Enabled`。

## 虚拟化与验证

大根集合和每一级子菜单使用原生 `VirtualizingStackPanel`，保留 `PART_*`、焦点、选择和展开逻辑。指定 `ChildrenBinding` 且根节点不超过 32 个时，根层改用 `StackPanel` 保留分组容器，避免快速反向滚动时整组回收、重挂载和高度估算跳变；子级仍虚拟化，并缓存前后各一个视口。根集合增删跨过此阈值时自动切换，显式设置的 `ItemsPanel` 优先。

PreviewHost 主侧栏已采用 NavMenu，分为基础控件、数据展示、导航与布局、图表控件、扩展控件、交互验证六区。扩展控件中的“导航菜单 NavMenu”演示每级 1,000 条、共 5,000 个模型，可跳到末尾根节点、二级及三级页面，实时查看实际容器数量。

专项覆盖明暗主题和三种尺寸、实际容器数量、远距离选择与键盘导航、回收后的展开状态、编译模板更新、动态子项、禁用状态、分组不导航、自定义模板操作、实时尺寸及主题切换。独立原生消费项目复用同一编译 XAML 和交互验证，不使用反射绑定、动态 XAML 或整程序集保留。

## 按功能查阅

- [侧栏折叠、悬停级联与边缘适配](nav-menu-popups.md)。
- [多级搜索、高亮、键盘与顶部自动完成](nav-menu-search.md)。
- [展开与收起动效](nav-menu-motion.md)。

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

### NavMenuFilterBox

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterBox.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### NavMenuFilterResult

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuFilterResult.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Item`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L108) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Label`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L109) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 显示标签文本；不作为模型身份、字段路径或序列化标识。 |
| [`Path`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L110) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 输出字段的稳定路径键；与数据引用中的 FieldPath 一致。 |
| [`SearchPath`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L111) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### NavMenuItem

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuItem.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsActiveAncestor`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L50) | `bool` | 可写; OneWay | `false` | 当前项是否为选中项的祖先；由菜单维护，不能用作普通子项的选中状态。 |
| [`IsCascadeItem`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L38) | `bool` | 可写; OneWay | `false` | 当前容器是否属于级联弹出菜单；由模板／菜单维护。 |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L44) | `bool` | 可写; OneWay | `false` | 启用紧凑布局；具体显示字段由所属模板决定，不改变业务数据。 |
| [`IsGroup`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L56) | `bool` | 可写; OneWay | `false` | 此菜单项是否为分组；不等同于当前项是否选中。 |

### NavMenuSearchText

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.NavMenuSearchText.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`HighlightQuery`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L14) | `string?` | 可写; OneWay; 附加属性; 向后代继承 | `null` | 需在源文本中高亮的查询；只改变绘制，不修改 SourceText。 |
| [`SourceText`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L17) | `string?` | 可写; OneWay | `null` | 高亮前的完整源文本；不要传入格式化的富文本标记。 |

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
