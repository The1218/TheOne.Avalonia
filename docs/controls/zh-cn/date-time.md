# 日期与时间

**简体中文** · [English](../en/date-time.md)

CalendarItem、CalendarButton、CalendarDayButton 属于日历模板；DatePickerPresenter、TimePickerPresenter 属于选择器弹层。它们随父控件提供主题，不单独创建。替换模板时保留原生 PART_* 与绑定合约。

[返回控件文档](README.md)。扩展属性使用 `xmlns:one="using:TheOne.Avalonia.Controls"`。

## CalendarDatePicker

```xml
<CalendarDatePicker SelectedDate="{Binding Date, Mode=TwoWay}"
                    SelectedDateFormat="Custom"
                    CustomDateFormatString="yyyy-MM-dd"
                    PlaceholderText="yyyy-MM-dd"
                    one:CalendarInput.OpenOnClick="False" />
```

`SelectedDate` 为 `DateTime?`。支持键盘输入与 Enter 提交，非法输入沿用官方 `DateValidationError` 事件。编辑区点击默认打开日历；`OpenOnClick="False"` 关闭自动打开，日历按钮仍可使用。宿主可通过 `DisplayDateStart`、`DisplayDateEnd` 与 `BlackoutDates` 限制选择范围。

## 日期时间、月份、年份输入

```xml
<CalendarDatePicker one:CalendarOptions.DisplayStyle="DateTime"
                    one:CalendarOptions.Format="yyyy/MM/dd HH:mm"
                    SelectedDate="{Binding Appointment, Mode=TwoWay}" />
<CalendarDatePicker one:CalendarOptions.DisplayStyle="Month"
                    SelectedDate="{Binding ReportingMonth, Mode=TwoWay}" />
<CalendarDatePicker one:CalendarOptions.DisplayStyle="Year"
                    SelectedDate="{Binding ReportingYear, Mode=TwoWay}" />
```

| 附加属性 | 用法 |
| --- | --- |
| `CalendarOptions.DisplayStyle` | `Date` 默认日期；`DateTime` 日期与时间；`Month` 月份；`Year` 年份 |
| `CalendarOptions.Format` | .NET 日期时间格式字符串，优先于 DisplayStyle 的自动格式 |
| `CalendarOptions.InitialView` | `Month` 日期网格；`Year` 月份网格；`Decade` 年份网格；显式设置优先 |
| `CalendarOptions.Time` | `TimeSpan?`，DateTime 模式下与 SelectedDate 的时间部分同步；有效范围为 `[00:00, 24:00)` |
| `CalendarOptions.WheelNavigation` | 设置在 Calendar 上，控制滚轮切换月份 / 年份 / 十年；主题默认开启 |

DateTime、Month、Year 默认格式分别为 `yyyy-MM-dd HH:mm`、`yyyy-MM`、`yyyy`。Month / Year 修改显示格式与初始视图，`SelectedDate` 仍是完整日期，业务按需要提取年份和月份。DateTime 模式包含时间编辑器，切换日期保留时间部分；普通 Date 使用原生格式属性。

## 独立 Calendar

```xml
<Calendar SelectedDate="{Binding Date, Mode=TwoWay}"
          SelectionMode="SingleDate"
          one:CalendarOptions.WheelNavigation="False" />
```

保留月份翻页、年 / 十年视图、范围与禁选日期、键盘选择。今天用描边标记，选中日期用强调色。需要多日期时使用官方 `SelectionMode` 和 `SelectedDates`；日期网格为固定上限布局。

日期、月份、年份网格在同一档尺寸下预留相同主体高度：Small 258px、Normal 294px、Large 342px。切换视图或直接打开月份／年份选择时，外部高度保持稳定，日期单元格按自然尺寸居中排列。月份／年份按钮按文字自然尺寸及原生最小高度居中，选中背景不再铺满整个网格槽，命中区域、焦点与选择行为仍由原生按钮提供。CalendarDatePicker 弹层共用此布局。自定义日历字体或单元格尺寸时，可覆盖 `One.CalendarBodyHeight.Small`、`.Normal`、`.Large` 资源。PreviewHost 的 `--verify-calendar-layout [output]` 仅验证 Calendar 及其 CalendarDatePicker 弹层调用方。

## DatePicker 与 TimePicker

```xml
<DatePicker SelectedDate="{Binding OffsetDate, Mode=TwoWay}" />
<TimePicker ClockIdentifier="24HourClock" MinuteIncrement="5"
            SelectedTime="{Binding Time, Mode=TwoWay}" />
```

DatePicker 的 `SelectedDate` 为 `DateTimeOffset?`，TimePicker 的 `SelectedTime` 为 `TimeSpan?`，与 CalendarDatePicker 的 `DateTime?` 类型不同。保留原生按视口回收的 DateTimePickerPanel，不需要替换内部滚动面板。示例见 [EnhancementsPreview.cs](../../../design/PreviewHost/Verification/Basic/EnhancementsPreview.cs) 和 [ControlGallery.axaml](../../../design/PreviewHost/Verification/Fixtures/LegacyControlGallery.axaml)。

显示区域的日期、月份、时间和时段文本在 small / normal / large 下均按原生字段水平居中、垂直居中，空值与禁用状态保持同一布局。

条目／选中条高度为 32／40／48 px，弹层字号 12／14／16 px，高度上限 288／340／400 px。确认提交，取消保留原值；箭头支持按住重复。

内部 PickerScroll.Enabled 默认启用：滚轮停止 140 ms 后吸附整数行，手势结束立即对齐，确定／Enter 对齐后提交；非循环列表限制有效范围。选中条内外文字分层裁剪，避免小数滚动时状态文字落在错误背景上；不改变原生选择模型，卸载时停止计时并解除订阅。专项和定向截图见[预览专项入口](../../../design/PreviewHost/Program.cs)。
