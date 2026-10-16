# NavMenuSearchText

[English](../en/TheOne.Avalonia.Controls.NavMenuSearchText.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavMenuSearchText.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavMenuSearchText` · Class · `Avalonia.Controls.TextBlock`

Search-aware text for NavMenu content templates. SourceText remains bindable when inlines are rebuilt.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L9)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`HighlightQuery`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L12) | `string?` | 可写; OneWay; 附加属性; 向后代继承 | `null` | 需在源文本中高亮的查询；只改变绘制，不修改 SourceText。 |
| [`SourceText`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L13) | `string?` | 可写; OneWay | `null` | 高亮前的完整源文本；不要传入格式化的富文本标记。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static string? NavMenuSearchText.GetHighlightQuery(Control control)`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L14) | 读取附加属性。需在源文本中高亮的查询；只改变绘制，不修改 SourceText。 |
| [`static void NavMenuSearchText.SetHighlightQuery(Control control, string? value)`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L15) | 设置附加属性。需在源文本中高亮的查询；只改变绘制，不修改 SourceText。 |
| [`NavMenuSearchText.NavMenuSearchText()`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L9) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string?> NavMenuSearchText.SourceTextProperty`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<string?> NavMenuSearchText.HighlightQueryProperty`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
