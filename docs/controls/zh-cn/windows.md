# OneWindow、LoginWindow 与窗口服务

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

ThemeButton 为无文本的图标 ToggleButton，勾选代表 Dark，仅切换 Light / Dark；配色由独立的 [ThemeColorPicker](../../theme/zh-cn/theme-color-picker.md) 控制。

ShowTitleBar 和 ShowThemeButton 默认为 True；TitleBarHeight 可覆盖。默认 small / normal / large 高度为 42 / 48 / 56，正文及对话框字号为 12 / 14 / 16。使用 Classes 或 ControlSizing.Size，沿用窗口主题与动效继承。

空白标题栏可拖动，双击最大化／还原；右侧按钮使用原生 WindowState、CanMinimize、CanMaximize 和 Close，关闭可由原生 Closing 取消。窗口默认使用 BorderOnly 与扩展客户区，保留平台边框；标题栏主体声明 User 角色，由 OneWindow 统一处理拖动与双击，避免 macOS 原生标题栏命中测试和托管事件同时切换状态；系统按钮继续使用原生装饰角色。真实平台的拖动、缩放、贴靠和 DPI 行为需在对应桌面验证。

ThemeButton 默认切换最近窗口的有效 Light / Dark 主题，图标、提示及辅助名称随有效主题更新。OneWindow.ThemeButtonScope="Application" 切换应用全局主题；有显式局部主题的窗口仍遵循自己的设置。独立按钮也可以放在其他界面：

```xml
<one:ThemeButton Scope="Application" Classes="small" />
```

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

PreviewHost 的“消息与对话框”保留 Toast、Dialog、Notification 独立页面，支持类型、正文、尺寸及动效配置；Toast 不提供标题配置，Notification 和 Dialog 保留标题。通知／Toast 支持位置、显示上限、停留秒数、自动／手动关闭和回调状态；Dialog 支持标题栏、关闭按钮、确认按钮组合、蒙层与 Esc 关闭及 VM 表单返回。OneWindow、LoginWindow、NotificationCard 不再作为目录展示页，控件和底层回归保留；ThemeButton 位于 Button 示例。独立编译绑定与原生消费示例见 [NativeAotSmoke](../../../design/PreviewHost/Consumers/NativeAotSmoke/Program.cs)。

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

完成、取消或宿主关闭都会解绑关闭事件，同一个 VM 可顺序复用。为避免同一个关闭事件误关多个窗口，同一个 DialogContext 不允许同时用于多个请求；同时调用返回故障 Task。千条泛型请求验证仅创建当前显示的一个 View，其余只保留请求数据。PreviewHost「消息与对话框 → Dialog」中的自定义表单可切换标题、关闭按钮、蒙层及 Escape 选项，并通过编译绑定的内部保存／取消命令验证返回值。

Toast 独立检查由 NativeAotSmoke 的 `--verify-toast` 提供，覆盖明暗主题、三档尺寸、四种状态、窄窗口多行居中、回调与自动消失；托管运行不传 `--require-aot`。只有涉及 AOT／绑定／元数据时，才发布并实跑原生产物，附加 `--require-aot`。只构建所需消费项目，使用任务独立产物目录，见[接入与验证约定](../../integration/zh-cn/README.md#6-执行顺序与验收)。PreviewHost 的 `--verify-buttons-messages` 是按钮与消息组合入口，不是 Toast 独占入口。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 在各控件页直接标明示例的 Classes、尺寸及状态；公共属性、API／事件和模型字段使用 Description 组件说明，包含属性类型、默认值、绑定模式、校验条件及 API 签名。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### DialogContext

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.DialogContext.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### DialogOptions

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.DialogOptions.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Buttons`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L14) | `TheOne.Avalonia.Controls.OneDialogButtons` | 可写; CLR / init-only | `OneDialogButtons.OkCancel` | 对话框按钮配置集合／标志；由选项类型和关闭结果契约解释。 可选值：`Ok`, `OkCancel`, `YesNo`, `YesNoCancel` |
| [`CloseOnEscape`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L12) | `bool` | 可写; CLR / init-only | `true` | 允许 Esc 关闭对话框；结果与取消语义按对话框选项处理。 |
| [`CloseOnOverlayClick`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L11) | `bool` | 可写; CLR / init-only | `false` | 允许点击遮罩关闭弹窗；取消／返回结果遵循对话框选项。 |
| [`MotionEnabled`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L7) | `bool?` | 可写; CLR / init-only | `null` | 该弹窗的动效开关；同时遵守继承的 Motion.Enabled。 |
| [`ShowButtons`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L13) | `bool` | 可写; CLR / init-only | `false` | 显示默认按钮区；自定义内容中的按钮不受此开关控制。 |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L10) | `bool` | 可写; CLR / init-only | `true` | 显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`ShowTitle`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L9) | `bool` | 可写; CLR / init-only | `true` | 显示对话框标题区；标题值仍保留在选项中。 |
| [`Size`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L6) | `TheOne.Avalonia.Styling.ControlSize?` | 可写; CLR / init-only | `null` | 对话框尺寸枚举；采用库提供的尺寸档位。 可选值：`Small`, `Normal`, `Large` |
| [`Title`](../../../TheOne.Avalonia/Controls/DialogOptions.cs#L8) | `string?` | 可写; CLR / init-only | `null` | 显示标题；不等于业务主键或本地化资源键。 |

### OneDialog

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialog.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L123) | `Avalonia.Controls.Window?` | 可写; CLR / n/a | `accessor-defined / state` | `child.Show(parent)` / `child.ShowDialog<T>(parent)` 建立的原生窗口所属关系，影响平台窗口生命周期。 |

### OneDialogPresenter

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialogPresenter.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Buttons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L12) | `TheOne.Avalonia.Controls.OneDialogButtons` | 可写; OneWay | `0 / default` | 对话框按钮配置集合／标志；由选项类型和关闭结果契约解释。 可选值：`Ok`, `OkCancel`, `YesNo`, `YesNoCancel` |
| [`CloseOnEscape`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L20) | `bool` | 可写; OneWay | `true` | 允许 Esc 关闭对话框；结果与取消语义按对话框选项处理。 |
| [`ShowButtons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L18) | `bool` | 可写; OneWay | `true` | 显示默认按钮区；自定义内容中的按钮不受此开关控制。 |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L16) | `bool` | 可写; OneWay | `true` | 显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`ShowTitle`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L14) | `bool` | 可写; OneWay | `true` | 显示对话框标题区；标题值仍保留在选项中。 |

