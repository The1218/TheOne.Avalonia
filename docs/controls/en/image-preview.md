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
dotnet build TheOne.Avalonia.slnx
dotnet design/PreviewHost/bin/Debug/net10.0/PreviewHost.dll --verify-image-preview .preview/image-preview/managed
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 -o .preview/image-preview/native
.preview/image-preview/native/NativeAotSmoke --require-aot --verify-image-preview .preview/image-preview/native-pixels
```

The suite uses a local HTTP server to verify real requests, retry and cancellation races without external network dependencies. It also checks local paths, Bitmap ownership, collection changes, compiled bindings, full-screen behavior, both themes, three densities, narrow viewports and rendered pixels.
