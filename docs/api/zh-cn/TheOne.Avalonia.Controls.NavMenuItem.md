# NavMenuItem

[English](../en/TheOne.Avalonia.Controls.NavMenuItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavMenuItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavMenuItem` · Class · `Avalonia.Controls.TreeViewItem`

A recycled native tree container; group rows expand without navigating.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L14)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsActiveAncestor`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L22) | `bool` | 可写; OneWay | `false` | 当前项是否为选中项的祖先；由菜单维护，不能用作普通子项的选中状态。 |
| [`IsCascadeItem`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L20) | `bool` | 可写; OneWay | `false` | 当前容器是否属于级联弹出菜单；由模板／菜单维护。 |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L21) | `bool` | 可写; OneWay | `false` | 启用紧凑布局；具体显示字段由所属模板决定，不改变业务数据。 |
| [`IsGroup`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L23) | `bool` | 可写; OneWay | `false` | 此菜单项是否为分组；不等同于当前项是否选中。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`NavMenuItem.NavMenuItem()`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L38) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<bool> NavMenuItem.IsGroupProperty`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> NavMenuItem.IsCompactProperty`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> NavMenuItem.IsActiveAncestorProperty`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> NavMenuItem.IsCascadeItemProperty`](../../../TheOne.Avalonia/Controls/NavMenuItem.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
