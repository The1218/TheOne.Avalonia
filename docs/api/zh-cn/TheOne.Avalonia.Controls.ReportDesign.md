# ReportDesign

[English](../en/TheOne.Avalonia.Controls.ReportDesign.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ReportDesign.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ReportDesign` · Class · `Avalonia.Controls.UserControl`

A single-table report workbench with typed field binding and paginated A4 preview.

[Source / 声明](../../../TheOne.Avalonia/Controls/ReportDesign.Commands.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ConditionIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L21) | `int` | 可写; OneWay | `computed / state` | 条件格式编辑器的当前选项索引；与选中列模型同步。 |
| [`CurrentPageNumber`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L39) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`DataSource`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L23) | `TheOne.Avalonia.Reporting.ReportDataSource?` | 可写; OneWay | `null` | 显式数据来源；由宿主提供符合目标模型契约的数据，不能用显示模板代替数据适配。 |
| [`Document`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L22) | `TheOne.Avalonia.Reporting.ReportDocument?` | 可写; OneWay | `null` | 当前业务文档模型；替换时重接订阅、选择和历史的具体行为见编辑器文档。 |
| [`FontFamilyName`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L30) | `string` | 可写; OneWay | `computed / state` | 报表文本字体族名称；使用目标平台可用字体并检查回退。 |
| [`FontSizeValue`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L25) | `double` | 可写; OneWay | `computed / state` | 报表属性编辑器当前字号值；与选中元素／文档同步。 |
| [`FormatIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L31) | `int` | 可写; OneWay | `computed / state` | 报表数值格式编辑器的当前选项索引。 |
| [`ImageFitIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L39) | `int` | 可写; OneWay | `computed / state` | 报表图片适应模式编辑器的当前选项索引。 |
| [`IsPreview`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L38) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Layout`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L36) | `TheOne.Avalonia.Reporting.ReportLayout?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SaveCommand`](../../../TheOne.Avalonia/Controls/ReportDesign.Commands.cs#L26) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | Consumer save command. Its parameter is the current ReportDocument; call document.Save() to obtain JSON. |
| [`SelectedColumn`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L24) | `TheOne.Avalonia.Reporting.ReportColumn?` | 可写; TwoWay | `null` | 当前选中的报表列模型；空值表示未选择。 |
| [`SelectedElement`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L19) | `TheOne.Avalonia.Reporting.ReportElement?` | 可写; TwoWay | `null` | 当前选中的报表元素模型；空值表示未选择。 |
| [`SelectedField`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L26) | `TheOne.Avalonia.Reporting.ReportFieldInfo?` | 可写; OneWay | `computed / state` | 当前选中的可用报表字段；用于插入或编辑绑定。 |
| [`SourceFields`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L25) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportFieldInfo>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SumIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L40) | `int` | 可写; OneWay | `computed / state` | 报表汇总操作编辑器的当前选项索引。 |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L37) | `string?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Zoom`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L40) | `double` | 可写; CLR / n/a | `accessor-defined / state` | 视图缩放倍率，1 表示 100%；只缩放视图，不修改报表毫米尺寸或流程坐标。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void ReportDesign.MarkSaved()`](../../../TheOne.Avalonia/Controls/ReportDesign.Commands.cs#L28) | Call after the consumer's server save succeeds; executing SaveCommand alone does not imply success. |
| [`Task ReportDesign.DropDataNodeAsync(ReportDataNode node, double x, double y)`](../../../TheOne.Avalonia/Controls/ReportDesign.Data.cs#L100) | Drops an actual JSON node or typed source field at a paper position in millimetres. |
| [`ReportElement ReportDesign.AddElement(ReportElementKind kind, double x, double y)`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L54) | Adds an unbound component at a page position in millimetres. A report supports one repeating table. |
| [`void ReportDesign.DropField(string fieldKey, double x, double y)`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L115) | Binds a dropped field to the hit table detail cell or text element; empty paper creates a bound text element. |
| [`Task ReportDesign.FetchApiDataAsync(string url, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L136) | GET a JSON endpoint and expose its actual object, scalar and collection nodes for binding. |
| [`Task ReportDesign.FetchApiDataAsync(ReportApiRequest request, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L139) | Fetches a configured API request and imports its response tree without changing existing bindings. |
| [`void ReportDesign.SetImage(ReportElement element, ReadOnlySpan<byte> bytes, string name = "图片")`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L49) | Assigns a validated, embedded raster image as one undoable edit. |
| [`string ReportDesign.SaveTemplate()`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L61) | Serializes only the report definition, bindings and embedded images. |
| [`void ReportDesign.LoadTemplate(string json)`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L63) | Validates a template, replaces the document and automatically restores its optional API data. |
| [`Task ReportDesign.ExportPdfAsync(Stream destination, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L72) | Exports every preview page as a vector PDF. Must be called on the UI thread. |
| [`Task ReportDesign.LoadTemplateAsync(string json, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportDesign.Loading.cs#L103) | Loads a saved definition and awaits its optional API data and bound images. |
| [`void ReportDesign.RefreshReport()`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L184) | Re-evaluates registered getters and repaginates. Source row membership is a creation-time snapshot. |
| [`void ReportDesign.ShowPreview()`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L185) | 将报表设计器切换到预览模式，保留同一文档。 |
| [`void ReportDesign.ShowDesign()`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L186) | 将报表设计器切换回设计模式。 |
| [`void ReportDesign.GoToPage(int pageNumber)`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L187) | 切换到指定页码；分页器与报表的页码／越界处理分别见其契约。 |
| [`void ReportDesign.BindField(string fieldKey)`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L192) | 将选中报表元素／列绑定到显式注册的数据字段。 |
| [`ReportDesign.ReportDesign()`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L54) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<ICommand?> ReportDesign.SaveCommandProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Commands.cs#L25) | Consumer save command. Its parameter is the current ReportDocument; call document.Save() to obtain JSON. |
| [`static readonly StyledProperty<ReportElement?> ReportDesign.SelectedElementProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ReportDesign, int> ReportDesign.ImageFitIndexProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ReportDesign, int> ReportDesign.SumIndexProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ReportDesign, int> ReportDesign.ConditionIndexProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ReportDesign, string> ReportDesign.FontFamilyNameProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ReportDesign, double> ReportDesign.FontSizeValueProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ReportDocument?> ReportDesign.DocumentProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ReportDataSource?> ReportDesign.DataSourceProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ReportColumn?> ReportDesign.SelectedColumnProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ReportDesign, IReadOnlyList<ReportFieldInfo>> ReportDesign.SourceFieldsProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ReportDesign, ReportFieldInfo?> ReportDesign.SelectedFieldProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ReportDesign, int> ReportDesign.FormatIndexProperty`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportApiRequest`](TheOne.Avalonia.Reporting.ReportApiRequest.md)
- [`TheOne.Avalonia.Reporting.ReportColumn`](TheOne.Avalonia.Reporting.ReportColumn.md)
- [`TheOne.Avalonia.Reporting.ReportDataNode`](TheOne.Avalonia.Reporting.ReportDataNode.md)
- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
- [`TheOne.Avalonia.Reporting.ReportElement`](TheOne.Avalonia.Reporting.ReportElement.md)
- [`TheOne.Avalonia.Reporting.ReportElementKind`](TheOne.Avalonia.Reporting.ReportElementKind.md)
- [`TheOne.Avalonia.Reporting.ReportFieldInfo`](TheOne.Avalonia.Reporting.ReportFieldInfo.md)
- [`TheOne.Avalonia.Reporting.ReportLayout`](TheOne.Avalonia.Reporting.ReportLayout.md)
