# OneWindow、LoginWindow 与窗口服务

OneWindow 继承官方 Window，加载 OneTheme 后即可使用。保留原生 Content / ContentTemplate、所有者、Closing、窗口状态和平台窗口生命周期；标题栏、消息与对话框位于模板中，不把业务 Content 包装后重新赋值。

```xml
<one:OneWindow xmlns="https://github.com/avaloniaui"
               xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
               xmlns:one="using:TheOne.Avalonia.Controls"
               xmlns:vm="using:YourApp.ViewModels"
               x:Class="YourApp.MainWindow" x:DataType="vm:MainViewModel"
               Title="我的工作区" Width="1000" Height="720">
  <one:OneWindow.LeftContent>
    <TextBlock Text="{Binding WorkspaceName}" />
  </one:OneWindow.LeftContent>
  <one:OneWindow.RightContent>
    <StackPanel Orientation="Horizontal" Spacing="8">
      <TextBox Width="160" Text="{Binding Search, Mode=TwoWay}" />
      <Button Classes="ghost" Content="设置" Command="{Binding SettingsCommand}" />
    </StackPanel>
  </one:OneWindow.RightContent>
  <Grid><!-- 正常业务内容 --></Grid>
</one:OneWindow>
```

对应 code-behind 的基类也改为 OneWindow。LeftContent / RightContent 支持普通对象、Control，以及 LeftContentTemplate / RightContentTemplate；对象模板需声明 x:DataType。控件继续继承窗口 DataContext，按钮和输入区域不会触发标题栏拖动。

登录界面、认证后交接与进程退出策略见 [LoginWindow](login-window.md)。

## 标题栏与主题

ShowTitleBar 和 ShowThemeButton 默认为 True；TitleBarHeight 可覆盖。默认 small / normal / large 高度为 42 / 48 / 56，正文及对话框字号为 12 / 14 / 16。使用 Classes 或 ControlSizing.Size，沿用窗口主题与动效继承。

空白标题栏可拖动，双击最大化／还原；右侧按钮使用原生 WindowState、CanMinimize、CanMaximize 和 Close，关闭可由原生 Closing 取消。窗口默认使用 BorderOnly 与扩展客户区，保留平台边框；标题栏主体声明 User 角色，由 OneWindow 统一处理拖动与双击，避免 macOS 原生标题栏命中测试和托管事件同时切换状态；系统按钮继续使用原生装饰角色。真实平台的拖动、缩放、贴靠和 DPI 行为需在对应桌面验证。

ThemeButton 默认切换最近窗口的有效 Light / Dark 主题，图标、提示及辅助名称随有效主题更新。OneWindow.ThemeButtonScope="Application" 切换应用全局主题；有显式局部主题的窗口仍遵循自己的设置。独立按钮也可以放在其他界面：

```xml
<one:ThemeButton Scope="Application" Classes="small" />
```

点击使用 SetCurrentValue，保留原有绑定；主题快捷按钮切换明暗，跟随系统仍可通过 RequestedThemeVariant=Default 设置。PreviewHost 的主窗体使用 Application 范围，与原主题下拉框同步。

## 自动消息宿主

使用 OneWindow 并显示窗口后即可调用，不需查找 TopLevel、创建管理器或注册宿主：

```csharp
using Avalonia.Controls.Notifications;
using TheOne.Avalonia.Controls;

OneNotification.Show("保存成功", "资料已保存。", NotificationType.Success);
OneToast.Show("更新完成", NotificationType.Success);
var result = await OneDialog.ShowAsync("确认", "是否继续？", OneDialogButtons.YesNoCancel);
```

通知使用窗口内部的原生 WindowNotificationManager，默认右上方、最多 4 条活动通知、4 秒自动关闭；Toast 使用独立管理器，顶部居中、最多 3 条、2.5 秒自动关闭。TimeSpan.Zero 保留到手动关闭。两者使用紧凑的 NotificationCard：三档宽度 280 / 320 / 360，字号 11 / 12 / 13、内边距 8 / 10 / 12，沿用语义类型与 Motion.Enabled 动画；CloseAll 可清空指定类别。

未传 owner 时使用各服务的 Owner；默认 Owner 随 OneWindow / LoginWindow 打开或激活自动更新，也可在 UI 线程设置。隐藏、关闭或尚未打开的默认 owner 会回退到最近激活的可见主题窗口。多窗口时可显式传 owner；不可用的显式 owner 报错，不静默转到其他窗口。OneWindow.Current 提供 UI 线程上的最近可见 OneWindow，仅使用弱引用登记，关闭时自动移除。

同步 Show / CloseAll 在 UI 线程调用。后台任务使用异步入口；对话框 API 始终自动切回 UI 线程：

```csharp
await OneNotification.ShowAsync("处理完成", "可继续操作。", owner: mainWindow);
await OneToast.ShowAsync("同步完成", owner: mainWindow);
```

没有打开的 OneWindow 或 LoginWindow 时 API 报出生命周期错误，避免消息悄悄丢失。普通 Window 不自动获得宿主。

