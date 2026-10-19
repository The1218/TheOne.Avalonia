# ReportDesign report designer

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

[简体中文](../zh-cn/report-design.md) · [Control documentation](README.md) · [ReportPreview](report-preview.md)

`ReportDesign` is a desktop report designer for free positioning. A data source is optional: design static reports before connecting an API. Load `OneTheme`; editor surfaces use `One.*` resources and the three control densities. Printed paper stays white and keeps its real aspect ratio.

```xml
<one:ReportDesign Document="{Binding Report}"
                  DataSource="{Binding SalesSource}"
                  SaveCommand="{Binding SaveReportCommand}" />
```

Declare the view's correct `x:DataType`; `one` is `using:TheOne.Avalonia.Controls`. Models are in `TheOne.Avalonia.Reporting`.

## Workbench and editing

The report-name header uses a native `TabStrip`, with the document title, built-in close action, saved state and document actions in the top bar above the formatting toolbar. It retains the native tab item's content layout while removing duplicate border and spacing around the strip. Closing its single document clears `Document` and `DataSource`, disables Save/PDF export and leaves New/Open available to restore a document. The component/data sidebar spans the work area vertically; design/preview tabs sit above the paper area. The compact inspector has Properties and Data tabs, with grouped geometry, content, typography and colors in Properties. Component/data (`SourceTabs`), Properties/Data (`PropertyTabs`) and design/preview (`ViewTabs`) all use the built-in `TabControl` with `Classes="card"`, without custom tab templates, retaining native keyboard behavior. These fixed editor pages explicitly disable `TabFeatures.CanClose` and `TabFeatures.CanReorder`, so they cannot be closed or reordered. The report-name strip uses [TabFeatures closing](collections.md#standalone-tabstrip).

Drag text, a table, an image, a line, a rectangle, a page number, a checkbox, a barcode or a QR code onto paper. The palette uses three columns, 30 DIP minimum button height, 6 × 3 DIP padding, 14 DIP icons and 12 DIP text. This compact tool layout stays consistent across the three editor densities; new entries add natural rows, while the structure tree receives the remaining height. Drag an element to move it, or its bottom-right handle to resize it. A table also has an outer right-edge handle for its overall width; this changes the table width while keeping column proportions. Internal column boundaries adjust neighboring widths, the header/detail boundary changes header height, and the bottom boundary changes detail template height. Columns store relative `WidthWeight`; `TableHeaderHeightMm` sets header height, subject to text measurement.

Geometry uses mm; optional snapping uses 1 mm. Arrow keys move by 1 mm (Shift: 5 mm); Delete removes the selection. Undo/redo records a canvas drag as one edit. The toolbar edits font family, size, bold, italic, underline, alignment and lists. The inspector and border/fill dropdowns use the built-in `ColorPicker`. The font editor and `FontSizeValue` use print points; `ReportElement.FontSize` uses DIPs: `pt = DIP × 0.75`. New elements default to 12 pt (16 DIPs).

The font-family dropdown lists local system fonts through Avalonia's `FontManager`, rather than a fixed preset list. The current document font remains selectable even when it is not installed locally; rendering then follows available font fallback. Saving a font name does not embed that font in the template.

Select an element on paper or in the structure tree; click a table template cell to edit its column. The tree groups title, header, detail, summary and footer elements. Clicking a structure item switches to design mode and scrolls its element, cell or region into view. Selecting a child keeps its parent expanded; element selection updates tree selection without rebuilding the hierarchy. Rebuilds caused by renaming or structural edits preserve existing expansion state. One document currently supports one repeating detail table and multiple other elements.

Right-click a table column in the design page for **Insert column left**, **Insert column right** and **Delete column**. Pointer requests target the hit column rather than a previously selected column; keyboard context requests target the current selection. Inserts create and select a blank column; deletion selects an adjacent column. Each action is one undoable edit, retaining other columns' bindings and formatting. Keep at least one and at most 100 columns; insertion is disabled if any resulting column would be narrower than 32 DIPs. Preview and non-table positions do not offer the column menu. Changing documents or entering preview closes an old menu.

A new column inherits the target column's `WidthWeight`; existing weights remain unchanged on insertion or deletion. Actual width is `table width × column weight / sum of all column weights`, keeping the table's overall width unchanged. Existing columns therefore shrink proportionally on insertion and grow on deletion; their absolute widths are not retained. The inspector's Add Column appends a column, inherits the selected weight and uses the pending field. It enforces the same count and minimum-width limits as the context menu. Delete Column also keeps the last remaining column. Deletion selects the next column, or the preceding column when deleting the last one.

