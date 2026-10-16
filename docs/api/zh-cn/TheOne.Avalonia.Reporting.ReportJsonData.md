# ReportJsonData

[English](../en/TheOne.Avalonia.Reporting.ReportJsonData.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportJsonData.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportJsonData` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool ReportJsonData.IsRootField(string key)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L11) | 判断字段键是否使用报表根级字段前缀。 |
| [`static object? ReportJsonData.ReadRootValue(string json, string pointer)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L13) | 从 JSON 根按指定路径读取标量字段。 |
| [`static IReadOnlyList<ReportDataNode> ReportJsonData.CreateTree(string json)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L22) | Builds a bounded response tree. Truncating display nodes never truncates the source rows. |
| [`static IReadOnlyList<ReportJsonBody> ReportJsonData.FindBodies(string json)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L57) | 枚举 JSON 中可作为报表明细记录源的数组及其路径。 |
| [`static ReportDataSource ReportJsonData.CreateSource(string json, string bodyPointer)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L73) | 从 JSON 记录数组路径和显式字段配置创建报表数据源。 |
| [`static ReportDataSource ReportJsonData.CreateSource(string json, string bodyPointer, IEnumerable<string>? rootFieldKeys)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L75) | 从 JSON 记录数组路径和显式字段配置创建报表数据源。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`const string ReportJsonData.RootFieldPrefix`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L10) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportDataNode`](TheOne.Avalonia.Reporting.ReportDataNode.md)
- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportJsonBody`](TheOne.Avalonia.Reporting.ReportJsonBody.md)
