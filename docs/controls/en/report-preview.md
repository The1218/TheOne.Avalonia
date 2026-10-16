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
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost's Control Laboratory (top-right action on each page) groups live examples by styles, properties, APIs/events and models; complex models use the complete business samples.

### ReportPreview

[API / model reference](../../api/en/TheOne.Avalonia.Controls.ReportPreview.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CurrentPageNumber`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L29) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`DataSource`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L21) | `TheOne.Avalonia.Reporting.ReportDataSource?` | writable; OneWay | `null` | Explicit data source supplied by the host according to the model contract; templates do not replace data adaptation. |
| [`Document`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L22) | `TheOne.Avalonia.Reporting.ReportDocument?` | read-only; OneWay | `computed / state` | Current business document model; editor documentation defines subscription, selection and history behavior on replacement. |
| [`DocumentJson`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L20) | `string?` | writable; OneWay | `null` | Report document JSON; loading failures/replacement/data refresh follow the report loading contract. |
| [`Error`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L23) | `string?` | read-only; OneWay | `computed / state` | Structured/text failure information; inspect it together with the success flag. |
| [`HttpClient`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L26) | `System.Net.Http.HttpClient?` | writable; CLR / n/a | `null` | Optional consumer-owned client for authenticated API and image requests. Not serialized. |
| [`Layout`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L24) | `TheOne.Avalonia.Reporting.ReportLayout?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`PrintHandler`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L28) | `System.Func<string, TheOne.Avalonia.Reporting.ReportPrintOptions?, System.Threading.CancellationToken, System.Threading.Tasks.Task<TheOne.Avalonia.Reporting.ReportPrintResult>>?` | writable; CLR / n/a | `null` | Optional host printer. The PDF exists until the returned task completes; KeepDocument controls its subsequent lifetime. |
| [`Zoom`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L30) | `double` | writable; CLR / n/a | `.75` | View scale factor, where 1 means 100%; does not modify report millimetres or workflow coordinates. |

### Shared attached properties

[ControlSizing](../../api/en/TheOne.Avalonia.Styling.ControlSizing.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | writable; OneWay; attached; inherits | `ControlSize.Normal` | Shared Small/Normal/Large density inherited by descendants; explicit local values/classes can override it. Values: `Small`, `Normal`, `Large` |

[Motion](../../api/en/TheOne.Avalonia.Styling.Motion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | writable; OneWay; attached; inherits | `true` | Inherited motion switch; disabling goes to final layout/state while preserving native input. |

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.ReportPreview`](../../api/en/TheOne.Avalonia.Controls.ReportPreview.md) | Class | 9 | 5 | 0 |
| [`TheOne.Avalonia.Reporting.PrintTool`](../../api/en/TheOne.Avalonia.Reporting.PrintTool.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Reporting.ReportApiAuthentication`](../../api/en/TheOne.Avalonia.Reporting.ReportApiAuthentication.md) | Class | 7 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](../../api/en/TheOne.Avalonia.Reporting.ReportApiParameterMode.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportApiRequest`](../../api/en/TheOne.Avalonia.Reporting.ReportApiRequest.md) | Class | 5 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportColumn`](../../api/en/TheOne.Avalonia.Reporting.ReportColumn.md) | Class | 12 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportCondition`](../../api/en/TheOne.Avalonia.Reporting.ReportCondition.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDataNode`](../../api/en/TheOne.Avalonia.Reporting.ReportDataNode.md) | Class | 9 | 0 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDataNodeKind`](../../api/en/TheOne.Avalonia.Reporting.ReportDataNodeKind.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDataSource`](../../api/en/TheOne.Avalonia.Reporting.ReportDataSource.md) | Class | 3 | 2 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDocument`](../../api/en/TheOne.Avalonia.Reporting.ReportDocument.md) | Class | 29 | 3 | 0 |
| [`TheOne.Avalonia.Reporting.ReportElement`](../../api/en/TheOne.Avalonia.Reporting.ReportElement.md) | Class | 25 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportElementKind`](../../api/en/TheOne.Avalonia.Reporting.ReportElementKind.md) | Enum | 8 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportField<T>`](../../api/en/TheOne.Avalonia.Reporting.ReportField_T_.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportFieldInfo`](../../api/en/TheOne.Avalonia.Reporting.ReportFieldInfo.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportImageFit`](../../api/en/TheOne.Avalonia.Reporting.ReportImageFit.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportJsonBody`](../../api/en/TheOne.Avalonia.Reporting.ReportJsonBody.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportJsonData`](../../api/en/TheOne.Avalonia.Reporting.ReportJsonData.md) | Class | 0 | 6 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayout`](../../api/en/TheOne.Avalonia.Reporting.ReportLayout.md) | Class | 19 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayoutColumn`](../../api/en/TheOne.Avalonia.Reporting.ReportLayoutColumn.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayoutElement`](../../api/en/TheOne.Avalonia.Reporting.ReportLayoutElement.md) | Class | 12 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayoutRow`](../../api/en/TheOne.Avalonia.Reporting.ReportLayoutRow.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportObservable`](../../api/en/TheOne.Avalonia.Reporting.ReportObservable.md) | Class | 0 | 0 | 1 |
| [`TheOne.Avalonia.Reporting.ReportPage`](../../api/en/TheOne.Avalonia.Reporting.ReportPage.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPaginator`](../../api/en/TheOne.Avalonia.Reporting.ReportPaginator.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPdfExporter`](../../api/en/TheOne.Avalonia.Reporting.ReportPdfExporter.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPrintOptions`](../../api/en/TheOne.Avalonia.Reporting.ReportPrintOptions.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPrintPlatform`](../../api/en/TheOne.Avalonia.Reporting.ReportPrintPlatform.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPrintResult`](../../api/en/TheOne.Avalonia.Reporting.ReportPrintResult.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportResources`](../../api/en/TheOne.Avalonia.Reporting.ReportResources.md) | Class | 0 | 4 | 0 |
| [`TheOne.Avalonia.Reporting.ReportTemplate`](../../api/en/TheOne.Avalonia.Reporting.ReportTemplate.md) | Class | 0 | 3 | 0 |
| [`TheOne.Avalonia.Reporting.ReportTextAlignment`](../../api/en/TheOne.Avalonia.Reporting.ReportTextAlignment.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportTextListStyle`](../../api/en/TheOne.Avalonia.Reporting.ReportTextListStyle.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportValueFormat`](../../api/en/TheOne.Avalonia.Reporting.ReportValueFormat.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportValueKind`](../../api/en/TheOne.Avalonia.Reporting.ReportValueKind.md) | Enum | 3 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
