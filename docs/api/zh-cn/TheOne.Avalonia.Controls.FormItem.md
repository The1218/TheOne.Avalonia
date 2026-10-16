# FormItem

[English](../en/TheOne.Avalonia.Controls.FormItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.FormItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.FormItem` · Class · `TheOne.Avalonia.Controls.StepField`

[继承的额外成员: StepField](TheOne.Avalonia.Controls.StepField.md)

A labelled field with caller-owned native input and shared step validation messages.

[Source / 声明](../../../TheOne.Avalonia/Controls/FormItem.cs#L13)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

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

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`FormItem.FormItem()`](../../../TheOne.Avalonia/Controls/FormItem.cs#L13) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string?> FormItem.LabelProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> FormItem.HelpTextProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> FormItem.IsRequiredProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Orientation> FormItem.LabelOrientationProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> FormItem.LabelWidthProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<FormItem, Dock> FormItem.LabelDockProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<FormItem, double> FormItem.EffectiveLabelWidthProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<FormItem, bool> FormItem.HasLabelProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<FormItem, bool> FormItem.HasHelpTextProperty`](../../../TheOne.Avalonia/Controls/FormItem.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
