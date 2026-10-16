# 项目接入、改造与迁移

**简体中文** · [English](../en/README.md) · [文档总入口](../../README.md)

本文供其他设备上的开发者或 Codex 接入现有项目时使用。版本基线为 TheOne.Avalonia / TheOne.Avalonia.DataGrid **1.0.0-preview.4**、**.NET 10**、**Avalonia 12.1.4**。先确认目标项目的技术栈，再按功能迁移；运行 PreviewHost 不是消费本库的前提。

## 1. 先审查目标项目

记录以下信息，作为迁移范围与验收基线：

| 信息 | 为什么需要 |
| --- | --- |
| SDK、TargetFramework、Avalonia 包版本、目标 OS / RID | 本库当前使用 net10.0 / Avalonia 12.1.4，不能直接假设 Avalonia 11 或 net8.0 项目兼容 |
| UI 框架与应用生命周期 | 现有 Avalonia 项目可替换主题；WPF / WinForms 项目需先完成框架迁移，不能只替换包名或 XML 命名空间 |
| App 启动、主窗口、登录交接和退出策略 | 保留 DI、认证和业务导航；不要照搬预览程序的启动流程 |
| 已安装主题、DataGrid 来源及自定义模板 | 查找 Fluent / Semi / Ursa 等主题和旧模板覆盖，避免主题叠加和同名程序集冲突 |
| MVVM、模型通知、命令及校验方案 | 主库不要求 ReactiveUI，保留既有框架；示例的业务 VM、命令和属性需由应用提供 |
| 待迁移页面、数据规模与已有检查 | 选一个代表性编辑页和列表页，先建立最小可运行切片 |

先读取目标项目的约定并执行 `git status --short`。涉及 SDK / 框架升级、应用生命周期或第三方控件替换时，列明影响后再实施，不把这些工作藏在换主题里。保留用户已有改动。

## 2. 安装与主题启动

已有兼容的 Avalonia 项目安装主包即可：

```sh
dotnet add path/to/YourApp.csproj package TheOne.Avalonia --version 1.0.0-preview.4
```

这是现有应用的配置片段，不是完整新项目：

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
</PropertyGroup>
<ItemGroup>
  <PackageReference Include="TheOne.Avalonia" Version="1.0.0-preview.4" />
</ItemGroup>
```

保留应用自己的桌面／单视图启动方式及必要的 Avalonia 平台包，将其 Avalonia 依赖统一到兼容的 12.1.4 基线。主包自动依赖 `TheOne.Avalonia.DataGrid >= 1.0.0-preview.4`；若项目显式引用 DataGrid 配套包，也按同一版本升级。移除官方 `Avalonia.Controls.DataGrid` 的重复引用：本库 fork 保留程序集名 `Avalonia.Controls.DataGrid`、版本 `12.1.2.0` 和公钥标识，不能并存两个实现。主库程序集版本保持 `1.0.0.0`，与 NuGet 版本不是同一概念。

在 `App.axaml` 用一个 `OneTheme` 替换原来的基础主题注册，保留现有 `x:Class`、DataTemplates 和业务资源：

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:themes="using:TheOne.Avalonia.Themes"
             x:Class="YourApp.App"
             RequestedThemeVariant="Default">
  <Application.Styles>
    <themes:OneTheme ColorTheme="Nova" />
    <!-- 经审查仍需要的应用样式放在后面。 -->
  </Application.Styles>
</Application>
```

保留原 App 的 XAML 加载和 ViewModel 初始化。`OneTheme` 已包括 Fluent 支持层和 DataGrid 主题，不再额外注册 FluentTheme、SimpleTheme 或官方 DataGrid Fluent 样式。业务自定义样式需逐项检查，尤其是写死颜色、Height 或完整 ControlTemplate 的覆盖。

