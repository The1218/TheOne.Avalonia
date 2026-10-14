# 按改动范围验证

先判断影响范围，再选择检查；通过后只有新改动、失败或未解疑点才重跑。纯文档不构建、不运行 UI 回归。以下命令在仓库根目录执行。

## 测试分级

| 改动 | 必要检查 | 升级条件 |
| --- | --- | --- |
| 文档、计划、项目 skill | `python3 scripts/check-docs.py`；skill 格式验证 | 文档附带可执行代码改动时按对应行处理 |
| 局部 C# / XAML / 控件行为 | `dotnet build TheOne.Avalonia.slnx` + 相关专项 | 公共组件被其他控件复用时增加其直接调用者检查 |
| 模板、布局、交互、密度 | 上述检查 + 受影响控件 Light / Dark × 三尺寸、真实输入与布局检查；定向截图 | 跨控件共享模板／密度规则改变时完整控件回归 |
| 集合、虚拟化、回收 | 相关专项 + 1,000 项实际容器数量、远处选择／导航、滚动回收及状态保留 | 没有专项入口时使用完整控件回归 |
| 语义色、状态画刷 | `python3 scripts/check-theme-contrast.py` + 受影响外观 | 全局语义色变化时完整控件回归 |
| 编译绑定、公共属性、裁剪、消费契约 | `python3 scripts/check-aot-contracts.py` + 实际 NativeAOT 相关专项 | 无原生专项时默认消费回归；共享绑定／主题注册改动时 `--full` |
| OneTheme、全局样式／尺寸／动效、升级依赖、交付全面审查 | 构建 + `--verify-controls`；涉及 AOT 时 `bash scripts/verify-native-aot.sh --full` | 全量截图仍需用户明确要求 |

只改共享文件中的单个控件选择器时，可依据调用范围选择专项；影响范围无法可靠界定才升级全量。全量回归是测试套件的扩大，不能代替平台验证，也不自动触发全量截图。

## NuGet 双包专项

仅改版本、包自述、许可或发布元数据时，构建并运行以下包消费检查，不重复全控件、截图或六 RID 发布。版本规则见[发布策略](releasing.md)。

```sh
dotnet build TheOne.Avalonia.slnx
THEONE_DATAGRID_OUTPUT="$PWD/.preview/package-versions/preview" bash scripts/verify-datagrid-package.sh --native --desktop
THEONE_PACKAGE_VERSION=1.0.0 THEONE_DATAGRID_OUTPUT="$PWD/.preview/package-versions/stable" bash scripts/verify-datagrid-package.sh --desktop
```

首个消费检查使用共享默认版本，执行本机托管／NativeAOT 的 Headless／桌面模式；第二个只模拟正式版本号，验证同版本双包的打包和托管消费，不表示发布正式版。脚本核对版本、依赖、包内 README／许可／发布说明、还原包哈希及实际程序集身份。

发布后运行 `bash scripts/verify-remote-nuget.sh --desktop`：仅从 NuGet.org 使用隔离缓存还原两包，核对实际版本、来源和已验收包内容，再执行独立消费端 Headless／本机桌面专项。脚本读取 `.preview/package-versions/verification.json` 中的验收哈希；NuGet.org 添加仓库签名后整个包哈希会变化，因此逐项比较除签名外的内容并分别记录下载哈希。日志默认保存在 `.preview/nuget-publish-<共同版本>/consumer/`；等待平台校验时不要重新打包或上传同一版本。

## 托管专项入口

统一调用：`dotnet run --project design/PreviewHost -- <入口>`。构建结果仍有效时可加 `--no-build`，源码变化后先重新构建。

| 范围 | 入口 |
| --- | --- |
| 按钮状态、IconButton、消息配置与目录调整 | `--verify-buttons-messages` |
| 图表与图表消费 | `--verify-charts` |
| DataGrid 强类型集合绑定、排序按钮、单元格密度及千条虚拟化 | `--verify-datagrid-binding` |
| DataGrid 统计、自动属性、筛选与左右冻结集成 | `--verify-datagrid-summary` |
| DataGrid 右侧冻结、筛选及左侧原生回归 | `--verify-datagrid-right` |
| DataGrid 列筛选及原生表格行为 | `--verify-datagrid-filters` |
| DataGrid 右冻结可行性实验 | `--verify-datagrid-stage1` |
| 文本输入／按钮对齐 | `--verify-text-alignment` |
| 日期时间弹层 | `--verify-pickers` |
| Tag、Step 与属性验证 | `--verify-tag-step` |
| InfoCard、窗口、登录 | `--verify-infocard`、`--verify-onewindow`、`--verify-loginwindow` |
| NavMenu 全部／悬停／边缘／搜索／动效／内置筛选 | `--verify-navmenu`、`--verify-navmenu-hover`、`--verify-navmenu-edge`、`--verify-navmenu-search`、`--verify-navmenu-motion`、`--verify-navmenu-filter` |
| 展开／轮播／下拉刷新 | `--verify-expander-motion`、`--verify-carousel`、`--verify-refresh` |
| 浮层／通知／内容切换 | `--verify-flyout`、`--verify-notifications`、`--verify-content-transition` |
| 预览页面组织与绑定 | `--verify-preview-pages` |
| 全控件 | `--verify-controls` |

没有匹配的现有专项时，选择能覆盖改动的现有套件；不要虚构 CLI 参数。新功能若反复需要独立检查，再增加可复用入口。

登录进程生命周期属于桌面检查：`--verify-login-lifetime <ShutdownMode>` 与 `--verify-login-lifetime-close <ShutdownMode>` 分别传入 OnMainWindowClose、OnLastWindowClose、OnExplicitShutdown，在独立进程执行。

## NativeAOT

`bash scripts/verify-native-aot.sh <专项>` 发布并执行真正的本机原生程序。脚本当前支持 `--verify-tag-step`、`--verify-charts`、`--verify-datagrid-binding`、`--verify-datagrid-summary`、`--verify-datagrid-right`、`--verify-datagrid-filters`、`--verify-datagrid-stage1`、`--verify-navmenu-filter`、`--verify-navmenu-motion`、`--verify-preview-pages`、`--verify-buttons-messages`。页面专项发布原生 PreviewHost，检查 VM 通知、派生属性、图表系列回写和控件／VM 双向同步。不传参数运行默认消费回归；`--full` 额外发布并执行 PreviewHost 全控件检查，不导出全量截图。

脚本不支持的专项可先发布，再对原生程序调用其已有入口；参数以 `tests/NativeAotSmoke/Program.cs` 为准，必须带 `--require-aot`。普通 `dotnet run` 不能作为 NativeAOT 证据。更多配置与 RID 边界见 [NativeAOT](../native-aot.md)。

## 定向截图与结果

DataGrid 阶段 5 的包消费、跨 RID 发布和平台证据分层见[集成验收](datagrid-acceptance.md)。

```sh
dotnet run --project design/PreviewHost -- --render-controls .preview/<任务>/images <控件名...>
```

只导出本次涉及控件及必要场景。缺少控件名或未知名字会报错；`--all` 仅用于用户明确要求的全量截图。渲染工厂见 [TargetedControlPreview](../../design/PreviewHost/Verification/Infrastructure/TargetedControlPreview.cs)。新增／重写控件补充独立渲染入口。

日志、图片及必要源码哈希保存 `.preview/<任务>/`。完成报告注明运行命令、覆盖范围、结果和未验证部分，不把旧轮次通过数量写成当前结果。不要为满足模板反复运行无关套件。
