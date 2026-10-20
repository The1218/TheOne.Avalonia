# MessageBox 确认框

`OneMessageBox` 只处理确认 / 取消，复用 `OneWindow`、`LoginWindow` 已有的窗口内对话框层，不创建额外原生窗口。表单和自定义内容继续使用 `OneDialog`。

```csharp
// 在 ViewModel 的异步 ICommand 中调用。
var confirmed = await OneMessageBox.ConfirmAsync(
    "删除这个文件？",
    "文件及其预览图将永久删除，删除后无法恢复。",
    new MessageBoxOptions
    {
        ConfirmText = "确认删除",
        CancelText = "保留文件",
        IsDangerous = true
    });
if (confirmed)
    await fileService.DeleteAsync(fileId);
```

只有执行确认命令返回 `true`；取消、Escape、关闭所属窗口均返回 `false`。初始焦点位于取消按钮，危险操作也保持这一默认行为。Tab 切换焦点，Enter 执行当前按钮。点击遮罩保持确认框打开。关闭后由现有对话框宿主恢复原先焦点。

`MessageBoxOptions` 提供 `ConfirmText`（默认“确认”）、`CancelText`（默认“取消”）、`IsDangerous`（默认 `false`）及可选的 `ControlSize? Size`。不设置 `Size` 时继承窗口的 Small / Normal / Large 尺寸。标题、正文及按钮使用 `One.*` 主题资源，支持 Light / Dark。标题、消息和按钮文案不能为空白。

可选 `owner` 参数指定已打开的 `OneWindow` 或 `LoginWindow`；省略时与 `OneDialog` 一样使用注册的默认窗口。确认框与其他对话框共用每个窗口的顺序队列。取消 `cancellationToken` 会取消任务并释放对话框，不把任务取消视为用户选择。

PreviewHost 的 MessageBox 页面通过 ViewModel 命令打开确认框，展示可配置确认和危险操作确认；等待布尔结果后才执行业务操作，全部使用编译绑定。

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
| [`TheOne.Avalonia.Styling.ControlSize`](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSize.md) | 3 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
