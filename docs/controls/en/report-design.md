# ReportDesign report designer

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

Drag text, a table, an image, a line, a rectangle or a page number onto paper. Palette buttons have a compact 44 px minimum height; the structure tree uses the remaining sidebar space. Drag an element to move it, or its bottom-right handle to resize it. A table also has an outer right-edge handle for its overall width; this changes the table width while keeping column proportions. Internal column boundaries adjust neighboring widths, the header/detail boundary changes header height, and the bottom boundary changes detail template height. Columns store relative `WidthWeight`; `TableHeaderHeightMm` sets header height, subject to text measurement.

Geometry uses mm; optional snapping uses 1 mm. Arrow keys move by 1 mm (Shift: 5 mm); Delete removes the selection. Undo/redo records a canvas drag as one edit. The toolbar edits font family, size, bold, italic, underline, alignment and lists. The inspector and border/fill dropdowns use the built-in `ColorPicker`. The font editor and `FontSizeValue` use print points; `ReportElement.FontSize` uses DIPs: `pt = DIP × 0.75`. New elements default to 12 pt (16 DIPs).

The font-family dropdown lists local system fonts through Avalonia's `FontManager`, rather than a fixed preset list. The current document font remains selectable even when it is not installed locally; rendering then follows available font fallback. Saving a font name does not embed that font in the template.

