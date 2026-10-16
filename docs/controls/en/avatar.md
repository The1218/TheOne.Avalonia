# Avatar

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

Preview: Extensions / Avatar. Run `dotnet run --project design/PreviewHost -- --verify-avatar .preview/avatar` for file/HTTP loading, cancellation, caller ownership, density, Unicode fallback and actual clipping-pixel checks, plus Light / Dark gallery exports and a narrow viewport. The native consumer accepts `--require-aot --verify-avatar .preview/avatar-native`. Network checks use a deterministic loopback HTTP server, without third-party image hosting.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost keeps complete demos on their own pages, with styles, additional properties, APIs/events and models on each page. Only Button, Input and Select share comparison pages.

### Avatar

[API / model reference](../../api/en/TheOne.Avalonia.Controls.Avatar.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CurrentImage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L34) | `Avalonia.Media.IImage?` | read-only; OneWay | `computed / state` | Read-only decoded image and successful-image state |
| [`DisplayText`](../../../TheOne.Avalonia/Controls/Avatar.cs#L38) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`EffectiveCornerRadius`](../../../TheOne.Avalonia/Controls/Avatar.cs#L39) | `Avalonia.CornerRadius` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L37) | `string?` | read-only; OneWay | `computed / state` | Read-only loading state and failure details; empty source clears errors |
| [`HasImage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L36) | `bool` | read-only; OneWay | `computed / state` | Read-only decoded image and successful-image state |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/Avatar.cs#L35) | `bool` | read-only; OneWay | `computed / state` | Read-only loading state and failure details; empty source clears errors |
| [`Shape`](../../../TheOne.Avalonia/Controls/Avatar.cs#L32) | `TheOne.Avalonia.Controls.AvatarShape` | writable; OneWay | `0 / default` | `Circle` (default), `Rounded`, or `Square` Validation: `Enum.IsDefined` Values: `Circle`, `Rounded`, `Square` |
| [`Source`](../../../TheOne.Avalonia/Controls/Avatar.cs#L30) | `object?` | writable; OneWay | `null` | Local absolute/relative path, `file://` URI, HTTP(S) URL, `avares://` URI, or caller-owned `IImage`; accepts strings and `Uri` objects |
| [`Stretch`](../../../TheOne.Avalonia/Controls/Avatar.cs#L33) | `Avalonia.Media.Stretch` | writable; OneWay | `Stretch.UniformToFill` | Native image stretch; default `UniformToFill` crops to fill the avatar Validation: `Enum.IsDefined` Values: `None`, `Fill`, `Uniform`, `UniformToFill` |
| [`Text`](../../../TheOne.Avalonia/Controls/Avatar.cs#L31) | `string?` | writable; OneWay | `null` | Fallback text; trims whitespace and displays at most two graphemes without splitting emoji; blank text uses a person icon |

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
| Avatar | `one-large` | automatic template class; do not bind manually | one&#124;Avatar.one-large => Width=56; Height=56; FontSize=18 |
| Avatar | `one-normal` | automatic template class; do not bind manually | one&#124;Avatar.one-normal => Width=40; Height=40; FontSize=14 |
| Avatar | `one-small` | automatic template class; do not bind manually | one&#124;Avatar.one-small => Width=32; Height=32; FontSize=12 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.Avatar`](../../api/en/TheOne.Avalonia.Controls.Avatar.md) | Class | 10 | 2 | 0 |
| [`TheOne.Avalonia.Controls.AvatarShape`](../../api/en/TheOne.Avalonia.Controls.AvatarShape.md) | Enum | 3 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
