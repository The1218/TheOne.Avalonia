# ImageUpload

[English](../en/TheOne.Avalonia.Controls.ImageUpload.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ImageUpload.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ImageUpload` · Class · `TheOne.Avalonia.Controls.Upload`

[继承的额外成员: Upload](TheOne.Avalonia.Controls.Upload.md)

A single-image card with an accessible hover toolbar and a read-only preview mode.

[Source / 声明](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L22)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`DisplayHint`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L65) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HintText`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L62) | `string?` | 可写; OneWay | `null` | ImageUpload 的 HintText 配置字段；值类型和初值如表，通过 HintTextProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`ImageHeight`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L63) | `double` | 可写; OneWay | `240` | ImageUpload 的 ImageHeight 配置字段；值类型和初值如表，通过 ImageHeightProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 校验：`value => double.IsFinite(value) && value > 0` |
| [`ImageStretch`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L64) | `Avalonia.Media.Stretch` | 可写; OneWay | `Stretch.Uniform` | ImageUpload 的 ImageStretch 配置字段；值类型和初值如表，通过 ImageStretchProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 可选值：`None`, `Fill`, `Uniform`, `UniformToFill` |
| [`Mode`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L60) | `TheOne.Avalonia.Controls.ImageUploadMode` | 可写; OneWay | `0 / default` | ImageUpload 的 Mode 配置字段；值类型和初值如表，通过 ModeProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 校验：`value => value is ImageUploadMode.Upload or ImageUploadMode.Preview` 可选值：`Upload`, `Preview` |
| [`ViewCommand`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L66) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `null` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`ViewText`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L61) | `string` | 可写; OneWay | `"查看"` | ImageUpload 的 ViewText 配置字段；值类型和初值如表，通过 ViewTextProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void ImageUpload.View()`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L99) | Opens the image viewer; Escape or the close button dismisses it. |
| [`ImageUpload.ImageUpload()`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L55) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<ImageUploadMode> ImageUpload.ModeProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> ImageUpload.ViewTextProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> ImageUpload.HintTextProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> ImageUpload.ImageHeightProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Stretch> ImageUpload.ImageStretchProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L38) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImageUpload, string> ImageUpload.DisplayHintProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L41) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.ImageUploadMode`](TheOne.Avalonia.Controls.ImageUploadMode.md)
