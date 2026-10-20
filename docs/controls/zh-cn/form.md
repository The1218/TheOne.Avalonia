# Form 与 FormItem

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

`Form` 为普通编辑页提供统一校验与提交入口，复用 `VerificationStep` 的 `IStepValidator`、`StepValidation.For<T>`、`StepValidationError` 和 `StepCompareAttribute`。`FormItem` 为调用方的原生输入提供标签、必填标记、帮助文本与字段错误。加载 `OneTheme` 后即可使用，无需额外包或主题。

## 编译绑定与接入

在 ViewModel 中为具体模型类型创建校验器：

```csharp
public AccountModel Account { get; } = new();
public IStepValidator Validator { get; }
public ICommand SaveCommand { get; }

public EditViewModel()
{
    Validator = StepValidation.For(Account);
    SaveCommand = ReactiveCommand.Create(Save);
}
```

模型属性可使用 `Required`、`Range`、`StringLength`、`EmailAddress` 等特性。跨字段比较用 `StepCompare`；也可实现 `IValidatableObject` 返回字段或模型级错误。模型应实现 `INotifyPropertyChanged` 并在 UI 线程通知。`ReactiveCommand` 只是示例，消费者可以使用其他 MVVM 框架。

```xml
<one:Form x:Name="Editor" Validator="{Binding Validator}"
          SubmitCommand="{Binding SaveCommand}"
          LabelOrientation="Horizontal" LabelWidth="112">
  <StackPanel Spacing="16">
    <one:FormItem MemberName="Name" Label="姓名" IsRequired="True"
                  HelpText="请输入 2–24 个字符。">
      <TextBox Text="{Binding Account.Name, Mode=TwoWay}" />
    </one:FormItem>
    <one:FormItem MemberName="Email" Label="邮箱" IsRequired="True">
      <TextBox Text="{Binding Account.Email, Mode=TwoWay}" />
    </one:FormItem>
    <Button Content="校验并保存" Click="SaveClicked"
            IsEnabled="{Binding #Editor.CanSubmit}" />
  </StackPanel>
</one:Form>
```

视图声明 `xmlns:one="using:TheOne.Avalonia.Controls"` 和具体 `x:DataType`，保持编译绑定。按钮只调用表单入口：

```csharp
private async void SaveClicked(object? sender, RoutedEventArgs args)
    => await Editor.SubmitAsync();
```

不要同时把按钮的 `Command` 直接绑定到保存命令，以免绕过校验。需要自有提交工作流时，先等待 `Editor.ValidateAsync()` 返回 `true` 再保存。

## Form 契约

| 属性或方法 | 行为 |
| --- | --- |
| `Content` / `ContentTemplate` | 调用方布局或编译数据模板，保留原生输入及自定义控件 |
| `Validator` | `IStepValidator?`；可复用步骤校验器，或用 `StepValidation.For(model, memberNames)` 限定字段 |
| `ValidationModel` | 自定义校验器的可选通知来源；属性校验器优先使用其注册模型 |
| `LabelOrientation` / `LabelWidth` | 默认 `Vertical` / `120`，向字段继承；宽度必须为正有限数 |
| `ShowErrorSummary` | 默认 `true`，仅显示模型级和无对应字段的错误；已映射错误只在字段下方显示 |
| `SubmitCommand` / `SubmitCommandParameter` | 保存命令与参数；异步校验前后均检查 `CanExecute` |
| `ValidationErrors` / `HasErrors` | 只读，最近一次校验的去重错误与是否失败 |
| `UnmappedErrors` / `HasUnmappedErrors` | 只读，无对应字段的错误及其状态；用于顶部摘要，不重复字段提示 |
| `IsBusy` | 只读，正在校验；内容暂时禁用，重复校验或提交返回 `false` |
| `IsValidated` | 只读，最近校验成功；相关模型通知、清除、卸载或替换内容／校验器后复位 |
| `CanSubmit` | 只读，允许尝试提交，不表示模型已经有效 |
| `ValidateAsync(CancellationToken)` | 校验模型规则与原生输入转换错误；失败、取消、未挂载、禁用或忙碌时返回 `false` |
| `SubmitAsync(CancellationToken)` | 校验通过且命令可执行时调用命令；不自动拦截 Enter |
| `ClearValidation()` | 取消校验并清除表单提示，保留模型值和输入草稿 |
| `FocusFirstError()` | 滚动并聚焦首个可见、可用的错误输入；无目标返回 `false` |
| `ValidationFailed` | 冒泡事件；模型、转换或校验器异常导致失败时触发，取消不触发 |

