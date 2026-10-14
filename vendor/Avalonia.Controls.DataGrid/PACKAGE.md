# TheOne.Avalonia.DataGrid

TheOne's source extension of upstream Avalonia.Controls.DataGrid **12.1.2**, built for .NET 10 and Avalonia **12.1.4**.

Its NuGet version follows TheOne.Avalonia's release train, independently of the upstream DataGrid version. `Avalonia.Controls.DataGrid` remains the assembly name, `12.1.2.0` remains AssemblyVersion, and the existing strong-name identity is preserved.

Adds native right frozen columns, compiled entity value readers, shared column geometry, generic collection views, sorting and drag feedback support. Native row/cell presenters, virtualization, keyboard navigation and editing remain in use.

Normally install TheOne.Avalonia, which brings this dependency and the themes. Both packages must be available in the source; upgrade them together. Do not simultaneously reference the official Avalonia.Controls.DataGrid package.

This is a coordinated preview. NativeAOT still requires typed row preservation and compiled bindings; upstream reflection/conversion warnings remain and are not suppressed. Cross-RID publishing does not prove execution on the target operating system.

Source provenance is included in UPSTREAM.json; license in LICENSE.md. Documentation and platform boundaries: [TheOne.Avalonia repository](https://github.com/The1218/TheOne.Avalonia).
