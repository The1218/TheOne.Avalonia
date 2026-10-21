# OneWindow、LoginWindow 与窗口服务

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

**简体中文** · [English](../en/windows.md)

OneWindow 继承官方 Window，加载 OneTheme 后即可使用。保留原生 Content / ContentTemplate、所有者、Closing、窗口状态和平台窗口生命周期；标题栏、消息与对话框位于模板中，不把业务 Content 包装后重新赋值。

主工作区使用 `OneWindow`；登录并交接主窗口的流程使用 [LoginWindow](login-window.md)。两者均已有专门说明，并保留原生窗口所有者关系。PreviewHost 默认打开 Home 首页，可从可操作示例及用法片段进入完整控件演示。

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

代码隐藏需继承相同类型：`public partial class MainWindow : OneWindow`。桌面启动时设置 `desktop.MainWindow = new MainWindow();`；在 App.axaml 中注册一次 `OneTheme`，窗口内的普通控件会跟随应用主题与密度。

对应 code-behind 的基类也改为 OneWindow。LeftContent / RightContent 支持普通对象、Control，以及 LeftContentTemplate / RightContentTemplate；对象模板需声明 x:DataType。控件继续继承窗口 DataContext，按钮和输入区域不会触发标题栏拖动。

登录界面、认证后交接与进程退出策略见 [LoginWindow](login-window.md)。

## 标题栏与主题

ThemeButton 为无文本的图标 ToggleButton，默认双态切换 Light / Dark。设置继承的 `IsThreeState="True"` 后按 Default → Dark → Light → Default 循环，`IsChecked` 分别为 null / true / false；Default 显示系统图标。配色由独立的 [ThemeColorPicker](../../theme/zh-cn/theme-color-picker.md) 控制。

ShowTitleBar 和 ShowThemeButton 默认为 True；TitleBarHeight 可覆盖。默认 small / normal / large 高度为 42 / 48 / 56，正文及对话框字号为 12 / 14 / 16。使用 Classes 或 ControlSizing.Size，沿用窗口主题与动效继承。

空白标题栏可拖动，双击最大化／还原；右侧按钮使用原生 WindowState、CanMinimize、CanMaximize 和 Close，关闭可由原生 Closing 取消。窗口默认使用 BorderOnly 与扩展客户区，保留平台边框；标题栏主体声明 User 角色，由 OneWindow 统一处理拖动与双击，避免 macOS 原生标题栏命中测试和托管事件同时切换状态；系统按钮继续使用原生装饰角色。真实平台的拖动、缩放、贴靠和 DPI 行为需在对应桌面验证。

ThemeButton 默认切换最近窗口的有效 Light / Dark 主题，图标、提示及辅助名称随有效主题更新。OneWindow.ThemeButtonScope="Application" 切换应用全局主题；有显式局部主题的窗口仍遵循自己的设置。独立按钮也可以放在其他界面：

```xml
<one:ThemeButton Scope="Application" Classes="small" />
<one:ThemeButton Scope="Application" IsThreeState="True" />
<one:OneWindow ThemeButtonScope="Application" ThemeButtonIsThreeState="True" />
```

Application 的 Default 自动跟随系统明暗变化；Window 的 Default 恢复继承应用主题。控制同一界面的按钮应使用相同作用域：窗口显式主题优先于应用主题。按钮同时观察目标的请求主题和实际主题，因此切回 Default 时，即使实际明暗未变，状态也会同步。

点击使用 SetCurrentValue，保留原有绑定；主题快捷按钮切换明暗，跟随系统仍可通过 RequestedThemeVariant=Default 设置。PreviewHost 的主窗体使用 Application 范围，与原主题下拉框同步。

## 自动消息宿主

owner 的解析顺序、原生窗口归属与消息目标的区别、线程要求、Dialog 队列和登录交接详见[窗口 owner 与消息服务](window-owners.md)。

使用 OneWindow 并显示窗口后即可调用，不需查找 TopLevel、创建管理器或注册宿主：

