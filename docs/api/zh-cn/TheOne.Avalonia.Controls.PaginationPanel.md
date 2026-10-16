# PaginationPanel

[English](../en/TheOne.Avalonia.Controls.PaginationPanel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.PaginationPanel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.PaginationPanel` · Class · `Avalonia.Controls.Panel`

Places three standard groups in a wrapping row, or navigation above a compact summary.

[Source / 声明](../../../TheOne.Avalonia/Controls/PaginationPanel.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/PaginationPanel.cs#L10) | `bool` | 可写; OneWay | `false` | 启用紧凑布局；具体显示字段由所属模板决定，不改变业务数据。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`PaginationPanel.PaginationPanel()`](../../../TheOne.Avalonia/Controls/PaginationPanel.cs#L7) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<bool> PaginationPanel.IsCompactProperty`](../../../TheOne.Avalonia/Controls/PaginationPanel.cs#L9) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
