# ControlSizing

[English](../en/TheOne.Avalonia.Styling.ControlSizing.md) · [简体中文](../zh-cn/TheOne.Avalonia.Styling.ControlSizing.md) · [API index](README.md)

`TheOne.Avalonia.Styling.ControlSizing` · Class · `Avalonia.AvaloniaObject`

Inheritable density: set on an individual control or on a whole form.

[Source / 声明](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L14)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L16) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static ControlSize ControlSizing.GetSize(Control control)`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L29) | 读取附加属性。Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 |
| [`static void ControlSizing.SetSize(Control control, ControlSize value)`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L34) | 设置附加属性。Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 |
| [`ControlSizing.ControlSizing()`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L14) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<ControlSize> ControlSizing.SizeProperty`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Styling.ControlSize`](TheOne.Avalonia.Styling.ControlSize.md)
