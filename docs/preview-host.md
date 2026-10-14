# PreviewHost 结构与页面规范

正式预览界面按基础控件、数据展示、导航与布局、图表控件、扩展控件、消息与窗口六类组织。同类控件集中展示，避免为每种按钮或列表单独设置菜单项：

- 基础控件：按钮（Button、ToggleButton、RepeatButton、SplitButton、ToggleSplitButton）、文本与输入、选择与开关、数值与进度、日期与时间。
- 数据展示：Label、SelectableTextBlock、ListBox、ItemsControl、TreeView、Carousel、InfoCard 在同一页展示，DataGrid 保留独立页面。
- 导航与布局：标签与内容切换、菜单与浮层、滚动与分隔分别归组；Expander、SplitView 保留独立页面。
- 图表：14 种类型合并为趋势图（折线、面积、散点）、柱状图（普通、堆叠、百分比堆叠）、条形图（普通、堆叠、百分比堆叠）、堆叠面积图（普通、百分比）、饼图与环形图、仪表图六页。每种类型在同一区块同时展示三种样式，数据实验按需展开，并锁定原生图表类型。
- 扩展控件新增 Tag 状态标签和 Step 验证步骤；Step 由 StepItem<T> 自动生成当前表单和字段错误，提供标准属性验证、完成命令与 1,000 步回收演示。
- 消息与窗口保留独立页面。

组合页面同时展示每个控件的样式与状态，例如按钮的主要／次要／图标／禁用及尺寸对比，列表与树的默认／自定义内容模板，轮播的完整／相邻页面卡片。页面内名称按钮用于滚动定位，搜索具体控件名称也会跳到相应区块，重复搜索当前页面中的控件同样有效。

## 目录

- `Views/<分类>/`：页面 XAML、代码后置。保留控件生命周期、焦点、滚动、动画完成及布局测量等 UI 适配。
- `ViewModels/<分类>/`：输入值、选择、开关、示例数据、命令和操作结果。直接继承 ReactiveObject；控件页可继承具有实际状态的 BasicControlViewModel。输入与可选择状态使用明确的 TwoWay 编译绑定。
- `Models/<分类>/`：行数据、树节点和内容模型。
- `Infrastructure/`：共享 ViewModel／命令基类、显式页面目录与控件工厂、窗口流程。
- `Verification/<分类>/`：Headless 交互与 Skia 验证。
- `Verification/Fixtures/`：仍被回归调用的旧 Gallery、状态对比视图、DataGrid 实验及其 ViewModel／布局支持代码；App 与登录后主窗口使用正式组合页与专用页面。

未被引用的单图表 Page 和早期 A/B/C 设计场景已删除；当前图表由 ChartShowcaseView 与按需创建的 ChartDemoView 展示。历史方案仅保留静态截图，参见[设计依据](design/README.md)。

入口和项目文件保留在项目根目录。所有类型仍使用 TheOne.DesignPreview 命名空间，便于原生消费项目链接共享的编译 XAML 与专项验证。

## 页面与绑定

`Infrastructure/Navigation/PreviewPageCatalog.cs` 明确注册页面元数据与工厂，不反射查找 View、不使用 Activator。Shell ViewModel 仅持有导航与设置；ControlGallery 根据当前页面按需创建 View，并保留已经访问的状态。ShowcasePage 组合固定数量的子 View，每个控件继续使用自己的 ViewModel 与名称作用域。数据表格维持 660 高度的有界布局，各类增长列表保持有限视口、ItemsSource 和官方虚拟化面板。

编译绑定需要 x:DataType。每种基础页面有独立 ViewModel，共享 BasicControlViewModel 的常用字段。复杂页面单独维护专有 ViewModel。纯 UI 操作请求由 ViewModel 发出语义事件，View 负责滚动、聚焦或建立原生窗口；ViewModel 不保存控件引用。

### ReactiveUI 与设计器类型

展示目录不再包含 OneWindow、LoginWindow、NotificationCard；相关回归保留在 Verification。ThemeButton 位于 Button 内，IconButton 在按钮分类中单独展示。Toast、Dialog、Notification 的独立页提供消息类型、标题与显示行为配置，通过 MVVM 编译绑定直接控制实际宿主。

PreviewHost 使用 `ReactiveUI.Avalonia 12.1.4`，启动通过 `UseReactiveUI(static _ => { })` 注册平台服务。可变绑定属性使用 .NET 10 的 field 与 RaiseAndSetIfChanged，派生属性显式 RaisePropertyChanged；不保留 PreviewViewModel 的 Set / NotifyChanged 封装。命令直接使用 ReactiveCommand.Create<object?>，异步使用 CreateFromTask<object?>，不保留 PreviewCommand 或自定义 ICommand 实现。绑定属性保留 ICommand 接口，传入原始参数及 null；无参数操作通过 `_ => Action()` 忽略附带参数。

登录／分裂按钮的 CanExecute 使用 Changed 按属性名过滤、Select 强类型属性、StartWith 显式数组、DistinctUntilChanged；扩展来自 ReactiveUI.Primitives，不使用反射表达式或另加 Rx 依赖。异步命令在等待期间保持 IsExecuting，控件按 CanExecute 禁止重复触发，完成后恢复；代码直接调用 ICommand.Execute 时应先检查 CanExecute。错误可通过 ThrownExceptions 观察。DialogEditorViewModel 仍继承框架独立的 DialogContext，field 属性使用其 SetProperty，保留关闭／结果合约；命令直接使用 ReactiveCommand。类库不依赖 ReactiveUI，消费方可以选择自己的 MVVM 框架。