Undo restores the selection before the edit; redo restores the selection after it. Field bindings, static text, formatting, decimal places, sums and conditional styles follow the restored column instead of the changed index. History preserves the `Document` instance but replaces column and element instances; consumers should obtain current instances from `SelectedColumn` / `SelectedElement`. Templates persist order and weights, excluding selection and runtime rows. Reload initializes selection and restores API data or requires the consumer to reassign an explicit `DataSource`. Design, preview and PDF share column order, proportional widths and field resolution.

## QR code components

Drag **QRCode** from the palette, or call `AddElement(ReportElementKind.QRCode, x, y)` with millimetre coordinates. New codes are 32 × 32 mm with a static example URL. The inspector edits `Text`, `QRCodeErrorCorrection` (default `Medium`) and `QRCodeQuietZone` (4–32 modules, default 4). Foreground controls dark modules; the code area stays white, protecting the quiet zone from a page fill. Non-square elements center a square symbol rather than stretching it. Keep dark modules and strong contrast.

```csharp
var qr = designer.AddElement(ReportElementKind.QRCode, 150, 16);
qr.Text = "https://example.com/report"; // Static payload, including Unicode
qr.Width = qr.Height = 32;
qr.QRCodeErrorCorrection = QRCodeErrorCorrection.High;
qr.QRCodeQuietZone = 4;

// After assigning a source with a registered OrderNo field:
designer.SelectedElement = qr;
designer.BindField("OrderNo"); // Or drop that field onto the QR code
// qr.FieldKey = "" returns to its static Text.
```

`FieldKey` takes precedence over `Text` and resolves the source's first record, or an explicitly registered root JSON value. It does not generate one code per detail row. Changing the source or refreshing row values replaces the payload; missing fields, empty resolved values, invalid Unicode, capacity overflow and modules below 1 DIP report validation errors. A bound template can be saved without runtime data; design displays a pending-data placeholder, while preview and PDF require actual nonempty field data. Field placeholders are never encoded as payloads.

Saving and reloading preserves `FieldKey`, static `Text`, correction and quiet zone, but does not serialize runtime rows. Loading clears the previous source; a saved API configuration refetches its data, while a template without an endpoint needs its runtime `DataSource` assigned again. Replacing that source never rewrites the binding or static text. After changing an existing row object's value, call `RefreshReport()` to rebuild the QR payload. Row membership is captured when the source is created; replace `DataSource` after adding, removing or replacing rows. Clear `FieldKey` to return to the preserved static text.

```csharp
var template = designer.SaveTemplate();
designer.LoadTemplate(template); // No endpoint: runtime rows must be supplied again
designer.DataSource = ReportDataSource.Create("Orders", rows,
    new ReportField<Order>("OrderNo", "Order number", row => row.OrderNo));
rows[0].OrderNo = "SO-2026-002";
designer.RefreshReport(); // Re-read current row values on the UI thread
```

The data sidebar's **Refresh DataSource** action fetches the API and replaces the bound record snapshot using the saved collection pointer and root field keys. It retains QR bindings and static text. A response arriving after the document, request configuration or runtime source was replaced cannot overwrite the replacement. A successful response with an empty or missing QR value clears the old symbol and blocks preview/PDF; a failed HTTP request retains the existing source, displays an error and allows retry.

Multi-page reports repeat each QR element at its defined position. A bound code still uses the first record or its registered root value on every page; it does not follow the first detail row of the current page. Changing pages reuses the current layout and encoded symbols. PDF export includes all pages regardless of the selected preview page and leaves that page selection unchanged. Each export captures the definition, resolved QR payloads and pagination before waiting for network images; editing shared row objects or starting another export during that wait cannot change its QR content. For batch exports, set the source and any static text for each job, then await its export before reusing the destination stream. Separate overlapping jobs need separate streams.

Design, preview and vector PDF share the same UTF-8/ECI encoder, exact correction level, square layout and quiet zone. The immutable layout retains encoded geometry, so selection and drawing do not re-encode it. Template JSON and undo/redo retain the payload, binding, geometry in mm, correction and quiet-zone settings; QR matrices and source rows are not serialized. `QRCode = 8` is appended to `ReportElementKind`, retaining existing enum values; older library versions cannot read templates containing the new kind.

