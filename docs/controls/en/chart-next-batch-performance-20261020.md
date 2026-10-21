# Second-batch charts and preview-opening measurements

**English** · [简体中文](../zh-cn/chart-next-batch-performance-20261020.md) · [Chart overview](charts.md)

Date: 2026-10-20. BoxPlot, Candlestick, Treemap and Sunburst bring ChartBase controls to 21 types. See [statistical charts](chart-statistical.md) and [hierarchy charts](chart-hierarchy.md) for configuration, models and limitations. This report separates before/after measurements of existing preview pages from initial baselines for newly introduced charts.

## Environment and method

- macOS / Apple M4, .NET 10, Avalonia 12.1.4, Release. Comparisons use the task's starting working tree and frozen builds of each optimized path, rather than substituting an older Git commit. Later changes affect only new charts; native existing-preview measurements and final new-chart results record their respective assembly identities separately.
- Native desktop measurements use an Avalonia.Native window, the same 1100×820 viewport and OneTheme. Each page has one fresh-process cold sample followed by 20 page recreations. Timing spans construction through the first RAF callback, including attachment and layout. **A RAF callback is not GPU completion.**
- p50 is the median, averaging the two middle values for even sample counts. p95 is nearest-rank, element `ceil(0.95×n)`. Cold n=1 has no p95. Allocations are UI-thread managed bytes, excluding native / GPU memory.
- Offscreen new-chart draws use Avalonia.Headless + Skia. Their CPU time cannot be compared directly with native desktop RAF or converted into a real frame rate.
- Raw samples, probes, absolute assembly paths and SHA-256 identities live under `.preview/chart-next-batch-20261020/`, without adding application startup switches. Explicit binary references and hash checks guard against stale assemblies.

## Existing preview pages: opening stalls

| Page | Cold opening before→after ms, n=1 | Repeat p50 before→after ms | Repeat p95 before→after ms | Median initial-layout allocations before→after MiB |
| --- | ---: | ---: | ---: | ---: |
| Gauge | 922.17 → 370.45 | 150.08 → 65.04 | 250.17 → 80.68 | 9.85 → 2.70 |
| Line | 677.41 → 463.56 | 181.27 → 83.27 | 244.67 → 108.91 | 9.69 → 3.72 |
| Scatter | 786.46 → 578.34 | 185.54 → 108.11 | 238.03 → 151.41 | 9.69 → 3.72 |

p95 falls by approximately 68%, 55% and 36%. These results apply to the measured host and viewport; page opening still exceeds a single-frame budget. Cold opening includes theme, JIT, template and text-system initialization.

The main bottleneck was eagerly creating offscreen syntax-highlighted code and its control tree. A phase probe measured attachment / layout of two highlighted Description rows at about 100.91 ms / 6.80 MB, versus 1.43 ms / 24 KB for plain text. This locates the cost; the implementation keeps the actual highlighted editor.

All labeled style comparisons remain. The first chart is immediate; additional variants, animation documentation and highlighted code materialize through the actual EffectiveViewport. Created content uses its natural height, without retaining forced placeholder heights. This does not defer the same work through Dispatcher. Samples have separate series wrappers so legend state cannot leak between charts. Gauge no longer constructs unused view models, series or points.

Initial visual controls fall from 271 to 52 for Gauge and from 261 to 109 for Line / Scatter. Created highlighted editors fall from two to zero. Scrolling still reaches every sample and code editor. The first visit to a code area still pays its initialization cost; a viewport large enough to show the area creates it immediately.

## New-chart performance

Statistical inputs contain 1,000 observations; hierarchy inputs contain 10,000 nodes, rendered offscreen at 640×420. Fifteen warm-ups precede 80 cached draws, including actual Skia drawing-context submission rather than geometry preparation alone. The single-update measurement sets one point's Y and prepares target geometry, n=1; it **excludes subsequent rasterization and is not an update p95**.

### Initial statistical-chart baseline