`SubmitCommand` 为 `null` 时只校验。宿主命令负责异步保存、业务异常及防重复保存；`IsBusy` 只覆盖校验阶段。命令执行异常向调用方传播。

## FormItem 与布局

| 属性 | 行为 |
| --- | --- |
| `MemberName` | 继承自 `StepField`，匹配错误中的属性名，如 `Name`；不会作为反射路径执行 |
| `Label` / `HelpText` | 标签及输入下方帮助文本，空值隐藏对应区域 |
| `IsRequired` | 默认 `false`，仅展示标记；实际必填规则由校验器提供 |
| `LabelOrientation` | `Vertical` 在上方；`Horizontal` 在左侧；支持逐项覆盖继承值 |
| `LabelWidth` | 横向标签区域宽度，包含必填标记；纵向按内容布局 |
| `Errors` / `HasErrors` | 继承的只读字段错误；也可在 `VerificationStep` 自定义内容中使用 |
| `HasLabel` / `HasHelpText` / `LabelDock` / `EffectiveLabelWidth` | 只读模板状态：区域可见性、标签停靠位置和实际标签宽度 |

标签关联首个可聚焦输入，保留已有的 `AutomationProperties.LabeledBy`。一个字段有多个输入时，宿主应为各输入提供可访问名称或标签。原生输入的键盘、焦点、`IsReadOnly`、`IsEnabled` 保持原行为。

字段间距、滚动和响应式切换由宿主组合；普通表单用 `StackPanel`，长表单按需使用有界 `Grid` / `ScrollViewer`。窄窗口可切换为纵向标签。明暗颜色复用 `One.*` 资源，输入继承 Small / Normal / Large 密度。

## 校验范围与生命周期

- 属性校验器使用具体泛型类型保留所需公共属性元数据，不以 `object` 或基类隐藏实际类型，不关闭编译绑定。
- 自定义校验器实现 `IStepValidator.ValidateAsync`，返回 `IReadOnlyList<StepValidationError>` 并响应取消令牌；`MemberName` 为空表示模型级错误。
- 相关模型通知清除旧错误并取消校验，`StepCompare` 依赖也参与失效判断。自定义校验器设置实现 `INotifyPropertyChanged` 的 `ValidationModel` 可获得同样的通知行为。
- 更换校验器、通知模型、内容或内容模板以及卸载时取消旧校验；即使服务忽略取消，过期结果也不能覆盖新状态。重新挂载恢复订阅。
- 不发通知的模型修改需由宿主调用 `ClearValidation()` 并重新校验；表单不推断任意 `DataContext` 的运行时类型。
- 转换失败保留最后有效模型值及原生输入草稿。清除提示或隐藏摘要不能绕过转换错误。若宿主虚拟化或移除显式字段，草稿由宿主保存；表单不持有卸载后的编辑器。
- 嵌套表单与步骤按最近校验宿主管理字段；外层不自动校验内层，也不覆盖内层错误。整体提交由宿主显式组合。

首版采用显式字段和模板，不提供独立自动表单生成器；现有 `StepItem<T>` 自动表单保持原行为，见 [VerificationStep](tag-verification-step.md)。

## 预览与专项