PreviewHost's ReportDesign sales sample displays a static link and a first-order field code side by side in the header. The standalone QRCode page shows property examples in `Description`. See [QRCode](qr-code.md) for payload limits, rendering sizes and scanning considerations.

Binding a QR code through the field picker, `BindField`, a typed field drop or a JSON scalar drop preserves its static `Text`. Switching fields also preserves that fallback. While a field is bound, editing `Text` changes only the fallback; the displayed code continues to use the field. Use **Restore static content** in the QR inspector, or clear `FieldKey`, to encode the latest static text. Undo/redo restores both the binding and fallback, including edits made while bound. Invalid or empty active content clears the previous code and prevents PDF export until corrected; it does not silently fall back to the last valid payload.

## Page settings and fixed regions

Page Settings opens an owned modal dialog with grouped paper/orientation, four-margin and fixed-region editors, a live proportional paper preview and inline validation. It supports A4, A3, Letter and custom dimensions. The window inherits the host's actual theme/density; its form scrolls while primary/secondary actions remain at the bottom. Apply validates a draft before changing the document; Cancel or closing the window preserves existing settings.

`PageWidthMm` / `PageHeightMm` are physical paper dimensions. Nullable `MarginLeftMm`, `MarginTopMm`, `MarginRightMm` and `MarginBottomMm` individually override the legacy uniform `MarginMm`.

The header begins at `TopMargin` and has `HeaderHeightMm`. The footer begins at `PageHeightMm - BottomMargin - FooterHeightMm`. The canvas draws these known region boundaries and aligns ruler labels to them independently of table position. In design mode, drag the left-side header/footer boundary grip vertically to change the region height; footer elements move with the footer boundary. Pagination reserves the footer region; fixed elements and table headers repeat across pages. The regions must leave room for detail content. A4 scrolls without stretching into the viewport.

## Network images and MVVM JSON saving

For server images, set `ReportElement.ImageUrl` to an HTTP/HTTPS URL, or enter it in the image's Properties section and click Load Network Image. Downloaded bytes are runtime caches excluded from JSON; `ImageUrl` is saved for the viewer to fetch again. Local PNG, JPEG, WebP and BMP import via `SetImage` remains available as embedded `ImageData`. `ReportImageFit` supports contain, cover/crop and fill. Limits are 8 MiB and 16 million pixels per image, with 32 MiB and 32 million pixels across remote images in a report. Invalid local replacements preserve the existing image; a failed new network URL shows a loading/failure placeholder in design mode instead of displaying the old URL's cached image. Image prompts are design-only.

`SaveCommand` is an `ICommand`. The Save button passes the current `ReportDocument` and respects `CanExecute`. Call `document.Save()` in the consumer's command and persist the returned JSON to your server. Command execution alone does not mark the report saved: call `designer.MarkSaved()` after persistence succeeds. Without a command, Save uses the native local file dialog. Open reads a local template; New creates a blank report.

```csharp
// In the consumer's save-command handler:
string json = document.Save();
// Await the application's repository or HTTP save operation here.
designer.MarkSaved(); // only after successful persistence

// Later, after retrieving the stored JSON:
ReportDocument restored = ReportDocument.Load(json);
designer.Document = restored;
```

`ReportDocument.Save()` / static `Load(json)` wrap versioned `ReportTemplate` JSON. It preserves paper, margins, region heights, elements, fonts, columns, bindings, formats, totals, conditions, embedded images and public `DataSourceUrl`, `DataBodyPointer`, `DataSourceAuthentication` and `ImageUrl` values. It excludes fetched rows, API responses, runtime image caches, HTTP clients and resolved tokens. Login parameters are saved definition data; applications that keep credentials outside templates can supply a consumer-owned authenticated client to the standalone viewer.

Invalid templates are rejected before replacing the document. Limits are 32 MiB, 1000 elements, 100 columns and one table. Serialization uses generated metadata for NativeAOT. `SaveTemplate()` / `LoadTemplate(json)` remain available. Loading clears the previous runtime source, then automatically uses the saved endpoint, method, parameters, authentication and collection pointer to restore bound data and the response tree. No field dropping is needed again. Use `await LoadTemplateAsync(json, cancellationToken)` when the host needs to await data and bound network images, for example before export. Network/field failures retain the loaded definition and bindings, show an error, and propagate to explicit async callers. Source-free templates remain static.

