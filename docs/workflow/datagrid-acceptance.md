# DataGrid 集成与打包验收

范围：筛选、数值统计、左右冻结、原生列拖动及其消费契约。沿用[测试分级](testing.md)，不默认运行其他控件回归或全量截图。

## 两层消费验证

- PreviewHost `--verify-datagrid-summary`：覆盖 Light / Dark × 三尺寸、自动编译字段、全部数值类型、五种聚合、多列与业务 Filter、跨分页统计、数据更新、原生编辑／排序／分组、列布局和千行实际回收；同时验证拖影、抓取位置、冻结区内落点、Esc／越界释放／卸载清理和动效开关。
- [DataGridPackageConsumer](../../tests/DataGridPackageConsumer/DataGridPackageConsumer.csproj)：独立实体与编译 XAML，只引用打包的 TheOne.Avalonia，无 ProjectReference、PreviewHost 链接或额外 DataGrid 样式。使用 1,000 行复验上述功能共存；容器数与远处跳转实际新增容器单独记录，不把 LoadingRow 事件次数当作创建数。

```sh
dotnet build TheOne.Avalonia.slnx
dotnet run --project design/PreviewHost --no-build -- --verify-datagrid-summary
bash scripts/verify-datagrid-package.sh --native --cross --desktop
dotnet run --project design/PreviewHost --no-build -- --render-controls .preview/datagrid-integration/images DataGridSummary
```

打包脚本先产生两个本地 nupkg，核对传递依赖及许可，再用专用缓存恢复独立消费端；用户 NuGet 缓存不修改。`--native` 编译并执行本机 NativeAOT；`--desktop` 使用真实桌面后端；`--cross` 仅生成六种桌面 RID 的框架依赖发布目录。未加这些选项时只执行本机 Skia Headless 消费专项。全部日志、包、缓存与发布目录位于 `.preview/datagrid-integration/`，可用 `THEONE_DATAGRID_OUTPUT` 改为绝对路径。

必须同时交付 `TheOne.Avalonia` 与其依赖的 `TheOne.Avalonia.DataGrid`，不能额外混入官方 `Avalonia.Controls.DataGrid` 同名程序集。消费测试从包还原，项目引用成功不能替代这一层证据。

## 平台证据的边界

| 范围 | 可证明内容 | 仍需对应宿主验证 |
| --- | --- | --- |
| 本机 Skia Headless | 编译 XAML、交互、虚拟化和几何布局 | 操作系统输入、窗口与 GPU |
| 本机桌面后端 | 原生窗口创建、布局、实体编辑及筛选／统计／冻结共存 | 人工拖动手感、系统缩放、多屏、GPU 差异 |
| 本机 NativeAOT | 真正原生程序、裁剪后字段／编辑及包资源 | 其他 RID 的本地编译器与运行行为 |
| 其他 RID 框架依赖发布 | NuGet 恢复、编译和目标平台应用及原生资源打包 | 目标运行时、桌面交互、GPU、字体与 NativeAOT |

当前宿主为 macOS ARM64。Windows、Linux、macOS Intel 不能由跨 RID 发布声明为实跑通过；本机 Docker 服务不可用，也没有 Windows / Linux 运行环境。移动端、浏览器、安装器、代码签名和公证不属于此次桌面验收。

阶段 5 基线完成 osx-arm64 的托管／NativeAOT Headless／桌面检查；其他五种 RID 仅检查发布及目标架构／资源／依赖，记录在 `.preview/datagrid-integration/verification.json`。后续仅改双包版本时按[包专项](testing.md#nuget-双包专项)重验，不能把旧发布目录当作新版本的跨 RID 证据。

在其他宿主复制同一份源码和 feed，以隔离缓存恢复该消费项目，然后运行托管版本及 `dotnet publish tests/DataGridPackageConsumer -c Release -r <本机RID> -p:PublishAot=true` 产生的程序，加 `--require-aot`。Windows 本机原生文件带 `.exe`；实际运行后才可更新平台状态。完整步骤可沿用脚本的恢复参数，避免旧同版本包污染结果。

本地统计只读取已加载数据，默认聚合完整筛选结果（分页前）；服务端未加载数据需要业务端聚合。官方 DataGrid 源码已有的裁剪／动态转换警告需逐项审查，不能使用 NoWarn 隐藏。

主库与 vendor 共享同一包版本和渠道；不再使用“主库默认稳定版＋vendor 预发布版”的组合。版本号、程序集身份、发布说明和正式渠道模拟见[发布策略](releasing.md)；NU5104 作为打包错误处理。