### OneNotification

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.OneNotification.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L86) | `Avalonia.Controls.Window?` | 可写; CLR / n/a | `accessor-defined / state` | `child.Show(parent)` / `child.ShowDialog<T>(parent)` 建立的原生窗口所属关系，影响平台窗口生命周期。 |

### OneToast

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.OneToast.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L103) | `Avalonia.Controls.Window?` | 可写; CLR / n/a | `accessor-defined / state` | `child.Show(parent)` / `child.ShowDialog<T>(parent)` 建立的原生窗口所属关系，影响平台窗口生命周期。 |

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

### ThemeButton

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButton.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Scope`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L16) | `TheOne.Avalonia.Controls.ThemeButtonScope` | 可写; OneWay | `0 / default` | 主题按钮切换的作用域；影响 Light/Dark，不切换 ThemeColor 颜色枚举。 可选值：`Window`, `Application` |

### 公共附加属性

[ControlSizing](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性; 向后代继承 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

[Motion](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| OneDialogPresenter | `one-large` | 模板自动类；不手动绑定 | one&#124;OneDialogPresenter.one-large => FontSize=16; Padding=32 |
| OneDialogPresenter | `one-normal` | 模板自动类；不手动绑定 | one&#124;OneDialogPresenter.one-normal => FontSize=14; Padding=24 |
| OneDialogPresenter | `one-small` | 模板自动类；不手动绑定 | one&#124;OneDialogPresenter.one-small => FontSize=12; Padding=16 |
| OneWindow | `one-large` | 模板自动类；不手动绑定 | one&#124;OneWindow.one-large => TitleBarHeight=56; FontSize=16 |
| OneWindow | `one-normal` | 模板自动类；不手动绑定 | one&#124;OneWindow.one-normal => TitleBarHeight=48; FontSize=14 |
| OneWindow | `one-small` | 模板自动类；不手动绑定 | one&#124;OneWindow.one-small => TitleBarHeight=42; FontSize=12 |
| ThemeButton | `one-large` | 模板自动类；不手动绑定 | one&#124;ThemeButton.one-large => Width=44; Height=44 |
| ThemeButton | `one-normal` | 模板自动类；不手动绑定 | one&#124;ThemeButton.one-normal => Width=36; Height=36 |
| ThemeButton | `one-small` | 模板自动类；不手动绑定 | one&#124;ThemeButton.one-small => Width=30; Height=30 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DialogContext`](../../api/zh-cn/TheOne.Avalonia.Controls.DialogContext.md) | Class | 0 | 1 | 2 |
| [`TheOne.Avalonia.Controls.DialogOptions`](../../api/zh-cn/TheOne.Avalonia.Controls.DialogOptions.md) | Class | 9 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneDialog`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialog.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogButtons`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialogButtons.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogPresenter`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialogPresenter.md) | Class | 5 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogResult`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialogResult.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneNotification`](../../api/zh-cn/TheOne.Avalonia.Controls.OneNotification.md) | Class | 1 | 4 | 0 |
| [`TheOne.Avalonia.Controls.OneToast`](../../api/zh-cn/TheOne.Avalonia.Controls.OneToast.md) | Class | 1 | 4 | 0 |
| [`TheOne.Avalonia.Controls.OneWindow`](../../api/zh-cn/TheOne.Avalonia.Controls.OneWindow.md) | Class | 10 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeButton`](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButton.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButtonScope.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Styling.ControlSize`](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSize.md) | Enum | 3 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
