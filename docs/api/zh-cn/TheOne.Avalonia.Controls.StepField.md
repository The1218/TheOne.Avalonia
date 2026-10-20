# StepField

[English](../en/TheOne.Avalonia.Controls.StepField.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.StepField.md) · [API index](README.md)

`TheOne.Avalonia.Controls.StepField` · Class · `Avalonia.Controls.ContentControl`

Places a step member's validation messages directly below its caller-owned input.

[Source / 声明](../../../TheOne.Avalonia/Controls/StepField.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Errors`](../../../TheOne.Avalonia/Controls/StepField.cs#L28) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.StepValidationError>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasErrors`](../../../TheOne.Avalonia/Controls/StepField.cs#L29) | `bool` | 只读; OneWay | `computed / state` | 全部验证错误 |
| [`MemberName`](../../../TheOne.Avalonia/Controls/StepField.cs#L22) | `string` | 可写; OneWay | `""` | 对应模型成员的准确名称；用于将校验错误映射到字段，不是显示标签。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`StepField.StepField()`](../../../TheOne.Avalonia/Controls/StepField.cs#L7) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string> StepField.MemberNameProperty`](../../../TheOne.Avalonia/Controls/StepField.cs#L9) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<StepField, IReadOnlyList<StepValidationError>> StepField.ErrorsProperty`](../../../TheOne.Avalonia/Controls/StepField.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<StepField, bool> StepField.HasErrorsProperty`](../../../TheOne.Avalonia/Controls/StepField.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.StepValidationError`](TheOne.Avalonia.Controls.StepValidationError.md)
