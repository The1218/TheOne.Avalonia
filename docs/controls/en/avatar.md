# Avatar

[简体中文](../zh-cn/avatar.md) · **English**

`Avatar` displays a local or remote image, with up to two Unicode text elements or a person icon as fallback. Load `OneTheme` before use. It is a display control; wrap it in a native `Button` when a profile action is needed.

```xml
<one:Avatar Source="/Users/me/Pictures/avatar.jpg" Text="LM" AutomationProperties.Name="Lin's avatar" />
<one:Avatar Source="https://example.com/avatar.png" Text="林默" Shape="Rounded" />
<one:Avatar Source="{Binding AvatarPath}" Text="AK" Shape="Square" />
```

Declare `xmlns:one="using:TheOne.Avalonia.Controls"` and the concrete `x:DataType` on a view using compiled bindings.

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
