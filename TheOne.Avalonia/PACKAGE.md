# TheOne.Avalonia

Still themes and extended controls for .NET 10 and Avalonia 12.1.4. Includes independent charts, navigation, windows and DataGrid entity filters, left/right frozen columns and numeric summaries.

```xml
<Application.Styles>
  <one:OneTheme />
</Application.Styles>
```

Declare `xmlns:one="using:TheOne.Avalonia.Themes"` on the application. OneTheme includes the Fluent support layer and DataGrid themes. Use `Classes="small"`, `normal` or `large` for density and Light / Dark theme variants.

TheOne.Avalonia and TheOne.Avalonia.DataGrid share one release version and channel. Both packages must be available in your package source. The DataGrid dependency uses NuGet's normal inclusive minimum version; upgrade the pair together. Do not add the official Avalonia.Controls.DataGrid package alongside this fork.

The current release is a preview, not a declaration that all target platforms have been tested. NativeAOT consumers must use compiled column bindings, disable auto-generated columns and call `DataGridAot.PreserveRowType<T>()` from TheOne.Avalonia.Controls.

Source, API documentation, release policy and platform limitations: [TheOne.Avalonia repository](https://github.com/The1218/TheOne.Avalonia).

MIT license. Third-party notices are included in THIRD-PARTY-NOTICES.md.
