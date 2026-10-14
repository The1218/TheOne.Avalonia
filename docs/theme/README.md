# Still 主题维护

加载方式见[仓库 README](../../README.md#快速开始)，控件 API 见[控件索引](../controls/README.md)。只加载一个 OneTheme；它已包含 Fluent 支持层和 DataGrid 模板。

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

基础模板基于 Avalonia **12.1.4** 官方 Fluent 源码适配；Calendar 系列及 Card 模板为本库自行实现。模板保留 `PART_*`、输入 Presenter、验证容器、Popup、Track、Spinner、滚动手势与原生键盘行为。颜色、圆角、默认尺寸、辅助主题键和视觉状态已按 Still 重写。来源与 MIT 许可见仓库根目录的 `THIRD-PARTY-NOTICES.md`，该文件也纳入打包内容。

## 样式接口

| 控件 | Classes / 属性 | 用途 |
| --- | --- | --- |
| Button | `primary`，兼容 `accent` | 强调色主操作 |
| Button / IconButton | `outline`、`ghost`、`link`、`success`、`info`、`warning`、`danger` | 轻操作与语义状态 |
| 官方控件 / 容器 | `small`、`normal`、`large` | 统一三档密度，支持继承及单控件覆盖 |
| TabControl | `card` | 连续外边框、可关闭、可拖动排序 |
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

画刷使用 DynamicResource，允许宿主在主题资源中覆盖。调整强调色需同步 One.Accent、AccentHover、AccentPressed、AccentSoft、OnAccent。正文对比度至少 4.5:1，交互边界和焦点至少 3:1；报告为 [contrast-report.json](contrast-report.json)。

Classes="small|normal|large" 或 ControlSizing.Size 向后代继承，单控件可覆盖。标准交互高度 30／36／44 px；ProgressBar 厚度 4／6／8 px，日历日期格 32／40／48 px。多行文本和展开内容按实际布局；显式 Height、MinHeight、Padding 遵循属性优先级。尺寸资源可覆盖 One.Size.Height、One.Size.Small.Height、One.Size.Large.Height；图表另有[尺寸参数](../controls/chart-rendering.md#主题与三档尺寸)。

## 动效与模板合约

Motion.Enabled 可在容器设置并继承；关闭或零时长立即完成当前状态，重新启用不重播已展开内容。画刷默认 120 ms，展开 200 ms，收起 140 ms；可覆盖 One.Motion.Fast／One.Motion.Expand。当前不自动读取系统减少动画偏好。

模板使用 TemplateBinding／编译绑定，MultiBinding 子项显式 CompiledBindingExtension；不能以关闭编译绑定兼容旧示例。保留官方 PART_*、输入 Presenter、Popup、验证、Track、Spinner、手势及键盘行为。模板升级需比较官方部件与绑定变化。来源与许可见 [第三方声明](../../THIRD-PARTY-NOTICES.md)。

集合维护保留有界视口、按需模板和原生专用面板；完整数据遍历模型，实际回收检查见[测试策略](../workflow/testing.md)。DataGrid 有限 MaxColumnWidth 在创建列前配置，AOT 要求见[NativeAOT](../native-aot.md)。

## 对应控件文档

- [输入／按钮／选择](../controls/basic.md) · [日历与日期时间](../controls/date-time.md)。
- [树、列表、Card 标签与表格](../controls/collections.md) · [轮播与刷新](../controls/collection-content.md)。
- [菜单、浮层与布局](../controls/layout.md) · [内容过渡与通知](../controls/content-feedback.md)。
- [窗口与消息](../controls/windows.md) · [图表](../controls/charts.md)。

验证与截图命令统一维护于[测试策略](../workflow/testing.md)。无需为一次局部模板调整重复全量回归。
