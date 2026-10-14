# 主题配色维护

**简体中文** · [English](../en/README.md)

OneTheme 是类库默认主题，使用 `One.*` 全库资源，包含 DataGrid。[加载](../../../README.zh-cn.md#快速开始) · [配色与变体](theme-colors.md) · [控件 API](../../controls/zh-cn/README.md)。

## 官方模板

| 分组 | 控件 |
| --- | --- |
| 操作 | Button、ToggleButton、RepeatButton、SplitButton、ToggleSplitButton |
| 文本与数值输入 | TextBox、NumericUpDown、ButtonSpinner、AutoCompleteBox |
| 选择 | ComboBox、ComboBoxItem、CheckBox、RadioButton、ToggleSwitch |
| 数值与反馈 | Slider、ProgressBar、ToolTip、NotificationCard |
| 日期时间 | CalendarDatePicker、Calendar、CalendarItem、日期单元格、DatePicker、TimePicker，以及日期时间弹层 Presenter |
| 导航与列表 | TabControl、TabItem、TabStrip、TabStripItem、Carousel、ItemsControl、ListBox、ListBoxItem、TreeView、TreeViewItem、DataGrid 及其单元格／表头／行／行标题／分组标题 |
| 菜单 | Menu、MenuItem、ContextMenu、MenuFlyoutPresenter，以及菜单滚动辅助模板 |
| 标签与文本 | Label、SelectableTextBlock |
| 内容弹层 | FlyoutPresenter（Flyout 的原生内容容器） |
| 布局与滚动 | Expander、SplitView、GridSplitter、TransitioningContentControl、RefreshContainer、RefreshVisualizer、ScrollViewer、ScrollBar、Separator |

基础模板基于 Avalonia **12.1.4** 官方 Fluent 源码适配；Calendar 系列及 Card 模板为本库自行实现。模板保留 `PART_*`、输入 Presenter、验证容器、Popup、Track、Spinner、滚动手势与原生键盘行为。颜色、圆角、默认尺寸、辅助主题键和视觉状态已按 TheOne 重写。来源与 MIT 许可见仓库根目录的 `THIRD-PARTY-NOTICES.md`，该文件也纳入打包内容。

## 样式接口

| 控件 | Classes / 属性 | 用途 |
| --- | --- | --- |
| Button | `primary`，兼容 `accent` | 强调色主操作 |
| Button / IconButton | `outline`、`ghost`、`link`、`success`、`info`、`warning`、`danger` | 轻操作与语义状态 |
| 官方控件 / 容器 | `small`、`normal`、`large` | 统一三档密度，支持继承及单控件覆盖 |
| TabControl | `card` | 无边框标题、正文边框、关闭与排序 |
| ToggleButton | `segment` | 分段选择外观 |
| TextBox | `clearButton` | 清除输入 |
| TextBox | `revealPasswordButton` | 显示 / 隐藏密码 |
| TextBox | `error` | 显式错误外观；真实绑定错误也触发 `:error` |
| TextBox | `IsReadOnly` | 次级背景表示只读 |
| 交互控件 | `IsEnabled`、官方伪类 | 悬停、按下、焦点、禁用与选择状态 |
| TransitioningContentControl | `PageTransition`、`IsTransitionReversed`、`Motion.Enabled` | 原生内容过渡、反向及可继承的动画开关 |
| NotificationCard | `NotificationType`、`Motion.Enabled` | 提示／成功／警告／错误、显示与关闭动画 |

```xml
<Button Classes="primary" Content="保存" Command="{Binding SaveCommand}" />
<TextBox Classes="clearButton" Text="{Binding Email, Mode=TwoWay}" />
<TextBox Classes="revealPasswordButton" PasswordChar="●" />
```

## 资源与尺寸

| 文件／类型 | 维护内容 |
| --- | --- |
| `Themes/Colors.axaml` | Light / Dark 的 One.* 语义色与画刷 |
| `Themes/Metrics.axaml`、`Sizing.axaml` | 字号、高度、圆角、间距及密度 |
| `Themes/Controls/*.axaml` | 控件模板与辅助主题 |
| `Themes/Styles.axaml`、`Motion.axaml` | 样式类、焦点、错误与画刷过渡 |
| `Themes/OneTheme.axaml` | 主题入口与资源合并 |
| `Controls/RevealHost.cs` | 树／Expander 裁切展开、占位与反向动画 |
| `Controls/TabFeatures.cs`、`CardTabChrome.cs`、`CardTabStrip.cs` | Card 标签行为与轮廓 |

画刷使用 DynamicResource，可由宿主覆盖。调整强调色需同步 One.Accent、AccentHover、AccentPressed、AccentSoft、OnAccent。正文对比度至少 4.5:1，焦点至少 3:1。Nova 原色有边框及按下文字对比度例外，见[配色说明](theme-colors.md)；报告为 [contrast-report.json](../contrast-report.json)。

Classes="small|normal|large" 或 ControlSizing.Size 向后代继承，单控件可覆盖。标准交互高度 30／36／44 px；ProgressBar 厚度 4／6／8 px，日历日期格 32／40／48 px。多行文本和展开内容按实际布局；显式 Height、MinHeight、Padding 遵循属性优先级。尺寸资源可覆盖 One.Size.Height、One.Size.Small.Height、One.Size.Large.Height；图表另有[尺寸参数](../../controls/zh-cn/chart-rendering.md#主题与三档尺寸)。

## 动效与模板合约

Motion.Enabled 可在容器设置并继承；关闭或零时长立即完成当前状态，重新启用不重播已展开内容。画刷默认 120 ms，展开 200 ms，收起 140 ms；可覆盖 One.Motion.Fast／One.Motion.Expand。当前不自动读取系统减少动画偏好。

模板使用 TemplateBinding／编译绑定，MultiBinding 子项显式 CompiledBindingExtension；不能以关闭编译绑定兼容旧示例。保留官方 PART_*、输入 Presenter、Popup、验证、Track、Spinner、手势及键盘行为。模板升级需比较官方部件与绑定变化。来源与许可见 [第三方声明](../../../THIRD-PARTY-NOTICES.md)。

集合保留有界视口、按需模板和专用面板；回收检查入口见 [PreviewHost](../../../design/PreviewHost/Program.cs)。DataGrid 有限 MaxColumnWidth 在创建列前配置，AOT 要求见 [DataGrid 控件文档](../../controls/zh-cn/datagrid-binding.md#nativeaot)。

## 对应控件文档

- [输入／按钮／选择](../../controls/zh-cn/basic.md) · [日历与日期时间](../../controls/zh-cn/date-time.md)。
- [树、列表、Card 标签与表格](../../controls/zh-cn/collections.md) · [轮播与刷新](../../controls/zh-cn/collection-content.md)。
- [菜单、浮层与布局](../../controls/zh-cn/layout.md) · [内容过渡与通知](../../controls/zh-cn/content-feedback.md)。
- [窗口与消息](../../controls/zh-cn/windows.md) · [图表](../../controls/zh-cn/charts.md)。

## RelationGraph

`RelationGraph` 与 `RelationGraphNodePresenter` 使用共享语义颜色及可继承的 Small / Normal / Large 尺寸。根节点使用 `:root`；空区域可见性使用 `:has-icon`、`:has-title`、`:has-content`、`:icon-only`。通过基于默认节点主题的 `NodeTheme` 调整卡片。参见[控件契约与验证命令](../../controls/zh-cn/relation-graph.md)。

## FlowStep 与 FlowNode

FlowStep 主题包含不接收输入的连接线层 `PART_Connectors`、原生 `PART_ItemsPresenter` 与 `PART_ScrollViewer`。FlowNode 保留 `PART_ContentPresenter`，并通过 `PART_Indicator`、`PART_Ring`、`PART_Check`、`PART_Dot` 展示状态。状态伪类为 `:completed`、`:in-progress`、`:pending`、`:vertical`。默认内容资源键为 `One.FlowNode.DefaultContentTemplate`；共享语义画刷与继承尺寸支持明暗主题及三档尺寸。横向空间不足时滚动，纵向按内容真实高度布局；连接线层不拦截输入。模板优先级、尺寸与专项验证见 [FlowStep 与 FlowNode](../../controls/zh-cn/flow-step.md)。

GanttChart 使用独立 `One.Gantt.*` 动态画刷、36／44／52 行高和固定表头；资源、模板 PART 与窄视口契约见 [甘特图](../../controls/zh-cn/gantt-chart.md)。

InfoCard 支持 `hove` / `shadow` 类，外阴影采用 `One.InfoCard.Shadow` / `One.InfoCard.HoverShadow`，由不拦截输入的 `PART_Shadow` 绘制；详见 [InfoCard](../../controls/zh-cn/info-card.md)。RelationGraph 的节点主题支持只读 IsSelected / :selected 状态；选择不会改变节点尺寸，详见 [节点选择](../../controls/zh-cn/relation-graph.md#节点选择)。