```csharp
var picture = designer.AddElement(ReportElementKind.Image, 150, 30);
picture.ImageUrl = "https://reports.example.com/logo.png";
picture.ImageFit = ReportImageFit.Contain;
var json = designer.Document!.Save();
await designer.LoadTemplateAsync(json);
await using var pdf = new FileStream("report.pdf", FileMode.OpenOrCreate,
    FileAccess.ReadWrite, FileShare.None);
await designer.ExportPdfAsync(pdf); // UI thread; every page
```

PDF shares pagination and Avalonia/Skia drawing with preview. Text and shapes stay vector; images are embedded. Physical mm map to 72 points per inch, excluding rulers, handles and editor chrome. Source-free reports export placeholders; invalid layouts block export. Available system fonts can affect wrapping and glyph fallback.

## Optional API data and binding

Click Add Data Source in the Data tab. The dialog accepts an HTTP/HTTPS URL, GET / POST / PUT / PATCH / DELETE, and parameters as a JSON object (64 KiB maximum). GET encodes them into the existing URL query; other methods send JSON or Form data. Nested Form values become JSON strings. Requests use a 20-second timeout and a 4 MiB response limit.

The Data tab uses the full left sidebar: its response tree fills the height below the endpoint controls and search box. The report structure appears only in the Components tab, beneath the compact palette, and retains its selection when switching tabs. Switching Components/Data also retains data-tree expansion, selection, scroll offset and search. It does not refetch data. Filtered roots are reused until the source or query changes, so refreshes do not discard selection.

An assigned saved `Document` also loads its optional API data when the designer is attached, unless the application has supplied its own source. Assigning `DataSource` during automatic loading takes precedence and cancels that pending fetch. Closing/replacing the document cancels older requests; late responses cannot replace a newer report. Detaching interrupts an in-flight automatic load and reattachment restarts it; an already loaded source survives cached navigation without another data request. Automatic loading has a 30-second overall limit, keeps runtime data outside template JSON and does not create an undoable definition edit.

The dialog uses `VerificationStep`. Without **Requires authentication**, the two steps are **Data interface → Select data**. Enabling it creates four automatically numbered steps: **Authentication interface → Header and Token → Data interface → Select data**. Next validates and calls the current endpoint as needed; the page's call button can also preview the response first. Editing upstream settings invalidates dependent results. Cancel/close aborts pending requests and preserves the existing source. Confirm imports the validated response without another HTTP request.

Going back and forward without edits reuses the validated response and retains the current selection/search. Editing endpoints, parameters, headers or the token path clears dependent data responses, choices and search. Fresh results default to all available items, without reusing the previous configuration's filter. Saved template choices are restored only on the wizard's initial load of the same configuration. Toggling authentication clears the login response, token candidates/highlight and header preview; enabling it again requires a new login call. Editing a token path synchronizes the candidate highlight, clearing it for an invalid path. Closing also releases the runtime login response.

Leaving a step with a pending request or cancelling validation aborts that operation and immediately restores inputs. Retrying starts a fresh request. Invalidation observes Text synchronously so deferred TextChanged events cannot cancel newer work. Each operation captures its own cancellation source and checks its generation; late responses, including those ignoring cancellation, cannot repopulate old data, tokens or choices, or cancel a newer retry.

The final step names the source and selects **Scalar fields / Datasets**. Each scalar property is one item and each entire array is one item; objects only organize paths. Search preserves hidden selections. A nonempty name and at least one selected item are required. Confirmation saves `ReportDocument.DataSourceName` and `SelectedDataPointers` (JSON Pointers), including undo and template persistence. A null selection in older templates displays all items. The sidebar groups a named source into scalar fields and datasets; expanding a dataset shows its field schema rather than individual records. JSON text preview is limited to 200,000 characters, while the complete response and existing report bindings remain available. Unselected items are hidden from the designer catalog, not removed from the underlying data.

The editor still manages one API source. Confirming another configuration replaces its catalog/settings without clearing existing field bindings. Refresh refetches the current configuration while retaining its name and selection; template reload still fetches the complete response.

Drag a scalar tree node onto text or a particular table cell to fill its current value and retain its binding; dropping onto blank paper creates bound text. A scalar containing an HTTP/HTTPS image URL can be dropped onto an image component to load and bind that image. Objects organize the tree and are not dropped as values.

