# ThemeColorPicker and Light / Dark switching

[简体中文](../zh-cn/theme-color-picker.md) · **English**

Palette and mode are independent settings: `OneTheme.ColorTheme` selects `One.*` color resources; `RequestedThemeVariant` selects Light / Dark. `ThemeColorPicker` specializes native ComboBox, while `ThemeButton` is a ToggleButton with built-in icons.

After loading OneTheme, pass an enum on the UI thread to switch palettes without finding a window, dictionary, or theme instance:

```csharp
using TheOne.Avalonia.Themes;

OneTheme.SetColor(ThemeColor.Atlas);
OneTheme.SetColor(ThemeColor.Nova); // Restore the default Nova palette.
```

This API updates the application palette, preserves property bindings, and leaves Light / Dark unchanged. Invalid enum values throw ArgumentOutOfRangeException; calling without OneTheme registered in Application.Styles throws InvalidOperationException.

## Palette resources

```xml
<Application.Styles>
  <themes:OneTheme ColorTheme="Atlas" />
</Application.Styles>
<!-- themes: using:TheOne.Avalonia.Themes -->
```

`ThemeColor` contains `Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, and `VegaDeep`. The default label is restored to **Nova**. Default was renamed to Nova, retaining numeric value 0; other numeric values remain unchanged. Light / Dark semantic colors for the other nine palettes map from [Eremex palette resources](https://github.com/Eremex/controlthemes/tree/5f6cb3ad8b904cead6bd432f62596648cc9a1039/Source/Eremex.Avalonia.Themes.DeltaDesign/PaletteResources), without adding Eremex packages or templates.

Resources cover neutral surfaces, text, accents, selection, focus, borders, and status colors. Soft status backgrounds, card Footer, and gradients are TheOne derivations; chart base colors and six series retain shared resources. Switching palettes does not recreate views, change density, or switch Light / Dark. Fluent fallback controls also receive the updated accent.

## Dropdown selector

```xml
<one:ThemeColorPicker Width="170"
                     SelectThemeColor="{Binding Palette, Mode=TwoWay}" />
<!-- one: using:TheOne.Avalonia.Controls -->
```

The VM's `Palette` is a `ThemeColor`. Without a binding, the selector connects to the `OneTheme` directly registered in `Application.Styles`. Each entry shows its name and Light / Dark accent swatches. Native pointer, arrow-key, and Enter selection retain ComboBox borders, item spacing, disabled behavior, and three densities.

This is an enum selector: `ItemsSource` and `OneTheme.ColorThemes` contain `ThemeColor` values, and `SelectedItem` is an enum rather than a ThemeColorOption wrapper. `SelectedColor` provides a strongly typed property; native SelectedItem also exposes the selected enum.

`SelectThemeColor` binds two-way by default. User selection updates the application OneTheme; programmatic changes to `OneTheme.ColorTheme` synchronize attached selectors. Change palettes on the UI thread. Without an application OneTheme, selection remains available but does not alter other theme libraries. Applications using a separate OneTheme in local Styles should set its ColorTheme explicitly.

```csharp
using TheOne.Avalonia.Themes;

OneTheme.SetColor(ThemeColor.Vega);
// Read-only enum list, also usable directly by ComboBox: OneTheme.ColorThemes.
```

## Switching and refresh

### Persisting and restoring preferences

`SelectThemeColor` is a `StyledProperty<ThemeColor>` with TwoWay binding by default. `SelectedColor` remains compatible and synchronized. Bind one of these properties per control, avoiding conflicting configuration sources. On first attachment an explicit Nova binding (enum value 0) is applied; an unbound picker follows the current application palette. Reattachment follows the current application instead of restoring stale state.

```xml
<!-- Declare the view's actual VM x:DataType; Palette is a ThemeColor. -->
<one:ThemeColorPicker SelectThemeColor="{Binding Palette}" />
```

The control exposes the preference without reading or writing files. Store `Palette.ToString()` using your application settings. After registering OneTheme, on the UI thread and before creating the main window, restore it:

```csharp
var palette = Enum.TryParse<ThemeColor>(savedName, out var value) && Enum.IsDefined(value)
    ? value : ThemeColor.Nova;
