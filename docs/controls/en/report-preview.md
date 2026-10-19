# ReportPreview report viewer

[简体中文](../zh-cn/report-preview.md) · [Control documentation](README.md) · [ReportDesign](report-design.md)

`ReportPreview` is a standalone read-only viewer for JSON returned by `ReportDocument.Save()`. It parses the definition, fetches optional API data/network images, paginates and displays the report. It includes page navigation, zoom, reload, PDF export and printing, without a designer instance. Load `OneTheme` and initialize Avalonia for measurement/rendering.

```xml
<one:ReportPreview DocumentJson="{CompiledBinding ReportJson}"
                   DataSource="{CompiledBinding SourceOverride}" />
```

Declare the correct `x:DataType`; `one` is `using:TheOne.Avalonia.Controls`.

Templates may include [QRCode elements](report-design.md#qr-code-components). The viewer uses the same UTF-8 encoding, correction level, quiet zone and vector geometry as ReportDesign/PDF. Static `Text` is supported; bound `FieldKey` uses the first source record or a registered root JSON value. Missing/empty payloads, unresolved bound data, capacity overflow and insufficient size fail loading rather than displaying a stale or placeholder code. PreviewHost's sales sample includes both static and bound header codes.

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

PDF uses the already loaded data, image and pagination snapshot, with physical paper dimensions, vector text/shapes and embedded images. `ExportPdfAsync` and `PrintAsync` make no HTTP requests; refresh or load again to update resources. The export button uses the native save dialog. Printing exports a temporary PDF. Without a host `PrintHandler`, it delegates to `PrintTool` in `TheOne.Avalonia.Reporting`; this default submits to CUPS or invokes the Windows PDF print handler.

`PrintHandler` is an optional per-viewer CLR callback with signature `Func<string, ReportPrintOptions?, CancellationToken, Task<ReportPrintResult>>`. Its path remains valid until the task completes. Return `KeepDocument=false` after consuming the PDF, or true when an asynchronous printer still needs it. Exceptions and cancellation restore the print button and clean up non-retained temporary files.

PreviewHost references [TheOne.PrintingTools.Avalonia](https://github.com/The1218/TheOne.PrintingTools.Avalonia) `1.0.0-preview.3` and installs this callback on its ReportPreview sample. It opens that package's print settings / preview window; no job is submitted until the user confirms. The package theme is scoped to the print window, with the current Light/Dark variant and Small/Normal/Large density. The main control library has no dependency on the printing package; see [PreviewPrinting](../../../design/PreviewHost/Infrastructure/Printing/PreviewPrinting.cs) for the adapter.

`PrintTool.PrintPdfAsync(path, options, cancellationToken)` accepts an existing PDF. `CreateStartInfo(path, platform, options)` exposes invocation parameters without submitting a job.

| Platform | Invocation and requirements |
| --- | --- |
| Windows | Registered PDF application's `print` verb; requires a PDF handler supporting it. Only default options are supported: custom printer names/multiple copies throw `NotSupportedException`. Settings are managed by the handler. |
| Linux | CUPS `lp`; requires the command, a configured queue and print service access. Printer name and 1–999 copies are passed as separate arguments. |
| macOS | `/usr/bin/lp` through configured CUPS, with the same printer/copy options as Linux. |

`ReportPrintResult.Message` reports submission/handler invocation, not physical paper output. CUPS failures throw with command error output. Windows handlers may read the PDF after invocation returns, so `KeepDocument` is true; `ReportPreview.PrintAsync` retains the file and returns its path through `DocumentPath` and the message. It schedules cleanup after 24 hours of application runtime; later viewer initialization or printing also removes expired files. Application exit does not immediately delete retained files. `PrintTool.CleanupTemporaryDocuments()` can also clean expired library print files at application startup. CUPS temporary files are deleted after submission/failure. Direct `PrintTool` callers manage their own PDF lifetime.

Command-construction checks do not establish physical printing, Windows handler behavior or a Linux queue run. Verify the target platform's service and device in the consuming application. The default PrintTool path does not provide a print-settings dialog; PreviewHost uses the package preview described above.

## Integration and limits

PreviewHost → Extensions → ReportPreview loads saved JSON and an explicit 96-row sample source without external network access. The viewer fills the host content area.

JSON includes public endpoint/image URLs and body selection, excluding fetched rows, responses and HTTP client credentials. Credentials belong in the consumer client, not saved URLs. Generated JSON metadata supports NativeAOT. Formatting, totals, conditions and validation follow [ReportDesign's model](report-design.md#preview-and-update-contract). One repeating detail table is supported; the viewer has no report editing or request method/body editor.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### ReportPreview

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.ReportPreview.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CurrentPageNumber`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L126) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`DataSource`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L107) | `TheOne.Avalonia.Reporting.ReportDataSource?` | writable; OneWay | `null` | Explicit data source supplied by the host according to the model contract; templates do not replace data adaptation. |
| [`Document`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L113) | `TheOne.Avalonia.Reporting.ReportDocument?` | read-only; OneWay | `computed / state` | Current business document model; editor documentation defines subscription, selection and history behavior on replacement. |
| [`DocumentJson`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L101) | `string?` | writable; OneWay | `null` | Report document JSON; loading failures/replacement/data refresh follow the report loading contract. |
| [`Error`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L114) | `string?` | read-only; OneWay | `computed / state` | Structured/text failure information; inspect it together with the success flag. |
| [`HttpClient`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L118) | `System.Net.Http.HttpClient?` | writable; CLR / n/a | `null` | Optional consumer-owned client for authenticated API and image requests. Not serialized. |
| [`Layout`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L115) | `TheOne.Avalonia.Reporting.ReportLayout?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`PrintHandler`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L124) | `System.Func<string, TheOne.Avalonia.Reporting.ReportPrintOptions?, System.Threading.CancellationToken, System.Threading.Tasks.Task<TheOne.Avalonia.Reporting.ReportPrintResult>>?` | writable; CLR / n/a | `null` | Optional host printer. The PDF exists until the returned task completes; KeepDocument controls its subsequent lifetime. |
| [`Zoom`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L128) | `double` | writable; CLR / n/a | `.75` | View scale factor, where 1 means 100%; does not modify report millimetres or workflow coordinates. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`ReportPreview`](../../api/en/TheOne.Avalonia.Controls.ReportPreview.md) | `Task ReportPreview.LoadAsync(string json, CancellationToken cancellationToken = default(CancellationToken))` | Loads report JSON and associated data/images asynchronously, respecting cancellation, failures and stale-load isolation. |
| [`ReportPreview`](../../api/en/TheOne.Avalonia.Controls.ReportPreview.md) | `void ReportPreview.GoToPage(int page)` | Navigates to a requested page; paging/report contracts define indexing and range handling. |
| [`ReportPreview`](../../api/en/TheOne.Avalonia.Controls.ReportPreview.md) | `Task ReportPreview.ExportPdfAsync(Stream destination, CancellationToken cancellationToken = default(CancellationToken))` | Asynchronously exports the current report layout to PDF; see output/cancellation parameters. |
| [`ReportPreview`](../../api/en/TheOne.Avalonia.Controls.ReportPreview.md) | `Task<ReportPrintResult> ReportPreview.PrintAsync(ReportPrintOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))` | Invokes the configured print handler and awaits its result; the host chooses printing dependencies/submission policy. |
| [`PrintTool`](../../api/en/TheOne.Avalonia.Reporting.PrintTool.md) | `static ProcessStartInfo PrintTool.CreateStartInfo(string pdfPath, ReportPrintPlatform platform, ReportPrintOptions? options = null)` | Builds shell-free CUPS arguments, or the Windows registered PDF print verb. |
| [`PrintTool`](../../api/en/TheOne.Avalonia.Reporting.PrintTool.md) | `static Task<ReportPrintResult> PrintTool.PrintPdfAsync(string pdfPath, ReportPrintOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))` | Passes an existing PDF to the desktop print adapter and returns its result; see platform/file-retention rules. |
| [`PrintTool`](../../api/en/TheOne.Avalonia.Reporting.PrintTool.md) | `static void PrintTool.CleanupTemporaryDocuments()` | Removes print PDFs older than 24 hours, including files left by a previous process. Call at application startup if printing is infrequent. Only this library's temporary folders are inspected. |
| [`ReportDataSource`](../../api/en/TheOne.Avalonia.Reporting.ReportDataSource.md) | `object? ReportDataSource.GetValue(int rowIndex, string fieldKey)` | Reads an original-row value through an explicit field key/typed getter, without reflection paths. |
| [`ReportDocument`](../../api/en/TheOne.Avalonia.Reporting.ReportDocument.md) | `string ReportDocument.Save()` | Returns a portable JSON definition including public API/image URLs, but no fetched records. |
| [`ReportDocument`](../../api/en/TheOne.Avalonia.Reporting.ReportDocument.md) | `static ReportDocument ReportDocument.Load(string json)` | Loads a typed business document from JSON under its version/field/structure validation contract. |
| [`ReportJsonData`](../../api/en/TheOne.Avalonia.Reporting.ReportJsonData.md) | `static bool ReportJsonData.IsRootField(string key)` | Checks whether a field key uses the report root-field prefix. |
| [`ReportJsonData`](../../api/en/TheOne.Avalonia.Reporting.ReportJsonData.md) | `static object? ReportJsonData.ReadRootValue(string json, string pointer)` | Reads a scalar field from the JSON root using its path. |
| [`ReportJsonData`](../../api/en/TheOne.Avalonia.Reporting.ReportJsonData.md) | `static IReadOnlyList<ReportDataNode> ReportJsonData.CreateTree(string json)` | Builds a bounded response tree. Truncating display nodes never truncates the source rows. |
| [`ReportJsonData`](../../api/en/TheOne.Avalonia.Reporting.ReportJsonData.md) | `static IReadOnlyList<ReportJsonBody> ReportJsonData.FindBodies(string json)` | Finds JSON arrays and paths that can serve as report detail-record sources. |
| [`ReportJsonData`](../../api/en/TheOne.Avalonia.Reporting.ReportJsonData.md) | `static ReportDataSource ReportJsonData.CreateSource(string json, string bodyPointer)` | Creates a report source from a JSON record-array path and explicit field configuration. |
| [`ReportJsonData`](../../api/en/TheOne.Avalonia.Reporting.ReportJsonData.md) | `static ReportDataSource ReportJsonData.CreateSource(string json, string bodyPointer, IEnumerable<string>? rootFieldKeys)` | Creates a report source from a JSON record-array path and explicit field configuration. |
| [`ReportPaginator`](../../api/en/TheOne.Avalonia.Reporting.ReportPaginator.md) | `static ReportLayout ReportPaginator.Paginate(ReportDocument document, ReportDataSource? source = null)` | Validates the document and computes immutable paginated layout shared by preview/printing. |
| [`ReportPdfExporter`](../../api/en/TheOne.Avalonia.Reporting.ReportPdfExporter.md) | `static Task ReportPdfExporter.ExportAsync(ReportDocument definition, ReportDataSource? source, Stream destination, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))` | Stages a complete PDF; concurrent jobs require separate destination streams. |
| [`ReportResources`](../../api/en/TheOne.Avalonia.Reporting.ReportResources.md) | `static Task<string> ReportResources.FetchJsonAsync(ReportApiRequest request, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))` | Fetches one bounded JSON response using the same request path as the independent preview. |
| [`ReportResources`](../../api/en/TheOne.Avalonia.Reporting.ReportResources.md) | `static Task<ReportDataSource?> ReportResources.LoadDataAsync(ReportDocument document, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))` | Loads records asynchronously from report API configuration without persisting fetched rows in the template. |
| [`ReportResources`](../../api/en/TheOne.Avalonia.Reporting.ReportResources.md) | `static Task ReportResources.LoadImagesAsync(ReportDocument document, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))` | Resolves report images asynchronously with batch-owned caching/failure state. |
| [`ReportResources`](../../api/en/TheOne.Avalonia.Reporting.ReportResources.md) | `static Task ReportResources.LoadImagesAsync(ReportDocument document, HttpClient? client, CancellationToken cancellationToken, ReportDataSource? source)` | Resolves report images asynchronously with batch-owned caching/failure state. |
| [`ReportTemplate`](../../api/en/TheOne.Avalonia.Reporting.ReportTemplate.md) | `static ReportDocument ReportTemplate.CreateSalesDetail()` | A source-independent starting template matching the sales report design. |
| [`ReportTemplate`](../../api/en/TheOne.Avalonia.Reporting.ReportTemplate.md) | `static string ReportTemplate.Save(ReportDocument document)` | Saves portable business-document JSON, excluding runtime credentials, visual objects and transient state. |
| [`ReportTemplate`](../../api/en/TheOne.Avalonia.Reporting.ReportTemplate.md) | `static ReportDocument ReportTemplate.Load(string json)` | Loads a typed business document from JSON under its version/field/structure validation contract. |

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`ReportObservable`](../../api/en/TheOne.Avalonia.Reporting.ReportObservable.md) | `PropertyChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.QRCodeErrorCorrection`](../../api/en/TheOne.Avalonia.Controls.QRCodeErrorCorrection.md) | 4 |
| [`TheOne.Avalonia.Reporting.ReportApiAuthentication`](../../api/en/TheOne.Avalonia.Reporting.ReportApiAuthentication.md) | 7 |
| [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](../../api/en/TheOne.Avalonia.Reporting.ReportApiParameterMode.md) | 2 |
| [`TheOne.Avalonia.Reporting.ReportApiRequest`](../../api/en/TheOne.Avalonia.Reporting.ReportApiRequest.md) | 7 |
| [`TheOne.Avalonia.Reporting.ReportColumn`](../../api/en/TheOne.Avalonia.Reporting.ReportColumn.md) | 12 |
| [`TheOne.Avalonia.Reporting.ReportCondition`](../../api/en/TheOne.Avalonia.Reporting.ReportCondition.md) | 3 |
| [`TheOne.Avalonia.Reporting.ReportDataNode`](../../api/en/TheOne.Avalonia.Reporting.ReportDataNode.md) | 10 |
| [`TheOne.Avalonia.Reporting.ReportDataNodeKind`](../../api/en/TheOne.Avalonia.Reporting.ReportDataNodeKind.md) | 3 |
| [`TheOne.Avalonia.Reporting.ReportDataSource`](../../api/en/TheOne.Avalonia.Reporting.ReportDataSource.md) | 3 |
| [`TheOne.Avalonia.Reporting.ReportDocument`](../../api/en/TheOne.Avalonia.Reporting.ReportDocument.md) | 31 |
| [`TheOne.Avalonia.Reporting.ReportElement`](../../api/en/TheOne.Avalonia.Reporting.ReportElement.md) | 27 |
| [`TheOne.Avalonia.Reporting.ReportElementKind`](../../api/en/TheOne.Avalonia.Reporting.ReportElementKind.md) | 9 |
| [`TheOne.Avalonia.Reporting.ReportField<T>`](../../api/en/TheOne.Avalonia.Reporting.ReportField_T_.md) | 4 |
| [`TheOne.Avalonia.Reporting.ReportFieldInfo`](../../api/en/TheOne.Avalonia.Reporting.ReportFieldInfo.md) | 4 |
| [`TheOne.Avalonia.Reporting.ReportImageFit`](../../api/en/TheOne.Avalonia.Reporting.ReportImageFit.md) | 3 |
| [`TheOne.Avalonia.Reporting.ReportJsonBody`](../../api/en/TheOne.Avalonia.Reporting.ReportJsonBody.md) | 2 |
| [`TheOne.Avalonia.Reporting.ReportLayout`](../../api/en/TheOne.Avalonia.Reporting.ReportLayout.md) | 19 |
| [`TheOne.Avalonia.Reporting.ReportLayoutColumn`](../../api/en/TheOne.Avalonia.Reporting.ReportLayoutColumn.md) | 4 |
| [`TheOne.Avalonia.Reporting.ReportLayoutElement`](../../api/en/TheOne.Avalonia.Reporting.ReportLayoutElement.md) | 12 |
| [`TheOne.Avalonia.Reporting.ReportLayoutRow`](../../api/en/TheOne.Avalonia.Reporting.ReportLayoutRow.md) | 4 |
| [`TheOne.Avalonia.Reporting.ReportPage`](../../api/en/TheOne.Avalonia.Reporting.ReportPage.md) | 3 |
| [`TheOne.Avalonia.Reporting.ReportPrintOptions`](../../api/en/TheOne.Avalonia.Reporting.ReportPrintOptions.md) | 2 |
| [`TheOne.Avalonia.Reporting.ReportPrintPlatform`](../../api/en/TheOne.Avalonia.Reporting.ReportPrintPlatform.md) | 3 |
| [`TheOne.Avalonia.Reporting.ReportPrintResult`](../../api/en/TheOne.Avalonia.Reporting.ReportPrintResult.md) | 3 |
| [`TheOne.Avalonia.Reporting.ReportTextAlignment`](../../api/en/TheOne.Avalonia.Reporting.ReportTextAlignment.md) | 4 |
| [`TheOne.Avalonia.Reporting.ReportTextListStyle`](../../api/en/TheOne.Avalonia.Reporting.ReportTextListStyle.md) | 3 |
| [`TheOne.Avalonia.Reporting.ReportValueFormat`](../../api/en/TheOne.Avalonia.Reporting.ReportValueFormat.md) | 5 |
| [`TheOne.Avalonia.Reporting.ReportValueKind`](../../api/en/TheOne.Avalonia.Reporting.ReportValueKind.md) | 3 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
