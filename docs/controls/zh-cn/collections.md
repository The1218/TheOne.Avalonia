# 列表、树、标签与表格

**简体中文** · [English](../en/collections.md)

[返回控件文档](README.md)。视图绑定需有对应的 ViewModel 和 `x:DataType`；扩展使用 `xmlns:one="using:TheOne.Avalonia.Controls"`。

## ItemsControl 与 ListBox

```xml
<Grid RowDefinitions="Auto,*" Height="420">
  <TextBlock Text="记录" />
  <ListBox Grid.Row="1" ItemsSource="{Binding Records}"
           SelectedItem="{Binding SelectedRecord, Mode=TwoWay}">
    <ListBox.ItemTemplate>
      <DataTemplate x:DataType="vm:Record">
        <TextBlock Text="{Binding Name}" />
      </DataTemplate>
    </ListBox.ItemTemplate>
  </ListBox>
</Grid>
```

`Records` 是模型集合，`Record.Name` 为显示文字。ListBox 提供选择与键盘导航，多选可用 `SelectionMode="Multiple,Toggle"` 与官方 SelectedItems API。不需要选择时使用 ItemsControl：

```xml
<ItemsControl Height="320" ItemsSource="{Binding Records}">
  <ItemsControl.ItemTemplate>
    <DataTemplate x:DataType="vm:Record">
      <TextBlock Text="{Binding Name}" Margin="8" />
    </DataTemplate>
  </ItemsControl.ItemTemplate>
</ItemsControl>
```

主题为普通 ItemsControl 提供滚动视口，两者默认使用 VirtualizingStackPanel。列表必须有有限可视区域；不要用外层无限高度 StackPanel / ScrollViewer 承载大列表，也不要把 ItemsPanel 替换为普通 StackPanel 或 WrapPanel。

## 动态集合与容器生命周期

默认主题修复了调用方拥有的 Item 控件重排、移除后重新插入时的挂载问题，以及 Replace、单次 Reset 和切换 ItemsSource 后的旧数据残留。动态业务仍推荐绑定模型与编译模板，使用 ObservableCollection 或正确实现集合通知；直接修改不通知的底层列表不会自动刷新 UI。

替换数据源后，选择只能指向当前集合中的项。需要跨重载保留业务选择时，由 ViewModel 按稳定业务标识找到新集合中的实例再选中，不依赖旧容器或旧模型引用。树节点的展开与选择状态应保存在模型中，回收后会重新绑定。

TabControl 重排到最后一个标题之后会提交到末尾；松开或取消拖动后清理预览位移。关闭、重排或重置集合时无需先手动从视觉树移除 TabItem，也不要把生成容器缓存为数据源。

## TreeView

```xml
<TreeView x:Name="ProjectTree" Height="360"
          ItemsSource="{Binding Roots}"
          SelectedItem="{Binding SelectedNode, Mode=TwoWay}">
  <TreeView.Styles>
    <Style Selector="TreeViewItem" x:DataType="vm:TreeNode">
      <Setter Property="IsExpanded" Value="{Binding IsExpanded, Mode=TwoWay}" />
    </Style>
  </TreeView.Styles>
  <TreeView.ItemTemplate>
    <TreeDataTemplate x:DataType="vm:TreeNode" ItemsSource="{Binding Children}">
      <TextBlock Text="{Binding Name}" />
    </TreeDataTemplate>
  </TreeView.ItemTemplate>
</TreeView>
```

节点模型提供 `Name`、`Children`、`IsExpanded`，并对变化的属性发出通知；参考 [PreviewTreeNode.cs](../../../design/PreviewHost/Models/NavigationLayout/PreviewTreeNode.cs)。不同节点应有独立身份，文字可以重复。若数据重载后需要保留展开状态，按稳定 ID 恢复模型的 IsExpanded，再通过双向绑定同步容器。

根与各级子节点均按视口回收。主题启用 `Virtualization.Enabled`，补充跨未创建节点的键盘导航、Home / End、Ctrl 焦点移动与 Shift 范围选择。展开动画由主题处理，可按子树关闭 Motion。

在 UI 线程、树已挂载后按零基索引路径定位远处节点：

