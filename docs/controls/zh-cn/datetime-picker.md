# DateTimePicker 日期时间选择

**简体中文** · [English](../en/datetime-picker.md) · [控件目录](README.md)

`TheOne.Avalonia.Controls.DateTimePicker` 继承原生 `CalendarDatePicker`，在同一日历弹层中提供 24 小时时间编辑器。无需再设置 `CalendarOptions.DisplayStyle="DateTime"`。复用原生输入、解析、校验、日期范围、禁选日期和弹层滚动隔离，不引入另一套日期模型。

```xml
<one:DateTimePicker xmlns:one="using:TheOne.Avalonia.Controls"
    SelectedDate="{CompiledBinding Appointment, Mode=TwoWay}"
    Format="yyyy-MM-dd HH:mm:ss" />
```

`Appointment` 使用 `DateTime?`，包含日期与时间；空值表示未选择。设置显示格式本身不截断秒或毫秒，控件不进行时区转换。默认格式为 `yyyy-MM-dd HH:mm`。文本提交按格式重新解析，精度及 `DateTime.Kind` 遵循原生解析规则；需编辑秒时使用含 `ss` 的格式，需编辑小数秒时加入 `fffffff`。文本解析和 `DateValidationError` 沿用原生行为。选择日历日期保留现有时间，修改时间只更新日期中的时间部分。

## 附加能力

| 属性 | 类型 / 默认值 | 用法 |
| --- | --- | --- |
| `Format` | `string?` / `null` | .NET 日期时间格式；`null` 恢复默认格式。无效格式沿用原生格式异常行为，不静默替换。 |
| `Time` | `TimeSpan?` / `null`，默认双向 | 与 `SelectedDate` 的时间部分同步，有效范围 `[00:00,24:00)`，越界值拒绝赋值。 |

```xml
<one:DateTimePicker SelectedDate="{CompiledBinding Appointment, Mode=TwoWay}"
                    Time="{CompiledBinding AppointmentTime, Mode=TwoWay}" />
<one:DateTimePicker Classes="filled" Format="yyyy/MM/dd HH:mm" />
```

通常只绑定 `SelectedDate`；仅需独立显示或保存时分时再绑定 `Time`，不要让两个绑定源相互强制写入冲突值。空日期时设置 `Time` 不自动生成日期，随后通过日历选择日期会合并预设时间。外部直接赋值完整 `SelectedDate` 则以该值的时间为准。清空已选日期同步清空时间；有日期时设置 `Time=null` 将该日期时间归零至午夜。

`filled` 沿用基础日期控件的外观；Light / Dark、三档尺寸与禁用状态使用相同主题。通用配置见 [Home / 主题](../../theme/zh-cn/README.md)。没有额外业务方法或内置模型。原有 `CalendarOptions` 附加用法继续有效；新控件建议直接使用 `Format`、`Time`，不要再覆盖 `DisplayStyle`。

## 原生行为与边界

- `DisplayDateStart`、`DisplayDateEnd`、`BlackoutDates` 限制日历日期，并非精确到时分秒的区间。
- Enter 提交文本，日历按钮打开弹层；默认点击输入区也打开，可用 `one:CalendarInput.OpenOnClick="False"` 保留纯文本编辑。
- 时间编辑器的确认 / 取消遵循原生 `TimePicker`；日期选择即时生效，不是整体事务式确认。日历上的 Escape 通过现有 `CalendarInput` 恢复打开时的日期时间。
- 日期和嵌套时间弹层沿用现有滚轮、触摸板与惯性滚动隔离。

PreviewHost 独立 **DateTimePicker** 页展示双向值、自定义格式、可空时间与 `filled`。相关原生控件和附加模式见 [日期与时间](date-time.md)。

## 专项验证

本轮托管和 macOS ARM64 NativeAOT 消费运行各通过 55 项检查：编译双向绑定、清空与预设时间、时间边界、Enter 提交、鼠标选日保留时间、Escape 恢复、再次打开及外部赋值后的日历同步、两种主题和三档尺寸、`filled`、禁用及预览页加载。交互通过 Avalonia Headless 输入管线，布局使用 Skia 绘制；不代表 Windows / Linux 原生窗口实跑或全库回归。Release 构建通过，保留现有无关 LoginDemoViewModel 可空性警告。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### DateTimePicker

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.DateTimePicker.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Format`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L71) | `string?` | 可写; OneWay | `null` | `string?` / `null` ; .NET 日期时间格式；`null` 恢复默认格式。无效格式沿用原生格式异常行为，不静默替换。 |
| [`Time`](../../../TheOne.Avalonia/Controls/DateTimePicker.cs#L77) | `System.TimeSpan?` | 可写; TwoWay | `inherited registration` | `TimeSpan?` / `null`，默认双向 ; 与 `SelectedDate` 的时间部分同步，有效范围 `[00:00,24:00)`，越界值拒绝赋值。 |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

没有额外业务方法；通过本节专属属性配置。

### 关联模型

没有额外关联数据模型。

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
