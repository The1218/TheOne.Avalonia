# ReportPdfExporter

[English](../en/TheOne.Avalonia.Reporting.ReportPdfExporter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPdfExporter.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPdfExporter` · Class · `object`

Exports an immutable definition snapshot through the same drawing path as ReportPreview.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPdfExporter.cs#L11)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static Task ReportPdfExporter.ExportAsync(ReportDocument definition, ReportDataSource? source, Stream destination, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/ReportPdfExporter.cs#L38) | 完整生成分页 PDF 后再提交；取消在目标提交前生效，并发任务使用独立流。可读可寻址目标写入失败时恢复原内容、长度及位置。 |

覆盖非空可寻址只写流会抛出 `NotSupportedException`；同一流的并发任务抛出 `InvalidOperationException`。恢复失败时，`AggregateException` 同时包含写入与恢复错误。不可寻址流无法回退已接收字节。失败重试、取消时机及文件打开方式见 [ReportDesign 导出保护](../../controls/zh-cn/report-design.md#导出取消失败重试与目标占用)。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
