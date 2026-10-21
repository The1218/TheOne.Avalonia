# ImageUpload

[English](../en/TheOne.Avalonia.Controls.ImageUpload.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ImageUpload.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ImageUpload` · Class · `TheOne.Avalonia.Controls.Upload`

[Inherited additional members: Upload](TheOne.Avalonia.Controls.Upload.md)

A single-image card with an accessible hover toolbar and a read-only preview mode.

[Source / 声明](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L22)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`DisplayHint`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L65) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HintText`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L62) | `string?` | writable; OneWay | `null` | HintText configuration on ImageUpload; type and initial value are listed here. HintTextProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`ImageHeight`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L63) | `double` | writable; OneWay | `240` | ImageHeight configuration on ImageUpload; type and initial value are listed here. ImageHeightProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => double.IsFinite(value) && value > 0` |
| [`ImageStretch`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L64) | `Avalonia.Media.Stretch` | writable; OneWay | `Stretch.Uniform` | ImageStretch configuration on ImageUpload; type and initial value are listed here. ImageStretchProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Values: `None`, `Fill`, `Uniform`, `UniformToFill` |
| [`Mode`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L60) | `TheOne.Avalonia.Controls.ImageUploadMode` | writable; OneWay | `0 / default` | Mode configuration on ImageUpload; type and initial value are listed here. ModeProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `value => value is ImageUploadMode.Upload or ImageUploadMode.Preview` Values: `Upload`, `Preview` |
| [`ViewCommand`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L66) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `null` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`ViewText`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L61) | `string` | writable; OneWay | `"查看"` | ViewText configuration on ImageUpload; type and initial value are listed here. ViewTextProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void ImageUpload.View()`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L99) | Opens the image viewer; Escape or the close button dismisses it. |
| [`ImageUpload.ImageUpload()`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L55) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<ImageUploadMode> ImageUpload.ModeProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> ImageUpload.ViewTextProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> ImageUpload.HintTextProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> ImageUpload.ImageHeightProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<Stretch> ImageUpload.ImageStretchProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ImageUpload, string> ImageUpload.DisplayHintProperty`](../../../TheOne.Avalonia/Controls/ImageUpload.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ImageUploadMode`](TheOne.Avalonia.Controls.ImageUploadMode.md)
