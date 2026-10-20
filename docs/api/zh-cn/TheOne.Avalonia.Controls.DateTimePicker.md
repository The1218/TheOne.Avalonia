# DateTimePicker

[English](../en/TheOne.Avalonia.Controls.DateTimePicker.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DateTimePicker.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DateTimePicker` · Class · `Avalonia.Controls.CalendarDatePicker`

A native calendar input with an integrated 24-hour time editor.

[Source / 声明](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L11)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Format`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L71) | `string?` | 可写; OneWay | `null` | `string?` / `null` ; .NET 日期时间格式；`null` 恢复默认格式。无效格式沿用原生格式异常行为，不静默替换。 |
| [`Time`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L77) | `System.TimeSpan?` | 可写; TwoWay | `inherited registration` | `TimeSpan?` / `null`，默认双向 ; 与 `SelectedDate` 的时间部分同步，有效范围 `[00:00,24:00)`，越界值拒绝赋值。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`DateTimePicker.DateTimePicker()`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L25) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string?> DateTimePicker.FormatProperty`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L17) | The display format; null uses yyyy-MM-dd HH:mm. |
| [`static readonly StyledProperty<TimeSpan?> DateTimePicker.TimeProperty`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L21) | The time of day, synchronized with SelectedDate without replacing bindings. |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `filled` | [`TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled`](../../../TheOne.Avalonia/Themes/Variants.axaml#L86) | TextBox.filled, ComboBox.filled, NumericUpDown.filled, AutoCompleteBox.filled, CalendarDatePicker.filled, DatePicker.filled, TimePicker.filled => Background={DynamicResource One.Subtle} |
