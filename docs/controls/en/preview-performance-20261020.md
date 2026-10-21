# PreviewHost page and property presentation optimization (2026-10-20)

[简体中文](../zh-cn/preview-performance-20261020.md) · [Control documentation](README.md)

> This document preserves phase one's historical implementation and measurements. The current preview removes the bottom source panel and further optimizes return after a full browse; see [phase two](preview-return-performance-20261020.md).

## Presentation policy

Additional-property descriptions retain project-added public writable bindable properties: Avalonia properties on controls, attached properties, and public writable properties on related models. Both `Binding` and `CompiledBinding` are supported. The default binding mode does not have to be `TwoWay`; writable properties that default to `OneWay` can explicitly use `Mode=TwoWay`. Applications must supply the appropriate `x:DataType`, data context and change notifications.

Inherited native-property duplicates, read-only runtime state, static fields, init-only members and internal identifiers are omitted. Models without writable properties do not produce empty sections. Enum choices remain beside their property, without separate enum inventories. This changes preview presentation only; it does not remove control/model members or entries from the [complete public contract](../../api/en/README.md).

Business APIs retain actual actions such as saving, loading, exporting, validating and undoing, with short code examples. Property Get/Set wrappers, constructors and property identifiers are excluded. Classes remain labeled live examples; shared sizing, theming and motion settings are explained on Home.

## Implementation

- Shared property, business-API and model descriptions are created on entering the viewport, in `Description` batches of at most six rows. All filtered content remains available as subsequent batches enter view.
- Offscreen style samples defer attachment. Grouped Button / Input / Select pages no longer instantiate original demos that the content policy excludes.
- Snippet editors are created on first visibility. The bottom source editor is created on first expansion and reused when switching files. Deferred creation reads the latest text, language and density.
- Ordinary navigation resets the scroll offset without forcing synchronous `UpdateLayout()` or repeating the reset in the window. Explicit section navigation still obtains measured coordinates; request sequencing rejects stale scroll callbacks.
- Estimated heights reserve space only before creation. Materialized content uses its natural height, without fixed documentation heights. Created instances remain available and do not lose business state when scrolled out of view.
- Deferred interactive content remains reachable through Tab / Shift+Tab. Focus moves synchronously to the real child, skips disabled candidates, and removes the placeholder from the tab sequence. No delayed focus restoration is used.

## Measurement boundary

Environment: macOS, Apple M4, .NET 10, Avalonia 12.1.4, Release, 1100 × 820 window, native desktop backend. Navigation uses `GalleryViewModel.Select` with the complete `ControlGallery`, including presentation wrappers, documentation and source panel, rather than an isolated Chart. Timing ends at the first native animation-frame callback and includes synchronous creation and layout; **it is not GPU presentation completion time**.

Before and after runs use identical control-library, theme and dependency assemblies, replacing only PreviewHost to exclude unrelated workspace changes. Uncached runs create a new Gallery each time. The first navigation is reported separately; warm statistics use ten subsequent Line / Gauge samples and five Button samples. Nearest-rank p95 is close to the maximum with these small samples and is an exploratory comparison, not a stable tail-latency guarantee. Process startup and initial window creation are outside the navigation timer.

Logs, JSON, screenshots and temporary probes are under `.preview/preview-open-20261020/`. They are not normal application startup branches or public control APIs.

## Desktop results

Milliseconds, before → after. “First” is the first navigation in the process, not startup time. Cached-return statistics exclude initial creation.

| Scenario | First | Warm median | Warm p95 | Statistical samples |
| --- | ---: | ---: | ---: | ---: |
| Uncached Line | 1445.5 → 409.7 | 290.1 → 66.1 | 1241.0 → 160.7 | 10 |
| Uncached Gauge | 1570.0 → 259.6 | 364.7 → 88.5 | 887.1 → 119.1 | 10 |
| Uncached grouped Button | 899.8 → 434.0 | 403.2 → 138.5 | 729.5 → 373.3 | 5 |
| Cached Line after viewing its initial viewport | — | 355.7 → 79.6 | 725.9 → 151.4 | 10 |
| Cached Line after browsing all descriptions | — | 339.9 → 197.8 | 1665.3 → 1159.8 | 10 |

| Initial attached content | Before | After |
| --- | ---: | ---: |
| Line visual-tree elements / created description rows | 1912 / 139 | 167 / 2 |
| Gauge visual-tree elements / created description rows | 2459 / 185 | 110 / 0 |
| Button visual-tree elements / created shared detail sections | 1308 / 6 | 499 / 0 |

Raw samples and assembly SHA-256 values are stored in `before-shared-native-*` and `final-native-*`. Shared control-library SHA-256: `16b3a724cbaef4bb2154182870b8319bc6db2dacc8324f32c995046bcab926f7`; final PreviewHost: `ef7695030e078b82d00ddf0e2a76402328ee4a9da49f6c2e32951c285ad9c0a6`. Separate process runs still vary; this single sample set is not a per-navigation guarantee. Button p95 retains substantial remaining cost.

The fully visited case uses separate `cached-full-line` runs. After initial opening, all descriptions are traversed before alternating Home / Line; scrolling is outside the navigation timer. The final page retains 1593 visual-tree elements, 87 rows and two previously used editors, so preserved instances still incur reattachment costs. Its p95 remains above one second. This tail latency is unresolved; view recycling needs a separate evaluation that preserves selection, editing and focus rather than simply destroying controls to lower the measurement.

## Validation and remaining costs

Only PreviewHost was built. Targeted native desktop validation passed 2009 checks for presentation metadata and affected callers: Line / Gauge / Button / Input / Select / NavPage, complete scrolling, light/dark, three densities, narrow windows, section navigation, source expansion/file changes, rapid navigation, detach/reattach, forward/reverse Tab through deferred content, disabled candidates and current snippet configuration. Filtered Line descriptions grow from two initial rows to all 87 rows, and Gauge from zero to all 116 rows. Content is not truncated to improve opening time.

Real pointer and keyboard checks also used the final MainWindow: menu search opens LineChart, source expansion shows the correct file, and Tab / Shift+Tab moves between the source selector and copy button. After collapsing the source panel, Shift+Tab enters previously uncreated business-API descriptions, followed by normal forward/backward navigation. Narrow-window screenshots in Small / Normal / Large and light/dark were inspected for wrapping and clipping. The temporary validation window was closed; normal Program / App startup has no test branches.

First visibility of a new description batch, complex business example or editor still requires creation, layout or highlighting. Fully visited pages retain their instances and state, so returning can still incur reattachment costs. Control rendering, business data and binding contracts are unchanged. This was not a library-wide functional regression, AOT consumer test, or Windows/Linux runtime validation. Measure real business pages on the target machine for production decisions.
