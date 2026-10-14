# 控件使用文档

TheOne.Avalonia 为官方 Avalonia 控件提供主题与行为，并提供窗口、消息、InfoCard 信息卡片与 Chart 图表扩展。先按[仓库 README](../../README.md#快速开始)引用类库并加载 `OneTheme`，再使用本目录的示例。无需把业务视图迁移成 TheOne 包装控件。

## 文档导航

| 文档 | 控件 / 能力 |
| --- | --- |
| [IconButton 与按钮层次](icon-button.md) | 属性图标、Geometry／PathIcon／Material 模板、主次操作与语义状态 |
| [分裂按钮](split-button.md) | SplitButton、ToggleSplitButton、主操作、菜单、键盘和虚拟回收 |
| [文本展示](text-display.md) | TextBlock、Label、SelectableTextBlock、选择复制和字段关联 |
| [按钮、输入、选择与反馈](basic.md) | Button、RepeatButton、ToggleButton、TextBox、CheckBox、RadioButton、ToggleSwitch、ComboBox、AutoCompleteBox、NumericUpDown、ButtonSpinner、Slider、ProgressBar、ToolTip |
| [日期与时间](date-time.md) | Calendar、CalendarDatePicker、DatePicker、TimePicker、格式、日期时间 / 月份 / 年份输入 |
| [列表、树、标签与表格](collections.md) | ItemsControl、ListBox、TreeView、TabControl、TabStrip、Card 标签、DataGrid、回收与远距离定位 |
| [DataGrid 扩展](datagrid-extensions.md) | 自动实体属性筛选、多列组合、右侧冻结、底部数值统计 |
| [DataGrid 绑定与排序](datagrid-binding.md) | 类型推断、注册元数据、编译编辑、排序按钮和虚拟化 |
| [加载控件](loading.md) | LoadingIndicator、按钮加载和 LoadingContainer |
| [菜单与布局](layout.md) | Menu、ContextMenu、MenuFlyout、Flyout、FlyoutPresenter、Expander、SplitView、GridSplitter、ScrollViewer、ScrollBar、Separator |
| [轮播与刷新](collection-content.md) | Carousel、RefreshContainer、RefreshVisualizer |
| [内容过渡与通知](content-feedback.md) | TransitioningContentControl、NotificationCard |
| [NavMenu](nav-menu.md) | 导航、折叠、级联、搜索与动效，按功能继续查阅 |
| [LoginWindow](login-window.md) | 登录交接与退出生命周期 |
| [主题实现说明](../theme/README.md) | 资源、模板、动画、验证范围与实现约束 |
| [窗口与消息](windows.md) | OneWindow、ThemeButton、LeftContent、RightContent、静态通知／Toast／对话框 API |
| [Chart 图表](charts.md) | 十四种图形、堆叠／双轴／采样、视口导航、共享提示与图例、平滑／动画／PNG 导出、指针仪表 |
| [InfoCard 信息卡片](info-card.md) | Header、图标、操作、Content、Footer、编译模板与三档尺寸 |
| [Tag 与 Step](tag-step.md) | 图标状态标签、模型属性自动表单与单步验证导航 |

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

常用资源还有 `One.Subtle`、`One.Accent`、`One.AccentSoft`、`One.OnAccent`；完整键值见 [Colors.axaml](../../TheOne.Avalonia/Themes/Colors.axaml) 和 [Metrics.axaml](../../TheOne.Avalonia/Themes/Metrics.axaml)。只加载一个 `OneTheme`，无需另外叠加 FluentTheme 或 DataGrid Fluent 样式。

## 集合与动画的共同约定

- 可增长集合使用 `ItemsSource` 与数据模板，不预创建大量 Control。
- 给列表有限视口：放入有界 Grid 的 `*` 行，或设置 `Height` / `MaxHeight`。不要把大列表放入无限高度的外层 StackPanel 或 ScrollViewer。
- 保留主题的虚拟化面板；DataGrid 使用自己的官方行与单元格 Presenter。
- 需要持久化的展开与选择状态保存在模型或 ViewModel 中。
- 用 `styling:Motion.Enabled="False"` 关闭某个控件子树的主题动画，或用 `Motion.SetEnabled(control, false)` 在代码中设置。

可运行的完整示例见 [PreviewHost](../../design/PreviewHost)，大数据入口包含容器计数与远距离操作。

- [加载状态与 LoadingContainer](loading.md)：Button／IconButton 加载、内容覆盖与 Motion 开关。
