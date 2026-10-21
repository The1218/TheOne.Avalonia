# TheOne.Avalonia

[简体中文](README.zh-cn.md) · [English](README.en.md)

基于 Avalonia 官方控件的跨平台主题与控件库，内置明暗主题、三档尺寸、多种控件外观与渐变配色。PreviewHost 提供可交互的控件示例。

- 为按钮、输入、选择、日期时间、列表、树、菜单和布局控件提供统一主题。
- 提供 21 种图表，以及 Form/FormItem、MultiSelectBox、TreeSelect、NavMenu、InfoCard、Description、Tag、VerificationStep、窗口与消息扩展。
- DataGrid 支持编译绑定、自动实体属性筛选、左右冻结列与底部数值统计。
- QRCode、ReportDesign/ReportPreview、WorkflowEditor、GanttChart 和 NavPage 提供报表、工作流及工作区扩展，参见 [preview.11 发行说明](docs/releases/zh-cn/1.0.0-preview.11.md)。


本次统一图标宿主的 OneIcon 接入，修复 Geometry／模板动态切换时的尺寸与禁用前景，并修正 ImageUpload 查看弹层的滚动条问题；包含双语图文指南。NuGet 包需要维护者手动上传，当前源码版本不表示包已上架。

## 环境与版本

| 项目 | 版本 |
| --- | --- |
| .NET SDK / TargetFramework | .NET 10 / `net10.0` |
| Avalonia 核心、Skia、Fluent | 12.1.4 |
| DataGrid | 基于 12.1.2，包名 `TheOne.Avalonia.DataGrid` |
| TheOne.Avalonia / TheOne.Avalonia.DataGrid | 1.0.0-preview.11 |
| PreviewHost MVVM | ReactiveUI.Avalonia 12.1.4 |

## 快速开始

克隆并运行交互预览：

```sh
git clone https://github.com/The1218/TheOne.Avalonia.git
cd TheOne.Avalonia
dotnet run --project design/PreviewHost
```

在消费项目中安装预览包，配套 DataGrid 包会自动还原：

```sh
dotnet add path/to/YourApp.csproj package TheOne.Avalonia --version 1.0.0-preview.11
```

远程包：[TheOne.Avalonia](https://www.nuget.org/packages/TheOne.Avalonia/1.0.0-preview.11) · [TheOne.Avalonia.DataGrid](https://www.nuget.org/packages/TheOne.Avalonia.DataGrid/1.0.0-preview.11)。也可引用源码（路径按项目位置调整）：

```sh
dotnet add path/to/YourApp.csproj reference TheOne.Avalonia/TheOne.Avalonia.csproj
```

消费项目使用 `net10.0`，Avalonia 核心包版本统一为 12.1.4。在 `App.axaml` 中加载主题，替换 `YourApp.App` 为实际应用类：

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:one="using:TheOne.Avalonia.Themes"
             x:Class="YourApp.App"
             RequestedThemeVariant="Default">
  <Application.Styles>
    <one:OneTheme />
  </Application.Styles>
</Application>
```

`OneTheme` 已包含 Fluent 支持层与 DataGrid 模板，不要同时引用官方 `Avalonia.Controls.DataGrid` 包。接入后即可使用官方控件：

```xml
<StackPanel Spacing="12" Classes="normal">
  <TextBox Classes="clearButton" PlaceholderText="请输入名称" />
  <Button Classes="primary" Content="保存" />
  <CalendarDatePicker SelectedDateFormat="Custom"
                      CustomDateFormatString="yyyy-MM-dd" />
</StackPanel>
```

`RequestedThemeVariant="Default"` 跟随系统，`Light` / `Dark` 强制指定主题；`Classes="small"`、`normal`、`large` 可设置控件或整个容器的密度。

## 文档

- [项目接入与迁移](docs/integration/zh-cn/README.md)：外部项目改造、跨平台边界及可复制给 Codex 的任务说明。
- [文档总入口](docs/README.md)：按任务和语言选择文档。
- [完整源码契约](docs/api/zh-cn/README.md)：保留全部公开声明，包含构造、属性访问器与标识符，供精确查阅。
- [控件用法](docs/controls/zh-cn/README.md)：本项目专属属性、Classes 标签样例、实际业务 API 与关联模型。PreviewHost 为属性与业务动作附短示例代码；ControlSizing、Motion 等通用配置集中在 Home 说明。
- [主题文档](docs/theme/zh-cn/README.md)：配色、样式、尺寸与动效。

## 按需引入的独立包

| 可选包 | 作用 | 主题注册 |
| --- | --- | --- |
| `TheOne.Avalonia.CodeEditor` | 独立代码编辑器，提供语法高亮、补全、诊断和格式化 | `CodeEditorTheme` |
| `TheOne.PrintingTools.Avalonia` | 桌面打印预览、打印机设置、PDF 导出及 Windows / macOS / Linux 打印后端 | `PrintingToolsTheme` |

这两个包均为**按需单独引入**，不会随 TheOne.Avalonia 自动安装，源码、版本与文档各自维护。PreviewHost 显式引用 CodeEditor 1.0.0-preview.2 展示源码高亮、PrintingTools 1.0.0-preview.3 展示打印：前者在 App.axaml 注册主题，后者在打印窗口局部注册主题。普通消费应用不必引入这些预览依赖，也不依赖相邻仓库，详见[接入指南](docs/integration/zh-cn/README.md#主库与预览程序的依赖不同)。

## 仓库结构

| 路径 | 用途 |
| --- | --- |
| `TheOne.Avalonia/` | 公共主题、控件与行为 |
| `design/PreviewHost/` | 交互预览与消费示例 |
| `docs/controls/` | 控件文档 |
| `docs/theme/` | 主题文档 |

## 许可

[MIT](LICENSE) · [第三方许可声明](THIRD-PARTY-NOTICES.md)
