# 按改动范围验证

验证代码集中在 PreviewHost 及其 Consumers 子目录；项目 Agent 约定与维护 Skill 见根 AGENTS.md 和 `.agents/skills/theone-maintenance/`，不另建辅助脚本体系。命令在仓库根目录执行，日志和截图放入忽略目录 `.preview/`。

## 测试分级

| 改动 | 必要验证 |
| --- | --- |
| 纯文档 | 核对链接、示例和接口，不运行 UI 回归 |
| 局部控件行为或 XAML | 构建与相关专项 |
| 尺寸、模板、状态色 | 相关专项覆盖 Light / Dark × Small / Normal / Large；必要时定向截图 |
| 集合或虚拟化 | 相关专项检查千项容器数、远距离操作和滚动回收 |
| 绑定、裁剪或 DataGrid 元数据 | 实际 NativeAOT 发布和相关原生专项 |
| 公共主题或全面交付审查 | 全控件回归；涉及 AOT 时同时执行原生消费和原生 PreviewHost |

```sh
dotnet build TheOne.Avalonia.slnx
dotnet run --no-build --project design/PreviewHost -- --verify-datagrid-binding
```

通过后只在新增改动、失败或未解疑点需要时扩大或重复验证。

## 托管专项入口

调用方式为 `dotnet run --project design/PreviewHost -- <入口>`；源码未变可加 `--no-build`。

| 范围 | 入口 |
| --- | --- |
| DataGrid 注册元数据、自动列、标量转换、编译绑定、排序、密度与千行回收 | `--verify-datagrid-binding` |
| DataGrid 统计、筛选、冻结和原生编辑集成 | `--verify-datagrid-summary` |
| DataGrid 列筛选或右冻结 | `--verify-datagrid-filters`、`--verify-datagrid-right` |
| Tag、Step、自动属性验证 | `--verify-tag-step` |
| IconButton、ToggleButton 三态、按钮和消息 | `--verify-buttons-messages` |
| 加载状态、图标四向布局、LoadingContainer | `--verify-loading` |
| Nova 配色、外观变体和列表回收 | `--verify-control-variants` |
| 标签拖动、取消、边缘滚动和回收 | `--verify-tab-drag` |
| 输入对齐、日期时间弹层 | `--verify-text-alignment`、`--verify-pickers` |
| 图表、渲染、选择、缩放与导出 | `--verify-charts` |
| 小尺寸 X 轴月份、长标签、递减 X 和图例开关的实际像素 | `--verify-chart-axis-labels [输出目录]` |
| 仪表枚举、属性覆盖、明暗三档尺寸、窄视口、连续拖动文字稳定性、等级标签遮挡与屏幕／PNG 像素一致性 | `--verify-gauge-styles [截图目录]` |
| 窗口、登录、InfoCard | `--verify-onewindow`、`--verify-loginwindow`、`--verify-infocard` |
| NavMenu 全部、搜索、动效、筛选 | `--verify-navmenu`、`--verify-navmenu-search`、`--verify-navmenu-motion`、`--verify-navmenu-filter` |
| 轮播、刷新、展开、内容过渡 | `--verify-carousel`、`--verify-refresh`、`--verify-expander-motion`、`--verify-content-transition` |
| 浮层、通知、标题栏及导航 | `--verify-flyout`、`--verify-notifications`、`--verify-navigation-chrome` |
| 预览页面、基础控件或全控件 | `--verify-preview-pages`、`--verify-core-controls`、`--verify-controls` |

完整入口以 PreviewHost 的 `--help` 和 Program.cs 为准。登录退出生命周期的桌面专项传入 ShutdownMode，在独立进程执行。

## NativeAOT

本机 macOS ARM64 使用 Xcode clang 与 SDK，避免 PATH 中旧链接器不能读取新 SDK：

```sh
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" -o .preview/native-consumer
.preview/native-consumer/NativeAotSmoke --require-aot --verify-datagrid-binding
.preview/native-consumer/NativeAotSmoke --require-aot --verify-datagrid-summary
.preview/native-consumer/NativeAotSmoke --require-aot --verify-chart-axis-labels
```

不传专项参数时执行独立消费回归。支持的参数见 [Program.cs](../../design/PreviewHost/Consumers/NativeAotSmoke/Program.cs)。原生程序必须带 `--require-aot`；普通 `dotnet run` 不构成原生证据。

```sh
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" -o .preview/native-preview
.preview/native-preview/PreviewHost --verify-buttons-messages
.preview/native-preview/PreviewHost --verify-controls
```

Windows / Linux 改用宿主 RID，省略 macOS 的 SDK 与 clang 参数，并在对应机器实跑。跨 RID 编译成功不表示该平台行为通过；边界见 [NativeAOT](../native-aot.md)。禁止 NoWarn、裁剪警告抑制、整程序集保留和 ReflectionBinding 绕过。

仪表专项可以保存八种布局的明暗三档尺寸合成图：

```sh
dotnet run --no-build --project design/PreviewHost -- --verify-gauge-styles .preview/gauge/images
```

`NativeAotSmoke` 支持 `--require-aot --verify-gauge-styles`，在实际原生程序中执行相同绘制检查，并验证消费端 `GaugeStyle` 的编译绑定。使用前按上面的原生发布命令生成当前源码的产物。

## NuGet 双包专项

消费项目只引用包，不依赖源码项目；使用新 feed 和隔离缓存避免恢复旧同版本包。

```sh
dotnet pack vendor/Avalonia.Controls.DataGrid -c Release -p:TheOnePackageVersion=1.0.0-preview.local -o .preview/feed
dotnet pack TheOne.Avalonia -c Release -p:TheOnePackageVersion=1.0.0-preview.local -o .preview/feed
dotnet restore design/PreviewHost/Consumers/DataGridPackageConsumer -p:TheOnePackageVersion=1.0.0-preview.local --source .preview/feed --source https://api.nuget.org/v3/index.json --packages .preview/package-cache
dotnet run --project design/PreviewHost/Consumers/DataGridPackageConsumer -c Release -p:TheOnePackageVersion=1.0.0-preview.local --no-restore
dotnet run --project design/PreviewHost/Consumers/DataGridPackageConsumer -c Release -p:TheOnePackageVersion=1.0.0-preview.local --no-restore -- --desktop
```

本地验证使用未发布的共同包号，避免公共源中的同版本旧包抢先恢复；重复使用前清理该专用 feed 和缓存。原生发布消费项目时增加相同的 `-p:TheOnePackageVersion=1.0.0-preview.local` 以及 `-p:RestorePackagesPath=.preview/package-cache -p:RestoreAdditionalProjectSources=.preview/feed`，沿用本机 RID 和工具链参数，分别执行 `--require-aot`、`--require-aot --desktop`。打包核对版本、依赖、README、许可和程序集身份；不上传验证产物，发布步骤见 [发布策略](releasing.md)。

## 定向渲染

```sh
dotnet run --project design/PreviewHost -- --render-controls .preview/images DataGridBinding Tag IconButton ReferenceChrome
```

名称以渲染目录为准；只导出受影响控件。截图不能替代实际输入、绑定回写或容器回收检查。Nova 原始配色与明确保留的对比度差异见 [主题说明](../theme/theme-colors.md)。
