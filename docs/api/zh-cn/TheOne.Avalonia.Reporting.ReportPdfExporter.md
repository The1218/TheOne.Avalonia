# ReportPdfExporter

[English](../en/TheOne.Avalonia.Reporting.ReportPdfExporter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPdfExporter.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPdfExporter` · Class · `object`

Exports an immutable definition snapshot through the same drawing path as ReportPreview.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPdfExporter.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static Task ReportPdfExporter.ExportAsync(ReportDocument definition, ReportDataSource? source, Stream destination, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/ReportPdfExporter.cs#L12) | 异步输出分页报表 PDF；调用方提供目标并遵守取消与文件生命周期。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
