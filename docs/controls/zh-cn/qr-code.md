# QRCode 二维码

[English](../en/qr-code.md) · [简体中文](../zh-cn/qr-code.md) · [控件索引](README.md)

`TheOne.Avalonia.Controls.QRCode` 直接使用矢量几何绘制 QR Code Model 2，支持静态文本和编译绑定，包括中文、Emoji 和链接。内容按带 ECI 标记的 UTF-8 编码，原样保留。编码使用 MIT 许可的 [Net.Codecrete.QrCodeGenerator](https://github.com/manuelbl/QrCodeGenerator)，不依赖系统位图组件。

```xml
<!-- xmlns:one="using:TheOne.Avalonia.Controls"；所属视图声明 ViewModel 的 x:DataType -->
<one:QRCode Text="{CompiledBinding QrText}"
            ErrorCorrection="Medium" QuietZone="4" Width="192" Height="192" />
```

| 属性 | 默认值 | 说明 |
| --- | --- | --- |
| `Text` | `""` | 原样编码。空值、无效 Unicode 或超限时清除旧二维码并设置 `ValidationError`；空格会保留。 |
| `ErrorCorrection` | `Medium` | `QRCodeErrorCorrection.Low`、`Medium`、`Quartile`、`High`，使用指定等级，不自动提升；更高纠错等级减少可编码容量。 |
| `QuietZone` | `4` | 四边静区宽度，单位为模块，范围 4–32。 |
| `Foreground` | 黑色 | 深色模块画刷，自定义时应保留足够扫描对比度。 |
| `Background` | 白色 | 浅色模块及静区画刷，默认在 Light / Dark 下都保持黑白，不随主题反色。 |
| `ValidationError` | 空内容错误 | 只读；有效时为 null，非法状态不会继续显示旧二维码。 |

没有专属 Classes 或必须调用的业务 API。期望尺寸为 160 × 160 DIP，可通过 `Width` / `Height` 指定显示或打印尺寸。非正方形容器内居中绘制正方形，完整保留静区；包含静区在内，每个模块至少需要 1 DIP。这只是绘制下限，实体打印仍需结合分辨率、扫描距离和设备验证。

```xml
<StackPanel>
  <one:QRCode x:Name="Qr" Text="https://example.com/report"
              ErrorCorrection="High" QuietZone="6" Width="200" Height="200" />
  <TextBlock Text="{CompiledBinding #Qr.ValidationError}" TextWrapping="Wrap" />
</StackPanel>
```

编码前最多允许 7089 个 UTF-8 字节，以限制处理量；实际容量还受模式和纠错等级影响，可能更低。超限报错，不截断、不拆分。仅内容或纠错等级变化会重新编码，尺寸、画刷和静区变化复用几何缓存；不建立全局内容缓存。

PreviewHost 独立的 **QRCode** 页可修改文本、纠错等级和静区，显示校验结果，并通过 `Description` 展示简短代码。报表元素、字段绑定、撤销和 PDF 用法见 [ReportDesign](report-design.md#二维码组件)。