```csharp
using TheOne.Avalonia.Controls;

var container = Virtualization.BringIntoView(ProjectTree, 0, 9000);
if (container is not null)
    ProjectTree.SelectedItem = container.DataContext;
```

该路径表示根节点 0 的第 9001 个子节点；方法只创建路径所需容器，完成祖先展开并滚动，无效路径返回 null。多选树可调用 `Virtualization.SelectAll(tree)` 选择已展开分支中的全部节点，包含视口外节点；折叠分支的后代不在这次选择范围内。

`GetRealizedContainers()` 仅返回当前已创建容器，适合观察虚拟化数量；读取全量数据、导出或递归业务操作必须遍历模型。

## TabControl 与 Card 标签页

新增页签图标与右键菜单。`TabFeatures.ShowItemIcon`（TabControl，默认 True）统一控制图标；`TabFeatures.Icon`（TabItem）为图标数据，默认支持 Geometry／文本，可用 `IconTemplate` 自定义。`TabFeatures.IsPinned=True` 固定到左侧，固定页不能关闭；`IsClosable=False` 只禁止关闭，不等于固定。

```xml
<TabControl Classes="card" one:TabFeatures.CanClose="True"
            one:TabFeatures.CanReorder="True" one:TabFeatures.ShowItemIcon="True">
  <TabItem Header="首页" one:TabFeatures.Icon="{StaticResource HomeGeometry}"
           one:TabFeatures.IsPinned="True"><TextBlock Text="首页" /></TabItem>
  <TabItem Header="编辑"><TextBox Text="草稿" /></TabItem>
</TabControl>
```

右键菜单含关闭当前页、固定到左侧／取消固定、关闭所有页；关闭所有页跳过固定、不可关闭和取消关闭的项。自定义 ContextMenu 不会被覆盖。固定页显示固钉，禁止拖动、排序快捷键和 Move 移动；普通页的拖动预览及放置位置不能越过固定区。ShowItemIcon 只控制内容图标，不隐藏固钉。按钮、快捷键和 API 均检查相同关闭规则。绑定集合的固定状态按模型实例保留，容器回收不串用。

页签采用紧凑留白，内容图标统一缩放到 14 DIP，固钉 12 DIP，关闭图标 10 DIP；关闭按钮仍保留 24 DIP 点击区域。三档尺寸继续沿用通用尺寸规范。

OneTheme 默认开启菜单；设置 `one:TabFeatures.EnableContextMenu="False"` 可关闭内置菜单，自定义 ContextMenu 仍由使用者管理。

```csharp
TabFeatures.Pin(tabItem);        // 固定并移到左侧，保持选择
TabFeatures.Pin(tabItem, false); // 取消固定
int closed = TabFeatures.CloseAll(tabs); // 逐项遵循取消事件及 CloseCommand
```

```xml
<TabControl Classes="card" Height="320"
            ItemsSource="{Binding Documents}"
            SelectedItem="{Binding SelectedDocument, Mode=TwoWay}"
            one:TabFeatures.CanClose="True"
            one:TabFeatures.CanReorder="True">
  <TabControl.ItemTemplate>
    <DataTemplate x:DataType="vm:Document">
      <TextBlock Text="{Binding Title}" />
    </DataTemplate>
  </TabControl.ItemTemplate>
  <TabControl.ContentTemplate>
    <DataTemplate x:DataType="vm:Document">
      <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
    </DataTemplate>
  </TabControl.ContentTemplate>
  <TabControl.Styles>
    <Style Selector="TabItem" x:DataType="vm:Document">
      <Setter Property="one:TabFeatures.IsClosable" Value="{Binding CanClose}" />
    </Style>
  </TabControl.Styles>
</TabControl>
```

`Document` 提供 `Title`、`Description`、`CanClose`。ItemsSource 建议使用 `ObservableCollection<Document>`，ItemTemplate 定义标题，ContentTemplate 定义当前页内容。移除 `card` 类名即可使用普通标签外观。

Card 支持关闭与重排。标题仅有背景色，正文保留边框；溢出显示滚动箭头，按需创建标题和选中内容。`TabStripPlacement` 支持 Top / Bottom / Left / Right。

