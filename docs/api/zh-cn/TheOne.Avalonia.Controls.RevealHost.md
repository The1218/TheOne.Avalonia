# RevealHost

[English](../en/TheOne.Avalonia.Controls.RevealHost.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RevealHost.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RevealHost` · Class · `Avalonia.Controls.Decorator`

Reveals a fully laid-out child through a smoothly sized, clipped viewport.

[Source / 声明](../../../TheOne.Avalonia/Controls/RevealHost.cs#L14)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ConstrainContent`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L31) | `bool` | 可写; OneWay | `false` | Preserve a finite parent viewport for content such as a virtualized list. |
| [`Direction`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L28) | `Avalonia.Controls.ExpandDirection` | 可写; OneWay | `ExpandDirection.Down` | 展开或端口方向枚举；具体枚举类型列明可选值，不用布尔值代替方向。 可选值：`Down`, `Up`, `Left`, `Right` |
| [`Duration`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L29) | `System.TimeSpan` | 可写; OneWay | `TimeSpan.FromMilliseconds(200)` | 持续时间；动效仍遵守继承的 Motion.Enabled，不能仅用时长替代关闭开关。 |
| [`IsOpen`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L27) | `bool` | 可写; OneWay | `false` | 展开或显示内容；收起保留内容对象，过渡遵循 Motion.Enabled。 |
| [`TransitionExtent`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L22) | `double` | 可写; OneWay | `double.PositiveInfinity` | Optional visible-distance cap during animation; the settled layout retains its full extent. 校验：`value => value > 0 && !double.IsNaN(value)` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`RevealHost.RevealHost()`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L34) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<bool> RevealHost.IsOpenProperty`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ExpandDirection> RevealHost.DirectionProperty`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TimeSpan> RevealHost.DurationProperty`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> RevealHost.ConstrainContentProperty`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> RevealHost.TransitionExtentProperty`](../../../TheOne.Avalonia/Controls/RevealHost.cs#L21) | Optional visible-distance cap during animation; the settled layout retains its full extent. |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