```csharp
using Avalonia.Controls.Notifications;
using TheOne.Avalonia.Controls;

OneNotification.Show("保存成功", "资料已保存。", NotificationType.Success);
OneToast.Show("更新完成", NotificationType.Success);
var result = await OneDialog.ShowAsync("确认", "是否继续？", OneDialogButtons.YesNoCancel);
```

通知使用窗口内部的原生 WindowNotificationManager，默认右上方、最多 4 条活动通知、4 秒自动关闭；Toast 使用独立管理器，顶部居中、最多 3 条、2.5 秒自动关闭。TimeSpan.Zero 保留到手动关闭。两者沿用原生 NotificationCard 生命周期、语义状态与 Motion.Enabled 动画；CloseAll 可清空指定类别。Notification 保留标题和关闭按钮，三档宽度 280 / 320 / 360，字号 11 / 12 / 13、内边距 8 / 10 / 12。Toast 使用 One.ToastCard 专用模板，仅展示状态图标与正文，无标题、关闭按钮、装饰色条或图标徽章；传入 INotification.Title 也不会显示。短内容自然收缩，长内容在 280 / 320 / 360 最大宽度内换行，窄窗口继续受可用宽度约束；图标与整段文字垂直居中，无固定正文高度或省略号。Toast 三档字号 12 / 14 / 16、图标 16 / 18 / 20、内边距 12,8 / 16,10 / 20,12。持久 Toast 可点击正文或调用 CloseAll 关闭，点击／关闭回调仍执行一次。

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
    public string Name
    {
        get => _name;
        set { if (SetProperty(ref _name, value)) OnPropertyChanged(nameof(Summary)); }
    }
    public string Summary => $"姓名：{Name}";
    // 在绑定的 SaveCommand / CancelCommand 中调用：
    public void Save() => RequestClose(OneDialogResult.Ok);
    public void Cancel() => RequestClose(OneDialogResult.Cancel);
}
```

RequestClose 会发布 CloseRequested 事件，协调器只关闭与此 VM 关联的请求；父页面 await 得到 OneDialogResult。该事件支持后台线程触发，排队中的 VM 也可以主动结束，不会创建其 View 或影响当前对话框。关闭按钮、可选蒙层点击返回 Closed；Escape 返回 Cancel；取消令牌取消 Task；宿主关闭返回 Closed。

`SetProperty` 只在字段值变化时发布属性通知；派生 VM 使用受保护的 `OnPropertyChanged(nameof(Summary))` 通知计算属性或命令条件变化，无需重复声明 PropertyChanged 事件或依赖反射绑定。通知方法不切换线程，绑定状态应在 UI 上下文更新。需要强类型业务结果时保存在 VM 中，仅在 OneDialogResult.Ok 且接口已成功后读取；Cancel / Closed 不应触发保存成功分支。

完成、取消或宿主关闭都会解绑关闭事件，同一个 VM 可顺序复用。为避免同一个关闭事件误关多个窗口，同一个 DialogContext 不允许同时用于多个请求；同时调用返回故障 Task。千条泛型请求验证仅创建当前显示的一个 View，其余只保留请求数据。PreviewHost「消息与对话框 → Dialog」中的自定义表单可切换标题、关闭按钮、蒙层及 Escape 选项，并通过编译绑定的内部保存／取消命令验证返回值。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### DialogContext

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.DialogContext.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### DialogOptions

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.DialogOptions.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Buttons`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L16) | `TheOne.Avalonia.Controls.OneDialogButtons` | 可写; CLR / init-only | `OneDialogButtons.OkCancel` | 对话框按钮配置集合／标志；由选项类型和关闭结果契约解释。 可选值：`Ok`, `OkCancel`, `YesNo`, `YesNoCancel` |
| [`CloseOnEscape`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L14) | `bool` | 可写; CLR / init-only | `true` | 允许 Esc 关闭对话框；结果与取消语义按对话框选项处理。 |
| [`CloseOnOverlayClick`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L13) | `bool` | 可写; CLR / init-only | `false` | 允许点击遮罩关闭弹窗；取消／返回结果遵循对话框选项。 |
| [`MotionEnabled`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L9) | `bool?` | 可写; CLR / init-only | `null` | 该弹窗的动效开关；同时遵守继承的 Motion.Enabled。 |
| [`ShowButtons`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L15) | `bool` | 可写; CLR / init-only | `false` | 显示默认按钮区；自定义内容中的按钮不受此开关控制。 |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L12) | `bool` | 可写; CLR / init-only | `true` | 显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`ShowTitle`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L11) | `bool` | 可写; CLR / init-only | `true` | 显示对话框标题区；标题值仍保留在选项中。 |
| [`Size`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L8) | `TheOne.Avalonia.Styling.ControlSize?` | 可写; CLR / init-only | `null` | 对话框尺寸枚举；采用库提供的尺寸档位。 可选值：`Small`, `Normal`, `Large` |
| [`Title`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L10) | `string?` | 可写; CLR / init-only | `null` | 显示标题；不等于业务主键或本地化资源键。 |