`card` 外观在原生 TabControl 主题内部应用，不再由类选择器替换 `Theme`。缓存页面离开、重新加入可视／逻辑树后，保留页签、选择和原生虚拟化，宿主无须重新创建设计器或页签控件。

拖动超过 6 px 后，标题跟手，邻项平滑让位；松手仅提交一次排序，保留选择。边缘自动滚动，预览不受容器回收影响。Esc、捕获丢失、禁用／关闭重排、外部集合变化或卸载会取消并清理，不会迟到提交。

让位／落位使用 `One.Motion.Fast`（120 ms）与 CubicEaseOut；`Motion.Enabled=False` 或零时长立即完成。普通／pills 标签设置 `TabFeatures.CanReorder=True` 即可使用。保留原生虚拟面板，只生成当前拖动的标题预览。

| 接口 | 用途 |
| --- | --- |
| `TabFeatures.CanClose` | 允许关闭；可显式设为 False |
| `TabFeatures.CanReorder` | 允许当前 TabControl 内拖动与键盘排序 |
| `TabFeatures.IsClosable` | 设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| `TabFeatures.CloseCommand` | 宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| `TabFeatures.TabClosingEvent` | 关闭前的可取消事件，参数含 Item 与 Cancel |
| `TabFeatures.Close(tabItem)` | 尝试关闭容器对应的数据项 |
| `TabFeatures.Move(tabs, from, to)` | 按零基索引尝试重排 |

`TabClosing` 回调返回后，关闭操作会重新核对容器所属控件、数据身份与当前关闭权限。若回调更换数据源或容器、固定或禁用标签、拒绝命令，或已关闭该项，原操作会停止，不再改动回调设置的选择状态。

取消有未保存内容的关闭，可在视图代码中监听（`DocumentsTabs` 为具名 TabControl）：

```csharp
DocumentsTabs.AddHandler(TabFeatures.TabClosingEvent, (_, args) =>
{
    if (args.Item is Document document && document.HasUnsavedChanges)
        args.Cancel = true;
});
```

`HasUnsavedChanges` 为业务模型属性。需要异步确认时先取消当前关闭，再由宿主确认并更新集合。默认关闭 / 排序要求可修改的 IList；只读或定长集合不能默认删除或排序，可由 CloseCommand 更新来源。

快捷键：Ctrl / macOS ⌘ + W 关闭当前项，Ctrl / ⌘ + Shift + ← / → 排序，Esc 取消拖动。关闭后选择相邻项；排序保留选中模型。Card 颜色可覆盖 One.CardTabStrip、One.CardTabBorder、One.CardTabHover。完整演示见 [ControlGallery.axaml](../../../design/PreviewHost/Verification/Fixtures/LegacyControlGallery.axaml) 与 [CardTabPreview.cs](../../../design/PreviewHost/Verification/Basic/CardTabPreview.cs)。

## DataGrid

类库引用基于 `Avalonia.Controls.DataGrid` 12.1.2 的原生布局源码扩展，应用加载 OneTheme 即可使用。核心 Avalonia 保持 12.1.4。

```xml
<DataGrid Height="420" ItemsSource="{Binding Records}"
          AutoGenerateColumns="False" MaxColumnWidth="320"
          CanUserSortColumns="True" CanUserResizeColumns="True"
          CanUserReorderColumns="True" FrozenColumnCount="1">
  <DataGrid.Columns>
    <DataGridTextColumn Header="编号" Binding="{Binding Id}"
                        IsReadOnly="True" />
    <DataGridTextColumn Header="名称" Binding="{Binding Name, Mode=TwoWay}" />
    <DataGridCheckBoxColumn Header="启用" Binding="{Binding Enabled, Mode=TwoWay}" />
  </DataGrid.Columns>
</DataGrid>
```

项目启用默认编译绑定，列从强类型 Records 集合自动推断每行模型；无需逐个绑定写 DataType 或 CompiledBindingExtension。List<Record>、ObservableCollection<Record> 和 IEnumerable<Record> 均会推断行模型；需要筛选／分组视图时可用 DataGridCollectionView<Record>，同样无需列级 x:DataType。非泛型 DataGridCollectionView 仍需声明行类型。Records 的项提供 `Id`、`Name`、`Enabled`。编辑属性需可写并发出变化通知；业务验证使用 Avalonia 支持的数据验证机制。`IsReadOnly` 可作用于全表或单列。自定义单元格使用 DataGridTemplateColumn 的 CellTemplate / CellEditingTemplate。

