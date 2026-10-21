# Tag

[English](../en/TheOne.Avalonia.Controls.Tag.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Tag.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Tag` · Class · `Avalonia.Controls.ContentControl`

A compact status label with optional caller-owned icon and content.

[Source / 声明](../../../TheOne.Avalonia/Controls/Tag.cs#L27)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Icon`](../../../TheOne.Avalonia/Controls/Tag.cs#L59) | `object?` | 可写; OneWay | `null` | 可选图标槽，支持 PathIcon、Image 或模型模板；保留控件逻辑树与 DataContext |
| [`IconSize`](../../../TheOne.Avalonia/Controls/Tag.cs#L53) | `double` | 可写; OneWay | `13` | 图标缩放边长，默认 13 px，比原 14 px 基准减小一号；三档密度共用，可覆盖，不修改调用方图标尺寸 校验：`value => double.IsFinite(value) && value > 0` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/Tag.cs#L65) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 可选图标槽，支持 PathIcon、Image 或模型模板；保留控件逻辑树与 DataContext |
| [`Spacing`](../../../TheOne.Avalonia/Controls/Tag.cs#L83) | `double` | 可写; OneWay | `6` | 图标与内容间距，默认 6；没有图标或文字时不保留空隙 校验：`value => double.IsFinite(value) && value >= 0` |
| [`Status`](../../../TheOne.Avalonia/Controls/Tag.cs#L71) | `TheOne.Avalonia.Controls.TagStatus` | 可写; OneWay | `0 / default` | `Default`、`Information`、`Success`、`Warning`、`Error`、`Processing` 校验：`Enum.IsDefined` 可选值：`Default`, `Information`, `Success`, `Warning`, `Error`, `Processing` |
| [`Variant`](../../../TheOne.Avalonia/Controls/Tag.cs#L77) | `TheOne.Avalonia.Controls.TagVariant` | 可写; OneWay | `0 / default` | `Soft` 浅底、`Outline` 描边、`Solid` 实心 校验：`Enum.IsDefined` 可选值：`Soft`, `Outline`, `Solid` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Tag.Tag()`](../../../TheOne.Avalonia/Controls/Tag.cs#L48) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<object?> Tag.IconProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> Tag.IconTemplateProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TagStatus> Tag.StatusProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TagVariant> Tag.VariantProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> Tag.IconSizeProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L40) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> Tag.SpacingProperty`](../../../TheOne.Avalonia/Controls/Tag.cs#L44) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `dashed` | [`^.dashed`](../../../TheOne.Avalonia/Themes/Controls/Tag.axaml#L191) | ^.dashed => BorderBrush=Transparent; ^.dashed /template/ Rectangle#PART_DashedOutline => IsVisible=True |
| `dot` | [`^.dot /template/ Ellipse#PART_StatusDot`](../../../TheOne.Avalonia/Themes/Controls/Tag.axaml#L197) | ^.dot /template/ Ellipse#PART_StatusDot => IsVisible=True |
| `gradient` | [`^.gradient`](../../../TheOne.Avalonia/Themes/Controls/Tag.axaml#L200) | ^.gradient => Background={DynamicResource One.Gradient.Brand}; Foreground={DynamicResource One.OnAccent}; BorderBrush=Transparent |
| `pill` | [`^.pill`](../../../TheOne.Avalonia/Themes/Controls/Tag.axaml#L185) | ^.pill => CornerRadius=999 |
| `square` | [`^.square`](../../../TheOne.Avalonia/Themes/Controls/Tag.axaml#L188) | ^.square => CornerRadius=0 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.TagStatus`](TheOne.Avalonia.Controls.TagStatus.md)
- [`TheOne.Avalonia.Controls.TagVariant`](TheOne.Avalonia.Controls.TagVariant.md)
