# 主题配色维护

**简体中文** · [English](../en/README.md)

OneTheme 是类库默认主题，使用 `One.*` 全库资源，包含 DataGrid。[加载](../../../README.zh-cn.md#快速开始) · [配色与变体](theme-colors.md) · [控件 API](../../controls/zh-cn/README.md)。

## 官方模板

完整声明见 [API 与内置模型](../../api/zh-cn/README.md)，每个控件指南末尾均列出额外属性和样式选择器。PreviewHost 在对应控件页完整展示示例，提供本页样式选择、属性编辑、事件观察与模型查看。`one-*` 等模板内部类仍由主题自动维护。

运行时配色选择与独立明暗开关见 [ThemeColorPicker / ThemeButton](theme-color-picker.md)，包含 PreviewHost 代码高亮与专项入口。

| 分组 | 控件 |
| --- | --- |
| 操作 | Button、ToggleButton、RepeatButton、SplitButton、ToggleSplitButton |
| 文本与数值输入 | TextBox、NumericUpDown、ButtonSpinner、AutoCompleteBox |
| 选择 | ComboBox、ComboBoxItem、CheckBox、RadioButton、ToggleSwitch |
| 颜色 | 官方 ColorPicker、ColorView、ColorSpectrum、ColorSlider 与 ColorPreviewer；见[使用文档](../../controls/zh-cn/color-picker.md) |
| 数值与反馈 | Slider、ProgressBar、ToolTip、NotificationCard |
| 日期时间 | CalendarDatePicker、Calendar、CalendarItem、日期单元格、DatePicker、TimePicker，以及日期时间弹层 Presenter |
| 导航与列表 | TabControl、TabItem、TabStrip、TabStripItem、Carousel、ItemsControl、ListBox、ListBoxItem、TreeView、TreeViewItem、DataGrid 及其单元格／表头／行／行标题／分组标题 |
| 菜单 | Menu、MenuItem、ContextMenu、MenuFlyoutPresenter，以及菜单滚动辅助模板 |
| 标签与文本 | Label、SelectableTextBlock |
| 内容弹层 | FlyoutPresenter（Flyout 的原生内容容器） |
| 布局与滚动 | Expander、SplitView、GridSplitter、TransitioningContentControl、RefreshContainer、RefreshVisualizer、ScrollViewer、ScrollBar、Separator |

基础模板基于 Avalonia **12.1.4** 官方 Fluent 源码适配；Calendar 系列及 Card 模板为本库自行实现。模板保留 `PART_*`、输入 Presenter、验证容器、Popup、Track、Spinner、滚动手势与原生键盘行为。颜色、圆角、默认尺寸、辅助主题键和视觉状态已按 TheOne 重写。来源与 MIT 许可见仓库根目录的 `THIRD-PARTY-NOTICES.md`，该文件也纳入打包内容。

## 样式接口

统一间距规范、控件检查范围和独立验证入口见[项表面间距](item-spacing.md)。

TreeViewItem 在节点标题表面（`PART_LayoutRoot`）使用 `Margin="4,2"`：左右各留 4px，可见行之间保留 4px 空隙。子项 Presenter 保留原生层级缩进，边距不随树深度累加。`--verify-treeview-spacing [output]` 检查标题间距、选中像素、三级缩进、指针选择、展开按钮折叠与键盘展开，覆盖明暗主题、三档尺寸和宽窄视口。

TextBox、AutoCompleteBox、NumericUpDown、ComboBox、CalendarDatePicker、DatePicker、TimePicker 和 MultiSelectBox 的常态边框统一使用可见的 1px `One.InputLine`；焦点和错误状态保留各自语义色，嵌入式编辑器保留单层外边框。ListBoxItem 和 ComboBoxItem 均使用 `Margin="4,2"`：左右各留 4px，相邻项表面之间保留 4px 空隙，继续使用原生容器和虚拟化。需要紧凑排列时，可在项上局部覆盖 Margin。`--verify-combobox-spacing [output]` 专项只检查 ComboBox 的边距、实际像素与原生选择，覆盖明暗主题、三档尺寸和宽窄视口。`--verify-listbox-spacing [output]` 检查默认及自定义内容的 ListBox 边距、选中像素、原生多选与容器回收。

PreviewHost 的 `--verify-input-appearance [output]` 专项覆盖明暗主题、三档尺寸、两种背景和宽窄视口下的输入边框像素、列表／下拉项间距及原生输入。`--verify-datagrid-filter-entries` 只检查筛选按钮显隐和运行时配置变更。

| 控件 | Classes / 属性 | 用途 |
| --- | --- | --- |
| Avatar | `Shape`、`CornerRadius`、`Stretch`、`:loading`、`:error`、`:has-image`、`:has-text` | [头像](../../controls/zh-cn/avatar.md)：32 / 40 / 56 px，共享语义颜色与图片裁剪、占位 |
| Button | `primary`，兼容 `accent` | 强调色主操作 |
| Button / IconButton | `outline`、`ghost`、`link`、`success`、`info`、`warning`、`danger` | 轻操作与语义状态 |
| 官方控件 / 容器 | `small`、`normal`、`large` | 统一三档密度，支持继承及单控件覆盖 |
| TabControl | `card` | 无边框标题、正文边框、关闭与排序 |
| TabStrip | `TabFeatures.CanClose`、`CloseCommand`、单项 `IsClosable` | 按需开启原生标题关闭及 Ctrl / ⌘ + W；见[接口](../../controls/zh-cn/collections.md#tabstrip-独立标签导航)，排序仍仅适用于 TabControl |
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
| `Controls/TabFeatures.cs`、`TabFeatures.Close.cs`、`CardTabChrome.cs`、`CardTabStrip.cs` | TabControl／TabStrip 关闭、TabControl 排序及 Card 轮廓 |

画刷使用 DynamicResource，可由宿主覆盖。调整强调色需同步 One.Accent、AccentHover、AccentPressed、AccentSoft、OnAccent。正文对比度至少 4.5:1，焦点至少 3:1。Nova 原色有边框及按下文字对比度例外，见[配色说明](theme-colors.md)；报告为 [contrast-report.json](../contrast-report.json)。

<a id="shared-configuration"></a>

## 通用主题配置

以下为跨控件通用的项目扩展。PreviewHost 在 Home 统一说明；各控件页只关注本控件附加的 Classes、属性、扩展 API 与关联模型，不列原生 Avalonia 继承 API。

| 配置 | 契约 |
| --- | --- |
| `Classes="small"`、`normal`、`large` | 公开尺寸快捷入口；`one-small`、`one-normal`、`one-large` 是模板自动类。 |
| `styling:ControlSizing.Size` | `ControlSize`，默认 `Normal`；向后代继承，可局部覆盖。枚举值：`Small`、`Normal`、`Large`。[完整契约](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)。 |
| `styling:Motion.Enabled` | `bool`，默认 `true`；向后代继承，可局部覆盖。设为 `False` 关闭子树动效。[完整契约](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)。 |

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

ImagePreview 的 PART_Viewport 直接绘制图片与透明棋盘格，导航与工具使用原生按钮；复用 One.* 语义画刷及三档密度。见 [图片预览](../../controls/zh-cn/image-preview.md)。

## Pagination

Pagination 主题保留原生 Button、ComboBox 与 TextBox。当前页使用主按钮状态，省略号不接收输入；`:compact`、`:empty`、`:invalid-jump` 表示布局、空数据与跳页校验。ItemHeight 随继承尺寸使用 30 / 36 / 44。部件、文本格式与验证见[控件契约](../../controls/zh-cn/pagination.md)。

Toast 通过 one-toast 类选择 One.ToastCard 模板，仅呈现垂直居中的状态图标与正文；NotificationCard 默认通知模板保留标题和关闭按钮。两者共享原生生命周期与动效，排版及验证见 [窗口与消息](../../controls/zh-cn/windows.md)。

## WorkflowEditor

WorkflowEditor 复用 One.* 画刷，明暗画布与注释背景使用 One.Workflow.Canvas / One.Workflow.Note。HeaderTemplate、NodeTemplate、ContentTemplate、NodeDefinitionTemplate、PropertyTemplate 保留图编辑行为并允许定制展示。默认模板包括可编辑属性面板与 Command 驱动的保存标题栏，控件默认填满父容器视口。PART_Canvas 直接绘制圆角连线和点阵，缩略图绘制轻量投影。详见[流程编辑器](../../controls/zh-cn/workflow-editor.md)。

## MessageBox 与 Upload

OneMessageBox 复用已带主题的 OneDialog Presenter 及其模态生命周期。默认内容使用 One.Accent / One.Danger 语义画刷、原生命令按钮和继承尺寸。见[确认框](../../controls/zh-cn/message-box.md)。

Upload 内置 EmptyTemplate、FileTemplate、ActionsTemplate，三个模板均接收 Upload 控件，便于编译绑定。主题使用原生 Button / Image、公共语义画刷、12 / 14 / 16 字号，以及 `:has-file`、`:loading`、`:error`、`:busy` 状态。见[上传控件](../../controls/zh-cn/upload.md)。
