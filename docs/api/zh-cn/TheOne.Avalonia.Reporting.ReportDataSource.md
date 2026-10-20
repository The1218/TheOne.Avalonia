# ReportDataSource

[English](../en/TheOne.Avalonia.Reporting.ReportDataSource.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportDataSource.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportDataSource` · Class · `object`

A row snapshot with explicitly typed accessors. Replace the source to refresh changed data.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L16)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Fields`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L33) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportFieldInfo>` | 只读; CLR / n/a | `accessor-defined / state` | 端口公开的输出字段元数据；允许宿主描述可引用的数据结构。 |
| [`Name`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L31) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 名称字段，用于业务展示或标识；与显示模板、键值字段保持一致。 |
| [`RowCount`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L32) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`object? ReportDataSource.GetValue(int rowIndex, string fieldKey)`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L35) | 通过显式字段键／类型化 getter 从原始行读取值；不使用反射路径。 |
| [`static ReportDataSource ReportDataSource.Create<T>(string name, IEnumerable<T> rows, params ReportField<T>[] fields)`](../../../TheOne.Avalonia/Reporting/ReportDataSource.cs#L58) | 通过显式类型参数、模型及委托创建适配器／数据源；签名中的 getter 保留编译类型信息。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportField<T>`](TheOne.Avalonia.Reporting.ReportField_T_.md)
- [`TheOne.Avalonia.Reporting.ReportFieldInfo`](TheOne.Avalonia.Reporting.ReportFieldInfo.md)
