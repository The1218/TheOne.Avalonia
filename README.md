# TheOne.Avalonia

[简体中文](README.zh-cn.md) · [English](README.en.md)

A cross-platform theme and control library built on Avalonia's native controls. It includes light and dark themes, three control sizes, multiple control styles, and gradient palettes. PreviewHost provides interactive examples.

- Consistent themes for buttons, inputs, selectors, date and time pickers, lists, trees, menus, and layout controls.
- Fourteen chart types, plus NavMenu, InfoCard, Tag, Step, window, and messaging extensions.
- DataGrid with compiled bindings, automatic entity-property filters, frozen columns on both sides, and numeric summaries.

## Requirements and versions

| Component | Version |
| --- | --- |
| .NET SDK / TargetFramework | .NET 10 / `net10.0` |
| Avalonia core, Skia, Fluent | 12.1.4 |
| DataGrid | Based on 12.1.2; package: `TheOne.Avalonia.DataGrid` |
| TheOne.Avalonia / TheOne.Avalonia.DataGrid | 1.0.0-preview.2 |
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
dotnet add path/to/YourApp.csproj package TheOne.Avalonia --version 1.0.0-preview.2
```

NuGet packages: [TheOne.Avalonia](https://www.nuget.org/packages/TheOne.Avalonia/1.0.0-preview.2) · [TheOne.Avalonia.DataGrid](https://www.nuget.org/packages/TheOne.Avalonia.DataGrid/1.0.0-preview.2). You can also reference the source project; adjust the path for your application:

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

- [Control documentation](docs/controls/en/README.md): properties, behavior, and usage examples.
- [Theme documentation](docs/theme/en/README.md): colors, styles, sizes, and motion.

## Repository layout

| Path | Purpose |
| --- | --- |
| `TheOne.Avalonia/` | Shared themes, controls, and behaviors |
| `design/PreviewHost/` | Interactive preview and consumer examples |
| `docs/controls/` | Control documentation |
| `docs/theme/` | Theme documentation |

## License

[MIT](LICENSE) · [Third-party notices](THIRD-PARTY-NOTICES.md)
