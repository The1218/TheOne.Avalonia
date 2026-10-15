# ReportPreview report viewer

[简体中文](../zh-cn/report-preview.md) · [Control documentation](README.md) · [ReportDesign](report-design.md)

`ReportPreview` is a standalone read-only viewer for JSON returned by `ReportDocument.Save()`. It parses the definition, fetches optional API data/network images, paginates and displays the report. It includes page navigation, zoom, reload, PDF export and printing, without a designer instance. Load `OneTheme` and initialize Avalonia for measurement/rendering.

```xml
<one:ReportPreview DocumentJson="{Binding ReportJson}"
                   DataSource="{Binding SourceOverride}" />
```

Declare the correct `x:DataType`; `one` is `using:TheOne.Avalonia.Controls`.

## Loading definitions and data

Setting `DocumentJson` starts asynchronous loading. To await completion or supply cancellation, call `LoadAsync(json, cancellationToken)` on the UI thread. `Document`, `Layout` and `Error` expose the parsed definition, pagination snapshot and failure message; `Document` / `Error` are read-only Avalonia properties. Failed loads clear the displayed layout. `LoadAsync` sets `Error` and throws to its caller; property-driven loading displays errors.

```csharp
viewer.HttpClient = applicationHttpClient; // set before loading if needed
await viewer.LoadAsync(savedJson, cancellationToken);
viewer.GoToPage(2); // one-based and clamped
viewer.Zoom = 1;   // range: 0.25–2
```

The viewer requests `DataSourceUrl` using the saved `DataSourceMethod`, `DataSourceParameters` and `DataSourceParameterMode`; older templates default to GET. GET parameters extend the query string, while other methods send JSON or Form bodies. `DataBodyPointer` selects the detail collection. Saved absolute keys such as `$root/company` and `$root/orders/0/logo` restore values from the full response alongside collection-relative table fields. Explicit `DataSource` takes precedence over the saved endpoint, allowing typed or application-fetched records; it must expose the keys required by the template. Images use `ImageUrl`, or resolve an image element's `FieldKey` from the loaded source to obtain its current HTTP/HTTPS URL. Embedded `ImageData` remains supported. Checkboxes and Code 128B barcodes share the designer's binding and vector rendering. Without an endpoint or supplied source, static content and binding placeholders can be viewed.

The designer's response-tree display limits do not truncate preview records: all rows in the saved collection body are read. `ReportResources.LoadDataAsync` restores saved root binding keys automatically. Custom hosts can use `ReportJsonData.CreateSource(json, bodyPointer, rootFieldKeys)` and `ReportResources.LoadImagesAsync(document, client, cancellationToken, source)` to reproduce source-aware image loading.

When `DataSourceAuthentication` is set, the viewer performs its configured login request, extracts `TokenField`, adds `HeaderPrefix + token` under `HeaderName` to the data request, and then parses report data. Login settings include URL, method, JSON/Form parameters and token/header fields; see [ReportDesign](report-design.md). Every reload logs in afresh. Failed login/token extraction or cancellation stops before requesting data. Resolved tokens are not saved, shared client defaults are unchanged, and image downloads do not automatically receive this header. Explicit `DataSource` skips both login and API data fetching.

API and login responses are each limited to 4 MiB. Each image is limited to 8 MiB and 16 million pixels; remote images in one report are limited to 32 MiB and 32 million pixels in total. Overall loading cancels after 30 seconds, including login; the default HTTP client uses a 20-second request timeout. Set the optional CLR `HttpClient` property before loading for application authentication/custom transport. A supplied client remains consumer-owned and is not disposed or serialized.

Changing `DocumentJson` or `DataSource`, or clicking Reload, starts a new load of the requested definition and its required resources. Loading cancels its predecessor and resets to page one. `LoadAsync` also sets the definition used by Reload. Available fonts affect wrapping/fallback. Operate on the UI thread.

## PDF and printing

```csharp
await using var pdf = File.Create("report.pdf");
await viewer.ExportPdfAsync(pdf, cancellationToken);
ReportPrintResult result = await viewer.PrintAsync(
    new ReportPrintOptions(PrinterName: null, Copies: 1), cancellationToken);
```

PDF uses the already loaded data, image and pagination snapshot, with physical paper dimensions, vector text/shapes and embedded images. `ExportPdfAsync` and `PrintAsync` make no HTTP requests; refresh or load again to update resources. The export button uses the native save dialog. Printing exports a temporary PDF and delegates to `PrintTool` in `TheOne.Avalonia.Reporting`. Clicking Print explicitly submits to the default CUPS printer or invokes the Windows PDF print handler.

`PrintTool.PrintPdfAsync(path, options, cancellationToken)` accepts an existing PDF. `CreateStartInfo(path, platform, options)` exposes invocation parameters without submitting a job.

| Platform | Invocation and requirements |
| --- | --- |
| Windows | Registered PDF application's `print` verb; requires a PDF handler supporting it. Only default options are supported: custom printer names/multiple copies throw `NotSupportedException`. Settings are managed by the handler. |
| Linux | CUPS `lp`; requires the command, a configured queue and print service access. Printer name and 1–999 copies are passed as separate arguments. |
| macOS | `/usr/bin/lp` through configured CUPS, with the same printer/copy options as Linux. |

`ReportPrintResult.Message` reports submission/handler invocation, not physical paper output. CUPS failures throw with command error output. Windows handlers may read the PDF after invocation returns, so `KeepDocument` is true; `ReportPreview.PrintAsync` retains the file and returns its path through `DocumentPath` and the message. It schedules cleanup after 24 hours of application runtime; later viewer initialization or printing also removes expired files. Application exit does not immediately delete retained files. `PrintTool.CleanupTemporaryDocuments()` can also clean expired library print files at application startup. CUPS temporary files are deleted after submission/failure. Direct `PrintTool` callers manage their own PDF lifetime.

Command-construction checks do not establish physical printing, Windows handler behavior or a Linux queue run. Verify the target platform's service and device in the consuming application. The built-in Print button does not provide a cross-platform native print-settings dialog.

## Integration and limits

PreviewHost → Extensions → ReportPreview loads saved JSON and an explicit 96-row sample source without external network access. The viewer fills the host content area.

JSON includes public endpoint/image URLs and body selection, excluding fetched rows, responses and HTTP client credentials. Credentials belong in the consumer client, not saved URLs. Generated JSON metadata supports NativeAOT. Formatting, totals, conditions and validation follow [ReportDesign's model](report-design.md#preview-and-update-contract). One repeating detail table is supported; the viewer has no report editing or request method/body editor.
