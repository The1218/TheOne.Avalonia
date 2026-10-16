# ImagePreviewSurface

[English](../en/TheOne.Avalonia.Controls.ImagePreviewSurface.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ImagePreviewSurface.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ImagePreviewSurface` · Class · `Avalonia.Controls.Control`

Clipped, directly drawn image viewport used by the ImagePreview template.

[Source / 声明](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`AlternateCheckerBrush`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L15) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Alternate transparency checker brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`CheckerBrush`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L14) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Primary transparency checker brush; use One.* dynamic resources for themes. Explicit local values take precedence. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void ImagePreviewSurface.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L103) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`ImagePreviewSurface.ImagePreviewSurface()`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L33) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IBrush?> ImagePreviewSurface.CheckerBrushProperty`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> ImagePreviewSurface.AlternateCheckerBrushProperty`](../../../TheOne.Avalonia/Controls/ImagePreviewSurface.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |
