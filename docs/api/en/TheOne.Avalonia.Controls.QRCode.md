# QRCode

[English](../en/TheOne.Avalonia.Controls.QRCode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.QRCode.md) · [API index](README.md)

`TheOne.Avalonia.Controls.QRCode` · Class · `Avalonia.Controls.Control`

A vector QR code with a square symbol and a protected quiet zone.

[Source / 声明](../../../TheOne.Avalonia/Controls/QRCode.cs#L11)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/QRCode.cs#L42) | `Avalonia.Media.IBrush?` | writable; OneWay | `Brushes.White` | white ; Light-module and quiet-zone brush; default colors stay black on white in Light and Dark themes. |
| [`ErrorCorrection`](../../../TheOne.Avalonia/Controls/QRCode.cs#L36) | `TheOne.Avalonia.Controls.QRCodeErrorCorrection` | writable; OneWay | `QRCodeErrorCorrection.Medium` | `Medium` ; `QRCodeErrorCorrection.Low`, `Medium`, `Quartile` or `High`. Uses the requested level without automatic boosting; higher correction reduces capacity. Validation: `value => Enum.IsDefined(value)` Values: `Low`, `Medium`, `Quartile`, `High` |
| [`Foreground`](../../../TheOne.Avalonia/Controls/QRCode.cs#L40) | `Avalonia.Media.IBrush?` | writable; OneWay | `Brushes.Black` | black ; Dark-module brush; keep strong contrast when overriding. |
| [`QuietZone`](../../../TheOne.Avalonia/Controls/QRCode.cs#L38) | `int` | writable; OneWay | `4` | `4` ; Clear border on every side, in modules; accepts 4–32. Validation: `value => value is >= 4 and <= 32` |
| [`Text`](../../../TheOne.Avalonia/Controls/QRCode.cs#L34) | `string?` | writable; OneWay | `""` | `""` ; Exact payload. Null/empty, invalid Unicode or over-capacity content clears the old symbol and sets `ValidationError`. Whitespace is preserved. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/QRCode.cs#L44) | `string?` | read-only; OneWay | `computed / state` | empty-content error ; Read-only; null for a valid current symbol. Invalid states never retain the previous code. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`override void QRCode.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/QRCode.cs#L71) | Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly. |
| [`QRCode.QRCode()`](../../../TheOne.Avalonia/Controls/QRCode.cs#L11) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string?> QRCode.TextProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<QRCodeErrorCorrection> QRCode.ErrorCorrectionProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<int> QRCode.QuietZoneProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> QRCode.ForegroundProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> QRCode.BackgroundProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<QRCode, string?> QRCode.ValidationErrorProperty`](../../../TheOne.Avalonia/Controls/QRCode.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.QRCodeErrorCorrection`](TheOne.Avalonia.Controls.QRCodeErrorCorrection.md)