Select an element on paper or in the structure tree; click a table template cell to edit its column. The tree groups title, header, detail, summary and footer elements. Clicking a structure item switches to design mode and scrolls its element, cell or region into view. Selecting a child keeps its parent expanded; element selection updates tree selection without rebuilding the hierarchy. Rebuilds caused by renaming or structural edits preserve existing expansion state. One document currently supports one repeating detail table and multiple other elements.

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
await using var pdf = File.Create("report.pdf");
await designer.ExportPdfAsync(pdf); // UI thread; every page
```

PDF shares pagination and Avalonia/Skia drawing with preview. Text and shapes stay vector; images are embedded. Physical mm map to 72 points per inch, excluding rulers, handles and editor chrome. Source-free reports export placeholders; invalid layouts block export. Available system fonts can affect wrapping and glyph fallback.

## Optional API data and binding

Click Add Data Source in the Data tab. The dialog accepts an HTTP/HTTPS URL, GET / POST / PUT / PATCH / DELETE, and parameters as a JSON object (64 KiB maximum). GET encodes them into the existing URL query; other methods send JSON or Form data. Nested Form values become JSON strings. Requests use a 20-second timeout and a 4 MiB response limit.

The Data tab uses the full left sidebar: its response tree fills the height below the endpoint controls and search box. The report structure appears only in the Components tab, beneath the compact palette, and retains its selection when switching tabs. Switching Components/Data also retains data-tree expansion, selection, scroll offset and search. It does not refetch data. Filtered roots are reused until the source or query changes, so refreshes do not discard selection.

An assigned saved `Document` also loads its optional API data when the designer is attached, unless the application has supplied its own source. Assigning `DataSource` during automatic loading takes precedence and cancels that pending fetch. Closing/replacing the document cancels older requests; late responses cannot replace a newer report. Detaching interrupts an in-flight automatic load and reattachment restarts it; an already loaded source survives cached navigation without another data request. Automatic loading has a 30-second overall limit, keeps runtime data outside template JSON and does not create an undoable definition edit.

Fetch first displays formatted response JSON inside the dialog; only a successful response can be confirmed. Editing request settings invalidates the previous confirmation. Cancel/close preserves the existing source, and confirmation imports the response tree without another HTTP request. Text preview displays at most 200,000 characters; imported data remains complete. The tree shows objects, scalars, collections and actual array indices. Importing keeps bound records unchanged until nodes are dropped to establish bindings.

Drag a scalar tree node onto text or a particular table cell to fill its current value and retain its binding; dropping onto blank paper creates bound text. A scalar containing an HTTP/HTTPS image URL can be dropped onto an image component to load and bind that image. Objects organize the tree and are not dropped as values.

Drop a collection onto an existing table to open the modal field-mapping dialog. It shows each existing column, a field picker and first-record samples, with native scrolling for long lists. Confirming sets the collection as the detail source and applies all mappings as one undoable edit. Unselected columns keep static `ReportColumn.Text` and clear previous field bindings; nonnumeric/unbound columns also disable sums and numeric conditions. Cancel or closing the dialog changes neither columns nor the source. Collections cannot be dropped onto text or blank paper.

The chosen endpoint and collection JSON Pointer are saved as `DataSourceUrl` and `DataBodyPointer`. Absolute scalar keys such as `$root/company` or `$root/orders/0/logo` can coexist with fields relative to the chosen collection. Saving JSON preserves those binding keys; standalone preview refetches the response and restores root values, table records and bound image URLs together. Escaped JSON Pointer keys keep dotted/slashed property names and actual array indices unambiguous. Failed fetches preserve the current source and design.

Binding also saves `DataSourceMethod`, `DataSourceParameters` and `DataSourceParameterMode` for standalone preview. The GET overload `FetchApiDataAsync(url, cancellationToken)` remains available; use `FetchApiDataAsync(new ReportApiRequest(url, "POST", "{\"page\":1}"), cancellationToken)` for configured requests. `ReportResources.FetchJsonAsync(request, client, cancellationToken)` accepts a consumer-owned `HttpClient`.

The dialog places **Requires authentication** first, the optional login/header section second, and the data interface third. Sections use the dialog background, retaining the native inputs' own fills. Enable authentication to show the login URL, method, JSON/Form parameters, response token field and outgoing header settings. **Get authorization** calls only the login endpoint, independently of the data URL, and shows its response JSON and resolved header value. An incorrect token path preserves the login JSON for inspection. Testing authorization does not enable source confirmation or import data; **Fetch data source** still performs the complete login/data chain.

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

Binding a fetched node saves settings in `ReportDocument.DataSourceAuthentication`, including undo/redo; `null` requests data directly. Each authenticated fetch performs login → token extraction → data request. Login failure, invalid tokens and cancellation stop the chain. Only the data request receives the header; shared client defaults and separate image requests are unchanged. The dialog displays the resolved header and JSON before confirmation. Runtime tokens are not serialized; standalone preview logs in afresh. Login and data responses each have the same 4 MiB/20-second limits. The editor supports one token header, without interactive OAuth, token refresh or multiple authentication headers.

## Checkbox and barcode components

Drag `ReportElementKind.CheckBox` or `Barcode` from the palette. Both share positioning, resizing, structure navigation and undo. Checkbox `Text` is its caption and `IsChecked` its default state. Bound fields take precedence and accept booleans, 1/0, yes/no, 是/否 or an empty unchecked value; unsupported values block preview/export. Dropping a boolean preserves the caption.

Barcodes use Code 128B: `Text` is a static value or is resolved through `FieldKey`; `BarcodeShowText` controls the readable caption. Values contain 1–128 printable ASCII characters, with checksum, stop and ten-module quiet zones on both sides. Chinese text, GS1/FNC and other symbologies are unsupported. Module width must be at least 0.75 DIP (about 0.20 mm), with invalid sizes reported. Both components use vector drawing in template JSON, standalone preview and PDF. Consumers should verify their printer resolution and scanning devices.

The display tree expands to depth 16, shows at most the first 200 indexed children of each array and uses an approximately 4000-node budget, with truncation hints. These are display limits: collection binding reads all source rows. A source supports at most 4096 distinct row-field keys; exceeding that metadata limit reports an error. Automatic root-field discovery is also bounded, while explicitly saved root bindings are restored as needed.

The source search box and TreeView use native Small sizing independently of the host density. Nodes show their property name and value/count preview on one line; long content is ellipsized, with name, JSON Pointer and value preview available in the tooltip. This bounded response hierarchy uses native StackPanel items panels and one physical scrolling viewport, avoiding nested virtualizing panels losing expanded content while scrolling. Horizontal overflow is clipped to the sidebar width. Successful import/binding messages are hidden above the tree; failed fetch/binding messages remain visible.

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

## PreviewHost and focused verification

Extensions → ReportDesign opens the sales example; Extensions → ReportPreview opens the standalone viewer with saved JSON and an explicit 96-row source, without external network access. Both fill the host content area. Search by control name; navigating away and back preserves designer state.

```sh
dotnet run --project design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/reportdesign/artifacts -- --reportdesign
dotnet run --project design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/reportdesign/artifacts -- --verify-reportdesign .preview/reportdesign/screenshots
```

Focused report checks cover layout, bindings, pointer editing, response-tree drops and field mapping, JSON roundtrips, images, PDF and themed drawing. NativeAotSmoke `--require-aot --verify-reportdesign` runs the report checks for native consumption. These checks do not establish physical printer output or Windows/Linux runtime behavior.

`--verify-report-authentication` checks login/data ordering, editable headers, failure/cancellation, confirmation, persistence and standalone preview against a loopback API. `--verify-report-lifecycle` checks native plain/card tabs and cached ReportDesign pages in light/dark and all three densities, plus overflow, keyboard and closing in four placements. Both accept an optional screenshot folder. Add `-p:ReportOnlyPreview=true` before `--` to isolate these from unrelated PreviewHost demos. NativeAotSmoke supports the same property/commands with `--require-aot`.

`--verify-report-tree-scroll` isolates the response tree: actual wheel input, stable extents, rendered text at the bottom, returning to the root, collapse/re-expand, filtering, scalar pointer drag and fetch-error recovery. It covers both themes, all host densities and a narrower viewport; the same isolation property and native consumer command apply.

`--verify-report-sidebar` combines the tree scroll checks with structure navigation and repeated Components/Data switching. An optional output folder also captures both sidebar tabs across light/dark and the three host densities.

`--verify-report-reload` checks saved-template reopening, fresh root/table values, request modes and authentication, network-image binding, cancellation, late-response isolation, cached navigation and direct MVVM document assignment. The tree scroll checks also verify expansion, selection, offset and filtered-tree identity across repeated tab changes.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### ReportDesign

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.ReportDesign.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ConditionIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L21) | `int` | writable; OneWay | `computed / state` | Current conditional-format editor option index, synchronized with the selected column. |
| [`CurrentPageNumber`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L39) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`DataSource`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L23) | `TheOne.Avalonia.Reporting.ReportDataSource?` | writable; OneWay | `null` | Explicit data source supplied by the host according to the model contract; templates do not replace data adaptation. |
| [`Document`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L22) | `TheOne.Avalonia.Reporting.ReportDocument?` | writable; OneWay | `null` | Current business document model; editor documentation defines subscription, selection and history behavior on replacement. |
| [`FontFamilyName`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L30) | `string` | writable; OneWay | `computed / state` | Report text font-family name; use fonts available on the target platform and check fallback. |
| [`FontSizeValue`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L25) | `double` | writable; OneWay | `computed / state` | Current font-size editor value, synchronized with the selected report element/document. |
| [`FormatIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L31) | `int` | writable; OneWay | `computed / state` | Current option index in the report numeric-format editor. |
| [`ImageFitIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L39) | `int` | writable; OneWay | `computed / state` | Current option index in the report image-fit editor. |
| [`IsPreview`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L38) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Layout`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L36) | `TheOne.Avalonia.Reporting.ReportLayout?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SaveCommand`](../../../TheOne.Avalonia/Controls/ReportDesign.Commands.cs#L26) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Consumer save command. Its parameter is the current ReportDocument; call document.Save() to obtain JSON. |
| [`SelectedColumn`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L24) | `TheOne.Avalonia.Reporting.ReportColumn?` | writable; TwoWay | `null` | Selected report column model; null means no selection. |
| [`SelectedElement`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L19) | `TheOne.Avalonia.Reporting.ReportElement?` | writable; TwoWay | `null` | Selected report element model; null means no selection. |
| [`SelectedField`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L26) | `TheOne.Avalonia.Reporting.ReportFieldInfo?` | writable; OneWay | `computed / state` | Selected available report field, used for insertion/binding edits. |
| [`SourceFields`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L25) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportFieldInfo>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SumIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L40) | `int` | writable; OneWay | `computed / state` | Current option index in the report summary-operation editor. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L37) | `string?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Zoom`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L40) | `double` | writable; CLR / n/a | `accessor-defined / state` | View scale factor, where 1 means 100%; does not modify report millimetres or workflow coordinates. |

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
| [`ReportPdfExporter`](../../api/en/TheOne.Avalonia.Reporting.ReportPdfExporter.md) | `static Task ReportPdfExporter.ExportAsync(ReportDocument definition, ReportDataSource? source, Stream destination, HttpClient? client = null, CancellationToken cancellationToken = default(CancellationToken))` | Exports a paginated report PDF asynchronously with caller-supplied destination/cancellation/file lifetime. |
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
| [`TheOne.Avalonia.Reporting.ReportApiAuthentication`](../../api/en/TheOne.Avalonia.Reporting.ReportApiAuthentication.md) | 7 |
| [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](../../api/en/TheOne.Avalonia.Reporting.ReportApiParameterMode.md) | 2 |
| [`TheOne.Avalonia.Reporting.ReportApiRequest`](../../api/en/TheOne.Avalonia.Reporting.ReportApiRequest.md) | 5 |
| [`TheOne.Avalonia.Reporting.ReportColumn`](../../api/en/TheOne.Avalonia.Reporting.ReportColumn.md) | 12 |
| [`TheOne.Avalonia.Reporting.ReportCondition`](../../api/en/TheOne.Avalonia.Reporting.ReportCondition.md) | 3 |
| [`TheOne.Avalonia.Reporting.ReportDataNode`](../../api/en/TheOne.Avalonia.Reporting.ReportDataNode.md) | 9 |
| [`TheOne.Avalonia.Reporting.ReportDataNodeKind`](../../api/en/TheOne.Avalonia.Reporting.ReportDataNodeKind.md) | 3 |
| [`TheOne.Avalonia.Reporting.ReportDataSource`](../../api/en/TheOne.Avalonia.Reporting.ReportDataSource.md) | 3 |
| [`TheOne.Avalonia.Reporting.ReportDocument`](../../api/en/TheOne.Avalonia.Reporting.ReportDocument.md) | 29 |
| [`TheOne.Avalonia.Reporting.ReportElement`](../../api/en/TheOne.Avalonia.Reporting.ReportElement.md) | 25 |
| [`TheOne.Avalonia.Reporting.ReportElementKind`](../../api/en/TheOne.Avalonia.Reporting.ReportElementKind.md) | 8 |
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