`ItemsSource` 的集合类型不会决定绑定源的 ViewModel 类型，代码后置设置 DataContext 也不能替代 XAML 的编译类型声明。每个绑定作用域声明对应 `x:DataType` 后，可直接绑定 VM 的属性：

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:YourApp.ViewModels"
             x:DataType="vm:MembersViewModel">
  <ListBox Height="320" ItemsSource="{Binding Rows}"
           SelectedItem="{Binding SelectedRow, Mode=TwoWay}" />
</UserControl>
```

DataTemplate 内部声明行模型的 x:DataType；重新赋值 DataContext 的弹层／子区域声明新的 VM 类型。不要把未经转换的 `#Root.DataContext` 当作强类型源：Avalonia 的 DataContext 属性声明为 object，设计器可能因此把后续 Rows 等字段解析为 object 成员。Flyout 预览使用有类型的 Model 属性传递上下文：

```xml
<StackPanel DataContext="{Binding #FlyoutRoot.Model}"
            x:DataType="local:FlyoutDemoViewModel">
  <ListBox Height="248" ItemsSource="{Binding Rows}" />
</StackPanel>
```

若消费端必须引用 DataContext，可在绑定路径中明确转换 VM 类型，例如 `#Root.((vm:MembersViewModel)DataContext).Rows`，不要关闭编译绑定。静态合约检查会拒绝预览中丢失类型的 object 上下文桥接；页面专项检查 ReactiveObject 通知、ReactiveCommand 参数与控件／VM 双向同步。依赖接入参照 [ReactiveUI 官方 Avalonia 文档](https://www.reactiveui.net/documentation/reactiveui/handbook/data-binding/avalonia/)。

PreviewHost、主题与消费示例启用 `AvaloniaUseCompiledBindingsByDefault=true`，所以 `{Binding Property}` 本身就是编译绑定。视图根节点声明一次 VM 类型：

```xml
<UserControl x:DataType="local:MenuViewModel" ...>
  <one:NavMenu ItemsSource="{Binding MenuRoots}"
               ChildrenBinding="{Binding Entries}"
               ExpandedBinding="{Binding IsExpanded, Mode=TwoWay}"
               SearchTextBinding="{Binding Title}" />
</UserControl>
```

运行时仍需设置 DataContext；x:DataType 提供编译器和 IDE 所需的静态类型，无法从代码中任意赋值的 object DataContext 自动获知。NavMenu 的节点绑定及图标／内容模板使用 InheritDataTypeFromItems，从强类型集合（如 ObservableCollection<MenuNode>）推断节点类型。DataGrid 的列也保留官方的项类型推断，强类型 ItemsSource 不必逐列填写 DataType。预览表格使用 DataGridCollectionView<GridRecord>，保留原生筛选、排序与分组，列无需声明 x:DataType。页面还提供 List<GridRecord>／ObservableCollection<GridRecord> 千条集合切换、动态增项和单元格按钮／开关操作。非泛型 DataGridCollectionView 仍需声明行类型。

当前 Rider 对 Avalonia 12 的未限定 `{CompiledBinding ...}` 会混淆运行时 `Avalonia.Data.CompiledBinding` 和 XAML 扩展类型。命名空间限定的 `{binding:CompiledBinding ...}` 可以编译并消除此类解析错误，但实测仍未正常显示源类型。因此消费示例采用默认编译的 `{Binding ...}`，确保 IDE 能推断 VM 属性；无需手写 CompiledBindingExtension。此限制不能通过控件的运行时 DataContext 修复。

共享主题的 MultiBinding 内保留原生 CompiledBindingExtension 元素；源路径使用 `Path="$self.((TextBox)TemplatedParent).Text"` 或 `Path="#PART_TextPresenter.PreeditText"`，避免把编译器合成的 RelativeSource / ElementName 当成扩展的 CLR 属性。

Rider 预览通过 Program 的公开无参数 `BuildAvaloniaApp()` 启动；带 headless 参数的内部工厂继续供测试使用。模板内自定义面板的名称使用 x:Name，使设计器与原生命名作用域一致。静态检查保护视图类型作用域及模板的明确源路径；相关设计器操作见 [Rider 官方 Avalonia 说明](https://www.jetbrains.com/help/rider/Avalonia.html)。

导航目录使用 NavMenu，窄窗口仍保留图标导航。搜索结果为列表选择，结果显示控件名和所属组合页；搜索选择定位区块，页面内快捷入口只滚动，不替换控件类型。

图表区块使用 `ChartShowcaseView`：三种固定样式按可用宽度换行，包含平滑曲线、数值标签、单系列、不同环宽、选中扇区及仪表指针／刻度等变化。首次展开交互区才创建完整的 ChartDemoView；切换页面后保留其数据与配置，搜索任意图表类型会定位到所属组合页的相应区块。

## 验证入口

页面组织改动使用 `--verify-preview-pages`，控件行为选择对应专项；命令与升级条件统一见[测试策略](workflow/testing.md)。图表组合定向渲染名称为 ChartGallery；DataGrid 集合绑定与按钮为 DataGridBinding；DataGrid 筛选为 DataGridFilters，右侧冻结为 DataGridRightFrozen，底部统计为 DataGridSummary；冻结实验为 DataGridStage1，桌面实验入口为 `--datagrid-stage1`。

交互预览保留 Light / Dark × 三尺寸、有限视口与实际容器计数。测试运行日志保存 `.preview/<任务>/`，不在结构文档累计历次结果。原生消费边界见 [NativeAOT](native-aot.md)。
