# QRCode

[English](../en/qr-code.md) · [简体中文](../zh-cn/qr-code.md) · [Controls](README.md)

`TheOne.Avalonia.Controls.QRCode` draws QR Code Model 2 directly as vector geometry. Static text and compiled bindings support Chinese, emoji and URLs. Payloads use UTF-8 with ECI and are preserved exactly. Encoding uses the MIT-licensed [Net.Codecrete.QrCodeGenerator](https://github.com/manuelbl/QrCodeGenerator), without a platform bitmap dependency.

```xml
<!-- xmlns:one="using:TheOne.Avalonia.Controls"; the containing view declares its ViewModel x:DataType -->
<one:QRCode Text="{CompiledBinding QrText}"
            ErrorCorrection="Medium" QuietZone="4" Width="192" Height="192" />
```

| Property | Default | Behavior |
| --- | --- | --- |
| `Text` | `""` | Exact payload. Null/empty, invalid Unicode or over-capacity content clears the old symbol and sets `ValidationError`. Whitespace is preserved. |
| `ErrorCorrection` | `Medium` | `QRCodeErrorCorrection.Low`, `Medium`, `Quartile` or `High`. Uses the requested level without automatic boosting; higher correction reduces capacity. |
| `QuietZone` | `4` | Clear border on every side, in modules; accepts 4–32. |
| `Foreground` | black | Dark-module brush; keep strong contrast when overriding. |
| `Background` | white | Light-module and quiet-zone brush; default colors stay black on white in Light and Dark themes. |
| `ValidationError` | empty-content error | Read-only; null for a valid current symbol. Invalid states never retain the previous code. |

No additional Classes or imperative business APIs are required. Desired size is 160 × 160 DIP; set `Width` / `Height` for the intended display or print size. Non-square bounds center a square symbol with its full quiet zone. Every module, including the quiet zone, needs at least 1 DIP. This is a rendering minimum; verify physical output with the intended printer, scanner and viewing distance.

```xml
<StackPanel>
  <one:QRCode x:Name="Qr" Text="https://example.com/report"
              ErrorCorrection="High" QuietZone="6" Width="200" Height="200" />
  <TextBlock Text="{CompiledBinding #Qr.ValidationError}" TextWrapping="Wrap" />
</StackPanel>
```

Input is limited to 7089 UTF-8 bytes before encoding to bound work. Actual capacity depends on mode and correction level and can be lower. Content is never truncated or split. Payload/correction changes regenerate the symbol; resizing, brushes and quiet-zone changes reuse its cached geometry. No global payload cache is kept.

PreviewHost's **QRCode** page provides editable payload, correction, quiet zone and error feedback, with short code examples in `Description`. See [ReportDesign](report-design.md#qr-code-components) for report elements, field binding, history and PDF.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### QRCode

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.QRCode.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/QRCode.cs#L42) | `Avalonia.Media.IBrush?` | writable; OneWay | `Brushes.White` | white ; Light-module and quiet-zone brush; default colors stay black on white in Light and Dark themes. |
| [`ErrorCorrection`](../../../TheOne.Avalonia/Controls/QRCode.cs#L36) | `TheOne.Avalonia.Controls.QRCodeErrorCorrection` | writable; OneWay | `QRCodeErrorCorrection.Medium` | `Medium` ; `QRCodeErrorCorrection.Low`, `Medium`, `Quartile` or `High`. Uses the requested level without automatic boosting; higher correction reduces capacity. Validation: `value => Enum.IsDefined(value)` Values: `Low`, `Medium`, `Quartile`, `High` |
| [`Foreground`](../../../TheOne.Avalonia/Controls/QRCode.cs#L40) | `Avalonia.Media.IBrush?` | writable; OneWay | `Brushes.Black` | black ; Dark-module brush; keep strong contrast when overriding. |
| [`QuietZone`](../../../TheOne.Avalonia/Controls/QRCode.cs#L38) | `int` | writable; OneWay | `4` | `4` ; Clear border on every side, in modules; accepts 4–32. Validation: `value => value is >= 4 and <= 32` |
| [`Text`](../../../TheOne.Avalonia/Controls/QRCode.cs#L34) | `string?` | writable; OneWay | `""` | `""` ; Exact payload. Null/empty, invalid Unicode or over-capacity content clears the old symbol and sets `ValidationError`. Whitespace is preserved. |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/QRCode.cs#L44) | `string?` | read-only; OneWay | `computed / state` | empty-content error ; Read-only; null for a valid current symbol. Invalid states never retain the previous code. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

No additional business methods; configure the control-specific properties above.

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.QRCodeErrorCorrection`](../../api/en/TheOne.Avalonia.Controls.QRCodeErrorCorrection.md) | 4 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