Drop a collection onto an existing table to open the modal field-mapping dialog. It shows each existing column, a field picker and first-record samples, with native scrolling for long lists. Confirming sets the collection as the detail source and applies all mappings as one undoable edit. Unselected columns keep static `ReportColumn.Text` and clear previous field bindings; nonnumeric/unbound columns also disable sums and numeric conditions. Cancel or closing the dialog changes neither columns nor the source. Collections cannot be dropped onto text or blank paper.

The chosen endpoint and collection JSON Pointer are saved as `DataSourceUrl` and `DataBodyPointer`. Absolute scalar keys such as `$root/company` or `$root/orders/0/logo` can coexist with fields relative to the chosen collection. Saving JSON preserves those binding keys; standalone preview refetches the response and restores root values, table records and bound image URLs together. Escaped JSON Pointer keys keep dotted/slashed property names and actual array indices unambiguous. Failed fetches preserve the current source and design.

Binding also saves `DataSourceMethod`, `DataSourceParameters` and `DataSourceParameterMode` for standalone preview. The GET overload `FetchApiDataAsync(url, cancellationToken)` remains available; use `FetchApiDataAsync(new ReportApiRequest(url, "POST", "{\"page\":1}"), cancellationToken)` for configured requests. `ReportResources.FetchJsonAsync(request, client, cancellationToken)` accepts a consumer-owned `HttpClient`.

The **Authentication interface** step calls only the login endpoint, independently of the data URL and before choosing a token. It supports request methods and JSON/Form parameters. **Header and Token** selects a string/number leaf from the login response, or accepts a manual path, then configures the editable header name and prefix. Invalid paths and empty tokens block progress. Login-response strings and the header preview are masked. The data step reuses this validated login response without logging in again; editing login settings requires returning to the first step. The body scrolls independently of Cancel, Previous and Confirm.

The editable header selector offers Authorization, X-API-Key, X-Auth-Token, Token, Access-Token and Auth-Token, and accepts custom names. It uses native `AutoCompleteBox` with a dropdown button. Token paths accept JSON Pointer (`/data/token`, including `~0`/`~1` escapes) or dotted paths (`data.token`). The value must be a nonempty string or number. Header value is the optional prefix plus that field; `Bearer ` includes a trailing space. Leave the prefix empty for raw tokens.

```csharp
var request = new ReportApiRequest("https://reports.example.com/rows")
{
    Authentication = new ReportApiAuthentication
    {
        Url = "https://reports.example.com/login", Method = "POST",
        Parameters = "{\"username\":\"example\",\"password\":\"example\"}",
        ParameterMode = ReportApiParameterMode.Json,
        TokenField = "/data/token", HeaderName = "Authorization", HeaderPrefix = "Bearer "
    }
};
await designer.FetchApiDataAsync(request);
```

Confirming a source or binding a fetched node saves settings in `ReportDocument.DataSourceAuthentication`, including undo/redo; `null` requests data directly. Public fetch APIs, template reload and standalone preview still perform login → token extraction → data request. Login failure, invalid tokens and cancellation stop the chain. Only the data request receives the header; shared client defaults and separate image requests are unchanged. Runtime tokens are not serialized, and reloading logs in afresh. Login and data responses each have the same 4 MiB/20-second limits. The editor supports one token header, without interactive OAuth, token refresh or multiple authentication headers.

Configured requests can select the same catalog presentation in code:

```csharp
await designer.FetchApiDataAsync(new ReportApiRequest("https://example.com/report")
{
    SourceName = "Sales report",
    SelectedDataPointers = ["/reportTitle", "/sales"]
});
```

## Checkbox and barcode components

Drag `ReportElementKind.CheckBox` or `Barcode` from the palette. Both share positioning, resizing, structure navigation and undo. Checkbox `Text` is its caption and `IsChecked` its default state. Bound fields take precedence and accept booleans, 1/0, yes/no, 是/否 or an empty unchecked value; unsupported values block preview/export. Dropping a boolean preserves the caption.

Barcodes use Code 128B: `Text` is a static value or is resolved through `FieldKey`; `BarcodeShowText` controls the readable caption. Values contain 1–128 printable ASCII characters, with checksum, stop and ten-module quiet zones on both sides. Chinese text, GS1/FNC and other symbologies are unsupported. Module width must be at least 0.75 DIP (about 0.20 mm), with invalid sizes reported. Both components use vector drawing in template JSON, standalone preview and PDF. Consumers should verify their printer resolution and scanning devices.