### OneDialog

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialog.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L217) | `Avalonia.Controls.Window?` | 可写; CLR / n/a | `accessor-defined / state` | `child.Show(parent)` / `child.ShowDialog<T>(parent)` 建立的原生窗口所属关系，影响平台窗口生命周期。 |

### OneDialogPresenter

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialogPresenter.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Buttons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L44) | `TheOne.Avalonia.Controls.OneDialogButtons` | 可写; OneWay | `0 / default` | 对话框按钮配置集合／标志；由选项类型和关闭结果契约解释。 可选值：`Ok`, `OkCancel`, `YesNo`, `YesNoCancel` |
| [`CloseOnEscape`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L68) | `bool` | 可写; OneWay | `true` | 允许 Esc 关闭对话框；结果与取消语义按对话框选项处理。 |
| [`ShowButtons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L62) | `bool` | 可写; OneWay | `true` | 显示默认按钮区；自定义内容中的按钮不受此开关控制。 |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L56) | `bool` | 可写; OneWay | `true` | 显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`ShowTitle`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L50) | `bool` | 可写; OneWay | `true` | 显示对话框标题区；标题值仍保留在选项中。 |

### OneNotification

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.OneNotification.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L120) | `Avalonia.Controls.Window?` | 可写; CLR / n/a | `accessor-defined / state` | `child.Show(parent)` / `child.ShowDialog<T>(parent)` 建立的原生窗口所属关系，影响平台窗口生命周期。 |

### OneToast

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.OneToast.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L159) | `Avalonia.Controls.Window?` | 可写; CLR / n/a | `accessor-defined / state` | `child.Show(parent)` / `child.ShowDialog<T>(parent)` 建立的原生窗口所属关系，影响平台窗口生命周期。 |

### OneWindow

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.OneWindow.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Current`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L123) | `TheOne.Avalonia.Controls.OneWindow?` | 只读; CLR / n/a | `accessor-defined / state` | UI 线程。 |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L122) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`LeftContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L67) | `object?` | 可写; OneWay | `null` | 窗口标题栏左侧扩展内容，位于库提供的标题栏结构内。 |
| [`LeftContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L79) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`RightContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L73) | `object?` | 可写; OneWay | `null` | 窗口标题栏右侧扩展内容；避免覆盖原生窗口操作区域。 |
| [`RightContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L85) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`ShowThemeButton`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L91) | `bool` | 可写; OneWay | `true` | 显示窗口内置明暗切换按钮；配色资源仍由 ThemeColorPicker 或 OneTheme 控制。 |
| [`ShowTitleBar`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L110) | `bool` | 可写; OneWay | `true` | 显示库提供的标题栏区域；窗口生命周期与 owner 不因此变化。 |
| [`ThemeButtonIsThreeState`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L104) | `bool` | 可写; OneWay | `false` | Includes Default in the built-in theme button's Light/Dark cycle. |
| [`ThemeButtonScope`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L97) | `TheOne.Avalonia.Controls.ThemeButtonScope` | 可写; OneWay | `0 / default` | 窗口内置主题按钮的切换作用域；使用枚举区分应用与窗口。 可选值：`Window`, `Application` |
| [`TitleBarHeight`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L116) | `double` | 可写; OneWay | `48` | 标题栏区域的 DIP 高度；改变时保留标题与窗口按钮点击空间。 校验：`value => double.IsFinite(value) && value >= 0` |

