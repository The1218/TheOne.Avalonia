# VerificationStep

[English](../en/TheOne.Avalonia.Controls.VerificationStep.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.VerificationStep.md) · [API index](README.md)

`TheOne.Avalonia.Controls.VerificationStep` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A sequential form with virtualized headers and validation before every forward transition.

[Source / 声明](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L18)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanGoBack`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L160) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanGoNext`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L161) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CompleteText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L143) | `string` | 可写; OneWay | `"完成"` | 默认“下一步”／“上一步”／“完成” |
| [`CompletedCommand`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L166) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`CompletedCommandParameter`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L172) | `object?` | 可写; OneWay | `null` | 校验通过并完成步骤时传给 CompletedCommand 的业务参数。 |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L119) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 文字、控件或有类型的数据模板 |
| [`CurrentContent`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L156) | `object?` | 只读; OneWay | `computed / state` | 只读当前位置、模型／内容、实际内容模板；不能设置索引绕过校验 |
| [`CurrentContentTemplate`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L157) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; OneWay | `computed / state` | 只读当前位置、模型／内容、实际内容模板；不能设置索引绕过校验 |
| [`ForwardText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L162) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L164) | `bool` | 只读; OneWay | `computed / state` | 全部验证错误 |
| [`HasUnmappedErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L179) | `bool` | 只读; OneWay | `computed / state` | 整步或没有对应输入承载位置的错误 |
| [`HeaderHorizontalAlignment`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L131) | `Avalonia.Layout.HorizontalAlignment` | 可写; OneWay | `HorizontalAlignment.Center` | 默认 Center，与内容同轴居中；Stretch 均分可用宽度；也支持 Left / Right 校验：`Enum.IsDefined` 可选值：`Stretch`, `Left`, `Center`, `Right` |
| [`HeaderMaxWidth`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L125) | `double` | 可写; OneWay | `960` | 标题视口宽度上限，默认 960，必须为有限正数 校验：`value => double.IsFinite(value) && value > 0` |
| [`IsBusy`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L158) | `bool` | 只读; OneWay | `computed / state` | 校验中与最终完成状态 |
| [`IsCompleted`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L159) | `bool` | 只读; OneWay | `computed / state` | 校验中与最终完成状态 |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L113) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Controls.StepItem>?` | 可写; OneWay | `null` | IEnumerable&lt;StepItem&gt;；默认使用 StepItem&lt;具体模型类型&gt;；集合变化使用 ObservableCollection，每项保持独立身份 |
| [`NextText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L137) | `string` | 可写; OneWay | `"下一步"` | 默认“下一步”／“上一步”／“完成” |
| [`PreviousText`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L149) | `string` | 可写; OneWay | `"上一步"` | 默认“下一步”／“上一步”／“完成” |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L155) | `int` | 只读; OneWay | `computed / state` | 只读当前位置、模型／内容、实际内容模板；不能设置索引绕过校验 |
| [`UnmappedErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L178) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | 只读; OneWay | `computed / state` | 整步或没有对应输入承载位置的错误 |
| [`ValidationErrors`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L163) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | 只读; OneWay | `computed / state` | 全部验证错误 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void VerificationStep.Reset()`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L356) | 重置进度与选择，不清空业务数据或尚未修正的输入草稿 |
| [`bool VerificationStep.GoBack()`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L362) | 可返回先前步骤；前进仅限相邻下一步并验证，不能跳步 |
| [`Task<bool> VerificationStep.GoToAsync(int index, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L377) | Backward navigation is free; forward navigation is limited to the immediate next step and validates the current one. |
| [`Task<bool> VerificationStep.GoNextAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L395) | Validates current bound model attributes and native input errors; returns false on invalid, busy or cancelled transitions. |
| [`VerificationStep.VerificationStep()`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L18) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<RoutedEventArgs>? VerificationStep.ValidationFailed`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L182) | 失败的冒泡事件；完成使用 CompletedCommand |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IEnumerable<StepItem>?> VerificationStep.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> VerificationStep.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> VerificationStep.HeaderMaxWidthProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<HorizontalAlignment> VerificationStep.HeaderHorizontalAlignmentProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> VerificationStep.NextTextProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> VerificationStep.CompleteTextProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> VerificationStep.PreviousTextProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L40) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, int> VerificationStep.SelectedIndexProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L43) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, IDataTemplate?> VerificationStep.CurrentContentTemplateProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L46) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, object?> VerificationStep.CurrentContentProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L50) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.IsBusyProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L54) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.IsCompletedProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L57) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.CanGoBackProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L60) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.CanGoNextProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L63) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, string> VerificationStep.ForwardTextProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L66) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, IReadOnlyList<StepValidationError>> VerificationStep.ValidationErrorsProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L70) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.HasErrorsProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L74) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ICommand?> VerificationStep.CompletedCommandProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L77) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> VerificationStep.CompletedCommandParameterProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L80) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, IReadOnlyList<StepValidationError>> VerificationStep.UnmappedErrorsProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L83) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<VerificationStep, bool> VerificationStep.HasUnmappedErrorsProperty`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L87) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<RoutedEventArgs> VerificationStep.ValidationFailedEvent`](../../../TheOne.Avalonia/Controls/VerificationStep.cs#L91) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.StepCompareAttribute`](TheOne.Avalonia.Controls.StepCompareAttribute.md)
- [`TheOne.Avalonia.Controls.StepItem`](TheOne.Avalonia.Controls.StepItem.md)
- [`TheOne.Avalonia.Controls.StepItem<T>`](TheOne.Avalonia.Controls.StepItem_T_.md)
- [`TheOne.Avalonia.Controls.StepState`](TheOne.Avalonia.Controls.StepState.md)
- [`TheOne.Avalonia.Controls.StepValidation`](TheOne.Avalonia.Controls.StepValidation.md)
- [`TheOne.Avalonia.Controls.StepValidationError`](TheOne.Avalonia.Controls.StepValidationError.md)