The designer catalog supports 16 object levels, 4096 selectable items and 4096 distinct fields per dataset. Exceeding a limit reports an error and preserves the previous source. Dataset schemas union fields across all records and use each field's first actual JSON Pointer; they do not create per-record nodes. Public `ReportJsonData.CreateTree(json)` retains its indexed-tree contract (the first 200 array indices and approximately 4000 display nodes). Collection bindings still read all records and restore explicitly saved root bindings as needed.

The source search box and TreeView use native Small sizing independently of the host density. Nodes show an icon, name, type and value or row/field-count summary. Long content is ellipsized, with the name, JSON Pointer and preview in the tooltip. The catalog uses native StackPanel item panels and one physical scrolling viewport, avoiding nested virtualizing panels losing expanded content while scrolling. Horizontal overflow is clipped to the sidebar width. Successful import/binding messages are hidden above the tree; failed fetch/binding messages remain visible.

`FetchApiDataAsync(url, cancellationToken)` populates the tree. Public `ReportDataNode` exposes `Kind` (`Scalar`, `Object`, `Collection`), `Pointer`, `FieldKey`, `Preview`, `Children` and `IsExpanded`; `DropDataNodeAsync(node, x, y)` performs the same drop workflow with coordinates in mm. `ReportJsonData.CreateTree(json)` builds nodes, and `CreateSource(json, bodyPointer, rootFieldKeys: ...)` creates collection data plus explicitly requested root keys:

```csharp
var nodes = ReportJsonData.CreateTree(responseJson);
var source = ReportJsonData.CreateSource(responseJson, "/orders", ["$root/company"]);
var collectionNode = nodes[0].Children.Single(item => item.Pointer == "/orders");
await designer.DropDataNodeAsync(collectionNode, 30, 70); // existing table at this position
```

`FindBodies(json)` remains available as a JSON-body utility; the designer uses direct tree drops.

Typed data can also be supplied without HTTP:

```csharp
var report = new ReportDocument { Title = "Sales detail" };
report.Columns.Add(new ReportColumn { Header = "Product", FieldKey = "Product" });
report.Columns.Add(new ReportColumn
{
    Header = "Amount", FieldKey = "Amount", Format = ReportValueFormat.Currency,
    DecimalPlaces = 2, ShowSum = true
});
var source = ReportDataSource.Create("Sales", sales,
    new ReportField<Sale>("Product", "Product", row => row.Product),
    new ReportField<Sale>("Amount", "Amount", row => row.Price * row.Quantity, ReportValueKind.Number));
designer.Document = report;
designer.DataSource = source;
```

Getters are registered explicitly, including nested accesses, without property reflection scans or dynamic expression compilation. Formats support automatic, text, number, currency and date display, currently using `zh-CN` and `¥`. Only numeric fields support sums; nulls render blank. Numeric conditions compare greater-than, less-than or equality thresholds and change text color without altering values/totals; conditions persist in JSON and share results across design, preview and PDF.

## Preview and update contract

`ShowPreview()` / `ShowDesign()` switch modes. `GoToPage(int)` accepts one-based pages and clamps to available pages; `Zoom` is 0.35–1.5. `SelectedElement` / `SelectedColumn` support two-way binding. Use [ReportPreview](report-preview.md) for standalone read-only JSON viewing, API/image loading, PDF export and printing.

- Geometry uses mm converted at `96 / 25.4` DIP. Model font sizes and minimum row heights use DIPs. Column weights divide table width proportionally.
- Table height includes its header and one template row. Resizing changes minimum expanded row height; wrapping may increase it.
- Text is formatted before measuring. Whole detail rows remain on one page. Footer space is reserved; final totals may create a summary page.
- Empty sources produce one page with zero configured totals. Missing fields, invalid formats/geometry, numeric overflow, narrow columns and oversized rows clear `Layout` and report errors. Correcting the definition restores preview.
- Sources snapshot row membership; getters read current row values. Call `RefreshReport()` after mutating those values, or replace the source to refresh membership/reset page one. Document/column notifications refresh automatically. Operate on the UI thread.
- `ReportPaginator.Paginate(document, source)` returns read-only text/geometry and requires initialized Avalonia for measurement. Pagination is synchronous; drawing creates no Control per source row/cell.
- Undo/redo preserves the document instance but replaces elements/columns. Direct model edits enter history; replacing the document resets it.

