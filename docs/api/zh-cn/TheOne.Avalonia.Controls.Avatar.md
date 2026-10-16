# Avatar

[English](../en/TheOne.Avalonia.Controls.Avatar.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Avatar.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Avatar` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

An asynchronously loaded avatar with text or person-icon fallback.

[Source / 声明](../../../TheOne.Avalonia/Controls/Avatar.cs#L17)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CurrentImage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L34) | `Avalonia.Media.IImage?` | 只读; OneWay | `computed / state` | 只读的已解码图片与成功加载状态 |
| [`DisplayText`](../../../TheOne.Avalonia/Controls/Avatar.cs#L38) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`EffectiveCornerRadius`](../../../TheOne.Avalonia/Controls/Avatar.cs#L39) | `Avalonia.CornerRadius` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L37) | `string?` | 只读; OneWay | `computed / state` | 只读加载状态与失败原因；清空来源会清除错误 |
| [`HasImage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L36) | `bool` | 只读; OneWay | `computed / state` | 只读的已解码图片与成功加载状态 |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/Avatar.cs#L35) | `bool` | 只读; OneWay | `computed / state` | 只读加载状态与失败原因；清空来源会清除错误 |
| [`Shape`](../../../TheOne.Avalonia/Controls/Avatar.cs#L32) | `TheOne.Avalonia.Controls.AvatarShape` | 可写; OneWay | `0 / default` | 默认 `Circle` 圆形，也支持 `Rounded` 圆角方形和 `Square` 方形 校验：`Enum.IsDefined` 可选值：`Circle`, `Rounded`, `Square` |
| [`Source`](../../../TheOne.Avalonia/Controls/Avatar.cs#L30) | `object?` | 可写; OneWay | `null` | 本地绝对／相对路径、`file://` URI、HTTP(S) 地址、`avares://` URI，或调用方持有的 `IImage`；地址支持字符串和 `Uri` 对象 |
| [`Stretch`](../../../TheOne.Avalonia/Controls/Avatar.cs#L33) | `Avalonia.Media.Stretch` | 可写; OneWay | `Stretch.UniformToFill` | 原生图片拉伸模式，默认 `UniformToFill`，保持比例并裁剪填充 校验：`Enum.IsDefined` 可选值：`None`, `Fill`, `Uniform`, `UniformToFill` |
| [`Text`](../../../TheOne.Avalonia/Controls/Avatar.cs#L31) | `string?` | 可写; OneWay | `null` | 占位文字；去除两端空白后最多显示两个字素，不拆分组合 emoji；空文字显示人物图标 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void Avatar.Reload()`](../../../TheOne.Avalonia/Controls/Avatar.cs#L83) | Reloads the current source while attached, for example after a failed request. |
| [`Avatar.Avatar()`](../../../TheOne.Avalonia/Controls/Avatar.cs#L51) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<object?> Avatar.SourceProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> Avatar.TextProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<AvatarShape> Avatar.ShapeProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Stretch> Avatar.StretchProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Avatar, IImage?> Avatar.CurrentImageProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Avatar, bool> Avatar.IsLoadingProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Avatar, bool> Avatar.HasImageProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Avatar, string?> Avatar.ErrorMessageProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Avatar, string> Avatar.DisplayTextProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Avatar, CornerRadius> Avatar.EffectiveCornerRadiusProperty`](../../../TheOne.Avalonia/Controls/Avatar.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.AvatarShape`](TheOne.Avalonia.Controls.AvatarShape.md)
