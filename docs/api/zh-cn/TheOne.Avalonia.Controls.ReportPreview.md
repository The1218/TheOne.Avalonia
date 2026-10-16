# ReportPreview

[English](../en/TheOne.Avalonia.Controls.ReportPreview.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ReportPreview.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ReportPreview` · Class · `Avalonia.Controls.UserControl`

Standalone read-only report viewer. Parses document JSON and loads its optional API/images.

[Source / 声明](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L14)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CurrentPageNumber`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L29) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`DataSource`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L21) | `TheOne.Avalonia.Reporting.ReportDataSource?` | 可写; OneWay | `null` | 显式数据来源；由宿主提供符合目标模型契约的数据，不能用显示模板代替数据适配。 |
| [`Document`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L22) | `TheOne.Avalonia.Reporting.ReportDocument?` | 只读; OneWay | `computed / state` | 当前业务文档模型；替换时重接订阅、选择和历史的具体行为见编辑器文档。 |
| [`DocumentJson`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L20) | `string?` | 可写; OneWay | `null` | 报表文档 JSON；读取／替换失败和数据刷新行为按报表加载契约处理。 |
| [`Error`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L23) | `string?` | 只读; OneWay | `computed / state` | 操作失败的结构化／文本信息；与成功标志一起检查，不忽略失败。 |
| [`HttpClient`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L26) | `System.Net.Http.HttpClient?` | 可写; CLR / n/a | `null` | Optional consumer-owned client for authenticated API and image requests. Not serialized. |
| [`Layout`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L24) | `TheOne.Avalonia.Reporting.ReportLayout?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`PrintHandler`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L28) | `System.Func<string, TheOne.Avalonia.Reporting.ReportPrintOptions?, System.Threading.CancellationToken, System.Threading.Tasks.Task<TheOne.Avalonia.Reporting.ReportPrintResult>>?` | 可写; CLR / n/a | `null` | Optional host printer. The PDF exists until the returned task completes; KeepDocument controls its subsequent lifetime. |
| [`Zoom`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L30) | `double` | 可写; CLR / n/a | `.75` | 视图缩放倍率，1 表示 100%；只缩放视图，不修改报表毫米尺寸或流程坐标。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Task ReportPreview.LoadAsync(string json, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L103) | 异步加载报表 JSON 及相关数据／图片；遵守取消、失败和新旧加载隔离契约。 |
| [`void ReportPreview.GoToPage(int page)`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L137) | 切换到指定页码；分页器与报表的页码／越界处理分别见其契约。 |
| [`Task ReportPreview.ExportPdfAsync(Stream destination, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L149) | 将当前报表布局异步导出为 PDF；输出路径与取消参数见签名。 |
| [`Task<ReportPrintResult> ReportPreview.PrintAsync(ReportPrintOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L155) | 调用配置的打印处理器并等待结果；主库不替宿主选择打印依赖或静默提交策略。 |
| [`ReportPreview.ReportPreview()`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L44) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string?> ReportPreview.DocumentJsonProperty`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ReportDataSource?> ReportPreview.DataSourceProperty`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ReportPreview, ReportDocument?> ReportPreview.DocumentProperty`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ReportPreview, string?> ReportPreview.ErrorProperty`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
- [`TheOne.Avalonia.Reporting.ReportLayout`](TheOne.Avalonia.Reporting.ReportLayout.md)
- [`TheOne.Avalonia.Reporting.ReportPrintOptions`](TheOne.Avalonia.Reporting.ReportPrintOptions.md)
- [`TheOne.Avalonia.Reporting.ReportPrintResult`](TheOne.Avalonia.Reporting.ReportPrintResult.md)
