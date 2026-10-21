# First chart batch and animation performance

Recorded 2026-10-20. [简体中文](../zh-cn/chart-echarts-performance-20261020.md) · **English**

## Scope and outcome

The agreed [first roadmap batch](../../plans/chart-echarts-gap-and-optimization.md) adds HeatmapChart, RadarChart and FunnelChart, and Auto / None / Fade / Reveal animation styles across seventeen ChartBase controls. Existing series, sampling, hit testing, selection and raw-value semantics remain. Maps, networks, Sankey and hierarchy charts remain future work; GanttChart is outside this statistical-chart change.

- New charts reuse reference-counted subscriptions and native drawing, with no per-point Controls.
- Cartesian transitions prepare an index and reusable interpolation frame once. Ticks update coordinates in place instead of creating series, arrays, dictionaries and frames; the animation callback delegate is reused.
- Pie/Doughnut cache the most recent geometry and label per sector. Idle redraws reuse them; interrupted transitions do not accumulate old frame chains.
- Heatmap batches provably disjoint cells into at most 256 colors; overlapping cells, transparency and labels retain painter order. Screen-space hit buckets reduce pointer scanning; Heatmap/Funnel avoid duplicate base hit tables.
- Geometry/legend queries do not consume first-presentation animation. Disable, Motion changes, detach and style changes finish old animation; PNG exports the target.
- Radar releases axis subscriptions on detach and reconciles plain Axes IList mutations through RefreshData. Dense funnel stages use actual available height and stay clipped to the plot.

## Environment and methodology

Apple M4, macOS 27.0.1, .NET SDK 10.0.100, Release. The baseline library was frozen before this round's chart edits on `2cb58c5`; unrelated working-tree changes were preserved and are not chart-performance evidence.

Three measurements are intentionally distinct:

1. **Interpolation CPU:** a real Avalonia.Native process repeatedly interpolates fixed source/target frames with changing progress, 100 warmups and 800 samples. Inputs are Line 2,000/6,000 and Bar 1,000 raw points with unchanged default sampling; prepared frames retain 709/832/1,000 marks respectively. This is not a full draw.
2. **Skia offscreen CPU:** the native process reuses a 640×360 RenderTargetBitmap and calls Chart.Render, 25 warmups and 140 samples. Pie/Doughnut have 64 sectors, others 2,000 raw points. It measures CPU submission and current-thread allocation, not display presentation or complete GPU execution.
3. **Desktop scheduling:** a real Avalonia.Native window repeatedly invalidates charts through RequestAnimationFrame, updating data every 18 callbacks with 500 ms animation and collecting 129 intervals per type. Refresh scheduling contributes to these values; they are not GPU raster timings.

Allocations use GC.GetAllocatedBytesForCurrentThread. The timing object adds about 40 B per group, or 0.05 B per 800-sample iteration. “0” means no steady-state allocation observed in the interpolation path after warmup, not zero allocation across the renderer. p95 is the sorted sample percentile. This sample size does not justify strong occasional-p99 conclusions.

## Interpolation

| Input | p95 before → after, ms | Allocated bytes before → after / iteration |
| --- | --- | --- |
| Line, 2,000 raw points | 0.2276 → 0.0392 | 29,552 → 0 |
| Line, 6,000 raw points | 0.2585 → 0.0427 | 34,472 → 0 |
| Bar, 1,000 raw points | 0.2485 → 0.0461 | 224,528 → 0 |

These are steady-state interpolation-plan gains. Starting a transition still allocates its plan, target caches and required text/path data. Sampling budgets and selected-point retention were not reduced.

## Skia offscreen redraw, including controls

| Scenario | p95 before → after, ms | Allocated bytes before → after / iteration |
| --- | --- | --- |
| Pie, 64 sectors | 0.3295 → 0.2502 | 41,192 → 744 |
| Doughnut, 64 sectors | 1.4888 → 0.2025 | 167,144 → 744 |
| Line, 2,000 raw points | 1.3096 → 1.3495 | 45,876 → 45,876 |
| Bar, 2,000 raw points | 2.7195 → 2.9505 | 52,316 → 52,316 |
| Scatter, 2,000 raw points | 2.5567 → 2.5435 | 1,849 → 1,849 |

Radial geometry reuse provides the main idle-redraw gain. Idle Line/Bar/Scatter drawing was not the optimization target; unchanged and slower original results are retained. This does not establish that every chart/scenario got faster.

