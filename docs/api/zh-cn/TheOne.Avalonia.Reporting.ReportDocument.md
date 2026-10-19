# ReportDocument

[English](../en/TheOne.Avalonia.Reporting.ReportDocument.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportDocument.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportDocument` · Class · `TheOne.Avalonia.Reporting.ReportObservable`

[继承的额外成员: ReportObservable](TheOne.Avalonia.Reporting.ReportObservable.md)

Mutable single-table report definition. Geometry is in millimetres; text and row sizes are in DIPs.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L234)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`BottomMargin`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L366) | `double` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Columns`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L383) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Reporting.ReportColumn>` | 只读; CLR / n/a | `[]` | 报表列定义或分页列快照集合；前者可编辑，快照用于绘制，元素类型见签名。 |
| [`DataBodyPointer`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L357) | `string` | 可写; CLR / n/a | `""` | HTTP JSON 响应中记录数组的 JSON Pointer；空字符串表示响应根。 |
| [`DataSourceAuthentication`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L338) | `TheOne.Avalonia.Reporting.ReportApiAuthentication?` | 可写; CLR / n/a | `null` | 数据源请求使用的登录配置；不在文档内保存运行期 token。 |
| [`DataSourceMethod`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L320) | `string` | 可写; CLR / n/a | `"GET"` | 报表数据源的 HTTP 方法，默认 GET。 |
| [`DataSourceName`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L344) | `string` | 可写; CLR / n/a | `"接口数据"` | 设计器左侧数据目录的来源名称，默认“接口数据”；随报表模板保存，不改变接口地址或字段键。 |
| [`DataSourceParameterMode`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L332) | `TheOne.Avalonia.Reporting.ReportApiParameterMode` | 可写; CLR / n/a | `0 / default` | 报表数据源参数的编码方式枚举。 可选值：`Json`, `Form` |
| [`DataSourceParameters`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L326) | `string` | 可写; CLR / n/a | `""` | 报表数据源的参数配置文本，与编码方式共同使用。 |
| [`DataSourceUrl`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L314) | `string` | 可写; CLR / n/a | `""` | 报表远程 JSON 数据源地址；替换与刷新按报表加载契约处理。 |
| [`Description`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L248) | `string` | 可写; CLR / n/a | `""` | 描述文本；由宿主提供业务说明，未提供时沿用初值。 |
| [`Elements`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L386) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Reporting.ReportElement>` | 只读; CLR / n/a | `[]` | 报表自由元素定义或分页元素快照；源模型使用毫米，布局快照使用 DIP。 |
| [`FontSize`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L370) | `double` | 可写; CLR / n/a | `12` | 报表文本字号，单位 DIP；文档和单元素范围校验由分页器处理。 |
| [`Footer`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L254) | `string` | 可写; CLR / n/a | `""` | 底部区域内容；与主体和标题分别定制。 |
| [`FooterHeightMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L308) | `double` | 可写; CLR / n/a | `18` | FooterHeightMm：纸张／元素几何尺寸，单位毫米；可空边距回退到 MarginMm。分页时转换为 DIP，不预先乘缩放倍率。 |
| [`FooterTop`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L368) | `double` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HeaderBottom`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L367) | `double` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HeaderHeightMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L302) | `double` | 可写; CLR / n/a | `32` | HeaderHeightMm：纸张／元素几何尺寸，单位毫米；可空边距回退到 MarginMm。分页时转换为 DIP，不预先乘缩放倍率。 |
| [`LeftMargin`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L363) | `double` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`MarginBottomMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L296) | `double?` | 可写; CLR / n/a | `null` | MarginBottomMm：纸张／元素几何尺寸，单位毫米；可空边距回退到 MarginMm。分页时转换为 DIP，不预先乘缩放倍率。 |
| [`MarginLeftMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L278) | `double?` | 可写; CLR / n/a | `null` | MarginLeftMm：纸张／元素几何尺寸，单位毫米；可空边距回退到 MarginMm。分页时转换为 DIP，不预先乘缩放倍率。 |
| [`MarginMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L272) | `double` | 可写; CLR / n/a | `14` | MarginMm：纸张／元素几何尺寸，单位毫米；可空边距回退到 MarginMm。分页时转换为 DIP，不预先乘缩放倍率。 |
| [`MarginRightMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L290) | `double?` | 可写; CLR / n/a | `null` | MarginRightMm：纸张／元素几何尺寸，单位毫米；可空边距回退到 MarginMm。分页时转换为 DIP，不预先乘缩放倍率。 |
| [`MarginTopMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L284) | `double?` | 可写; CLR / n/a | `null` | MarginTopMm：纸张／元素几何尺寸，单位毫米；可空边距回退到 MarginMm。分页时转换为 DIP，不预先乘缩放倍率。 |
| [`MinimumRowHeight`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L376) | `double` | 可写; CLR / n/a | `30` | 报表明细最小行高，单位 DIP；换行内容可使实际行高增加。 |
| [`PageHeightMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L266) | `double` | 可写; CLR / n/a | `297` | PageHeightMm：纸张／元素几何尺寸，单位毫米；可空边距回退到 MarginMm。分页时转换为 DIP，不预先乘缩放倍率。 |
| [`PageWidthMm`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L260) | `double` | 可写; CLR / n/a | `210` | PageWidthMm：纸张／元素几何尺寸，单位毫米；可空边距回退到 MarginMm。分页时转换为 DIP，不预先乘缩放倍率。 |
| [`RightMargin`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L365) | `double` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SelectedDataPointers`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L351) | `string[]?` | 可写; CLR / n/a | `null` | Designer item selection, independent of the data used by existing report bindings. |
| [`Subtitle`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L242) | `string` | 可写; CLR / n/a | `"DETAIL REPORT"` | 报表副标题文本，与主标题独立显示。 |
| [`Title`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L236) | `string` | 可写; CLR / n/a | `"新建报表"` | 显示标题；不等于业务主键或本地化资源键。 |
| [`TopMargin`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L364) | `double` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`string ReportDocument.Save()`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L389) | Returns a portable JSON definition including public API/image URLs, but no fetched records. |
| [`static ReportDocument ReportDocument.Load(string json)`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L394) | 从 JSON 载入类型化业务文档；版本、字段及结构校验见对应序列化契约。 |
| [`ReportDocument.ReportDocument()`](../../../TheOne.Avalonia/Reporting/ReportDocument.cs#L234) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportApiAuthentication`](TheOne.Avalonia.Reporting.ReportApiAuthentication.md)
- [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](TheOne.Avalonia.Reporting.ReportApiParameterMode.md)
- [`TheOne.Avalonia.Reporting.ReportColumn`](TheOne.Avalonia.Reporting.ReportColumn.md)
- [`TheOne.Avalonia.Reporting.ReportElement`](TheOne.Avalonia.Reporting.ReportElement.md)
