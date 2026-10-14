# NavMenu 多级导航

`NavMenu` 继承官方 `TreeView`，使用 `NavMenuItem` 容器和 Still 主题。支持多级菜单、独立图标与文字模板、页面选择、分组标题、原生键盘导航，以及所有层级的虚拟化和容器回收。加载 `OneTheme` 后自动支持 Light / Dark 和三种尺寸。

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

根级和每一级子菜单均使用原生 `VirtualizingStackPanel`，并保留 `PART_*`、焦点、选择和展开逻辑。需要有限高度或位于 `Grid` 的有界 `*` 行中；不要将无限增长的导航放进无高度限制的 `StackPanel` 或外层无限 ScrollViewer。虚拟化减少 UI 容器，不等于业务数据分页。

PreviewHost 主侧栏已采用 NavMenu，分为基础控件、数据展示、导航与布局、图表控件、扩展控件、交互验证六区。扩展控件中的“导航菜单 NavMenu”演示每级 1,000 条、共 5,000 个模型，可跳到末尾根节点、二级及三级页面，实时查看实际容器数量。

执行命令与测试范围见[测试策略](../workflow/testing.md)。

专项覆盖明暗主题和三种尺寸、实际容器数量、远距离选择与键盘导航、回收后的展开状态、编译模板更新、动态子项、禁用状态、分组不导航、自定义模板操作、实时尺寸及主题切换。独立原生消费项目复用同一编译 XAML 和交互验证，不使用反射绑定、动态 XAML 或整程序集保留。

## 按功能查阅

- [侧栏折叠、悬停级联与边缘适配](nav-menu-popups.md)。
- [多级搜索、高亮、键盘与顶部自动完成](nav-menu-search.md)。
- [展开与收起动效](nav-menu-motion.md)。

验证入口及原生专项选择见[测试策略](../workflow/testing.md)。
