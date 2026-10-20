# PrintTool

[English](../en/TheOne.Avalonia.Reporting.PrintTool.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.PrintTool.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.PrintTool` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L23)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CurrentPlatform`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L27) | `TheOne.Avalonia.Reporting.ReportPrintPlatform` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 可选值：`Windows`, `Linux`, `MacOS` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static ProcessStartInfo PrintTool.CreateStartInfo(string pdfPath, ReportPrintPlatform platform, ReportPrintOptions? options = null)`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L33) | Builds shell-free CUPS arguments, or the Windows registered PDF print verb. |
| [`static Task<ReportPrintResult> PrintTool.PrintPdfAsync(string pdfPath, ReportPrintOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L66) | 将现有 PDF 交给桌面打印适配器并返回结果；平台支持与保留文件规则见打印文档。 |
| [`static void PrintTool.CleanupTemporaryDocuments()`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L171) | Removes print PDFs older than 24 hours, including files left by a previous process. Call at application startup if printing is infrequent. Only this library's temporary folders are inspected. |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportPrintOptions`](TheOne.Avalonia.Reporting.ReportPrintOptions.md)
- [`TheOne.Avalonia.Reporting.ReportPrintPlatform`](TheOne.Avalonia.Reporting.ReportPrintPlatform.md)
- [`TheOne.Avalonia.Reporting.ReportPrintResult`](TheOne.Avalonia.Reporting.ReportPrintResult.md)
