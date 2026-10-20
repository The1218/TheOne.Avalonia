# Form

[English](../en/TheOne.Avalonia.Controls.Form.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Form.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Form` · Class · `Avalonia.Controls.ContentControl`

Validates caller-owned, compiled-bound fields using the same validators as VerificationStep.

[Source / 声明](../../../TheOne.Avalonia/Controls/Form.cs#L15)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

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

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void Form.ClearValidation()`](../../../TheOne.Avalonia/Controls/Form.cs#L234) | Cancels pending validation and clears form messages, retaining model values and native input drafts. |
| [`Task<bool> Form.ValidateAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/Form.cs#L275) | Runs attribute/custom validation and native conversion validation. Invalid, busy, detached or cancelled calls return false. |
| [`Task<bool> Form.SubmitAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/Form.cs#L328) | Validates before executing the host command, checking CanExecute again after asynchronous validation. |
| [`bool Form.FocusFirstError()`](../../../TheOne.Avalonia/Controls/Form.cs#L348) | 滚动并聚焦首个可见、可用的错误输入；无目标返回 `false` |
| [`Form.Form()`](../../../TheOne.Avalonia/Controls/Form.cs#L15) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<RoutedEventArgs>? Form.ValidationFailed`](../../../TheOne.Avalonia/Controls/Form.cs#L128) | 冒泡事件；模型、转换或校验器异常导致失败时触发，取消不触发 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IStepValidator?> Form.ValidatorProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> Form.ValidationModelProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ICommand?> Form.SubmitCommandProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> Form.SubmitCommandParameterProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> Form.ShowErrorSummaryProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Orientation> Form.LabelOrientationProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> Form.LabelWidthProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Form, IReadOnlyList<StepValidationError>> Form.ValidationErrorsProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Form, bool> Form.HasErrorsProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L41) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Form, IReadOnlyList<StepValidationError>> Form.UnmappedErrorsProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L44) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Form, bool> Form.HasUnmappedErrorsProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L48) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Form, bool> Form.IsBusyProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L51) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Form, bool> Form.IsValidatedProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L54) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Form, bool> Form.CanSubmitProperty`](../../../TheOne.Avalonia/Controls/Form.cs#L57) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<RoutedEventArgs> Form.ValidationFailedEvent`](../../../TheOne.Avalonia/Controls/Form.cs#L60) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.IStepValidator`](TheOne.Avalonia.Controls.IStepValidator.md)
- [`TheOne.Avalonia.Controls.StepCompareAttribute`](TheOne.Avalonia.Controls.StepCompareAttribute.md)
- [`TheOne.Avalonia.Controls.StepValidation`](TheOne.Avalonia.Controls.StepValidation.md)
- [`TheOne.Avalonia.Controls.StepValidationError`](TheOne.Avalonia.Controls.StepValidationError.md)