### ThemeButton

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButton.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Scope`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L30) | `TheOne.Avalonia.Controls.ThemeButtonScope` | 可写; OneWay | `0 / default` | 主题按钮切换的作用域；影响 Light/Dark，不切换 ThemeColor 颜色枚举。 可选值：`Window`, `Application` |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`DialogContext`](../../api/zh-cn/TheOne.Avalonia.Controls.DialogContext.md) | `void DialogContext.RequestClose(OneDialogResult result = OneDialogResult.Ok)` | Requests completion of this dialog, including when it is still queued. |
| [`OneDialog`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialog.md) | `static Task<OneDialogResult> OneDialog.ShowAsync<TView, TViewModel>(TViewModel viewModel, DialogOptions? options = null, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken)) where TView : UserControl, new() where TViewModel : class` | Creates a typed view lazily on the UI thread; compiled bindings and NativeAOT are supported. |
| [`OneDialog`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialog.md) | `static Task<OneDialogResult> OneDialog.ShowAsync(string title, string message, OneDialogButtons buttons = OneDialogButtons.Ok, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken))` | 内部调度至 UI 线程。 |
| [`OneDialog`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialog.md) | `static Task<OneDialogResult> OneDialog.ShowContentAsync(object? content, string? title = null, OneDialogButtons buttons = OneDialogButtons.OkCancel, IDataTemplate? contentTemplate = null, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken))` | 内部调度至 UI 线程。 |
| [`OneNotification`](../../api/zh-cn/TheOne.Avalonia.Controls.OneNotification.md) | `static void OneNotification.Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)` | Shows on the UI thread. The owner is optional; the registered default owner or most recently activated visible themed window is used. |
| [`OneNotification`](../../api/zh-cn/TheOne.Avalonia.Controls.OneNotification.md) | `static void OneNotification.Show(INotification notification, Window? owner = null)` | UI 线程。 |
| [`OneNotification`](../../api/zh-cn/TheOne.Avalonia.Controls.OneNotification.md) | `static Task OneNotification.ShowAsync(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)` | 内部调度至 UI 线程。 |
| [`OneNotification`](../../api/zh-cn/TheOne.Avalonia.Controls.OneNotification.md) | `static void OneNotification.CloseAll(Window? owner = null)` | UI 线程。 |
| [`OneToast`](../../api/zh-cn/TheOne.Avalonia.Controls.OneToast.md) | `static void OneToast.Show(string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)` | UI 线程。 |
| [`OneToast`](../../api/zh-cn/TheOne.Avalonia.Controls.OneToast.md) | `static void OneToast.Show(INotification notification, Window? owner = null)` | Shows the notification's message and status icon, ignoring Title, while retaining expiration and click/close callbacks. |
| [`OneToast`](../../api/zh-cn/TheOne.Avalonia.Controls.OneToast.md) | `static Task OneToast.ShowAsync(string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)` | 内部调度至 UI 线程。 |
| [`OneToast`](../../api/zh-cn/TheOne.Avalonia.Controls.OneToast.md) | `static void OneToast.CloseAll(Window? owner = null)` | UI 线程。 |

### 事件订阅

按需订阅下列通知，并在宿主释放时解除订阅；这里只列事件入口，不列路由事件标识字段。

| 类型 | 事件 | 用途 |
| --- | --- | --- |
| [`DialogContext`](../../api/zh-cn/TheOne.Avalonia.Controls.DialogContext.md) | `PropertyChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`DialogContext`](../../api/zh-cn/TheOne.Avalonia.Controls.DialogContext.md) | `CloseRequested` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs.md) | 1 |
| [`TheOne.Avalonia.Controls.DialogOptions`](../../api/zh-cn/TheOne.Avalonia.Controls.DialogOptions.md) | 9 |
| [`TheOne.Avalonia.Controls.OneDialogButtons`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialogButtons.md) | 4 |
| [`TheOne.Avalonia.Controls.OneDialogResult`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialogResult.md) | 5 |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButtonScope.md) | 2 |
| [`TheOne.Avalonia.Styling.ControlSize`](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSize.md) | 3 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
