# 日期与时间

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

**简体中文** · [English](../en/date-time.md)

CalendarItem、CalendarButton、CalendarDayButton 属于日历模板；DatePickerPresenter、TimePickerPresenter 属于选择器弹层。它们随父控件提供主题，不单独创建。替换模板时保留原生 PART_* 与绑定合约。

[返回控件文档](README.md)。扩展属性使用 `xmlns:one="using:TheOne.Avalonia.Controls"`。

<!-- visual-guide:start -->
## 外观与场景图示

[配图口径与控件图鉴](visual-index.md)；下图是当前源码的真实渲染，绑定与交互以本页用法说明为准。

### CalendarDatePicker

![CalendarDatePicker — 日期输入的三种外观；展开后复用原生日历选择，不改变日期语义。](../images/CalendarDatePicker-light.png)

日期输入的三种外观；展开后复用原生日历选择，不改变日期语义。

[查看 Dark 对照](../images/CalendarDatePicker-dark.png)

### Calendar

![Calendar — 日视图中的选中日和今天标记；切换年月日视图保持外部高度稳定。](../images/Calendar-light.png)

日视图中的选中日和今天标记；切换年月日视图保持外部高度稳定。

[查看 Dark 对照](../images/Calendar-dark.png)

### DatePicker

![DatePicker — 原生分段日期选择器在三种输入外观下的对比。](../images/DatePicker-light.png)

原生分段日期选择器在三种输入外观下的对比。

[查看 Dark 对照](../images/DatePicker-dark.png)

### TimePicker

![TimePicker — 小时与分钟选择的输入外观；时间格式由原生 ClockIdentifier 等配置决定。](../images/TimePicker-light.png)

小时与分钟选择的输入外观；时间格式由原生 ClockIdentifier 等配置决定。

[查看 Dark 对照](../images/TimePicker-dark.png)

<!-- visual-guide:end -->

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

需要独立控件时使用 [DateTimePicker](datetime-picker.md)，直接提供 `Format`、`Time` 属性；以下附加用法继续支持。

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

## DatePicker 与 TimePicker

```xml
<DatePicker SelectedDate="{Binding OffsetDate, Mode=TwoWay}" />
<TimePicker ClockIdentifier="24HourClock" MinuteIncrement="5"
            SelectedTime="{Binding Time, Mode=TwoWay}" />
```

显示区域的日期、月份、时间和时段文本在 small / normal / large 下均按原生字段水平居中、垂直居中，空值与禁用状态保持同一布局。

条目／选中条高度为 32／40／48 px，弹层字号 12／14／16 px，高度上限 288／340／400 px。确认提交，取消保留原值；箭头支持按住重复。

内部 PickerScroll.Enabled 默认启用：滚轮停止 140 ms 后吸附整数行，手势结束立即对齐，确定／Enter 对齐后提交；非循环列表限制有效范围。 选中条内外文字分层裁剪，避免小数滚动时状态文字落在错误背景上；不改变原生选择模型，卸载时停止计时并解除订阅。

日期／时间弹层会隔离未被内部消费的细粒度横向、纵向和斜向滚动及手势惯性／结束阶段，空白区域或滚动边界不会带动外层容器。内部日期切换、滚轮列滚动、惯性与结束对齐仍由原生控件处理；关闭后来自旧弹层的尾帧不影响外层，新页面滚动正常。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### CalendarInput

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.CalendarInput.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`OpenOnClick`](../../../TheOne.Avalonia/Controls/CalendarInput.cs#L14) | `bool` | 可写; OneWay; 附加属性 | `false` | 点击日期输入区域时打开原生选择弹层；不替换文本输入和键盘行为。 |

### CalendarOptions

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.CalendarOptions.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`DisplayStyle`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L25) | `TheOne.Avalonia.Controls.DateInputDisplayStyle` | 可写; OneWay; 附加属性 | `0 / default` | `Date` 默认日期；`DateTime` 日期与时间；`Month` 月份；`Year` 年份 可选值：`Date`, `DateTime`, `Month`, `Year` |
| [`Format`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L22) | `string?` | 可写; OneWay; 附加属性 | `null` | .NET 日期时间格式字符串，优先于 DisplayStyle 的自动格式 |
| [`InitialView`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L28) | `Avalonia.Controls.CalendarMode` | 可写; OneWay; 附加属性 | `0 / default` | `Month` 日期网格；`Year` 月份网格；`Decade` 年份网格；显式设置优先 可选值：`Month`, `Year`, `Decade` |
| [`ShowTime`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L38) | `bool` | 只读; OneWay; 附加属性 | `false` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Time`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L34) | `System.TimeSpan?` | 可写; OneWay; 附加属性 | `null` | `TimeSpan?`，DateTime 模式下与 SelectedDate 的时间部分同步；有效范围为 `[00:00, 24:00)` 校验：`value => value is null &#124;&#124; (value >= TimeSpan.Zero && value < TimeSpan.FromDays(1))` |
| [`WheelNavigation`](../../../TheOne.Avalonia/Controls/CalendarOptions.cs#L31) | `bool` | 可写; OneWay; 附加属性 | `false` | 设置在 Calendar 上，控制滚轮切换月份 / 年份 / 十年；主题默认开启 |

### PickerScroll

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Styling.PickerScroll.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L17) | `bool` | 可写; OneWay; 附加属性 | `false` | 日期／时间选择滚轮的交互开关；不替换原生提交、取消和焦点行为。 |
| [`HighlightClip`](../../../TheOne.Avalonia/Styling/PickerScroll.cs#L20) | `Avalonia.Media.Geometry` | 可写; OneWay; 附加属性 | `new RectangleGeometry(new Rect())` | 滚轮高亮区域的裁剪配置；通常由选择器模板维护。 |

### PickerSizing

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Styling.PickerSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ItemHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L9) | `double` | 可写; OneWay; 附加属性 | `40` | 单项／滚轮单元的 DIP 高度；主题密度和显式局部值可能覆盖注册初值。 |
| [`PopupHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L12) | `double` | 可写; OneWay; 附加属性 | `340` | 选择弹层的 DIP 高度；与 ItemHeight 和 ViewportHeight 协调。 |
| [`ViewportHeight`](../../../TheOne.Avalonia/Styling/PickerSizing.cs#L15) | `double` | 可写; OneWay; 附加属性 | `200` | 选择滚轮可见区域的 DIP 高度；不等于数据项数量。 |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

- `CalendarDatePicker`: `Classes="filled"`
- `DatePicker`: `Classes="filled"`
- `TimePicker`: `Classes="filled"`

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

没有额外业务方法；通过本节专属属性配置。

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.CalendarInput`](../../api/zh-cn/TheOne.Avalonia.Controls.CalendarInput.md) | 1 |
| [`TheOne.Avalonia.Controls.CalendarOptions`](../../api/zh-cn/TheOne.Avalonia.Controls.CalendarOptions.md) | 6 |
| [`TheOne.Avalonia.Controls.DateInputDisplayStyle`](../../api/zh-cn/TheOne.Avalonia.Controls.DateInputDisplayStyle.md) | 4 |
| [`TheOne.Avalonia.Styling.PickerScroll`](../../api/zh-cn/TheOne.Avalonia.Styling.PickerScroll.md) | 2 |
| [`TheOne.Avalonia.Styling.PickerSizing`](../../api/zh-cn/TheOne.Avalonia.Styling.PickerSizing.md) | 3 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
