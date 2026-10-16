# IconButtonPanel

[English](../en/TheOne.Avalonia.Controls.IconButtonPanel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IconButtonPanel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IconButtonPanel` · Class · `Avalonia.Controls.Panel`

Template layout: the first two children are icon and label, the third is the loading replacement.

[Source / 声明](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L11)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IconPlacement`](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L15) | `TheOne.Avalonia.Controls.IconPlacement` | 可写; OneWay | `0 / default` | 图标相对文本的位置枚举；左右或上下布局由模板处理。 可选值：`Left`, `Right`, `Top`, `Bottom` |
| [`Spacing`](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L16) | `double` | 可写; OneWay | `8` | 子内容之间的 DIP 间距；不代替容器 Padding 或子项 Margin。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`IconButtonPanel.IconButtonPanel()`](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L11) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IconPlacement> IconButtonPanel.IconPlacementProperty`](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> IconButtonPanel.SpacingProperty`](../../../TheOne.Avalonia/Controls/IconButtonPanel.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.IconPlacement`](TheOne.Avalonia.Controls.IconPlacement.md)
