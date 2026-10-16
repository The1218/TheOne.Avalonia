# LoginWindow 与主窗体交接

**简体中文** · [English](../en/login-window.md)

主题与公共窗口服务见[窗口文档](windows.md)。

登录窗的显式 owner、错误提示辅助方法、Toast / Notification / Dialog 生命周期和交接后的消息归属详见[窗口 owner 与消息服务](window-owners.md)。

## LoginWindow 与主窗体交接

LoginWindow 直接继承官方 Window，使用 OneTheme 的语义色、字号和 small / normal / large 密度。默认保留平台完整窗口装饰及标题栏，大小 440 × 540、内容内边距 24、居中显示，关闭按钮保留原生行为；默认禁止缩放和最大化，可按应用需要覆盖。类库不提供账号、密码或认证规则，Content / ContentTemplate 均由开发者定义，支持控件和有明确 x:DataType 的模型模板。

```xml
<one:LoginWindow xmlns="https://github.com/avaloniaui"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:one="using:TheOne.Avalonia.Controls"
                 xmlns:views="using:YourApp.Views"
                 x:Class="YourApp.SignInWindow" Title="登录工作区">
  <views:LoginFormView />
</one:LoginWindow>
```

对应 code-behind 继承 LoginWindow，LoginFormView 内部启用编译绑定并声明自己的 ViewModel 类型。应用启动时把登录窗口指定为桌面 MainWindow，推荐使用 OnMainWindowClose：

```csharp
public override void OnFrameworkInitializationCompleted()
{
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
        desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
        desktop.MainWindow = new SignInWindow();
    }
    base.OnFrameworkInitializationCompleted();
}
```

在应用自己的认证成功回调中传入全新、尚未显示且没有原生 Window.Owner 的 OneWindow。窗口须在 UI 线程构造：

```csharp
// UI 线程认证成功回调；MainWindow 继承 OneWindow。
var mainWindow = new MainWindow();
bool opened = await login.CompleteLoginAsync(mainWindow);
// UI 线程上的同步入口：login.CompleteLogin(mainWindow);
```

CompleteLoginAsync 将交接切回 UI 线程；同步入口要求 UI 线程。交接时先执行登录窗口的原生 Closing 处理，接受关闭后将 desktop.MainWindow 设为传入实例，校正 OneToast.Owner、OneNotification.Owner、OneDialog.Owner，然后独立 Show 主窗体；再次检查三个 owner 后关闭登录窗口。最终校正也覆盖登录 Closed 回调更改 owner 的情况。主窗体不会成为登录窗口的 owned window，关闭登录窗口不会连带销毁主窗体或其消息宿主。应用无需另行 Show、Close 或设置 MainWindow。

Closing 取消时返回 false，传入窗口保持未打开状态，由调用方保留或释放；交接正在进行时的重复调用同样返回 false。成功后的重复调用返回 true，不再次打开窗口，即使传入另一实例也不会创建第二个主窗体，未使用的实例仍由调用方负责。调用时登录窗口必须是已显示、无原生 Owner 的 desktop.MainWindow；已经关闭、作为子窗口显示或未注册为主窗体的登录窗口不能交接。原有 Func<OneWindow> 工厂重载保留兼容：工厂只在 Closing 接受关闭后于 UI 线程执行，适合在后台认证完成后延迟构造窗口。

交接期间短暂使用 OnExplicitShutdown，最终恢复调用前的 ShutdownMode，避免关闭回调或主窗体显示失败导致应用意外退出。Show 失败会抛出异常，恢复登录 MainWindow 和交接前的三个消息 owner，并释放已接受但打开失败的主窗体及其 owned 子窗口，可在登录表单内显示错误并重试。业务认证失败不调用完成入口；登录窗口 Closed 时应取消仍在运行的认证请求，并在回调前检查取消状态，关闭后的迟到完成调用会被拒绝。

LoginWindow 打开或激活时自动注册三个消息服务的默认 Owner，内部包含独立 Toast、Notification 和 Dialog 层。服务的 owner 参数现在接受 Window（仅支持 LoginWindow / OneWindow）；显式 owner 优先，未指定时使用仍可用的默认 owner，隐藏或关闭后回退到其他可用的主题窗口。Owner 属性通过弱引用存储，须在 UI 线程读写。OneWindow.Current 继续只返回可见的 OneWindow。三个错误提示方法明确绑定当前登录窗口，即使其他窗口激活也不会把登录错误显示到其他窗口：

```csharp
login.ShowErrorToast("账号或密码错误");
login.ShowErrorNotification("认证服务暂不可用", title: "登录失败");
OneDialogResult result = await login.ShowErrorDialog("请检查账号和密码", title: "登录失败");
```

ShowErrorToast / ShowErrorNotification 使用 NotificationType.Error，在 UI 线程调用，可传 expiration（TimeSpan.Zero 表示保留）；ShowErrorDialog 返回可等待的 Ok / Closed 结果，并支持 CancellationToken。登录窗口关闭时，当前及排队中的对话框以 Closed 完成，不迁移到主窗体；交接后的新请求使用 OneWindow 的宿主。业务 Content、DataContext、ContentTemplate 与宿主实例也不复制到主窗体。