PreviewHost：**扩展控件 → Form**。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### Form

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.Form.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanSubmit`](../../../TheOne.Avalonia/Controls/Form.cs#L125) | `bool` | 只读; OneWay | `computed / state` | 只读，允许尝试提交，不表示模型已经有效 |
| [`HasErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L120) | `bool` | 只读; OneWay | `computed / state` | 只读，最近一次校验的去重错误与是否失败 |
| [`HasUnmappedErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L122) | `bool` | 只读; OneWay | `computed / state` | 只读，无对应字段的错误及其状态；用于顶部摘要，不重复字段提示 |
| [`IsBusy`](../../../TheOne.Avalonia/Controls/Form.cs#L123) | `bool` | 只读; OneWay | `computed / state` | 只读，正在校验；内容暂时禁用，重复校验或提交返回 `false` |
| [`IsValidated`](../../../TheOne.Avalonia/Controls/Form.cs#L124) | `bool` | 只读; OneWay | `computed / state` | 只读，最近校验成功；相关模型通知、清除、卸载或替换内容／校验器后复位 |
| [`LabelOrientation`](../../../TheOne.Avalonia/Controls/Form.cs#L107) | `Avalonia.Layout.Orientation` | 可写; OneWay | `Orientation.Vertical` | 默认 `Vertical` / `120`，向字段继承；宽度必须为正有限数 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/Form.cs#L113) | `double` | 可写; OneWay | `120` | 默认 `Vertical` / `120`，向字段继承；宽度必须为正有限数 校验：`value => double.IsFinite(value) && value > 0` |
| [`ShowErrorSummary`](../../../TheOne.Avalonia/Controls/Form.cs#L101) | `bool` | 可写; OneWay | `true` | 默认 `true`，仅显示模型级和无对应字段的错误；已映射错误只在字段下方显示 |
| [`SubmitCommand`](../../../TheOne.Avalonia/Controls/Form.cs#L89) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | 保存命令与参数；异步校验前后均检查 `CanExecute` |
| [`SubmitCommandParameter`](../../../TheOne.Avalonia/Controls/Form.cs#L95) | `object?` | 可写; OneWay | `null` | 保存命令与参数；异步校验前后均检查 `CanExecute` |
| [`UnmappedErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L121) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | 只读; OneWay | `computed / state` | 只读，无对应字段的错误及其状态；用于顶部摘要，不重复字段提示 |
| [`ValidationErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L119) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | 只读; OneWay | `computed / state` | 只读，最近一次校验的去重错误与是否失败 |
| [`ValidationModel`](../../../TheOne.Avalonia/Controls/Form.cs#L83) | `object?` | 可写; OneWay | `null` | 自定义校验器的可选通知来源；属性校验器优先使用其注册模型 |
| [`Validator`](../../../TheOne.Avalonia/Controls/Form.cs#L73) | `TheOne.Avalonia.Controls.IStepValidator?` | 可写; OneWay | `null` | `IStepValidator?`；可复用步骤校验器，或用 `StepValidation.For(model, memberNames)` 限定字段 |

### FormItem

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.FormItem.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`EffectiveLabelWidth`](../../../TheOne.Avalonia/Controls/FormItem.cs#L84) | `double` | 只读; OneWay | `computed / state` | 只读模板状态：区域可见性、标签停靠位置和实际标签宽度 |
| [`HasHelpText`](../../../TheOne.Avalonia/Controls/FormItem.cs#L86) | `bool` | 只读; OneWay | `computed / state` | 只读模板状态：区域可见性、标签停靠位置和实际标签宽度 |
| [`HasLabel`](../../../TheOne.Avalonia/Controls/FormItem.cs#L85) | `bool` | 只读; OneWay | `computed / state` | 只读模板状态：区域可见性、标签停靠位置和实际标签宽度 |
| [`HelpText`](../../../TheOne.Avalonia/Controls/FormItem.cs#L58) | `string?` | 可写; OneWay | `null` | 标签及输入下方帮助文本，空值隐藏对应区域 |
| [`IsRequired`](../../../TheOne.Avalonia/Controls/FormItem.cs#L65) | `bool` | 可写; OneWay | `false` | 默认 `false`，仅展示标记；实际必填规则由校验器提供 |
| [`Label`](../../../TheOne.Avalonia/Controls/FormItem.cs#L52) | `string?` | 可写; OneWay | `null` | 标签及输入下方帮助文本，空值隐藏对应区域 |
| [`LabelDock`](../../../TheOne.Avalonia/Controls/FormItem.cs#L83) | `Avalonia.Controls.Dock` | 只读; OneWay | `computed / state` | 只读模板状态：区域可见性、标签停靠位置和实际标签宽度 可选值：`Left`, `Bottom`, `Right`, `Top` |
| [`LabelOrientation`](../../../TheOne.Avalonia/Controls/FormItem.cs#L71) | `Avalonia.Layout.Orientation` | 可写; OneWay | `Orientation.Vertical` | 默认 `Vertical` / `120`，向字段继承；宽度必须为正有限数 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/FormItem.cs#L77) | `double` | 可写; OneWay | `120` | 默认 `Vertical` / `120`，向字段继承；宽度必须为正有限数 校验：`value => double.IsFinite(value) && value > 0` |

### IStepValidator

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.IStepValidator.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### StepCompareAttribute

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.StepCompareAttribute.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`OtherProperty`](../../../TheOne.Avalonia/Controls/StepCompareAttribute.cs#L17) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### StepValidation

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidation.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### StepValidationError

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidationError.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`MemberName`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | 继承自 `StepField`，匹配错误中的属性名，如 `Name`；不会作为反射路径执行 |
| [`Message`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | 可供用户或诊断展示的结果说明；操作结果仍以状态字段为准。 |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`Form`](../../api/zh-cn/TheOne.Avalonia.Controls.Form.md) | `void Form.ClearValidation()` | Cancels pending validation and clears form messages, retaining model values and native input drafts. |
| [`Form`](../../api/zh-cn/TheOne.Avalonia.Controls.Form.md) | `Task<bool> Form.ValidateAsync(CancellationToken cancellationToken = default(CancellationToken))` | Runs attribute/custom validation and native conversion validation. Invalid, busy, detached or cancelled calls return false. |
| [`Form`](../../api/zh-cn/TheOne.Avalonia.Controls.Form.md) | `Task<bool> Form.SubmitAsync(CancellationToken cancellationToken = default(CancellationToken))` | Validates before executing the host command, checking CanExecute again after asynchronous validation. |
| [`Form`](../../api/zh-cn/TheOne.Avalonia.Controls.Form.md) | `bool Form.FocusFirstError()` | 滚动并聚焦首个可见、可用的错误输入；无目标返回 `false` |
| [`IStepValidator`](../../api/zh-cn/TheOne.Avalonia.Controls.IStepValidator.md) | `Task<IReadOnlyList<StepValidationError>> IStepValidator.ValidateAsync(CancellationToken cancellationToken)` | 校验模型规则与原生输入转换错误；失败、取消、未挂载、禁用或忙碌时返回 `false` |
| [`StepValidation`](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidation.md) | `static IStepValidator StepValidation.For<T>(T model, params string[] memberNames) where T : class` | 为指定模型／属性集合创建类型化步骤校验器。 |

### 事件订阅

按需订阅下列通知，并在宿主释放时解除订阅；这里只列事件入口，不列路由事件标识字段。

| 类型 | 事件 | 用途 |
| --- | --- | --- |
| [`Form`](../../api/zh-cn/TheOne.Avalonia.Controls.Form.md) | `ValidationFailed` | 冒泡事件；模型、转换或校验器异常导致失败时触发，取消不触发 |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.StepCompareAttribute`](../../api/zh-cn/TheOne.Avalonia.Controls.StepCompareAttribute.md) | 1 |
| [`TheOne.Avalonia.Controls.StepValidationError`](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidationError.md) | 2 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