源码引用是另一种选择：[仓库快速开始](../../../README.zh-cn.md#快速开始)。同一应用不要同时引用本库源码项目和同名 NuGet 包。

核对 API 时使用包内 nuspec 的 repository commit 对应源码，不要把后续 main 分支的接口误认为已存在于目标包中。Preview.4 包含[发行说明](../../releases/zh-cn/1.0.0-preview.4.md)中的控件文档与集合生命周期修复。迁移指南可以使用最新修订，成员声明仍以目标包版本为准。

### 主库与预览程序的依赖不同

| 能力 | 普通消费应用 | 当前 PreviewHost |
| --- | --- | --- |
| 主题、Form、MultiSelectBox、图表、窗口等 | 主包及其传递依赖 | 通过源码 ProjectReference 引用主库 |
| 代码编辑与高亮 | 按需独立引入 `TheOne.Avalonia.CodeEditor` | 使用 1.0.0-preview.2，并注册 CodeEditorTheme |
| 打印设置／预览 | 按需独立引入 `TheOne.PrintingTools.Avalonia` | 使用 1.0.0-preview.3，在打印窗口局部注册主题 |
| ReactiveUI、Material 图标、Inter 字体、Headless、诊断工具 | 按应用需要决定，不照搬预览依赖 | 为示例与验证使用 |

两个可选包不随主库自动安装，版本独立维护。代码编辑器包名是 **TheOne.Avalonia.CodeEditor**，不是 TheOne.Avalonia.Editor。不要把 PreviewHost、Verification 或本机相邻仓库当作应用依赖。打印桥接见 [ReportPreview](../../controls/zh-cn/report-preview.md#pdf-与打印)。

## 3. 按功能迁移，保留业务层

| 原有功能 | 接入方式及必读文档 |
| --- | --- |
| 官方 Button、TextBox、ComboBox、ListBox、TreeView、Calendar 等 | 保留原生控件、命令和绑定，先让 OneTheme 生效；[基础控件](../../controls/zh-cn/basic.md)、[集合](../../controls/zh-cn/collections.md)、[日期](../../controls/zh-cn/date-time.md) |
| 第三方控件／私有控件 | 对照业务行为和目标公开 API 逐项改造，不能只替换 xmlns；禁止猜测同名属性等价 |
| 编辑页与校验 | [Form / FormItem](../../controls/zh-cn/form.md)，复用 IStepValidator；字段 MemberName 对应模型属性；保留原生输入校验 |
| 多选输入 | [MultiSelectBox](../../controls/zh-cn/multi-select-box.md)，SelectedItems 集合、强类型 ItemAdapter、禁选项与清空语义需按契约迁移 |
| 侧栏与导航 | [NavMenu](../../controls/zh-cn/nav-menu.md)，模型层级、选中项、展开状态和命令保留在 VM 中；只创建有界视口的容器 |
| 数据表格 | 先读 [DataGrid 绑定](../../controls/zh-cn/datagrid-binding.md)，再读 [筛选／冻结／统计](../../controls/zh-cn/datagrid-extensions.md)；不要复制内部 Presenter |
| 登录、主窗口与消息 | [OneWindow](../../controls/zh-cn/windows.md)、[LoginWindow](../../controls/zh-cn/login-window.md)、[owner 路由](../../controls/zh-cn/window-owners.md) |
| 图表、甘特、工作流、报表 | 从[控件索引](../../controls/zh-cn/README.md)进入具体页面，先核对数据模型、编辑能力、序列化和规模限制 |

业务功能不要求重写成预览程序的 ViewModel。PreviewHost 登录示例只做本地演示校验，不能替代目标项目的认证、权限或令牌管理。

### 编译绑定与 DataGrid

为每个视图声明业务 VM 的 `x:DataType`，模板按模型类型声明作用域。运行时设置 DataContext 不会提供静态类型。可变属性通知、动态集合通知仍由业务层实现。不能通过 ReflectionBinding、关闭编译绑定、NoWarn 或整程序集保留来掩盖迁移错误。

DataGrid 在创建视图／表格前注册具体行及嵌套类型；这是托管与 AOT 共用的路径：

```csharp
using Avalonia.Collections;
using TheOne.Avalonia.Controls;

_ = DataGridAot.PreserveRowType<OrderRow>();
// 若列路径是 Customer.Name，还要注册 Customer 的具体模型类型。
_ = DataGridAot.PreserveRowType<Customer>();
var view = new DataGridCollectionView<OrderRow>(orders);
```

`OrderRow`、`Customer`、`orders` 是目标项目的模型和集合占位名。自定义空集合、比较类型、转换器和 NativeAOT 的精确保留要求见 [DataGrid 专页](../../controls/zh-cn/datagrid-binding.md#nativeaot)。不要复制非泛型 object 数据源后期待编译器推断行类型。

### 窗口 owner 与消息

OneToast、OneNotification、OneDialog 的目标必须是已打开的 OneWindow 或 LoginWindow；普通 Window 加载主题后不会自动获得消息宿主。多窗口业务优先显式传 `owner:`，原生 `Show(owner)`、`desktop.MainWindow` 和消息服务默认 `Owner` 是不同概念。登录交接、取消、队列和关闭清理按[窗口 owner 文档](../../controls/zh-cn/window-owners.md)实现，不直接照搬全局静态默认窗口。

## 4. 颜色、尺寸和旧示例适配

配色和 Light / Dark 是两份独立配置。在 UI 线程、注册应用级 OneTheme 后：

```csharp
using Avalonia;
using Avalonia.Styling;
using TheOne.Avalonia.Themes;

OneTheme.SetColor(ThemeColor.Nova);
Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
```

自定义界面用 `DynamicResource One.Surface / One.Text / One.Line / One.Accent` 等语义资源，避免硬编码旧主题颜色；渐变键见[配色资源](../../theme/zh-cn/theme-colors.md)。尺寸继承 `styling:ControlSizing.Size="Small|Normal|Large"` 或 small / normal / large 类。30 / 36 / 44 是标准交互高度，带外边距、多行标签和展开内容的控件总高度不一定相等；不要全局写死 Height。

| 遇到的旧用法 | 当前契约 |
| --- | --- |
| 按 Button 编写 ThemeButton 样式 | ThemeButton 继承 ToggleButton，使用内置图标与勾选状态；只切换 Light / Dark |
| 把配色选择器当包装对象列表 | ThemeColorPicker 是枚举选择器，使用 `SelectThemeColor` 双向绑定 ThemeColor |
| 早期源码示例里的 Default / ThemeColorOption | 当前默认枚举为 `ThemeColor.Nova`（0），选择器直接使用 ThemeColor。此项针对旧源码示例，不表示发布的 preview.2 含这些早期类型 |
| 同时绑定 SelectThemeColor 和 SelectedColor | 选一个属性绑定；SelectedColor 保留兼容，两者共享状态 |
| 选择变化时同步保存文件 | 保存由宿主异步处理；启动注册主题后恢复枚举与明暗模式，控件不自动持久化 |

详情见 [ThemeColorPicker / ThemeButton](../../theme/zh-cn/theme-color-picker.md)。颜色资源通知会合并到下一轮渲染，不以同一同步调用栈中的像素状态作为最终结果。

## 5. 跨平台边界

| 目标／功能 | 接入与验证要求 |
| --- | --- |
| Windows / macOS / Linux 桌面 | 保留各平台启动、字体与原生服务配置；在目标 OS 检查窗口、焦点、快捷键、DPI、输入法、文件对话框和资源路径大小写 |
| Android / iOS / Browser | 本仓库没有这些目标的完整接入及实跑保证；单视图页面须单独适配，不能直接复制 OneWindow / LoginWindow 桌面生命周期和窗口消息宿主 |
| PDF / 打印 | PDF 导出与物理打印是不同验收项；默认打印路径依赖 Windows PDF 处理程序或 CUPS；可选打印包按自己的平台约束接入 |
| NativeAOT / 裁剪 | 只对需要的消费程序与受影响控件验证；生成某个 RID 的产物不等于在该 OS 实跑通过 |
| 图片、字体和外部资源 | 相对资源使用正确的 avares 路径与程序集名，避免复制维护者的本地绝对路径；确认网络与字体回退由应用处理 |

现有 macOS Headless / Skia 记录不能当成 Windows 或 Linux 桌面实跑结论。跨平台状态应写明目标 OS、RID、运行方式与未覆盖功能。

## 6. 执行顺序与验收

1. 保留旧项目的构建与行为基线，列出目标页面和不能变化的业务流程。
2. 先完成依赖兼容和单主题加载，编译成功后验证一页原生输入与一页集合。
3. 按需迁移表单、多选、DataGrid、窗口 owner 及专用控件，每次检查绑定、命令和模型身份。
4. 加入配色／密度及配置恢复，检查明暗、三尺寸、窄视口、焦点、选中与虚拟化。
5. 只运行受影响控件专项；在目标应用和目标 OS 补验真实业务，不用 PreviewHost 通过替代实际集成结果。

以下命令在本仓库根目录运行，示例只验证本次涉及 NavMenu 时的路径：

```sh
dotnet build design/PreviewHost/PreviewHost.csproj -c Release --artifacts-path .preview/migration/artifacts
dotnet .preview/migration/artifacts/bin/PreviewHost/release/PreviewHost.dll --verify-navmenu-fling .preview/migration/navmenu.csv
```

换用其他功能时，从对应控件页及 [Program.cs](../../../design/PreviewHost/Program.cs)选择实际存在的专项入口，不顺带跑全部控件。外部目标项目只构建其必需的项目；产物使用任务自己的目录。包消费参考 [DataGridPackageConsumer](../../../design/PreviewHost/Consumers/DataGridPackageConsumer)，不依赖本地发布日志或二进制目录。

完成后报告：修改了什么、公开契约是否改变、实际构建／交互／目标 OS 验证、剩余限制及下一步。未运行的检查不能写成通过。[完整公共 API 与内置模型](../../api/zh-cn/README.md)列出本库全部公开类型、属性、构造、方法、事件和枚举，并链接声明；控件指南同时列出额外属性和样式。PreviewHost 在各控件独立页面展示完整示例，附本页样式、属性、API／事件与内置模型；仅 Button、Input、Select 合并比较。画布编辑器使用完整视口，说明单独放在页内标签中。当前发布包尚未附带完整 XML API 说明，仓库新增文档及演示以当前源码为准；消费 preview.4 时仍核对包的提交版本。

## 7. 可复制给其他 Codex 的任务说明

将下列占位项替换为实际信息，并提供本仓库 URL 或这份指南。这里的约定已经随文档分发，不依赖维护者本机被忽略的 AGENTS.md / Skill 文件。

```text
目标：将【目标项目／指定页面】接入 TheOne.Avalonia 1.0.0-preview.4。
目标 OS/RID：【填写】。当前框架/SDK：【填写】。范围：【填写】。
保留原有认证、业务模型、命令、DI、路由和用户未提交改动。

先读取目标项目自己的约定、git status --short，以及 TheOne 的
docs/integration/zh-cn/README.md；按功能只读相关控件和主题文档。
先报告版本/主题/DataGrid/窗口生命周期冲突，再分阶段实现。

当前基线：net10.0、Avalonia 12.1.4、主包及 DataGrid 配套包同为 preview.4。
只加载一个 OneTheme；保留原生控件，避免批量改成不存在的 TheOne 包装类。
不复制 PreviewHost 的认证演示或把可选编辑器/打印包当成必需依赖。
使用正确 x:DataType 和编译绑定；DataGrid 先注册具体行及嵌套类型。
不要关闭编译绑定、压制警告或保留整个程序集来绕过问题。
桌面消息目标使用明确的 OneWindow/LoginWindow owner；不要混用普通 Window。
保留虚拟化和有限视口；颜色用 One.* DynamicResource；配色与明暗独立保存。

只修改并验证指定控件/页面，构建使用独立产物目录，不跑全量回归。
交互修复用真实按键/指针；外观改动检查明暗、三尺寸、窄视口和实际绘制。
只有涉及 AOT/绑定/元数据时才跑相应原生消费专项，并区分跨 RID 构建与实跑。
遇到关键版本升级或缺失业务信息时指出阻塞；能独立完成的工作继续推进。
最后列出改动、验证证据、未覆盖平台与剩余问题。
未经单独授权不递增版本、不推送或发布 NuGet。
```
