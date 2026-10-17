# TheOne.Avalonia

TheOne themes and extended controls for .NET 10 and Avalonia 12.1.4. Includes independent charts, navigation, windows
and DataGrid entity filters, left/right frozen columns and numeric summaries.

Preview.5 adds NavPage with a collapsible navigation sidebar, retained card tabs, optional right header and compiled
model bindings. Tabs support icons, pinned pages and close menus. MultiSelectBox searches cached entries without
rebuilding source subscriptions; WorkflowEditor adds graph indexes, viewport caches and safe offscreen view recycling.
Popup boundaries contain wheel, touch and inertia input while preserving native internal scrolling. Bilingual guides
and individual previews document the added properties, business APIs, models and lifecycle rules.

```xml
<Application.Styles>
  <one:OneTheme />
</Application.Styles>
```

Declare `xmlns:one="using:TheOne.Avalonia.Themes"` on the application. OneTheme includes the Fluent support layer and
DataGrid themes. Use `Classes="small"`, `normal` or `large` for density and Light / Dark theme variants.

TheOne.Avalonia and TheOne.Avalonia.DataGrid share one release version and channel. Both packages must be available in
your package source. The DataGrid dependency uses NuGet's normal inclusive minimum version; upgrade the pair together.
Do not add the official Avalonia.Controls.DataGrid package alongside this fork.

Register each concrete row and nested property type with `DataGridAot.PreserveRowType<T>()` before creating a collection
view or grid, for both managed and NativeAOT applications. Explicit columns use compiled bindings; automatic columns use
registered public-property metadata and compiled accessors. Custom domain conversions require an explicit column
converter.

Existing-project integration, platform boundaries and a portable Codex migration
brief: [English](https://github.com/The1218/TheOne.Avalonia/blob/main/docs/integration/en/README.md) / [简体中文](https://github.com/The1218/TheOne.Avalonia/blob/main/docs/integration/zh-cn/README.md). [Documentation index](https://github.com/The1218/TheOne.Avalonia/blob/main/docs/README.md).

[Source](https://github.com/The1218/TheOne.Avalonia) ·
Controls: [English](https://github.com/The1218/TheOne.Avalonia/blob/main/docs/controls/en/README.md) / [简体中文](https://github.com/The1218/TheOne.Avalonia/blob/main/docs/controls/zh-cn/README.md) ·
Theme: [English](https://github.com/The1218/TheOne.Avalonia/blob/main/docs/theme/en/README.md) / [简体中文](https://github.com/The1218/TheOne.Avalonia/blob/main/docs/theme/zh-cn/README.md).

MIT license. Third-party notices are included in THIRD-PARTY-NOTICES.md.