OneTheme.SetColor(palette);
// Initialize VM.Palette with the same value; persist subsequent changes asynchronously.
```

Persist and restore `Application.Current.RequestedThemeVariant` separately for Light / Dark. Avoid synchronous disk I/O in selection callbacks.

### Refresh boundaries

Solid brushes retain their identity per resource key and repaint existing glyphs when colors change, avoiding unrelated text measurement. PreviewHost forwards editor palette notifications only when editor resource values change; One.* backgrounds, Light / Dark and custom syntax resource overrides still update. Gantt semantic aliases and workflow canvas, notes, selection shadows and minimap follow the active palette without rebuilding nodes or changing documents, selection or viewport.

NavMenu reuses icon, text and tooltip containers while scrolling, and does not rewrite unchanged row indentation resources. Initial loading, first palette creation and actual text layout still have costs; batching resource notifications does not promise a fixed latency on every device.

Light / Dark color snapshots and immutable gradients are cached per palette. Published solid brushes retain their identity and update their colors before the batched notification. One.*Color values and Fluent accents still receive normal resource notifications. Treat theme-owned brushes as read-only; override resource keys for customization. Switching does not remove or rebuild the resource tree.

ColorTheme and resource queries immediately return the new palette; visual resource notifications coalesce at the next Render priority. Consecutive calls in one dispatcher turn refresh only the final palette. Repeating the current value, or returning to the published palette before refresh, produces no redundant notification. Code inspecting control brushes synchronously should wait until the next render pass for their final visual state. This avoids synchronously walking the visible UI for every intermediate choice.

## Icon-only ThemeButton

```xml
<one:ThemeButton Scope="Application" />
```

ThemeButton now inherits ToggleButton and shows one built-in moon / sun icon without text, retaining tooltips and accessible names. `IsChecked=true` represents the effective Dark mode; false represents Light. External mode changes synchronize checked state. Clicking or pressing Space switches mode without changing ColorTheme; disabled behavior remains native.

The default `Scope="Window"` updates the nearest window's RequestedThemeVariant. `Application` updates the application setting; explicitly themed windows retain their local settings. Configure the button in OneWindow's title bar through `ThemeButtonScope`. See [windows](../../controls/en/windows.md) for window and owner behavior.

Changing the base class from Button to ToggleButton is an inheritance change. Styles targeting ThemeButton should account for ToggleButton states. Its default appearance has a dedicated control theme, and its icons do not depend on Content.

## PreviewHost and verification

PreviewHost opens the standalone top-level Home item by default. The top ThemeColorPicker selects resources; the title-bar ThemeButton switches Light / Dark only. Home snippets use the `TheOne.Avalonia.CodeEditor` NuGet package for read-only highlighting, wrapping, and copying. Expand each page's source panel to switch between its AXAML, C#, and related VM files. Sources are embedded, requiring no local repository path; the panel loads on demand in a bounded viewport.

```sh
dotnet build design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/theme-code/artifacts
# Run each command against PreviewHost.dll from those artifacts:
dotnet <PreviewHost.dll> --verify-theme-colors .preview/theme-code/palettes
dotnet <PreviewHost.dll> --verify-preview-code .preview/theme-code/code
dotnet <PreviewHost.dll> --verify-home .preview/theme-code/home
# Focused enum API, compiled-binding, and switching-performance checks:
dotnet <PreviewHost.dll> --verify-theme-api
dotnet <PreviewHost.dll> --verify-theme-performance .preview/theme-api/optimized.json
```

These scoped checks cover the palette selector, ThemeButton, code panel, and Home navigation, including native input, live resources, Light / Dark, densities, narrow viewports, and actual rendering. They do not run other controls' full regression suites.

For native consumption, publish the existing NativeAotSmoke project with `ThemeOnlyPreview=true`, `PublishAot=true`, and `PublishTrimmed=true`, then run with `--require-aot`. This executes only enum selector and compiled-binding checks.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost's Control Laboratory (top-right action on each page) groups live examples by styles, properties, APIs/events and models; complex models use the complete business samples.

### ThemeButton

[API / model reference](../../api/en/TheOne.Avalonia.Controls.ThemeButton.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Scope`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L16) | `TheOne.Avalonia.Controls.ThemeButtonScope` | writable; OneWay | `0 / default` | Scope of Light/Dark switching; does not change the ThemeColor palette enum. Values: `Window`, `Application` |

### ThemeColorPicker

[API / model reference](../../api/en/TheOne.Avalonia.Controls.ThemeColorPicker.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`SelectThemeColor`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L17) | `TheOne.Avalonia.Themes.ThemeColor` | writable; TwoWay | `0 / default` | The selected palette, suitable for a persisted two-way enum binding. Validation: `value => Enum.IsDefined(value)` Values: `Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |
| [`SelectedColor`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L25) | `TheOne.Avalonia.Themes.ThemeColor` | writable; TwoWay | `0 / default` | Compatibility alias of SelectedThemeColor; both represent the same palette enum. Prefer binding SelectedThemeColor. Validation: `value => Enum.IsDefined(value)` Values: `Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |

### OneTheme

[API / model reference](../../api/en/TheOne.Avalonia.Themes.OneTheme.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ColorTheme`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L14) | `TheOne.Avalonia.Themes.ThemeColor` | writable; OneWay | `0 / default` | Color palette; changing this does not change the requested Light / Dark variant. Validation: `value => Enum.IsDefined(value)` Values: `Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |
| [`ColorThemes`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L15) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Themes.ThemeColor>` | read-only; CLR / n/a | `Array.AsReadOnly(Enum.GetValues<ThemeColor>())` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### ThemeColor

[API / model reference](../../api/en/TheOne.Avalonia.Themes.ThemeColor.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### Shared attached properties

[ControlSizing](../../api/en/TheOne.Avalonia.Styling.ControlSizing.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | writable; OneWay; attached; inherits | `ControlSize.Normal` | Shared Small/Normal/Large density inherited by descendants; explicit local values/classes can override it. Values: `Small`, `Normal`, `Large` |

[Motion](../../api/en/TheOne.Avalonia.Styling.Motion.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | writable; OneWay; attached; inherits | `true` | Inherited motion switch; disabling goes to final layout/state while preserving native input. |

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| ThemeButton | `one-large` | automatic template class; do not bind manually | one&#124;ThemeButton.one-large => Width=44; Height=44 |
| ThemeButton | `one-normal` | automatic template class; do not bind manually | one&#124;ThemeButton.one-normal => Width=36; Height=36 |
| ThemeButton | `one-small` | automatic template class; do not bind manually | one&#124;ThemeButton.one-small => Width=30; Height=30 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.ThemeButton`](../../api/en/TheOne.Avalonia.Controls.ThemeButton.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/en/TheOne.Avalonia.Controls.ThemeButtonScope.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeColorPicker`](../../api/en/TheOne.Avalonia.Controls.ThemeColorPicker.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Themes.OneTheme`](../../api/en/TheOne.Avalonia.Themes.OneTheme.md) | Class | 2 | 2 | 0 |
| [`TheOne.Avalonia.Themes.ThemeColor`](../../api/en/TheOne.Avalonia.Themes.ThemeColor.md) | Enum | 10 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
