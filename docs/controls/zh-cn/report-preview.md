# ReportPreview 报表预览

[English](../en/report-preview.md) · [控件文档](README.md) · [ReportDesign 报表设计器](report-design.md)

`ReportPreview` 是独立只读控件，接收 `ReportDocument.Save()` 返回的 JSON，自行解析模板、获取可选接口数据／网络图片、分页并展示。内置翻页、缩放、刷新、PDF 导出和打印，不依赖设计器实例。使用前加载 `OneTheme` 并初始化 Avalonia，以测量文字和绘制。

```xml
<one:ReportPreview DocumentJson="{Binding ReportJson}"
                   DataSource="{Binding SourceOverride}" />
```

视图声明正确的 `x:DataType`；`one` 为 `using:TheOne.Avalonia.Controls`。

## 加载模板与数据

设置 `DocumentJson` 会异步加载；需要等待完成或传入取消令牌时，在 UI 线程调用 `LoadAsync(json, cancellationToken)`。`Document`、`Layout`、`Error` 分别提供已解析文档、分页快照及错误消息；`Document`、`Error` 为只读 Avalonia 属性。加载失败清空显示布局。`LoadAsync` 设置 `Error` 并向调用方抛出异常；属性驱动加载在界面显示错误。

```csharp
viewer.HttpClient = applicationHttpClient; // 需要时，在加载前设置
await viewer.LoadAsync(savedJson, cancellationToken);
viewer.GoToPage(2); // 从 1 开始，限制在有效范围
viewer.Zoom = 1;   // 范围 0.25–2
```

控件使用保存的 `DataSourceMethod`、`DataSourceParameters` 和 `DataSourceParameterMode` 请求 `DataSourceUrl`，旧模板默认 GET。GET 参数编码为查询字符串，其他方式发送 JSON 或 Form 请求体，用 `DataBodyPointer` 选择明细集合。保存的 `$root/company`、`$root/orders/0/logo` 等绝对键从完整响应恢复单值，与集合相对字段并存。显式 `DataSource` 优先于模板接口，可传入强类型或应用自行获取的数据，但需要提供模板所需字段键。图片从 `ImageUrl` 加载，或根据图片元素的 `FieldKey` 从源中取得当前 HTTP / HTTPS 地址；仍支持内嵌 `ImageData`。复选框和 Code 128B 条码使用与设计器相同的绑定和矢量绘制。无接口或数据源时可查看静态内容与绑定占位符。

设计器响应树的展示上限不会截断预览记录，已保存集合数据体中的全部行都会读取。`ReportResources.LoadDataAsync` 自动恢复模板根绑定键；自定义宿主可使用 `ReportJsonData.CreateSource(json, bodyPointer, rootFieldKeys)` 及 `ReportResources.LoadImagesAsync(document, client, cancellationToken, source)` 实现相同的按源加载图片流程。

配置 `DataSourceAuthentication` 后，预览先请求登录接口，提取 `TokenField`，将 `HeaderPrefix + token` 作为 `HeaderName` 请求头传给数据接口，再解析报表数据。登录设置包括 URL、方式、JSON／Form 入参及字段／请求头配置，详见 [ReportDesign](report-design.md)。每次重新加载都会重新登录；登录／字段解析失败或取消时不继续请求数据。实际 token 不保存，不修改共享客户端默认请求头，也不自动向图片请求传递权限头。显式 `DataSource` 会跳过登录与接口取数。

登录和数据接口响应分别上限 4 MiB；单张图片上限 8 MiB、1600 万像素，一份报表的网络图片合计上限 32 MiB、3200 万像素。预览整体加载 30 秒后取消（包含登录），默认 HTTP 客户端请求超时 20 秒。可在加载前设置 CLR 属性 `HttpClient`，提供鉴权或自定义传输。客户端由消费者持有，控件不会释放或序列化它。

更改 `DocumentJson`、`DataSource` 或点击刷新，会重新加载所请求的定义及所需资源；新加载取消旧加载并回到首页。`LoadAsync` 也会更新刷新时使用的定义。系统字体影响换行与字形回退，相关 API 在 UI 线程操作。

## PDF 与打印

```csharp
await using var pdf = File.Create("report.pdf");
await viewer.ExportPdfAsync(pdf, cancellationToken);
ReportPrintResult result = await viewer.PrintAsync(
    new ReportPrintOptions(PrinterName: null, Copies: 1), cancellationToken);
```

PDF 使用已加载的数据、图片及分页快照，保持物理纸张尺寸、矢量文字／图形和内嵌图片。`ExportPdfAsync`、`PrintAsync` 不再请求 HTTP；需要更新资源时刷新或重新加载。导出按钮使用系统保存对话框；打印先生成临时 PDF。未配置宿主 `PrintHandler` 时，仍交给 `TheOne.Avalonia.Reporting` 中的 `PrintTool`，直接提交到 CUPS 或调用 Windows PDF 打印处理程序。

`PrintHandler` 是每个预览实例可选的 CLR 回调，类型为 `Func<string, ReportPrintOptions?, CancellationToken, Task<ReportPrintResult>>`。PDF 路径在任务完成前保持有效；读取完毕后返回 `KeepDocument=false`，异步打印程序仍需读取时返回 true。异常和取消会恢复打印按钮并清理未保留的临时文件。

