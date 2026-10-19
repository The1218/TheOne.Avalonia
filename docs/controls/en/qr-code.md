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
