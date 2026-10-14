# TheOne 控件库：第一阶段设计提案

这份文档保留最初设计阶段的记录。当前正式主题与可操作预览已经实现，最新接入方式和控件范围见 [主题文档](../theme/README.md)。

本阶段完成设计探索与 NuGet 基础接入。设计源文件是可编译的 Avalonia XAML，截图由 Avalonia.Headless + Skia 生成。扩展控件为视觉样例，尚未提供公共 API、完整交互或可发布主题。

## 设计判断

推荐 **B / Still**：青绿强调色、暖白浅色背景、石墨深色背景，搭配 8 px 控件圆角与 12 px 面板圆角。视觉重心放在内容和交互状态上，适合跨平台使用，也能建立 TheOne 自己的识别。

| 方向 | 视觉语言 | 控件 / 面板圆角 | 适合场景 |
| --- | --- | --- | --- |
| A / Clarity | 蓝色、原生感、完整侧栏 | 6 / 8 px | 企业应用与密集表单 |
| B / Still | 青绿、柔和中性色、克制边框 | 8 / 12 px | 通用跨平台控件库，推荐 |
| C / Signal | 琥珀、较重标题、无侧栏布局 | 4 / 4 px | 创作工具、开发工具 |

三套提案展示相同内容：按钮与状态、输入与校验、选择与切换、标签输入与步骤条与进度环、数据与状态、通知与错误提示。每套都有 1280 × 800、800 × 600 的浅色和深色图，共 12 张。紧凑布局会折叠侧栏并排列为单列；截图呈现可滚动页面的首屏。

## 历史评审资料

- 打开 [静态预览](../design/previews/index.html)，切换主题和窗口尺寸，点击图片放大。页面和图片无需联网。
- 早期 XAML 与设计导出脚本已移除；这里仅保留当时的方案与静态评审结果。
- 本地浏览：`python3 -m http.server 8765 --bind 127.0.0.1 --directory docs/design/previews`。
- 构建：`dotnet build TheOne.Avalonia.slnx -c Release`。

当前正式预览由 PreviewHost 提供；临时验证结果保存在已忽略的 `.preview/`，历史截图保存在 `docs/design/previews/`。

## 主题约定

资源以 `One.` 为前缀，避免与宿主应用的资源键冲突。后续正式主题采用三层模型：原始色阶 → 语义资源 → 控件资源。

| 类别 | 约定 |
| --- | --- |
| 背景 | `One.Canvas`、`One.Surface`、`One.Subtle` |
| 文字 | `One.Text`、`One.Muted`、`One.OnAccent` |
| 边界 | `One.Line` 是分区线；`One.InputLine` 是交互边界 |
| 强调色 | `One.Accent`、`One.AccentHover`、`One.AccentPressed`、`One.AccentSoft` |
| 状态 | `One.Success`、`One.Warning`、`One.Danger`，并配合文字或图标 |
| 字体 | 英文采用随包嵌入的 Inter；中文使用平台字体回退 |
| 字阶 | 12 px 注释、14 px 正文、16–20 px 分组、28–40 px 页面标题 |
| 间距 | 以 4 / 8 为基础节奏；预览面板内边距 16 px，正式主题统一为密度资源 |
| 交互尺寸 | 默认 36 px；预览状态样本为 30 px，正式触控密度至少 44 px |
| 焦点 | 2 px 强调色边框；正式控件支持键盘操作与可见焦点 |
| 动效 | 后续悬停 120 ms、面板出现 180 ms；提供关闭动效选项 |

浅色 / 深色资源分别放在 `ThemeDictionaries`，主题相关属性通过 `DynamicResource` 引用。固定圆角、字体和尺寸使用 `StaticResource`。预览按钮已使用独立 `ControlTheme`，其余基础控件暂以 Fluent 为预览基底。正式主题在方向确定后逐个提供自己的模板与交互状态，避免把预览覆盖样式当作完成的控件库。

## 包与项目分层

保留初始项目的 `net10.0`，不在第一阶段改变框架目标。

| 项目 | 直接依赖 | 用途 |
| --- | --- | --- |
| TheOne.Avalonia | Avalonia 12.1.4、Avalonia.Skia 12.1.4 | 控件与未来自绘扩展的基础依赖 |
| design/PreviewHost | Avalonia.Desktop、Avalonia.Headless、Avalonia.Themes.Fluent、Avalonia.Fonts.Inter，均 12.1.4 | 真实 XAML 的设计渲染宿主 |
| design/PreviewHost，仅 Debug | AvaloniaUI.DiagnosticsSupport 2.2.3 | 新版诊断工具支持 |

`Avalonia.Skia` 提供匹配的 SkiaSharp 传递依赖，未另外固定可能冲突的旧 SkiaSharp。`Avalonia.Desktop`、Fluent 和诊断工具没有加入公共类库。已启用默认编译绑定；未来绑定的视图与数据模板需提供 `x:DataType`。

## 正式控件实现顺序

1. **主题基础**：`OneTheme` 入口、色阶、语义资源、字阶、密度、浅色 / 深色 / 跟随系统。
2. **基础控件**：Button、ToggleButton、TextBox、ComboBox、CheckBox、RadioButton、ToggleSwitch、Slider、ProgressBar、TabControl、ScrollViewer。覆盖默认、悬停、按下、焦点、禁用、错误和加载状态。
3. **第一批扩展**：Badge、Tag / TagInput、Notice、Toast、StepBar、ProgressRing。优先采用 `TemplatedControl` 和可样式化的 `StyledProperty`。
4. **第二批扩展**：FormField、Drawer、CommandPalette、Skeleton、EmptyState、Pagination。先确定键盘模型、焦点恢复、状态和布局，再定公共 API。
5. **展示应用与验证**：组件页、状态页、键盘操作、主题切换、不同 DPI，以及 Windows / macOS / Linux 检查。

## 已验证与边界

当时的设计导出脚本检查了 12 张 PNG 的尺寸，并检查三套主题的主要正文颜色对比度 ≥ 4.5:1、焦点与输入边框 ≥ 3:1；具体结果见 `contrast-report.json`。分区线不作为唯一交互边界，不要求达到控件边框对比度。

这些检查不能替代完整可访问性验证。尚未验证真实 GPU 路径、屏幕阅读器、触控、200% 字体缩放及三桌面平台表现；中文字体回退也会随平台变化。预览中的扩展组件和数据表是设计组合，搜索、分段按钮与通知没有业务交互。

下一步建议以 B / Still 为基础，先实现主题入口和基础控件，再逐项实现扩展控件。
