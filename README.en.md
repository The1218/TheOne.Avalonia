# TheOne.Avalonia

[简体中文](README.zh-cn.md) · [English](README.en.md)

A cross-platform theme and control library built on Avalonia's native controls. It includes light and dark themes, three control sizes, multiple control styles, and gradient palettes. PreviewHost provides interactive examples.

- Consistent themes for buttons, inputs, selectors, date and time pickers, lists, trees, menus, and layout controls.
- Fourteen chart types, plus Form/FormItem, MultiSelectBox, TreeSelect, NavMenu, InfoCard, Description, Tag, VerificationStep, window, and messaging extensions.
- DataGrid with compiled bindings, automatic entity-property filters, frozen columns on both sides, and numeric summaries.
- QRCode, ReportDesign/ReportPreview, WorkflowEditor, GanttChart and NavPage provide report, workflow and workspace extensions; see the [preview.7 release notes](docs/releases/en/1.0.0-preview.7.md).


This release adds TreeSelect, button color/appearance composition and Tag variants, and fixes TabControl/ColorPicker layout. NuGet upload is manual; this source version does not imply feed availability.

## Requirements and versions

| Component | Version |
| --- | --- |
| .NET SDK / TargetFramework | .NET 10 / `net10.0` |
| Avalonia core, Skia, Fluent | 12.1.4 |
| DataGrid | Based on 12.1.2; package: `TheOne.Avalonia.DataGrid` |
| TheOne.Avalonia / TheOne.Avalonia.DataGrid | 1.0.0-preview.7 |
| PreviewHost MVVM | ReactiveUI.Avalonia 12.1.4 |

## Quick start

Clone the repository and run the interactive preview:

```sh
git clone https://github.com/The1218/TheOne.Avalonia.git
cd TheOne.Avalonia
dotnet run --project design/PreviewHost
```

Install the preview package in your application. The companion DataGrid package is restored automatically:

```sh
dotnet add path/to/YourApp.csproj package TheOne.Avalonia --version 1.0.0-preview.7
```

NuGet packages: [TheOne.Avalonia](https://www.nuget.org/packages/TheOne.Avalonia/1.0.0-preview.7) · [TheOne.Avalonia.DataGrid](https://www.nuget.org/packages/TheOne.Avalonia.DataGrid/1.0.0-preview.7). You can also reference the source project; adjust the path for your application:

```sh
dotnet add path/to/YourApp.csproj reference TheOne.Avalonia/TheOne.Avalonia.csproj
```

Target `net10.0` and use Avalonia 12.1.4 consistently. Load the theme in `App.axaml`, replacing `YourApp.App` with your application class:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:one="using:TheOne.Avalonia.Themes"
             x:Class="YourApp.App"
             RequestedThemeVariant="Default">
  <Application.Styles>
    <one:OneTheme />
  </Application.Styles>
</Application>
```

`OneTheme` includes the Fluent support layer and DataGrid templates. Do not also reference the official `Avalonia.Controls.DataGrid` package. Use Avalonia's native controls directly:

```xml
<StackPanel Spacing="12" Classes="normal">
  <TextBox Classes="clearButton" PlaceholderText="Enter a name" />
  <Button Classes="primary" Content="Save" />
  <CalendarDatePicker SelectedDateFormat="Custom"
                      CustomDateFormatString="yyyy-MM-dd" />
</StackPanel>
```

`RequestedThemeVariant="Default"` follows the system theme; `Light` and `Dark` select a specific theme. Apply `Classes="small"`, `normal`, or `large` to a control or container to set its size.

## Documentation

- [Integration and migration](docs/integration/en/README.md): existing-project adoption, platform boundaries, and a portable brief for Codex.
- [Documentation index](docs/README.md): choose a task and language.
- [Complete source contract](docs/api/en/README.md): all exported declarations for precise lookup, including constructors, property accessors and identifiers.
- [Control usage](docs/controls/en/README.md): project-added properties, labeled Classes examples, practical business APIs and related models. PreviewHost properties and actions include short code examples; shared ControlSizing / Motion settings are explained on Home.
- [Theme documentation](docs/theme/en/README.md): colors, styles, sizes, and motion.

## Optional packages

| Optional package | Purpose | Theme registration |
| --- | --- | --- |
| `TheOne.Avalonia.CodeEditor` | Standalone code editor with syntax highlighting, completion, diagnostics and formatting | `CodeEditorTheme` |
| `TheOne.PrintingTools.Avalonia` | Desktop print preview, printer settings, PDF export and Windows / macOS / Linux printing backends | `PrintingToolsTheme` |

Reference these packages **only when needed**. Neither is automatically included by TheOne.Avalonia; each has independent source, versions and documentation. PreviewHost explicitly references both NuGet packages: CodeEditor 1.0.0-preview.2 for source highlighting and PrintingTools 1.0.0-preview.3 for print examples. It registers CodeEditorTheme in App.axaml and PrintingToolsTheme locally in print windows. Consumer applications do not need these preview dependencies or sibling checkouts; see the [integration guide](docs/integration/en/README.md#library-dependencies-differ-from-preview-dependencies).

## Repository layout

| Path | Purpose |
| --- | --- |
| `TheOne.Avalonia/` | Shared themes, controls, and behaviors |
| `design/PreviewHost/` | Interactive preview and consumer examples |
| `docs/controls/` | Control documentation |
| `docs/theme/` | Theme documentation |

## License

[MIT](LICENSE) · [Third-party notices](THIRD-PARTY-NOTICES.md)