## 对话框结果与生命周期

OneDialogButtons 支持 Ok、OkCancel、YesNo、YesNoCancel；结果为 Ok、Cancel、Yes、No 或 Closed。Escape 返回 Cancel；窗口关闭返回 Closed。取消令牌会取消 Task 并关闭当前弹层，也可以独立取消排队中的请求。

同一窗口的对话框按调用顺序排队，只为活动请求创建 OneDialogPresenter。背景内容、标题栏左右内容和主题按钮在模态期间禁用，初始焦点位于主要按钮，Tab 在对话框中循环；关闭后恢复先前焦点。长内容在有限视口内滚动，操作区保持可见。

```csharp
using var cancellation = new CancellationTokenSource();
var editor = new TextBox { Text = "工作区" };
var result = await OneDialog.ShowContentAsync(editor, "编辑名称",
    OneDialogButtons.OkCancel, owner: mainWindow, cancellationToken: cancellation.Token);
```

ShowContentAsync 也支持模型与显式 contentTemplate。Control 内容不能已经有父级；关闭会解除父级，输入状态保留，可再次展示。大数据表单仍需有界虚拟化列表；对话框宿主不会为任意业务集合自动增加虚拟化。

窗口关闭时清理活动和排队请求，防止等待悬空；关闭被取消时继续保留宿主。自定义 OneWindow / LoginWindow 模板必须保留 PART_VisualLayerManager、PART_ContentPresenter、PART_Notifications、PART_Toasts、PART_DialogShield 和 PART_DialogLayer，标题栏自定义继续遵循相应 PART_*。缺失消息部件时 API 报错。

PreviewHost 的“消息与对话框”保留 Toast、Dialog、Notification 独立页面，支持类型、标题、正文、尺寸及动效配置。通知／Toast 支持位置、显示上限、停留秒数、自动／手动关闭和回调状态；Dialog 支持标题栏、关闭按钮、确认按钮组合、蒙层与 Esc 关闭及 VM 表单返回。OneWindow、LoginWindow、NotificationCard 不再作为目录展示页，控件和底层回归保留；ThemeButton 位于 Button 示例。编译绑定与 NativeAOT 检查见 [NativeAOT 文档](../native-aot.md)。

## 泛型 VM 对话框

```csharp
var vm = new EditorViewModel();
var result = await OneDialog.ShowAsync<EditorView, EditorViewModel>(vm,
    new DialogOptions
    {
        Title = "编辑资料",
        ShowTitle = true,
        ShowCloseButton = true,
        CloseOnOverlayClick = false,
        CloseOnEscape = true
    });
if (result == OneDialogResult.Ok)
{
    // 使用 vm 中保存的数据，更新父页面。
}
```

这是 OneWindow 内的模态弹层，阻止背景内容交互，保持键盘焦点循环，关闭后恢复原焦点。TView 必须继承 UserControl 并提供 public 无参构造函数；TViewModel 可以是普通 class，也可以继承 DialogContext。内部通过泛型构造 View 并赋值 DataContext，完全不需要 ViewLocator、运行时类型查找或反射创建。View 的 AXAML 应启用编译绑定并声明 x:DataType 为对应 VM。

DialogOptions 为不可变 record；不传时每次创建默认配置，不共享可变全局设置。默认 ShowTitle=true（Title 为空时隐藏标题）、ShowCloseButton=true、CloseOnOverlayClick=false、CloseOnEscape=true、ShowButtons=false、Buttons=OkCancel。通用底部按钮可通过 ShowButtons=true 打开，Buttons 指定按钮组合。可选 Size、MotionEnabled 覆盖当前弹层，未设置时继承父窗体。ShowTitle 和 ShowCloseButton 均关闭时整个头部不占空间。旧字符串 / ShowContentAsync 接口继续显示通用底部按钮。

```csharp
public sealed class EditorViewModel : DialogContext
{
    private string _name = "新资料";
    public string Name { get => _name; set => SetProperty(ref _name, value); }
    // 在绑定的 SaveCommand / CancelCommand 中调用：
    public void Save() => RequestClose(OneDialogResult.Ok);
    public void Cancel() => RequestClose(OneDialogResult.Cancel);
}
```

RequestClose 会发布 CloseRequested 事件，协调器只关闭与此 VM 关联的请求；父页面 await 得到 OneDialogResult。该事件支持后台线程触发，排队中的 VM 也可以主动结束，不会创建其 View 或影响当前对话框。关闭按钮、可选蒙层点击返回 Closed；Escape 返回 Cancel；取消令牌取消 Task；宿主关闭返回 Closed。

完成、取消或宿主关闭都会解绑关闭事件，同一个 VM 可顺序复用。为避免同一个关闭事件误关多个窗口，同一个 DialogContext 不允许同时用于多个请求；同时调用返回故障 Task。千条泛型请求验证仅创建当前显示的一个 View，其余只保留请求数据。PreviewHost「窗口 OneWindow」中的自定义表单可切换标题、关闭按钮、蒙层及 Escape 选项，并通过编译绑定的内部保存／取消命令验证返回值。
