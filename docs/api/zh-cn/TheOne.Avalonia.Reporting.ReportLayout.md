# ReportLayout

[English](../en/TheOne.Avalonia.Reporting.ReportLayout.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportLayout.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportLayout` · Class · `object`

Immutable text and geometry snapshot shared by preview and the designer's drawing surface.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L16)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Columns`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L20) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportLayoutColumn>` | 可写; CLR / init-only | `required constructor argument` | 报表列定义或分页列快照集合；前者可编辑，快照用于绘制，元素类型见签名。 |
| [`ContentWidth`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L23) | `double` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Description`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L17) | `string` | 可写; CLR / init-only | `required constructor argument` | 描述文本；由宿主提供业务说明，未提供时沿用初值。 |
| [`DetailBottom`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L19) | `double` | 可写; CLR / init-only | `required constructor argument` | 报表布局中明细区的底部坐标，单位 DIP；供分页与绘制共享。 |
| [`Elements`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L26) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportLayoutElement>` | 可写; CLR / init-only | `[]` | 报表自由元素定义或分页元素快照；源模型使用毫米，布局快照使用 DIP。 |
| [`FontSize`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L18) | `double` | 可写; CLR / init-only | `required constructor argument` | 报表文本字号，单位 DIP；文档和单元素范围校验由分页器处理。 |
| [`Footer`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L17) | `string` | 可写; CLR / init-only | `required constructor argument` | 底部区域内容；与主体和标题分别定制。 |
| [`HeaderHeight`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L19) | `double` | 可写; CLR / init-only | `required constructor argument` | 标题区高度；图表使用 DIP，报表布局使用其布局坐标，勿与毫米源模型混用。 |
| [`Margin`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L18) | `double` | 可写; CLR / init-only | `required constructor argument` | 分页布局的默认边距，单位 DIP；与源模型的 MarginMm 区分。 |
| [`PageHeight`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L18) | `double` | 可写; CLR / init-only | `required constructor argument` | 分页后纸张高度，单位 DIP，已经由毫米转换。 |
| [`PageWidth`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L18) | `double` | 可写; CLR / init-only | `required constructor argument` | 分页后纸张宽度，单位 DIP，已经由毫米转换。 |
| [`Pages`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L21) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportPage>` | 可写; CLR / init-only | `required constructor argument` | 分页器生成的页面快照集合；预览和打印共用该布局。 |
| [`Subtitle`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L17) | `string` | 可写; CLR / init-only | `required constructor argument` | 报表副标题文本，与主标题独立显示。 |
| [`Summary`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L20) | `System.Collections.Generic.IReadOnlyList<string>` | 可写; CLR / init-only | `required constructor argument` | 按列对齐的汇总展示文本集合；由分页器从数据源计算。 |
| [`SummaryHeight`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L19) | `double` | 可写; CLR / init-only | `required constructor argument` | 分页汇总区域高度，单位 DIP。 |
| [`TableTop`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L19) | `double` | 可写; CLR / init-only | `required constructor argument` | 分页表格区域顶部坐标，单位 DIP。 |
| [`TableWidth`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L25) | `double` | 可写; CLR / init-only | `0 / default` | 分页表格的实际宽度，单位 DIP。 |
| [`TableX`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L24) | `double` | 可写; CLR / init-only | `Margin` | 分页表格的横向起始坐标，单位 DIP。 |
| [`Title`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L17) | `string` | 可写; CLR / init-only | `required constructor argument` | 显示标题；不等于业务主键或本地化资源键。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportLayout.ReportLayout(string Title, string Subtitle, string Description, string Footer, double PageWidth, double PageHeight, double Margin, double FontSize, double TableTop, double HeaderHeight, double DetailBottom, double SummaryHeight, IReadOnlyList<ReportLayoutColumn> Columns, IReadOnlyList<string> Summary, IReadOnlyList<ReportPage> Pages)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L16) | Immutable text and geometry snapshot shared by preview and the designer's drawing surface. |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportLayoutColumn`](TheOne.Avalonia.Reporting.ReportLayoutColumn.md)
- [`TheOne.Avalonia.Reporting.ReportLayoutElement`](TheOne.Avalonia.Reporting.ReportLayoutElement.md)
- [`TheOne.Avalonia.Reporting.ReportPage`](TheOne.Avalonia.Reporting.ReportPage.md)
