# 控件图鉴与配图说明

[English](../en/visual-index.md) · [控件入口](README.md)

配图来自本库实际控件与 PreviewHost 示例，使用 Avalonia 12.1.4 Headless + Skia 渲染，不是设计稿或浏览器重画。窗口图片包含客户区和自绘标题栏，不包含操作系统装饰。静态截图用于说明外观，不证明原生输入、动画帧率、打印或跨平台运行。

默认使用 Nova、Normal、Light / Dark；图中显式标注的 Classes 或局部配置优先。共享截图保留预览程序的中文演示文字，英文文档提供英文图注。列表和图标画廊显示当前可见范围；图片不是完整数据目录。弹层单独取景，保留真实主题、选择及项目留白。

业务数据为演示数据。附件状态和进度只是模型输入，没有发起上传；确认框没有执行删除；报表和工作流没有调用外部业务接口。各指南仍以实际公开属性、短代码、模型和行为说明为准。

## 快速看图

| 导航工作区 | 状态与样式 |
| --- | --- |
| [![NavPage 工作区](../images/NavPage-light.png)](nav-page.md) | [![Tag 状态样式](../images/Tag-light.png)](tag-verification-step.md) |
| [![Heatmap 热力图](../images/HeatmapChart-light.png)](chart-specialized.md) | [![ReportDesign 设计器](../images/ReportDesign-light.png)](report-design.md) |

## 使用顺序

1. 从下表查看外观及明暗对照。
2. 进入对应指南，先看图注，再复制最小代码，按需配置本库扩展属性。
3. 到完整 API 文档核对签名；在 PreviewHost 实际操作，确认自己的数据、主题和目标平台。

## 全部控件与场景