Current limits: no multiple detail tables/sources, grouping, crosstabs or subreports. Totals use `ShowSum`, not executable expressions. JSON date strings remain text. Editor labels are Chinese.

### Export cancellation, retries and destination ownership

`ExportPdfAsync(destination, token)` stages the complete PDF before modifying the target. Validation, image loading, rendering or cancellation before commit leaves its existing bytes intact. Cancellation is also honored while backing up a readable target. Once destination commit begins, it finishes without the cancellation token: a cancellation arriving during that write returns success with a complete PDF rather than cancellation with a partial file. Success leaves a seekable stream positioned at the new PDF's end.

For a readable, writable, seekable target, the exporter backs up existing bytes. A write, final truncation or buffered flush failure restores the previous bytes, length and position, then rethrows the write error; failure while backing up leaves the target unchanged. An empty seekable write-only target can be restored to empty. A nonempty seekable write-only target is rejected with `NotSupportedException` before modification because its old bytes cannot be recovered. If restoration also fails, `AggregateException` reports both errors and the destination must be treated as damaged. Backup uses additional memory proportional to existing target content. Nonseekable output remains supported, but an I/O failure cannot undo bytes that destination has already accepted.

Only one export may own a given `Stream` object at a time. A conflicting job fails with `InvalidOperationException` before fetching or writing; ownership is released after success, failure or cancellation so retry can reuse the same stream. Independent jobs may run concurrently with distinct streams and retain their own QR/pagination snapshots. Call on the UI thread, await every task and keep streams open until completion; the exporter does not dispose caller-owned streams. Separate wrappers for one underlying file and writes outside this exporter remain the caller's responsibility.

Use `FileMode.OpenOrCreate` with `FileAccess.ReadWrite` when an API caller must preserve an existing PDF, as above. `File.Create` / `FileMode.Create` truncates the file before export starts, so the exporter cannot recover that earlier content. The designer's built-in save action writes a temporary sibling file and replaces a local target only after the complete PDF has been written and flushed; failure/cancellation removes the temporary file and retains the old PDF. Storage providers without a local path use staged stream writing; a provider that truncates on opening, or does not support rollback, cannot offer the same replacement guarantee. Operating-system crashes and failed recovery writes are outside stream rollback guarantees.

