# ThemeButton

[English](../en/TheOne.Avalonia.Controls.ThemeButton.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ThemeButton.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ThemeButton` · Class · `Avalonia.Controls.Primitives.ToggleButton`

Switches the current effective light/dark theme without overriding a binding.

[Source / 声明](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L11)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Scope`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L16) | `TheOne.Avalonia.Controls.ThemeButtonScope` | 可写; OneWay | `0 / default` | 主题按钮切换的作用域；影响 Light/Dark，不切换 ThemeColor 颜色枚举。 可选值：`Window`, `Application` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ThemeButton.ThemeButton()`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L18) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<ThemeButtonScope> ThemeButton.ScopeProperty`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `segment` | [`ToggleButton.segment`](../../../TheOne.Avalonia/Themes/Styles.axaml#L69) | ToggleButton.segment => BorderThickness=0 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.ThemeButtonScope`](TheOne.Avalonia.Controls.ThemeButtonScope.md)
