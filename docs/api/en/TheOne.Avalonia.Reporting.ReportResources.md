# ReportResources

[English](../en/TheOne.Avalonia.Reporting.ReportResources.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportResources.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportResources` · Class · `object`

Loads public HTTP resources with bounded responses. Hosts can supply an authenticated HttpClient.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportResources.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static Task<string> ReportResources.FetchJsonAsync(ReportApiRequest request, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/ReportResources.cs#L43) | Fetches one bounded JSON response using the same request path as the independent preview. |
| [`static Task<ReportDataSource?> ReportResources.LoadDataAsync(ReportDocument document, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/ReportResources.cs#L68) | Loads records asynchronously from report API configuration without persisting fetched rows in the template. |
| [`static Task ReportResources.LoadImagesAsync(ReportDocument document, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Reporting/ReportResources.cs#L75) | Resolves report images asynchronously with batch-owned caching/failure state. |
| [`static Task ReportResources.LoadImagesAsync(ReportDocument document, HttpClient? client, CancellationToken cancellationToken, ReportDataSource? source)`](../../../TheOne.Avalonia/Reporting/ReportResources.cs#L78) | Resolves report images asynchronously with batch-owned caching/failure state. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportApiRequest`](TheOne.Avalonia.Reporting.ReportApiRequest.md)
- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
