# QRCode

[English](../en/TheOne.Avalonia.Controls.QRCode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.QRCode.md) · [API index](README.md)

`TheOne.Avalonia.Controls.QRCode` · Class · `Avalonia.Controls.Control`

A vector QR code with a square symbol and a protected quiet zone.

[Source / 声明](../../../TheOne.Avalonia/Controls/QRCode.cs#L11)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/QRCode.cs#L42) | `Avalonia.Media.IBrush?` | 可写; OneWay | `Brushes.White` | Light modules and quiet zone. Defaults to white independently of the application theme. |
| [`ErrorCorrection`](../../../TheOne.Avalonia/Controls/QRCode.cs#L36) | `TheOne.Avalonia.Controls.QRCodeErrorCorrection` | 可写; OneWay | `QRCodeErrorCorrection.Medium` | Defaults to Medium. Changes regenerate the cached symbol. 校验：`value => Enum.IsDefined(value)` 可选值：`Low`, `Medium`, `Quartile`, `High` |
| [`Foreground`](../../../TheOne.Avalonia/Controls/QRCode.cs#L40) | `Avalonia.Media.IBrush?` | 可写; OneWay | `Brushes.Black` | Dark modules. Defaults to black independently of the application theme. |
| [`QuietZone`](../../../TheOne.Avalonia/Controls/QRCode.cs#L38) | `int` | 可写; OneWay | `4` | Clear border on every side, in modules. Range 4–32; default 4. 校验：`value => value is >= 4 and <= 32` |
| [`Text`](../../../TheOne.Avalonia/Controls/QRCode.cs#L34) | `string?` | 可写; OneWay | `""` | Payload, preserved exactly, encoded as UTF-8 with an ECI designator. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/QRCode.cs#L44) | `string?` | 只读; OneWay | `computed / state` | Read-only encoding or minimum-module-size error; null when the current symbol is valid. |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override void QRCode.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/QRCode.cs#L71) | Avalonia 绘制回调；使用提供的 DrawingContext 绘制当前状态，业务代码通常不直接调用。 |
| [`QRCode.QRCode()`](../../../TheOne.Avalonia/Controls/QRCode.cs#L11) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string?> QRCode.TextProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<QRCodeErrorCorrection> QRCode.ErrorCorrectionProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<int> QRCode.QuietZoneProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> QRCode.ForegroundProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> QRCode.BackgroundProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<QRCode, string?> QRCode.ValidationErrorProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.QRCodeErrorCorrection`](TheOne.Avalonia.Controls.QRCodeErrorCorrection.md)
