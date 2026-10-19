# ReportElement

[English](../en/TheOne.Avalonia.Reporting.ReportElement.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportElement.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportElement` · Class · `TheOne.Avalonia.Reporting.ReportObservable`

[Inherited additional members: ReportObservable](TheOne.Avalonia.Reporting.ReportObservable.md)

A freely positioned report element. Geometry is measured in millimetres on the page.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L68)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`BarcodeShowText`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L120) | `bool` | writable; CLR / n/a | `true` | Whether to show source text below a barcode, without changing encoded content. |
| [`Bold`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L144) | `bool` | writable; CLR / n/a | `false` | Renders text bold/semibold while wrapping/layout still use actual font metrics. |
| [`FieldKey`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L108) | `string` | writable; CLR / n/a | `""` | Explicitly registered field key rather than a reflection path; must match a source field. |
| [`Fill`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L220) | `string` | writable; CLR / n/a | `"Transparent"` | Report element fill-color string; Transparent is transparent and values must be valid Avalonia colors. |
| [`FontFamily`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L150) | `string` | writable; CLR / n/a | `"Microsoft YaHei"` | Font family for text rendering; Avalonia handles fallback and actual text metrics. |
| [`FontSize`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L138) | `double` | writable; CLR / n/a | `12` | Report font size in DIPs, with document/element range validation performed by the paginator. |
| [`Foreground`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L214) | `string` | writable; CLR / n/a | `"#212222"` | Foreground brush; retain text, status and disabled contrast when overriding. |
| [`Height`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L96) | `double` | writable; CLR / n/a | `12` | Height configuration; preserve content/text/data contracts; fixed values can override theme density. |
| [`ImageData`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L181) | `string` | writable; CLR / n/a | `""` | Embedded PNG/JPEG/WebP image bytes, encoded as base64. No external file dependency. |
| [`ImageFit`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L208) | `TheOne.Avalonia.Reporting.ReportImageFit` | writable; CLR / n/a | `0 / default` | Image-fit enum: Contain preserves the whole image, Cover crops to fill, and Fill stretches. Values: `Contain`, `Cover`, `Fill` |
| [`ImageName`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L187) | `string` | writable; CLR / n/a | `""` | Display name for an embedded image whose bytes remain in ImageData. |
| [`ImageUrl`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L193) | `string` | writable; CLR / n/a | `""` | Remote image URL, with loaded results separated from source configuration under report loading rules. |
| [`IsChecked`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L114) | `bool` | writable; CLR / n/a | `false` | Checked state of a report checkbox element. |
| [`Italic`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L156) | `bool` | writable; CLR / n/a | `false` | Italic text styling, subject to font-system fallback when unsupported. |
| [`Kind`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L76) | `TheOne.Avalonia.Reporting.ReportElementKind` | writable; CLR / init-only | `0 / default` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Text`, `Table`, `Line`, `Rectangle`, `PageNumber`, `Image`, `CheckBox`, `Barcode`, `QRCode` |
| [`ListStyle`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L174) | `TheOne.Avalonia.Reporting.ReportTextListStyle` | writable; CLR / n/a | `0 / default` | Report text-line list prefix: none, bullet or checkbox style. Values: `None`, `Bullet`, `Check` |
| [`Name`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L70) | `string` | writable; CLR / n/a | `"元素"` | Name used for presentation/identity; keep it consistent with templates and keys. |
| [`QRCodeErrorCorrection`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L126) | `TheOne.Avalonia.Controls.QRCodeErrorCorrection` | writable; CLR / n/a | `QRCodeErrorCorrection.Medium` | Error correction for a QR report element, default Medium; retained by templates and undo/redo. Values: `Low`, `Medium`, `Quartile`, `High` |
| [`QRCodeQuietZone`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L132) | `int` | writable; CLR / n/a | `4` | Quiet-zone modules around a QR report element, range 4–32, default 4; shared by design, preview and PDF. |
| [`ShowBorder`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L226) | `bool` | writable; CLR / n/a | `false` | Shows a report element border without changing its geometry. |
| [`TableHeaderHeightMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L202) | `double` | writable; CLR / n/a | `11` | TableHeaderHeightMm: page/element geometry in millimetres. Nullable margins fall back to MarginMm. Pagination converts to DIPs; do not preapply view zoom. |
| [`Text`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L102) | `string` | writable; CLR / n/a | `"文本"` | Input/display text; notifications and validation follow the model/native editor contract. |
| [`TextAlignment`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L168) | `TheOne.Avalonia.Reporting.ReportTextAlignment` | writable; CLR / n/a | `0 / default` | Report text-alignment enum, including left, right, center and justified. Values: `Left`, `Center`, `Right`, `Justify` |
| [`Underline`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L162) | `bool` | writable; CLR / n/a | `false` | Underlines report text, distinct from an input control's underlined class. |
| [`Width`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L90) | `double` | writable; CLR / n/a | `60` | Width configuration; units follow the owning model, without changing data-coordinate meaning. |
| [`X`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L78) | `double` | writable; CLR / n/a | `14` | Horizontal model coordinate/value in its chart, workflow or report coordinate system. |
| [`Y`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L84) | `double` | writable; CLR / n/a | `14` | Vertical model coordinate/value in its chart, workflow or report coordinate system. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportElement.ReportElement()`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L68) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.QRCodeErrorCorrection`](TheOne.Avalonia.Controls.QRCodeErrorCorrection.md)
- [`TheOne.Avalonia.Reporting.ReportElementKind`](TheOne.Avalonia.Reporting.ReportElementKind.md)
- [`TheOne.Avalonia.Reporting.ReportImageFit`](TheOne.Avalonia.Reporting.ReportImageFit.md)
- [`TheOne.Avalonia.Reporting.ReportTextAlignment`](TheOne.Avalonia.Reporting.ReportTextAlignment.md)
- [`TheOne.Avalonia.Reporting.ReportTextListStyle`](TheOne.Avalonia.Reporting.ReportTextListStyle.md)
