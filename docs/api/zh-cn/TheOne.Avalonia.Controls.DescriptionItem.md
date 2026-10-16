# DescriptionItem

[English](../en/TheOne.Avalonia.Controls.DescriptionItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DescriptionItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DescriptionItem` · Class · `Avalonia.Controls.Primitives.HeaderedContentControl`

A label and value row. Header and Content accept text, controls or typed data templates.

[Source / 声明](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L18) | `double` | 可写; OneWay; 向后代继承 | `180` | `double`，`180` ; 双列布局中名称列宽度，单位 DIP。可绑定、可通过样式设置，并继承至行；必须为有限非负值。 校验：`value => double.IsFinite(value) && value >= 0` |
| [`StackWidth`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L21) | `double` | 可写; OneWay; 向后代继承 | `520` | `double`，`520` ; 行可用宽度小于此值时，名称移到内容上方。`0` 保持双列；必须为有限非负值。 校验：`value => double.IsFinite(value) && value >= 0` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`DescriptionItem.DescriptionItem()`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L10) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<double> DescriptionItem.LabelWidthProperty`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> DescriptionItem.StackWidthProperty`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.Description`](TheOne.Avalonia.Controls.Description.md)
