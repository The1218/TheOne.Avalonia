# OneTheme

[English](../en/TheOne.Avalonia.Themes.OneTheme.md) · [简体中文](../zh-cn/TheOne.Avalonia.Themes.OneTheme.md) · [API index](README.md)

`TheOne.Avalonia.Themes.OneTheme` · Class · `Avalonia.Styling.Styles`

TheOne theme for Avalonia's built-in controls, including light and dark resources.

[Source / 声明](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L9)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ColorTheme`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L14) | `TheOne.Avalonia.Themes.ThemeColor` | 可写; OneWay | `0 / default` | Color palette; changing this does not change the requested Light / Dark variant. 校验：`value => Enum.IsDefined(value)` 可选值：`Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |
| [`ColorThemes`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L15) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Themes.ThemeColor>` | 只读; CLR / n/a | `Array.AsReadOnly(Enum.GetValues<ThemeColor>())` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static void OneTheme.SetColor(ThemeColor color)`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L19) | Switches the application's OneTheme palette on the UI thread, preserving Light / Dark and property bindings. |
| [`OneTheme.OneTheme()`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L28) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<ThemeColor> OneTheme.ColorThemeProperty`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Themes.ThemeColor`](TheOne.Avalonia.Themes.ThemeColor.md)