创建视图前调用 `DataGridAot.PreserveRowType<Record>()`，注册编辑、排序和分组元数据。路径排序设置 SortMemberPath；托管与原生契约见 [DataGrid NativeAOT](datagrid-binding.md#nativeaot)。

默认 ColumnWidth 为 Auto、最小列宽 64 px；可用固定宽度、SizeToCells、SizeToHeader 或星号宽度。自动列宽基于已显示内容渐进计算，不预扫描整张表。RowHeight 保留 Auto，长内容可增高；三档密度沿用共同约定。

有限 `MaxColumnWidth` 必须在列创建前配置，如示例中的 XAML 属性；官方 12.1.2 在已有可见列后把无限上限改成有限值存在异常。代码创建表格时先设置上限，再添加列，或在列加入之前设置该列 MaxWidth。

DataGrid 保留官方 DataGridRowsPresenter / DataGridCellsPresenter、冻结列与行回收，不能替换为 VirtualizingStackPanel。为表格提供有限视口。行回收不代表数据分页，也不代表每个已创建行的列容器完全按需生成；大量列和远程数据仍应控制列数与分页。

预览页使用 10,000 行，支持排序、调整列宽与顺序、冻结编号列、F2 编辑、Enter 提交、Esc 撤销、筛选与首 / 中 / 末行定位。示例见 [DataGridDemoView.axaml](../../../design/PreviewHost/Views/DataDisplay/DataGridDemoView.axaml) 与 [DataGridDemoModel.cs](../../../design/PreviewHost/Models/DataDisplay/GridRecord.cs)。

每列表头排序和筛选按钮、文本／数值匹配、多列组合及后续冻结／统计接口见 [DataGrid 扩展契约](datagrid-extensions.md)。

## 大数据验证

PreviewHost 的“大数据虚拟化”页展示万项列表、树根与子节点、建议列表、标签和右键菜单，并显示实际容器数量。按[预览专项入口](../../../design/PreviewHost/Program.cs)选择覆盖远距离操作、回收、导航和选择的检查；DataGrid 有独立专项。维护集合模板时，应同时观察实际容器数量和远处项操作，不能仅检查 ItemsPanel 的类型。


## TabStrip 独立标签导航

```xml
<Grid RowDefinitions="Auto,*">
  <TabStrip Name="SectionTabs" Classes="normal"
            ItemsSource="{Binding Sections}"
            SelectedItem="{Binding ActiveSection, Mode=TwoWay}"
            one:TabFeatures.CanClose="True"
            one:TabFeatures.CloseCommand="{Binding CloseSectionCommand}">
    <TabStrip.ItemTemplate>
      <DataTemplate x:DataType="vm:Section">
        <TextBlock Text="{Binding Title}" />
      </DataTemplate>
    </TabStrip.ItemTemplate>
  </TabStrip>
  <ContentControl Grid.Row="1" Content="{Binding ActiveSection}" />
</Grid>
```

使用官方 `Avalonia.Controls.Primitives.TabStrip` 和 `TabStripItem`，默认三档高度 30 / 36 / 44、字号 12 / 14 / 16。默认保持一个选中项，支持点击、方向键、Home / End 和访问键；`AutoScrollToSelectedItem=False` 可关闭自动选中项定位。

标题默认横向虚拟化和回收，可使用 `ScrollIntoView(index)` 显式定位；需给外层有限宽度，并通过数据集合绑定标题。大量标签不要预创建控件，也不要替换为 WrapPanel。单独控制某项的禁用状态时，可在 `ItemContainerTheme` 中绑定模型的 `IsEnabled`。此控件只提供导航，页面内容由宿主绑定与模板创建。

关闭能力按需开启，`TabFeatures.CanClose` 默认为 False。启用后，原生 `TabStripItem` 显示关闭按钮；标签栏持有键盘焦点时，Ctrl / macOS ⌘ + W 关闭选中项。在容器上设置 `TabFeatures.IsClosable=False` 可固定某项。关闭复用可取消的 `TabFeatures.TabClosingEvent`，参数 `Item` 为数据项，`Cancel` 可阻止关闭。`CloseSectionCommand` 接收数据项并遵循 `CanExecute`，由命令更新集合；省略命令时自动从可修改 IList 删除。只读／定长源需由宿主命令处理。关闭后选择相邻项，保留原生虚拟化与回收。

```csharp
SectionTabs.AddHandler(TabFeatures.TabClosingEvent, (_, args) =>
{
    if (args.Item is Section section && section.HasUnsavedChanges)
        args.Cancel = true;
});
if (SectionTabs.ContainerFromIndex(SectionTabs.SelectedIndex) is TabStripItem item)
    TabFeatures.Close(item);
```

`Section.HasUnsavedChanges` 为业务属性；异步确认需先取消当前关闭，再由宿主确认并移除项。`CanClose`、`CloseCommand` 与 `Close(TabStripItem)` 新增独立标签关闭，保留现有 TabControl API；`CanReorder` 和 `Move` 仍仅适用于 TabControl。

PreviewHost 的“独立标签导航”页提供 10,000 项实际数据与容器计数、目标定位、末项和启用切换。


轮播与刷新见[Carousel／Refresh](collection-content.md)。验证范围与命令见[预览专项入口](../../../design/PreviewHost/Program.cs)。

## 外观类

`ListBox Classes="cards"` 使用独立圆角条目与选中边框，`borderless` 用于嵌入布局。`TabControl Classes="pills"` 使用胶囊式标题，现有 `card` 仍提供连续外框。均保留原生面板及有界视口，集合使用 ItemsSource 与编译模板；每个控件选择一种外观类。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### TabFeatures

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.TabFeatures.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanClose`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | `bool` | 可写; OneWay; 附加属性 | `false` | 允许关闭；可显式设为 False |
| [`CanReorder`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L22) | `bool` | 可写; OneWay; 附加属性 | `false` | 允许当前 TabControl 内拖动与键盘排序 |
| [`CloseCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | `System.Windows.Input.ICommand?` | 可写; OneWay; 附加属性 | `null` | 宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`CloseTabCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L35) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `new CloseTab()` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`IsClosable`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L23) | `bool` | 可写; OneWay; 附加属性 | `true` | 设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | `bool` | 可写; OneWay; 附加属性 | `false` | 显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |

### Virtualization

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.Virtualization.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L15) | `bool` | 可写; OneWay; 附加属性 | `false` | 启用库支持的虚拟化方案；仍需有限视口和模型数据源，不预建全部控件。 |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

- `DataGrid`: `Classes="borderless"`
- `ListBox`: `Classes="borderless"` / `Classes="cards"`
- `TabControl`: `Classes="card"` / `Classes="pills"`

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`TabFeatures`](../../api/zh-cn/TheOne.Avalonia.Controls.TabFeatures.md) | `static bool TabFeatures.Close(TabStripItem item)` | 尝试关闭容器对应的数据项 |
| [`TabFeatures`](../../api/zh-cn/TheOne.Avalonia.Controls.TabFeatures.md) | `static bool TabFeatures.Close(TabItem item)` | 尝试关闭容器对应的数据项 |
| [`TabFeatures`](../../api/zh-cn/TheOne.Avalonia.Controls.TabFeatures.md) | `static bool TabFeatures.Move(TabControl tabs, int from, int to)` | 按零基索引尝试重排 |
| [`Virtualization`](../../api/zh-cn/TheOne.Avalonia.Controls.Virtualization.md) | `static TreeViewItem? Virtualization.BringIntoView(TreeView tree, params int[] path)` | Materializes only the ancestor path and target of a large tree. |
| [`Virtualization`](../../api/zh-cn/TheOne.Avalonia.Controls.Virtualization.md) | `static void Virtualization.SelectAll(TreeView tree)` | Selects all nodes exposed by expanded branches without creating their containers. |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.TabFeatures`](../../api/zh-cn/TheOne.Avalonia.Controls.TabFeatures.md) | 6 |
| [`TheOne.Avalonia.Controls.Virtualization`](../../api/zh-cn/TheOne.Avalonia.Controls.Virtualization.md) | 1 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
