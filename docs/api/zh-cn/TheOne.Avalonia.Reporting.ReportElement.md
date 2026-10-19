# ReportElement

[English](../en/TheOne.Avalonia.Reporting.ReportElement.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportElement.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportElement` · Class · `TheOne.Avalonia.Reporting.ReportObservable`

[继承的额外成员: ReportObservable](TheOne.Avalonia.Reporting.ReportObservable.md)

A freely positioned report element. Geometry is measured in millimetres on the page.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L68)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`BarcodeShowText`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L120) | `bool` | 可写; CLR / n/a | `true` | 条码下方是否显示原始文本；不改变编码内容。 |
| [`Bold`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L144) | `bool` | 可写; CLR / n/a | `false` | 文本使用粗体／半粗体绘制；换行与布局仍按实际字体度量。 |
| [`FieldKey`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L108) | `string` | 可写; CLR / n/a | `""` | 显式注册的数据字段键；不是反射属性路径，必须与数据源字段一致。 |
| [`Fill`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L220) | `string` | 可写; CLR / n/a | `"Transparent"` | 报表元素填充颜色字符串；Transparent 表示透明，须为有效 Avalonia 颜色。 |
| [`FontFamily`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L150) | `string` | 可写; CLR / n/a | `"Microsoft YaHei"` | 文本绘制使用的字体族；字体回退和实际度量由 Avalonia 文本系统处理。 |
| [`FontSize`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L138) | `double` | 可写; CLR / n/a | `12` | 报表文本字号，单位 DIP；文档和单元素范围校验由分页器处理。 |
| [`Foreground`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L214) | `string` | 可写; CLR / n/a | `"#212222"` | 前景画刷；自定义时保持正文、状态与禁用对比度。 |
| [`Height`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L96) | `double` | 可写; CLR / n/a | `12` | 高度配置；保留内容、字号及数据坐标契约，固定值可能覆盖主题密度。 |
| [`ImageData`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L181) | `string` | 可写; CLR / n/a | `""` | Embedded PNG/JPEG/WebP image bytes, encoded as base64. No external file dependency. |
| [`ImageFit`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L208) | `TheOne.Avalonia.Reporting.ReportImageFit` | 可写; CLR / n/a | `0 / default` | 图片适应方式枚举：Contain 保持完整、Cover 裁切覆盖、Fill 拉伸。 可选值：`Contain`, `Cover`, `Fill` |
| [`ImageName`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L187) | `string` | 可写; CLR / n/a | `""` | 嵌入图片的显示名称；图片数据仍由 ImageData 保存。 |
| [`ImageUrl`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L193) | `string` | 可写; CLR / n/a | `""` | 远程图片地址；加载结果和原始配置分离，按报表图片加载约定处理。 |
| [`IsChecked`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L114) | `bool` | 可写; CLR / n/a | `false` | 报表复选框元素的勾选状态。 |
| [`Italic`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L156) | `bool` | 可写; CLR / n/a | `false` | 文本斜体显示；目标字体不支持时由字体系统处理回退。 |
| [`Kind`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L76) | `TheOne.Avalonia.Reporting.ReportElementKind` | 可写; CLR / init-only | `0 / default` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Text`, `Table`, `Line`, `Rectangle`, `PageNumber`, `Image`, `CheckBox`, `Barcode`, `QRCode` |
| [`ListStyle`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L174) | `TheOne.Avalonia.Reporting.ReportTextListStyle` | 可写; CLR / n/a | `0 / default` | 报表文本行的列表前缀：无、圆点或复选框样式。 可选值：`None`, `Bullet`, `Check` |
| [`Name`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L70) | `string` | 可写; CLR / n/a | `"元素"` | 名称字段，用于业务展示或标识；与显示模板、键值字段保持一致。 |
| [`QRCodeErrorCorrection`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L126) | `TheOne.Avalonia.Controls.QRCodeErrorCorrection` | 可写; CLR / n/a | `QRCodeErrorCorrection.Medium` | 二维码元素的纠错等级，默认 Medium；随模板、撤销和重做保存。 可选值：`Low`, `Medium`, `Quartile`, `High` |
| [`QRCodeQuietZone`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L132) | `int` | 可写; CLR / n/a | `4` | 二维码元素四周的静区模块数，范围 4–32，默认 4；设计、预览和 PDF 共用。 |
| [`ShowBorder`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L226) | `bool` | 可写; CLR / n/a | `false` | 显示报表元素边缘线；不改变元素几何尺寸。 |
| [`TableHeaderHeightMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L202) | `double` | 可写; CLR / n/a | `11` | TableHeaderHeightMm：纸张／元素几何尺寸，单位毫米；可空边距回退到 MarginMm。分页时转换为 DIP，不预先乘缩放倍率。 |
| [`Text`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L102) | `string` | 可写; CLR / n/a | `"文本"` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |
| [`TextAlignment`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L168) | `TheOne.Avalonia.Reporting.ReportTextAlignment` | 可写; CLR / n/a | `0 / default` | 报表文本对齐枚举，包括左右、居中和两端对齐。 可选值：`Left`, `Center`, `Right`, `Justify` |
| [`Underline`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L162) | `bool` | 可写; CLR / n/a | `false` | 在报表文本上绘制下划线，不切换输入控件的 underlined 类。 |
| [`Width`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L90) | `double` | 可写; CLR / n/a | `60` | 宽度配置；物理单位或布局单位遵循所属模型说明，不能据此改变数据坐标含义。 |
| [`X`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L78) | `double` | 可写; CLR / n/a | `14` | 模型横向坐标／数值；按所属图表、流程或报表坐标系统解释。 |
| [`Y`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L84) | `double` | 可写; CLR / n/a | `14` | 模型纵向坐标／数值；按所属图表、流程或报表坐标系统解释。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportElement.ReportElement()`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L68) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.QRCodeErrorCorrection`](TheOne.Avalonia.Controls.QRCodeErrorCorrection.md)
- [`TheOne.Avalonia.Reporting.ReportElementKind`](TheOne.Avalonia.Reporting.ReportElementKind.md)
- [`TheOne.Avalonia.Reporting.ReportImageFit`](TheOne.Avalonia.Reporting.ReportImageFit.md)
- [`TheOne.Avalonia.Reporting.ReportTextAlignment`](TheOne.Avalonia.Reporting.ReportTextAlignment.md)
- [`TheOne.Avalonia.Reporting.ReportTextListStyle`](TheOne.Avalonia.Reporting.ReportTextListStyle.md)
