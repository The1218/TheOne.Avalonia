# ImagePreviewSurface

[English](../en/TheOne.Avalonia.Controls.ImagePreviewSurface.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ImagePreviewSurface.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ImagePreviewSurface` · Class · `Avalonia.Controls.Control`

Clipped, directly drawn image viewport used by the ImagePreview template.

[Source / 声明](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`AlternateCheckerBrush`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L48) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Alternate transparency checker brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`CheckerBrush`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L42) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Primary transparency checker brush; use One.* dynamic resources for themes. Explicit local values take precedence. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void ImagePreviewSurface.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L180) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`ImagePreviewSurface.ImagePreviewSurface()`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L29) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> ImagePreviewSurface.CheckerBrushProperty`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> ImagePreviewSurface.AlternateCheckerBrushProperty`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
