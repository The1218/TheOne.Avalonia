# NuGet 版本与发布

## 单一发布序列

[PackageVersions.props](../../PackageVersions.props) 是 `TheOnePackageVersion` 的唯一默认来源，当前为 `1.0.0-preview.1`。主库、vendor 包和独立消费验证均导入它。两包同版本、同预发布渠道、同批交付；改变其中任一包都增加共同包号，不覆盖已经发布的版本。

| 标识 | 用途／当前值 |
| --- | --- |
| `TheOne.Avalonia`、`TheOne.Avalonia.DataGrid` 的 NuGet Version | 同一 TheOne 发布序列，与上游版本无关 |
| Avalonia 核心依赖 | 12.1.4 |
| DataGrid 源码基线 | 12.1.2；原始提交与哈希仍见 vendor 的 UPSTREAM.json |
| 主库 AssemblyVersion / FileVersion | 1.0.0.0 / 1.0.0.0 |
| vendor 程序集 | `Avalonia.Controls.DataGrid`；AssemblyVersion / FileVersion 为 12.1.2.0，保留强名称 |

包版本不能用来判断上游 DataGrid 版本；上游升级需要单独审查源码、friend API 和原生验证，不能只改包号。程序集／文件版本显式维护，不能因预发布后缀或包修订自动改变运行时身份；包发布版本可从 InformationalVersion 和 nuspec 追溯。

首个远程版本使用统一预发布序列 `1.0.0-preview.1`。此前本地生成的 vendor `12.1.2-one.1` 属于旧测试包，不作为新序列的版本前缀；不要混入新发布源。后续发布前应核对目标源的版本历史，递增共同版本。

主库对 vendor 使用 NuGet 标准的包含下限依赖，即 `>=` 当前共同包号。消费项目通常只引用主库；升级时确保源中存在配套 vendor，避免同时引用官方 `Avalonia.Controls.DataGrid`。独立验证端精确指定主库版本，并核对还原图中两个包的实际版本和包内容哈希。

## 预发布与正式版

- 开发验证使用 `X.Y.Z-preview.N`，候选版本可使用 `X.Y.Z-rc.N`；两包同步增加编号。
- 完成目标平台与发布检查后，才将同一发布序列提升为 `X.Y.Z`。后续兼容修复升 patch，兼容功能升 minor，破坏性 API 变更升 major；程序集身份变更需另外评估。
- 本地试验通过 `-p:TheOnePackageVersion=...` 覆盖共同版本；不直接覆盖 `Version`、`VersionSuffix` 或 `PackageVersion`。打包目标会拒绝独立版本覆盖，NU5104 作为错误处理，不使用 NoWarn。

NU5104 表示稳定包依赖了预发布包；同版本预发布或同版本正式双包均可避免该不一致。依据：[NuGet NU5104](https://learn.microsoft.com/en-us/nuget/reference/errors-and-warnings/nu5104)、[NuGet 版本与程序集版本](https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/versioning)。

## 本地验证与产物

默认预发布与正式渠道模拟的命令统一见[测试策略](testing.md#nuget-双包专项)。脚本默认读取共享版本；设置 `THEONE_PACKAGE_VERSION` 会统一覆盖两包及消费端。正式渠道模拟只生成本地包，不代表发布正式版。

脚本对打包、恢复、消费构建和原生发布统一传递版本。它检查两包号、vendor 依赖、README、许可、releaseNotes、上游来源，以及隔离缓存实际恢复的版本和字节哈希；消费端运行时检查程序集版本与 vendor 强名称。UI 行为没有改动时不重复全控件、截图或六 RID 发布；目标平台边界见[集成验收](datagrid-acceptance.md)。

每个 nupkg 都包含自述与许可；主库另含 THIRD-PARTY-NOTICES.md，vendor 另含 UPSTREAM.json。`PackageReleaseNotes` 位于各自项目文件，描述本次内容和兼容条件；发布前按实际变更替换，不能长期保留旧版本说明。

## 远程推送顺序

正式发布从可追溯提交构建，记录源码和两包哈希。确认共同版本、变更说明、目标源和消费验证后，先推送 vendor，再推送主库，避免用户暂时还原不到依赖。使用已经验收的同一份 feed 产物，不能重新构建后直接推送：

```sh
dotnet nuget push <feed>/TheOne.Avalonia.DataGrid.<版本>.nupkg --source <目标源> --api-key <密钥>
dotnet nuget push <feed>/TheOne.Avalonia.<版本>.nupkg --source <目标源> --api-key <密钥>
```

密钥由发布环境提供，不保存到仓库。也可使用 NuGet.org 已登录的上传页面提交同一产物。第二个包失败时保留已验证产物并重试主库；发现包内容错误则递增共同版本并重新验证，不覆盖相同版本。

上传成功不代表可以还原：NuGet.org 还需要异步校验和索引。两包可从公共源获取后，按[测试策略](testing.md#nuget-双包专项)执行远程专项；核对来源、版本及签名前后的包内容，再运行独立消费端。

`1.0.0-preview.1` 已于 2026-10-14 按 [vendor](https://www.nuget.org/packages/TheOne.Avalonia.DataGrid/1.0.0-preview.1)、[主库](https://www.nuget.org/packages/TheOne.Avalonia/1.0.0-preview.1) 顺序发布至 NuGet.org，并通过远程专项：只启用公共源、禁用 SDK 隐式离线源、使用隔离缓存，实际还原两包；除 NuGet.org 新增仓库签名外，所有包内容与已验收产物相同。独立消费端构建零警告／零错误，osx-arm64 托管 Headless 152 项、真实桌面 140 项通过。

发布与下载哈希、来源、日志和页面截图保存在 `.preview/nuget-publish-1.0.0-preview.1/`。本次未重复 NativeAOT 或其他 RID 验证；先前本机原生基线与其他平台的未验证边界仍见[集成验收](datagrid-acceptance.md)，不能将远程还原成功视为其他平台实跑通过。

对外发布内容维护在 [1.0.0-preview.1 发布说明](../releases/1.0.0-preview.1.md)，GitHub Release 使用同一份正文；精简验收 JSON 随版本提交，原始日志与包缓存保留在忽略目录。
