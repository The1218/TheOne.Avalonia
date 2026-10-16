# ReportFieldInfo

[English](../en/TheOne.Avalonia.Reporting.ReportFieldInfo.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportFieldInfo.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportFieldInfo` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L3)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Key`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L3) | `string` | 可写; CLR / init-only | `required constructor argument` | 字段／参数键，用于关联业务值；与显示名称分开维护。 |
| [`Kind`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L3) | `TheOne.Avalonia.Reporting.ReportValueKind` | 可写; CLR / init-only | `required constructor argument` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Text`, `Number`, `Date` |
| [`Label`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L3) | `string` | 可写; CLR / init-only | `required constructor argument` | 显示标签文本；不作为模型身份、字段路径或序列化标识。 |
| [`TypeLabel`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L5) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportFieldInfo.ReportFieldInfo(string Key, string Label, ReportValueKind Kind)`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L3) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportValueKind`](TheOne.Avalonia.Reporting.ReportValueKind.md)
