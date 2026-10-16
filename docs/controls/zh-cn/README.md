# 控件使用文档

**简体中文** · [English](../en/README.md)

TheOne.Avalonia 为官方 Avalonia 控件提供主题与行为，并提供窗口、消息、InfoCard 信息卡片与 Chart 图表扩展。先按[仓库 README](../../../README.zh-cn.md#快速开始)引用类库并加载 `OneTheme`，再使用本目录的示例。无需把业务视图迁移成 TheOne 包装控件。

## 文档导航

[完整公共 API 与内置模型](../../api/zh-cn/README.md)列出每个声明成员、初值、绑定模式、校验条件、样式选择器及源码链接。下列每个指南均补充对应的额外属性和样式清单。PreviewHost 在每个控件自己的页面完整展示示例，并附样式、额外属性、API／事件及内置模型；仅 Button、Input、Select 合并比较。属性调整使用类型化访问器并保留 StyledProperty 绑定。

外部项目首次接入或迁移，先读[项目接入与迁移](../../integration/zh-cn/README.md)，其中包含版本、依赖、跨平台边界及可复制给 Codex 的任务说明。

| 文档 | 控件 / 能力 |
| --- | --- |
| [Form 与 FormItem 表单](form.md) | 普通表单、字段标签、共享校验、错误摘要、异步取消与校验后提交 |
| [MultiSelectBox 多选框](multi-select-box.md) | 搜索、多选标签、清空、禁选项、键盘操作与虚拟化 |
| [ColorPicker / ColorView 颜色选择器](color-picker.md) | 官方颜色控件的 TheOne 外观、色谱、调色板、RGB/HSV、透明度与 Hex 输入 |
| [WorkflowEditor 流程编辑器](workflow-editor.md) | 图模型契约、模板、业务配置、上游数据引用和 JSON 保存加载 |
| [MessageBox 确认框](message-box.md) | 轻量确认／取消、默认焦点、命令与布尔结果 |
| [Upload 上传](upload.md) | 单文件选择、图片预览、替换／删除及宿主上传命令 |
| [IconButton 与按钮层次](icon-button.md) | 属性图标、Geometry／PathIcon／Material 模板、主次操作与语义状态 |
| [分裂按钮](split-button.md) | SplitButton、ToggleSplitButton、主操作、菜单、键盘和虚拟回收 |
| [文本展示](text-display.md) | TextBlock、Label、SelectableTextBlock、选择复制和字段关联 |
| [按钮、输入、选择与反馈](basic.md) | Button、RepeatButton、ToggleButton、TextBox、CheckBox、RadioButton、ToggleSwitch、ComboBox、AutoCompleteBox、NumericUpDown、ButtonSpinner、Slider、ProgressBar、ToolTip |
| [日期与时间](date-time.md) | Calendar、CalendarDatePicker、DatePicker、TimePicker、格式、日期时间 / 月份 / 年份输入 |
| [列表、树、标签与表格](collections.md) | ItemsControl、ListBox、TreeView、TabControl、TabStrip、Card 标签、DataGrid、回收与远距离定位 |
| [DataGrid 扩展](datagrid-extensions.md) | 自动实体属性筛选、多列组合、右侧冻结、底部数值统计 |
| [DataGrid 绑定与排序](datagrid-binding.md) | 类型推断、注册元数据、编译编辑、排序按钮和虚拟化 |
| [DataGrid 树视图原型](datagrid-tree.md) | 本地树、同级排序、祖先保留筛选、树统计与原生编辑 |
| [加载控件](loading.md) | LoadingIndicator、按钮加载和 LoadingContainer |
| [菜单与布局](layout.md) | Menu、ContextMenu、MenuFlyout、Flyout、FlyoutPresenter、Expander、SplitView、GridSplitter、ScrollViewer、ScrollBar、Separator |
| [轮播与刷新](collection-content.md) | Carousel、RefreshContainer、RefreshVisualizer |
| [内容过渡与通知](content-feedback.md) | TransitioningContentControl、NotificationCard |
| [NavMenu](nav-menu.md) | 导航、折叠、级联、搜索与动效，按功能继续查阅 |
| [LoginWindow](login-window.md) | 登录交接与退出生命周期 |
| [窗口 owner 与消息服务](window-owners.md) | 原生归属、默认路由、线程、Toast／Notification、Dialog 队列与取消、登录交接 |
| [主题实现说明](../../theme/zh-cn/README.md) | 资源、模板、动画、验证范围与实现约束 |
| [OneWindow 与窗口服务](windows.md) | 主题主窗口、ThemeButton、LeftContent、RightContent、静态通知／Toast／对话框 API |
| [ThemeColorPicker / ThemeButton](../../theme/zh-cn/theme-color-picker.md) | 配色下拉选择、独立明暗图标开关与 PreviewHost 代码高亮 |
| [Chart 图表](charts.md) | 十四种图形、堆叠／双轴／采样、视口导航、共享提示与图例、平滑／动画／PNG 导出、指针仪表 |
| [InfoCard 信息卡片](info-card.md) | Header、图标、操作、Content、Footer、hove / shadow、编译模板与三档尺寸 |
| [ImagePreview 图片预览](image-preview.md) | 网络地址、Bitmap、本地路径、单图／多图、缩放平移和全屏 |
| [GanttChart 甘特图](gantt-chart.md) | 只读任务层级、进度与里程碑、原生虚拟滚动和编译绑定 |
| [ReportDesign 报表设计器](report-design.md) | 自由拖放、固定页眉页脚、接口／图片地址、MVVM JSON 保存与 PDF |
| [ReportPreview 报表预览](report-preview.md) | JSON 只读报表、接口数据与图片、分页浏览、PDF 导出及桌面打印 |
| [RelationGraph 树形关系图](relation-graph.md) | 单根树、横纵布局、默认 / 自定义模型、ChildrenSource 编译绑定、SelectItem 选择与空值布局 |
| [Pagination 分页](pagination.md) | 标准／紧凑布局、每页条数、跳页与边界状态 |
| [FlowStep 与 FlowNode](flow-step.md) | 横向／纵向流程、三种状态、默认与自定义内容模板 |
| [Avatar 头像](avatar.md) | 本地与 HTTP(S) 图片、文字／图标占位、三种形状与异步加载生命周期 |
| [Tag 与 VerificationStep](tag-verification-step.md) | 图标状态标签、模型属性自动表单与单步验证导航 |

ComboBoxItem、ListBoxItem、TreeViewItem、TabItem、MenuItem、日历单元格和表格内部 Presenter 的样式由主题提供。动态集合优先绑定模型，由模板按需创建容器。

## 示例约定

XAML 片段放入已加载 `OneTheme` 的视图中。使用行为与尺寸属性时，在视图根元素添加：

```xml
xmlns:one="using:TheOne.Avalonia.Controls"
xmlns:styling="using:TheOne.Avalonia.Styling"
xmlns:vm="using:YourApp.ViewModels"
```

包含 `{Binding ...}` 的视图需要设置对应的 `DataContext`；项目启用编译绑定时，在视图根元素声明 `x:DataType="vm:YourViewModel"`，在数据模板或绑定容器样式上声明模型类型。文档中的命令、集合和模型属性由消费项目实现；变化中的属性需通知 `INotifyPropertyChanged`，变化中的集合可使用 `ObservableCollection<T>`。

## 主题与尺寸

```csharp
using Avalonia;
using Avalonia.Styling;

Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
// ThemeVariant.Light：浅色；ThemeVariant.Default：跟随系统。
```

```xml
<StackPanel Classes="small" Spacing="8">
  <TextBox PlaceholderText="继承小尺寸" />
  <Button Classes="large primary" Content="单独使用大尺寸" />
</StackPanel>
```

也可直接设置 `styling:ControlSizing.Size="Large"`，对应枚举为 `Small` / `Normal` / `Large`。尺寸向后代继承；标准交互高度为 30 / 36 / 44 px，多行文本和展开内容仍按实际内容布局。显式 `Height`、`Padding` 等局部值优先。

自定义表面与文字使用动态语义资源，使颜色随主题切换：

```xml
<Border Background="{DynamicResource One.Surface}"
        BorderBrush="{DynamicResource One.Line}"
        BorderThickness="1" CornerRadius="12" Padding="16">
  <TextBlock Foreground="{DynamicResource One.Text}" Text="业务内容" />
</Border>
```

常用资源还有 `One.Subtle`、`One.Accent`、`One.AccentSoft`、`One.OnAccent`；完整键值见 [Colors.axaml](../../../TheOne.Avalonia/Themes/Colors.axaml) 和 [Metrics.axaml](../../../TheOne.Avalonia/Themes/Metrics.axaml)。只加载一个 `OneTheme`，无需另外叠加 FluentTheme 或 DataGrid Fluent 样式。

## 集合与动画的共同约定

- 可增长集合使用 `ItemsSource` 与数据模板，不预创建大量 Control。
- 给列表有限视口：放入有界 Grid 的 `*` 行，或设置 `Height` / `MaxHeight`。不要把大列表放入无限高度的外层 StackPanel 或 ScrollViewer。
- 保留主题的虚拟化面板；DataGrid 使用自己的官方行与单元格 Presenter。
- 需要持久化的展开与选择状态保存在模型或 ViewModel 中。
- 用 `styling:Motion.Enabled="False"` 关闭某个控件子树的主题动画，或用 `Motion.SetEnabled(control, false)` 在代码中设置。

可运行的完整示例见 [PreviewHost](../../../design/PreviewHost)，大数据入口包含容器计数与远距离操作。

- [加载状态与 LoadingContainer](loading.md)：Button／IconButton 加载、内容覆盖与 Motion 开关。
