# Avatar

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

[简体中文](../zh-cn/avatar.md) · **English**

`Avatar` displays a local or remote image, with up to two Unicode text elements or a person icon as fallback. Load `OneTheme` before use. It is a display control; wrap it in a native `Button` when a profile action is needed.

```xml
<one:Avatar Source="avares://YourApp/Assets/avatar.png" Text="LM" AutomationProperties.Name="Lin's avatar" />
<one:Avatar Source="https://example.com/avatar.png" Text="林默" Shape="Rounded" />
<one:Avatar Source="{Binding AvatarPath}" Text="AK" Shape="Square" />
```

Declare `xmlns:one="using:TheOne.Avalonia.Controls"` and the concrete `x:DataType` on a view using compiled bindings. Replace YourApp with the actual resource assembly name and include the image as an AvaloniaResource. Supply local files through AvatarPath rather than copying another machine's absolute path.

| Property | Contract |
| --- | --- |
| `Source` | Local absolute/relative path, `file://` URI, HTTP(S) URL, `avares://` URI, or caller-owned `IImage`; accepts strings and `Uri` objects |
| `Text` | Fallback text; trims whitespace and displays at most two graphemes without splitting emoji; blank text uses a person icon |
| `Shape` | `Circle` (default), `Rounded`, or `Square` |
| `CornerRadius` | Radius for `Rounded`; default 8; use equal Width / Height for a circular avatar |
| `Stretch` | Native image stretch; default `UniformToFill` crops to fill the avatar |
| `CurrentImage`, `HasImage` | Read-only decoded image and successful-image state |
| `IsLoading`, `ErrorMessage` | Read-only loading state and failure details; empty source clears errors |
| `Reload()` | Retries the current source while the control is attached |

Inherited Small / Normal / Large densities use 32 / 40 / 56 px square bounds and 12 / 14 / 18 px fallback font sizes. Explicit Width / Height and font settings take priority. The fallback scales to fit. `One.AccentSoft`, `One.Accent`, and `One.Line` supply Light / Dark colors; standard Background, Foreground, BorderBrush, and BorderThickness remain customizable.

Image loading starts on visual attachment. Replacing Source or detaching cancels pending work. Late responses never replace a newer image. Owned bitmaps are disposed on replacement or detach; supplied `IImage` objects remain owned by the caller. Reattachment loads the current source again. Files and network responses are decoded off the UI thread. Remote loading has a 30-second total timeout and 16 MiB response limit. There is no persistent image cache, authentication UI, SVG decoder, or animated-image playback. Relative files resolve from the process working directory. HTTPS uses normal platform certificate validation.

Fallback remains visible during loading and on HTTP, file, or decode errors. Pseudoclasses: `:loading`, `:error`, `:has-image`, `:has-text`. The default template preserves `PART_Surface`, `PART_Fallback`, `PART_Icon`, `PART_Text`, and `PART_Image`. Set `AutomationProperties.Name` to describe the person; the automation peer otherwise uses Text, then “Avatar”.

<!-- visual-guide:start -->
## Appearance and example states

[Capture scope and visual index](visual-index.md). These are actual source-rendered examples; use this page’s usage notes for interaction and binding contracts.

### Avatar

![Avatar — Image/text placeholders, shapes and states; failures retain an identifiable placeholder.](../images/Avatar-light.png)

Image/text placeholders, shapes and states; failures retain an identifiable placeholder.

[View Dark comparison](../images/Avatar-dark.png)

<!-- visual-guide:end -->

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### Avatar

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.Avatar.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CurrentImage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L93) | `Avalonia.Media.IImage?` | read-only; OneWay | `computed / state` | Read-only decoded image and successful-image state |
| [`DisplayText`](../../../TheOne.Avalonia/Controls/Avatar.cs#L97) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`EffectiveCornerRadius`](../../../TheOne.Avalonia/Controls/Avatar.cs#L98) | `Avalonia.CornerRadius` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L96) | `string?` | read-only; OneWay | `computed / state` | Read-only loading state and failure details; empty source clears errors |
| [`HasImage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L95) | `bool` | read-only; OneWay | `computed / state` | Read-only decoded image and successful-image state |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/Avatar.cs#L94) | `bool` | read-only; OneWay | `computed / state` | Read-only loading state and failure details; empty source clears errors |
| [`Shape`](../../../TheOne.Avalonia/Controls/Avatar.cs#L81) | `TheOne.Avalonia.Controls.AvatarShape` | writable; OneWay | `0 / default` | `Circle` (default), `Rounded`, or `Square` Validation: `Enum.IsDefined` Values: `Circle`, `Rounded`, `Square` |
| [`Source`](../../../TheOne.Avalonia/Controls/Avatar.cs#L69) | `object?` | writable; OneWay | `null` | Local absolute/relative path, `file://` URI, HTTP(S) URL, `avares://` URI, or caller-owned `IImage`; accepts strings and `Uri` objects |
| [`Stretch`](../../../TheOne.Avalonia/Controls/Avatar.cs#L87) | `Avalonia.Media.Stretch` | writable; OneWay | `Stretch.UniformToFill` | Native image stretch; default `UniformToFill` crops to fill the avatar Validation: `Enum.IsDefined` Values: `None`, `Fill`, `Uniform`, `UniformToFill` |
| [`Text`](../../../TheOne.Avalonia/Controls/Avatar.cs#L75) | `string?` | writable; OneWay | `null` | Fallback text; trims whitespace and displays at most two graphemes without splitting emoji; blank text uses a person icon |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`Avatar`](../../api/en/TheOne.Avalonia.Controls.Avatar.md) | `void Avatar.Reload()` | Reloads the current source while attached, for example after a failed request. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.AvatarShape`](../../api/en/TheOne.Avalonia.Controls.AvatarShape.md) | 3 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
