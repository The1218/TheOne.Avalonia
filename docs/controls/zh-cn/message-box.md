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
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 的“控件能力实验室”（各页右上入口）按样式、属性、API／事件和模型分类演示；复杂模型使用原业务示例。

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

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.DialogCloseRequestedEventArgs.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DialogContext`](../../api/zh-cn/TheOne.Avalonia.Controls.DialogContext.md) | Class | 0 | 1 | 2 |
| [`TheOne.Avalonia.Controls.DialogOptions`](../../api/zh-cn/TheOne.Avalonia.Controls.DialogOptions.md) | Class | 9 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneDialog`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialog.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogButtons`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialogButtons.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.OneDialogResult`](../../api/zh-cn/TheOne.Avalonia.Controls.OneDialogResult.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Styling.ControlSize`](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSize.md) | Enum | 3 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