The built-in save picker explicitly requests overwrite confirmation. Cancelling the picker, or a cancelled picker task, reports cancellation and releases the export state without opening or changing a target. Failed saves also release that state, allowing retry after the file is unlocked or permissions are restored. Before replacing an existing local PDF, the designer opens it for read/write without truncation and keeps the handle through replacement, allowing delete sharing for its own rename. Exclusive sharing locks and denied write access therefore fail before replacement. Unix permission bits on an existing PDF are retained. Open files that the OS permits replacing follow native rules: Unix file sharing is advisory, not equivalent to mandatory Windows sharing. See the [.NET Unix file-lock implementation](https://github.com/dotnet/runtime/blob/v10.0.0/src/libraries/System.Private.CoreLib/src/Microsoft/Win32/SafeHandles/SafeFileHandle.Unix.cs#L311).

If folder permissions are revoked after the temporary file is created, deleting it may also be denied. The error then includes both the original save error and cleanup error, with the exact temporary path; the previous PDF remains intact. Retrying a local save in that folder after access is restored first removes temporary files tracked by this process. It does not scan or delete other applications' temporary files. Until access returns, retries create no additional staging files. Tracking is process-local: after restarting, any previously reported inaccessible temporary file must be removed using its reported path. Cleanup cannot be guaranteed while the OS denies deletion.

## PreviewHost and focused verification

Extensions → ReportDesign opens the sales example; Extensions → ReportPreview opens the standalone viewer with saved JSON and an explicit 96-row source, without external network access. Both fill the host content area. Search by control name; navigating away and back preserves designer state.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### ReportDesign

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ConditionIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L40) | `int` | writable; OneWay | `computed / state` | Current conditional-format editor option index, synchronized with the selected column. |
| [`CurrentPageNumber`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L109) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`DataSource`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L70) | `TheOne.Avalonia.Reporting.ReportDataSource?` | writable; OneWay | `null` | Explicit data source supplied by the host according to the model contract; templates do not replace data adaptation. |
| [`Document`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L64) | `TheOne.Avalonia.Reporting.ReportDocument?` | writable; OneWay | `null` | Current business document model; editor documentation defines subscription, selection and history behavior on replacement. |
| [`FontFamilyName`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L60) | `string` | writable; OneWay | `computed / state` | Report text font-family name; use fonts available on the target platform and check fallback. |
| [`FontSizeValue`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L50) | `double` | writable; OneWay | `computed / state` | Current font-size editor value, synchronized with the selected report element/document. |
| [`FormatIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L96) | `int` | writable; OneWay | `computed / state` | Current option index in the report numeric-format editor. |
| [`ImageFitIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L76) | `int` | writable; OneWay | `computed / state` | Current option index in the report image-fit editor. |
| [`IsPreview`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L108) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Layout`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L106) | `TheOne.Avalonia.Reporting.ReportLayout?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SaveCommand`](../../../TheOne.Avalonia/Controls/ReportDesign.Commands.cs#L17) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Consumer save command. Its parameter is the current ReportDocument; call document.Save() to obtain JSON. |
| [`SelectedColumn`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L76) | `TheOne.Avalonia.Reporting.ReportColumn?` | writable; TwoWay | `null` | Selected report column model; null means no selection. |
| [`SelectedElement`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L30) | `TheOne.Avalonia.Reporting.ReportElement?` | writable; TwoWay | `null` | Selected report element model; null means no selection. |
| [`SelectedField`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L84) | `TheOne.Avalonia.Reporting.ReportFieldInfo?` | writable; OneWay | `computed / state` | Selected available report field, used for insertion/binding edits. |
| [`SourceFields`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L82) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportFieldInfo>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SumIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L86) | `int` | writable; OneWay | `computed / state` | Current option index in the report summary-operation editor. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L107) | `string?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Zoom`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L111) | `double` | writable; CLR / n/a | `accessor-defined / state` | View scale factor, where 1 means 100%; does not modify report millimetres or workflow coordinates. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `void ReportDesign.MarkSaved()` | Call after the consumer's server save succeeds; executing SaveCommand alone does not imply success. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `Task ReportDesign.DropDataNodeAsync(ReportDataNode node, double x, double y)` | Drops an actual JSON node or typed source field at a paper position in millimetres. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `ReportElement ReportDesign.AddElement(ReportElementKind kind, double x, double y)` | Adds an unbound component at a page position in millimetres. A report supports one repeating table. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `void ReportDesign.DropField(string fieldKey, double x, double y)` | Binds a dropped field to the hit table detail cell or text element; empty paper creates a bound text element. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `Task ReportDesign.FetchApiDataAsync(string url, CancellationToken cancellationToken = default(CancellationToken))` | GET a JSON endpoint and expose its actual object, scalar and collection nodes for binding. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `Task ReportDesign.FetchApiDataAsync(ReportApiRequest request, CancellationToken cancellationToken = default(CancellationToken))` | Fetches a configured API request and imports its response tree without changing existing bindings. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `void ReportDesign.SetImage(ReportElement element, ReadOnlySpan<byte> bytes, string name = "图片")` | Assigns a validated, embedded raster image as one undoable edit. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `string ReportDesign.SaveTemplate()` | Serializes only the report definition, bindings and embedded images. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `void ReportDesign.LoadTemplate(string json)` | Validates a template, replaces the document and automatically restores its optional API data. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `Task ReportDesign.ExportPdfAsync(Stream destination, CancellationToken cancellationToken = default(CancellationToken))` | Exports every preview page as a vector PDF. Must be called on the UI thread. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `Task ReportDesign.LoadTemplateAsync(string json, CancellationToken cancellationToken = default(CancellationToken))` | Loads a saved definition and awaits its optional API data and bound images. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `void ReportDesign.RefreshReport()` | Re-evaluates registered getters and repaginates. Source row membership is a creation-time snapshot. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `void ReportDesign.ShowPreview()` | Switches the report designer to preview mode while retaining its document. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `void ReportDesign.ShowDesign()` | Switches the report designer back to design mode. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `void ReportDesign.GoToPage(int pageNumber)` | Navigates to a requested page; paging/report contracts define indexing and range handling. |
| [`ReportDesign`](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md) | `void ReportDesign.BindField(string fieldKey)` | Binds a selected report element/column to an explicitly registered data field. |
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