| Control, 1,000 observations | Cached draw p50 / p95 ms | Managed KiB / draw | Point update + geometry ms, n=1 | Update MiB |
| --- | ---: | ---: | ---: | ---: |
| BoxPlot | 15.08 / 18.16 | 28.14 | 7.80 | 1.00 |
| Candlestick | 13.71 / 16.15 | 26.31 | 10.05 | 0.96 |

Neither chart had a previous equivalent control. These are initial baselines, not speedups over an older version. Raw data: `stats-performance-final/metrics.json`.

### Hierarchy label-cache comparison

The before version is the first implementation of the new charts. The after version adds a bounded 16,384-node text-measurement cache, exact no-space checks and consistent text-paint handling. Weight updates retain projection, nodes and collection subscriptions. Font, theme, text, explicit Foreground and mutable brushes invalidate correctly.

| Control, 10,000 nodes | Cached draw p50 before→after ms | Cached draw p95 before→after ms | Point update + geometry before→after ms, n=1 | Update allocations before→after MiB |
| --- | ---: | ---: | ---: | ---: |
| Treemap | 11.36 → 5.68 | 14.14 → 6.93 | 143.96 → 10.07 | 27.15 → 0.081 |
| Sunburst | 33.91 → 36.39 | 37.70 → 56.34 | 211.16 → 55.26 | 34.25 → 7.30 |

**Sunburst does not show a stable cached-raster tail improvement.** A reverse-order after→before repeat of the same 80-frame script gave after p50 / p95 32.47 / 40.97 ms and before 99.82 / 162.07 ms; single-update times were after 38.23 ms and before 387.47 ms. Both runs are retained rather than selecting a favorable pair to claim a draw speedup. The more consistent result is update allocation falling from approximately 34.25 MiB to 7.30 MiB, about 78.7%. Ten-thousand-node path creation and raster tails remain bottlenecks.

Raw samples, assembly hashes and run order are in `tree-performance-before/`, `tree-performance-after/`, `tree-performance-before-run2/`, `tree-performance-after-run2/` and the local `VALIDATION.md`. The existing-preview optimized library has SHA prefix `c3573e9a`; final new charts use `db7d65e4`. Subsequent new-chart corrections did not change the construction paths of those three existing pages.

Cached draws do not re-enumerate source data and point changes do not rebuild all data subscriptions. Statistical updates still recompute range / layout; hierarchy changes still generate affected geometry. These results do not establish O(1) full-frame updates or real-time rendering for ten-thousand-node Sunburst.

## Targeted coverage

Only the four new controls, affected preview construction paths and minimal Line / Heatmap callers of shared subscription / navigation infrastructure were checked. No all-control regression was run.

Coverage includes statistical ordering and gaps, outlier collection replacement / shared references, Median / Close alias notifications and single Y invalidation, selection / hits / viewports, hierarchy Reset / Replace / ItemsSource replacement, unique-key fallback, cycles / repeated references, Root cleanup, detach / reload, target-frame PNG, animation switches, Light / Dark × three sizes and narrow viewports.

The initial four-chart suite passed 123 checks. Final statistical, hierarchy, anchor / outlier-budget / Heatmap-caller and native preview suites passed 57, 114, 7 and 91 checks respectively. Suites overlap and are not summed into a unique case count. Final builds have zero errors and retain the existing LoginDemoViewModel nullable warning without expanding scope.

Real pointer / keyboard checks cover statistical selection, zoom and pan plus hierarchy double-click drill-down, return and keyboard selection. Native preview scroll, narrow viewport, three sizes and detach / reattach checks ensure every deferred area eventually materializes; content was not discarded to obtain the improvement.

Current-host execution does not establish Windows / Linux execution. This round did not run NativeAOT publishing, GPU stack capture or a whole-library regression. Ten-thousand-node Sunburst rasterization and ancestor-brush lookup in exceptionally deep trees remain future optimization targets.