A targeted Bar recheck disabled tiered JIT and ran three rounds per version, with 50 warmups and 500 samples each, verifying loaded DLL paths and SHA-256:

| Order | Baseline p95 over three rounds, ms | New p95 over three rounds, ms |
| --- | --- | --- |
| Baseline → new | 1.9245 / 1.9240 / 1.9251 | 2.6660 / 2.8215 / 2.9189 |
| New → baseline | 3.2486 / 3.0660 / 2.9436 | 3.0594 / 3.1349 / 3.2300 |

Reverse-order baseline p50 was 2.4873/2.5094/2.5170 versus 2.5853/2.4980/2.4706 for the new version, with matching allocation around 51,311 B/iteration. Order, JIT and machine state substantially affect timings. Both sets remain: **neither a stable improvement nor a consistent code regression is established.** No new per-bar data processing was added, and noisy timings do not justify another implementation change in this round.

## Real desktop scheduling

| Chart | Callback interval p95 before → after, ms |
| --- | --- |
| Line | 16.7970 → 16.7314 |
| Bar | 17.6809 → 17.5781 |
| Pie | 17.6409 → 17.5897 |
| Doughnut | 17.6543 → 17.5720 |

Intervals are mainly limited by roughly 60 Hz refresh. **No substantial end-to-end frame-rate gain is established.** The evidence supports lower interpolation CPU/allocation and radial redraw costs. The earlier Scatter 100k-point, budget-2,000 full-cycle p99 scenario was not rerun.

## Targeted verification

Only charts added or affected by shared animation/cache changes were checked. Temporary probes and artifacts are isolated under `.preview/chart-echarts-20261020/`; PreviewHost Program/App have no added test arguments.

- All seventeen types: Auto/Fade/Reveal/None, first appearance, dynamically disabled animation, Motion, disabled controls, detach/reattach and export preserving live animation.
- Existing selection, positions and target PNGs; seventeen types × Light/Dark × three sizes at 260×220, producing 102 PNGs and reviewing new-chart samples.
- New-chart finite extremes, NaN gaps, negatives/zeroes, observable updates/source replacement, Radar axes Reset/Replace/plain-IList refresh, original indices after sorting and trapezoid boundary hits.
- Duplicate/partially overlapping heatmap cells, class conflicts/local property precedence and first animation after pre-render queries.
- Build only PreviewHost, checking the three dedicated pages, Description examples and compiled bindings. The LoginDemoViewModel nullable warning is pre-existing and unrelated. Page creation/detach also logs existing ComboBox SelectionBoxItem/IsEditable null-template-parent warnings, with no new chart/ViewModel property-resolution errors. Complete logs are retained; that unrelated template is unchanged.

Completed: 259 native-window animation assertions and 188 initial Headless+Skia logic/export/matrix checks passed. The final assembly passed 111 additional new-chart/page/style checks and three pre-render-query animation checks. Batches overlap and must not be summed as unique cases.

Actual macOS CUA input passed: click source index 2 on Heatmap/Radar/Funnel, press Right to move to index 3 and observe updated accessibility descriptions; Escape clears Funnel selection. Property assignments or synthetic routed events did not replace these input checks. The temporary window closed normally. Windows/Linux remain untested.

Identity checks confirm fourteen baseline ChartKind values without AnimationStyle and seventeen final values with it. The frozen baseline DLL SHA-256 is `f90c318333bee026ab81c964925262edc43f24beb043e781aebe9c4b22d9d8a2`; final functional verification uses `9033917f4ff9c95210be9cdb1b9e1d76c48532a6a4eaf92a4e29f9d8d1817c29`. Loaded paths are retained in baseline-identity/final-identity metrics.json. Performance samples used frozen optimized artifacts; later boundary fixes were checked against the final DLL without treating every functional recheck as a fresh full performance run.

Raw metrics are in baseline-native, after-native, baseline-raf and after-raf subdirectories as metrics.json. Frozen baseline binaries, probe source, logs and PNGs remain local cleanable artifacts. This document preserves the main measurements and interpretation.

## Limits and next step

No Windows/Linux runtime, NativeAOT consumer, GPU-native stack, geographic coordinate or later-chart tests were run. Large-data full cycles and p99 need their own workloads and longer runs. The next roadmap batch covers BoxPlot, Candlestick, Treemap and Sunburst; chart count does not imply complete ECharts feature parity.
