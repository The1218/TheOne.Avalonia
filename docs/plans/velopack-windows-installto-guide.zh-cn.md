# Velopack Windows 自定义安装目录：`Setup.exe --installto` 详细说明

核查日期：2026-10-20。核查范围：官方在线文档、GitHub Latest Release、`1.2.161` 标签下的 Windows 安装器源码。

**结论：Velopack 当前支持 `Setup.exe --installto "D:\Apps\MyApp"`，可以在安装时指定目录。默认 EXE 没有目录选择向导；这个参数也不等于安装范围、已有安装迁移或多实例安装管理。**

本文是资料核查与接入说明。命令和代码示例尚未在 Windows 上执行，不代表已通过安装、升级或卸载实测；没有修改现有控件库代码。

## 1. 官方入口与版本依据

| 资料 | 用途 |
| --- | --- |
| [Setup.exe CLI](https://docs.velopack.io/reference/cli/content/setup-windows) | 参数的官方说明 |
| [Installers](https://docs.velopack.io/packaging/installer) | EXE、MSI、自定义安装目录和安装范围 |
| [Windows Overview](https://docs.velopack.io/packaging/operating-systems/windows) | 安装布局、启动和更新行为 |
| [GitHub Release 1.2.161](https://github.com/velopack/velopack/releases/tag/1.2.161) | 本次核查时 Latest 指向的版本 |
| [1.2.161 Setup 源码](https://github.com/velopack/velopack/blob/1.2.161/src/bins/src/setup.rs#L97-L104) | 确认此发布版本实际解析 `--installto` |

官方 CLI 页面同样标示 `1.2.161`。本次确认的是该版本支持，**没有确认此参数最早在哪个历史版本加入**，因此不建议据此推断任意旧版安装包都支持。

参数由生成出来的 Setup EXE 解析。只升级应用引用的 Velopack NuGet，并不会改变已经分发的旧 Setup；应固定 SDK 与打包工具版本，重新生成安装包。官方也建议两者使用相同版本。[来源：.NET 接入指南](https://docs.velopack.io/getting-started/csharp)

## 2. 参数用法

基本形式：

```text
Setup.exe --installto <DIR>
Setup.exe -t <DIR>
```

`Setup.exe` 是泛指 Velopack 生成的安装 EXE，请替换为实际文件名。

| 参数 | 作用 |
| --- | --- |
| `--installto <DIR>` / `-t <DIR>` | 指定应用安装根目录 |
| `--silent` / `-s` | 静默安装，隐藏安装器对话框；涉及覆盖确认时自动接受 |
| `--log <FILE>` / `-l <FILE>` | 指定安装日志文件 |
| `--verbose` / `-v` | 增加诊断输出 |
| `--help` / `-h` | 查看当前安装器的帮助 |
| `-- <EXE_ARGS>...` | 把后面的参数传给安装后启动的应用 |

表中是安装器参数，不能直接放到 `vpk pack` 后面使用。打包命令中的 `-t` 等短参数也不能按 Setup 的含义理解。[来源：Setup CLI](https://docs.velopack.io/reference/cli/content/setup-windows)

未指定目录时，EXE 的默认位置为 `%LocalAppData%\{packId}`。指定目录后，使用给定目录作为根目录，不需要再手动追加 `current`。[来源：Windows 安装布局](https://docs.velopack.io/packaging/operating-systems/windows)

### 2.1 CMD 示例

指定独立应用目录：

```bat
MyApp-win-Setup.exe --installto "D:\Apps\MyApp"
```

目录有空格时：

```bat
MyApp-win-Setup.exe --installto "D:\My Applications\MyApp"
```

静默安装并记录日志：

```bat
MyApp-win-Setup.exe --silent --installto "D:\Apps\MyApp" --log "%TEMP%\MyApp-setup.log"
```

自动化脚本需要等待 GUI 安装进程退出时，可以使用：

```bat
start "" /wait ".\MyApp-win-Setup.exe" --silent --installto "D:\Apps\MyApp" --log "%TEMP%\MyApp-setup.log"
set "SETUP_EXIT=%ERRORLEVEL%"
echo Setup exit code: %SETUP_EXIT%
```

这里的空字符串是 `start` 的窗口标题，不能省略成把带引号的 EXE 路径当成标题。

### 2.2 PowerShell 示例

直接安装：

```powershell
& '.\MyApp-win-Setup.exe' --installto 'D:\Apps\MyApp'
```

等待完成并读取退出码：

```powershell
$setupPath = (Resolve-Path '.\MyApp-win-Setup.exe').Path
$installDir = 'D:\Apps\MyApp'
$logPath = Join-Path $env:TEMP 'MyApp-setup.log'
$setupArguments = '--silent --installto "{0}" --log "{1}"' -f $installDir, $logPath

$setupProcess = Start-Process -FilePath $setupPath `
    -ArgumentList $setupArguments -Wait -PassThru

if ($setupProcess.ExitCode -ne 0) {
    throw "安装失败，退出码 $($setupProcess.ExitCode)，日志：$logPath"
}
```

示例使用不以反斜杠结尾的完整应用路径。用于接收用户输入时，应先检查目录，再构造参数，不能直接接受任意路径并静默覆盖。

### 2.3 安装完成后向主程序传参

```bat
MyApp-win-Setup.exe --installto "D:\Apps\MyApp" -- --welcome
```

`--welcome` 是示例应用自行实现的参数，不是 Velopack 参数。分隔符 `--` 后的内容由应用解释。

**不要依赖 `--silent -- --welcome` 来自动打开主程序**：此版本安装代码在静默模式下跳过最终的应用启动。[来源：安装实现末尾](https://github.com/velopack/velopack/blob/1.2.161/src/bins/src/commands/install.rs#L249-L255)

## 3. 安装目录实际是什么

例如传入 `D:\Apps\MyApp`，核心布局为：

```text
D:\Apps\MyApp\
├── MyApp.exe               稳定的启动入口 / stub
├── Update.exe              更新器
└── current\
    ├── MyApp.exe           真正的主程序
    ├── MyApp.dll
    ├── 其他发布文件
    └── sq.version          安装版本元数据
```

图中省略了缓存等辅助文件。根目录入口和 `current` 中的程序作用不同；自定义启动器优先使用稳定入口，不应自行维护版本子目录。[来源：Windows Overview](https://docs.velopack.io/packaging/operating-systems/windows)

应用中需要获取位置时，可以在 Velopack 初始化后使用：

```csharp
var locator = Velopack.Locators.VelopackLocator.Current;
var installRoot = locator.RootAppDir;
var applicationFiles = locator.AppContentDir;
```

`RootAppDir` 与 `AppContentDir` 的含义由官方定位器定义；上述命名空间已核对 `1.2.161` 源码。[来源：运行时路径](https://docs.velopack.io/integrating/preserved-files#finding-app-paths-at-runtime)、[定位器源码](https://github.com/velopack/velopack/blob/1.2.161/src/lib-csharp/Locators/VelopackLocator.cs)

## 4. 路径限制、权限与已有内容

以下区分已确认实现和建议策略，避免把建议误认为 Velopack 的强制限制。

### 4.1 已确认的目录处理

`1.2.161` 安装代码会采用传入路径；目录不存在时尝试创建，然后检查磁盘空间。如果目录非空，会进入覆盖／修复确认流程。用户拒绝后会结束安装。[来源：install.rs](https://github.com/velopack/velopack/blob/1.2.161/src/bins/src/commands/install.rs#L37-L118)

这个流程不是“只向目录中追加几个文件”：后续会准备干净的安装目录。因此，**目标应是专属于该应用的目录**。不要传入 `D:\`、`D:\Apps`、桌面、文档目录或包含其他应用的公共目录。

静默模式下，覆盖／修复确认函数直接返回接受；它不会因为无法识别原有版本就一律拒绝。[来源：覆盖确认实现](https://github.com/velopack/velopack/blob/1.2.161/src/l18n/src/dialogs.rs#L188-L225)

### 4.2 推荐由自定义安装向导执行的检查

这些是接入层应增加的保护，不是宣称官方已全部实现：

1. 使用规范化的本地绝对路径，避免依赖当前工作目录。
2. 拒绝盘符根目录、系统目录以及其他公共数据目录。
3. 验证当前用户对目标和父目录的创建、写入及重命名权限。
4. 非空目录只在识别到同一产品时提供“修复／覆盖”，其他情况要求换目录。
5. 处理符号链接／目录联接，避免检查的路径和实际操作位置不一致。
6. 明确显示最终目录，之后才允许静默调用底层 Setup。

对于 UNC、网络盘、可移动盘、超长路径，本次没有取得足以证明完整安装、更新和卸载都受支持的证据；首版不建议默认承诺这些路径。

### 4.3 指定路径不改变安装范围

`--installto` 不会把 per-user EXE 转换为规范的 per-machine 安装。比如指定 `C:\Program Files\MyApp`，也不能据此认为它会自动处理管理员权限、所有用户快捷方式及机器级注册。

官方对面向所有用户、受保护目录的安装推荐 MSI `PerMachine`。即使第一次由管理员完成写入，也要单独验证普通用户后续的更新权限。[来源：Windows 权限说明](https://docs.velopack.io/packaging/operating-systems/windows#updating)

## 5. 重复安装、迁移与退出码

### 5.1 同一目录重复安装

安装器可以依据已安装版本，提供升级、降级或修复提示；同版本对应修复。静默模式会接受这一覆盖决策，因此发布系统不能只凭“有安装器”就无条件静默运行旧版本。[来源：版本比较逻辑](https://github.com/velopack/velopack/blob/1.2.161/src/l18n/src/dialogs.rs#L205-L225)

### 5.2 换一个目录再次安装

**这不是安装迁移命令，也不能作为同产品并行安装方案。** 维护者在官方仓库讨论中明确说明，同产品不同目录可能产生多个文件副本，但快捷方式及注册信息会被后一次安装覆盖；该回复发表于 2025-11-27，属于历史行为说明。[来源：官方讨论 #50](https://github.com/velopack/velopack.docs/discussions/50)

本次版本源码同样直接根据参数选择根目录，没有在该流程中自动发现旧根目录并完成迁移。因此，自定义向导应检测已有安装，并默认保留原目录；迁移需要另行设计旧安装清理、用户数据保留和失败恢复。

### 5.3 不能只用退出码判断“目标版本安装成功”

此版本 Setup 入口按结果返回 `0` 或 `1`。但是用户取消覆盖时，安装函数返回的是成功结束，可能最终同样得到 `0`。[来源：Setup 退出逻辑](https://github.com/velopack/velopack/blob/1.2.161/src/bins/src/setup.rs#L56-L60)、[取消覆盖分支](https://github.com/velopack/velopack/blob/1.2.161/src/bins/src/commands/install.rs#L94-L103)

所以外层向导至少应结合退出码、日志、目标安装元数据中的产品身份与版本，确认是否真正达到预期版本。只检查 EXE 存在也不充分，因为它可能是旧安装留下的。

覆盖安装代码包含重命名旧目录、失败时尝试恢复的流程；恢复和清理中部分错误会被忽略，不能写成“任何失败都必然无损回滚”。它也不意味着新版本启动后发生业务异常能自动回滚。[来源：安装恢复流程](https://github.com/velopack/velopack/blob/1.2.161/src/bins/src/commands/install.rs#L147-L164)

## 6. 与 MSI 自定义目录的区别

| 需求 | EXE | MSI |
| --- | --- | --- |
| 安装时指定目录 | `--installto` | `VELOPACK_INSTALLDIR` |
| 默认一键安装 | 是 | Windows Installer 流程 |
| 自定义目录选择页面 | 默认 EXE 没有，需外层向导 | 可使用 MSI 方案；具体目录页面需核对产物 |
| 每用户安装 | 适用 | `PerUser` |
| 所有用户、Program Files | 推荐改用 MSI | `PerMachine`，需要提升权限 |
| 自动升级 | 配合应用内 SDK | 配合应用内 SDK |

MSI 的安装范围通过打包参数确定，目录可以在安装时覆盖。`Either` 表示安装范围可选，不能仅凭该值推断安装器已有任意目录浏览页面。[来源：Installers](https://docs.velopack.io/packaging/installer#install-location)

生成 per-machine MSI，PowerShell 示例：

```powershell
vpk pack --packId Example.MyApp --packVersion 1.0.0 `
    --packDir '.\publish' --mainExe MyApp.exe `
    --msi --instLocation PerMachine
```

安装时指定路径：

```bat
msiexec /i MyApp.msi /qn VELOPACK_INSTALLDIR="D:\Apps\MyApp"
```

请替换实际生成的 MSI 文件名。`--instLocation` 控制默认范围，`VELOPACK_INSTALLDIR` 覆盖目录；不能把 EXE 的 `--installto` 原样传给 `msiexec`。[来源：MSI 目录覆盖](https://docs.velopack.io/packaging/installer#overriding-the-install-location-at-install-time)

## 7. 从打包到安装的最小流程

以下是示意工程 `MyApp` 的命令。假设已经完成 Velopack 启动接入，并在 Windows 打包机执行。

```powershell
# 打包机：固定工具和应用依赖版本。
dotnet tool install -g vpk --version 1.2.161
dotnet add '.\MyApp.csproj' package Velopack --version 1.2.161

# 先发布 Windows 应用文件。
dotnet publish '.\MyApp.csproj' -c Release -r win-x64 `
    --self-contained true -o '.\publish'

# 再生成安装器和更新包。
vpk pack --packId Example.MyApp --packVersion 1.0.0 `
    --packDir '.\publish' --mainExe MyApp.exe `
    --packTitle 'MyApp' --releaseNotes '.\release-notes.md' `
    --outputDir '.\Releases'
```

`release-notes.md` 需预先创建，实际生成文件名以 `Releases` 输出为准。更新到后续版本时，保留产品标识和发布配置，仅递增相应版本并提供新说明。[来源：.NET 入门](https://docs.velopack.io/getting-started/csharp)、[vpk 参数](https://docs.velopack.io/reference/cli/content/vpk-windows)

将生成的安装 EXE 交给用户后，在用户机器执行：

```bat
MyApp-win-Setup.exe --installto "D:\Apps\MyApp"
```

应用采用 self-contained 发布时，用户不需要为打包而安装 .NET SDK 或 `vpk`；它们属于开发／构建机器。首次安装目录是安装阶段的输入，不需要为了不同用户目录分别重新打包。

## 8. 如何加一个可选目录的安装向导

若需求是“用户双击安装器，选择 D 盘，再点击安装”，可以在官方 Setup 外加一个引导程序：

```text
自定义安装向导
  → 输入／浏览应用目录
  → 检测已有安装、权限和目标内容
  → 用户确认
  → 调用官方 Setup --silent --installto <目录> --log <日志>
  → 等待结束，验证产品和目标版本
  → 按用户选择启动根目录中的稳定入口
```

这部分是建议的产品设计。官方已有参数只解决把目录传入安装器，不包含这套自定义页面。

C# 调用片段，展示正确传参和等待方式；目录已经由向导校验：

```csharp
using System.Diagnostics;

string setupPath = Path.GetFullPath(@"payload\MyApp-win-Setup.exe");
string installDir = @"D:\Apps\MyApp";
string logDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "Example.MyApp.Installer", "Logs");
Directory.CreateDirectory(logDir);
string logPath = Path.Combine(logDir, $"setup-{Guid.NewGuid():N}.log");

var startInfo = new ProcessStartInfo(setupPath)
{
    UseShellExecute = false,
    CreateNoWindow = true
};
startInfo.ArgumentList.Add("--silent");
startInfo.ArgumentList.Add("--installto");
startInfo.ArgumentList.Add(installDir);
startInfo.ArgumentList.Add("--log");
startInfo.ArgumentList.Add(logPath);

using var process = Process.Start(startInfo)
    ?? throw new InvalidOperationException("无法启动安装器。");
await process.WaitForExitAsync();

if (process.ExitCode != 0)
    throw new InvalidOperationException($"安装失败，请查看：{logPath}");

// 接下来验证目标安装的产品身份与版本。
// 验证通过后，才允许按用户选择启动 installDir 下的稳定入口。
```

集成时还需要：

- 将外层引导程序和嵌入的安装器一并签名，校验解包的 payload。
- 使用进程参数列表，避免经 `cmd /c` 拼接用户目录。
- 安装 payload、工作目录与日志放在目标目录之外，避免覆盖／重命名时互相占用。
- 不用新的 NSIS／Inno Setup 脚本直接接管 `current` 文件替换、注册信息和卸载，保留 Velopack 的安装布局所有权。
- `WaitForExitAsync` 取消等待不等于安全取消安装；不可把强杀安装进程当作通用取消方案。
- `--silent` 不提供外层向导可直接绑定的实时百分比协议。首版可展示忙碌状态，日志只作为诊断依据。
- 普通用户可写路径适合这一 EXE 方案；需要 per-machine 的安装单独走 MSI 和权限验证。

## 9. 对自动更新和数据保存的影响

自定义首次安装位置后，更新应使用安装实例的定位信息，不能硬编码 `%LocalAppData%\{packId}`。官方安装代码根据实际根目录建立定位配置；应用 SDK 再从真实安装中检测、下载和应用更新。[来源：安装定位配置](https://github.com/velopack/velopack/blob/1.2.161/src/bins/src/commands/install.rs#L121-L127)

应用入口在其他启动代码前调用一次：

```csharp
Velopack.VelopackApp.Build().Run();
```

应用内更新主流程为 `CheckForUpdatesAsync()` → 自有对话框征得同意 → `DownloadUpdatesAsync(update)` → `ApplyUpdatesAndRestart(update)`。更新服务需要真实的 Velopack 安装，直接运行开发输出目录不等于安装验证。应在退出前保存业务数据；若要尊重“稍后更新”，也要明确配置默认的启动自动应用策略。[来源：应用接入与更新](https://docs.velopack.io/integrating/overview)

更新说明可通过打包参数 `--releaseNotes` 写入，检查更新时读取 `TargetFullRelease.ReleaseNotesHtml`，在下载前展示。Avalonia 中的说明对话框及其 Markdown／HTML 展示仍由接入包或应用实现。[来源：Release Notes](https://docs.velopack.io/integrating/release-notes)

更新会替换程序文件，配置、数据库和重要用户文件应存放在安装目录之外，例如按产品组织的用户数据目录。这样也避免覆盖安装或卸载误删业务数据；不要因为安装在 D 盘就把数据放入 `current`。[来源：Preserving Files & Settings](https://docs.velopack.io/integrating/preserved-files)

## 10. 常见问题排查

| 现象 | 优先检查 |
| --- | --- |
| 仍装到默认目录 | 实际执行的是否为新安装器；参数是否写在 `--` 前；用 `--log` 检查 Install To |
| 路径含空格时异常 | CMD／PowerShell 是否正确引用；C# 是否使用 `ArgumentList` |
| 指定目录后访问被拒绝 | 父目录和目标权限；是否误选 Program Files；不要假定 `--installto` 自动提升权限 |
| 再装一次出现覆盖／修复 | 目录是否非空、是否已有同产品及版本 |
| 返回 0，但版本没有变 | 是否取消了非静默覆盖；核查日志和安装元数据 |
| 静默安装后没有打开应用 | 此版本的正常行为；由外层向导验证完成后决定启动 |
| 更新后配置丢失 | 是否写在 `current`；是否遭到覆盖安装或卸载 |
| 换目录后旧文件还在 | 把重新安装误当成自动迁移；检查旧目录、快捷方式和注册信息 |
| 开发环境检测更新异常 | 是否从真正安装的应用启动，而非 `bin/Debug` |

官方没有在该 CLI 页面承诺完整的安装结果分类或机器可读进度接口；封装层应自定义结果模型，保留原退出码和日志路径。

## 11. 落地前的 Windows 专项验证

建议固定 `1.2.161`，用同一示例产品完成以下测试后再决定发布安装向导。这里只是验收清单，尚未执行：

| 场景 | 需要观察的结果 |
| --- | --- |
| 普通用户，新建 D 盘目录 | 安装、启动、快捷方式与卸载位置一致 |
| 中文与空格目录 | 安装、升级、重启、卸载都能定位同一根目录 |
| 同目录升级／修复／降级 | 提示和版本符合预期；静默覆盖策略明确 |
| 非空但非本产品目录 | 外层向导拒绝，不交给静默 Setup |
| 同产品已有安装后改目录 | 外层向导保留原位置，或进入明确的迁移流程 |
| 目标不可写／被占用 | 错误可理解，旧版本能否恢复需实测，重试可进行 |
| 用户取消覆盖 | 返回码、日志与外层结果分类一致 |
| 自定义目录内 1.0.0 升 1.0.1 | 更新沿用目录、说明正确、下载后自动重启到新版本 |
| 卸载与数据保留 | 程序资源清理正确，安装目录之外的用户数据保留 |
| MSI PerMachine 与普通用户升级 | 管理员安装、用户启动及后续更新权限分别验证 |

## 12. 对当前需求的判断

如果只是要求“Windows 能指定安装目录”，现有 Velopack 已有基础能力，无需重新实现更新引擎。

如果要求“用户有目录选择页面，应用内有主题一致的更新说明对话框，确认后下载并自动重启”，建议分别增加：

1. 应用端 Avalonia NuGet：包装检查更新、说明对话框、下载进度、退出前保存和重启。
2. 打包端 dotnet tool：固定并调用兼容版本的 Velopack 打包流程。
3. Windows 安装向导：目录选择与校验后，通过 `--installto` 调用官方 Setup；所有用户安装另走 MSI。

其中自定义目录、静默启动和覆盖行为已完成文档及发布版本源码核查；真实 Windows 安装、升级、卸载与权限行为仍需专项验证。
