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
| `TabFeatures.IsClosable` | 设置在单个 TabItem 上，False 可固定标签 |
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
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 在各控件页直接标明示例的 Classes、尺寸及状态；公共属性、API／事件和模型字段使用 Description 组件说明，包含属性类型、默认值、绑定模式、校验条件及 API 签名。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### TabFeatures

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.TabFeatures.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanClose`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | `bool` | 可写; OneWay; 附加属性 | `false` | 允许关闭；可显式设为 False |
| [`CanReorder`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L22) | `bool` | 可写; OneWay; 附加属性 | `false` | 允许当前 TabControl 内拖动与键盘排序 |
| [`CloseCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | `System.Windows.Input.ICommand?` | 可写; OneWay; 附加属性 | `null` | 宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`CloseTabCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L35) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `new CloseTab()` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`IsClosable`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L23) | `bool` | 可写; OneWay; 附加属性 | `true` | 设置在单个 TabItem 上，False 可固定标签 |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | `bool` | 可写; OneWay; 附加属性 | `false` | 显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |

### Virtualization

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.Virtualization.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L15) | `bool` | 可写; OneWay; 附加属性 | `false` | 启用库支持的虚拟化方案；仍需有限视口和模型数据源，不预建全部控件。 |

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
| DataGrid | `borderless` | 显式 Classes，可与尺寸类组合 | ListBox.borderless, DataGrid.borderless => BorderThickness=0 |
| DataGrid | `one-large` | 模板自动类；不手动绑定 | DataGrid.one-large => FontSize=16 |
| DataGrid | `one-normal` | 模板自动类；不手动绑定 | DataGrid.one-normal => FontSize=14 |
| DataGrid | `one-small` | 模板自动类；不手动绑定 | DataGrid.one-small => FontSize=12 |
| ListBox | `borderless` | 显式 Classes，可与尺寸类组合 | ListBox.borderless, DataGrid.borderless => BorderThickness=0 |
| ListBox | `cards` | 显式 Classes，可与尺寸类组合 | ListBox.cards => Background={DynamicResource One.Subtle}; BorderThickness=0; Padding=8; ListBox.cards > ListBoxItem => Margin=0,3; CornerRadius={DynamicResource One.Radius.Control}; BorderThickness=1; BorderBrush={DynamicResource One.Line}; ListBox.cards > ListBoxItem /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Surface}; ListBox.cards > ListBoxItem:pointerover /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Subtle}; ListBox.cards > ListBoxItem:selected /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentSoft}; BorderBrush={DynamicResource One.Accent}; Foreground={DynamicResource One.Text}; ListBox.cards > ListBoxItem:disabled /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.DisabledSurface}; Foreground={DynamicResource One.DisabledText}; BorderBrush={DynamicResource One.Line} |
| ListBox | `one-large` | 模板自动类；不手动绑定 | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| ListBox | `one-normal` | 模板自动类；不手动绑定 | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| ListBox | `one-small` | 模板自动类；不手动绑定 | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |
| TabControl | `card` | 显式 Classes，可与尺寸类组合 | ^.card => BorderBrush={DynamicResource One.CardTabBorder}; BorderThickness=1; CornerRadius={DynamicResource One.Radius.TabContent}; Padding=16; ItemContainerTheme={StaticResource One.CardTabItem}; ItemsPanel=template/resource; Template={StaticResource One.CardTabControlTemplate}; TabControl.card => one:TabFeatures.CanClose=True; one:TabFeatures.CanReorder=True |
| TabControl | `one-large` | 模板自动类；不手动绑定 | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| TabControl | `one-normal` | 模板自动类；不手动绑定 | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| TabControl | `one-small` | 模板自动类；不手动绑定 | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |
| TabControl | `pills` | 显式 Classes，可与尺寸类组合 | TabControl.pills > TabItem => CornerRadius=999; Margin=0,0,8,0; TabControl.pills > TabItem /template/ Border#PART_SelectedPipe => IsVisible=False; TabControl.pills > TabItem:selected /template/ Border#PART_LayoutRoot => Background={DynamicResource One.Accent}; TabControl.pills > TabItem:selected /template/ ContentPresenter#PART_ContentPresenter => Foreground={DynamicResource One.OnAccent}; TabControl.pills > TabItem:selected:pointerover /template/ Border#PART_LayoutRoot => Background={DynamicResource One.AccentHover}; TabControl.pills > TabItem:selected:pressed /template/ Border#PART_LayoutRoot => Background={DynamicResource One.AccentPressed}; TabControl.pills > TabItem:disabled /template/ Border#PART_LayoutRoot => Background={DynamicResource One.DisabledSurface}; TabControl.pills > TabItem:disabled /template/ ContentPresenter#PART_ContentPresenter => Foreground={DynamicResource One.DisabledText} |
| TabStrip | `one-large` | 模板自动类；不手动绑定 | TabStrip.one-large, TabStripItem.one-large => FontSize=16 |
| TabStrip | `one-normal` | 模板自动类；不手动绑定 | TabStrip.one-normal, TabStripItem.one-normal => FontSize=14 |
| TabStrip | `one-small` | 模板自动类；不手动绑定 | TabStrip.one-small, TabStripItem.one-small => FontSize=12 |
| TreeView | `one-large` | 模板自动类；不手动绑定 | ToolTip.one-large, TabControl.one-large, TreeView.one-large, ListBox.one-large, Menu.one-large, ContextMenu.one-large, Calendar.one-large => FontSize=16 |
| TreeView | `one-normal` | 模板自动类；不手动绑定 | ToolTip.one-normal, TabControl.one-normal, TreeView.one-normal, ListBox.one-normal, Menu.one-normal, ContextMenu.one-normal, Calendar.one-normal => FontSize=14 |
| TreeView | `one-small` | 模板自动类；不手动绑定 | ToolTip.one-small, TabControl.one-small, TreeView.one-small, ListBox.one-small, Menu.one-small, ContextMenu.one-small, Calendar.one-small => FontSize=12 |
| TreeViewItem | `one-large` | 模板自动类；不手动绑定 | :is(Button).one-large, ToggleButton.one-large, RepeatButton.one-large, TextBox.one-large, ComboBox.one-large, ComboBoxItem.one-large, AutoCompleteBox.one-large, NumericUpDown.one-large, ButtonSpinner.one-large, CheckBox.one-large, RadioButton.one-large, ToggleSwitch.one-large, ListBoxItem.one-large, TreeViewItem.one-large, MenuItem.one-large, TabItem.one-large, CalendarDatePicker.one-large, DatePicker.one-large, TimePicker.one-large, Expander.one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16; TreeViewItem.one-large => Padding=0,4,14,4; TreeViewItem.one-large /template/ ToggleButton#PART_ExpandCollapseChevron => Width=44; Height=44 |
| TreeViewItem | `one-no-motion` | 模板自动类；不手动绑定 | ToggleSwitch.one-no-motion /template/ Thumb, Expander.one-no-motion /template/ Path, TreeViewItem.one-no-motion /template/ Path, one&#124;NavMenuItem.one-no-motion /template/ Path => Transitions={x:Null} |
| TreeViewItem | `one-normal` | 模板自动类；不手动绑定 | :is(Button).one-normal, ToggleButton.one-normal, RepeatButton.one-normal, TextBox.one-normal, ComboBox.one-normal, ComboBoxItem.one-normal, AutoCompleteBox.one-normal, NumericUpDown.one-normal, ButtonSpinner.one-normal, CheckBox.one-normal, RadioButton.one-normal, ToggleSwitch.one-normal, ListBoxItem.one-normal, TreeViewItem.one-normal, MenuItem.one-normal, TabItem.one-normal, CalendarDatePicker.one-normal, DatePicker.one-normal, TimePicker.one-normal, Expander.one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14; TreeViewItem.one-normal => Padding=0,4,10,4; TreeViewItem.one-normal /template/ ToggleButton#PART_ExpandCollapseChevron => Width=36; Height=36 |
| TreeViewItem | `one-small` | 模板自动类；不手动绑定 | :is(Button).one-small, ToggleButton.one-small, RepeatButton.one-small, TextBox.one-small, ComboBox.one-small, ComboBoxItem.one-small, AutoCompleteBox.one-small, NumericUpDown.one-small, ButtonSpinner.one-small, CheckBox.one-small, RadioButton.one-small, ToggleSwitch.one-small, ListBoxItem.one-small, TreeViewItem.one-small, MenuItem.one-small, TabItem.one-small, CalendarDatePicker.one-small, DatePicker.one-small, TimePicker.one-small, Expander.one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12; TreeViewItem.one-small => Padding=0,4,10,4; TreeViewItem.one-small /template/ ToggleButton#PART_ExpandCollapseChevron => Width=30; Height=30 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.TabFeatures`](../../api/zh-cn/TheOne.Avalonia.Controls.TabFeatures.md) | Class | 6 | 22 | 0 |
| [`TheOne.Avalonia.Controls.Virtualization`](../../api/zh-cn/TheOne.Avalonia.Controls.Virtualization.md) | Class | 1 | 5 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