| 控件 / 场景 | Light | Dark | 用法 |
| --- | --- | --- | --- |
| AreaChart | [Light](../images/AreaChart-light.png) | [Dark](../images/AreaChart-dark.png) | [说明](chart-cartesian.md) |
| AttachmentList | [Light](../images/AttachmentList-light.png) | [Dark](../images/AttachmentList-dark.png) | [说明](attachment-list.md) |
| AutoCompleteBox | [Light](../images/AutoCompleteBox-light.png) | [Dark](../images/AutoCompleteBox-dark.png) | [说明](basic.md) |
| Avatar | [Light](../images/Avatar-light.png) | [Dark](../images/Avatar-dark.png) | [说明](avatar.md) |
| BarChart | [Light](../images/BarChart-light.png) | [Dark](../images/BarChart-dark.png) | [说明](chart-cartesian.md) |
| BoxPlotChart | [Light](../images/BoxPlotChart-light.png) | [Dark](../images/BoxPlotChart-dark.png) | [说明](chart-statistical.md) |
| Breadcrumb | [Light](../images/Breadcrumb-light.png) | [Dark](../images/Breadcrumb-dark.png) | [说明](breadcrumb.md) |
| Button | [Light](../images/Button-light.png) | [Dark](../images/Button-dark.png) | [说明](basic.md) |
| ButtonSpinner | [Light](../images/ButtonSpinner-light.png) | [Dark](../images/ButtonSpinner-dark.png) | [说明](basic.md) |
| Calendar | [Light](../images/Calendar-light.png) | [Dark](../images/Calendar-dark.png) | [说明](date-time.md) |
| CalendarDatePicker | [Light](../images/CalendarDatePicker-light.png) | [Dark](../images/CalendarDatePicker-dark.png) | [说明](date-time.md) |
| CandlestickChart | [Light](../images/CandlestickChart-light.png) | [Dark](../images/CandlestickChart-dark.png) | [说明](chart-statistical.md) |
| Carousel | [Light](../images/Carousel-light.png) | [Dark](../images/Carousel-dark.png) | [说明](collection-content.md) |
| CheckBox | [Light](../images/CheckBox-light.png) | [Dark](../images/CheckBox-dark.png) | [说明](basic.md) |
| ColorPicker | [Light](../images/ColorPicker-light.png) | [Dark](../images/ColorPicker-dark.png) | [说明](color-picker.md) |
| ColorView | [Light](../images/ColorView-light.png) | [Dark](../images/ColorView-dark.png) | [说明](color-picker.md) |
| ColorView · Custom 精细调色 | [Light](../images/ColorViewCustom-light.png) | [Dark](../images/ColorViewCustom-dark.png) | [说明](color-picker.md) |
| ComboBox | [Light](../images/ComboBox-light.png) | [Dark](../images/ComboBox-dark.png) | [说明](basic.md) |
| ComboBox · 下拉展开 | [Light](../images/ComboBoxPopup-light.png) | [Dark](../images/ComboBoxPopup-dark.png) | [说明](basic.md) |
| ContextMenu | [Light](../images/ContextMenu-light.png) | [Dark](../images/ContextMenu-dark.png) | [说明](layout.md) |
| DataGrid | [Light](../images/DataGrid-light.png) | [Dark](../images/DataGrid-dark.png) | [说明](collections.md) |
| DataGridTree | [Light](../images/DataGridTree-light.png) | [Dark](../images/DataGridTree-dark.png) | [说明](datagrid-tree.md) |
| DatePicker | [Light](../images/DatePicker-light.png) | [Dark](../images/DatePicker-dark.png) | [说明](date-time.md) |
| DateTimePicker | [Light](../images/DateTimePicker-light.png) | [Dark](../images/DateTimePicker-dark.png) | [说明](datetime-picker.md) |
| Description | [Light](../images/Description-light.png) | [Dark](../images/Description-dark.png) | [说明](description.md) |
| Dialog | [Light](../images/Dialog-light.png) | [Dark](../images/Dialog-dark.png) | [说明](window-owners.md) |
| DoughnutChart | [Light](../images/DoughnutChart-light.png) | [Dark](../images/DoughnutChart-dark.png) | [说明](chart-radial.md) |
| Expander | [Light](../images/Expander-light.png) | [Dark](../images/Expander-dark.png) | [说明](layout.md) |
| FlowStep | [Light](../images/FlowStep-light.png) | [Dark](../images/FlowStep-dark.png) | [说明](flow-step.md) |
| FlyoutPresenter | [Light](../images/FlyoutPresenter-light.png) | [Dark](../images/FlyoutPresenter-dark.png) | [说明](layout.md) |
| Form | [Light](../images/Form-light.png) | [Dark](../images/Form-dark.png) | [说明](form.md) |
| FunnelChart | [Light](../images/FunnelChart-light.png) | [Dark](../images/FunnelChart-dark.png) | [说明](chart-specialized.md) |
| GanttChart | [Light](../images/GanttChart-light.png) | [Dark](../images/GanttChart-dark.png) | [说明](gantt-chart.md) |
| GaugeChart | [Light](../images/GaugeChart-light.png) | [Dark](../images/GaugeChart-dark.png) | [说明](chart-radial.md) |
| GridSplitter | [Light](../images/GridSplitter-light.png) | [Dark](../images/GridSplitter-dark.png) | [说明](layout.md) |
| HeatmapChart | [Light](../images/HeatmapChart-light.png) | [Dark](../images/HeatmapChart-dark.png) | [说明](chart-specialized.md) |
| HorizontalBarChart | [Light](../images/HorizontalBarChart-light.png) | [Dark](../images/HorizontalBarChart-dark.png) | [说明](chart-cartesian.md) |
| IconButton | [Light](../images/IconButton-light.png) | [Dark](../images/IconButton-dark.png) | [说明](icon-button.md) |
| ImagePreview | [Light](../images/ImagePreview-light.png) | [Dark](../images/ImagePreview-dark.png) | [说明](image-preview.md) |
| ImageUpload | [Light](../images/ImageUpload-light.png) | [Dark](../images/ImageUpload-dark.png) | [说明](image-upload.md) |
| InfoCard | [Light](../images/InfoCard-light.png) | [Dark](../images/InfoCard-dark.png) | [说明](info-card.md) |
| ItemsControl | [Light](../images/ItemsControl-light.png) | [Dark](../images/ItemsControl-dark.png) | [说明](collections.md) |
| Label | [Light](../images/Label-light.png) | [Dark](../images/Label-dark.png) | [说明](text-display.md) |
| LineChart | [Light](../images/LineChart-light.png) | [Dark](../images/LineChart-dark.png) | [说明](chart-cartesian.md) |
| ListBox | [Light](../images/ListBox-light.png) | [Dark](../images/ListBox-dark.png) | [说明](collections.md) |
| LoadingContainer | [Light](../images/LoadingContainer-light.png) | [Dark](../images/LoadingContainer-dark.png) | [说明](loading.md) |
| LoginWindow | [Light](../images/LoginWindow-light.png) | [Dark](../images/LoginWindow-dark.png) | [说明](login-window.md) |
| Menu | [Light](../images/Menu-light.png) | [Dark](../images/Menu-dark.png) | [说明](layout.md) |
| MessageBox | [Light](../images/MessageBox-light.png) | [Dark](../images/MessageBox-dark.png) | [说明](message-box.md) |
| MultiSelectBox | [Light](../images/MultiSelectBox-light.png) | [Dark](../images/MultiSelectBox-dark.png) | [说明](multi-select-box.md) |
| MultiSelectBox · 搜索与多选列表 | [Light](../images/MultiSelectBoxPopup-light.png) | [Dark](../images/MultiSelectBoxPopup-dark.png) | [说明](multi-select-box.md) |
| NavMenu | [Light](../images/NavMenu-light.png) | [Dark](../images/NavMenu-dark.png) | [说明](nav-menu-motion.md) |
| NavMenu · 折叠弹层 | [Light](../images/NavMenuPopup-light.png) | [Dark](../images/NavMenuPopup-dark.png) | [说明](nav-menu-popups.md) |
| NavMenu · 多级搜索 | [Light](../images/NavMenuSearch-light.png) | [Dark](../images/NavMenuSearch-dark.png) | [说明](nav-menu-search.md) |
| NavPage | [Light](../images/NavPage-light.png) | [Dark](../images/NavPage-dark.png) | [说明](nav-page.md) |
| NotificationCard | [Light](../images/NotificationCard-light.png) | [Dark](../images/NotificationCard-dark.png) | [说明](content-feedback.md) |
| NumericUpDown | [Light](../images/NumericUpDown-light.png) | [Dark](../images/NumericUpDown-dark.png) | [说明](basic.md) |
| OneIcon | [Light](../images/OneIcon-light.png) | [Dark](../images/OneIcon-dark.png) | [说明](one-icon.md) |
| OneNotification | [Light](../images/OneNotification-light.png) | [Dark](../images/OneNotification-dark.png) | [说明](content-feedback.md) |
| OneWindow | [Light](../images/OneWindow-light.png) | [Dark](../images/OneWindow-dark.png) | [说明](window-owners.md) |
| Pagination | [Light](../images/Pagination-light.png) | [Dark](../images/Pagination-dark.png) | [说明](pagination.md) |
| PercentStackedAreaChart | [Light](../images/PercentStackedAreaChart-light.png) | [Dark](../images/PercentStackedAreaChart-dark.png) | [说明](chart-cartesian.md) |
| PercentStackedBarChart | [Light](../images/PercentStackedBarChart-light.png) | [Dark](../images/PercentStackedBarChart-dark.png) | [说明](chart-cartesian.md) |
| PercentStackedHorizontalBarChart | [Light](../images/PercentStackedHorizontalBarChart-light.png) | [Dark](../images/PercentStackedHorizontalBarChart-dark.png) | [说明](chart-cartesian.md) |
| PieChart | [Light](../images/PieChart-light.png) | [Dark](../images/PieChart-dark.png) | [说明](chart-radial.md) |
| ProgressBar | [Light](../images/ProgressBar-light.png) | [Dark](../images/ProgressBar-dark.png) | [说明](basic.md) |
| QRCode | [Light](../images/QRCode-light.png) | [Dark](../images/QRCode-dark.png) | [说明](qr-code.md) |
| RadarChart | [Light](../images/RadarChart-light.png) | [Dark](../images/RadarChart-dark.png) | [说明](chart-specialized.md) |
| RadioButton | [Light](../images/RadioButton-light.png) | [Dark](../images/RadioButton-dark.png) | [说明](basic.md) |
| RefreshContainer | [Light](../images/RefreshContainer-light.png) | [Dark](../images/RefreshContainer-dark.png) | [说明](collection-content.md) |
| RelationGraph | [Light](../images/RelationGraph-light.png) | [Dark](../images/RelationGraph-dark.png) | [说明](relation-graph.md) |
| RepeatButton | [Light](../images/RepeatButton-light.png) | [Dark](../images/RepeatButton-dark.png) | [说明](basic.md) |
| ReportDesign | [Light](../images/ReportDesign-light.png) | [Dark](../images/ReportDesign-dark.png) | [说明](report-design.md) |
| ReportPreview | [Light](../images/ReportPreview-light.png) | [Dark](../images/ReportPreview-dark.png) | [说明](report-preview.md) |
| ScatterChart | [Light](../images/ScatterChart-light.png) | [Dark](../images/ScatterChart-dark.png) | [说明](chart-cartesian.md) |
| ScrollBar | [Light](../images/ScrollBar-light.png) | [Dark](../images/ScrollBar-dark.png) | [说明](layout.md) |
| ScrollViewer | [Light](../images/ScrollViewer-light.png) | [Dark](../images/ScrollViewer-dark.png) | [说明](layout.md) |
| SelectableTextBlock | [Light](../images/SelectableTextBlock-light.png) | [Dark](../images/SelectableTextBlock-dark.png) | [说明](text-display.md) |
| Separator | [Light](../images/Separator-light.png) | [Dark](../images/Separator-dark.png) | [说明](layout.md) |
| Slider | [Light](../images/Slider-light.png) | [Dark](../images/Slider-dark.png) | [说明](basic.md) |
| SplitButton | [Light](../images/SplitButton-light.png) | [Dark](../images/SplitButton-dark.png) | [说明](split-button.md) |
| SplitView | [Light](../images/SplitView-light.png) | [Dark](../images/SplitView-dark.png) | [说明](layout.md) |
| StackedAreaChart | [Light](../images/StackedAreaChart-light.png) | [Dark](../images/StackedAreaChart-dark.png) | [说明](chart-cartesian.md) |
| StackedBarChart | [Light](../images/StackedBarChart-light.png) | [Dark](../images/StackedBarChart-dark.png) | [说明](chart-cartesian.md) |
| StackedHorizontalBarChart | [Light](../images/StackedHorizontalBarChart-light.png) | [Dark](../images/StackedHorizontalBarChart-dark.png) | [说明](chart-cartesian.md) |
| SunburstChart | [Light](../images/SunburstChart-light.png) | [Dark](../images/SunburstChart-dark.png) | [说明](chart-hierarchy.md) |
| TabControl | [Light](../images/TabControl-light.png) | [Dark](../images/TabControl-dark.png) | [说明](collections.md) |
| TabStrip | [Light](../images/TabStrip-light.png) | [Dark](../images/TabStrip-dark.png) | [说明](collections.md) |
| Tag | [Light](../images/Tag-light.png) | [Dark](../images/Tag-dark.png) | [说明](tag-verification-step.md) |
| TextBox | [Light](../images/TextBox-light.png) | [Dark](../images/TextBox-dark.png) | [说明](basic.md) |
| ThemeButton | [Light](../images/ThemeButton-light.png) | [Dark](../images/ThemeButton-dark.png) | [说明](windows.md) |
| ThemeColorPicker | [Light](../images/ThemeColorPicker-light.png) | [Dark](../images/ThemeColorPicker-dark.png) | [说明](../../theme/zh-cn/theme-color-picker.md) |
| ThemeColorPicker · 配色列表 | [Light](../images/ThemeColorPickerPopup-light.png) | [Dark](../images/ThemeColorPickerPopup-dark.png) | [说明](../../theme/zh-cn/theme-color-picker.md) |
| TimePicker | [Light](../images/TimePicker-light.png) | [Dark](../images/TimePicker-dark.png) | [说明](date-time.md) |
| Timeline | [Light](../images/Timeline-light.png) | [Dark](../images/Timeline-dark.png) | [说明](timeline.md) |
| Toast | [Light](../images/Toast-light.png) | [Dark](../images/Toast-dark.png) | [说明](window-owners.md) |
| ToggleButton | [Light](../images/ToggleButton-light.png) | [Dark](../images/ToggleButton-dark.png) | [说明](basic.md) |
| ToggleSplitButton | [Light](../images/ToggleSplitButton-light.png) | [Dark](../images/ToggleSplitButton-dark.png) | [说明](split-button.md) |
| ToggleSwitch | [Light](../images/ToggleSwitch-light.png) | [Dark](../images/ToggleSwitch-dark.png) | [说明](basic.md) |
| ToolTip | [Light](../images/ToolTip-light.png) | [Dark](../images/ToolTip-dark.png) | [说明](basic.md) |
| TransitioningContentControl | [Light](../images/TransitioningContentControl-light.png) | [Dark](../images/TransitioningContentControl-dark.png) | [说明](content-feedback.md) |
| TreeSelect | [Light](../images/TreeSelect-light.png) | [Dark](../images/TreeSelect-dark.png) | [说明](tree-select.md) |
| TreeSelect · 树形弹层 | [Light](../images/TreeSelectPopup-light.png) | [Dark](../images/TreeSelectPopup-dark.png) | [说明](tree-select.md) |
| TreeView | [Light](../images/TreeView-light.png) | [Dark](../images/TreeView-dark.png) | [说明](collections.md) |
| TreemapChart | [Light](../images/TreemapChart-light.png) | [Dark](../images/TreemapChart-dark.png) | [说明](chart-hierarchy.md) |
| Upload | [Light](../images/Upload-light.png) | [Dark](../images/Upload-dark.png) | [说明](upload.md) |
| VerificationStep | [Light](../images/VerificationStep-light.png) | [Dark](../images/VerificationStep-dark.png) | [说明](tag-verification-step.md) |
| WorkflowEditor | [Light](../images/WorkflowEditor-light.png) | [Dark](../images/WorkflowEditor-dark.png) | [说明](workflow-editor.md) |

## 维护与证据

[截图清单](../images/capture-manifest.json)记录场景、主题和像素尺寸。截图源码及临时构建位于 `.preview/docs-illustrated-20261021/`，不修改正常 Program/App 入口。图文区位于 `visual-guide` 标记内、公共契约生成区之外；重新生成 API 文档不会覆盖图注。

更改控件外观后，仅重拍受影响图片；同时检查对应的 Light / Dark、图注、来源和链接。只读状态、非可视模型与辅助 API 不伪造独立截图，而是链接到使用该契约的实际控件场景。
