# InfoCard

[English](../en/TheOne.Avalonia.Controls.InfoCard.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.InfoCard.md) · [API index](README.md)

`TheOne.Avalonia.Controls.InfoCard` · Class · `Avalonia.Controls.Primitives.HeaderedContentControl`

A themed information container with a title, icon, actions, body and footer.

[Source / 声明](../../../TheOne.Avalonia/Controls/InfoCard.cs#L17)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`BoxShadow`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L93) | `Avalonia.Media.BoxShadows` | 可写; OneWay | `inherited registration` | Outer shadow, supplied by the shadow and hove theme classes. |
| [`Footer`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L123) | `object?` | 可写; OneWay | `null` | 底部区域内容；与主体和标题分别定制。 |
| [`FooterBackground`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L143) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 页脚区域背景，默认 `One.InfoCard.Footer` |
| [`FooterPadding`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L153) | `Avalonia.Thickness` | 只读; OneWay | `computed / state` | Computed footer insets: horizontal Padding and vertical FooterSpacing. |
| [`FooterSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L193) | `double` | 可写; OneWay | `12` | 页脚区域的上下内边距 校验：`value => double.IsFinite(value) && value >= 0` |
| [`FooterTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L129) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`HeaderActions`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L111) | `object?` | 可写; OneWay | `null` | 标题区右侧的操作内容；可放置业务按钮，保持命令和焦点行为。 |
| [`HeaderActionsTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L117) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`HeaderBackground`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L136) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 标题区域背景，默认 `One.Subtle` |
| [`HeaderContentSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L173) | `double` | 可写; OneWay | `10` | 8 ; 10 ; 12 校验：`value => double.IsFinite(value) && value >= 0` |
| [`HeaderFontSize`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L161) | `double` | 可写; OneWay | `16` | 14 ; 16 ; 18 校验：`value => double.IsFinite(value) && value > 0` |
| [`HeaderIcon`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L99) | `object?` | 可写; OneWay | `null` | 标题区图标内容，与标题文本分开定制。 |
| [`HeaderIconSize`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L167) | `double` | 可写; OneWay | `18` | 16 ; 18 ; 20 校验：`value => double.IsFinite(value) && value > 0` |
| [`HeaderIconTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L105) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`HeaderPadding`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L150) | `Avalonia.Thickness` | 只读; OneWay | `computed / state` | Computed header insets: horizontal Padding and vertical HeaderSpacing. |
| [`HeaderSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L155) | `double` | 可写; OneWay | `12` | 标题区域的上下内边距 校验：`value => double.IsFinite(value) && value >= 0` |
| [`ShowFooterSeparator`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L187) | `bool` | 可写; OneWay | `true` | Shows a divider before an existing footer when another region precedes it. |
| [`ShowHeaderSeparator`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L180) | `bool` | 可写; OneWay | `true` | Shows an edge-to-edge divider between the header and body. |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`InfoCard.InfoCard()`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L87) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<BoxShadows> InfoCard.BoxShadowProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> InfoCard.HeaderIconProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> InfoCard.HeaderIconTemplateProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> InfoCard.HeaderActionsProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> InfoCard.HeaderActionsTemplateProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> InfoCard.FooterProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> InfoCard.FooterTemplateProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L36) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> InfoCard.HeaderBackgroundProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L39) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IBrush?> InfoCard.FooterBackgroundProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L42) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<InfoCard, Thickness> InfoCard.HeaderPaddingProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L45) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<InfoCard, Thickness> InfoCard.FooterPaddingProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L48) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> InfoCard.HeaderSpacingProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L51) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> InfoCard.HeaderFontSizeProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L55) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> InfoCard.HeaderIconSizeProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L59) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> InfoCard.HeaderContentSpacingProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L63) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> InfoCard.ShowHeaderSeparatorProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L67) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> InfoCard.ShowFooterSeparatorProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L70) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> InfoCard.FooterSpacingProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L73) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `flat` | [`one&#124;InfoCard.flat`](../../../TheOne.Avalonia/Themes/Variants.axaml#L114) | one&#124;InfoCard.flat => BorderThickness=0; HeaderBackground=Transparent; FooterBackground=Transparent; ShowHeaderSeparator=False; ShowFooterSeparator=False; one&#124;InfoCard.glow:disabled, one&#124;InfoCard.flat:disabled => HeaderBackground={DynamicResource One.DisabledSurface}; FooterBackground={DynamicResource One.DisabledSurface} |
| `glow` | [`one&#124;InfoCard.glow`](../../../TheOne.Avalonia/Themes/Variants.axaml#L111) | one&#124;InfoCard.glow => HeaderBackground={DynamicResource One.Gradient.Glow}; one&#124;InfoCard.glow:disabled, one&#124;InfoCard.flat:disabled => HeaderBackground={DynamicResource One.DisabledSurface}; FooterBackground={DynamicResource One.DisabledSurface} |
| `hove` | [`^.hove:pointerover:not(:disabled)`](../../../TheOne.Avalonia/Themes/Controls/InfoCard.axaml#L219) | ^.hove:pointerover:not(:disabled) => BorderBrush={DynamicResource One.Accent}; Background={DynamicResource One.SurfaceHover}; BoxShadow={DynamicResource One.InfoCard.HoverShadow} |
| `outlined` | [`one&#124;InfoCard.outlined`](../../../TheOne.Avalonia/Themes/Variants.axaml#L219) | one&#124;InfoCard.outlined => BorderBrush={DynamicResource One.Accent}; HeaderBackground={DynamicResource One.AccentSoft}; one&#124;InfoCard.outlined:disabled => BorderBrush={DynamicResource One.Line}; HeaderBackground={DynamicResource One.DisabledSurface} |
| `shadow` | [`^.shadow:not(:disabled)`](../../../TheOne.Avalonia/Themes/Controls/InfoCard.axaml#L216) | ^.shadow:not(:disabled) => BoxShadow={DynamicResource One.InfoCard.Shadow} |
