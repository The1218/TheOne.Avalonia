# ImagePreview

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

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

The suite uses a local HTTP server to verify real requests, retry and cancellation races without external network dependencies. It also checks local paths, Bitmap ownership, collection changes, compiled bindings, full-screen behavior, both themes, three densities, narrow viewports and rendered pixels.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### ImagePreview

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanNext`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L188) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanPrevious`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L182) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ChromeHeight`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L146) | `double` | writable; OneWay | `44` | Height of the image-preview chrome/tool area in DIPs. Validation: `value => double.IsFinite(value) && value > 0` |
| [`CurrentImage`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L225) | `Avalonia.Media.IImage?` | read-only; CLR / n/a | `accessor-defined / state` | The currently decoded image. Images created from paths are owned by the viewer. |
| [`DisplayTitle`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L206) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L200) | `string?` | read-only; OneWay | `computed / state` | Current failure/validation message; an error display is not evidence of successful completion. |
| [`HasError`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L170) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasImage`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L164) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ImageCount`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L152) | `int` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsFitToWindow`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L134) | `bool` | writable; TwoWay | `true` | Fits the image into the viewport; see image-preview interaction rules for manual zoom behavior. |
| [`IsFullScreen`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L194) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L158) | `bool` | read-only; OneWay | `computed / state` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |
| [`IsMultiple`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L176) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L104) | `System.Collections.IEnumerable?` | writable; OneWay | `null` | Bind source models rather than precreated controls; use notified collections for changes and a bounded viewport. |
| [`PageText`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L212) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L110) | `int` | writable; TwoWay | `0 / default` | Zero-based displayed-item index; range handling follows the owning control's selection contract. |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L128) | `bool` | writable; OneWay | `false` | Shows the close button; the host and associated commands retain control over closing. |
| [`ShowHeader`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L122) | `bool` | writable; OneWay | `true` | Shows the header area without deleting the model's title content. |
| [`Source`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L98) | `object?` | writable; OneWay | `null` | Source object/address of a supported type; usage documentation defines resource ownership/loading limits. |
| [`Title`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L116) | `string?` | writable; OneWay | `null` | Display title, distinct from business identity or a localization key. |
| [`ZoomFactor`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L140) | `double` | writable; TwoWay | `1` | Image display scale factor; changes the view rather than source pixels. Validation: `value => double.IsFinite(value) && value is >= .001 and <= 64` |
| [`ZoomText`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L218) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

### ImagePreviewSurface

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.ImagePreviewSurface.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`AlternateCheckerBrush`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L48) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Alternate transparency checker brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`CheckerBrush`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L42) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Primary transparency checker brush; use One.* dynamic resources for themes. Explicit local values take precedence. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`ImagePreview`](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md) | `void ImagePreview.Previous()` | Moves to the previous available preview image. |
| [`ImagePreview`](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md) | `void ImagePreview.Next()` | Moves to the next available preview image. |
| [`ImagePreview`](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md) | `void ImagePreview.Reload()` | Reloads the current image under its source/error loading contract. |
| [`ImagePreview`](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md) | `void ImagePreview.FitToWindow()` | Fits the image into the viewport without modifying source pixels. |
| [`ImagePreview`](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md) | `void ImagePreview.ActualSize()` | Displays the image at actual pixel scale, potentially creating scrollable overflow. |
| [`ImagePreview`](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md) | `void ImagePreview.ZoomIn()` | Zooms the current preview in within supported limits. |
| [`ImagePreview`](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md) | `void ImagePreview.ZoomOut()` | Zooms the current preview out within supported limits. |
| [`ImagePreview`](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md) | `void ImagePreview.ToggleFullScreen()` | Toggles image-preview fullscreen presentation. |

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`ImagePreview`](../../api/en/TheOne.Avalonia.Controls.ImagePreview.md) | `CloseRequested` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

### Related models

No additional related data models.

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
