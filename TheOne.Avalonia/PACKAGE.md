# TheOne.Avalonia

TheOne themes and extended controls for .NET 10 and Avalonia 12.1.4. Includes independent charts, navigation, windows
and DataGrid entity filters, left/right frozen columns and numeric summaries.

Preview.4 refines bilingual control/API/model documentation and individual preview pages with practical code examples.
It fixes MultiSelectBox popup scrolling, tab end-drop behavior, virtualized item reinsertion and collection/source changes,
plus native menu first-open sizing and scroll-button layout. Short menus measure their complete content; large menus
retain virtualization. Form/FormItem, enum-based theme switching, gradients and window-owner guidance remain available.

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
