# ReportColumn

[English](../en/TheOne.Avalonia.Reporting.ReportColumn.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportColumn.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportColumn` · Class · `TheOne.Avalonia.Reporting.ReportObservable`

[继承的额外成员: ReportObservable](TheOne.Avalonia.Reporting.ReportObservable.md)

A detail column. FieldKey identifies an explicitly registered getter, never a reflection path.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L401)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Condition`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L457) | `TheOne.Avalonia.Reporting.ReportCondition` | 可写; CLR / n/a | `0 / default` | 报表条件格式的比较运算枚举，与 ConditionValue 一起使用。 可选值：`GreaterThan`, `LessThan`, `Equal` |
| [`ConditionColor`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L469) | `string` | 可写; CLR / n/a | `"#C43C3C"` | 命中列条件格式时使用的颜色字符串；分页器校验颜色合法性。 |
| [`ConditionEnabled`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L451) | `bool` | 可写; CLR / n/a | `false` | 启用此列的条件格式；关闭时保留条件配置但不应用颜色。 |
| [`ConditionValue`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L463) | `decimal` | 可写; CLR / n/a | `0 / default` | 报表条件比较的十进制阈值，与 Condition 运算符共同使用。 |
| [`DecimalPlaces`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L433) | `int` | 可写; CLR / n/a | `2` | 数值展示的小数位数；仅格式化，不更改源数据精度。 |
| [`FieldKey`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L409) | `string` | 可写; CLR / n/a | `""` | 显式注册的数据字段键；不是反射属性路径，必须与数据源字段一致。 |
| [`Format`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L427) | `TheOne.Avalonia.Reporting.ReportValueFormat` | 可写; CLR / n/a | `0 / default` | 字段显示格式枚举；自动、文本、数值、货币或日期，不改变源值。 可选值：`Automatic`, `Text`, `Number`, `Currency`, `Date` |
| [`Header`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L403) | `string` | 可写; CLR / n/a | `"新列"` | 标题区内容，可为文本或模型；自定义显示由 HeaderTemplate 负责。 |
| [`ShowSum`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L445) | `bool` | 可写; CLR / n/a | `false` | 在汇总区域显示此列数值合计；非数值字段按报表规则处理。 |
| [`Text`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L415) | `string` | 可写; CLR / n/a | `""` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |
| [`UseThousandsSeparator`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L439) | `bool` | 可写; CLR / n/a | `true` | 数值格式启用千位分隔，仅影响展示文本。 |
| [`WidthWeight`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L421) | `double` | 可写; CLR / n/a | `1` | 表格列的相对宽度权重；实际宽度按可用表格宽度分配。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportColumn.ReportColumn()`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L401) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportCondition`](TheOne.Avalonia.Reporting.ReportCondition.md)
- [`TheOne.Avalonia.Reporting.ReportValueFormat`](TheOne.Avalonia.Reporting.ReportValueFormat.md)
