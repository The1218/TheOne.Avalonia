# ImagePreview

[简体中文](../zh-cn/image-preview.md) · **English**

A lightweight single/multiple image viewer for HTTP(S), local paths / file URIs and Bitmap (also accepts IImage). Load OneTheme and declare the view's correct x:DataType for compiled bindings.

## Single and multiple images

```xml
<one:ImagePreview Source="{Binding Picture}" Height="440" />
<one:ImagePreview ItemsSource="{Binding Pictures}"
                  SelectedIndex="{Binding PictureIndex}" Height="440" />
```

- Picture accepts a Bitmap, local path string, or HTTP(S) string / Uri. Relative local paths resolve against the process working directory. Bitmaps do not need a temporary file.
- Pictures is IEnumerable and may mix these source types. Non-null ItemsSource selects collection mode, including an empty collection, and takes precedence over Source. Clearing ItemsSource restores Source. Prefer binding one input at a time.
- SelectedIndex is zero-based and defaults to TwoWay; empty lists use -1. Out-of-range indices clamp to the available range and write back. ObservableCollection changes update navigation; insertion and movement preserve the selected source object when possible.
- Single image mode hides navigation and count. Multiple images show previous / next and a page count, disable the first / last boundary and do not wrap. Failed images do not block navigation.
- Supply finite Height or a Grid star row, rather than unbounded height in a vertical StackPanel.

## Loading and ownership

File decoding and HTTP(S) loading run in the background. Switching images or detaching cancels pending requests and rejects stale results. Reattaching reloads the current source. HTTP timeout is 30 seconds. Use the retry button or Reload() after failure. Read-only IsLoading, HasImage, HasError and ErrorMessage expose state; ErrorMessage retains diagnostics while the default UI shows a short failure message.

Null sources display an empty state. Requests display a loading indicator; missing files, HTTP failures, unsupported formats and decoding failures display retry. Transparent images use a checkerboard. Loading failures remain distinguishable from empty content.

Caller-supplied Bitmap / IImage instances remain caller-owned and are never disposed by the viewer. Dispose them only after all previews, including full-screen windows, have finished. Bitmaps loaded from paths are viewer-owned and released when replaced or detached. Cross-image caching, animation playback and EXIF parsing are not provided.

## Input and windows

The wheel zooms around the pointer; dragging pans; double-click toggles fit and 1:1. Panning stays inside the image bounds, with smaller dimensions centered. At 1:1 one Bitmap pixel maps to one physical screen pixel, accounting for host RenderScaling. ZoomFactor 1 means 100%, with a range of 0.001 to 64. Setting ZoomFactor leaves fit mode. IsFitToWindow is bindable.

The toolbar invokes ZoomIn(), ZoomOut(), FitToWindow() and ActualSize(). Below 380 DIP, the separate 1:1 button hides; double-click remains available. When focus is inside the viewer, arrow keys switch images and plus / minus zoom. Native buttons retain their keyboard and focus behavior.

ShowHeader defaults to true. Title overrides the name inferred from a path; unnamed Bitmaps use the generic preview title. ShowCloseButton defaults to false. When enabled, it raises the bubbling CloseRequested event for the host to dismiss its overlay.

Inside a desktop Window, ToggleFullScreen() or the full-screen button opens a separate owned window while retaining the embedded preview and its original SelectedIndex binding. Selection synchronizes back to the original view. Escape / close exits only the full-screen preview. Non-Window hosts do not open a full-screen window. IsFullScreen is read-only.

## Theme and template

Colors reuse One.Canvas, One.Subtle, One.Surface, One.Text, One.Muted and One.Line. Light / dark themes and all three inherited ControlSizing.Size densities are supported. ChromeHeight is 36 / 44 / 52; default corner radius is 8. Background, Foreground, BorderBrush, BorderThickness and CornerRadius are overridable.

Template parts: PART_Viewport (ImagePreviewSurface), PART_Previous, PART_Next, PART_Fit, PART_Actual, PART_ZoomOut, PART_ZoomIn, PART_Retry, PART_FullScreen and PART_Close. Buttons remain native Button instances. ImagePreviewSurface draws the image and checkerboard directly, without per-pixel controls. Pseudoclasses: :multiple, :loading, :error, :has-image, :compact, :fullscreen.

## Preview and verification

PreviewHost's ImagePreview page includes a Bitmap and transparent sample, HTTP(S) input, local file selection, mixed-source collections and compiled SelectedIndex bindings. Enter a network address explicitly; the demo does not contact an external service by default. The bundled alpine lake sample was generated with ImageGen.

