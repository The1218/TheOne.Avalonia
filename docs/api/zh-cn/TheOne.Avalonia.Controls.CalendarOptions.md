# CalendarOptions

[English](../en/TheOne.Avalonia.Controls.CalendarOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.CalendarOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.CalendarOptions` · Class · `Avalonia.AvaloniaObject`

Date formatting, optional time editing and wheel navigation for native calendars.

[Source / 声明](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L20)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`DisplayStyle`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L25) | `TheOne.Avalonia.Controls.DateInputDisplayStyle` | 可写; OneWay; 附加属性 | `0 / default` | `Date` 默认日期；`DateTime` 日期与时间；`Month` 月份；`Year` 年份 可选值：`Date`, `DateTime`, `Month`, `Year` |
| [`Format`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L22) | `string?` | 可写; OneWay; 附加属性 | `null` | .NET 日期时间格式字符串，优先于 DisplayStyle 的自动格式 |
| [`InitialView`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L28) | `Avalonia.Controls.CalendarMode` | 可写; OneWay; 附加属性 | `0 / default` | `Month` 日期网格；`Year` 月份网格；`Decade` 年份网格；显式设置优先 可选值：`Month`, `Year`, `Decade` |
| [`ShowTime`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L38) | `bool` | 只读; OneWay; 附加属性 | `false` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Time`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L34) | `System.TimeSpan?` | 可写; OneWay; 附加属性 | `null` | `TimeSpan?`，DateTime 模式下与 SelectedDate 的时间部分同步；有效范围为 `[00:00, 24:00)` 校验：`value => value is null &#124;&#124; (value >= TimeSpan.Zero && value < TimeSpan.FromDays(1))` |
| [`WheelNavigation`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L31) | `bool` | 可写; OneWay; 附加属性 | `false` | 设置在 Calendar 上，控制滚轮切换月份 / 年份 / 十年；主题默认开启 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static string? CalendarOptions.GetFormat(CalendarDatePicker picker)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L103) | 读取附加属性。.NET 日期时间格式字符串，优先于 DisplayStyle 的自动格式 |
| [`static void CalendarOptions.SetFormat(CalendarDatePicker picker, string? value)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L108) | 设置附加属性。.NET 日期时间格式字符串，优先于 DisplayStyle 的自动格式 |
| [`static DateInputDisplayStyle CalendarOptions.GetDisplayStyle(Control control)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L113) | 读取附加属性。`Date` 默认日期；`DateTime` 日期与时间；`Month` 月份；`Year` 年份 |
| [`static void CalendarOptions.SetDisplayStyle(Control control, DateInputDisplayStyle value)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L118) | 设置附加属性。`Date` 默认日期；`DateTime` 日期与时间；`Month` 月份；`Year` 年份 |
| [`static CalendarMode CalendarOptions.GetInitialView(CalendarDatePicker picker)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L123) | 读取附加属性。`Month` 日期网格；`Year` 月份网格；`Decade` 年份网格；显式设置优先 |
| [`static void CalendarOptions.SetInitialView(CalendarDatePicker picker, CalendarMode value)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L128) | 设置附加属性。`Month` 日期网格；`Year` 月份网格；`Decade` 年份网格；显式设置优先 |
| [`static bool CalendarOptions.GetWheelNavigation(Calendar calendar)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L133) | 读取附加属性。设置在 Calendar 上，控制滚轮切换月份 / 年份 / 十年；主题默认开启 |
| [`static void CalendarOptions.SetWheelNavigation(Calendar calendar, bool value)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L138) | 设置附加属性。设置在 Calendar 上，控制滚轮切换月份 / 年份 / 十年；主题默认开启 |
| [`static TimeSpan? CalendarOptions.GetTime(Control control)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L143) | 读取附加属性。`TimeSpan?`，DateTime 模式下与 SelectedDate 的时间部分同步；有效范围为 `[00:00, 24:00)` |
| [`static void CalendarOptions.SetTime(Control control, TimeSpan? value)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L148) | 设置附加属性。`TimeSpan?`，DateTime 模式下与 SelectedDate 的时间部分同步；有效范围为 `[00:00, 24:00)` |
| [`static bool CalendarOptions.GetShowTime(Control control)`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L153) | 读取附加属性。只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CalendarOptions.CalendarOptions()`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L20) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<string?> CalendarOptions.FormatProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<DateInputDisplayStyle> CalendarOptions.DisplayStyleProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<CalendarMode> CalendarOptions.InitialViewProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> CalendarOptions.WheelNavigationProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<TimeSpan?> CalendarOptions.TimeProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> CalendarOptions.ShowTimeProperty`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L38) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.DateInputDisplayStyle`](TheOne.Avalonia.Controls.DateInputDisplayStyle.md)
