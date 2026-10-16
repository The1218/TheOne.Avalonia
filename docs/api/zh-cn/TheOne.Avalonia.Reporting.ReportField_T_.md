# ReportField

[English](../en/TheOne.Avalonia.Reporting.ReportField_T_.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportField_T_.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportField<T>` · Class · `object`

Register the actual row accessor at compile time, including any nested property access.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L9)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Getter`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L9) | `System.Func<T, object?>` | 可写; CLR / init-only | `required constructor argument` | 显式类型化的字段读取委托；从原始行读取业务值，不执行字符串反射查找。 |
| [`Key`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L9) | `string` | 可写; CLR / init-only | `required constructor argument` | 字段／参数键，用于关联业务值；与显示名称分开维护。 |
| [`Kind`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L9) | `TheOne.Avalonia.Reporting.ReportValueKind` | 可写; CLR / init-only | `ReportValueKind.Text` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Text`, `Number`, `Date` |
| [`Label`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L9) | `string` | 可写; CLR / init-only | `required constructor argument` | 显示标签文本；不作为模型身份、字段路径或序列化标识。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportField<T>.ReportField(string Key, string Label, Func<T, object?> Getter, ReportValueKind Kind = ReportValueKind.Text)`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L9) | Register the actual row accessor at compile time, including any nested property access. |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportValueKind`](TheOne.Avalonia.Reporting.ReportValueKind.md)
