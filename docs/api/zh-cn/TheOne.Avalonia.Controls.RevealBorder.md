# RevealBorder

[English](../en/TheOne.Avalonia.Controls.RevealBorder.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RevealBorder.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RevealBorder` · Class · `Avalonia.Controls.Border`

Fades and translates content; keeps it laid out until the closing animation completes.

[Source / 声明](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L13)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Duration`](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L20) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(180)` | 持续时间；动效仍遵守继承的 Motion.Enabled，不能仅用时长替代关闭开关。 |
| [`IsOpen`](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L19) | `bool` | 可写; OneWay | `false` | 展开或显示内容；收起保留内容对象，过渡遵循 Motion.Enabled。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`RevealBorder.RevealBorder()`](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L22) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<bool> RevealBorder.IsOpenProperty`](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TimeSpan> RevealBorder.DurationProperty`](../../../TheOne.Avalonia/Controls/RevealBorder.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
