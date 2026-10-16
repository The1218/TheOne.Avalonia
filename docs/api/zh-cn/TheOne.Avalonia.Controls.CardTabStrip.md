# CardTabStrip

[English](../en/TheOne.Avalonia.Controls.CardTabStrip.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CardTabStrip.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CardTabStrip` · Class · `Avalonia.Controls.DockPanel`

Header scrolling and adjacent-tab separators for the card theme.

[Source / 声明](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L12)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanScrollBackward`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L26) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanScrollForward`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L27) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasOverflow`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L25) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`CardTabStrip.CardTabStrip()`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L30) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly DirectProperty<CardTabStrip, bool> CardTabStrip.HasOverflowProperty`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<CardTabStrip, bool> CardTabStrip.CanScrollBackwardProperty`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<CardTabStrip, bool> CardTabStrip.CanScrollForwardProperty`](../../../TheOne.Avalonia/Controls/CardTabStrip.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
