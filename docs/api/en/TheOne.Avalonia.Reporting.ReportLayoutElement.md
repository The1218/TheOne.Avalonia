# ReportLayoutElement

[English](../en/TheOne.Avalonia.Reporting.ReportLayoutElement.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportLayoutElement.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportLayoutElement` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Bold`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `bool` | writable; CLR / init-only | `required constructor argument` | Renders text bold/semibold while wrapping/layout still use actual font metrics. |
| [`Fill`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `string` | writable; CLR / init-only | `required constructor argument` | Report element fill-color string; Transparent is transparent and values must be valid Avalonia colors. |
| [`FontSize`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `double` | writable; CLR / init-only | `required constructor argument` | Report font size in DIPs, with document/element range validation performed by the paginator. |
| [`Foreground`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `string` | writable; CLR / init-only | `required constructor argument` | Foreground brush; retain text, status and disabled contrast when overriding. |
| [`Height`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `double` | writable; CLR / init-only | `required constructor argument` | Height configuration; preserve content/text/data contracts; fixed values can override theme density. |
| [`Kind`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `TheOne.Avalonia.Reporting.ReportElementKind` | writable; CLR / init-only | `required constructor argument` | Kind enum for the owning field/property/element; see its concrete enum for choices. Values: `Text`, `Table`, `Line`, `Rectangle`, `PageNumber`, `Image`, `CheckBox`, `Barcode` |
| [`Name`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `string` | writable; CLR / init-only | `required constructor argument` | Name used for presentation/identity; keep it consistent with templates and keys. |
| [`ShowBorder`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `bool` | writable; CLR / init-only | `required constructor argument` | Shows a report element border without changing its geometry. |
| [`Text`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `string` | writable; CLR / init-only | `required constructor argument` | Input/display text; notifications and validation follow the model/native editor contract. |
| [`Width`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `double` | writable; CLR / init-only | `required constructor argument` | Width configuration; units follow the owning model, without changing data-coordinate meaning. |
| [`X`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `double` | writable; CLR / init-only | `required constructor argument` | Horizontal model coordinate/value in its chart, workflow or report coordinate system. |
| [`Y`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `double` | writable; CLR / init-only | `required constructor argument` | Vertical model coordinate/value in its chart, workflow or report coordinate system. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportLayoutElement.ReportLayoutElement(string Name, ReportElementKind Kind, double X, double Y, double Width, double Height, string Text, double FontSize, bool Bold, string Foreground, string Fill, bool ShowBorder)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportElementKind`](TheOne.Avalonia.Reporting.ReportElementKind.md)
