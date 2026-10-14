# TheOne.Avalonia

基于 Avalonia 官方控件的跨平台主题库，采用 Still 语义色，支持明暗主题与三档尺寸。公共主题／行为位于类库，PreviewHost 提供交互示例与验证。扩展包括 14 种独立图表、NavMenu、InfoCard、窗口与消息；DataGrid 支持自动实体属性筛选、原生右侧冻结与底部数值统计。

## 环境与版本

| 项目 | 版本 / 状态 |
| --- | --- |
| .NET SDK / TargetFramework | .NET 10 / `net10.0` |
| Avalonia 核心、Skia、Fluent | 12.1.4 |
| DataGrid | 12.1.2 源码扩展，包标识 TheOne.Avalonia.DataGrid |
| TheOne 双包发布序列 | 共享 PackageVersions.props；当前 1.0.0-preview.1，与上游版本分开 |
| PreviewHost MVVM | ReactiveUI.Avalonia 12.1.4 |
| 接入方式 | NuGet `1.0.0-preview.1`，或源码 `ProjectReference` |
| 许可 | [MIT](LICENSE)，第三方模板许可见 [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) |

## 快速开始

克隆并运行交互预览：

```sh
git clone https://github.com/The1218/TheOne.Avalonia.git
cd TheOne.Avalonia
dotnet run --project design/PreviewHost
```

在消费项目中安装预览包，配套 DataGrid 包会自动还原：

```sh
dotnet add path/to/YourApp.csproj package TheOne.Avalonia --version 1.0.0-preview.1
```

远程包：[TheOne.Avalonia](https://www.nuget.org/packages/TheOne.Avalonia/1.0.0-preview.1) · [TheOne.Avalonia.DataGrid](https://www.nuget.org/packages/TheOne.Avalonia.DataGrid/1.0.0-preview.1)。也可引用源码（路径按项目位置调整）：

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

`OneTheme` 已包含 Fluent 支持层与 DataGrid 模板。接入后即可使用官方控件：

```xml
<StackPanel Spacing="12" Classes="normal">
  <TextBox Classes="clearButton" PlaceholderText="请输入名称" />
  <Button Classes="primary" Content="保存" />
  <CalendarDatePicker SelectedDateFormat="Custom"
                      CustomDateFormatString="yyyy-MM-dd" />
</StackPanel>
```

`RequestedThemeVariant="Default"` 跟随系统，`Light` / `Dark` 强制指定主题；`Classes="small"`、`normal`、`large` 可设置控件或整个容器的密度。

## 文档与维护

- [文档索引](docs/README.md)：按任务读取，避免加载全部上下文。
- [控件索引](docs/controls/README.md)：接口、示例和限制。
- [主题](docs/theme/README.md) · [NativeAOT](docs/native-aot.md) · [PreviewHost](docs/preview-host.md)。
- [测试策略](docs/workflow/testing.md)：局部修改跑相关专项，公共基础设施／全面审查跑全量。
- [NuGet 版本与发布](docs/workflow/releasing.md)：双包共同版本、渠道、打包验证与交付顺序。
- [DataGrid 当前计划](docs/plans/datagrid.md)。

代码／XAML 修改后构建，并选择对应专项。例如列筛选：

```sh
dotnet build TheOne.Avalonia.slnx
dotnet run --project design/PreviewHost --no-build -- --verify-datagrid-filters
```

纯文档运行 `python3 scripts/check-docs.py`。截图仅导出当前控件，命令与测试升级条件统一见测试策略。

## 仓库结构

| 路径 | 用途 |
| --- | --- |
| `TheOne.Avalonia/` | 公共主题、控件与行为 |
| `design/PreviewHost/` | 演示、Headless 交互与定向渲染 |
| `tests/NativeAotSmoke/` | 独立原生消费验证 |
| `docs/` | 当前接口、计划及按需归档 |
| `.agents/skills/theone-maintenance/` | 项目维护 skill |
| `scripts/` | 文档、对比度、AOT 和渲染工具 |

## 验证边界

当前实跑环境为 macOS ARM64／Skia Headless。Windows／Linux、真实 GPU、屏幕阅读器、200% 字体缩放及系统高对比度尚未完成全面验证；动效开关尚未自动跟随系统减少动画偏好。未重写控件由 Fluent 支持层提供，历史设计示例不代表独立 API 已实现。UI 虚拟化减少容器创建，大数据仍需分页或懒加载。
