# Form 与 FormItem

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

```sh
dotnet build design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/form/artifacts
dotnet .preview/form/artifacts/bin/PreviewHost/debug/PreviewHost.dll --verify-form .preview/form/screenshots
```

专项仅验证 Form / FormItem：编译绑定、属性与转换错误、标签关联、真实按键／指针、取消与过期结果、命令可用性、嵌套归属、模板替换、重新挂载，以及明暗 × 三档尺寸 × 宽窄视口绘制和像素。共享字段改动另跑 `--verify-verification-step`，不跑 Tag 或全库回归。

`design/PreviewHost/Consumers/NativeAotSmoke` 复用同一专项。绑定／元数据变化时发布并执行原生程序的 `--require-aot --verify-form`。托管专项与跨 RID 构建不代表目标平台原生运行或读屏器实测通过。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节只列出本项目增加的 Classes、属性、扩展 API／事件及关联模型，不重复 Avalonia 原生继承 API。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用可见示例对照本项目附加样式，用 Description 组件说明专属属性、API 签名与模型字段；ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### Form

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.Form.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanSubmit`](../../../TheOne.Avalonia/Controls/Form.cs#L56) | `bool` | 只读; OneWay | `computed / state` | 只读，允许尝试提交，不表示模型已经有效 |
| [`HasErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L51) | `bool` | 只读; OneWay | `computed / state` | 只读，最近一次校验的去重错误与是否失败 |
| [`HasUnmappedErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L53) | `bool` | 只读; OneWay | `computed / state` | 只读，无对应字段的错误及其状态；用于顶部摘要，不重复字段提示 |
| [`IsBusy`](../../../TheOne.Avalonia/Controls/Form.cs#L54) | `bool` | 只读; OneWay | `computed / state` | 只读，正在校验；内容暂时禁用，重复校验或提交返回 `false` |
| [`IsValidated`](../../../TheOne.Avalonia/Controls/Form.cs#L55) | `bool` | 只读; OneWay | `computed / state` | 只读，最近校验成功；相关模型通知、清除、卸载或替换内容／校验器后复位 |
| [`LabelOrientation`](../../../TheOne.Avalonia/Controls/Form.cs#L48) | `Avalonia.Layout.Orientation` | 可写; OneWay; 向后代继承 | `Orientation.Vertical` | 默认 `Vertical` / `120`，向字段继承；宽度必须为正有限数 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/Form.cs#L49) | `double` | 可写; OneWay; 向后代继承 | `120` | 默认 `Vertical` / `120`，向字段继承；宽度必须为正有限数 校验：`value => double.IsFinite(value) && value > 0` |
| [`ShowErrorSummary`](../../../TheOne.Avalonia/Controls/Form.cs#L47) | `bool` | 可写; OneWay | `true` | 默认 `true`，仅显示模型级和无对应字段的错误；已映射错误只在字段下方显示 |
| [`SubmitCommand`](../../../TheOne.Avalonia/Controls/Form.cs#L45) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | 保存命令与参数；异步校验前后均检查 `CanExecute` |
| [`SubmitCommandParameter`](../../../TheOne.Avalonia/Controls/Form.cs#L46) | `object?` | 可写; OneWay | `null` | 保存命令与参数；异步校验前后均检查 `CanExecute` |
| [`UnmappedErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L52) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | 只读; OneWay | `computed / state` | 只读，无对应字段的错误及其状态；用于顶部摘要，不重复字段提示 |
| [`ValidationErrors`](../../../TheOne.Avalonia/Controls/Form.cs#L50) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | 只读; OneWay | `computed / state` | 只读，最近一次校验的去重错误与是否失败 |
| [`ValidationModel`](../../../TheOne.Avalonia/Controls/Form.cs#L44) | `object?` | 可写; OneWay | `null` | 自定义校验器的可选通知来源；属性校验器优先使用其注册模型 |
| [`Validator`](../../../TheOne.Avalonia/Controls/Form.cs#L42) | `TheOne.Avalonia.Controls.IStepValidator?` | 可写; OneWay | `null` | `IStepValidator?`；可复用步骤校验器，或用 `StepValidation.For(model, memberNames)` 限定字段 |

### FormItem

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.FormItem.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`EffectiveLabelWidth`](../../../TheOne.Avalonia/Controls/FormItem.cs#L37) | `double` | 只读; OneWay | `computed / state` | 只读模板状态：区域可见性、标签停靠位置和实际标签宽度 |
| [`HasHelpText`](../../../TheOne.Avalonia/Controls/FormItem.cs#L39) | `bool` | 只读; OneWay | `computed / state` | 只读模板状态：区域可见性、标签停靠位置和实际标签宽度 |
| [`HasLabel`](../../../TheOne.Avalonia/Controls/FormItem.cs#L38) | `bool` | 只读; OneWay | `computed / state` | 只读模板状态：区域可见性、标签停靠位置和实际标签宽度 |
| [`HelpText`](../../../TheOne.Avalonia/Controls/FormItem.cs#L31) | `string?` | 可写; OneWay | `null` | 标签及输入下方帮助文本，空值隐藏对应区域 |
| [`IsRequired`](../../../TheOne.Avalonia/Controls/FormItem.cs#L33) | `bool` | 可写; OneWay | `false` | 默认 `false`，仅展示标记；实际必填规则由校验器提供 |
| [`Label`](../../../TheOne.Avalonia/Controls/FormItem.cs#L30) | `string?` | 可写; OneWay | `null` | 标签及输入下方帮助文本，空值隐藏对应区域 |
| [`LabelDock`](../../../TheOne.Avalonia/Controls/FormItem.cs#L36) | `Avalonia.Controls.Dock` | 只读; OneWay | `computed / state` | 只读模板状态：区域可见性、标签停靠位置和实际标签宽度 可选值：`Left`, `Bottom`, `Right`, `Top` |
| [`LabelOrientation`](../../../TheOne.Avalonia/Controls/FormItem.cs#L34) | `Avalonia.Layout.Orientation` | 可写; OneWay; 向后代继承 | `Orientation.Vertical` | 默认 `Vertical` / `120`，向字段继承；宽度必须为正有限数 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/FormItem.cs#L35) | `double` | 可写; OneWay; 向后代继承 | `120` | 默认 `Vertical` / `120`，向字段继承；宽度必须为正有限数 校验：`value => double.IsFinite(value) && value > 0` |

### IStepValidator

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.IStepValidator.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### StepCompareAttribute

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StepCompareAttribute.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`OtherProperty`](../../../TheOne.Avalonia/Controls/StepCompareAttribute.cs#L10) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### StepValidation

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidation.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### StepValidationError

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidationError.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`MemberName`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | 继承自 `StepField`，匹配错误中的属性名，如 `Name`；不会作为反射路径执行 |
| [`Message`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | 可供用户或诊断展示的结果说明；操作结果仍以状态字段为准。 |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |

`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出附加外观类的控件使用默认主题及其专属属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.Form`](../../api/zh-cn/TheOne.Avalonia.Controls.Form.md) | Class | 14 | 5 | 1 |
| [`TheOne.Avalonia.Controls.FormItem`](../../api/zh-cn/TheOne.Avalonia.Controls.FormItem.md) | Class | 9 | 1 | 0 |
| [`TheOne.Avalonia.Controls.IStepValidator`](../../api/zh-cn/TheOne.Avalonia.Controls.IStepValidator.md) | Interface | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepCompareAttribute`](../../api/zh-cn/TheOne.Avalonia.Controls.StepCompareAttribute.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepValidation`](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidation.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.StepValidationError`](../../api/zh-cn/TheOne.Avalonia.Controls.StepValidationError.md) | Class | 2 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
