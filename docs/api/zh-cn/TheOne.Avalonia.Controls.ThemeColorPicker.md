# ThemeColorPicker

[English](../en/TheOne.Avalonia.Controls.ThemeColorPicker.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ThemeColorPicker.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ThemeColorPicker` · Class · `Avalonia.Controls.ComboBox`

Selects the application OneTheme color palette independently of Light / Dark.

[Source / 声明](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L11)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`SelectThemeColor`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L17) | `TheOne.Avalonia.Themes.ThemeColor` | 可写; TwoWay | `0 / default` | The selected palette, suitable for a persisted two-way enum binding. 校验：`value => Enum.IsDefined(value)` 可选值：`Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |
| [`SelectedColor`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L25) | `TheOne.Avalonia.Themes.ThemeColor` | 可写; TwoWay | `0 / default` | SelectedThemeColor 的兼容别名；两者表示同一颜色枚举，优先绑定 SelectedThemeColor。 校验：`value => Enum.IsDefined(value)` 可选值：`Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ThemeColorPicker.ThemeColorPicker()`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L37) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<ThemeColor> ThemeColorPicker.SelectThemeColorProperty`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L14) | The selected palette, suitable for a persisted two-way enum binding. |
| [`static readonly StyledProperty<ThemeColor> ThemeColorPicker.SelectedColorProperty`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `filled` | [`TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled`](../../../TheOne.Avalonia/Themes/Variants.axaml#L83) | TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled => Background={DynamicResource One.Subtle}; ComboBox.filled /template/ Border#HighlightBackground => Background={DynamicResource One.Subtle} |
| `underlined` | [`TextBox.underlined, ComboBox.underlined, NumericUpDown.underlined, AutoCompleteBox.underlined`](../../../TheOne.Avalonia/Themes/Variants.axaml#L133) | TextBox.underlined, ComboBox.underlined, NumericUpDown.underlined, AutoCompleteBox.underlined => Background={DynamicResource One.Surface}; BorderThickness=0,0,0,1; CornerRadius=0; ComboBox.underlined:focus-within, NumericUpDown.underlined:focus-within, AutoCompleteBox.underlined:focus-within => BorderThickness=0,0,0,2; ComboBox.underlined /template/ TextBox#PART_EditableTextBox => MinHeight=0; FocusAdorner={x:Null}; ComboBox.underlined /template/ TextBox#PART_EditableTextBox /template/ Border#PART_BorderElement => Background=Transparent; BorderBrush=Transparent; BorderThickness=0; CornerRadius=0; ComboBox.underlined:focus-within /template/ Border#Background, ComboBox.underlined:focus-within /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Focus}; ComboBox.underlined:focus-within /template/ Border#Background, ComboBox.underlined:focus-within /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Focus}; ComboBox.underlined:error /template/ Border#Background, ComboBox.underlined:error /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Danger}; ComboBox.underlined:error /template/ Border#Background, ComboBox.underlined:error /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.Danger}; TextBox.underlined:disabled /template/ Border#PART_BorderElement, AutoCompleteBox.underlined:disabled /template/ TextBox#PART_TextBox /template/ Border#PART_BorderElement, ComboBox.underlined:disabled /template/ Border#Background, ComboBox.underlined:disabled /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.InputLine}; TextBox.underlined:disabled /template/ Border#PART_BorderElement, AutoCompleteBox.underlined:disabled /template/ TextBox#PART_TextBox /template/ Border#PART_BorderElement, ComboBox.underlined:disabled /template/ Border#Background, ComboBox.underlined:disabled /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.InputLine} |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Themes.ThemeColor`](TheOne.Avalonia.Themes.ThemeColor.md)