PreviewHost 引用 [TheOne.PrintingTools.Avalonia](https://github.com/The1218/TheOne.PrintingTools.Avalonia) `1.0.0-preview.3`，并在 ReportPreview 示例上配置该回调。点击打印先打开包提供的打印设置／预览，用户确认后才提交任务。打印包主题仅加载到打印窗口内，跟随当前明暗主题与三档密度。主控件库不依赖此打印包；适配实现见 [PreviewPrinting](../../../design/PreviewHost/Infrastructure/Printing/PreviewPrinting.cs)。

`PrintTool.PrintPdfAsync(path, options, cancellationToken)` 可提交已有 PDF；`CreateStartInfo(path, platform, options)` 只构造平台调用参数，不提交作业。

| 平台 | 调用方式与前提 |
| --- | --- |
| Windows | 已关联 PDF 应用的 `print` 动词，需要安装支持该动词的处理程序。只支持默认参数；指定打印机或多份会抛出 `NotSupportedException`，设置由处理程序管理。 |
| Linux | CUPS `lp`，需要可用命令、配置好的队列及打印服务权限。可指定打印机与 1–999 份，参数独立传递。 |
| macOS | 通过已配置 CUPS 调用 `/usr/bin/lp`，打印机与份数参数与 Linux 相同。 |

`ReportPrintResult.Message` 只表示提交或调用处理程序的结果，不表示纸张已实际打印。CUPS 失败抛出包含命令错误的异常。Windows 处理程序可能在调用返回后读取 PDF，因此 `KeepDocument` 为 true；`ReportPreview.PrintAsync` 保留文件，通过 `DocumentPath` 和消息返回路径。应用运行 24 小时后计时清理，后续初始化预览控件或打印时也会清理过期文件；应用退出不会立即删除保留文件。应用启动时还可调用 `PrintTool.CleanupTemporaryDocuments()` 清理库产生的过期打印文件。CUPS 临时文件在提交／失败后删除；直接调用 `PrintTool` 的消费者自行管理 PDF 生命周期。

命令构造检查不代表物理打印、Windows PDF 处理程序或 Linux 队列已实跑。消费者需在目标平台验证打印服务及设备；默认 PrintTool 路径没有打印设置对话框，PreviewHost 使用上述包提供的预览界面。

## 接入与限制

PreviewHost 的“扩展控件 → ReportPreview”以保存的 JSON 和显式 96 行示例源展示，不依赖外网，预览填满宿主内容区。

JSON 保存公共接口／图片 URL 和数据体路径，不包含数据记录、响应或 HTTP 客户端凭证。鉴权放在消费者客户端中，不放入保存的 URL；源生成 JSON 元数据支持 NativeAOT。格式、合计、条件和校验遵循 [ReportDesign 模型](report-design.md#预览与更新契约)。当前支持一个重复明细表格，不提供报表编辑或请求方法／请求体编辑。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节只列出本项目增加的 Classes、属性、扩展 API／事件及关联模型，不重复 Avalonia 原生继承 API。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用可见示例对照本项目附加样式，用 Description 组件说明专属属性、API 签名与模型字段；ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### ReportPreview

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.ReportPreview.md)

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

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |

`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出附加外观类的控件使用默认主题及其专属属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.ReportPreview`](../../api/zh-cn/TheOne.Avalonia.Controls.ReportPreview.md) | Class | 9 | 5 | 0 |
| [`TheOne.Avalonia.Reporting.PrintTool`](../../api/zh-cn/TheOne.Avalonia.Reporting.PrintTool.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Reporting.ReportApiAuthentication`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportApiAuthentication.md) | Class | 7 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportApiParameterMode.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportApiRequest`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportApiRequest.md) | Class | 5 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportColumn`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportColumn.md) | Class | 12 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportCondition`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportCondition.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDataNode`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportDataNode.md) | Class | 9 | 0 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDataNodeKind`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportDataNodeKind.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDataSource`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportDataSource.md) | Class | 3 | 2 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDocument`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportDocument.md) | Class | 29 | 3 | 0 |
| [`TheOne.Avalonia.Reporting.ReportElement`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportElement.md) | Class | 25 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportElementKind`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportElementKind.md) | Enum | 8 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportField<T>`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportField_T_.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportFieldInfo`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportFieldInfo.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportImageFit`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportImageFit.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportJsonBody`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportJsonBody.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportJsonData`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportJsonData.md) | Class | 0 | 6 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayout`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportLayout.md) | Class | 19 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayoutColumn`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportLayoutColumn.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayoutElement`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportLayoutElement.md) | Class | 12 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayoutRow`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportLayoutRow.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportObservable`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportObservable.md) | Class | 0 | 0 | 1 |
| [`TheOne.Avalonia.Reporting.ReportPage`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPage.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPaginator`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPaginator.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPdfExporter`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPdfExporter.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPrintOptions`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPrintOptions.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPrintPlatform`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPrintPlatform.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPrintResult`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPrintResult.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportResources`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportResources.md) | Class | 0 | 4 | 0 |
| [`TheOne.Avalonia.Reporting.ReportTemplate`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportTemplate.md) | Class | 0 | 3 | 0 |
| [`TheOne.Avalonia.Reporting.ReportTextAlignment`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportTextAlignment.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportTextListStyle`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportTextListStyle.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportValueFormat`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportValueFormat.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportValueKind`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportValueKind.md) | Enum | 3 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
