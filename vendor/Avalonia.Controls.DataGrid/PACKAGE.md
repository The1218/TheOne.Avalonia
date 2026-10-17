# TheOne.Avalonia.DataGrid

TheOne's source extension of upstream Avalonia.Controls.DataGrid **12.1.2**, built for .NET 10 and Avalonia **12.1.4**.

Its NuGet version follows TheOne.Avalonia's release train, independently of the upstream DataGrid version.
`Avalonia.Controls.DataGrid` remains the assembly name, `12.1.2.0` remains AssemblyVersion, and the existing strong-name
identity is preserved.

Adds native right frozen columns, compiled entity value readers, shared column geometry, generic collection views,
sorting and drag feedback support. Native row/cell presenters, virtualization, keyboard navigation and editing remain in
use.

Normally install TheOne.Avalonia, which brings this dependency and the themes. Both packages must be available in the
source; upgrade them together. Do not simultaneously reference the official Avalonia.Controls.DataGrid package.

This is a coordinated preview. Register row and nested property metadata with `DataGridRowMetadata.Register<T>()` before
creating collection views or grids. Sorting uses registered typed comparers, automatic columns use compiled property
accessors, and scalar editing uses explicit conversion APIs. The DataGrid IL2026 / IL2070 / IL2075 / IL3050 warnings
have been eliminated without warning suppression or whole-assembly preservation. Custom conversions require an explicit
column converter. Cross-RID publishing does not prove execution on the target operating system.

Source provenance is included in UPSTREAM.json; license in
LICENSE.md. [Source](https://github.com/The1218/TheOne.Avalonia) · DataGrid
documentation: [English](https://github.com/The1218/TheOne.Avalonia/blob/main/docs/controls/en/datagrid-binding.md) / [简体中文](https://github.com/The1218/TheOne.Avalonia/blob/main/docs/controls/zh-cn/datagrid-binding.md).
