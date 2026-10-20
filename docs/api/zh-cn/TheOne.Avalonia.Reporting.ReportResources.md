# ReportResources

[English](../en/TheOne.Avalonia.Reporting.ReportResources.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportResources.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportResources` · Class · `object`

Loads public HTTP resources with bounded responses. Hosts can supply an authenticated HttpClient.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportResources.cs#L9)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static Task<string> ReportResources.FetchJsonAsync(ReportApiRequest request, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/ReportResources.cs#L55) | Fetches one bounded JSON response using the same request path as the independent preview. |
| [`static Task<ReportDataSource?> ReportResources.LoadDataAsync(ReportDocument document, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/ReportResources.cs#L100) | 按报表 API 配置异步加载记录，不在序列化文档内保存返回记录。 |
| [`static Task ReportResources.LoadImagesAsync(ReportDocument document, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/ReportResources.cs#L110) | 异步解析报表外部图片，按加载批次维护缓存和失败状态。 |
| [`static Task ReportResources.LoadImagesAsync(ReportDocument document, HttpClient? client, CancellationToken cancellationToken, ReportDataSource? source)`](../../../TheOne.Avalonia/Reporting/ReportResources.cs#L116) | 异步解析报表外部图片，按加载批次维护缓存和失败状态。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportApiRequest`](TheOne.Avalonia.Reporting.ReportApiRequest.md)
- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