```bash
dotnet build design/PreviewHost/PreviewHost.csproj -c Release --artifacts-path .preview/image-preview/artifacts
dotnet .preview/image-preview/artifacts/bin/PreviewHost/release/PreviewHost.dll --verify-image-preview .preview/image-preview/managed
# macOS: native checks only for affected AOT/binding/metadata changes.
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 --artifacts-path .preview/image-preview/native-artifacts -o .preview/image-preview/native
.preview/image-preview/native/NativeAotSmoke --require-aot --verify-image-preview .preview/image-preview/native-pixels
```

The suite uses a local HTTP server to verify real requests, retry and cancellation races without external network dependencies. It also checks local paths, Bitmap ownership, collection changes, compiled bindings, full-screen behavior, both themes, three densities, narrow viewports and rendered pixels.

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section enumerates declarations added by this library; native inherited members remain Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. Type links provide full model, method, event and enum signatures. PreviewHost's Control Laboratory (top-right action on each page) groups live examples by styles, properties, APIs/events and models; complex models use the complete business samples.

### ImagePreview

[API / model reference](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanNext`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L56) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanPrevious`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L55) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ChromeHeight`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L49) | `double` | writable; OneWay | `44` | Height of the image-preview chrome/tool area in DIPs. Validation: `value => double.IsFinite(value) && value > 0` |
| [`CurrentImage`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L64) | `Avalonia.Media.IImage?` | read-only; CLR / n/a | `null` | The currently decoded image. Images created from paths are owned by the viewer. |
| [`DisplayTitle`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L59) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L58) | `string?` | read-only; OneWay | `computed / state` | Current failure/validation message; an error display is not evidence of successful completion. |
| [`HasError`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L53) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasImage`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L52) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ImageCount`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L50) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsFitToWindow`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L47) | `bool` | writable; TwoWay | `true` | Fits the image into the viewport; see image-preview interaction rules for manual zoom behavior. |
| [`IsFullScreen`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L57) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L51) | `bool` | read-only; OneWay | `computed / state` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |
| [`IsMultiple`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L54) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L42) | `System.Collections.IEnumerable?` | writable; OneWay | `null` | Bind source models rather than precreated controls; use notified collections for changes and a bounded viewport. |
| [`PageText`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L60) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L43) | `int` | writable; TwoWay | `0` | Zero-based displayed-item index; range handling follows the owning control's selection contract. |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L46) | `bool` | writable; OneWay | `false` | Shows the close button; the host and associated commands retain control over closing. |
| [`ShowHeader`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L45) | `bool` | writable; OneWay | `true` | Shows the header area without deleting the model's title content. |
| [`Source`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L41) | `object?` | writable; OneWay | `null` | Source object/address of a supported type; usage documentation defines resource ownership/loading limits. |
| [`Title`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L44) | `string?` | writable; OneWay | `null` | Display title, distinct from business identity or a localization key. |
| [`ZoomFactor`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L48) | `double` | writable; TwoWay | `1` | Image display scale factor; changes the view rather than source pixels. Validation: `value => double.IsFinite(value) && value is >= .001 and <= 64` |
| [`ZoomText`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L61) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### ImagePreviewSurface

[API / model reference](../../api/en/TheOne.Avalonia.Controls.ImagePreviewSurface.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`AlternateCheckerBrush`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L15) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Alternate transparency checker brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`CheckerBrush`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L14) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Primary transparency checker brush; use One.* dynamic resources for themes. Explicit local values take precedence. |

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
| ImagePreview | `one-large` | automatic template class; do not bind manually | one&#124;ImagePreview.one-large => ChromeHeight=52; FontSize=16 |
| ImagePreview | `one-normal` | automatic template class; do not bind manually | one&#124;ImagePreview.one-normal => ChromeHeight=44; FontSize=14 |
| ImagePreview | `one-small` | automatic template class; do not bind manually | one&#124;ImagePreview.one-small => ChromeHeight=36; FontSize=12 |

`small` / `normal` / `large` are public density inputs; `one-small` / `one-normal` / `one-large` are automatic internal classes. `:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without additional appearance classes use their default theme and properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.ImagePreview`](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md) | Class | 22 | 9 | 1 |
| [`TheOne.Avalonia.Controls.ImagePreviewSurface`](../../api/en/TheOne.Avalonia.Controls.ImagePreviewSurface.md) | Class | 2 | 2 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
