# NavMenuFilterBox

[English](../en/TheOne.Avalonia.Controls.NavMenuFilterBox.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavMenuFilterBox.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavMenuFilterBox` · Class · `Avalonia.Controls.AutoCompleteBox`

A native AutoCompleteBox used by NavMenu's optional filter header.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L12)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`NavMenuFilterBox.NavMenuFilterBox()`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L19) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<NavMenuFilterResult>? NavMenuFilterBox.ResultCommitted`](../../../TheOne.Avalonia/Controls/NavMenuFilterBox.cs#L17) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `filled` | [`TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled`](../../../TheOne.Avalonia/Themes/Variants.axaml#L83) | TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled => Background={DynamicResource One.Subtle} |
| `underlined` | [`TextBox.underlined, ComboBox.underlined, NumericUpDown.underlined, AutoCompleteBox.underlined`](../../../TheOne.Avalonia/Themes/Variants.axaml#L133) | TextBox.underlined, ComboBox.underlined, NumericUpDown.underlined, AutoCompleteBox.underlined => Background={DynamicResource One.Surface}; BorderThickness=0,0,0,1; CornerRadius=0; TextBox.underlined:focus /template/ Border#PART_BorderElement, AutoCompleteBox.underlined /template/ TextBox#PART_TextBox:focus /template/ Border#PART_BorderElement => BorderThickness=0,0,0,2; CornerRadius=0; ComboBox.underlined:focus-within, NumericUpDown.underlined:focus-within, AutoCompleteBox.underlined:focus-within => BorderThickness=0,0,0,2; TextBox.underlined:disabled /template/ Border#PART_BorderElement, AutoCompleteBox.underlined:disabled /template/ TextBox#PART_TextBox /template/ Border#PART_BorderElement, ComboBox.underlined:disabled /template/ Border#Background, ComboBox.underlined:disabled /template/ Border#HighlightBackground => BorderBrush={DynamicResource One.InputLine} |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.NavMenuFilterResult`](TheOne.Avalonia.Controls.NavMenuFilterResult.md)
