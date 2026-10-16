# CardTabChrome

[English](../en/TheOne.Avalonia.Controls.CardTabChrome.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CardTabChrome.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CardTabChrome` · Class · `Avalonia.Controls.Decorator`

Draws a card tab with an open content edge and small concave joining corners.

[Source / 声明](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L15) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 背景画刷；主题相关背景使用 DynamicResource，避免固定颜色破坏明暗切换。 |
| [`BorderBrush`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L16) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 边缘描边画刷；使用动态主题资源以保持明暗切换。 |
| [`CornerRadius`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L17) | `Avalonia.CornerRadius` | 可写; OneWay | `inherited registration` | 外观圆角，单位为 Avalonia DIP；尺寸和背景裁剪仍由所属模板决定。 |
| [`IsSelected`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L18) | `bool` | 可写; OneWay | `false` | 当前项的选择状态；与控件选中模型保持一致，不与无关项共享状态。 |
| [`Placement`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L19) | `Avalonia.Controls.Dock` | 可写; OneWay | `Dock.Top` | 装饰或内容的位置枚举；按枚举值选择边缘，不使用字符串坐标。 可选值：`Left`, `Bottom`, `Right`, `Top` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override void CardTabChrome.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L26) | Avalonia 绘制回调；使用提供的 DrawingContext 绘制当前状态，业务代码通常不直接调用。 |
| [`CardTabChrome.CardTabChrome()`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L8) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> CardTabChrome.BackgroundProperty`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L10) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> CardTabChrome.BorderBrushProperty`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<CornerRadius> CardTabChrome.CornerRadiusProperty`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> CardTabChrome.IsSelectedProperty`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Dock> CardTabChrome.PlacementProperty`](../../../TheOne.Avalonia/Controls/CardTabChrome.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
