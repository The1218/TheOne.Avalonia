# StepItem

[English](../en/TheOne.Avalonia.Controls.StepItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.StepItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.StepItem` · Class · `object`

Stable step identity; content is a model resolved lazily by the VerificationStep's data templates.

[Source / 声明](../../../TheOne.Avalonia/Controls/StepItem.cs#L17)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Content`](../../../TheOne.Avalonia/Controls/StepItem.cs#L54) | `object?` | 可写; CLR / n/a | `null` | 宿主内容或业务展示对象；具体模板作用域见内容模板契约，不把视图对象当作持久化数据。 |
| [`Description`](../../../TheOne.Avalonia/Controls/StepItem.cs#L48) | `string` | 可写; CLR / n/a | `""` | 描述文本；由宿主提供业务说明，未提供时沿用初值。 |
| [`HasNext`](../../../TheOne.Avalonia/Controls/StepItem.cs#L32) | `bool` | 只读; CLR / n/a | `false` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasPrevious`](../../../TheOne.Avalonia/Controls/StepItem.cs#L26) | `bool` | 只读; CLR / n/a | `false` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Indicator`](../../../TheOne.Avalonia/Controls/StepItem.cs#L98) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IndicatorVariant`](../../../TheOne.Avalonia/Controls/StepItem.cs#L40) | `TheOne.Avalonia.Controls.TagVariant` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 可选值：`Soft`, `Outline`, `Solid` |
| [`IsReached`](../../../TheOne.Avalonia/Controls/StepItem.cs#L38) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsSuccess`](../../../TheOne.Avalonia/Controls/StepItem.cs#L39) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Number`](../../../TheOne.Avalonia/Controls/StepItem.cs#L75) | `int` | 只读; CLR / n/a | `0 / default` | 分页结果中的页号；用于页码绘制。 |
| [`State`](../../../TheOne.Avalonia/Controls/StepItem.cs#L84) | `TheOne.Avalonia.Controls.StepState` | 只读; CLR / n/a | `0 / default` | 模型的状态枚举；可选值及持久化含义见对应枚举和所属控件契约。 可选值：`Waiting`, `Current`, `Success`, `Error` |
| [`Status`](../../../TheOne.Avalonia/Controls/StepItem.cs#L103) | `TheOne.Avalonia.Controls.TagStatus` | 只读; CLR / n/a | `accessor-defined / state` | 节点业务状态枚举；样式由状态派生，不把样式类作为状态存储。 可选值：`Default`, `Information`, `Success`, `Warning`, `Error`, `Processing` |
| [`Title`](../../../TheOne.Avalonia/Controls/StepItem.cs#L42) | `string` | 可写; CLR / n/a | `""` | 显示标题；不等于业务主键或本地化资源键。 |
| [`Validator`](../../../TheOne.Avalonia/Controls/StepItem.cs#L66) | `TheOne.Avalonia.Controls.IStepValidator?` | 可写; CLR / n/a | `null` | 步骤使用的校验器实例；返回错误关联成员名，复用 VerificationStep 校验契约。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`StepItem.StepItem()`](../../../TheOne.Avalonia/Controls/StepItem.cs#L17) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event PropertyChangedEventHandler? StepItem.PropertyChanged`](../../../TheOne.Avalonia/Controls/StepItem.cs#L109) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.IStepValidator`](TheOne.Avalonia.Controls.IStepValidator.md)
- [`TheOne.Avalonia.Controls.StepState`](TheOne.Avalonia.Controls.StepState.md)
- [`TheOne.Avalonia.Controls.TagStatus`](TheOne.Avalonia.Controls.TagStatus.md)
- [`TheOne.Avalonia.Controls.TagVariant`](TheOne.Avalonia.Controls.TagVariant.md)
