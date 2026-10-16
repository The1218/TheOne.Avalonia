# ImagePreview

[English](../en/TheOne.Avalonia.Controls.ImagePreview.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ImagePreview.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ImagePreview` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A single/multiple image viewer for HTTP(S), local paths and caller-owned Bitmap instances.

[Source / 声明](../../../TheOne.Avalonia/Controls/ImagePreview.Loading.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

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

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void ImagePreview.Previous()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L193) | Moves to the previous available preview image. |
| [`void ImagePreview.Next()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L194) | Moves to the next available preview image. |
| [`void ImagePreview.Reload()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L195) | Reloads the current image under its source/error loading contract. |
| [`void ImagePreview.FitToWindow()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L196) | Fits the image into the viewport without modifying source pixels. |
| [`void ImagePreview.ActualSize()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L200) | Displays the image at actual pixel scale, potentially creating scrollable overflow. |
| [`void ImagePreview.ZoomIn()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L201) | Zooms the current preview in within supported limits. |
| [`void ImagePreview.ZoomOut()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L202) | Zooms the current preview out within supported limits. |
| [`void ImagePreview.ToggleFullScreen()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L224) | Toggles image-preview fullscreen presentation. |
| [`ImagePreview.ImagePreview()`](../../../TheOne.Avalonia/Controls/ImagePreview.Loading.cs#L8) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<RoutedEventArgs>? ImagePreview.CloseRequested`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L62) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<object?> ImagePreview.SourceProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IEnumerable?> ImagePreview.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<int> ImagePreview.SelectedIndexProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> ImagePreview.TitleProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> ImagePreview.ShowHeaderProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> ImagePreview.ShowCloseButtonProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> ImagePreview.IsFitToWindowProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> ImagePreview.ZoomFactorProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> ImagePreview.ChromeHeightProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, int> ImagePreview.ImageCountProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.IsLoadingProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.HasImageProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.HasErrorProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.IsMultipleProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.CanPreviousProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.CanNextProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.IsFullScreenProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, string?> ImagePreview.ErrorMessageProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, string> ImagePreview.DisplayTitleProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L37) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, string> ImagePreview.PageTextProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImagePreview, string> ImagePreview.ZoomTextProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L39) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<RoutedEventArgs> ImagePreview.CloseRequestedEvent`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L40) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
