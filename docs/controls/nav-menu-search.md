# NavMenu 搜索

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

执行命令与测试范围见[测试策略](../workflow/testing.md)。


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

独立 NavMenu 预览页展示上述输入操作和调用次数。`--verify-navmenu-search` 包含三尺寸 × 明暗主题的真实输入、逐项导航至第 1,000 项、容器数量与回收、Enter 单次调用、Esc 快照恢复、折叠级联、高亮分词／大小写／重叠，以及默认与自定义编译模板；完整控件检查及原生 AOT 消费验证复用这些检查。

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

执行命令与测试范围见[测试策略](../workflow/testing.md)。
