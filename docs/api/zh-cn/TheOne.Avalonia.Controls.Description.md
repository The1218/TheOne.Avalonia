# Description

[English](../en/TheOne.Avalonia.Controls.Description.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Description.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Description` · Class · `Avalonia.Controls.ItemsControl`

A naturally sized list of labelled values, with automatic stacking in narrow layouts.

[Source / 声明](../../../TheOne.Avalonia/Controls/Description.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/Description.cs#L18) | `double` | 可写; OneWay | `180` | `double`，`180` ; 双列布局中名称列宽度，单位 DIP。可绑定、可通过样式设置，并继承至行；必须为有限非负值。 校验：`value => double.IsFinite(value) && value >= 0` |
| [`StackWidth`](../../../TheOne.Avalonia/Controls/Description.cs#L25) | `double` | 可写; OneWay | `520` | `double`，`520` ; 行可用宽度小于此值时，名称移到内容上方。`0` 保持双列；必须为有限非负值。 校验：`value => double.IsFinite(value) && value >= 0` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Description.Description()`](../../../TheOne.Avalonia/Controls/Description.cs#L7) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<double> Description.LabelWidthProperty`](../../../TheOne.Avalonia/Controls/Description.cs#L9) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> Description.StackWidthProperty`](../../../TheOne.Avalonia/Controls/Description.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `borderless` | [`^.borderless`](../../../TheOne.Avalonia/Themes/Controls/Description.axaml#L33) | ^.borderless => BorderThickness=0; CornerRadius=0 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.DescriptionItem`](TheOne.Avalonia.Controls.DescriptionItem.md)
