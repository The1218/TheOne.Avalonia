# CalendarInput

[English](../en/TheOne.Avalonia.Controls.CalendarInput.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CalendarInput.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CalendarInput` · Class · `Avalonia.AvaloniaObject`

Click-to-open calendar with native date parsing and an Escape path back to the editor.

[Source / 声明](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L12)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`OpenOnClick`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L14) | `bool` | 可写; OneWay; 附加属性 | `false` | 点击日期输入区域时打开原生选择弹层；不替换文本输入和键盘行为。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool CalendarInput.GetOpenOnClick(CalendarDatePicker picker)`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L43) | 读取附加属性。点击日期输入区域时打开原生选择弹层；不替换文本输入和键盘行为。 |
| [`static void CalendarInput.SetOpenOnClick(CalendarDatePicker picker, bool value)`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L48) | 设置附加属性。点击日期输入区域时打开原生选择弹层；不替换文本输入和键盘行为。 |
| [`CalendarInput.CalendarInput()`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L12) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<bool> CalendarInput.OpenOnClickProperty`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