LoginWindow 不管理依赖注入容器或 .NET Generic Host。Generic Host 及应用共享服务应与应用生命周期关联，在桌面 lifetime 的 Exit / Shutdown 路径停止、释放；不要把 Host.StopAsync / Dispose 注册到登录窗口 Closed。登录验证的局部取消与清理仍由登录表单负责。

运行 `dotnet run --project design/PreviewHost -- --login` 可验证完整启动和交接流程：示例表单使用编译绑定，账号和密码非空即通过本地演示校验，空输入显示错误 Toast，提供三个错误提示按钮；关闭时取消等待并清空密码。它不连接认证服务。主窗体目录不再提供 OneWindow / LoginWindow 展示页；完整登录流程使用该独立启动入口。

定向交互验证使用 `--verify-loginwindow`；`--verify-login-lifetime <ShutdownMode>` 在真实桌面主循环中验证退出策略，`--verify-login-lifetime-close <ShutdownMode>` 验证登录 Closed 回调立即关闭新主窗体。两个入口分别传入 OnMainWindowClose、OnLastWindowClose、OnExplicitShutdown，并在独立进程中运行。截图只导出当前控件：`--render-controls .preview/loginwindow LoginWindow`。

实机检查窗口创建次数可运行 `dotnet run --project design/PreviewHost -- --login --trace-login`。可选日志以 `LOGIN_TRACE` 开头，记录校验忙碌状态、完成请求、主窗体工厂调用次数、窗口打开／关闭及应用退出时的窗口数量，不记录账号或密码。校验延迟与退出策略保持演示原有设置。

生命周期实现依据 [Avalonia 桌面生命周期](https://docs.avaloniaui.net/docs/fundamentals/application-lifetimes) 与 [12.1.4 Window 源码](https://github.com/AvaloniaUI/Avalonia/blob/12.1.4/src/Avalonia.Controls/Window.cs)。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节只列出本项目增加的 Classes、属性、扩展 API／事件及关联模型，不重复 Avalonia 原生继承 API。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用可见示例对照本项目附加样式，用 Description 组件说明专属属性、API 签名与模型字段；ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### LoginWindow

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.LoginWindow.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/LoginWindow.cs#L20) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### OneWindow

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.OneWindow.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Current`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L39) | `TheOne.Avalonia.Controls.OneWindow?` | 只读; CLR / n/a | `accessor-defined / state` | UI 线程。 |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L38) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`LeftContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L30) | `object?` | 可写; OneWay | `null` | 窗口标题栏左侧扩展内容，位于库提供的标题栏结构内。 |
| [`LeftContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L32) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`RightContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L31) | `object?` | 可写; OneWay | `null` | 窗口标题栏右侧扩展内容；避免覆盖原生窗口操作区域。 |
| [`RightContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L33) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`ShowThemeButton`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L34) | `bool` | 可写; OneWay | `true` | 显示窗口内置明暗切换按钮；配色资源仍由 ThemeColorPicker 或 OneTheme 控制。 |
| [`ShowTitleBar`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L36) | `bool` | 可写; OneWay | `true` | 显示库提供的标题栏区域；窗口生命周期与 owner 不因此变化。 |
| [`ThemeButtonScope`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L35) | `TheOne.Avalonia.Controls.ThemeButtonScope` | 可写; OneWay | `0 / default` | 窗口内置主题按钮的切换作用域；使用枚举区分应用与窗口。 可选值：`Window`, `Application` |
| [`TitleBarHeight`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L37) | `double` | 可写; OneWay | `48` | 标题栏区域的 DIP 高度；改变时保留标题与窗口按钮点击空间。 校验：`value => double.IsFinite(value) && value >= 0` |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| LoginWindow | `one-large` | 模板自动类；不手动绑定 | one&#124;LoginWindow.one-large => FontSize=16 |
| LoginWindow | `one-normal` | 模板自动类；不手动绑定 | one&#124;LoginWindow.one-normal => FontSize=14 |
| LoginWindow | `one-small` | 模板自动类；不手动绑定 | one&#124;LoginWindow.one-small => FontSize=12 |
| OneWindow | `one-large` | 模板自动类；不手动绑定 | one&#124;OneWindow.one-large => TitleBarHeight=56; FontSize=16 |
| OneWindow | `one-normal` | 模板自动类；不手动绑定 | one&#124;OneWindow.one-normal => TitleBarHeight=48; FontSize=14 |
| OneWindow | `one-small` | 模板自动类；不手动绑定 | one&#124;OneWindow.one-small => TitleBarHeight=42; FontSize=12 |

`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出附加外观类的控件使用默认主题及其专属属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.LoginWindow`](../../api/zh-cn/TheOne.Avalonia.Controls.LoginWindow.md) | Class | 1 | 8 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogResult`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialogResult.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneWindow`](../../api/zh-cn/TheOne.Avalonia.Controls.OneWindow.md) | Class | 10 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButtonScope.md) | Enum | 2 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
